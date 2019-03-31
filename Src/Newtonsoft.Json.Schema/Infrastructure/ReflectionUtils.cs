#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Schema.Infrastructure
{
#if (DOTNET || PORTABLE || PORTABLE40) && !NETSTANDARD2_0
    [Flags]
    internal enum MemberTypes
    {
        Event = 2,
        Field = 4,
        Method = 8,
        Property = 16
    }
#endif

#if PORTABLE && !NETSTANDARD2_0
    [Flags]
    internal enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192,
        PutDispProperty = 16384,
        ExactBinding = 65536,
        PutRefDispProperty = 32768,
        SuppressChangeType = 131072,
        OptionalParamBinding = 262144,
        IgnoreReturn = 16777216
    }
#endif

    internal class ReflectionUtils
    {
        public static void GetDictionaryKeyValueTypes(Type dictionaryType, out Type keyType, out Type valueType)
        {
            ValidationUtils.ArgumentNotNull(dictionaryType, nameof(dictionaryType));

            if (ImplementsGenericDefinition(dictionaryType, typeof(IDictionary<,>), out Type genericDictionaryType))
            {
                if (genericDictionaryType.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
                }

                Type[] dictionaryGenericArguments = genericDictionaryType.GetGenericArguments();

                keyType = dictionaryGenericArguments[0];
                valueType = dictionaryGenericArguments[1];
                return;
            }
            if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
            {
                keyType = null;
                valueType = null;
                return;
            }

            throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));
            ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, nameof(genericInterfaceDefinition));

            if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition())
            {
                throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture, genericInterfaceDefinition));
            }

            if (type.IsInterface())
            {
                if (type.IsGenericType())
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type i in type.GetInterfaces())
            {
                if (i.IsGenericType())
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }

        public static bool HasDefaultConstructor(Type t, bool nonPublic)
        {
            ValidationUtils.ArgumentNotNull(t, nameof(t));

            if (t.IsValueType())
            {
                return true;
            }

            return (GetDefaultConstructor(t, nonPublic) != null);
        }

        public static ConstructorInfo GetDefaultConstructor(Type t)
        {
            return GetDefaultConstructor(t, false);
        }

        public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (nonPublic)
            {
                bindingFlags = bindingFlags | BindingFlags.NonPublic;
            }

            return t.GetConstructors(bindingFlags).SingleOrDefault(c => !c.GetParameters().Any());
        }

        public static bool IsNullable(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, nameof(t));

            if (t.IsValueType())
            {
                return IsNullableType(t);
            }

            return true;
        }

        public static bool IsNullableType(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, nameof(t));

            return (t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static Type GetCollectionItemType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));

            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out Type genericListType))
            {
                if (genericListType.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
                }

                return genericListType.GetGenericArguments()[0];
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
        }

        public static T GetAttribute<T>(object attributeProvider) where T : Attribute
        {
            return GetAttribute<T>(attributeProvider, true);
        }

        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            T[] attributes = GetAttributes<T>(attributeProvider, inherit);

            return attributes?.FirstOrDefault();
        }

#if !(DOTNET || PORTABLE) || NETSTANDARD2_0
        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            Attribute[] a = GetAttributes(attributeProvider, typeof(T), inherit);

            if (a is T[] attributes)
            {
                return attributes;
            }

            return a.Cast<T>().ToArray();
        }

        public static Attribute[] GetAttributes(object attributeProvider, Type attributeType, bool inherit)
        {
            ValidationUtils.ArgumentNotNull(attributeProvider, nameof(attributeProvider));

            object provider = attributeProvider;

            // http://hyperthink.net/blog/getcustomattributes-gotcha/
            // ICustomAttributeProvider doesn't do inheritance

            switch (provider)
            {
                case Type t:
                    object[] array = attributeType != null ? t.GetCustomAttributes(attributeType, inherit) : t.GetCustomAttributes(inherit);
                    Attribute[] attributes = array.Cast<Attribute>().ToArray();

#if (NET20 || NET35)
                    // ye olde .NET GetCustomAttributes doesn't respect the inherit argument
                    if (inherit && t.BaseType != null)
                    {
                        attributes = attributes.Union(GetAttributes(t.BaseType, attributeType, inherit)).ToArray();
                    }
#endif

                    return attributes;
                case Assembly a:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(a, attributeType) : Attribute.GetCustomAttributes(a);
                case MemberInfo mi:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(mi, attributeType, inherit) : Attribute.GetCustomAttributes(mi, inherit);
#if !PORTABLE40
                case Module m:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(m, attributeType, inherit) : Attribute.GetCustomAttributes(m, inherit);
#endif
                case ParameterInfo p:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(p, attributeType, inherit) : Attribute.GetCustomAttributes(p, inherit);
                default:
#if !PORTABLE40
                    ICustomAttributeProvider customAttributeProvider = (ICustomAttributeProvider)attributeProvider;
                    object[] result = (attributeType != null) ? customAttributeProvider.GetCustomAttributes(attributeType, inherit) : customAttributeProvider.GetCustomAttributes(inherit);

                    return (Attribute[])result;
#else
                    throw new Exception("Cannot get attributes from '{0}'.".FormatWith(CultureInfo.InvariantCulture, provider));
#endif
            }
        }
#else
        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            return GetAttributes(attributeProvider, typeof(T), inherit).Cast<T>().ToArray();
        }

        public static Attribute[] GetAttributes(object provider, Type attributeType, bool inherit)
        {
            switch (provider)
            {
                case Type t:
                    return (attributeType != null)
                        ? t.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray()
                        : t.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
                case Assembly a:
                    return (attributeType != null) ? a.GetCustomAttributes(attributeType).ToArray() : a.GetCustomAttributes().ToArray();
                case MemberInfo memberInfo:
                    return (attributeType != null) ? memberInfo.GetCustomAttributes(attributeType, inherit).ToArray() : memberInfo.GetCustomAttributes(inherit).ToArray();
                case Module module:
                    return (attributeType != null) ? module.GetCustomAttributes(attributeType).ToArray() : module.GetCustomAttributes().ToArray();
                case ParameterInfo parameterInfo:
                    return (attributeType != null) ? parameterInfo.GetCustomAttributes(attributeType, inherit).ToArray() : parameterInfo.GetCustomAttributes(inherit).ToArray();
            }

            throw new Exception("Cannot get attributes from '{0}'.".FormatWith(CultureInfo.InvariantCulture, provider));
        }
#endif

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            T attribute;

#if !(NET20 || DOTNET)
            Type metadataType = GetAssociatedMetadataType(type);
            if (metadataType != null)
            {
                attribute = ReflectionUtils.GetAttribute<T>(metadataType, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }
#endif

            attribute = ReflectionUtils.GetAttribute<T>(type, true);
            if (attribute != null)
            {
                return attribute;
            }

            foreach (Type typeInterface in type.GetInterfaces())
            {
                attribute = ReflectionUtils.GetAttribute<T>(typeInterface, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
        }

#if !(NET20 || DOTNET)
        private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(GetAssociateMetadataTypeFromAttribute);
        private static ReflectionObject _metadataTypeAttributeReflectionObject;

        private static Type GetAssociatedMetadataType(Type type)
        {
            return AssociatedMetadataTypesCache.Get(type);
        }

        private static Type GetAssociateMetadataTypeFromAttribute(Type type)
        {
            Attribute[] customAttributes = ReflectionUtils.GetAttributes(type, null, true);

            foreach (Attribute attribute in customAttributes)
            {
                Type attributeType = attribute.GetType();

                // only test on attribute type name
                // attribute assembly could change because of type forwarding, etc
                if (string.Equals(attributeType.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
                {
                    const string metadataClassTypeName = "MetadataClassType";

                    if (_metadataTypeAttributeReflectionObject == null)
                    {
                        _metadataTypeAttributeReflectionObject = ReflectionObject.Create(attributeType, metadataClassTypeName);
                    }

                    return (Type)_metadataTypeAttributeReflectionObject.GetValue(attribute, metadataClassTypeName);
                }
            }

            return null;
        }
#endif

        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// /// <param name="nonPublic">if set to <c>true</c> then allow the member to be gotten non-publicly.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
        {
            switch (member.MemberType())
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = (FieldInfo)member;

                    if (nonPublic)
                    {
                        return true;
                    }
                    else if (fieldInfo.IsPublic)
                    {
                        return true;
                    }
                    return false;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanRead)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    return (propertyInfo.GetGetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <param name="nonPublic">if set to <c>true</c> then allow the member to be set non-publicly.</param>
        /// <param name="canSetReadOnly">if set to <c>true</c> then allow the member to be set if read-only.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
        {
            switch (member.MemberType())
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = (FieldInfo)member;

                    if (fieldInfo.IsLiteral)
                    {
                        return false;
                    }
                    if (fieldInfo.IsInitOnly && !canSetReadOnly)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    if (fieldInfo.IsPublic)
                    {
                        return true;
                    }
                    return false;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanWrite)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    return (propertyInfo.GetSetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            ValidationUtils.ArgumentNotNull(member, nameof(member));

            switch (member.MemberType())
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                default:
                    throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", nameof(member));
            }
        }
    }
}
