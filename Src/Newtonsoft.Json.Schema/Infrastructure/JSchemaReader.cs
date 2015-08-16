#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#if !(NET20 || NET35 || PORTABLE || PORTABLE40)
using System.Numerics;
#endif
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal enum SchemaVersion
    {
        Unknown,
        Draft3,
        Draft4
    }

    internal class JSchemaReader
    {
        private static readonly ThreadSafeStore<string, JSchema> SpecSchemaCache = new ThreadSafeStore<string, JSchema>(LoadResourceSchema);

        internal JSchemaDiscovery _schemaDiscovery;
        internal readonly Stack<JSchema> _schemaStack;
        private readonly IList<DeferedSchema> _deferedSchemas;
        private readonly JSchemaResolver _resolver;
        private readonly Uri _baseUri;
        private readonly bool _validateSchema;
        private readonly SchemaValidationEventHandler _validationEventHandler;
        private readonly List<ValidationError> _validationErrors;

        private Uri _schemaVersionUri;
        private SchemaVersion _schemaVersion;
        private JSchema _validatingSchema;

        public JSchema RootSchema { get; set; }
        public Dictionary<Uri, JSchema> Cache { get; set; }

        public JSchemaReader(JSchemaResolver resolver)
            : this(new JSchemaReaderSettings { Resolver = resolver })
        {
        }

        public JSchemaReader(JSchemaReaderSettings settings)
        {
            Cache = new Dictionary<Uri, JSchema>(UriComparer.Instance);
            _deferedSchemas = new List<DeferedSchema>();
            _schemaStack = new Stack<JSchema>();

            _resolver = settings.Resolver ?? JSchemaDummyResolver.Instance;
            _baseUri = settings.BaseUri;
            _validateSchema = settings.ValidateVersion;
            _schemaDiscovery = new JSchemaDiscovery();
            _validationEventHandler = settings.GetValidationEventHandler();

            if (_validationEventHandler != null)
            {
                _validationErrors = new List<ValidationError>();
                _schemaDiscovery.ValidationErrors = _validationErrors;
            }
        }

        internal JSchema ReadRoot(JsonReader reader, bool resolveDeferedSchemas = true)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error reading schema from JsonReader.");
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading schema. Expected StartObject, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

            RootSchema = new JSchema();

            LoadAndSetSchema(reader, s =>
            {
                RootSchema = s;

                if (s.BaseUri != null)
                    Cache[s.BaseUri] = s;
            }, true);

            if (resolveDeferedSchemas)
                ResolveDeferedSchemas();

            if (_validationErrors != null)
            {
                foreach (ValidationError error in _validationErrors)
                {
                    KnownSchema knownSchema = _schemaDiscovery.KnownSchemas.SingleOrDefault(s => s.Schema == error.Schema);
                    if (knownSchema != null)
                        error.SchemaId = knownSchema.Id;

                    _validationEventHandler(this, new SchemaValidationEventArgs(error));
                }
            }

            return RootSchema;
        }

        private bool EnsureVersion(SchemaVersion minimum)
        {
            return EnsureVersion(minimum, null);
        }

        private bool EnsureVersion(SchemaVersion minimum, SchemaVersion? maximum)
        {
            if (_schemaVersion == SchemaVersion.Unknown)
                return true;

            if (_schemaVersion >= minimum)
            {
                if (_schemaVersion <= maximum || maximum == null)
                    return true;
            }

            return false;
        }

        internal JSchema ReadInlineSchema(Action<JSchema> setSchema, JToken inlineToken)
        {
            JTokenPathAnnotation pathAnnotation = inlineToken.Root.Annotation<JTokenPathAnnotation>();
            string path;
            if (pathAnnotation != null && !string.IsNullOrEmpty(pathAnnotation.BasePath))
            {
                path = pathAnnotation.BasePath;

                if (!inlineToken.Path.StartsWith("[", StringComparison.Ordinal))
                    path += ".";

                path += inlineToken.Path;
            }
            else
            {
                path = inlineToken.Path;
            }

            JsonReader reader = new JTokenReader(inlineToken, path);

            if (_validatingSchema != null)
            {
                JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
                validatingReader.Schema = _validatingSchema;
                validatingReader.ValidationEventHandler += RaiseSchemaValidationError;

                reader = validatingReader;
            }

            reader.Read();

            LoadAndSetSchema(reader, s =>
            {
                setSchema(s);
                inlineToken.RemoveAnnotations<JSchemaAnnotation>();
                inlineToken.AddAnnotation(new JSchemaAnnotation(s));
            });

            return inlineToken.Annotation<JSchemaAnnotation>().Schema;
        }

        internal void AddDeferedSchema(Action<JSchema> setSchema, JSchema referenceSchema)
        {
            Uri resolvedReference = ResolveSchemaReference(referenceSchema);

            AddDeferedSchema(resolvedReference, referenceSchema, setSchema);
        }

        private void ReadSchemaProperties(JsonReader reader, bool isRoot, JSchema schema)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        ProcessSchemaName(ref reader, isRoot, schema, (string)reader.Value);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading schema: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
        }

        public void ResolveDeferedSchemas()
        {
            if (_deferedSchemas.Count > 0)
            {
                List<DeferedSchema> resolvedDeferedSchemas = new List<DeferedSchema>();

                int initialCount = _deferedSchemas.Count;

                int i = 0;
                // note that defered schemas could be added while resolving
                while (_deferedSchemas.Count > 0)
                {
                    DeferedSchema deferedSchema = _deferedSchemas[0];

                    ResolveDeferedSchema(deferedSchema);

                    resolvedDeferedSchemas.Add(deferedSchema);
                    _deferedSchemas.Remove(deferedSchema);

                    if (!deferedSchema.Success && i >= initialCount)
                    {
                        // if schema resolved to another reference and that reference has already not be resolved then fail
                        // probably a circular reference
                        for (int j = initialCount; j < resolvedDeferedSchemas.Count; j++)
                        {
                            DeferedSchema resolvedSchema = resolvedDeferedSchemas[j];
                            if (deferedSchema != resolvedSchema && deferedSchema.ResolvedReference.ToString() == resolvedSchema.ResolvedReference.ToString())
                                throw JSchemaReaderException.Create(resolvedSchema.ReferenceSchema, _baseUri, resolvedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferedSchema.ResolvedReference));
                        }
                    }
                    i++;
                }

                foreach (DeferedSchema resolvedDeferedSchema in resolvedDeferedSchemas)
                {
                    if (!resolvedDeferedSchema.Success)
                        throw JSchemaReaderException.Create(resolvedDeferedSchema.ReferenceSchema, _baseUri, resolvedDeferedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, resolvedDeferedSchema.ResolvedReference));
                }
            }
        }

        private void ResolveDeferedSchema(DeferedSchema deferedSchema)
        {
            Uri reference = deferedSchema.ResolvedReference;

            bool found = SchemaDiscovery.FindSchema(s =>
            {
                // additional json copied to referenced schema
                // kind of hacky
                if (deferedSchema.ReferenceSchema._extensionData != null)
                {
                    foreach (KeyValuePair<string, JToken> keyValuePair in deferedSchema.ReferenceSchema._extensionData.ToList())
                    {
                        s.ExtensionData[keyValuePair.Key] = keyValuePair.Value;
                    }
                }

                deferedSchema.SetResolvedSchema(s);
            }, RootSchema, RootSchema.Id, reference, this, ref _schemaDiscovery);

            if (found)
                return;

            JSchema resolvedSchema;
            try
            {
                resolvedSchema = ResolvedSchema(deferedSchema.ReferenceSchema.Reference, deferedSchema.ResolvedReference);
            }
            catch (Exception ex)
            {
                throw JSchemaReaderException.Create(deferedSchema.ReferenceSchema, _baseUri, deferedSchema.ReferenceSchema.Path, "Error when resolving schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferedSchema.ReferenceSchema.Reference), ex);
            }

            if (resolvedSchema != null)
            {
                deferedSchema.SetResolvedSchema(resolvedSchema);
                return;
            }

            throw JSchemaReaderException.Create(deferedSchema.ReferenceSchema, _baseUri, deferedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferedSchema.ResolvedReference));
        }

        private void ReadSchema(JsonReader reader, string name, Action<JSchema> setSchema)
        {
            EnsureToken(reader, name, JsonToken.StartObject);

            LoadAndSetSchema(reader, setSchema);
        }

        private void EnsureRead(JsonReader reader, string name)
        {
            // read to the next non-comment
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.Comment)
                    return;
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading value for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void EnsureToken(JsonReader reader, string name, JsonToken tokenType)
        {
            EnsureRead(reader, name);

            if (reader.TokenType != tokenType)
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.".FormatWith(CultureInfo.InvariantCulture, name, tokenType, reader.TokenType));
        }

        private void EnsureToken(JsonReader reader, string name, List<JsonToken> tokenTypes)
        {
            EnsureRead(reader, name);

            if (!tokenTypes.Contains(reader.TokenType))
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.".FormatWith(CultureInfo.InvariantCulture, name, StringHelpers.Join(", ", tokenTypes), reader.TokenType));
        }

        private Uri ReadUri(JsonReader reader, string name)
        {
            EnsureToken(reader, name, JsonToken.String);
            return new Uri((string)reader.Value, UriKind.RelativeOrAbsolute);
        }

        private string ReadString(JsonReader reader, string name)
        {
            EnsureToken(reader, name, JsonToken.String);
            return (string)reader.Value;
        }

        private bool ReadBoolean(JsonReader reader, string name)
        {
            EnsureToken(reader, name, JsonToken.Boolean);
            return (bool)reader.Value;
        }

        private int ReadInteger(JsonReader reader, string name)
        {
            EnsureToken(reader, name, JsonToken.Integer);
            return Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);
        }

        private double ReadDouble(JsonReader reader, string name)
        {
            EnsureToken(reader, name, Constants.NumberTokens);

#if !(NET20 || NET35 || PORTABLE || PORTABLE40)
            if (reader.Value is BigInteger)
            {
                BigInteger i = (BigInteger)reader.Value;
                return (double)i;
            }
#endif

            return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
        }

        private Dictionary<string, JSchema> ReadProperties(JsonReader reader)
        {
            EnsureToken(reader, Constants.PropertyNames.Properties, JsonToken.StartObject);

            Dictionary<string, JSchema> properties = new Dictionary<string, JSchema>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string)reader.Value;

                        EnsureToken(reader, name, JsonToken.StartObject);

                        // use last schema for duplicates
                        // will this cause issues with a previously deferred schemas?
                        LoadAndSetSchema(reader, s => properties[name] = s);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return properties;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading properties: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema properties.");
        }

        private static void SetAtIndex<T>(List<T> list, int index, T value)
        {
            if (index == list.Count)
                list.Add(value);
            else if (index < list.Count)
                list[index] = value;
            else
                throw new InvalidOperationException("Could not add value to list. Index is greater than the list length.");
        }

        private void EnsureList<T>(ref List<T> value)
        {
            if (value == null)
                value = new List<T>();
        }

        private void ReadItems(JsonReader reader, JSchema schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Items);
            EnsureList(ref schema._items);

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    LoadAndSetSchema(reader, s => SetAtIndex(schema._items, 0, s));
                    schema.ItemsPositionValidation = false;
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, Constants.PropertyNames.Items, schema._items);
                    schema.ItemsPositionValidation = true;
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected array or JSON schema object for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Items, reader.TokenType));
            }
        }

        private void ReadSchemaArray(JsonReader reader, string name, out List<JSchema> schemas)
        {
            EnsureToken(reader, name, JsonToken.StartArray);

            schemas = new List<JSchema>();

            PopulateSchemaArray(reader, name, schemas);
        }

        private void ReadAdditionalItems(JsonReader reader, JSchema schema)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalItems);

            if (reader.TokenType == JsonToken.Boolean)
                schema.AllowAdditionalItems = (bool)reader.Value;
            else
                LoadAndSetSchema(reader, s => schema.AdditionalItems = s);
        }

        private void ReadAdditionalProperties(JsonReader reader, JSchema schema)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalProperties);

            if (reader.TokenType == JsonToken.Boolean)
                schema.AllowAdditionalProperties = (bool)reader.Value;
            else
                LoadAndSetSchema(reader, s => schema.AdditionalProperties = s);
        }

        private void ReadRequired(JsonReader reader, JSchema schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Required);

            if (reader.TokenType == JsonToken.Boolean)
            {
                if (!EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected StartArray, got Boolean.");

                schema.DeprecatedRequired = (bool)reader.Value;
            }
            else
            {
                if (!EnsureVersion(SchemaVersion.Draft4))
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected Boolean, got StartArray.");

                ReadStringArray(reader, Constants.PropertyNames.Required, out schema._required);
            }
        }

        private void ReadExtends(JsonReader reader, JSchema schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Extends);
            EnsureList(ref schema._allOf);

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    int index = schema._allOf.Count;
                    LoadAndSetSchema(reader, s => SetAtIndex(schema._allOf, index, s));
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, Constants.PropertyNames.Extends, schema._allOf);
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Extends, reader.TokenType));
            }
        }

        private void ReadTokenArray(JsonReader reader, string name, ref List<JToken> values)
        {
            EnsureToken(reader, name, JsonToken.StartArray);

            if (values == null)
                values = new List<JToken>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        JToken t = JToken.ReadFrom(reader);

                        values.Add(t);
                        break;
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadStringArray(JsonReader reader, string name, out List<string> values)
        {
            values = new List<string>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                        values.Add((string)reader.Value);
                        break;
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, name, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadDependencies(JsonReader reader, JSchema schema)
        {
            EnsureToken(reader, Constants.PropertyNames.Dependencies, JsonToken.StartObject);

            Dictionary<string, object> dependencies = new Dictionary<string, object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string)reader.Value;

                        EnsureToken(reader, name, Constants.DependencyTokens);

                        // use last dependency when duplicates are defined
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            LoadAndSetSchema(reader, s => dependencies[name] = s);
                        }
                        else if (reader.TokenType == JsonToken.StartArray)
                        {
                            List<string> l;
                            ReadStringArray(reader, name, out l);
                            dependencies[name] = l;
                        }
                        else
                        {
                            EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3);
                            
                            dependencies[name] = new List<string>
                            {
                                (string)reader.Value
                            };
                        }
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        schema._dependencies = dependencies;
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading dependencies: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading dependencies.");
        }

        private void PopulateSchemaArray(JsonReader reader, string name, List<JSchema> schemas)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        int itemCount = schemas.Count;
                        LoadAndSetSchema(reader, s => SetAtIndex(schemas, itemCount, s));
                        break;
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading schemas: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schemas for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        internal JSchemaType? MapType(JsonReader reader)
        {
            string typeName = (string)reader.Value;

            JSchemaType mappedType;
            if (!Constants.JSchemaTypeMapping.TryGetValue(typeName, out mappedType))
            {
                if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                {
                    if (typeName == Constants.Types.Any)
                        return null;
                }

                throw JSchemaReaderException.Create(reader, _baseUri, "Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, typeName));
            }

            return mappedType;
        }

        private object ReadType(JsonReader reader, string name)
        {
            EnsureRead(reader, name);

            List<JSchemaType> types = new List<JSchemaType>();
            List<JSchema> typeSchemas = null;
            bool isAny = false;

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return MapType(reader);
                case JsonToken.StartArray:
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.String:
                                JSchemaType? t = MapType(reader);

                                if (t == null)
                                {
                                    isAny = true;
                                }
                                else
                                {
                                    if (typeSchemas != null)
                                        typeSchemas.Add(new JSchema { Type = t });
                                    else
                                        types.Add(t.Value);
                                }
                                break;
                            case JsonToken.Comment:
                                // nom nom nom
                                break;
                            case JsonToken.EndArray:
                                // type of "any" removes all other type constraints
                                if (isAny)
                                    return null;

                                if (typeSchemas != null)
                                    return typeSchemas;

                                JSchemaType finalType = JSchemaType.None;
                                foreach (JSchemaType type in types)
                                {
                                    finalType = finalType | type;
                                }

                                return finalType;
                            default:
                                if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                                {
                                    if (typeSchemas == null)
                                    {
                                        typeSchemas = new List<JSchema>();
                                        foreach (JSchemaType type in types)
                                        {
                                            typeSchemas.Add(new JSchema
                                            {
                                                Type = type
                                            });
                                        }
                                        types = null;
                                    }
                                    int count = typeSchemas.Count;
                                    List<JSchema> l = typeSchemas;
                                    LoadAndSetSchema(reader, s => SetAtIndex(l, count, s));
                                }
                                else
                                {
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected string token for type, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected array or string for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Type, reader.TokenType));
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema type.");
        }

        private void LoadAndSetSchema(JsonReader reader, Action<JSchema> setSchema, bool isRoot = false)
        {
            JSchema schema = new JSchema();
            schema.BaseUri = _baseUri;

            IJsonLineInfo lineInfo = reader as IJsonLineInfo;
            if (lineInfo != null)
                schema.SetLineInfo(lineInfo);
            schema.Path = reader.Path;

            _schemaStack.Push(schema);

            ReadSchemaProperties(reader, isRoot, schema);

            if (schema.Reference != null)
            {
                Uri resolvedReference = ResolveSchemaReference(schema);

                AddDeferedSchema(resolvedReference, schema, setSchema);
            }

            setSchema(schema);

            _schemaStack.Pop();
        }

        private void AddDeferedSchema(Uri resolvedReference, JSchema referenceSchema, Action<JSchema> setSchema)
        {
            DeferedSchema deferedSchema = _deferedSchemas.SingleOrDefault(d => UriComparer.Instance.Equals(d.ResolvedReference, resolvedReference));

            if (deferedSchema == null)
            {
                deferedSchema = new DeferedSchema(resolvedReference, referenceSchema);
                _deferedSchemas.Add(deferedSchema);
            }

            deferedSchema.AddSchemaSet(setSchema);
        }

        private JSchema ResolvedSchema(Uri schemaId, Uri resolvedSchemaId)
        {
            ResolveSchemaContext context = new ResolveSchemaContext
            {
                ResolverBaseUri = _baseUri,
                SchemaId = schemaId,
                ResolvedSchemaId = resolvedSchemaId
            };

            SchemaReference schemaReference = _resolver.ResolveSchemaReference(context);

            if (schemaReference.BaseUri == _baseUri)
            {
                // reference is to inside the current schema
                // use normal schema resolution
                return null;
            }

            if (Cache.ContainsKey(schemaReference.BaseUri))
            {
                // base URI has already been resolved
                // use previously retrieved schema
                JSchema cachedSchema = Cache[schemaReference.BaseUri];
                return _resolver.GetSubschema(schemaReference, cachedSchema);
            }

            Stream schemaData = _resolver.GetSchemaResource(context, schemaReference);

            if (schemaData == null)
            {
                // resolver returned no data
                return null;
            }
            
            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                BaseUri = schemaReference.BaseUri,
                Resolver = _resolver
            };
            JSchemaReader schemaReader = new JSchemaReader(settings);

            schemaReader.Cache = Cache;

            JSchema rootSchema;
            using (StreamReader streamReader = new StreamReader(schemaData))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                rootSchema = schemaReader.ReadRoot(jsonReader, false);
            }

            Cache[schemaReference.BaseUri] = rootSchema;

            // resolve defered schemas after it has been cached to avoid
            // stackoverflow on circular references
            schemaReader.ResolveDeferedSchemas();

            return _resolver.GetSubschema(schemaReference, rootSchema);
        }

        private Uri ResolveSchemaReference(JSchema schema)
        {
            Uri resolvedReference = null;
            foreach (JSchema s in _schemaStack.Reverse())
            {
                Uri part = s.Id;

                if (part != null)
                {
                    if (resolvedReference == null)
                        resolvedReference = part;
                    else
                        resolvedReference = SchemaDiscovery.ResolveSchemaId(resolvedReference, part);
                }
            }

            resolvedReference = SchemaDiscovery.ResolveSchemaId(resolvedReference, schema.Reference);
            return resolvedReference;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private void ProcessSchemaName(ref JsonReader reader, bool isRoot, JSchema schema, string name)
        {
            switch (name)
            {
                case Constants.PropertyNames.Id:
                    schema.Id = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.Ref:
                    schema.Reference = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.Properties:
                    schema._properties = ReadProperties(reader);

                    // add schemas with deprecated required flag to new required array
                    foreach (KeyValuePair<string, JSchema> schemaProperty in schema.Properties)
                    {
                        if (schemaProperty.Value.DeprecatedRequired)
                        {
                            if (!schema.Required.Contains(schemaProperty.Key))
                                schema.Required.Add(schemaProperty.Key);
                        }
                    }
                    break;
                case Constants.PropertyNames.Items:
                    ReadItems(reader, schema);
                    break;
                case Constants.PropertyNames.Type:
                {
                    object typeResult = ReadType(reader, name);
                    if (typeResult is JSchemaType)
                        schema.Type = (JSchemaType)typeResult;
                    else
                        schema._anyOf = (List<JSchema>)typeResult;
                    break;
                }
                case Constants.PropertyNames.AnyOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        ReadSchemaArray(reader, name, out schema._anyOf);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.AllOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        ReadSchemaArray(reader, name, out schema._allOf);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.OneOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        ReadSchemaArray(reader, name, out schema._oneOf);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.Not:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        ReadSchema(reader, name, s => schema.Not = s);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.Title:
                    schema.Title = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Description:
                    schema.Description = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Format:
                    schema.Format = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.AdditionalProperties:
                    ReadAdditionalProperties(reader, schema);
                    break;
                case Constants.PropertyNames.AdditionalItems:
                    ReadAdditionalItems(reader, schema);
                    break;
                case Constants.PropertyNames.PatternProperties:
                    schema._patternProperties = ReadProperties(reader);
                    break;
                case Constants.PropertyNames.Required:
                    ReadRequired(reader, schema);
                    break;
                case Constants.PropertyNames.Dependencies:
                    ReadDependencies(reader, schema);
                    break;
                case Constants.PropertyNames.Minimum:
                    schema.Minimum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.Maximum:
                    schema.Maximum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.ExclusiveMinimum:
                    schema.ExclusiveMinimum = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.ExclusiveMaximum:
                    schema.ExclusiveMaximum = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.MaximumLength:
                    schema.MaximumLength = ReadInteger(reader, name);
                    break;
                case Constants.PropertyNames.MinimumLength:
                    schema.MinimumLength = ReadInteger(reader, name);
                    break;
                case Constants.PropertyNames.MaximumItems:
                    schema.MaximumItems = ReadInteger(reader, name);
                    break;
                case Constants.PropertyNames.MinimumItems:
                    schema.MinimumItems = ReadInteger(reader, name);
                    break;
                case Constants.PropertyNames.MaximumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        schema.MaximumProperties = ReadInteger(reader, name);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.MinimumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        schema.MinimumProperties = ReadInteger(reader, name);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.DivisibleBy:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                        schema.MultipleOf = ReadDouble(reader, name);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.MultipleOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                        schema.MultipleOf = ReadDouble(reader, name);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.Disallow:
                {
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        if (schema.Not == null)
                            schema.Not = new JSchema();

                        object disallowResult = ReadType(reader, name);
                        if (disallowResult is JSchemaType)
                        {
                            JSchemaType type = schema.Not.Type ?? JSchemaType.None;
                            schema.Not.Type = type | (JSchemaType)disallowResult;
                        }
                        else
                        {
                            schema.Not._anyOf = (List<JSchema>)disallowResult;
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, schema, name);
                    }
                    break;
                }
                case Constants.PropertyNames.Pattern:
                    schema.Pattern = ReadString(reader, name);

                    if (_validationErrors != null)
                    {
                        Regex patternRegex;
                        string errorMessage;
                        if (!schema.TryGetPatternRegex(out patternRegex, out errorMessage))
                        {
                            ValidationError error = ValidationError.CreateValidationError("Could not parse regex pattern '{0}'. Regex parser error: {1}".FormatWith(CultureInfo.InvariantCulture, schema.Pattern, errorMessage), ErrorType.Pattern, schema, null, schema.Pattern, null, schema, schema.Path);
                            _validationErrors.Add(error);
                        }                        
                    }
                    break;
                case Constants.PropertyNames.Enum:
                    ReadTokenArray(reader, name, ref schema._enum);
                    if (schema._enum.Count == 0)
                        throw JSchemaReaderException.Create(reader, _baseUri, "Enum array must have at least one value.");
                    break;
                case Constants.PropertyNames.Extends:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                        ReadExtends(reader, schema);
                    else
                        ReadExtensionData(reader, schema, name);
                    break;
                case Constants.PropertyNames.UniqueItems:
                    schema.UniqueItems = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.Default:
                    EnsureRead(reader, Constants.PropertyNames.Default);
                    schema.Default = JToken.ReadFrom(reader);
                    break;
                default:
                    if (isRoot && name == Constants.PropertyNames.Schema)
                    {
                        if (!reader.Read())
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");

                        if (reader.TokenType != JsonToken.String)
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for '$schema'. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

                        _schemaVersionUri = new Uri((string)reader.Value, UriKind.RelativeOrAbsolute);

#if !PORTABLE
                        string tempUriText = _schemaVersionUri.OriginalString.ToLower(CultureInfo.InvariantCulture);
#else
                        string tempUriText = _schemaVersionUri.OriginalString.ToLower();
#endif

                        Uri tempUri = new Uri(tempUriText, UriKind.RelativeOrAbsolute);
                        if (tempUri == Constants.SchemaVersions.Draft3)
                        {
                            _schemaVersion = SchemaVersion.Draft3;
                        }
                        else if (tempUri == Constants.SchemaVersions.Draft4)
                        {
                            _schemaVersion = SchemaVersion.Draft4;
                        }

                        if (_validateSchema)
                        {
                            if (_schemaVersion == SchemaVersion.Draft3)
                            {
                                _validatingSchema = SpecSchemaCache.Get("schema-draft-v3.json");
                            }
                            else if (_schemaVersion == SchemaVersion.Draft4)
                            {
                                _validatingSchema = SpecSchemaCache.Get("schema-draft-v4.json");
                            }
                            else
                            {
                                if (!_schemaVersionUri.IsAbsoluteUri)
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Schema version identifier '{0}' is not an absolute URI.".FormatWith(CultureInfo.InvariantCulture, _schemaVersionUri.OriginalString));

                                _validatingSchema = ResolvedSchema(_schemaVersionUri, _schemaVersionUri);

                                if (_validatingSchema == null)
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Could not resolve schema version identifier '{0}'.".FormatWith(CultureInfo.InvariantCulture, _schemaVersionUri.OriginalString));
                            }

                            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
                            validatingReader.Schema = _validatingSchema;
                            // push state that we're already inside an object
                            validatingReader.Validator.ValidateCurrentToken(JsonToken.StartObject, null, 0);
                            validatingReader.ValidationEventHandler += RaiseSchemaValidationError;

                            reader = validatingReader;
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, schema, name);
                    }
                    break;
            }
        }

        private void ReadExtensionData(JsonReader reader, JSchema schema, string name)
        {
            if (!reader.Read())
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");

            JToken t;
            if (reader is JTokenReader)
            {
                t = ((JTokenReader)reader).CurrentToken;
                reader.Skip();
            }
            else
            {
                string basePath = reader.Path;
                t = JToken.ReadFrom(reader);
                t.AddAnnotation(new JTokenPathAnnotation(basePath));
            }

            schema.ExtensionData[name] = t;
        }

        private void RaiseSchemaValidationError(object sender, SchemaValidationEventArgs e)
        {
            throw JSchemaReaderException.Create(
                (JsonReader)sender,
                _baseUri,
                "Validation error raised by version schema '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, _schemaVersionUri, e.ValidationError.Message),
                JSchemaValidationException.Create(e.ValidationError));
        }

        private static JSchema LoadResourceSchema(string name)
        {
            using (Stream schemaData = typeof(JSchemaReader).Assembly().GetManifestResourceStream("Newtonsoft.Json.Schema.Resources." + name))
            using (StreamReader sr = new StreamReader(schemaData))
            {
                return JSchema.Load(new JsonTextReader(sr));
            }
        }
    }
}