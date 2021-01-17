﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class AttributeHelpers
    {
        private static ReflectionObject? _dataTypeReflectionObject;
        private static ReflectionObject? _regexReflectionObject;
        private static ReflectionObject? _maxLengthReflectionObject;
        private static ReflectionObject? _minLengthReflectionObject;
        private static ReflectionObject? _enumTypeReflectionObject;
        private static ReflectionObject? _stringLengthReflectionObject;
        private static ReflectionObject? _rangeReflectionObject;
        private static ReflectionObject? _displayReflectionObject;
        private static ReflectionObject? _displayNameReflectionObject;
        private static ReflectionObject? _descriptionReflectionObject;

        private const string DisplayNameAttributeName = "System.ComponentModel.DisplayNameAttribute";
        private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";
        private const string DisplayAttributeName = "System.ComponentModel.DataAnnotations.DisplayAttribute";
        private const string RequiredAttributeName = "System.ComponentModel.DataAnnotations.RequiredAttribute";
        private const string MinLengthAttributeName = "System.ComponentModel.DataAnnotations.MinLengthAttribute";
        private const string MaxLengthAttributeName = "System.ComponentModel.DataAnnotations.MaxLengthAttribute";
        private const string DataTypeAttributeName = "System.ComponentModel.DataAnnotations.DataTypeAttribute";
        private const string RegularExpressionAttributeName = "System.ComponentModel.DataAnnotations.RegularExpressionAttribute";
        private const string RangeAttributeName = "System.ComponentModel.DataAnnotations.RangeAttribute";
        private const string UrlAttributeName = "System.ComponentModel.DataAnnotations.UrlAttribute";
        private const string PhoneAttributeName = "System.ComponentModel.DataAnnotations.PhoneAttribute";
        private const string EmailAddressAttributeName = "System.ComponentModel.DataAnnotations.EmailAddressAttribute";
        private const string StringLengthAttributeName = "System.ComponentModel.DataAnnotations.StringLengthAttribute";
        private const string EnumDataTypeAttributeName = "System.ComponentModel.DataAnnotations.EnumDataTypeAttribute";

        private static bool GetDisplay(Type type, JsonProperty? memberProperty, out string? name, out string? description)
        {
            Attribute? displayAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayAttributeName, out Type? matchingType);
            if (displayAttribute != null)
            {
                if (_displayReflectionObject == null)
                {
                    _displayReflectionObject = ReflectionObject.Create(matchingType!, "GetName", "GetDescription");
                }
                name = (string?)_displayReflectionObject.GetValue(displayAttribute, "GetName");
                description = (string?)_displayReflectionObject.GetValue(displayAttribute, "GetDescription");
                return true;
            }

            name = null;
            description = null;
            return false;
        }

        public static bool GetDisplayName(Type type, JsonProperty? memberProperty, out string? displayName)
        {
            if (GetDisplay(type, memberProperty, out displayName, out _) && !string.IsNullOrEmpty(displayName))
            {
                return true;
            }

            Attribute? displayNameAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayNameAttributeName, out Type? matchingType);
            if (displayNameAttribute != null)
            {
                if (_displayNameReflectionObject == null)
                {
                    _displayNameReflectionObject = ReflectionObject.Create(matchingType!, "DisplayName");
                }
                displayName = (string?)_displayNameReflectionObject.GetValue(displayNameAttribute, "DisplayName");
                return true;
            }

            displayName = null;
            return false;
        }

        public static bool GetDescription(Type type, JsonProperty? memberProperty, out string? description)
        {
            if (GetDisplay(type, memberProperty, out _, out description) && !string.IsNullOrEmpty(description))
            {
                return true;
            }

            Attribute? descriptionAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DescriptionAttributeName, out Type? matchingType);
            if (descriptionAttribute != null)
            {
                if (_descriptionReflectionObject == null)
                {
                    _descriptionReflectionObject = ReflectionObject.Create(matchingType!, "Description");
                }
                description = (string?)_descriptionReflectionObject.GetValue(descriptionAttribute, "Description");
                return true;
            }

            description = null;
            return false;
        }

        public static bool GetRange(JsonProperty? property, out double minimum, out double maximum)
        {
            if (property != null)
            {
                Attribute? rangeAttribute = GetAttributeByName(property, RangeAttributeName, out Type? matchingType);
                if (rangeAttribute != null)
                {
                    if (_rangeReflectionObject == null)
                    {
                        _rangeReflectionObject = ReflectionObject.Create(matchingType!, "Minimum", "Maximum");
                    }
                    minimum = Convert.ToDouble(_rangeReflectionObject.GetValue(rangeAttribute, "Minimum"), CultureInfo.InvariantCulture);
                    maximum = Convert.ToDouble(_rangeReflectionObject.GetValue(rangeAttribute, "Maximum"), CultureInfo.InvariantCulture);
                    return true;
                }
            }

            minimum = 0;
            maximum = 0;
            return false;
        }

        public static bool GetStringLength(JsonProperty? property, out int minimumLength, out int maximumLength)
        {
            if (property != null)
            {
                Attribute? attribute = GetAttributeByName(property, StringLengthAttributeName, out Type? matchingType);
                if (attribute != null)
                {
                    if (_stringLengthReflectionObject == null)
                    {
                        _stringLengthReflectionObject = ReflectionObject.Create(
                            matchingType!,
#if !NET35
                            "MinimumLength",
#endif
                            "MaximumLength");
                    }

#if !NET35
                    minimumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MinimumLength")!;
#else
                    minimumLength = 0;
#endif
                    maximumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MaximumLength")!;
                    return true;
                }
            }

            minimumLength = 0;
            maximumLength = 0;
            return false;
        }

        public static Type? GetEnumDataType(JsonProperty? property)
        {
            if (property != null)
            {
                Attribute? attribute = GetAttributeByName(property, EnumDataTypeAttributeName, out Type? matchingType);
                if (attribute != null)
                {
                    if (_enumTypeReflectionObject == null)
                    {
                        _enumTypeReflectionObject = ReflectionObject.Create(matchingType!, "EnumType");
                    }
                    return (Type)_enumTypeReflectionObject.GetValue(attribute, "EnumType")!;
                }
            }

            return null;
        }

        public static int? GetMinLength(JsonProperty? property)
        {
            if (property != null)
            {
                Attribute? minLengthAttribute = GetAttributeByName(property, MinLengthAttributeName, out Type? matchingType);
                if (minLengthAttribute != null)
                {
                    if (_minLengthReflectionObject == null)
                    {
                        _minLengthReflectionObject = ReflectionObject.Create(matchingType!, "Length");
                    }
                    return (int)_minLengthReflectionObject.GetValue(minLengthAttribute, "Length")!;
                }
            }

            return null;
        }

        public static int? GetMaxLength(JsonProperty? property)
        {
            if (property != null)
            {
                Attribute? maxLengthAttribute = GetAttributeByName(property, MaxLengthAttributeName, out Type? matchingType);
                if (maxLengthAttribute != null)
                {
                    if (_maxLengthReflectionObject == null)
                    {
                        _maxLengthReflectionObject = ReflectionObject.Create(matchingType!, "Length");
                    }
                    return (int)_maxLengthReflectionObject.GetValue(maxLengthAttribute, "Length")!;
                }
            }

            return null;
        }

        public static bool GetRequired(JsonProperty property)
        {
            if (property != null)
            {
                Attribute? required = GetAttributeByName(property, RequiredAttributeName, out _);
                return (required != null);
            }

            return false;
        }

        public static string? GetPattern(JsonProperty? property)
        {
            if (property != null)
            {
                Attribute? regexAttribute = GetAttributeByName(property, RegularExpressionAttributeName, out Type? matchingType);
                if (regexAttribute != null)
                {
                    if (_regexReflectionObject == null)
                    {
                        _regexReflectionObject = ReflectionObject.Create(matchingType!, "Pattern");
                    }
                    return (string)_regexReflectionObject.GetValue(regexAttribute, "Pattern")!;
                }
            }

            return null;
        }

        public static string? GetFormat(JsonProperty? property)
        {
            if (property != null)
            {
                if (GetAttributeByName(property, UrlAttributeName, out _) != null)
                {
                    return Constants.Formats.Uri;
                }

                if (GetAttributeByName(property, PhoneAttributeName, out _) != null)
                {
                    return Constants.Formats.Phone;
                }

                if (GetAttributeByName(property, EmailAddressAttributeName, out _) != null)
                {
                    return Constants.Formats.Email;
                }

                Attribute? dataTypeAttribute = GetAttributeByName(property, DataTypeAttributeName, out Type? matchingType);
                if (dataTypeAttribute != null)
                {
                    if (_dataTypeReflectionObject == null)
                    {
                        _dataTypeReflectionObject = ReflectionObject.Create(matchingType!, "DataType");
                    }
                    string s = _dataTypeReflectionObject.GetValue(dataTypeAttribute, "DataType")!.ToString();
                    switch (s)
                    {
                        case "Url":
                            return Constants.Formats.Uri;
                        case "Date":
                            return Constants.Formats.Date;
                        case "Time":
                            return Constants.Formats.Time;
                        case "DateTime":
                            return Constants.Formats.DateTime;
                        case "EmailAddress":
                            return Constants.Formats.Email;
                        case "PhoneNumber":
                            return Constants.Formats.Phone;
                    }
                }
            }

            return null;
        }

        private static Attribute? GetAttributeByName(JsonProperty property, string name, out Type? matchingType)
        {
            return GetAttributeByName(property.AttributeProvider, name, out matchingType);
        }

        private static Attribute? GetAttributeByName(IAttributeProvider? attributeProvider, string name, out Type? matchingType)
        {
            if (attributeProvider != null)
            {
                IList<Attribute> attributes = attributeProvider.GetAttributes(true);
                foreach (Attribute attribute in attributes)
                {
                    if (IsMatchingAttribute(attribute.GetType(), name, out matchingType))
                    {
                        return attribute;
                    }
                }
            }

            matchingType = null;
            return null;
        }

        private static bool IsMatchingAttribute(Type attributeType, string name, [NotNullWhen(true)] out Type? matchingType)
        {
            // check that attribute or its base class matches the name
            // e.g. attribute might inherit from DescriptionAttribute
            Type currentType = attributeType;
            do
            {
                if (string.Equals(currentType.FullName, name, StringComparison.Ordinal))
                {
                    matchingType = currentType;
                    return true;
                }
            } while ((currentType = currentType.BaseType()) != null);

            matchingType = null;
            return false;
        }

        private static Attribute? GetAttributeByNameFromTypeOrProperty(Type type, JsonProperty? memberProperty, string name, out Type? matchingType)
        {
            matchingType = null;
            Attribute? attribute = null;

            // check for property attribute first
            if (memberProperty != null)
            {
                attribute = GetAttributeByName(memberProperty.AttributeProvider, name, out matchingType);
            }

            // fall back to type attribute
            if (attribute == null)
            {
                attribute = GetAttributeByName(new ReflectionAttributeProvider(type), name, out matchingType);
            }

            return attribute;
        }
    }
}