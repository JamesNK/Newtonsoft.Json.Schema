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
    /// <summary>
    /// An in-memory representation of a JSON Schema.
    /// </summary>
    public class JSchema
    {
        internal Uri Reference { get; set; }
        internal bool DeprecatedRequired { get; set; }

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

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the types of values allowed by the object.
        /// </summary>
        /// <value>The type.</value>
        public JSchemaType? Type { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public JToken Default { get; set; }

        /// <summary>
        /// Gets the object property <see cref="JSchema"/>s.
        /// </summary>
        /// <value>The object property <see cref="JSchema"/>s.</value>
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
        /// Gets the array item <see cref="JSchema"/>s.
        /// </summary>
        /// <value>The array item <see cref="JSchema"/>s.</value>
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

        /// <summary>
        /// Gets the required object properties.
        /// </summary>
        /// <value>The required object properties.</value>
        public IList<string> Required
        {
            get
            {
                if (_required == null)
                    _required = new List<string>();

                return _required;
            }
        }

        /// <summary>
        /// Gets the AllOf schemas.
        /// </summary>
        /// <value>The AllOf schemas.</value>
        public IList<JSchema> AllOf
        {
            get
            {
                if (_allOf == null)
                    _allOf = new List<JSchema>();

                return _allOf;
            }
        }

        /// <summary>
        /// Gets the AnyOf schemas.
        /// </summary>
        /// <value>The AnyOf schemas.</value>
        public IList<JSchema> AnyOf
        {
            get
            {
                if (_anyOf == null)
                    _anyOf = new List<JSchema>();

                return _anyOf;
            }
        }

        /// <summary>
        /// Gets the OneOf schemas.
        /// </summary>
        /// <value>The OneOf schemas.</value>
        public IList<JSchema> OneOf
        {
            get
            {
                if (_oneOf == null)
                    _oneOf = new List<JSchema>();

                return _oneOf;
            }
        }

        /// <summary>
        /// Gets the Not schema.
        /// </summary>
        /// <value>The Not schema.</value>
        public JSchema Not { get; set; }

        /// <summary>
        /// Gets the collection of valid enum values allowed.
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
        /// Gets or sets a flag indicating whether the array items must be unique.
        /// </summary>
        /// <value>A flag indicating whether the array items must be unique.</value>
        public bool UniqueItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of a string.
        /// </summary>
        /// <value>The minimum length of a string.</value>
        public int? MinimumLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of a string.
        /// </summary>
        /// <value>The maximum length of a string.</value>
        public int? MaximumLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of a number.
        /// </summary>
        /// <value>The minimum value of a number.</value>
        public double? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of a number.
        /// </summary>
        /// <value>The maximum value of a number.</value>
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
        /// Gets or sets the minimum number of array items.
        /// </summary>
        /// <value>The minimum number of array items.</value>
        public int? MinimumItems { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of array items.
        /// </summary>
        /// <value>The maximum number of array items.</value>
        public int? MaximumItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of object properties.
        /// </summary>
        /// <value>The minimum number of object properties.</value>
        public int? MinimumProperties { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of object properties.
        /// </summary>
        /// <value>The maximum number of object properties.</value>
        public int? MaximumProperties { get; set; }

        /// <summary>
        /// Gets the extension data for the <see cref="JSchema"/>.
        /// </summary>
        /// <value>The extension data for the <see cref="JSchema"/>.</value>
        public IDictionary<string, JToken> ExtensionData
        {
            get
            {
                if (_extensionData == null)
                    _extensionData = new Dictionary<string, JToken>();

                return _extensionData;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchema"/> class.
        /// </summary>
        public JSchema()
        {
            AllowAdditionalProperties = true;
            AllowAdditionalItems = true;
        }

        /// <summary>
        /// Gets a <see cref="JToken"/> associated with the <see cref="JSchema"/>.
        /// </summary>
        /// <param name="s">The schema.</param>
        /// <returns>A <see cref="JToken"/> associated with the <see cref="JSchema"/>.</returns>
        public static implicit operator JToken(JSchema s)
        {
            JObject token = new JObject();
            token.AddAnnotation(new JSchemaAnnotation(s));

            return token;
        }

        /// <summary>
        /// Gets the <see cref="JSchema"/> associated with the <see cref="JToken"/>.
        /// </summary>
        /// <param name="t">The token.</param>
        /// <returns>The <see cref="JSchema"/> associated with the <see cref="JToken"/>.</returns>
        public static explicit operator JSchema(JToken t)
        {
            JSchemaAnnotation annotation = t.Annotation<JSchemaAnnotation>();
            if (annotation != null)
                return annotation.Schema;

            throw new JsonException("Cannot convert JToken to JSchema. No schema is associated with this token.");
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
        public void WriteTo(JsonWriter writer, JSchemaWriterSettings settings)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            ValidationUtils.ArgumentNotNull(settings, "settings");

            WriteToInternal(writer, settings);
        }

        private void WriteToInternal(JsonWriter writer, JSchemaWriterSettings settings)
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
        /// Gets or sets the title of the schema.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the schema.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the multiple of.
        /// </summary>
        public double? MultipleOf { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets the object property dependencies.
        /// </summary>
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
        /// Gets or sets the <see cref="JSchema"/> for additional properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> for additional properties.</value>
        public JSchema AdditionalProperties { get; set; }

        /// <summary>
        /// Gets the object pattern properties.
        /// </summary>
        /// <value>The object pattern properties.</value>
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
        /// Gets or sets the <see cref="JSchema"/> for additional items.
        /// </summary>
        /// <value>The <see cref="JSchema"/> for additional items.</value>
        public JSchema AdditionalItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional items are allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if additional items are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAdditionalItems { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>
        /// Reads a <see cref="JSchema"/> from the specified <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to read.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Read(JsonReader reader)
        {
            return Read(reader, JSchemaDummyResolver.Instance);
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
            return Parse(json, JSchemaDummyResolver.Instance);
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