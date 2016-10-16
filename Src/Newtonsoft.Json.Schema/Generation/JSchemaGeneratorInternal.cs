#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Generation
{
    internal class JSchemaGeneratorInternal
    {
        internal readonly JSchemaGenerator _generator;
        private readonly List<TypeSchema> _typeSchemas;

        public JSchemaGeneratorInternal(JSchemaGenerator generator)
        {
            _generator = generator;
            _typeSchemas = new List<TypeSchema>();
        }

        public JSchema Generate(Type type, Required required)
        {
            JSchema schema = GenerateInternal(type, required, null, null, null);

            if (_generator.SchemaLocationHandling == SchemaLocationHandling.Definitions)
            {
                if (_typeSchemas.Count > 1)
                {
                    JToken definitions;
                    if (!schema.ExtensionData.TryGetValue(Constants.PropertyNames.Definitions, out definitions))
                    {
                        definitions = new JObject();
                        schema.ExtensionData[Constants.PropertyNames.Definitions] = definitions;
                    }

                    // reverse schemas so schemas are added to definition in the order they were generated in
                    // means schemas with few/no references to other schemas are first
                    // avoids definitions that are just a reference to somewhere else
                    List<TypeSchema> reversedTypeSchemas = Enumerable.Reverse(_typeSchemas).ToList();

                    foreach (TypeSchema t in reversedTypeSchemas)
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
                            string typeName = GetTypeName(t.Key.Type);

                            id = typeName;
                            int i = 1;
                            while (definitions[id] != null)
                            {
                                id = typeName + "-" + i;
                                i++;
                            }
                        }

                        definitions[id] = t.Schema;
                    }
                }
            }

            return schema;
        }

        private string GetTypeName(Type type)
        {
#if (PORTABLE || NETSTANDARD1_3)
            if (type.GetTypeInfo().IsGenericType)
#else
            if (type.IsGenericType)
#endif
            {
#if (PORTABLE || NETSTANDARD1_3)
                Type[] genericTypeArguments = type.GetTypeInfo().GenericTypeArguments;
#else
                Type[] genericTypeArguments = type.GetGenericArguments();
#endif

                return type.Name.Split('`')[0] + "<" + string.Join(", ", genericTypeArguments.Select(GetTypeName).ToArray()) + ">";
            }

            return type.Name;
        }

        public JSchema GenerateSubschema(Type type, Required required, JSchemaGenerationProvider currentGenerationProvider)
        {
            JSchema schema = GenerateInternal(type, required, null, null, currentGenerationProvider);

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

                switch (_generator.SchemaIdGenerationHandling)
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

        private JSchema GenerateInternal(Type type, Required? valueRequired, JsonProperty memberProperty, JsonContainerContract container, JSchemaGenerationProvider currentGenerationProvider)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));

            Type nonNullableType = ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;

            Uri explicitId = GetTypeId(nonNullableType, true);

            JsonContract contract = _generator.ContractResolver.ResolveContract(type);

            Required resolvedRequired = valueRequired ?? _generator.DefaultRequired;

            TypeSchemaKey key = CreateKey(resolvedRequired, memberProperty, contract);

            if (ShouldReferenceType(contract))
            {
                TypeSchema typeSchema = GetCachedSchema(key);
                if (typeSchema != null)
                {
                    return typeSchema.Schema;
                }
            }

            JSchema schema = null;

            JSchemaGenerationProvider provider = ResolveTypeProvider(nonNullableType, memberProperty);
            if (provider != null && provider != currentGenerationProvider)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, resolvedRequired, memberProperty, container, this);
                context.GenerationProvider = provider;

                schema = provider.GetSchema(context);

                if (schema == null)
                {
                    throw new JSchemaException("Could not get schema for type '{0}' from provider '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.FullName, provider.GetType().FullName));
                }
            }

            if (_generator._generationProviders != null)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(type, resolvedRequired, memberProperty, container, this);

                foreach (JSchemaGenerationProvider generationProvider in _generator._generationProviders)
                {
                    if (generationProvider != currentGenerationProvider && generationProvider.CanGenerateSchema(context))
                    {
                        context.GenerationProvider = generationProvider;

                        schema = generationProvider.GetSchema(context);
                        break;
                    }
                }
            }

            if (schema != null)
            {
                // check to see whether the generation provide had already generated the type recursively
                // and reuse that cached schema rather than duplicate
                TypeSchema typeSchema = GetCachedSchema(key);

                if (typeSchema != null)
                {
                    schema = typeSchema.Schema;
                }
                else
                {
                    if (ShouldReferenceType(contract))
                    {
                        _typeSchemas.Add(new TypeSchema(key, schema));
                    }
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

        private TypeSchema GetCachedSchema(TypeSchemaKey key)
        {
            return _typeSchemas.SingleOrDefault(s => s.Key.Equals(key));
        }

        private bool ShouldReferenceType(JsonContract contract)
        {
            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                    if (contract.NonNullableUnderlyingType == typeof(object))
                    {
                        return false;
                    }

                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Objects) == SchemaReferenceHandling.Objects;
                case JsonContractType.Array:
                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Arrays) == SchemaReferenceHandling.Arrays;
                case JsonContractType.Dictionary:
                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Dictionaries) == SchemaReferenceHandling.Dictionaries;
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
                    throw new ArgumentOutOfRangeException(nameof(valueRequired));
            }

            TypeSchemaKey key = new TypeSchemaKey(contract.UnderlyingType, resolvedRequired, minLength, maxLength);

            return key;
        }

        private void PopulateSchema(JSchema schema, JsonContract contract, JsonProperty memberProperty, Required valueRequired)
        {
            schema.Title = GetTitle(contract.NonNullableUnderlyingType);
            schema.Description = GetDescription(contract.NonNullableUnderlyingType);

            JsonConverter converter;
            if (contract.Converter != null && contract.Converter.CanWrite)
            {
                converter = contract.Converter;
            }
            else if (contract.InternalConverter != null && contract.InternalConverter.CanWrite)
            {
                converter = contract.InternalConverter;
            }
            else
            {
                converter = null;
            }

            if (converter != null)
            {
                schema.Type = null;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        if (contract.NonNullableUnderlyingType == typeof(object))
                        {
                            PopulatePrimativeSchema(schema, contract, memberProperty, valueRequired);
                        }
                        else
                        {
                            if (schema.Id == null)
                            {
                                schema.Id = GetTypeId(contract.NonNullableUnderlyingType, false);
                            }

                            schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                            GenerateObjectSchema(schema, contract.NonNullableUnderlyingType, (JsonObjectContract) contract);
                        }
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
                            schema.Items.Add(GenerateInternal(collectionItemType, required, null, (JsonArrayContract)contract, null));
                        }
                        break;
                    case JsonContractType.Primitive:
                        PopulatePrimativeSchema(schema, contract, memberProperty, valueRequired);
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
                            JsonContract keyContract = _generator.ContractResolver.ResolveContract(keyType);

                            // can be converted to a string
                            if (keyContract.ContractType == JsonContractType.Primitive)
                            {
                                schema.AdditionalProperties = GenerateInternal(valueType, _generator.DefaultRequired, null, (JsonDictionaryContract)contract, null);
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

        private void PopulatePrimativeSchema(JSchema schema, JsonContract contract, JsonProperty memberProperty, Required valueRequired)
        {
            JSchemaType type = GetJSchemaType(contract.UnderlyingType, valueRequired);

            if (type != Constants.AnyType)
            {
                schema.Type = GetJSchemaType(contract.UnderlyingType, valueRequired);
            }

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
            if (JSchemaTypeHelpers.HasFlag(type, JSchemaType.Number) || JSchemaTypeHelpers.HasFlag(type, JSchemaType.Integer))
            {
                double minimum;
                double maximum;
                if (DataAnnotationHelpers.GetRange(memberProperty, out minimum, out maximum))
                {
                    schema.Minimum = minimum;
                    schema.Maximum = maximum;
                }
            }

            if (JSchemaTypeHelpers.HasFlag(type, JSchemaType.Integer)
                && contract.NonNullableUnderlyingType.IsEnum()
                && ReflectionUtils.GetAttribute<FlagsAttribute>(contract.NonNullableUnderlyingType) == null)
            {
                if ((type & JSchemaType.Null) == JSchemaType.Null)
                {
                    schema.Enum.Add(JValue.CreateNull());
                }

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
                    JToken value = (JSchemaTypeHelpers.HasFlag(type, JSchemaType.String))
                        ? enumValue.Name
                        : JToken.FromObject(enumValue.Value);

                    schema.Enum.Add(value);
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
            if (_generator.SchemaPropertyOrderHandling == SchemaPropertyOrderHandling.Alphabetical)
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

                    JSchema propertySchema = GenerateInternal(property.PropertyType, required, property, contract, null);

                    if (property.DefaultValue != null)
                    {
                        propertySchema.Default = JToken.FromObject(property.DefaultValue);
                    }

                    schema.Properties.Add(property.PropertyName, propertySchema);

                    Required resolvedRequired = required ?? _generator.DefaultRequired;
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
                    return schemaType | JSchemaType.String | JSchemaType.Boolean | JSchemaType.Integer | JSchemaType.Number | JSchemaType.Object | JSchemaType.Array;
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
