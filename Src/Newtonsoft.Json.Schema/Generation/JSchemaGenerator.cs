#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Generates a <see cref="JSchema"/> from a specified <see cref="Type"/>.
    /// </summary>
    public class JSchemaGenerator
    {
        private readonly List<TypeSchema> _typeSchemas;

        private IContractResolver _contractResolver;
        private IList<JSchemaGenerationProvider> _generationProviders;

        /// <summary>
        /// Gets or sets how IDs are generated for schemas with no ID.
        /// </summary>
        public SchemaIdGenerationHandling SchemaIdGenerationHandling { get; set; }

        /// <summary>
        /// Gets or sets the schema property order.
        /// </summary>
        public SchemaPropertyOrderHandling SchemaPropertyOrderHandling { get; set; }

        /// <summary>
        /// Gets or sets the location of referenced schemas.
        /// </summary>
        public SchemaLocationHandling SchemaLocationHandling { get; set; }

        /// <summary>
        /// Gets or sets whether generated schemas can be referenced.
        /// </summary>
        public SchemaReferenceHandling SchemaReferenceHandling { get; set; }

        /// <summary>
        /// Gets or sets the default required state of schemas.
        /// </summary>
        public Required DefaultRequired { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="JSchemaGenerationProvider"/> instances that are used to customize <see cref="JSchema"/> generation.
        /// </summary>
        public IList<JSchemaGenerationProvider> GenerationProviders
        {
            get
            {
                if (_generationProviders == null)
                {
                    _generationProviders = new List<JSchemaGenerationProvider>();
                }

                return _generationProviders;
            }
        }

        /// <summary>
        /// Gets or sets the contract resolver.
        /// </summary>
        /// <value>The contract resolver.</value>
        public IContractResolver ContractResolver
        {
            get
            {
                if (_contractResolver == null)
                {
                    return DefaultContractResolver.Instance;
                }

                return _contractResolver;
            }
            set { _contractResolver = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaGenerator"/> class.
        /// </summary>
        public JSchemaGenerator()
        {
            _typeSchemas = new List<TypeSchema>();

            SchemaReferenceHandling = SchemaReferenceHandling.Objects;
            DefaultRequired = Required.AllowNull;
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            return Generate(type, false);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="JSchema"/> will be nullable.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            LicenseHelpers.IncrementAndCheckGenerationCount();

            Required required = rootSchemaNullable ? Required.AllowNull : Required.Always;

            JSchema schema = GenerateInternal(type, required, null, null);

            if (SchemaLocationHandling == SchemaLocationHandling.Definitions)
            {
                if (_typeSchemas.Count > 1)
                {
                    JToken definitions;
                    if (!schema.ExtensionData.TryGetValue("definitions", out definitions))
                    {
                        definitions = new JObject();
                        schema.ExtensionData["definitions"] = definitions;
                    }

                    foreach (TypeSchema t in _typeSchemas)
                    {
                        if (t.Schema == schema)
                        {
                            continue;
                        }

                        string id;
                        if (t.Schema.Id != null)
                        {
                            id = t.Schema.Id.OriginalString;
                        }
                        else
                        {
                            id = t.Key.Type.Name;
                            int i = 1;
                            while (definitions[id] != null)
                            {
                                id = t.Key.Type.Name + "-" + i;
                                i++;
                            }
                        }

                        definitions[id] = t.Schema;
                    }
                }
            }

            return schema;
        }

        private string GetTitle(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Title))
            {
                return containerAttribute.Title;
            }

            return null;
        }

        private string GetDescription(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Description))
            {
                return containerAttribute.Description;
            }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            DescriptionAttribute descriptionAttribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }
#endif

            return null;
        }

        private Uri GetTypeId(Type type, bool explicitOnly)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            Uri typeId;

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Id))
            {
                typeId = new Uri(containerAttribute.Id, UriKind.RelativeOrAbsolute);
            }
            else
            {
                if (explicitOnly)
                {
                    return null;
                }

                switch (SchemaIdGenerationHandling)
                {
                    case SchemaIdGenerationHandling.TypeName:
                        typeId = new Uri(type.Name, UriKind.RelativeOrAbsolute);
                        break;
                    case SchemaIdGenerationHandling.FullTypeName:
                        typeId = new Uri(type.FullName, UriKind.RelativeOrAbsolute);
                        break;
                    case SchemaIdGenerationHandling.AssemblyQualifiedName:
                        typeId = new Uri(type.AssemblyQualifiedName, UriKind.RelativeOrAbsolute);
                        break;
                    default:
                        return null;
                }
            }

            // avoid id conflicts
            Uri resolvedTypeId = typeId;
            int i = 1;
            while (_typeSchemas.Any(s => s.Schema.Id == resolvedTypeId))
            {
                resolvedTypeId = new Uri(typeId.OriginalString + "-" + i, UriKind.RelativeOrAbsolute);
                i++;
            }

            return resolvedTypeId;
        }

        private JSchemaGenerationProvider ResolveTypeProvider(Type nonNullableType, JsonProperty memberProperty)
        {
            JSchemaGenerationProviderAttribute providerAttribute = null;

            if (memberProperty != null && memberProperty.AttributeProvider != null)
            {
                providerAttribute = (JSchemaGenerationProviderAttribute)memberProperty.AttributeProvider.GetAttributes(typeof(JSchemaGenerationProviderAttribute), true).SingleOrDefault();
            }

            if (providerAttribute == null)
            {
                providerAttribute = ReflectionUtils.GetAttribute<JSchemaGenerationProviderAttribute>(nonNullableType, true);

                if (providerAttribute == null)
                {
                    return null;
                }
            }

            JSchemaGenerationProvider provider = (JSchemaGenerationProvider)Activator.CreateInstance(providerAttribute.ProviderType, providerAttribute.ProviderParameters);
            return provider;
        }

        private JSchema GenerateInternal(Type type, Required? valueRequired, JsonProperty memberProperty, JsonContainerContract container)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            Type nonNullableType = ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;

            Uri explicitId = GetTypeId(nonNullableType, true);

            JsonContract contract = ContractResolver.ResolveContract(type);

            Required resolvedRequired = valueRequired ?? DefaultRequired;

            TypeSchemaKey key = CreateKey(resolvedRequired, memberProperty, contract);

            if (ShouldReferenceType(contract))
            {
                TypeSchema typeSchema = _typeSchemas.SingleOrDefault(s => s.Key.Equals(key));
                if (typeSchema != null)
                {
                    return typeSchema.Schema;
                }
            }

            JSchema schema = null;

            JSchemaGenerationProvider provider = ResolveTypeProvider(nonNullableType, memberProperty);
            if (provider != null)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, resolvedRequired, memberProperty, container, this);

                schema = provider.GetSchema(context);

                if (schema == null)
                {
                    throw new JSchemaException("Could not get schema for type '{0}' from provider '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.FullName, provider.GetType().FullName));
                }
            }
            
            if (_generationProviders != null)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, resolvedRequired, memberProperty, container, this);

                foreach (JSchemaGenerationProvider generationProvider in _generationProviders)
                {
                    schema = generationProvider.GetSchema(context);
                }
            }

            if (schema != null)
            {
                if (ShouldReferenceType(contract))
                {
                    _typeSchemas.Add(new TypeSchema(key, schema));
                }
            }
            else
            {
                schema = new JSchema();
                if (explicitId != null)
                {
                    schema.Id = explicitId;
                }

                if (ShouldReferenceType(contract))
                {
                    _typeSchemas.Add(new TypeSchema(key, schema));
                }

                PopulateSchema(schema, contract, memberProperty, resolvedRequired);
            }

            return schema;
        }

        private bool ShouldReferenceType(JsonContract contract)
        {
            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                    return (SchemaReferenceHandling & SchemaReferenceHandling.Objects) == SchemaReferenceHandling.Objects;
                case JsonContractType.Array:
                    return (SchemaReferenceHandling & SchemaReferenceHandling.Arrays) == SchemaReferenceHandling.Arrays;
                case JsonContractType.Dictionary:
                    return (SchemaReferenceHandling & SchemaReferenceHandling.Dictionaries) == SchemaReferenceHandling.Dictionaries;
                default:
                    return false;
            }
        }

        private static TypeSchemaKey CreateKey(Required valueRequired, JsonProperty memberProperty, JsonContract contract)
        {
            int? minLength = DataAnnotationHelpers.GetMinLength(memberProperty);
            int? maxLength = DataAnnotationHelpers.GetMaxLength(memberProperty);

            Required resolvedRequired;
            switch (valueRequired)
            {
                case Required.Default:
                case Required.AllowNull:
                    resolvedRequired = Required.AllowNull;
                    break;
                case Required.Always:
                case Required.DisallowNull:
                    resolvedRequired = Required.DisallowNull;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("valueRequired");
            }

            TypeSchemaKey key = new TypeSchemaKey(contract.UnderlyingType, resolvedRequired, minLength, maxLength);

            return key;
        }

        private void PopulateSchema(JSchema schema, JsonContract contract, JsonProperty memberProperty, Required valueRequired)
        {
            schema.Title = GetTitle(contract.NonNullableUnderlyingType);
            schema.Description = GetDescription(contract.NonNullableUnderlyingType);

            JsonConverter converter = contract.Converter ?? contract.InternalConverter;

            if (converter != null)
            {
                schema.Type = null;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        if (schema.Id == null)
                        {
                            schema.Id = GetTypeId(contract.NonNullableUnderlyingType, false);
                        }

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        GenerateObjectSchema(schema, contract.NonNullableUnderlyingType, (JsonObjectContract)contract);
                        break;
                    case JsonContractType.Array:
                        if (schema.Id == null)
                        {
                            schema.Id = GetTypeId(contract.NonNullableUnderlyingType, false);
                        }

                        schema.Type = AddNullType(JSchemaType.Array, valueRequired);
                        schema.MinimumItems = DataAnnotationHelpers.GetMinLength(memberProperty);
                        schema.MaximumItems = DataAnnotationHelpers.GetMaxLength(memberProperty);

                        JsonArrayAttribute arrayAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(contract.NonNullableUnderlyingType);

                        Required? required = null;
                        if (arrayAttribute != null && !arrayAttribute.AllowNullItems)
                        {
                            required = Required.Always;
                        }

                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(contract.NonNullableUnderlyingType);
                        if (collectionItemType != null)
                        {
                            schema.Items.Add(GenerateInternal(collectionItemType, required, null, (JsonArrayContract)contract));
                        }
                        break;
                    case JsonContractType.Primitive:
                        schema.Type = GetJSchemaType(contract.UnderlyingType, valueRequired);

                        if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.String))
                        {
                            int minimumLength;
                            int maximumLength;
                            if (DataAnnotationHelpers.GetStringLength(memberProperty, out minimumLength, out maximumLength))
                            {
                                schema.MinimumLength = minimumLength;
                                schema.MaximumLength = maximumLength;
                            }
                            else
                            {
                                schema.MinimumLength = DataAnnotationHelpers.GetMinLength(memberProperty);
                                schema.MaximumLength = DataAnnotationHelpers.GetMaxLength(memberProperty);
                            }

                            schema.Pattern = DataAnnotationHelpers.GetPattern(memberProperty);
                            schema.Format = DataAnnotationHelpers.GetFormat(memberProperty);
                        }
                        if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Number) || JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Integer))
                        {
                            double minimum;
                            double maximum;
                            if (DataAnnotationHelpers.GetRange(memberProperty, out minimum, out maximum))
                            {
                                schema.Minimum = minimum;
                                schema.Maximum = maximum;
                            }
                        }

                        if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Integer)
                            && contract.NonNullableUnderlyingType.IsEnum()
                            && ReflectionUtils.GetAttribute<FlagsAttribute>(contract.NonNullableUnderlyingType) == null)
                        {
                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(contract.NonNullableUnderlyingType);
                            foreach (EnumValue<long> enumValue in enumValues)
                            {
                                JToken value = JToken.FromObject(enumValue.Value);

                                schema.Enum.Add(value);
                            }
                        }

                        Type enumDataType = DataAnnotationHelpers.GetEnumDataType(memberProperty);
                        if (enumDataType != null && CollectionUtils.IsNullOrEmpty(schema._enum))
                        {
                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(enumDataType);
                            foreach (EnumValue<long> enumValue in enumValues)
                            {
                                JToken value = (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.String))
                                    ? enumValue.Name
                                    : JToken.FromObject(enumValue.Value);

                                schema.Enum.Add(value);
                            }
                        }
                        break;
                    case JsonContractType.String:
                        JSchemaType schemaType = (!ReflectionUtils.IsNullable(contract.UnderlyingType))
                            ? JSchemaType.String
                            : AddNullType(JSchemaType.String, valueRequired);

                        schema.Type = schemaType;
                        schema.MinimumLength = DataAnnotationHelpers.GetMinLength(memberProperty);
                        schema.MaximumLength = DataAnnotationHelpers.GetMaxLength(memberProperty);
                        break;
                    case JsonContractType.Dictionary:
                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        schema.MinimumProperties = DataAnnotationHelpers.GetMinLength(memberProperty);
                        schema.MaximumProperties = DataAnnotationHelpers.GetMaxLength(memberProperty);

                        Type keyType;
                        Type valueType;
                        ReflectionUtils.GetDictionaryKeyValueTypes(contract.NonNullableUnderlyingType, out keyType, out valueType);

                        if (keyType != null)
                        {
                            JsonContract keyContract = ContractResolver.ResolveContract(keyType);

                            // can be converted to a string
                            if (keyContract.ContractType == JsonContractType.Primitive)
                            {
                                schema.AdditionalProperties = GenerateInternal(valueType, Required.Default, null, (JsonDictionaryContract)contract);
                            }
                        }
                        break;
                    case JsonContractType.Serializable:
                        if (schema.Id == null)
                        {
                            schema.Id = GetTypeId(contract.NonNullableUnderlyingType, false);
                        }

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        schema.AllowAdditionalProperties = true;
                        break;
                    case JsonContractType.Dynamic:
                    case JsonContractType.Linq:
                        schema.Type = null;
                        break;
                    default:
                        throw new JSchemaException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
                }
            }
        }

        private JSchemaType AddNullType(JSchemaType type, Required valueRequired)
        {
            if (valueRequired == Required.Default || valueRequired == Required.AllowNull)
            {
                return type | JSchemaType.Null;
            }

            return type;
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
        {
            return ((value & flag) == flag);
        }

        private void GenerateObjectSchema(JSchema schema, Type type, JsonObjectContract contract)
        {
            IList<JsonProperty> properties;
            if (SchemaPropertyOrderHandling == SchemaPropertyOrderHandling.Alphabetical)
            {
                properties = contract.Properties
                    .OrderBy(p => p.Order ?? 0)
                    .ThenBy(p => p.PropertyName)
                    .ToList();
            }
            else
            {
                properties = contract.Properties;
            }

            foreach (JsonProperty property in properties)
            {
                if (!property.Ignored)
                {
                    Required? required = property._required;
                    if (DataAnnotationHelpers.GetRequired(property))
                    {
                        required = Required.Always;
                    }

                    JSchema propertySchema = GenerateInternal(property.PropertyType, required, property, contract);

                    if (property.DefaultValue != null)
                    {
                        propertySchema.Default = JToken.FromObject(property.DefaultValue);
                    }

                    schema.Properties.Add(property.PropertyName, propertySchema);

                    Required resolvedRequired = required ?? DefaultRequired;
                    bool optional;
                    switch (resolvedRequired)
                    {
                        case Required.Default:
                        case Required.DisallowNull:
                            optional = true;
                            break;
                        case Required.Always:
                        case Required.AllowNull:
                            optional = property.NullValueHandling == NullValueHandling.Ignore ||
                                       HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) ||
                                       property.ShouldSerialize != null ||
                                       property.GetIsSpecified != null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("required");
                    }

                    if (!optional)
                    {
                        schema.Required.Add(property.PropertyName);
                    }
                }
            }

            if (type.IsSealed())
            {
                schema.AllowAdditionalProperties = false;
            }
        }

        internal static bool HasFlag(JSchemaType? value, JSchemaType flag)
        {
            // default value is Any
            if (value == null)
            {
                return true;
            }

            bool match = ((value & flag) == flag);
            if (match)
            {
                return true;
            }

            // integer is a subset of float
            if (flag == JSchemaType.Integer && (value & JSchemaType.Number) == JSchemaType.Number)
            {
                return true;
            }

            return false;
        }

        private JSchemaType GetJSchemaType(Type type, Required valueRequired)
        {
            JSchemaType schemaType = JSchemaType.None;
            if (ReflectionUtils.IsNullable(type))
            {
                if (valueRequired == Required.Default || valueRequired == Required.AllowNull)
                {
                    schemaType = JSchemaType.Null;
                }

                if (ReflectionUtils.IsNullableType(type))
                {
                    type = Nullable.GetUnderlyingType(type);
                }
            }

            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(type);

            switch (typeCode)
            {
                case PrimitiveTypeCode.Empty:
                case PrimitiveTypeCode.Object:
                    return schemaType | JSchemaType.String;
                case PrimitiveTypeCode.DBNull:
                    return schemaType | JSchemaType.Null;
                case PrimitiveTypeCode.Boolean:
                    return schemaType | JSchemaType.Boolean;
                case PrimitiveTypeCode.Char:
                    return schemaType | JSchemaType.String;
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                case PrimitiveTypeCode.BigInteger:
                    return schemaType | JSchemaType.Integer;
                case PrimitiveTypeCode.Single:
                case PrimitiveTypeCode.Double:
                case PrimitiveTypeCode.Decimal:
                    return schemaType | JSchemaType.Number;
                case PrimitiveTypeCode.DateTime:
                case PrimitiveTypeCode.DateTimeOffset:
                    return schemaType | JSchemaType.String;
                case PrimitiveTypeCode.String:
                case PrimitiveTypeCode.Uri:
                case PrimitiveTypeCode.Guid:
                case PrimitiveTypeCode.TimeSpan:
                case PrimitiveTypeCode.Bytes:
                    return schemaType | JSchemaType.String;
                default:
                    throw new JSchemaException("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
            }
        }
    }
}