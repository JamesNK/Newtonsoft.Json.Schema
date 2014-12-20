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
        private readonly List<ExternalSchema> _externalSchemas;
        private readonly List<KnownSchema> _knownSchemas;
        private readonly List<TypeSchema> _typeSchemas;

        private JSchemaResolver _resolver;
        private IContractResolver _contractResolver;

        /// <summary>
        /// Gets or sets how undefined schemas are handled by the serializer.
        /// </summary>
        public JSchemaUndefinedIdHandling UndefinedSchemaIdHandling { get; set; }

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

        public IList<ExternalSchema> ExternalSchemas
        {
            get { return _externalSchemas; }
        }

        public JSchemaGenerator()
        {
            _externalSchemas = new List<ExternalSchema>();
            _knownSchemas = new List<KnownSchema>();
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

            _knownSchemas.Clear();
            foreach (ExternalSchema externalSchema in _externalSchemas)
            {
                JSchemaDiscovery discovery = new JSchemaDiscovery(_knownSchemas, KnownSchemaState.External);
                discovery.Discover(externalSchema.Schema, null);
            }

            return GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default);
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

        private JSchema GenerateInternal(Type type, Required valueRequired)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            Uri resolvedId = GetTypeId(type, false);
            Uri explicitId = GetTypeId(type, true);

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

            JsonContract contract = ContractResolver.ResolveContract(type);
            JsonConverter converter;
            if ((converter = contract.Converter) != null || (converter = contract.InternalConverter) != null)
            {
                //JSchema converterSchema = converter.GetSchema();
                //if (converterSchema != null)
                //    return converterSchema;
            }

            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                case JsonContractType.Array:
                case JsonContractType.Dictionary:
                    TypeSchema typeSchema = _typeSchemas.SingleOrDefault(s => s.Type == type && s.Required == valueRequired);
                    if (typeSchema != null)
                        return typeSchema.Schema;
                    break;
            }

            JSchema schema = new JSchema();
            if (explicitId != null)
                schema.Id = explicitId;

            _typeSchemas.Add(new TypeSchema(type, valueRequired, schema));
            _knownSchemas.Add(new KnownSchema(schema.Id, schema, KnownSchemaState.External));

            schema.Title = GetTitle(type);
            schema.Description = GetDescription(type);

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
                            schema.Id = GetTypeId(type, false);

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        GenerateObjectSchema(schema, type, (JsonObjectContract)contract);
                        break;
                    case JsonContractType.Array:
                        if (schema.Id == null)
                            schema.Id = GetTypeId(type, false);

                        schema.Type = AddNullType(JSchemaType.Array, valueRequired);

                        JsonArrayAttribute arrayAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(type);
                        bool allowNullItem = (arrayAttribute == null || arrayAttribute.AllowNullItems);

                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
                        if (collectionItemType != null)
                        {
                            schema.Items.Add(GenerateInternal(collectionItemType, (!allowNullItem) ? Required.Always : Required.Default));
                        }
                        break;
                    case JsonContractType.Primitive:
                        schema.Type = GetJSchemaType(type, valueRequired);

                        if (schema.Type == JSchemaType.Integer && type.IsEnum() && !IsDefined(type, typeof(FlagsAttribute), true))
                        {
                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(type);
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
                                schema.AdditionalProperties = GenerateInternal(valueType, Required.Default);
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

                    JSchema propertySchema = GenerateInternal(property.PropertyType, property.Required);

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
            if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
            {
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