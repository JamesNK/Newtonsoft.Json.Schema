#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Utilities;
using System.Globalization;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// An in-memory representation of a JSON Schema.
    /// </summary>
    public class JSchema
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets whether the object is required.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        /// Gets or sets whether the object is read only.
        /// </summary>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets whether the object is visible to users.
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// Gets or sets whether the object is transient.
        /// </summary>
        public bool? Transient { get; set; }

        /// <summary>
        /// Gets or sets the description of the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the types of values allowed by the object.
        /// </summary>
        /// <value>The type.</value>
        public JSchemaType? Type { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern { get; set; }

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
        /// Gets or sets a number that the value should be divisble by.
        /// </summary>
        /// <value>A number that the value should be divisble by.</value>
        public double? DivisibleBy { get; set; }

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
        public bool? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the value can not equal the number defined by the "maximum" attribute.
        /// </summary>
        /// <value>A flag indicating whether the value can not equal the number defined by the "maximum" attribute.</value>
        public bool? ExclusiveMaximum { get; set; }

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
        /// Gets or sets the <see cref="JSchema"/> of items.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of items.</value>
        public IList<JSchema> Items { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether items in an array are validated using the <see cref="JSchema"/> instance at their array position from <see cref="JSchema.Items"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if items are validated using their array position; otherwise, <c>false</c>.
        /// </value>
        public bool PositionalItemsValidation { get; set; }

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

        /// <summary>
        /// Gets or sets whether the array items must be unique.
        /// </summary>
        public bool UniqueItems { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of properties.</value>
        public IDictionary<string, JSchema> Properties { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> of additional properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> of additional properties.</value>
        public JSchema AdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets the pattern properties.
        /// </summary>
        /// <value>The pattern properties.</value>
        public IDictionary<string, JSchema> PatternProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional properties are allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if additional properties are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets the required property if this property is present.
        /// </summary>
        /// <value>The required property if this property is present.</value>
        public string Requires { get; set; }

        /// <summary>
        /// Gets or sets the a collection of valid enum values allowed.
        /// </summary>
        /// <value>A collection of valid enum values allowed.</value>
        public IList<JToken> Enum { get; set; }

        /// <summary>
        /// Gets or sets disallowed types.
        /// </summary>
        /// <value>The disallow types.</value>
        public JSchemaType? Disallow { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public JToken Default { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="JSchema"/> that this schema extends.
        /// </summary>
        /// <value>The collection of <see cref="JSchema"/> that this schema extends.</value>
        public IList<JSchema> Extends { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        internal string Location { get; set; }

        private readonly string _internalId = Guid.NewGuid().ToString("N");

        internal string InternalId
        {
            get { return _internalId; }
        }

        // if this is set then this schema instance is just a deferred reference
        // and will be replaced when the schema reference is resolved
        internal string DeferredReference { get; set; }
        internal bool ReferencesResolved { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchema"/> class.
        /// </summary>
        public JSchema()
        {
            AllowAdditionalProperties = true;
            AllowAdditionalItems = true;
        }

        /// <summary>
        /// Reads a <see cref="JSchema"/> from the specified <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to read.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Read(JsonReader reader)
        {
            return Read(reader, new JSchemaResolver());
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

            JSchemaBuilder builder = new JSchemaBuilder(resolver);
            return builder.Read(reader);
        }

        /// <summary>
        /// Load a <see cref="JSchema"/> from a string that contains schema JSON.
        /// </summary>
        /// <param name="json">A <see cref="String"/> that contains JSON.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json)
        {
            return Parse(json, new JSchemaResolver());
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

        /// <summary>
        /// Writes this schema to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        public void WriteTo(JsonWriter writer)
        {
            WriteTo(writer, new JSchemaResolver());
        }

        /// <summary>
        /// Writes this schema to a <see cref="JsonWriter"/> using the specified <see cref="JSchemaResolver"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        /// <param name="resolver">The resolver used.</param>
        public void WriteTo(JsonWriter writer, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            JSchemaWriter schemaWriter = new JSchemaWriter(writer, resolver);
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
    }
}