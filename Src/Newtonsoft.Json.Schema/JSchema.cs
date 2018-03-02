#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Collections;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    internal enum JSchemaState
    {
        Default,
        Loading,
        Reentrant
    }

    /// <summary>
    /// An in-memory representation of a JSON Schema.
    /// </summary>
    [JsonConverter(typeof(JSchemaConverter))]
    public class JSchema : IJsonLineInfo, IIdentiferScope
    {
        internal bool DeprecatedRequired { get; set; }
        internal JSchemaReader InternalReader { get; set; }

        internal Dictionary<string, JToken> _extensionData;
        internal JSchemaCollection _items;
        internal JSchemaCollection _anyOf;
        internal JSchemaCollection _allOf;
        internal JSchemaCollection _oneOf;
        internal JSchemaDependencyDictionary _dependencies;
        internal List<JToken> _enum;
        internal JSchemaDictionary _properties;
        internal JSchemaDictionary _patternProperties;
        internal List<string> _required;
        internal List<JsonValidator> _validators;

        private int _lineNumber;
        private int _linePosition;

        // this is used when the schema path is built
        // store the original reference to reset nested path back to this "root"
        internal Uri _referencedAs;

        private string _pattern;
        private Regex _patternRegex;
        private string _patternError;
        private Uri _id;
        private bool _itemsPositionValidation;
        private JSchema _if;
        private JSchema _then;
        private JSchema _else;
        private JSchema _not;
        private JSchema _contains;
        internal JSchema _propertyNames;
        private JSchema _additionalProperties;
        private JSchema _additionalItems;
        private JSchemaPatternDictionary _internalPatternProperties;

        internal Uri BaseUri;
        internal string Path;

        internal event Action<JSchema> Changed;
        internal readonly KnownSchemaCollection KnownSchemas;
        internal JSchemaState State;
        private double? _multipleOf;

        internal void OnChildChanged(JSchema changedSchema)
        {
            OnChanged(changedSchema);
        }

        internal void OnSelfChanged()
        {
            OnChanged(this);
        }

        private void OnChanged(JSchema changedSchema)
        {
            if (State != JSchemaState.Default)
            {
                return;
            }

            try
            {
                KnownSchemas.Clear();

                State = JSchemaState.Reentrant;
                Changed?.Invoke(changedSchema);
            }
            finally
            {
                State = JSchemaState.Default;
            }
        }

        internal IEnumerable<PatternSchema> GetPatternSchemas()
        {
            return _internalPatternProperties.GetPatternSchemas();
        }

        /// <summary>
        /// Gets or sets the $schema. This value will only be read from JSON and written to JSON if the <see cref="JSchema"/> is the root schema.
        /// </summary>
        public Uri SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this schema is <c>true</c> and always valid, or <c>false</c> and always invalid.
        /// </summary>
        /// <value>A flag indicating whether this schema is <c>true</c> and always valid, or <c>false</c> and always invalid.</value>
        public bool? Valid { get; set; }

        /// <summary>
        /// Gets or sets the $ref. This property is used when reading or writing referenced schemas without resolving them.
        /// Validating JSON with a schema that has a not null <see cref="Reference"/> value will error.
        /// </summary>
        public Uri Reference { get; set; }

        /// <summary>
        /// Gets or sets the schema ID.
        /// </summary>
        public Uri Id
        {
            get => _id;
            set
            {
                if (!UriComparer.Instance.Equals(value, _id))
                {
                    _id = value;
                    OnSelfChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the types of values allowed by the schema.
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
                {
                    _properties = new JSchemaDictionary(this);
                }

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
                {
                    _items = new JSchemaCollection(this);
                }

                return _items;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether items in an array are validated using the <see cref="JSchema"/> instance at their array position from <see cref="JSchema.Items"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if items are validated using their array position; otherwise, <c>false</c>.
        /// </value>
        public bool ItemsPositionValidation
        {
            get => _itemsPositionValidation;
            set
            {
                if (value != _itemsPositionValidation)
                {
                    _itemsPositionValidation = value;
                    OnSelfChanged();
                }
            }
        }

        /// <summary>
        /// Gets the required object properties.
        /// </summary>
        /// <value>The required object properties.</value>
        public IList<string> Required
        {
            get
            {
                if (_required == null)
                {
                    _required = new List<string>();
                }

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
                {
                    _allOf = new JSchemaCollection(this);
                }

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
                {
                    _anyOf = new JSchemaCollection(this);
                }

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
                {
                    _oneOf = new JSchemaCollection(this);
                }

                return _oneOf;
            }
        }

        /// <summary>
        /// Gets the If schema.
        /// </summary>
        /// <value>The If schema.</value>
        public JSchema If
        {
            get => _if;
            set => SetSchema(ref _if, value);
        }

        /// <summary>
        /// Gets the Then schema.
        /// </summary>
        /// <value>The Then schema.</value>
        public JSchema Then
        {
            get => _then;
            set => SetSchema(ref _then, value);
        }

        /// <summary>
        /// Gets the Else schema.
        /// </summary>
        /// <value>The Else schema.</value>
        public JSchema Else
        {
            get => _else;
            set => SetSchema(ref _else, value);
        }

        /// <summary>
        /// Gets the Not schema.
        /// </summary>
        /// <value>The Not schema.</value>
        public JSchema Not
        {
            get => _not;
            set => SetSchema(ref _not, value);
        }

        /// <summary>
        /// Gets the Contains schema.
        /// </summary>
        /// <value>The Contains schema.</value>
        public JSchema Contains
        {
            get => _contains;
            set => SetSchema(ref _contains, value);
        }

        /// <summary>
        /// Gets the PropertyNames schema.
        /// </summary>
        /// <value>The PropertyNames schema.</value>
        public JSchema PropertyNames
        {
            get => _propertyNames;
            set => SetSchema(ref _propertyNames, value);
        }

        private void SetSchema(ref JSchema schema, JSchema newSchema)
        {
            if (schema != newSchema)
            {
                if (schema != null)
                {
                    schema.Changed -= OnChildChanged;
                }

                schema = newSchema;
                schema.Changed += OnChildChanged;

                OnSelfChanged();
            }
        }

        /// <summary>
        /// Gets the collection of valid enum values allowed.
        /// </summary>
        /// <value>A collection of valid enum values allowed.</value>
        public IList<JToken> Enum
        {
            get
            {
                if (_enum == null)
                {
                    _enum = new List<JToken>();
                }

                return _enum;
            }
        }

        /// <summary>
        /// Gets or sets the const value.
        /// </summary>
        /// <value>The const value.</value>
        public JToken Const { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the array items must be unique.
        /// </summary>
        /// <value>A flag indicating whether the array items must be unique.</value>
        public bool UniqueItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of a string.
        /// </summary>
        /// <value>The minimum length of a string.</value>
        public long? MinimumLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of a string.
        /// </summary>
        /// <value>The maximum length of a string.</value>
        public long? MaximumLength { get; set; }

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
        public long? MinimumItems { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of array items.
        /// </summary>
        /// <value>The maximum number of array items.</value>
        public long? MaximumItems { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of object properties.
        /// </summary>
        /// <value>The minimum number of object properties.</value>
        public long? MinimumProperties { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of object properties.
        /// </summary>
        /// <value>The maximum number of object properties.</value>
        public long? MaximumProperties { get; set; }

        /// <summary>
        /// Gets or sets the content encoding of a string.
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Gets or sets the content media type of a string.
        /// </summary>
        public string ContentMediaType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the data is write only.
        /// Has no no effect on validation.
        /// </summary>
        public bool? WriteOnly { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the data is read only.
        /// Has no no effect on validation.
        /// </summary>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets the extension data for the <see cref="JSchema"/>.
        /// </summary>
        /// <value>The extension data for the <see cref="JSchema"/>.</value>
        public IDictionary<string, JToken> ExtensionData
        {
            get
            {
                if (_extensionData == null)
                {
                    _extensionData = new Dictionary<string, JToken>(StringComparer.Ordinal);
                }

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
            KnownSchemas = new KnownSchemaCollection();
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
            {
                return annotation.Schema;
            }

            throw new JSchemaException("Cannot convert JToken to JSchema. No schema is associated with this token.");
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
            ValidationUtils.ArgumentNotNull(writer, nameof(writer));
            ValidationUtils.ArgumentNotNull(settings, nameof(settings));

            WriteToInternal(writer, settings);
        }

        private void WriteToInternal(JsonWriter writer, JSchemaWriterSettings settings)
        {
            JSchemaWriter schemaWriter = new JSchemaWriter(writer, settings);

            schemaWriter.WriteSchema(this);
        }

        /// <summary>
        /// Returns the JSON for this schema.
        /// </summary>
        /// <returns>The JSON for this schema.</returns>
        public override string ToString()
        {
            return ToStringInternal(null);
        }

        /// <summary>
        /// Returns the JSON for this schema using the specified <see cref="SchemaVersion"/>.
        /// </summary>
        /// <param name="version">The <see cref="SchemaVersion"/> used to create JSON for this schema.</param>
        /// <returns>The JSON for this schema.</returns>
        public string ToString(SchemaVersion version)
        {
            JSchemaWriterSettings settings = version != Schema.SchemaVersion.Unset
                ? new JSchemaWriterSettings { Version = version }
                : null;

            return ToStringInternal(settings);
        }

        private string ToStringInternal(JSchemaWriterSettings settings)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            WriteToInternal(jsonWriter, settings);

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
        public double? MultipleOf
        {
            get => _multipleOf;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("MultipleOf must be greater than zero.", nameof(value));
                }

                _multipleOf = value;
            }
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern
        {
            get => _pattern;
            set
            {
                if (value != _pattern)
                {
                    // clear cached regex when pattern changes
                    _patternRegex = null;
                    _patternError = null;
                    _pattern = value;
                }
            }
        }

        internal bool TryGetPatternRegex(
#if !(NET35 || NET40)
            TimeSpan? matchTimeout,
#endif
            out Regex regex,
            out string errorMessage)
        {
            bool result = RegexHelpers.TryGetPatternRegex(
                _pattern,
#if !(NET35 || NET40)
                matchTimeout,
#endif
                ref _patternRegex,
                ref _patternError);
            regex = _patternRegex;
            errorMessage = _patternError;

            return result;
        }

        /// <summary>
        /// Gets the object property dependencies.
        /// </summary>
        public IDictionary<string, object> Dependencies
        {
            get
            {
                if (_dependencies == null)
                {
                    _dependencies = new JSchemaDependencyDictionary(this);
                }

                return _dependencies;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="JSchema"/> for additional properties.
        /// </summary>
        /// <value>The <see cref="JSchema"/> for additional properties.</value>
        public JSchema AdditionalProperties
        {
            get => _additionalProperties;
            set => SetSchema(ref _additionalProperties, value);
        }

        /// <summary>
        /// Gets the object pattern properties.
        /// </summary>
        /// <value>The object pattern properties.</value>
        public IDictionary<string, JSchema> PatternProperties
        {
            get
            {
                if (_patternProperties == null)
                {
                    _internalPatternProperties = new JSchemaPatternDictionary();
                    _patternProperties = new JSchemaDictionary(this, _internalPatternProperties);
                }

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
        public JSchema AdditionalItems
        {
            get => _additionalItems;
            set => SetSchema(ref _additionalItems, value);
        }

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
        /// Gets a <see cref="JsonValidator"/> collection that will be used during validation.
        /// </summary>
        /// <value>The converters.</value>
        public List<JsonValidator> Validators
        {
            get
            {
                if (_validators == null)
                {
                    _validators = new List<JsonValidator>();
                }

                return _validators;
            }
        }

        /// <summary>
        /// Loads a <see cref="JSchema"/> from the specified <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to load.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Load(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, nameof(reader));

            return Load(reader, new JSchemaReaderSettings());
        }

        /// <summary>
        /// Loads a <see cref="JSchema"/> from a <see cref="JsonReader"/> using the given <see cref="JSchemaResolver"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to load.</param>
        /// <param name="resolver">The <see cref="JSchemaResolver"/> to use when resolving schema references.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Load(JsonReader reader, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(reader, nameof(reader));
            ValidationUtils.ArgumentNotNull(resolver, nameof(resolver));

            return Load(reader, new JSchemaReaderSettings
            {
                Resolver = resolver
            });
        }

        /// <summary>
        /// Loads a <see cref="JSchema"/> from a <see cref="JsonReader"/> using the given <see cref="JSchemaResolver"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> containing the JSON Schema to load.</param>
        /// <param name="settings">The <see cref="JSchemaReaderSettings"/> used to load the schema.</param>
        /// <returns>The <see cref="JSchema"/> object representing the JSON Schema.</returns>
        public static JSchema Load(JsonReader reader, JSchemaReaderSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, nameof(reader));
            ValidationUtils.ArgumentNotNull(settings, nameof(settings));

            JSchemaReader schemaReader = new JSchemaReader(settings);
            JSchema schema = schemaReader.ReadRoot(reader);

            return schema;
        }

        /// <summary>
        /// Load a <see cref="JSchema"/> from a string that contains schema JSON.
        /// </summary>
        /// <param name="json">A <see cref="String"/> that contains JSON.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json)
        {
            ValidationUtils.ArgumentNotNull(json, nameof(json));

            return Parse(json, new JSchemaReaderSettings());
        }

        /// <summary>
        /// Load a <see cref="JSchema"/> from a string that contains schema JSON using the given <see cref="JSchemaResolver"/>.
        /// </summary>
        /// <param name="json">The JSON.</param>
        /// <param name="resolver">The <see cref="JSchemaResolver"/> to use when resolving schema references.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json, JSchemaResolver resolver)
        {
            ValidationUtils.ArgumentNotNull(json, nameof(json));
            ValidationUtils.ArgumentNotNull(resolver, nameof(resolver));

            return Parse(json, new JSchemaReaderSettings
            {
                Resolver = resolver
            });
        }

        /// <summary>
        /// Load a <see cref="JSchema"/> from a string that contains schema JSON using the given <see cref="JSchemaReaderSettings"/>.
        /// </summary>
        /// <param name="json">The JSON.</param>
        /// <param name="settings">The <see cref="JSchemaReaderSettings"/> used to load the schema.</param>
        /// <returns>A <see cref="JSchema"/> populated from the string that contains JSON.</returns>
        public static JSchema Parse(string json, JSchemaReaderSettings settings)
        {
            ValidationUtils.ArgumentNotNull(json, nameof(json));
            ValidationUtils.ArgumentNotNull(settings, nameof(settings));

            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                JSchema s = Load(reader, settings);

                // read to make sure there isn't additional content
                reader.Read();

                return s;
            }
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return _lineNumber != 0;
        }

        int IJsonLineInfo.LineNumber => _lineNumber;

        int IJsonLineInfo.LinePosition => _linePosition;

        internal void SetLineInfo(IJsonLineInfo lineInfo)
        {
            _lineNumber = lineInfo.LineNumber;
            _linePosition = lineInfo.LinePosition;
        }
    }
}