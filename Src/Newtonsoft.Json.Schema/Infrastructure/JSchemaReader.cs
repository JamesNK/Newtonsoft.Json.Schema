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
#if !(NET20 || NET35 || PORTABLE || PORTABLE40) || NETSTANDARD1_3
using System.Numerics;
#endif
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Collections;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaReader
    {
        internal JSchemaDiscovery _schemaDiscovery;
        internal readonly List<IIdentiferScope> _identiferScopeStack;
        private readonly DeferedSchemaCollection _deferedSchemas;
        private readonly DeferedSchemaCollection _resolvedDeferedSchemas;
        private readonly JSchemaResolver _resolver;
        private readonly Uri _baseUri;
        private readonly bool _validateSchema;
        private readonly SchemaValidationEventHandler _validationEventHandler;
        private readonly List<ValidationError> _validationErrors;
        private readonly IList<JsonValidator> _validators;

        private Uri _versionUri;
        private SchemaVersion _version;
        private JSchema _validatingSchema;
        private bool _isReentrant;

        public JSchema RootSchema { get; set; }
        public Dictionary<Uri, JSchema> Cache { get; set; }

        public JSchemaReader(JSchemaResolver resolver)
            : this(new JSchemaReaderSettings { Resolver = resolver })
        {
        }

        public JSchemaReader(JSchemaReaderSettings settings)
        {
            Cache = new Dictionary<Uri, JSchema>(UriComparer.Instance);
            _deferedSchemas = new DeferedSchemaCollection();
            _resolvedDeferedSchemas = new DeferedSchemaCollection();
            _identiferScopeStack = new List<IIdentiferScope>();

            _resolver = settings.Resolver ?? JSchemaDummyResolver.Instance;
            _baseUri = settings.BaseUri;
            _validateSchema = settings.ValidateVersion;
            _schemaDiscovery = new JSchemaDiscovery();
            _validationEventHandler = settings.GetValidationEventHandler();
            _validators = settings.Validators;

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
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error reading schema from JsonReader.");
                }
            }

            ValidateSchemaStart(reader, null);

            RootSchema = new JSchema();

            LoadAndSetSchema(reader, null, s =>
            {
                RootSchema = s;

                if (s.BaseUri != null)
                {
                    Cache[s.BaseUri] = s;
                }
            }, true);

            if (resolveDeferedSchemas)
            {
                ResolveDeferedSchemas();
            }

            RaiseValidationErrors();

            return RootSchema;
        }

        public void RaiseValidationErrors()
        {
            if (_validationErrors != null)
            {
                _schemaDiscovery.Discover(RootSchema, null);

                foreach (ValidationError error in _validationErrors)
                {
                    KnownSchema knownSchema = _schemaDiscovery.KnownSchemas.SingleOrDefault(s => s.Schema == error.Schema);
                    if (knownSchema != null)
                    {
                        error.SchemaId = knownSchema.Id;
                    }

                    _validationEventHandler(this, new SchemaValidationEventArgs(error));
                }

                _validationErrors.Clear();
            }
        }

        internal bool EnsureVersion(SchemaVersion minimum, SchemaVersion? maximum = null)
        {
            return SchemaVersionHelpers.EnsureVersion(_version, minimum, maximum);
        }

        internal JSchema ReadInlineSchema(Action<JSchema> setSchema, JToken inlineToken)
        {
            JTokenPathAnnotation pathAnnotation = inlineToken.Root.Annotation<JTokenPathAnnotation>();
            string path;
            if (!string.IsNullOrEmpty(pathAnnotation?.BasePath))
            {
                path = pathAnnotation.BasePath;

                if (!string.IsNullOrEmpty(inlineToken.Path))
                {
                    if (!inlineToken.Path.StartsWith("[", StringComparison.Ordinal))
                    {
                        path += ".";
                    }

                    path += inlineToken.Path;
                }
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

            LoadAndSetSchema(reader, null, s =>
            {
                setSchema(s);
                inlineToken.RemoveAnnotations<JSchemaAnnotation>();
                inlineToken.AddAnnotation(new JSchemaAnnotation(s));
            });

            return inlineToken.Annotation<JSchemaAnnotation>().Schema;
        }

        internal bool AddDeferedSchema(JSchema target, Action<JSchema> setSchema, JSchema referenceSchema)
        {
            Uri resolvedReference = ResolveSchemaReference(referenceSchema);

            return AddDeferedSchema(resolvedReference, referenceSchema.Reference, referenceSchema, target, setSchema);
        }

        private void ReadSchemaProperties(JsonReader reader, JSchema target, bool isRoot)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        ProcessSchemaName(ref reader, target, isRoot, (string)reader.Value);
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
            if (_isReentrant)
            {
                return;
            }

            _isReentrant = true;

            if (_deferedSchemas.Count > 0)
            {
                // note that defered schemas could be added while resolving
                while (_deferedSchemas.Count > 0)
                {
                    DeferedSchema deferedSchema = _deferedSchemas[0];

                    ResolveDeferedSchema(deferedSchema);

                    if (_resolvedDeferedSchemas.TryGetValue(deferedSchema.ResolvedReference, out DeferedSchema previouslyResolvedSchema))
                    {
                        if (!deferedSchema.Success)
                        {
                            throw JSchemaReaderException.Create(previouslyResolvedSchema.ReferenceSchema, _baseUri, previouslyResolvedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferedSchema.ResolvedReference));
                        }
                        else
                        {
                            _resolvedDeferedSchemas.Remove(previouslyResolvedSchema);
                        }
                    }

                    _resolvedDeferedSchemas.Add(deferedSchema);
                    _deferedSchemas.Remove(deferedSchema);
                }

                foreach (DeferedSchema resolvedDeferedSchema in _resolvedDeferedSchemas)
                {
                    if (!resolvedDeferedSchema.Success)
                    {
                        throw JSchemaReaderException.Create(resolvedDeferedSchema.ReferenceSchema, _baseUri, resolvedDeferedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, resolvedDeferedSchema.ResolvedReference));
                    }
                }
            }

            _isReentrant = false;
        }

        private void ResolveDeferedSchema(DeferedSchema deferedSchema)
        {
            bool found = SchemaDiscovery.FindSchema(
                s =>
                {
                    // additional json copied to referenced schema
                    // kind of hacky
                    if (s != deferedSchema.ReferenceSchema && deferedSchema.ReferenceSchema._extensionData != null)
                    {
                        foreach (KeyValuePair<string, JToken> keyValuePair in deferedSchema.ReferenceSchema._extensionData)
                        {
                            s.ExtensionData[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }

                    deferedSchema.SetResolvedSchema(s);
                },
                RootSchema,
                RootSchema.Id,
                deferedSchema.ResolvedReference,
                deferedSchema.OriginalReference,
                this,
                ref _schemaDiscovery);

            if (found)
            {
                return;
            }

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

        private void ReadSchema(JsonReader reader, JSchema target, string name, Action<JSchema> setSchema)
        {
            ValidateSchemaStart(reader, name);

            LoadAndSetSchema(reader, target, setSchema);
        }

        private void ValidateSchemaStart(JsonReader reader, string name)
        {
            string errorMessage = (name == null)
                ? "Unexpected token encountered when reading schema. Expected {1}, got {2}."
                : null;

            if (EnsureVersion(SchemaVersion.Draft6))
            {
                if (name != null)
                {
                    EnsureRead(reader, name);
                }

                if (reader.TokenType != JsonToken.Boolean)
                {
                    EnsureToken(reader, name, Constants.SchemaTokens, errorMessage);
                }
            }
            else
            {
                EnsureReadAndToken(reader, name, JsonToken.StartObject, errorMessage);
            }
        }

        private void EnsureRead(JsonReader reader, string name)
        {
            // read to the next non-comment
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.Comment)
                {
                    return;
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading value for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void EnsureReadAndToken(JsonReader reader, string name, JsonToken tokenType, string errorMessage = null)
        {
            EnsureRead(reader, name);

            EnsureToken(reader, name, tokenType, errorMessage);
        }

        private void EnsureToken(JsonReader reader, string name, JsonToken tokenType, string errorMessage)
        {
            if (reader.TokenType != tokenType)
            {
                throw JSchemaReaderException.Create(reader, _baseUri, (errorMessage ?? "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.").FormatWith(CultureInfo.InvariantCulture, name, tokenType, reader.TokenType));
            }
        }

        private void EnsureToken(JsonReader reader, string name, List<JsonToken> tokenTypes, string errorMessage)
        {
            if (!tokenTypes.Contains(reader.TokenType))
            {
                throw JSchemaReaderException.Create(reader, _baseUri, (errorMessage ?? "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.").FormatWith(CultureInfo.InvariantCulture, name, StringHelpers.Join(", ", tokenTypes), reader.TokenType));
            }
        }

        private void EnsureReadAndToken(JsonReader reader, string name, List<JsonToken> tokenTypes, string errorMessage = null)
        {
            EnsureRead(reader, name);

            EnsureToken(reader, name, tokenTypes, errorMessage);
        }

        private Uri ReadUri(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.String);
            string id = (string) reader.Value;
            return ParseUri(reader, id);
        }

        private Uri ParseUri(JsonReader reader, string id)
        {
            try
            {
                return new Uri(id, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing id '{0}'. Id must be a valid URI.".FormatWith(CultureInfo.InvariantCulture, id), ex);
            }
        }

        private string ReadString(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.String);
            return (string) reader.Value;
        }

        private bool ReadBoolean(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.Boolean);
            return (bool) reader.Value;
        }

        private long ReadLong(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.Integer);

#if !(NET20 || NET35 || PORTABLE || PORTABLE40) || NETSTANDARD1_3
            if (reader.Value is BigInteger)
            {
                BigInteger i = (BigInteger) reader.Value;

                if (i > long.MaxValue || i < long.MaxValue)
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing integer for '{0}'. {1} cannot fit in an Int64.".FormatWith(CultureInfo.InvariantCulture, name, i));
                }

                return (long) i;
            }
#endif

            return Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture);
        }

        private double ReadDouble(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, Constants.NumberTokens);

            return GetDouble(reader);
        }

        private static double GetDouble(JsonReader reader)
        {
#if !(NET20 || NET35 || PORTABLE || PORTABLE40) || NETSTANDARD1_3
            if (reader.Value is BigInteger)
            {
                BigInteger i = (BigInteger) reader.Value;
                return (double) i;
            }
#endif

            return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
        }

        private void ReadProperties(JsonReader reader, JSchema target, IDictionary<string, JSchema> properties)
        {
            EnsureReadAndToken(reader, Constants.PropertyNames.Properties, JsonToken.StartObject);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string) reader.Value;

                        if (EnsureVersion(SchemaVersion.Draft6))
                        {
                            EnsureReadAndToken(reader, name, Constants.SchemaTokens);
                        }
                        else
                        {
                            EnsureReadAndToken(reader, name, JsonToken.StartObject);
                        }

                        // use last schema for duplicates
                        // will this cause issues with a previously deferred schemas?
                        LoadAndSetSchema(reader, target, s => properties[name] = s);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading properties: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema properties.");
        }

        private static void SetAtIndex<T>(IList<T> list, int index, T value)
        {
            if (index == list.Count)
            {
                list.Add(value);
            }
            else if (index < list.Count)
            {
                list[index] = value;
            }
            else
            {
                throw new InvalidOperationException("Could not add value to list. Index is greater than the list length.");
            }
        }

        private void ReadItems(JsonReader reader, JSchema target)
        {
            if (EnsureVersion(SchemaVersion.Draft6))
            {
                EnsureReadAndToken(reader, Constants.PropertyNames.Items, Constants.ItemsTokens);
            }
            else
            {
                EnsureReadAndToken(reader, Constants.PropertyNames.Items, Constants.ItemsDraft4Tokens);
            }

            if (target._items == null)
            {
                target._items = new JSchemaCollection(target);
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                case JsonToken.Boolean:
                    LoadAndSetSchema(reader, target, s => SetAtIndex(target._items, 0, s));
                    target.ItemsPositionValidation = false;
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, target, Constants.PropertyNames.Items, target._items);
                    target.ItemsPositionValidation = true;
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected array or JSON schema object for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Items, reader.TokenType));
            }
        }

        private void ReadSchemaArray(JsonReader reader, JSchema target, string name, out JSchemaCollection schemas)
        {
            EnsureReadAndToken(reader, name, JsonToken.StartArray);

            schemas = new JSchemaCollection(target);

            PopulateSchemaArray(reader, target, name, schemas);
        }

        private void ReadAdditionalItems(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalItems);

            if (reader.TokenType == JsonToken.Boolean)
            {
                target.AllowAdditionalItems = (bool) reader.Value;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.AdditionalItems = s);
            }
        }

        private void ReadAdditionalProperties(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalProperties);

            if (reader.TokenType == JsonToken.Boolean)
            {
                target.AllowAdditionalProperties = (bool) reader.Value;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.AdditionalProperties = s);
            }
        }

        private void ReadRequired(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.Required);

            if (reader.TokenType == JsonToken.Boolean)
            {
                if (!EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected StartArray, got Boolean.");
                }

                target.DeprecatedRequired = (bool) reader.Value;
            }
            else
            {
                if (!EnsureVersion(SchemaVersion.Draft4))
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected Boolean, got StartArray.");
                }

                ReadStringArray(reader, Constants.PropertyNames.Required, out target._required);
            }
        }

        private void ReadExtends(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.Extends);
            if (target._allOf == null)
            {
                target._allOf = new JSchemaCollection(target);
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    int index = target._allOf.Count;
                    LoadAndSetSchema(reader, target, s => SetAtIndex(target._allOf, index, s));
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, target, Constants.PropertyNames.Extends, target._allOf);
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Extends, reader.TokenType));
            }
        }

        private void ReadTokenArray(JsonReader reader, string name, ref List<JToken> values)
        {
            EnsureReadAndToken(reader, name, JsonToken.StartArray);

            if (values == null)
            {
                values = new List<JToken>();
            }

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
                        values.Add((string) reader.Value);
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

        private void ReadDependencies(JsonReader reader, JSchema target)
        {
            EnsureReadAndToken(reader, Constants.PropertyNames.Dependencies, JsonToken.StartObject);

            JSchemaDependencyDictionary dependencies = new JSchemaDependencyDictionary(target);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string) reader.Value;

                        List<JsonToken> validDependencyTokens = EnsureVersion(SchemaVersion.Draft6)
                            ? Constants.DependencyTokens
                            : Constants.DependencyDraft4Tokens;
                        EnsureReadAndToken(reader, name, validDependencyTokens);

                        // use last dependency when duplicates are defined
                        if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Boolean)
                        {
                            LoadAndSetSchema(reader, target, s => dependencies[name] = s);
                        }
                        else if (reader.TokenType == JsonToken.StartArray)
                        {
                            ReadStringArray(reader, name, out List<string> l);
                            dependencies[name] = l;
                        }
                        else
                        {
                            EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3);

                            dependencies[name] = new List<string>
                            {
                                (string) reader.Value
                            };
                        }
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        target._dependencies = dependencies;
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading dependencies: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading dependencies.");
        }

        private void PopulateSchemaArray(JsonReader reader, JSchema target, string name, IList<JSchema> schemas)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.Boolean:
                        int itemCount = schemas.Count;
                        LoadAndSetSchema(reader, target, s => SetAtIndex(schemas, itemCount, s));
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
            string typeName = (string) reader.Value;

            if (!Constants.JSchemaTypeMapping.TryGetValue(typeName, out JSchemaType mappedType))
            {
                if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                {
                    if (typeName == Constants.Types.Any)
                    {
                        return null;
                    }
                }

                throw JSchemaReaderException.Create(reader, _baseUri, "Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, typeName));
            }

            return mappedType;
        }

        private object ReadType(JsonReader reader, JSchema target, string name)
        {
            EnsureRead(reader, name);

            List<JSchemaType> types = new List<JSchemaType>();
            JSchemaCollection typeSchemas = null;
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
                                    {
                                        typeSchemas.Add(new JSchema {Type = t});
                                    }
                                    else
                                    {
                                        types.Add(t.Value);
                                    }
                                }
                                break;
                            case JsonToken.Comment:
                                // nom nom nom
                                break;
                            case JsonToken.EndArray:
                                // type of "any" removes all other type constraints
                                if (isAny)
                                {
                                    return null;
                                }

                                if (typeSchemas != null)
                                {
                                    return typeSchemas;
                                }

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
                                        typeSchemas = new JSchemaCollection(target);
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
                                    JSchemaCollection l = typeSchemas;
                                    LoadAndSetSchema(reader, target, s => SetAtIndex(l, count, s));
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

        private void LoadAndSetSchema(JsonReader reader, JSchema target, Action<JSchema> setSchema, bool isRoot = false)
        {
            // check whether a schema for this token has already been loaded
            if (reader is JTokenReader tokenReader)
            {
                JSchemaAnnotation schemaAnnotication = tokenReader.CurrentToken.Annotation<JSchemaAnnotation>();
                if (schemaAnnotication != null)
                {
                    setSchema(schemaAnnotication.Schema);
                    tokenReader.Skip();
                    return;
                }
            }

            JSchema loadingSchema = new JSchema();
            loadingSchema.State = JSchemaState.Loading;
            loadingSchema.BaseUri = _baseUri;

            if (reader is IJsonLineInfo lineInfo)
            {
                loadingSchema.SetLineInfo(lineInfo);
            }
            loadingSchema.Path = reader.Path;

            _identiferScopeStack.Add(loadingSchema);

            try
            {
                if (reader.TokenType == JsonToken.Boolean)
                {
                    loadingSchema.Valid = (bool)reader.Value;
                }
                else
                {
                    ReadSchemaProperties(reader, loadingSchema, isRoot);
                }

                if (loadingSchema.Reference != null)
                {
                    Uri resolvedReference = ResolveSchemaReference(loadingSchema);

                    if (AddDeferedSchema(resolvedReference, loadingSchema.Reference, loadingSchema, target, setSchema))
                    {
                        return;
                    }
                }
                else
                {
                    if (_validators != null)
                    {
                        foreach (JsonValidator validator in _validators)
                        {
                            if (validator.CanValidate(loadingSchema))
                            {
                                loadingSchema.Validators.Add(validator);
                            }
                        }
                    }
                }

                loadingSchema.State = JSchemaState.Default;

                setSchema(loadingSchema);
            }
            finally
            {
                _identiferScopeStack.RemoveAt(_identiferScopeStack.Count - 1);
            }
        }

        private bool AddDeferedSchema(Uri resolvedReference, Uri originalReference, JSchema referenceSchema, JSchema target, Action<JSchema> setSchema)
        {
            if (_resolvedDeferedSchemas.TryGetValue(resolvedReference, out DeferedSchema deferedSchema) && deferedSchema.Success)
            {
                if (deferedSchema.Success)
                {
                    // schema has already been successfully resolved
                    setSchema(deferedSchema.ResolvedSchema);
                    return true;
                }
                else
                {
                    deferedSchema.AddSchemaSet(setSchema, target);
                    return false;
                }
            }
            else
            {
                if (!_deferedSchemas.TryGetValue(resolvedReference, out deferedSchema))
                {
                    deferedSchema = new DeferedSchema(resolvedReference, originalReference, referenceSchema);
                    _deferedSchemas.Add(deferedSchema);
                }

                deferedSchema.AddSchemaSet(setSchema, target);
                return false;
            }
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

            if (schemaReference.BaseUri == _baseUri && schemaReference.SubschemaId != null)
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
                Resolver = _resolver,
                ValidateVersion = _validateSchema
            };
            settings.SetValidationEventHandler(_validationEventHandler);
            JSchemaReader schemaReader = new JSchemaReader(settings);

            schemaReader.Cache = Cache;

            JSchema rootSchema;
            using (StreamReader streamReader = new StreamReader(schemaData))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                rootSchema = schemaReader.ReadRoot(jsonReader, false);
                rootSchema.InternalReader = schemaReader;
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
            for (int i = 0; i < _identiferScopeStack.Count; i++)
            {
                Uri part = _identiferScopeStack[i].Id;

                if (part != null)
                {
                    if (resolvedReference == null)
                    {
                        resolvedReference = part;
                    }
                    else
                    {
                        resolvedReference = SchemaDiscovery.ResolveSchemaId(resolvedReference, part);
                    }
                }
            }

            try
            {
                resolvedReference = SchemaDiscovery.ResolveSchemaId(resolvedReference, schema.Reference);
            }
            catch (Exception ex)
            {
                string message = "Error resolving schema reference '{0}' in the scope '{1}'. The resolved reference must be a valid URI.".FormatWith(CultureInfo.InvariantCulture, schema.Reference, resolvedReference);
                throw JSchemaReaderException.Create(schema, _baseUri, schema.Path, message, ex);
            }

            return resolvedReference;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private void ProcessSchemaName(ref JsonReader reader, JSchema target, bool isRoot, string name)
        {
            switch (name)
            {
                case Constants.PropertyNames.Id:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        target.Id = ReadUri(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.IdDraft4:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        target.Id = ReadUri(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Ref:
                    target.Reference = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.Properties:
                    target._properties = new JSchemaDictionary(target);
                    ReadProperties(reader, target, target._properties);

                    // add schemas with deprecated required flag to new required array
                    foreach (KeyValuePair<string, JSchema> schemaProperty in target._properties)
                    {
                        if (schemaProperty.Value.DeprecatedRequired)
                        {
                            if (!target.Required.Contains(schemaProperty.Key))
                            {
                                target.Required.Add(schemaProperty.Key);
                            }
                        }
                    }
                    break;
                case Constants.PropertyNames.Items:
                    ReadItems(reader, target);
                    break;
                case Constants.PropertyNames.Type:
                {
                    object typeResult = ReadType(reader, target, name);
                    if (typeResult is JSchemaType)
                    {
                        target.Type = (JSchemaType) typeResult;
                    }
                    else
                    {
                        target._anyOf = (JSchemaCollection) typeResult;
                    }
                    break;
                }
                case Constants.PropertyNames.AnyOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._anyOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.AllOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._allOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.OneOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._oneOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Not:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchema(reader, target, name, s => target.Not = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.PropertyNamesSchema:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        ReadSchema(reader, target, name, s => target.PropertyNames = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Contains:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        ReadSchema(reader, target, name, s => target.Contains = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Title:
                    target.Title = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Description:
                    target.Description = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Format:
                    target.Format = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.AdditionalProperties:
                    ReadAdditionalProperties(reader, target);
                    break;
                case Constants.PropertyNames.AdditionalItems:
                    ReadAdditionalItems(reader, target);
                    break;
                case Constants.PropertyNames.PatternProperties:
                    ReadProperties(reader, target, target.PatternProperties);

                    if (_validationErrors != null)
                    {
                        foreach (PatternSchema patternProperty in target.GetPatternSchemas())
                        {
                            if (!patternProperty.TryGetPatternRegex(
#if !(NET35 || NET40)
                                null,
#endif
                                out Regex _,
                                out string errorMessage))
                            {
                                ValidationError error = ValidationError.CreateValidationError($"Could not parse regex pattern '{patternProperty.Pattern}'. Regex parser error: {errorMessage}", ErrorType.PatternProperties, target, null, patternProperty.Pattern, null, target, target.Path);
                                _validationErrors.Add(error);
                            }
                        }
                    }
                    break;
                case Constants.PropertyNames.Required:
                    ReadRequired(reader, target);
                    break;
                case Constants.PropertyNames.Dependencies:
                    ReadDependencies(reader, target);
                    break;
                case Constants.PropertyNames.Minimum:
                    target.Minimum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.Maximum:
                    target.Maximum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.ExclusiveMinimum:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMinimum, Constants.MaximumMinimumTokens);
                    }
                    else
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMinimum, JsonToken.Boolean);
                    }

                    if (reader.TokenType == JsonToken.Boolean)
                    {
                        target.ExclusiveMinimum = (bool)reader.Value;
                    }
                    else
                    {
                        target.Minimum = GetDouble(reader);
                        target.ExclusiveMinimum = true;
                    }
                    break;
                case Constants.PropertyNames.ExclusiveMaximum:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMaximum, Constants.MaximumMinimumTokens);
                    }
                    else
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMaximum, JsonToken.Boolean);
                    }

                    if (reader.TokenType == JsonToken.Boolean)
                    {
                        target.ExclusiveMaximum = (bool)reader.Value;
                    }
                    else
                    {
                        target.Maximum = GetDouble(reader);
                        target.ExclusiveMaximum = true;
                    }
                    break;
                case Constants.PropertyNames.MaximumLength:
                    target.MaximumLength = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MinimumLength:
                    target.MinimumLength = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MaximumItems:
                    target.MaximumItems = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MinimumItems:
                    target.MinimumItems = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MaximumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        target.MaximumProperties = ReadLong(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.MinimumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        target.MinimumProperties = ReadLong(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.DivisibleBy:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        target.MultipleOf = ReadDouble(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.MultipleOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        try
                        {
                            target.MultipleOf = ReadDouble(reader, name);
                        }
                        catch (Exception ex)
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "multipleOf must be greater than zero.", ex);
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Disallow:
                {
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        if (target.Not == null)
                        {
                            target.Not = new JSchema();
                        }

                        object disallowResult = ReadType(reader, target, name);
                        if (disallowResult is JSchemaType)
                        {
                            JSchemaType type = target.Not.Type ?? JSchemaType.None;
                            target.Not.Type = type | (JSchemaType)disallowResult;
                        }
                        else
                        {
                            target.Not._anyOf = (JSchemaCollection)disallowResult;
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                }
                case Constants.PropertyNames.Pattern:
                    target.Pattern = ReadString(reader, name);

                    if (_validationErrors != null)
                    {
                        if (!target.TryGetPatternRegex(
#if !(NET35 || NET40)
                            null,
#endif
                            out Regex _,
                            out string errorMessage))
                        {
                            ValidationError error = ValidationError.CreateValidationError($"Could not parse regex pattern '{target.Pattern}'. Regex parser error: {errorMessage}", ErrorType.Pattern, target, null, target.Pattern, null, target, target.Path);
                            _validationErrors.Add(error);
                        }
                    }
                    break;
                case Constants.PropertyNames.Enum:
                    ReadTokenArray(reader, name, ref target._enum);
                    if (target._enum.Count == 0)
                    {
                        throw JSchemaReaderException.Create(reader, _baseUri, "Enum array must have at least one value.");
                    }
                    break;
                case Constants.PropertyNames.Const:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureRead(reader, Constants.PropertyNames.Const);
                        JToken t = JToken.ReadFrom(reader);

                        target.Const = t;
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Extends:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        ReadExtends(reader, target);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.UniqueItems:
                    target.UniqueItems = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.Default:
                    EnsureRead(reader, Constants.PropertyNames.Default);
                    target.Default = JToken.ReadFrom(reader);
                    break;
                default:
                    if (isRoot && name == Constants.PropertyNames.Schema)
                    {
                        if (!reader.Read())
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
                        }

                        if (reader.TokenType != JsonToken.String)
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for '$schema'. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }

                        target.SchemaVersion = ParseUri(reader, (string)reader.Value);

                        _versionUri = target.SchemaVersion;

                        _version = SchemaVersionHelpers.MapSchemaUri(_versionUri);

                        if (_validateSchema)
                        {
                            _validatingSchema = SchemaVersionHelpers.GetSchema(_version);

                            if (_validatingSchema == null)
                            {
                                if (!_versionUri.IsAbsoluteUri)
                                {
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Schema version identifier '{0}' is not an absolute URI.".FormatWith(CultureInfo.InvariantCulture, _versionUri.OriginalString));
                                }

                                _validatingSchema = ResolvedSchema(_versionUri, _versionUri);

                                if (_validatingSchema == null)
                                {
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Could not resolve schema version identifier '{0}'.".FormatWith(CultureInfo.InvariantCulture, _versionUri.OriginalString));
                                }
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
                        ReadExtensionData(reader, target, name);
                    }
                    break;
            }
        }

        private void ReadExtensionData(JsonReader reader, JSchema target, string name)
        {
            if (!reader.Read())
            {
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
            }

            JToken t;
            if (reader is JTokenReader tokenReader)
            {
                t = tokenReader.CurrentToken;
                tokenReader.Skip();
            }
            else
            {
                string basePath = reader.Path;
                t = JToken.ReadFrom(reader);
                t.AddAnnotation(new JTokenPathAnnotation(basePath));
            }

            target.ExtensionData[name] = t;
        }

        private void RaiseSchemaValidationError(object sender, SchemaValidationEventArgs e)
        {
            throw JSchemaReaderException.Create(
                (JsonReader)sender,
                _baseUri,
                "Validation error raised by version schema '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, _versionUri, e.ValidationError.Message),
                JSchemaValidationException.Create(e.ValidationError));
        }
    }
}