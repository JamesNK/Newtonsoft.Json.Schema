#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaWriter
    {
        private readonly JsonWriter _writer;
        private readonly JSchemaResolver _resolver;

        public JSchemaWriter(JsonWriter writer, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            _writer = writer;
            _resolver = resolver;
        }

        private void ReferenceOrWriteSchema(JSchema schema)
        {
            if (schema.Id != null && _resolver.GetSchema(schema.Id) != null)
            {
                _writer.WriteStartObject();
                _writer.WritePropertyName(JsonTypeReflector.RefPropertyName);
                _writer.WriteValue(schema.Id);
                _writer.WriteEndObject();
            }
            else
            {
                WriteSchema(schema);
            }
        }

        public void WriteSchema(JSchema schema)
        {
            ValidationUtils.ArgumentNotNull(schema, "schema");

            if (!_resolver.LoadedSchemas.Contains(schema))
                _resolver.LoadedSchemas.Add(schema);

            _writer.WriteStartObject();
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Id, schema.Id);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Title, schema.Title);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Description, schema.Description);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Required, schema.Required);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.ReadOnly, schema.ReadOnly);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Hidden, schema.Hidden);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Transient, schema.Transient);
            if (schema.Type != null)
                WriteType(Constants.PropertyNames.Type, _writer, schema.Type.Value);
            if (!schema.AllowAdditionalProperties)
            {
                _writer.WritePropertyName(Constants.PropertyNames.AdditionalProperties);
                _writer.WriteValue(schema.AllowAdditionalProperties);
            }
            else
            {
                if (schema.AdditionalProperties != null)
                {
                    _writer.WritePropertyName(Constants.PropertyNames.AdditionalProperties);
                    ReferenceOrWriteSchema(schema.AdditionalProperties);
                }
            }
            if (!schema.AllowAdditionalItems)
            {
                _writer.WritePropertyName(Constants.PropertyNames.AdditionalItems);
                _writer.WriteValue(schema.AllowAdditionalItems);
            }
            else
            {
                if (schema.AdditionalItems != null)
                {
                    _writer.WritePropertyName(Constants.PropertyNames.AdditionalItems);
                    ReferenceOrWriteSchema(schema.AdditionalItems);
                }
            }
            WriteSchemaDictionaryIfNotNull(_writer, Constants.PropertyNames.Properties, schema.Properties);
            WriteSchemaDictionaryIfNotNull(_writer, Constants.PropertyNames.PatternProperties, schema.PatternProperties);
            WriteItems(schema);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Minimum, schema.Minimum);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Maximum, schema.Maximum);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.ExclusiveMinimum, schema.ExclusiveMinimum);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.ExclusiveMaximum, schema.ExclusiveMaximum);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumLength, schema.MinimumLength);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumLength, schema.MaximumLength);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumItems, schema.MinimumItems);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumItems, schema.MaximumItems);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.DivisibleBy, schema.DivisibleBy);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Format, schema.Format);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Pattern, schema.Pattern);
            if (schema.Enum != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.Enum);
                _writer.WriteStartArray();
                foreach (JToken token in schema.Enum)
                {
                    token.WriteTo(_writer);
                }
                _writer.WriteEndArray();
            }
            if (schema.Default != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.Default);
                schema.Default.WriteTo(_writer);
            }
            if (schema.Disallow != null)
                WriteType(Constants.PropertyNames.Disallow, _writer, schema.Disallow.Value);
            if (schema.Extends != null && schema.Extends.Count > 0)
            {
                _writer.WritePropertyName(Constants.PropertyNames.Extends);
                if (schema.Extends.Count == 1)
                {
                    ReferenceOrWriteSchema(schema.Extends[0]);
                }
                else
                {
                    _writer.WriteStartArray();
                    foreach (JSchema jsonSchema in schema.Extends)
                    {
                        ReferenceOrWriteSchema(jsonSchema);
                    }
                    _writer.WriteEndArray();
                }
            }
            _writer.WriteEndObject();
        }

        private void WriteSchemaDictionaryIfNotNull(JsonWriter writer, string propertyName, IDictionary<string, JSchema> properties)
        {
            if (properties != null)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteStartObject();
                foreach (KeyValuePair<string, JSchema> property in properties)
                {
                    writer.WritePropertyName(property.Key);
                    ReferenceOrWriteSchema(property.Value);
                }
                writer.WriteEndObject();
            }
        }

        private void WriteItems(JSchema schema)
        {
            if (schema.Items == null && !schema.PositionalItemsValidation)
                return;

            _writer.WritePropertyName(Constants.PropertyNames.Items);

            if (!schema.PositionalItemsValidation)
            {
                if (schema.Items != null && schema.Items.Count > 0)
                {
                    ReferenceOrWriteSchema(schema.Items[0]);
                }
                else
                {
                    _writer.WriteStartObject();
                    _writer.WriteEndObject();
                }
                return;
            }

            _writer.WriteStartArray();
            if (schema.Items != null)
            {
                foreach (JSchema itemSchema in schema.Items)
                {
                    ReferenceOrWriteSchema(itemSchema);
                }
            }
            _writer.WriteEndArray();
        }

        private void WriteType(string propertyName, JsonWriter writer, JSchemaType type)
        {
            IList<JSchemaType> types;
            if (System.Enum.IsDefined(typeof(JSchemaType), type))
                types = new List<JSchemaType> { type };
            else
                types = EnumUtils.GetFlagsValues(type).Where(v => v != JSchemaType.None).ToList();

            if (types.Count == 0)
                return;

            writer.WritePropertyName(propertyName);

            if (types.Count == 1)
            {
                writer.WriteValue(JSchemaBuilder.MapType(types[0]));
                return;
            }

            writer.WriteStartArray();
            foreach (JSchemaType jsonSchemaType in types)
            {
                writer.WriteValue(JSchemaBuilder.MapType(jsonSchemaType));
            }
            writer.WriteEndArray();
        }

        private void WritePropertyIfNotNull(JsonWriter writer, string propertyName, object value)
        {
            if (value != null)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteValue(value);
            }
        }
    }
}