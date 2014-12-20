#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    public class JSchema
    {
        internal string Location { get; set; }

        internal Dictionary<string, JToken> _extensionData;
        internal List<JSchema> _items;
        internal List<JSchema> _anyOf;
        internal List<JSchema> _allOf;
        internal List<JSchema> _oneOf;
        internal Dictionary<string, object> _dependencies;
        internal List<JToken> _enum;
        internal Dictionary<string, JSchema> _properties;
        internal Dictionary<string, JSchema> _patternProperties;
        internal List<string> _required;

        public Uri Id { get; set; }
        internal Uri Reference { get; set; }
        public JSchemaType? Type { get; set; }
        public JToken Default { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of properties.</value>
        public IDictionary<string, JSchema> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new Dictionary<string, JSchema>();

                return _properties;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of items.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of items.</value>
        public IList<JSchema> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<JSchema>();

                return _items;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether items in an array are validated using the <see cref="JSchema"/> instance at their array position from <see cref="JSchema.Items"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if items are validated using their array position; otherwise, <c>false</c>.
        /// </value>
        public bool ItemsPositionValidation { get; set; }

        public IList<string> Required
        {
            get
            {
                if (_required == null)
                    _required = new List<string>();

                return _required;
            }
        }

        public IList<JSchema> AllOf
        {
            get
            {
                if (_allOf == null)
                    _allOf = new List<JSchema>();

                return _allOf;
            }
        }

        public IList<JSchema> AnyOf
        {
            get
            {
                if (_anyOf == null)
                    _anyOf = new List<JSchema>();

                return _anyOf;
            }
        }

        public IList<JSchema> OneOf
        {
            get
            {
                if (_oneOf == null)
                    _oneOf = new List<JSchema>();

                return _oneOf;
            }
        }

        public JSchema Not { get; set; }

        /// <summary>
        /// Gets or sets the a collection of valid enum values allowed.
        /// </summary>
        /// <value>A collection of valid enum values allowed.</value>
        public IList<JToken> Enum
        {
            get
            {
                if (_enum == null)
                    _enum = new List<JToken>();

                return _enum;
            }
        }

        /// <summary>
        /// Gets or sets whether the array items must be unique.
        /// </summary>
        public bool UniqueItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>The minimum length.</value>
        public int? MinimumLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        public int? MaximumLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public double? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public double? Maximum { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the value can not equal the number defined by the "minimum" attribute.
        /// </summary>
        /// <value>A flag indicating whether the value can not equal the number defined by the "minimum" attribute.</value>
        public bool ExclusiveMinimum { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the value can not equal the number defined by the "maximum" attribute.
        /// </summary>
        /// <value>A flag indicating whether the value can not equal the number defined by the "maximum" attribute.</value>
        public bool ExclusiveMaximum { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of items.
        /// </summary>
        /// <value>The minimum number of items.</value>
        public int? MinimumItems { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items.
        /// </summary>
        /// <value>The maximum number of items.</value>
        public int? MaximumItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of properties.
        /// </summary>
        /// <value>The minimum number of properties.</value>
        public int? MinimumProperties { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of properties.
        /// </summary>
        /// <value>The maximum number of properties.</value>
        public int? MaximumProperties { get; set; }

        public IDictionary<string, JToken> ExtensionData
        {
            get
            {
                if (_extensionData == null)
                    _extensionData = new Dictionary<string, JToken>();

                return _extensionData;
            }
        }

        public JSchema()
        {
            AllowAdditionalProperties = true;
            AllowAdditionalItems = true;
        }

        public static implicit operator JToken(JSchema x)
        {
            JObject token = new JObject();
            token.AddAnnotation(new JSchemaAnnotation(x));

            return token;
        }

        public static explicit operator JSchema(JToken x)
        {
            JSchemaAnnotation annotation = x.Annotation<JSchemaAnnotation>();
            if (annotation != null)
                return annotation.Schema;

            throw new Exception("TODO: load schema from JSON");
        }

        /// <summary>
        /// Writes this schema to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        public void WriteTo(JsonWriter writer)
        {
            WriteToInternal(writer, null);
        }

        /// <summary>
        /// Writes this schema to a <see cref="JsonWriter"/> using the specified <see cref="JSchemaResolver"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        /// <param name="settings">The settings used to write the schema.</param>
        public void WriteTo(JsonWriter writer, JSchemaWriteSettings settings)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            ValidationUtils.ArgumentNotNull(settings, "settings");

            WriteToInternal(writer, settings);
        }

        private void WriteToInternal(JsonWriter writer, JSchemaWriteSettings settings)
        {
            JSchemaWriter schemaWriter = new JSchemaWriter(writer, settings);

            schemaWriter.WriteSchema(this);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            WriteTo(jsonWriter);

            return writer.ToString();
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the object.
        /// </summary>
        public string Description { get; set; }

        public double? MultipleOf { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern { get; set; }

        public IDictionary<string, object> Dependencies
        {
            get
            {
                if (_dependencies == null)
                    _dependencies = new Dictionary<string, object>();

                return _dependencies;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of additional properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of additional properties.</value>
        public JSchema AdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets the pattern properties.
        /// </summary>
        /// <value>The pattern properties.</value>
        public IDictionary<string, JSchema> PatternProperties
        {
            get
            {
                if (_patternProperties == null)
                    _patternProperties = new Dictionary<string, JSchema>();

                return _patternProperties;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether additional properties are allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if additional properties are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of additional items.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of additional items.</value>
        public JSchema AdditionalItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional items are allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if additional items are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAdditionalItems { get; set; }

        internal bool DeprecatedRequired { get; set; }

        /// <summary>
        /// Reads a <see cref="JSchema"/> from the specified <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to read.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Read(JsonReader reader)
        {
            return Read(reader, DummyJSchemaResolver.Instance);
        }

        /// <summary>
        /// Reads a <see cref="JSchema"/> from the specified <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to read.</param>
        /// <param name="resolver">The <see cref="JSchemaResolver"/> to use when resolving schema references.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Read(JsonReader reader, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            JSchemaReader schemaReader = new JSchemaReader(resolver);
            return schemaReader.ReadRoot(reader);
        }

        /// <summary>
        /// Load a <see cref="JSchema"/> from a string that contains schema JSON.
        /// </summary>
        /// <param name="json">A <see cref="String"/> that contains JSON.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json)
        {
            return Parse(json, DummyJSchemaResolver.Instance);
        }

        /// <summary>
        /// Parses the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(json, "json");

            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                return Read(reader, resolver);
            }
        }
    }
}