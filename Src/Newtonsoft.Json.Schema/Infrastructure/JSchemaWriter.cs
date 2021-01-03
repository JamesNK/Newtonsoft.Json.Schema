#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Collections;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaWriter
    {
        private readonly JsonWriter _writer;
        private readonly KnownSchemaCollection _knownSchemas;
        private readonly List<JSchema>? _schemaStack;
        private readonly IList<ExternalSchema>? _externalSchemas;
        private readonly JSchemaWriterReferenceHandling _referenceHandling;
        private SchemaVersion _version;
        private JSchema? _rootSchema;

        public JSchemaWriter(JsonWriter writer)
            : this(writer, null)
        {
        }

        public JSchemaWriter(JsonWriter writer, JSchemaWriterSettings? settings)
        {
            ValidationUtils.ArgumentNotNull(writer, nameof(writer));

            _writer = writer;
            _knownSchemas = new KnownSchemaCollection();

            if (settings != null)
            {
                _externalSchemas = settings.ExternalSchemas;
                _referenceHandling = settings.ReferenceHandling;
                if (settings.Version != null)
                {
                    _version = settings.Version.Value;
                }
            }

            if (_referenceHandling != JSchemaWriterReferenceHandling.Always)
            {
                _schemaStack = new List<JSchema>();
            }
        }

        private void ReferenceOrWriteSchema(JSchema context, JSchema schema, string? propertyName, bool isDefinitions = false)
        {
            KnownSchema? knownSchema = _knownSchemas.GetByJSchema(schema);
            if (knownSchema == null)
            {
#if DEBUG
                _writer.WriteValue("UNKNOWN SCHEMA: " + schema.ResolvedId);
#endif
                return;
            }

            if (propertyName != null)
            {
                _writer.WritePropertyName(propertyName);
            }

            if (ShouldWriteReference(knownSchema, isDefinitions))
            {
                Uri reference = ResolveReference(context, knownSchema);

                WriteReferenceObject(reference);
                return;
            }

            knownSchema.State = KnownSchemaState.Written;

            WriteSchemaInternal(schema);
        }

        private void WriteReference(JSchema context, JSchema schema, string propertyName)
        {
            KnownSchema? knownSchema = _knownSchemas.GetByJSchema(schema);
            if (knownSchema == null)
            {
                return;
            }

            if (propertyName != null)
            {
                _writer.WritePropertyName(propertyName);
            }

            Uri reference = ResolveReference(context, knownSchema);
            _writer.WriteValue(reference);
        }

        private Uri ResolveReference(JSchema context, KnownSchema knownSchema)
        {
            KnownSchema currentKnownSchema = _knownSchemas.GetByJSchema(context)!;

            Uri reference;

            // Id is fully qualified
            // make it relative to the current schema
            if (currentKnownSchema.Id.IsAbsoluteUri)
            {
                if (currentKnownSchema.Id.IsBaseOf(knownSchema.Id))
                {
                    reference = currentKnownSchema.Id.MakeRelativeUri(knownSchema.Id);

                    // MakeRelativeUri escapes the result, need to unescape
                    reference = new Uri(Uri.UnescapeDataString(reference.OriginalString), UriKind.RelativeOrAbsolute);
                }
                else if (knownSchema.Id == currentKnownSchema.Id)
                {
                    reference = new Uri("#", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    reference = knownSchema.Id;
                }
            }
            else
            {
                reference = knownSchema.Id;
            }

            return reference;
        }

        private bool ShouldWriteReference(KnownSchema knownSchema, bool isDefinitions)
        {
            if (knownSchema.State == KnownSchemaState.DefinitionPending)
            {
                // only write schemas found in dependencies in the dependencies object
                // otherwise write reference
                return !isDefinitions;
            }

            if (knownSchema.State != KnownSchemaState.InlinePending)
            {
                if (_referenceHandling == JSchemaWriterReferenceHandling.Always)
                {
                    return true;
                }

                bool isRecursive = _schemaStack!.Contains(knownSchema.Schema);

                if (isRecursive)
                {
                    if (_referenceHandling == JSchemaWriterReferenceHandling.Auto)
                    {
                        return true;
                    }

                    throw new JSchemaException("Error writing schema because writing schema references has been disabled and the schema contains a circular reference.");
                }

                return false;
            }

            return false;
        }

        private void WriteReferenceObject(Uri reference)
        {
            _writer.WriteStartObject();
            _writer.WritePropertyName("$ref");
            _writer.WriteValue(reference);
            _writer.WriteEndObject();
        }

        private void WriteToken(JSchema context, JsonWriter writer, JToken token, bool isDefinitions = false)
        {
            if (token is JObject o)
            {
                JSchemaAnnotation schemaAnnotation = o.Annotation<JSchemaAnnotation>();

                if (schemaAnnotation != null)
                {
                    ReferenceOrWriteSchema(context, schemaAnnotation.Schema, null, isDefinitions);
                }
                else
                {
                    writer.WriteStartObject();

                    foreach (JProperty property in o.Properties())
                    {
                        writer.WritePropertyName(property.Name);

                        JToken value = property.Value;
                        if (value != null)
                        {
                            WriteToken(context, writer, value, isDefinitions);
                        }
                        else
                        {
                            writer.WriteNull();
                        }
                    }

                    writer.WriteEndObject();
                }
            }
            else if (token is JArray)
            {
                JArray a = (JArray)token;

                writer.WriteStartArray();

                for (int i = 0; i < a.Count; i++)
                {
                    WriteToken(context, writer, a[i]);
                }

                writer.WriteEndArray();
            }
            else if (token is JConstructor)
            {
                JConstructor c = (JConstructor)token;

                writer.WriteStartConstructor(c.Name);

                foreach (JToken t in c.Children())
                {
                    WriteToken(context, writer, t);
                }

                writer.WriteEndConstructor();
            }
            else if (token is JValue)
            {
                token.WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public void WriteSchema(JSchema schema)
        {
            ValidationUtils.ArgumentNotNull(schema, nameof(schema));

            _rootSchema = schema;

            _knownSchemas.Clear();

            JSchemaDiscovery discovery;

            if (_externalSchemas != null)
            {
                foreach (ExternalSchema externalSchema in _externalSchemas)
                {
                    discovery = new JSchemaDiscovery(schema, _knownSchemas, KnownSchemaState.External);
                    discovery.Discover(externalSchema.Schema, externalSchema.Uri);
                }
            }

            if (_version == SchemaVersion.Unset)
            {
                _version = SchemaVersionHelpers.MapSchemaUri(schema.SchemaVersion);
            }

            discovery = new JSchemaDiscovery(schema, _knownSchemas, KnownSchemaState.InlinePending);
            discovery.Discover(schema, null);

            KnownSchema rootKnownSchema = _knownSchemas.Single(s => s.Schema == schema);
            rootKnownSchema.State = KnownSchemaState.Written;

            WriteSchemaInternal(schema);
        }

        private void WriteSchemaInternal(JSchema schema)
        {
            _schemaStack?.Add(schema);

            if (schema.Valid != null)
            {
                _writer.WriteValue(schema.Valid);
            }
            else
            {
                WriteSchemaObjectInternal(schema);
            }

            _schemaStack?.RemoveAt(_schemaStack.Count - 1);
        }

        private void WriteSchemaObjectInternal(JSchema schema)
        {
            _writer.WriteStartObject();

            if (schema == _rootSchema)
            {
                Uri? resolvedVersionUri = (_version != SchemaVersion.Unset)
                    ? SchemaVersionHelpers.MapSchemaVersion(_version)
                    : schema.SchemaVersion;

                WritePropertyIfNotNull(_writer, Constants.PropertyNames.Schema, resolvedVersionUri);
            }

            if (EnsureVersion(SchemaVersion.Draft6))
            {
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.Id, schema.Id);
            }
            else
            {
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.IdDraft4, schema.Id);
            }
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Anchor, schema.Anchor);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Title, schema.Title);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Description, schema.Description);

            if (schema._extensionData != null)
            {
                foreach (KeyValuePair<string, JToken> extensionDataPair in schema._extensionData)
                {
                    bool isDefinitions = schema == _rootSchema && Constants.PropertyNames.IsDefinition(extensionDataPair.Key);

                    _writer.WritePropertyName(extensionDataPair.Key);
                    WriteToken(schema, _writer, extensionDataPair.Value, isDefinitions);
                }
            }

            if (schema.Type != null && schema.Type != JSchemaType.None)
            {
                WriteType(Constants.PropertyNames.Type, _writer, schema.Type.Value);
            }

            if (schema.Default != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.Default);
                schema.Default.WriteTo(_writer);
            }
            if (schema._allowAdditionalProperties != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.AdditionalProperties);
                _writer.WriteValue(schema._allowAdditionalProperties);
            }
            else if (schema.AdditionalProperties != null)
            {
                ReferenceOrWriteSchema(schema, schema.AdditionalProperties, Constants.PropertyNames.AdditionalProperties);
            }
            if (schema._allowAdditionalItems != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.AdditionalItems);
                _writer.WriteValue(schema._allowAdditionalItems);
            }
            else if (schema.AdditionalItems != null)
            {
                ReferenceOrWriteSchema(schema, schema.AdditionalItems, Constants.PropertyNames.AdditionalItems);
            }
            if (schema.AllowUnevaluatedProperties != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.UnevaluatedProperties);
                _writer.WriteValue(schema.AllowUnevaluatedProperties);
            }
            else if (schema.UnevaluatedProperties != null)
            {
                ReferenceOrWriteSchema(schema, schema.UnevaluatedProperties, Constants.PropertyNames.UnevaluatedProperties);
            }
            if (schema.AllowUnevaluatedItems != null)
            {
                _writer.WritePropertyName(Constants.PropertyNames.UnevaluatedItems);
                _writer.WriteValue(schema.AllowUnevaluatedItems);
            }
            else if (schema.UnevaluatedItems != null)
            {
                ReferenceOrWriteSchema(schema, schema.UnevaluatedItems, Constants.PropertyNames.UnevaluatedItems);
            }
            WriteSchemaDictionaryIfNotNull(schema, _writer, Constants.PropertyNames.Properties, schema._properties);
            WriteRequired(schema);
            WriteSchemaDictionaryIfNotNull(schema, _writer, Constants.PropertyNames.PatternProperties, schema._patternProperties);
            WriteItems(schema);
            WritePropertyIfNotDefault(_writer, Constants.PropertyNames.UniqueItems, schema.UniqueItems);

            if (EnsureVersion(SchemaVersion.Draft6))
            {
                WritePropertyIfNotNull(_writer, schema.ExclusiveMinimum ? Constants.PropertyNames.ExclusiveMinimum : Constants.PropertyNames.Minimum, schema.Minimum);
                WritePropertyIfNotNull(_writer, schema.ExclusiveMaximum ? Constants.PropertyNames.ExclusiveMaximum : Constants.PropertyNames.Maximum, schema.Maximum);
            }
            else
            {
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.Minimum, schema.Minimum);
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.Maximum, schema.Maximum);
                WritePropertyIfNotDefault(_writer, Constants.PropertyNames.ExclusiveMinimum, schema.ExclusiveMinimum);
                WritePropertyIfNotDefault(_writer, Constants.PropertyNames.ExclusiveMaximum, schema.ExclusiveMaximum);
            }

            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumLength, schema.MinimumLength);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumLength, schema.MaximumLength);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumItems, schema.MinimumItems);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumItems, schema.MaximumItems);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumProperties, schema.MinimumProperties);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumProperties, schema.MaximumProperties);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MinimumContains, schema.MinimumContains);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MaximumContains, schema.MaximumContains);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.MultipleOf, schema.MultipleOf);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Pattern, schema.Pattern);
            WritePropertyIfNotNull(_writer, Constants.PropertyNames.Format, schema.Format);
            if (EnsureVersion(SchemaVersion.Draft7))
            {
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.ContentEncoding, schema.ContentEncoding);
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.ContentMediaType, schema.ContentMediaType);
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.WriteOnly, schema.WriteOnly);
                WritePropertyIfNotNull(_writer, Constants.PropertyNames.ReadOnly, schema.ReadOnly);
            }

            if (!schema._enum.IsNullOrEmpty())
            {
                _writer.WritePropertyName(Constants.PropertyNames.Enum);
                _writer.WriteStartArray();
                foreach (JToken token in schema._enum)
                {
                    token.WriteTo(_writer);
                }
                _writer.WriteEndArray();
            }
            if (EnsureVersion(SchemaVersion.Draft6))
            {
                if (schema.Const != null)
                {
                    _writer.WritePropertyName(Constants.PropertyNames.Const);
                    schema.Const.WriteTo(_writer);
                }
                WriteSchema(schema, schema.PropertyNames, Constants.PropertyNames.PropertyNamesSchema);
                WriteSchema(schema, schema.Contains, Constants.PropertyNames.Contains);
            }
            WriteSchemas(schema, schema._allOf, Constants.PropertyNames.AllOf);
            WriteSchemas(schema, schema._anyOf, Constants.PropertyNames.AnyOf);
            WriteSchemas(schema, schema._oneOf, Constants.PropertyNames.OneOf);
            WriteSchema(schema, schema.Not, Constants.PropertyNames.Not);
            if (EnsureVersion(SchemaVersion.Draft7))
            {
                WriteSchema(schema, schema.If, Constants.PropertyNames.If);
                WriteSchema(schema, schema.Then, Constants.PropertyNames.Then);
                WriteSchema(schema, schema.Else, Constants.PropertyNames.Else);
            }
            WriteSchemaDictionaryIfNotNull(schema, _writer, Constants.PropertyNames.DependentSchemas, schema._dependentSchemas);
            if (!schema._dependentRequired.IsNullOrEmpty())
            {
                _writer.WritePropertyName(Constants.PropertyNames.DependentRequired);
                _writer.WriteStartObject();
                foreach (KeyValuePair<string, IList<string>> dependentRequired in schema._dependentRequired)
                {
                    _writer.WritePropertyName(dependentRequired.Key);
                    _writer.WriteStartArray();
                    for (int i = 0; i < dependentRequired.Value.Count; i++)
                    {
                        _writer.WriteValue(dependentRequired.Value[i]);
                    }
                    _writer.WriteEndArray();
                }
                _writer.WriteEndObject();
            }
            if (!schema._dependencies.IsNullOrEmpty())
            {
                _writer.WritePropertyName(Constants.PropertyNames.Dependencies);
                _writer.WriteStartObject();
                foreach (KeyValuePair<string, object> dependency in schema._dependencies)
                {
                    if (dependency.Value is JSchema s)
                    {
                        ReferenceOrWriteSchema(schema, s, dependency.Key);
                    }
                    else if (dependency.Value is IList<string> a)
                    {
                        _writer.WritePropertyName(dependency.Key);
                        _writer.WriteStartArray();
                        for (int i = 0; i < a.Count; i++)
                        {
                            _writer.WriteValue(a[i]);
                        }
                        _writer.WriteEndArray();
                    }
                }
                _writer.WriteEndObject();
            }
            if (schema.Ref != null)
            {
                WriteReference(schema, schema.Ref, Constants.PropertyNames.Ref);
            }

            _writer.WriteEndObject();
        }

        private void WriteRequired(JSchema schema)
        {
            if (!schema._required.IsNullOrEmpty())
            {
                _writer.WritePropertyName(Constants.PropertyNames.Required);
                _writer.WriteStartArray();
                foreach (string s in schema._required)
                {
                    _writer.WriteValue(s);
                }
                _writer.WriteEndArray();
            }
        }

        private void WriteSchema(JSchema context, JSchema? schema, string name)
        {
            if (schema != null)
            {
                ReferenceOrWriteSchema(context, schema, name);
            }
        }

        private void WriteSchemas(JSchema context, JSchemaCollection? schemas, string name)
        {
            if (!schemas.IsNullOrEmpty())
            {
                _writer.WritePropertyName(name);
                _writer.WriteStartArray();
                foreach (JSchema s in schemas)
                {
                    ReferenceOrWriteSchema(context, s, null);
                }
                _writer.WriteEndArray();
            }
        }

        private void WriteItems(JSchema schema)
        {
            if (schema._items.IsNullOrEmpty() && !schema.ItemsPositionValidation)
            {
                return;
            }

            if (!schema.ItemsPositionValidation)
            {
                if (!schema._items.IsNullOrEmpty())
                {
                    ReferenceOrWriteSchema(schema, schema._items[0], Constants.PropertyNames.Items);
                }
                else
                {
                    _writer.WriteStartObject();
                    _writer.WriteEndObject();
                }
            }
            else
            {
                _writer.WritePropertyName(Constants.PropertyNames.Items);

                _writer.WriteStartArray();
                if (schema._items != null)
                {
                    for (int i = 0; i < schema._items.Count; i++)
                    {
                        JSchema itemSchema = schema._items[i];

                        ReferenceOrWriteSchema(schema, itemSchema, null);
                    }
                }
                _writer.WriteEndArray();
            }
        }

        private void WriteSchemaDictionaryIfNotNull(JSchema context, JsonWriter writer, string propertyName, IDictionary<string, JSchema>? properties)
        {
            if (!properties.IsNullOrEmpty())
            {
                writer.WritePropertyName(propertyName);

                writer.WriteStartObject();
                foreach (KeyValuePair<string, JSchema> property in properties)
                {
                    ReferenceOrWriteSchema(context, property.Value, property.Key);
                }
                writer.WriteEndObject();
            }
        }

        private void WriteType(string propertyName, JsonWriter writer, JSchemaType type)
        {
            if (Enum.IsDefined(typeof(JSchemaType), type))
            {
                writer.WritePropertyName(propertyName);
                writer.WriteValue(JSchemaTypeHelpers.MapType(type));
            }
            else
            {
                // Known to not need disposing.
                IEnumerator<JSchemaType> en = EnumUtils.GetFlagsValues(type).Where(v => v != JSchemaType.None).GetEnumerator();
                if (!en.MoveNext())
                {
                    return;
                }

                writer.WritePropertyName(propertyName);

                JSchemaType first = en.Current;

                if (en.MoveNext())
                {
                    writer.WriteStartArray();
                    writer.WriteValue(JSchemaTypeHelpers.MapType(first));
                    do
                    {
                        writer.WriteValue(JSchemaTypeHelpers.MapType(en.Current));
                    } while (en.MoveNext());

                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteValue(JSchemaTypeHelpers.MapType(first));
                }
            }
        }

        private void WritePropertyIfNotDefault<T>(JsonWriter writer, string propertyName, T value, T? defaultValue = default)
        {
            if (!Equals(value, defaultValue))
            {
                writer.WritePropertyName(propertyName);
                writer.WriteValue(value);
            }
        }

        private void WritePropertyIfNotNull(JsonWriter writer, string propertyName, object? value)
        {
            if (value != null)
            {
                writer.WritePropertyName(propertyName);
                writer.WriteValue(value);
            }
        }

        internal bool EnsureVersion(SchemaVersion minimum, SchemaVersion? maximum = null)
        {
            return SchemaVersionHelpers.EnsureVersion(_version, minimum, maximum);
        }
    }
}