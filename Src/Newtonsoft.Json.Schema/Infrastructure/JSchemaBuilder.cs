#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaBuilder
    {
        private readonly IList<JSchema> _stack;
        private readonly JSchemaResolver _resolver;
        private readonly IDictionary<string, JSchema> _documentSchemas;
        private JSchema _currentSchema;
        private JObject _rootSchema;

        public JSchemaBuilder(JSchemaResolver resolver)
        {
            _stack = new List<JSchema>();
            _documentSchemas = new Dictionary<string, JSchema>();
            _resolver = resolver;
        }

        private void Push(JSchema value)
        {
            _currentSchema = value;
            _stack.Add(value);
            _resolver.LoadedSchemas.Add(value);
            _documentSchemas.Add(value.Location, value);
        }

        private JSchema Pop()
        {
            JSchema poppedSchema = _currentSchema;
            _stack.RemoveAt(_stack.Count - 1);
            _currentSchema = _stack.LastOrDefault();

            return poppedSchema;
        }

        private JSchema CurrentSchema
        {
            get { return _currentSchema; }
        }

        internal JSchema Read(JsonReader reader)
        {
            JToken schemaToken = JToken.ReadFrom(reader);

            _rootSchema = schemaToken as JObject;

            JSchema schema = BuildSchema(schemaToken);

            ResolveReferences(schema);

            return schema;
        }

        private string UnescapeReference(string reference)
        {
            return Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");
        }

        private JSchema ResolveReferences(JSchema schema)
        {
            if (schema.DeferredReference != null)
            {
                string reference = schema.DeferredReference;

                bool locationReference = (reference.StartsWith("#", StringComparison.Ordinal));
                if (locationReference)
                    reference = UnescapeReference(reference);

                JSchema resolvedSchema = _resolver.GetSchema(reference);

                if (resolvedSchema == null)
                {
                    if (locationReference)
                    {
                        string[] escapedParts = schema.DeferredReference.TrimStart('#').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        JToken currentToken = _rootSchema;
                        foreach (string escapedPart in escapedParts)
                        {
                            string part = UnescapeReference(escapedPart);

                            if (currentToken.Type == JTokenType.Object)
                            {
                                currentToken = currentToken[part];
                            }
                            else if (currentToken.Type == JTokenType.Array || currentToken.Type == JTokenType.Constructor)
                            {
                                int index;
                                if (int.TryParse(part, out index) && index >= 0 && index < currentToken.Count())
                                    currentToken = currentToken[index];
                                else
                                    currentToken = null;
                            }

                            if (currentToken == null)
                                break;
                        }

                        if (currentToken != null)
                            resolvedSchema = BuildSchema(currentToken);
                    }

                    if (resolvedSchema == null)
                        throw new JsonException("Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, schema.DeferredReference));
                }

                schema = resolvedSchema;
            }

            if (schema.ReferencesResolved)
                return schema;

            schema.ReferencesResolved = true;

            if (schema.Extends != null)
            {
                for (int i = 0; i < schema.Extends.Count; i++)
                {
                    schema.Extends[i] = ResolveReferences(schema.Extends[i]);
                }
            }

            if (schema.Items != null)
            {
                for (int i = 0; i < schema.Items.Count; i++)
                {
                    schema.Items[i] = ResolveReferences(schema.Items[i]);
                }
            }

            if (schema.AdditionalItems != null)
                schema.AdditionalItems = ResolveReferences(schema.AdditionalItems);

            if (schema.PatternProperties != null)
            {
                foreach (KeyValuePair<string, JSchema> patternProperty in schema.PatternProperties.ToList())
                {
                    schema.PatternProperties[patternProperty.Key] = ResolveReferences(patternProperty.Value);
                }
            }

            if (schema.Properties != null)
            {
                foreach (KeyValuePair<string, JSchema> property in schema.Properties.ToList())
                {
                    schema.Properties[property.Key] = ResolveReferences(property.Value);
                }
            }

            if (schema.AdditionalProperties != null)
                schema.AdditionalProperties = ResolveReferences(schema.AdditionalProperties);

            return schema;
        }

        private JSchema BuildSchema(JToken token)
        {
            JObject schemaObject = token as JObject;
            if (schemaObject == null)
                throw JsonException.Create(token, token.Path, "Expected object while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));

            JToken referenceToken;
            if (schemaObject.TryGetValue(JsonTypeReflector.RefPropertyName, out referenceToken))
            {
                JSchema deferredSchema = new JSchema();
                deferredSchema.DeferredReference = (string)referenceToken;

                return deferredSchema;
            }

            string location = token.Path.Replace(".", "/").Replace("[", "/").Replace("]", string.Empty);
            if (!string.IsNullOrEmpty(location))
                location = "/" + location;
            location = "#" + location;

            JSchema existingSchema;
            if (_documentSchemas.TryGetValue(location, out existingSchema))
                return existingSchema;

            Push(new JSchema { Location = location });

            ProcessSchemaProperties(schemaObject);

            return Pop();
        }

        private void ProcessSchemaProperties(JObject schemaObject)
        {
            foreach (KeyValuePair<string, JToken> property in schemaObject)
            {
                switch (property.Key)
                {
                    case JSchemaConstants.TypePropertyName:
                        CurrentSchema.Type = ProcessType(property.Value);
                        break;
                    case JSchemaConstants.IdPropertyName:
                        CurrentSchema.Id = (string)property.Value;
                        break;
                    case JSchemaConstants.TitlePropertyName:
                        CurrentSchema.Title = (string)property.Value;
                        break;
                    case JSchemaConstants.DescriptionPropertyName:
                        CurrentSchema.Description = (string)property.Value;
                        break;
                    case JSchemaConstants.PropertiesPropertyName:
                        CurrentSchema.Properties = ProcessProperties(property.Value);
                        break;
                    case JSchemaConstants.ItemsPropertyName:
                        ProcessItems(property.Value);
                        break;
                    case JSchemaConstants.AdditionalPropertiesPropertyName:
                        ProcessAdditionalProperties(property.Value);
                        break;
                    case JSchemaConstants.AdditionalItemsPropertyName:
                        ProcessAdditionalItems(property.Value);
                        break;
                    case JSchemaConstants.PatternPropertiesPropertyName:
                        CurrentSchema.PatternProperties = ProcessProperties(property.Value);
                        break;
                    case JSchemaConstants.RequiredPropertyName:
                        CurrentSchema.Required = (bool)property.Value;
                        break;
                    case JSchemaConstants.RequiresPropertyName:
                        CurrentSchema.Requires = (string)property.Value;
                        break;
                    case JSchemaConstants.MinimumPropertyName:
                        CurrentSchema.Minimum = (double)property.Value;
                        break;
                    case JSchemaConstants.MaximumPropertyName:
                        CurrentSchema.Maximum = (double)property.Value;
                        break;
                    case JSchemaConstants.ExclusiveMinimumPropertyName:
                        CurrentSchema.ExclusiveMinimum = (bool)property.Value;
                        break;
                    case JSchemaConstants.ExclusiveMaximumPropertyName:
                        CurrentSchema.ExclusiveMaximum = (bool)property.Value;
                        break;
                    case JSchemaConstants.MaximumLengthPropertyName:
                        CurrentSchema.MaximumLength = (int)property.Value;
                        break;
                    case JSchemaConstants.MinimumLengthPropertyName:
                        CurrentSchema.MinimumLength = (int)property.Value;
                        break;
                    case JSchemaConstants.MaximumItemsPropertyName:
                        CurrentSchema.MaximumItems = (int)property.Value;
                        break;
                    case JSchemaConstants.MinimumItemsPropertyName:
                        CurrentSchema.MinimumItems = (int)property.Value;
                        break;
                    case JSchemaConstants.DivisibleByPropertyName:
                        CurrentSchema.DivisibleBy = (double)property.Value;
                        break;
                    case JSchemaConstants.DisallowPropertyName:
                        CurrentSchema.Disallow = ProcessType(property.Value);
                        break;
                    case JSchemaConstants.DefaultPropertyName:
                        CurrentSchema.Default = property.Value.DeepClone();
                        break;
                    case JSchemaConstants.HiddenPropertyName:
                        CurrentSchema.Hidden = (bool)property.Value;
                        break;
                    case JSchemaConstants.ReadOnlyPropertyName:
                        CurrentSchema.ReadOnly = (bool)property.Value;
                        break;
                    case JSchemaConstants.FormatPropertyName:
                        CurrentSchema.Format = (string)property.Value;
                        break;
                    case JSchemaConstants.PatternPropertyName:
                        CurrentSchema.Pattern = (string)property.Value;
                        break;
                    case JSchemaConstants.EnumPropertyName:
                        ProcessEnum(property.Value);
                        break;
                    case JSchemaConstants.ExtendsPropertyName:
                        ProcessExtends(property.Value);
                        break;
                    case JSchemaConstants.UniqueItemsPropertyName:
                        CurrentSchema.UniqueItems = (bool)property.Value;
                        break;
                }
            }
        }

        private void ProcessExtends(JToken token)
        {
            IList<JSchema> schemas = new List<JSchema>();

            if (token.Type == JTokenType.Array)
            {
                foreach (JToken schemaObject in token)
                {
                    schemas.Add(BuildSchema(schemaObject));
                }
            }
            else
            {
                JSchema schema = BuildSchema(token);
                if (schema != null)
                    schemas.Add(schema);
            }

            if (schemas.Count > 0)
                CurrentSchema.Extends = schemas;
        }

        private void ProcessEnum(JToken token)
        {
            if (token.Type != JTokenType.Array)
                throw JsonException.Create(token, token.Path, "Expected Array token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));

            CurrentSchema.Enum = new List<JToken>();

            foreach (JToken enumValue in token)
            {
                CurrentSchema.Enum.Add(enumValue.DeepClone());
            }
        }

        private void ProcessAdditionalProperties(JToken token)
        {
            if (token.Type == JTokenType.Boolean)
                CurrentSchema.AllowAdditionalProperties = (bool)token;
            else
                CurrentSchema.AdditionalProperties = BuildSchema(token);
        }

        private void ProcessAdditionalItems(JToken token)
        {
            if (token.Type == JTokenType.Boolean)
                CurrentSchema.AllowAdditionalItems = (bool)token;
            else
                CurrentSchema.AdditionalItems = BuildSchema(token);
        }

        private IDictionary<string, JSchema> ProcessProperties(JToken token)
        {
            IDictionary<string, JSchema> properties = new Dictionary<string, JSchema>();

            if (token.Type != JTokenType.Object)
                throw JsonException.Create(token, token.Path, "Expected Object token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));

            foreach (JProperty propertyToken in token)
            {
                if (properties.ContainsKey(propertyToken.Name))
                    throw new JsonException("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, propertyToken.Name));

                properties.Add(propertyToken.Name, BuildSchema(propertyToken.Value));
            }

            return properties;
        }

        private void ProcessItems(JToken token)
        {
            CurrentSchema.Items = new List<JSchema>();

            switch (token.Type)
            {
                case JTokenType.Object:
                    CurrentSchema.Items.Add(BuildSchema(token));
                    CurrentSchema.PositionalItemsValidation = false;
                    break;
                case JTokenType.Array:
                    CurrentSchema.PositionalItemsValidation = true;
                    foreach (JToken schemaToken in token)
                    {
                        CurrentSchema.Items.Add(BuildSchema(schemaToken));
                    }
                    break;
                default:
                    throw JsonException.Create(token, token.Path, "Expected array or JSON schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
        }

        private JSchemaType? ProcessType(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    // ensure type is in blank state before ORing values
                    JSchemaType? type = JSchemaType.None;

                    foreach (JToken typeToken in token)
                    {
                        if (typeToken.Type != JTokenType.String)
                            throw JsonException.Create(typeToken, typeToken.Path, "Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));

                        type = type | MapType((string)typeToken);
                    }

                    return type;
                case JTokenType.String:
                    return MapType((string)token);
                default:
                    throw JsonException.Create(token, token.Path, "Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }
        }

        internal static JSchemaType MapType(string type)
        {
            JSchemaType mappedType;
            if (!JSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out mappedType))
                throw new JsonException("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));

            return mappedType;
        }

        internal static string MapType(JSchemaType type)
        {
            return JSchemaConstants.JsonSchemaTypeMapping.Single(kv => kv.Value == type).Key;
        }
    }
}
