#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class JSchema4Reader
    {
        private readonly IList<DeferedSchema> _deferedSchemas;
        private readonly JSchema4Resolver _resolver;

        public JSchema4 RootSchema { get; set; }

        public JSchema4Reader(JSchema4Resolver resolver)
        {
            _resolver = resolver;
            _deferedSchemas = new List<DeferedSchema>();
        }

        internal JSchema4 ReadRoot(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                    throw JsonReaderException.Create(reader, "Error reading schema from JsonReader.");
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw JsonReaderException.Create(reader, "Unexpected token encountered when reading schema. Expected StartObject, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));

            RootSchema = new JSchema4();

            ReadSchema(reader, RootSchema);

            ResolveDeferedSchemas();

            return RootSchema;
        }

        internal JSchema4 ReadInlineSchema(JsonReader reader, JSchema4 referencedSchema)
        {
            JSchema4 schema = ReadSchema(reader, referencedSchema);

            ResolveDeferedSchemas();

            return schema;
        }

        private JSchema4 ReadSchema(JsonReader reader, JSchema4 schema)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        ProcessSchemaName(reader, schema, (string)reader.Value);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return schema;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected token when reading schema: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading schema.");
        }

        private void ResolveDeferedSchemas()
        {
            if (_deferedSchemas.Count > 0)
            {
                // note that defered schemas could be added while resolving
                while (_deferedSchemas.Count > 0)
                {
                    DeferedSchema deferedSchema = _deferedSchemas[0];

                    Uri reference = deferedSchema.Reference;

                    JSchema4 resolvedSchema = SchemaDiscovery.FindSchema(RootSchema, RootSchema.Id, reference, this);

                    if (resolvedSchema == null)
                        throw new JsonException("Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferedSchema.Reference));

                    deferedSchema.SetSchema(resolvedSchema);

                    _deferedSchemas.Remove(deferedSchema);
                }
            }
        }

        private void ReadSchema(JsonReader reader, string name, Action<JSchema4> setSchema)
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

            throw new JsonException("Unexpected end when reading value for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void EnsureToken(JsonReader reader, string name, JsonToken tokenType)
        {
            EnsureRead(reader, name);

            if (reader.TokenType != tokenType)
                throw new JsonException("Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.".FormatWith(CultureInfo.InvariantCulture, name, tokenType, reader.TokenType));
        }

        private void EnsureToken(JsonReader reader, string name, List<JsonToken> tokenTypes)
        {
            EnsureRead(reader, name);

            if (!tokenTypes.Contains(reader.TokenType))
                throw new JsonException("Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.".FormatWith(CultureInfo.InvariantCulture, name, string.Join(", ", tokenTypes), reader.TokenType));
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
            return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
        }

        private IDictionary<string, JSchema4> ReadProperties(JsonReader reader)
        {
            EnsureToken(reader, Constants.PropertyNames.Properties, JsonToken.StartObject);

            IDictionary<string, JSchema4> properties = new Dictionary<string, JSchema4>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string)reader.Value;

                        if (properties.ContainsKey(name))
                            throw new JsonException("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, name));

                        EnsureToken(reader, name, JsonToken.StartObject);

                        LoadAndSetSchema(reader, s => properties[name] = s);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return properties;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected token when reading properties: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading schema properties.");
        }

        private static void SetAtIndex<T>(IList<T> list, int index, T value)
        {
            if (index == list.Count)
                list.Add(value);
            else if (index < list.Count)
                list[index] = value;
            else
                throw new Exception("Could not add value to list. Index is greater than the list length.");
        }

        private void ReadItems(JsonReader reader, JSchema4 schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Items);

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    schema.Items = new List<JSchema4>();
                    LoadAndSetSchema(reader, s => SetAtIndex(schema.Items, 0, s));
                    schema.ItemsPositionValidation = false;
                    break;
                case JsonToken.StartArray:
                    schema.Items = new List<JSchema4>();
                    PopulateSchemaArray(reader, Constants.PropertyNames.Items, schema.Items);
                    schema.ItemsPositionValidation = true;
                    break;
                default:
                    throw JsonException.Create(reader as IJsonLineInfo, reader.Path, "Expected array or JSON schema object for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Items, reader.TokenType));
            }
        }

        private void ReadSchemaArray(JsonReader reader, string name, ref IList<JSchema4> schemas)
        {
            EnsureToken(reader, name, JsonToken.StartArray);

            schemas = new List<JSchema4>();

            PopulateSchemaArray(reader, name, schemas);
        }

        private void ReadAdditionalItems(JsonReader reader, JSchema4 schema)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalItems);

            if (reader.TokenType == JsonToken.Boolean)
                schema.AllowAdditionalItems = (bool)reader.Value;
            else
                LoadAndSetSchema(reader, s => schema.AdditionalItems = s);
        }

        private void ReadAdditionalProperties(JsonReader reader, JSchema4 schema)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalProperties);

            if (reader.TokenType == JsonToken.Boolean)
                schema.AllowAdditionalProperties = (bool)reader.Value;
            else
                LoadAndSetSchema(reader, s => schema.AdditionalProperties = s);
        }

        private void ReadRequired(JsonReader reader, JSchema4 schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Required);

            if (reader.TokenType == JsonToken.Boolean)
                schema.DeprecatedRequired = (bool)reader.Value;
            else
                ReadStringArray(reader, Constants.PropertyNames.Required, out schema._required);
        }

        private void ReadExtends(JsonReader reader, JSchema4 schema)
        {
            EnsureRead(reader, Constants.PropertyNames.Extends);

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    int index = schema.AllOf.Count;
                    LoadAndSetSchema(reader, s => SetAtIndex(schema.AllOf, index, s));
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, Constants.PropertyNames.Extends, schema.AllOf);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Extends, reader.TokenType));
            }
        }

        private void ReadTokenArray(JsonReader reader, string name, ref IList<JToken> values)
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

            throw new Exception("Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadStringArray(JsonReader reader, string name, out IList<string> values)
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
                        throw new ArgumentOutOfRangeException("Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, name, reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadDependencies(JsonReader reader, JSchema4 schema)
        {
            EnsureToken(reader, Constants.PropertyNames.Dependencies, JsonToken.StartObject);

            IDictionary<string, object> dependencies = new Dictionary<string, object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string)reader.Value;

                        if (dependencies.ContainsKey(name))
                            throw new JsonException("Dependency {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, name));

                        EnsureToken(reader, name, Constants.DependencyTokens);

                        if (reader.TokenType == JsonToken.StartObject)
                            LoadAndSetSchema(reader, s => dependencies[name] = s);
                        else
                            dependencies[name] = (string)reader.Value;
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        schema._dependencies = dependencies;
                        return;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected token when reading dependencies: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading dependencies.");
        }

        private void PopulateSchemaArray(JsonReader reader, string name, IList<JSchema4> schemas)
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
                        throw new ArgumentOutOfRangeException("Unexpected token when reading schemas: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading schemas for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        internal static JSchemaType MapType(string type)
        {
            JSchemaType mappedType;
            if (!Constants.JsonSchemaTypeMapping.TryGetValue(type, out mappedType))
                throw new JsonException("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));

            return mappedType;
        }

        private JSchemaType? ReadType(JsonReader reader)
        {
            EnsureRead(reader, Constants.PropertyNames.Type);

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return MapType((string)reader.Value);
                case JsonToken.StartArray:
                    // ensure type is in blank state before ORing values
                    JSchemaType? type = JSchemaType.None;

                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.String:
                                type = type | MapType((string)reader.Value);
                                break;
                            case JsonToken.Comment:
                                // nom nom nom
                                break;
                            case JsonToken.EndArray:
                                return type;
                            default:
                                throw JsonException.Create(reader as IJsonLineInfo, reader.Path, "Expected string token for type, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }
                    }
                    break;
                default:
                    throw JsonException.Create(null, reader.Path, "Expected array or string for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Type, reader.TokenType));
            }

            throw new Exception("Unexpected end when reading schema type.");
        }

        private void LoadAndSetSchema(JsonReader reader, Action<JSchema4> setSchema)
        {
            JSchema4 schema = new JSchema4();

            ReadSchema(reader, schema);

            Uri reference = schema.Reference;

            if (reference != null)
            {
                JSchema4 resolvedSchema = _resolver.GetSchema(reference);
                if (resolvedSchema != null)
                {
                    schema = resolvedSchema;
                }
                else
                {
                    _deferedSchemas.Add(new DeferedSchema(reference, setSchema));
                }
            }

            setSchema(schema);
        }

        private void ProcessSchemaName(JsonReader reader, JSchema4 schema, string name)
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
                    schema.Properties = ReadProperties(reader);
                    break;
                case Constants.PropertyNames.Items:
                    ReadItems(reader, schema);
                    break;
                case Constants.PropertyNames.Type:
                    schema.Type = ReadType(reader);
                    break;
                case Constants.PropertyNames.AnyOf:
                    ReadSchemaArray(reader, name, ref schema._anyOf);
                    break;
                case Constants.PropertyNames.AllOf:
                    ReadSchemaArray(reader, name, ref schema._allOf);
                    break;
                case Constants.PropertyNames.OneOf:
                    ReadSchemaArray(reader, name, ref schema._oneOf);
                    break;
                case Constants.PropertyNames.Not:
                    ReadSchema(reader, name, s => schema.Not = s);
                    break;
                case Constants.PropertyNames.Title:
                    schema.Title = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Description:
                    schema.Description = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.AdditionalProperties:
                    ReadAdditionalProperties(reader, schema);
                    break;
                case Constants.PropertyNames.AdditionalItems:
                    ReadAdditionalItems(reader, schema);
                    break;
                case Constants.PropertyNames.PatternProperties:
                    schema.PatternProperties = ReadProperties(reader);
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
                case Constants.PropertyNames.DivisibleBy:
                case Constants.PropertyNames.MultipleOf:
                    schema.MultipleOf = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.Disallow:
                    JSchemaType? disallow = ReadType(reader);
                    
                    if (schema.Not == null)
                        schema.Not = new JSchema4();

                    JSchemaType type = schema.Not.Type ?? JSchemaType.None;
                    schema.Not.Type = type | disallow;
                    break;
                case Constants.PropertyNames.Pattern:
                    schema.Pattern = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Enum:
                    ReadTokenArray(reader, name, ref schema._enum);
                    break;
                case Constants.PropertyNames.Extends:
                    ReadExtends(reader, schema);
                    break;
                case Constants.PropertyNames.UniqueItems:
                    schema.UniqueItems = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.Default:
                    EnsureRead(reader, Constants.PropertyNames.Default);
                    schema.Default = JToken.ReadFrom(reader);
                    break;
                default:
                    if (!reader.Read())
                        throw new Exception("Unexpected end when reading schema.");

                    JToken t;
                    if (reader is JTokenReader)
                        t = ((JTokenReader)reader).CurrentToken;
                    else
                        t = JToken.ReadFrom(reader);

                    schema.ExtensionData[name] = t;
                    break;
            }
        }
    }
}