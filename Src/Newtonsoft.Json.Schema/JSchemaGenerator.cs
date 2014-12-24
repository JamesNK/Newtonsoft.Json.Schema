#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Generates a <see cref="JSchema"/> from a specified <see cref="Type"/>.
    /// </summary>
    public class JSchemaGenerator
    {
        //private static ReflectionObject _regularExpressionAttributeReflectionObject;

        private readonly List<TypeSchema> _typeSchemas;

        private JSchemaResolver _resolver;
        private IContractResolver _contractResolver;
        private IList<JSchemaGenerationProvider> _generationProviders;

        /// <summary>
        /// Gets or sets how undefined schemas are handled by the serializer.
        /// </summary>
        public JSchemaUndefinedIdHandling UndefinedSchemaIdHandling { get; set; }

        public IList<JSchemaGenerationProvider> GenerationProviders
        {
            get
            {
                if (_generationProviders == null)
                    _generationProviders = new List<JSchemaGenerationProvider>();

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
                    return DefaultContractResolver.Instance;

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
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type)
        {
            return Generate(type, DummyJSchemaResolver.Instance, false);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="resolver">The <see cref="JSchemaResolver"/> used to resolve schema references.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type, JSchemaResolver resolver)
        {
            return Generate(type, resolver, false);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="JSchema"/> will be nullable.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type, bool rootSchemaNullable)
        {
            return Generate(type, DummyJSchemaResolver.Instance, rootSchemaNullable);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="resolver">The <see cref="JSchemaResolver"/> used to resolve schema references.</param>
        /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="JSchema"/> will be nullable.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type, JSchemaResolver resolver, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            _resolver = resolver;

            return GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default, null, null);
        }

        private string GetTitle(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Title))
                return containerAttribute.Title;

            return null;
        }

        private string GetDescription(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Description))
                return containerAttribute.Description;

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            DescriptionAttribute descriptionAttribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
            if (descriptionAttribute != null)
                return descriptionAttribute.Description;
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
                    return null;

                switch (UndefinedSchemaIdHandling)
                {
                    case JSchemaUndefinedIdHandling.UseTypeName:
                        typeId = new Uri(type.FullName, UriKind.RelativeOrAbsolute);
                        break;
                    case JSchemaUndefinedIdHandling.UseAssemblyQualifiedName:
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

        private static bool IsDefined(Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == attributeType);
        }

        private JSchema GenerateInternal(Type type, Required valueRequired, JsonProperty memberProperty, JsonContainerContract container)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            Type nonNullableType = ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;

            Uri resolvedId = GetTypeId(nonNullableType, false);
            Uri explicitId = GetTypeId(nonNullableType, true);

            if (resolvedId != null)
            {
                JSchema resolvedSchema = _resolver.GetSchema(resolvedId);
                if (resolvedSchema != null)
                {
                    // resolved schema is not null but referencing member allows nulls
                    // change resolved schema to allow nulls. hacky but what are ya gonna do?
                    if (valueRequired != Required.Always && !HasFlag(resolvedSchema.Type, JSchemaType.Null))
                        resolvedSchema.Type |= JSchemaType.Null;

                    return resolvedSchema;
                }
            }

            JSchemaGenerationProviderAttribute providerAttribute = null;
            if (memberProperty != null && memberProperty.AttributeProvider != null)
            {
                providerAttribute = (JSchemaGenerationProviderAttribute)memberProperty.AttributeProvider.GetAttributes(typeof(JSchemaGenerationProviderAttribute), true).SingleOrDefault();
            }
            if (providerAttribute == null)
            {
                providerAttribute = ReflectionUtils.GetAttribute<JSchemaGenerationProviderAttribute>(nonNullableType, true);
            }

            if (providerAttribute != null)
            {
                JSchemaGenerationProvider provider = (JSchemaGenerationProvider)Activator.CreateInstance(providerAttribute.ProviderType, providerAttribute.ProviderParameters);

                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, valueRequired, null, null, this);

                return provider.GetSchema(context);
            }

            JsonContract contract = ContractResolver.ResolveContract(type);

            TypeSchemaKey key = new TypeSchemaKey(type, valueRequired, null, null);

            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                case JsonContractType.Array:
                case JsonContractType.Dictionary:
                    TypeSchema typeSchema = _typeSchemas.SingleOrDefault(s => s.Key.Equals(key) && s.Key.Required == valueRequired);
                    if (typeSchema != null)
                        return typeSchema.Schema;
                    break;
            }

            JSchema schema = null;

            if (_generationProviders != null)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, valueRequired, null, null, this);

                foreach (JSchemaGenerationProvider generationProvider in _generationProviders)
                {
                    schema = generationProvider.GetSchema(context);
                }
            }

            if (schema != null)
            {
                _typeSchemas.Add(new TypeSchema(key, schema));
                return schema;
            }

            schema = new JSchema();
            if (explicitId != null)
                schema.Id = explicitId;

            _typeSchemas.Add(new TypeSchema(key, schema));

            schema.Title = GetTitle(nonNullableType);
            schema.Description = GetDescription(nonNullableType);

            JsonConverter converter = contract.Converter ?? contract.InternalConverter;

            if (converter != null)
            {
                // todo: Add GetSchema to JsonConverter and use here?
                schema.Type = JSchemaType.Any;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        if (schema.Id == null)
                            schema.Id = GetTypeId(nonNullableType, false);

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        GenerateObjectSchema(schema, type, (JsonObjectContract)contract);
                        break;
                    case JsonContractType.Array:
                        if (schema.Id == null)
                            schema.Id = GetTypeId(nonNullableType, false);

                        schema.Type = AddNullType(JSchemaType.Array, valueRequired);

                        JsonArrayAttribute arrayAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(nonNullableType);
                        bool allowNullItem = (arrayAttribute == null || arrayAttribute.AllowNullItems);

                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
                        if (collectionItemType != null)
                        {
                            schema.Items.Add(GenerateInternal(collectionItemType, (!allowNullItem) ? Required.Always : Required.Default, null, (JsonArrayContract)contract));
                        }
                        break;
                    case JsonContractType.Primitive:
                        schema.Type = GetJSchemaType(type, valueRequired);

                        if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Integer) && nonNullableType.IsEnum() && !IsDefined(nonNullableType, typeof(FlagsAttribute), true))
                        {
                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(nonNullableType);
                            foreach (EnumValue<long> enumValue in enumValues)
                            {
                                JToken value = JToken.FromObject(enumValue.Value);

                                schema.Enum.Add(value);
                            }
                        }
                        break;
                    case JsonContractType.String:
                        JSchemaType schemaType = (!ReflectionUtils.IsNullable(contract.UnderlyingType))
                            ? JSchemaType.String
                            : AddNullType(JSchemaType.String, valueRequired);

                        schema.Type = schemaType;
                        break;
                    case JsonContractType.Dictionary:
                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);

                        Type keyType;
                        Type valueType;
                        ReflectionUtils.GetDictionaryKeyValueTypes(type, out keyType, out valueType);

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
                            schema.Id = GetTypeId(type, false);

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        schema.AllowAdditionalProperties = true;
                        break;
                    case JsonContractType.Dynamic:
                    case JsonContractType.Linq:
                        schema.Type = JSchemaType.Any;
                        break;
                    default:
                        throw new JsonException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
                }
            }

            return schema;
        }

        private JSchemaType AddNullType(JSchemaType type, Required valueRequired)
        {
            if (valueRequired != Required.Always)
                return type | JSchemaType.Null;

            return type;
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
        {
            return ((value & flag) == flag);
        }

        private Attribute GetAttributeByName(JsonProperty property, string name)
        {
            if (property.AttributeProvider != null)
            {
                var attributes = property.AttributeProvider.GetAttributes(true);
                foreach (Attribute attribute in attributes)
                {
                    if (string.Equals(attribute.GetType().FullName, name, StringComparison.Ordinal))
                    {
                        return attribute;
                    }
                }
            }

            return null;
        }

        private void GenerateObjectSchema(JSchema schema, Type type, JsonObjectContract contract)
        {
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    bool optional = property.NullValueHandling == NullValueHandling.Ignore ||
                                    HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) ||
                                    property.ShouldSerialize != null ||
                                    property.GetIsSpecified != null;

                    Required required = property.Required;
                    if (GetAttributeByName(property, "System.ComponentModel.DataAnnotations.RequiredAttribute") != null)
                        required = Required.Always;

                    JSchema propertySchema = GenerateInternal(property.PropertyType, required, property, contract);

                    if (property.DefaultValue != null)
                        propertySchema.Default = JToken.FromObject(property.DefaultValue);

                    schema.Properties.Add(property.PropertyName, propertySchema);

                    if (!optional)
                        schema.Required.Add(property.PropertyName);
                }
            }

            if (type.IsSealed())
                schema.AllowAdditionalProperties = false;
        }

        internal static bool HasFlag(JSchemaType? value, JSchemaType flag)
        {
            // default value is Any
            if (value == null)
                return true;

            bool match = ((value & flag) == flag);
            if (match)
                return true;

            // integer is a subset of float
            if (flag == JSchemaType.Integer && (value & JSchemaType.Float) == JSchemaType.Float)
                return true;

            return false;
        }

        private JSchemaType GetJSchemaType(Type type, Required valueRequired)
        {
            JSchemaType schemaType = JSchemaType.None;
            if (ReflectionUtils.IsNullable(type))
            {
                if (valueRequired != Required.Always)
                    schemaType = JSchemaType.Null;

                if (ReflectionUtils.IsNullableType(type))
                    type = Nullable.GetUnderlyingType(type);
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
                    return schemaType | JSchemaType.Float;
                    // convert to string?
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
                    throw new JsonException("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
            }
        }
    }
}