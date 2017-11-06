#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class AttributeHelpers
    {
        private static ReflectionObject _dataTypeReflectionObject;
        private static ReflectionObject _regexReflectionObject;
        private static ReflectionObject _maxLengthReflectionObject;
        private static ReflectionObject _minLengthReflectionObject;
        private static ReflectionObject _enumTypeReflectionObject;
        private static ReflectionObject _stringLengthReflectionObject;
        private static ReflectionObject _rangeReflectionObject;
        private static ReflectionObject _displayReflectionObject;
        private static ReflectionObject _displayNameReflectionObject;
        private static ReflectionObject _descriptionReflectionObject;

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

        private static bool GetDisplay(Type type, JsonProperty memberProperty, out string name, out string description)
        {
            Attribute displayAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayAttributeName);
            if (displayAttribute != null)
            {
                if (_displayReflectionObject == null)
                {
                    _displayReflectionObject = ReflectionObject.Create(displayAttribute.GetType(), "GetName", "GetDescription");
                }
                name = (string)_displayReflectionObject.GetValue(displayAttribute, "GetName");
                description = (string)_displayReflectionObject.GetValue(displayAttribute, "GetDescription");
                return true;
            }

            name = null;
            description = null;
            return false;
        }

        public static bool GetDisplayName(Type type, JsonProperty memberProperty, out string displayName)
        {
            if (GetDisplay(type, memberProperty, out displayName, out _) && !string.IsNullOrEmpty(displayName))
            {
                return true;
            }

            Attribute displayNameAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayNameAttributeName);
            if (displayNameAttribute != null)
            {
                if (_displayNameReflectionObject == null)
                {
                    _displayNameReflectionObject = ReflectionObject.Create(displayNameAttribute.GetType(), "DisplayName");
                }
                displayName = (string)_displayNameReflectionObject.GetValue(displayNameAttribute, "DisplayName");
                return true;
            }

            displayName = null;
            return false;
        }

        public static bool GetDescription(Type type, JsonProperty memberProperty, out string description)
        {
            if (GetDisplay(type, memberProperty, out _, out description) && !string.IsNullOrEmpty(description))
            {
                return true;
            }

            Attribute descriptionAttribute = GetAttributeByNameFromTypeOrProperty(type, memberProperty, DescriptionAttributeName);
            if (descriptionAttribute != null)
            {
                if (_descriptionReflectionObject == null)
                {
                    _descriptionReflectionObject = ReflectionObject.Create(descriptionAttribute.GetType(), "Description");
                }
                description = (string)_descriptionReflectionObject.GetValue(descriptionAttribute, "Description");
                return true;
            }

            description = null;
            return false;
        }

        public static bool GetRange(JsonProperty property, out double minimum, out double maximum)
        {
            if (property != null)
            {
                Attribute rangeAttribute = GetAttributeByName(property, RangeAttributeName);
                if (rangeAttribute != null)
                {
                    if (_rangeReflectionObject == null)
                    {
                        _rangeReflectionObject = ReflectionObject.Create(rangeAttribute.GetType(), "Minimum", "Maximum");
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

        public static bool GetStringLength(JsonProperty property, out int minimumLength, out int maximumLength)
        {
            if (property != null)
            {
                Attribute attribute = GetAttributeByName(property, StringLengthAttributeName);
                if (attribute != null)
                {
                    if (_stringLengthReflectionObject == null)
                    {
                        _stringLengthReflectionObject = ReflectionObject.Create(
                            attribute.GetType(),
#if !NET35
                            "MinimumLength",
#endif
                            "MaximumLength");
                    }

#if !NET35
                    minimumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MinimumLength");
#else
                    minimumLength = 0;
#endif
                    maximumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MaximumLength");
                    return true;
                }
            }

            minimumLength = 0;
            maximumLength = 0;
            return false;
        }

        public static Type GetEnumDataType(JsonProperty property)
        {
            if (property != null)
            {
                Attribute attribute = GetAttributeByName(property, EnumDataTypeAttributeName);
                if (attribute != null)
                {
                    if (_enumTypeReflectionObject == null)
                    {
                        _enumTypeReflectionObject = ReflectionObject.Create(attribute.GetType(), "EnumType");
                    }
                    return (Type)_enumTypeReflectionObject.GetValue(attribute, "EnumType");
                }
            }

            return null;
        }

        public static int? GetMinLength(JsonProperty property)
        {
            if (property != null)
            {
                Attribute minLengthAttribute = GetAttributeByName(property, MinLengthAttributeName);
                if (minLengthAttribute != null)
                {
                    if (_minLengthReflectionObject == null)
                    {
                        _minLengthReflectionObject = ReflectionObject.Create(minLengthAttribute.GetType(), "Length");
                    }
                    return (int)_minLengthReflectionObject.GetValue(minLengthAttribute, "Length");
                }
            }

            return null;
        }

        public static int? GetMaxLength(JsonProperty property)
        {
            if (property != null)
            {
                Attribute maxLengthAttribute = GetAttributeByName(property, MaxLengthAttributeName);
                if (maxLengthAttribute != null)
                {
                    if (_maxLengthReflectionObject == null)
                    {
                        _maxLengthReflectionObject = ReflectionObject.Create(maxLengthAttribute.GetType(), "Length");
                    }
                    return (int)_maxLengthReflectionObject.GetValue(maxLengthAttribute, "Length");
                }
            }

            return null;
        }

        public static bool GetRequired(JsonProperty property)
        {
            if (property != null)
            {
                Attribute required = GetAttributeByName(property, RequiredAttributeName);
                return (required != null);
            }

            return false;
        }

        public static string GetPattern(JsonProperty property)
        {
            if (property != null)
            {
                Attribute regexAttribute = GetAttributeByName(property, RegularExpressionAttributeName);
                if (regexAttribute != null)
                {
                    if (_regexReflectionObject == null)
                    {
                        _regexReflectionObject = ReflectionObject.Create(regexAttribute.GetType(), "Pattern");
                    }
                    return (string)_regexReflectionObject.GetValue(regexAttribute, "Pattern");
                }
            }

            return null;
        }

        public static string GetFormat(JsonProperty property)
        {
            if (property != null)
            {
                if (GetAttributeByName(property, UrlAttributeName) != null)
                {
                    return Constants.Formats.Uri;
                }

                if (GetAttributeByName(property, PhoneAttributeName) != null)
                {
                    return Constants.Formats.Phone;
                }

                if (GetAttributeByName(property, EmailAddressAttributeName) != null)
                {
                    return Constants.Formats.Email;
                }

                Attribute dataTypeAttribute = GetAttributeByName(property, DataTypeAttributeName);
                if (dataTypeAttribute != null)
                {
                    if (_dataTypeReflectionObject == null)
                    {
                        _dataTypeReflectionObject = ReflectionObject.Create(dataTypeAttribute.GetType(), "DataType");
                    }
                    string s = _dataTypeReflectionObject.GetValue(dataTypeAttribute, "DataType").ToString();
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

        private static Attribute GetAttributeByName(JsonProperty property, string name)
        {
            return GetAttributeByName(property.AttributeProvider, name);
        }

        private static Attribute GetAttributeByName(IAttributeProvider attributeProvider, string name)
        {
            if (attributeProvider != null)
            {
                IList<Attribute> attributes = attributeProvider.GetAttributes(true);
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

        private static Attribute GetAttributeByNameFromTypeOrProperty(Type type, JsonProperty memberProperty, string name)
        {
            Attribute attribute = null;

            // check for property attribute first
            if (memberProperty != null)
            {
                attribute = GetAttributeByName(memberProperty.AttributeProvider, name);
            }

            // fall back to type attribute
            if (attribute == null)
            {
                attribute = GetAttributeByName(new ReflectionAttributeProvider(type), name);
            }

            return attribute;
        }
    }
}