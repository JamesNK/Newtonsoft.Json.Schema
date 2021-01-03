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
                    // TODO: Support $defs
                    if (!schema.ExtensionData.TryGetValue(Constants.PropertyNames.Definitions, out JToken definitions))
                    {
                        definitions = new JObject();
                        schema.ExtensionData[Constants.PropertyNames.Definitions] = definitions;
                    }

                    Dictionary<string, JSchema> definitionsSchemas = new Dictionary<string, JSchema>();

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
                            string typeName = GetTypeName(t.Key.Type);

                            id = typeName;
                            int i = 1;
                            while (definitionsSchemas.ContainsKey(id))
                            {
                                id = typeName + "-" + i;
                                i++;
                            }
                        }

                        definitionsSchemas[id] = t.Schema;
                    }

                    // definition schemas alphabetical ordered
                    foreach (KeyValuePair<string, JSchema> definitionSchema in definitionsSchemas.OrderBy(s => s.Key))
                    {
                        definitions[definitionSchema.Key] = definitionSchema.Value;
                    }
                }
            }

            return schema;
        }

        private string GetTypeName(Type type)
        {
#if (PORTABLE || NETSTANDARD1_3 || NETSTANDARD2_0)
            if (type.GetTypeInfo().IsGenericType)
#else
            if (type.IsGenericType)
#endif
            {
#if (PORTABLE || NETSTANDARD1_3 || NETSTANDARD2_0)
                Type[] genericTypeArguments = type.GetTypeInfo().GenericTypeArguments;
#else
                Type[] genericTypeArguments = type.GetGenericArguments();
#endif

                return type.Name.Split('`')[0] + "<" + string.Join(", ", genericTypeArguments.Select(GetTypeName)
#if NET35
                    .ToArray()
#endif
                    ) + ">";
            }

            return type.Name;
        }

        public JSchema GenerateSubschema(Type type, Required required, JSchemaGenerationProvider? currentGenerationProvider)
        {
            JSchema schema = GenerateInternal(type, required, null, null, currentGenerationProvider);

            return schema;
        }

        private string? GetTitle(Type type, JsonProperty? memberProperty)
        {
            JsonContainerAttribute? containerAttribute = ReflectionUtils.GetAttribute<JsonContainerAttribute>(type);
            if (!string.IsNullOrEmpty(containerAttribute?.Title))
            {
                return containerAttribute!.Title;
            }

            AttributeHelpers.GetDisplayName(type, memberProperty, out string? displayName);
            return displayName;
        }

        private string? GetDescription(Type type, JsonProperty? memberProperty)
        {
            JsonContainerAttribute? containerAttribute = ReflectionUtils.GetAttribute<JsonContainerAttribute>(type);
            if (!string.IsNullOrEmpty(containerAttribute?.Description))
            {
                return containerAttribute!.Description;
            }

            AttributeHelpers.GetDescription(type, memberProperty, out string? description);
            return description;
        }

        private Uri? GetTypeId(Type type, bool explicitOnly)
        {
            JsonContainerAttribute? containerAttribute = ReflectionUtils.GetAttribute<JsonContainerAttribute>(type);

            Uri typeId;

            if (!string.IsNullOrEmpty(containerAttribute?.Id))
            {
                typeId = new Uri(containerAttribute!.Id, UriKind.RelativeOrAbsolute);
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

        private JSchemaGenerationProvider? ResolveTypeProvider(Type nonNullableType, JsonProperty? memberProperty)
        {
            JSchemaGenerationProviderAttribute? providerAttribute = null;

            if (memberProperty?.AttributeProvider != null)
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

        private JSchema GenerateInternal(Type type, Required? valueRequired, JsonProperty? memberProperty, JsonContainerContract? container, JSchemaGenerationProvider? currentGenerationProvider)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));

            Type nonNullableType = ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;

            Uri? explicitId = GetTypeId(nonNullableType, true);

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

            JSchema? schema = null;

            JSchemaGenerationProvider? provider = ResolveTypeProvider(nonNullableType, memberProperty);
            if (provider != null && provider != currentGenerationProvider)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(
                    type,
                    resolvedRequired,
                    memberProperty,
                    container,
                    this,
                    GetTitle(type, memberProperty),
                    GetDescription(type, memberProperty));
                context.GenerationProvider = provider;

                schema = provider.GetSchema(context);

                if (schema == null)
                {
                    throw new JSchemaException("Could not get schema for type '{0}' from provider '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.FullName, provider.GetType().FullName));
                }
            }

            if (_generator._generationProviders != null)
            {
                JSchemaTypeGenerationContext context = new JSchemaTypeGenerationContext(
                    type,
                    resolvedRequired,
                    memberProperty,
                    container,
                    this,
                    GetTitle(type, memberProperty),
                    GetDescription(type, memberProperty));

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
            switch (contract)
            {
                case JsonObjectContract objectContract:
                    if (objectContract.UnderlyingType == typeof(object))
                    {
                        return false;
                    }

                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Objects) == SchemaReferenceHandling.Objects;
                case JsonArrayContract arrayContract:
                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Arrays) == SchemaReferenceHandling.Arrays;
                case JsonDictionaryContract dictionaryContract:
                    return (_generator.SchemaReferenceHandling & SchemaReferenceHandling.Dictionaries) == SchemaReferenceHandling.Dictionaries;
                default:
                    return false;
            }
        }

        private TypeSchemaKey CreateKey(Required valueRequired, JsonProperty? memberProperty, JsonContract contract)
        {
            Type nonNullableUnderlyingType = GetNonNullableUnderlyingType(contract);

            int? minLength = AttributeHelpers.GetMinLength(memberProperty);
            int? maxLength = AttributeHelpers.GetMaxLength(memberProperty);
            string? title = GetTitle(nonNullableUnderlyingType, memberProperty);
            string? description = GetDescription(nonNullableUnderlyingType, memberProperty);

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

            TypeSchemaKey key = new TypeSchemaKey(contract.UnderlyingType, resolvedRequired, minLength, maxLength, title, description);

            return key;
        }

        private void PopulateSchema(JSchema schema, JsonContract contract, JsonProperty? memberProperty, Required valueRequired)
        {
            Type nonNullableUnderlyingType = GetNonNullableUnderlyingType(contract);

            schema.Title = GetTitle(nonNullableUnderlyingType, memberProperty);
            schema.Description = GetDescription(nonNullableUnderlyingType, memberProperty);

            JsonConverter? converter;
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
                switch (contract)
                {
                    case JsonObjectContract objectContract:
                        if (nonNullableUnderlyingType == typeof(object))
                        {
                            PopulatePrimativeSchema(schema, objectContract, memberProperty, valueRequired);
                        }
                        else
                        {
                            if (schema.Id == null)
                            {
                                schema.Id = GetTypeId(nonNullableUnderlyingType, false);
                            }

                            schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                            GenerateObjectSchema(schema, nonNullableUnderlyingType, objectContract);
                        }
                        break;
                    case JsonArrayContract arrayContract:
                        if (schema.Id == null)
                        {
                            schema.Id = GetTypeId(nonNullableUnderlyingType, false);
                        }

                        schema.Type = AddNullType(JSchemaType.Array, valueRequired);
                        schema.MinimumItems = AttributeHelpers.GetMinLength(memberProperty);
                        schema.MaximumItems = AttributeHelpers.GetMaxLength(memberProperty);

                        JsonArrayAttribute? arrayAttribute = ReflectionUtils.GetAttribute<JsonArrayAttribute>(nonNullableUnderlyingType);

                        Required? required = null;
                        if (arrayAttribute != null && !arrayAttribute.AllowNullItems)
                        {
                            required = Required.Always;
                        }

                        if (arrayContract.CollectionItemType != null)
                        {
                            schema.Items.Add(GenerateInternal(arrayContract.CollectionItemType, required, null, arrayContract, null));
                        }
                        break;
                    case JsonStringContract stringContract:
                        JSchemaType schemaType = (!ReflectionUtils.IsNullable(stringContract.UnderlyingType))
                            ? JSchemaType.String
                            : AddNullType(JSchemaType.String, valueRequired);

                        schema.Type = schemaType;
                        schema.MinimumLength = AttributeHelpers.GetMinLength(memberProperty);
                        schema.MaximumLength = AttributeHelpers.GetMaxLength(memberProperty);
                        break;
                    case JsonPrimitiveContract primitiveContract:
                        PopulatePrimativeSchema(schema, primitiveContract, memberProperty, valueRequired);
                        break;
                    case JsonDictionaryContract dictionaryContract:
                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        schema.MinimumProperties = AttributeHelpers.GetMinLength(memberProperty);
                        schema.MaximumProperties = AttributeHelpers.GetMaxLength(memberProperty);

                        if (dictionaryContract.DictionaryKeyType != null)
                        {
                            JsonContract keyContract = _generator.ContractResolver.ResolveContract(dictionaryContract.DictionaryKeyType);

                            // can be converted to a string
                            if (keyContract is JsonPrimitiveContract)
                            {
                                schema.AdditionalProperties = GenerateInternal(dictionaryContract.DictionaryValueType, _generator.DefaultRequired, null, dictionaryContract, null);
                            }
                        }
                        break;
#if !PORTABLE || NETSTANDARD1_3 || NETSTANDARD2_0
                    case JsonISerializableContract serializableContract:
                        if (schema.Id == null)
                        {
                            schema.Id = GetTypeId(nonNullableUnderlyingType, false);
                        }

                        schema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        schema.AllowAdditionalPropertiesSpecified = false;
                        break;
#endif
#if !NET35
                    case JsonDynamicContract dynamicContract:
#endif
                    case JsonLinqContract linqContract:
                        schema.Type = null;
                        break;
                    default:
                        throw new JSchemaException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
                }
            }
        }

        private void PopulatePrimativeSchema(JSchema schema, JsonContract contract, JsonProperty? memberProperty, Required valueRequired)
        {
            Type nonNullableUnderlyingType = GetNonNullableUnderlyingType(contract);
            JSchemaType type = GetJSchemaType(contract.UnderlyingType, valueRequired);

            if (type != Constants.AnyType)
            {
                schema.Type = GetJSchemaType(contract.UnderlyingType, valueRequired);
            }

            if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.String))
            {
                if (AttributeHelpers.GetStringLength(memberProperty, out int minimumLength, out int maximumLength))
                {
                    schema.MinimumLength = minimumLength;
                    schema.MaximumLength = maximumLength;
                }
                else
                {
                    schema.MinimumLength = AttributeHelpers.GetMinLength(memberProperty);
                    schema.MaximumLength = AttributeHelpers.GetMaxLength(memberProperty);
                }

                schema.Pattern = AttributeHelpers.GetPattern(memberProperty);
                schema.Format = AttributeHelpers.GetFormat(memberProperty);

                // no format specified, derive from type
                if (schema.Format == null)
                {
                    if (nonNullableUnderlyingType == typeof(DateTime)
                        || nonNullableUnderlyingType == typeof(DateTimeOffset))
                    {
                        schema.Format = Constants.Formats.DateTime;
                    }
                    else if (nonNullableUnderlyingType == typeof(Uri))
                    {
                        schema.Format = Constants.Formats.Uri;
                    }
                }
            }
            if (JSchemaTypeHelpers.HasFlag(type, JSchemaType.Number) || JSchemaTypeHelpers.HasFlag(type, JSchemaType.Integer))
            {
                if (AttributeHelpers.GetRange(memberProperty, out double minimum, out double maximum))
                {
                    schema.Minimum = minimum;
                    schema.Maximum = maximum;
                }
            }

            if (JSchemaTypeHelpers.HasFlag(type, JSchemaType.Integer)
                && nonNullableUnderlyingType.IsEnum()
                && ReflectionUtils.GetAttribute<FlagsAttribute>(nonNullableUnderlyingType) == null)
            {
                if ((type & JSchemaType.Null) == JSchemaType.Null)
                {
                    schema.Enum.Add(JValue.CreateNull());
                }

                EnumInfo enumValues = EnumUtils.GetEnumValuesAndNames(nonNullableUnderlyingType, null);
                for (int i = 0; i < enumValues.Values.Length; i++)
                {
                    JToken value = JToken.FromObject(Enum.ToObject(nonNullableUnderlyingType, enumValues.Values[i]));

                    schema.Enum.Add(value);
                }
            }

            Type? enumDataType = AttributeHelpers.GetEnumDataType(memberProperty);
            if (enumDataType != null && CollectionHelpers.IsNullOrEmpty(schema._enum))
            {
                EnumInfo enumValues = EnumUtils.GetEnumValuesAndNames(enumDataType, null);
                for (int i = 0; i < enumValues.Values.Length; i++)
                {
                    JToken value = (JSchemaTypeHelpers.HasFlag(type, JSchemaType.String))
                        ? enumValues.ResolvedNames[i]
                        : JToken.FromObject(enumValues.Values[i]);

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
            IEnumerable<JsonProperty> properties;
            if (_generator.SchemaPropertyOrderHandling == SchemaPropertyOrderHandling.Alphabetical)
            {
                properties = contract.Properties
                    .OrderBy(p => p.Order ?? 0)
                    .ThenBy(p => p.PropertyName);
            }
            else
            {
                properties = contract.Properties;
            }

            foreach (JsonProperty property in properties)
            {
                if (!property.Ignored)
                {
                    Required? required = property.IsRequiredSpecified ? (Required?)property.Required : null;
                    if (AttributeHelpers.GetRequired(property))
                    {
                        required = Required.Always;
                    }

                    JSchema propertySchema = GenerateInternal(property.PropertyType, required, property, contract, null);

                    // the default value might have already been set by the schema generation provider
                    if (property.DefaultValue != null && propertySchema.Default == null)
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

        private Type GetNonNullableUnderlyingType(JsonContract contract)
        {
            return (ReflectionUtils.IsNullable(contract.UnderlyingType) && ReflectionUtils.IsNullableType(contract.UnderlyingType))
                ? Nullable.GetUnderlyingType(contract.UnderlyingType)
                : contract.UnderlyingType;
        }
    }
}
