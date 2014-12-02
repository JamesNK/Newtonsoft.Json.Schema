#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Generates a <see cref="JSchema"/> from a specified <see cref="Type"/>.
    /// </summary>
    public class JSchemaGenerator
    {
        /// <summary>
        /// Gets or sets how undefined schemas are handled by the serializer.
        /// </summary>
        public JSchemaUndefinedIdHandling UndefinedSchemaIdHandling { get; set; }

        private IContractResolver _contractResolver;

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

        private class TypeSchema
        {
            public Type Type { get; private set; }
            public JSchema Schema { get; private set; }

            public TypeSchema(Type type, JSchema schema)
            {
                ValidationUtils.ArgumentNotNull(type, "type");
                ValidationUtils.ArgumentNotNull(schema, "schema");

                Type = type;
                Schema = schema;
            }
        }

        private JSchemaResolver _resolver;
        private readonly IList<TypeSchema> _stack = new List<TypeSchema>();
        private JSchema _currentSchema;

        private JSchema CurrentSchema
        {
            get { return _currentSchema; }
        }

        private void Push(TypeSchema typeSchema)
        {
            _currentSchema = typeSchema.Schema;
            _stack.Add(typeSchema);
            _resolver.LoadedSchemas.Add(typeSchema.Schema);
        }

        private TypeSchema Pop()
        {
            TypeSchema popped = _stack[_stack.Count - 1];
            _stack.RemoveAt(_stack.Count - 1);
            TypeSchema newValue = _stack.LastOrDefault();
            if (newValue != null)
            {
                _currentSchema = newValue.Schema;
            }
            else
            {
                _currentSchema = null;
            }

            return popped;
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type)
        {
            return Generate(type, new JSchemaResolver(), false);
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
            return Generate(type, new JSchemaResolver(), rootSchemaNullable);
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

            return GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default, false);
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

        private string GetTypeId(Type type, bool explicitOnly)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Id))
                return containerAttribute.Id;

            if (explicitOnly)
                return null;

            switch (UndefinedSchemaIdHandling)
            {
                case JSchemaUndefinedIdHandling.UseTypeName:
                    return type.FullName;
                case JSchemaUndefinedIdHandling.UseAssemblyQualifiedName:
                    return type.AssemblyQualifiedName;
                default:
                    return null;
            }
        }

        private JSchema GenerateInternal(Type type, Required valueRequired, bool required)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            string resolvedId = GetTypeId(type, false);
            string explicitId = GetTypeId(type, true);

            if (!string.IsNullOrEmpty(resolvedId))
            {
                JSchema resolvedSchema = _resolver.GetSchema(resolvedId);
                if (resolvedSchema != null)
                {
                    // resolved schema is not null but referencing member allows nulls
                    // change resolved schema to allow nulls. hacky but what are ya gonna do?
                    if (valueRequired != Required.Always && !HasFlag(resolvedSchema.Type, JSchemaType.Null))
                        resolvedSchema.Type |= JSchemaType.Null;
                    if (required && resolvedSchema.Required != true)
                        resolvedSchema.Required = true;

                    return resolvedSchema;
                }
            }

            // test for unresolved circular reference
            if (_stack.Any(tc => tc.Type == type))
            {
                throw new JsonException("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith(CultureInfo.InvariantCulture, type));
            }

            JsonContract contract = ContractResolver.ResolveContract(type);
            JsonConverter converter;
            if ((converter = contract.Converter) != null || (converter = contract.InternalConverter) != null)
            {
                //JSchema converterSchema = converter.GetSchema();
                //if (converterSchema != null)
                //    return converterSchema;
            }

            Push(new TypeSchema(type, new JSchema()));

            if (explicitId != null)
                CurrentSchema.Id = explicitId;

            if (required)
                CurrentSchema.Required = true;
            CurrentSchema.Title = GetTitle(type);
            CurrentSchema.Description = GetDescription(type);

            if (converter != null)
            {
                // todo: Add GetSchema to JsonConverter and use here?
                CurrentSchema.Type = JSchemaType.Any;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        CurrentSchema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        CurrentSchema.Id = GetTypeId(type, false);
                        GenerateObjectSchema(type, (JsonObjectContract)contract);
                        break;
                    case JsonContractType.Array:
                        CurrentSchema.Type = AddNullType(JSchemaType.Array, valueRequired);

                        CurrentSchema.Id = GetTypeId(type, false);

                        JsonArrayAttribute arrayAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(type);
                        bool allowNullItem = (arrayAttribute == null || arrayAttribute.AllowNullItems);

                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
                        if (collectionItemType != null)
                        {
                            CurrentSchema.Items = new List<JSchema>();
                            CurrentSchema.Items.Add(GenerateInternal(collectionItemType, (!allowNullItem) ? Required.Always : Required.Default, false));
                        }
                        break;
                    case JsonContractType.Primitive:
                        CurrentSchema.Type = GetJSchemaType(type, valueRequired);

                        if (CurrentSchema.Type == JSchemaType.Integer && type.IsEnum() && !type.IsDefined(typeof(FlagsAttribute), true))
                        {
                            CurrentSchema.Enum = new List<JToken>();

                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(type);
                            foreach (EnumValue<long> enumValue in enumValues)
                            {
                                JToken value = JToken.FromObject(enumValue.Value);

                                CurrentSchema.Enum.Add(value);
                            }
                        }
                        break;
                    case JsonContractType.String:
                        JSchemaType schemaType = (!ReflectionUtils.IsNullable(contract.UnderlyingType))
                            ? JSchemaType.String
                            : AddNullType(JSchemaType.String, valueRequired);

                        CurrentSchema.Type = schemaType;
                        break;
                    case JsonContractType.Dictionary:
                        CurrentSchema.Type = AddNullType(JSchemaType.Object, valueRequired);

                        Type keyType;
                        Type valueType;
                        ReflectionUtils.GetDictionaryKeyValueTypes(type, out keyType, out valueType);

                        if (keyType != null)
                        {
                            JsonContract keyContract = ContractResolver.ResolveContract(keyType);

                            // can be converted to a string
                            if (keyContract.ContractType == JsonContractType.Primitive)
                            {
                                CurrentSchema.AdditionalProperties = GenerateInternal(valueType, Required.Default, false);
                            }
                        }
                        break;
                    case JsonContractType.Serializable:
                        CurrentSchema.Type = AddNullType(JSchemaType.Object, valueRequired);
                        CurrentSchema.Id = GetTypeId(type, false);
                        CurrentSchema.AllowAdditionalProperties = true;
                        break;
                    case JsonContractType.Dynamic:
                    case JsonContractType.Linq:
                        CurrentSchema.Type = JSchemaType.Any;
                        break;
                    default:
                        throw new JsonException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
                }
            }

            return Pop().Schema;
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

        private void GenerateObjectSchema(Type type, JsonObjectContract contract)
        {
            CurrentSchema.Properties = new Dictionary<string, JSchema>();
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    bool optional = property.NullValueHandling == NullValueHandling.Ignore ||
                                    HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) ||
                                    property.ShouldSerialize != null ||
                                    property.GetIsSpecified != null;

                    JSchema propertySchema = GenerateInternal(property.PropertyType, property.Required, !optional);

                    if (property.DefaultValue != null)
                        propertySchema.Default = JToken.FromObject(property.DefaultValue);

                    CurrentSchema.Properties.Add(property.PropertyName, propertySchema);
                }
            }

            if (type.IsSealed())
                CurrentSchema.AllowAdditionalProperties = false;
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