#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    public static class DataAnnotationHelpers
    {
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

        public static bool GetRange(JsonProperty property, out double minimum, out double maximum)
        {
            if (property != null)
            {
                Attribute attribute = GetAttributeByName(property, RangeAttributeName);
                if (attribute != null)
                {
                    ReflectionObject o = ReflectionObject.Create(attribute.GetType(), "Minimum", "Maximum");
                    minimum = Convert.ToDouble(o.GetValue(attribute, "Minimum"), CultureInfo.InvariantCulture);
                    maximum = Convert.ToDouble(o.GetValue(attribute, "Maximum"), CultureInfo.InvariantCulture);
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
                    ReflectionObject o = ReflectionObject.Create(attribute.GetType(), "MinimumLength", "MaximumLength");
                    minimumLength = (int)o.GetValue(attribute, "MinimumLength");
                    maximumLength = (int)o.GetValue(attribute, "MaximumLength");
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
                    ReflectionObject o = ReflectionObject.Create(attribute.GetType(), "EnumType");
                    return (Type)o.GetValue(attribute, "EnumType");
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
                    ReflectionObject o = ReflectionObject.Create(minLengthAttribute.GetType(), "Length");
                    return (int)o.GetValue(minLengthAttribute, "Length");
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
                    ReflectionObject o = ReflectionObject.Create(maxLengthAttribute.GetType(), "Length");
                    return (int)o.GetValue(maxLengthAttribute, "Length");
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
                    ReflectionObject o = ReflectionObject.Create(regexAttribute.GetType(), "Pattern");
                    return (string)o.GetValue(regexAttribute, "Pattern");
                }
            }

            return null;
        }

        public static string GetFormat(JsonProperty property)
        {
            if (property != null)
            {
                if (GetAttributeByName(property, UrlAttributeName) != null)
                    return Constants.Formats.Uri;

                if (GetAttributeByName(property, PhoneAttributeName) != null)
                    return Constants.Formats.Phone;

                if (GetAttributeByName(property, EmailAddressAttributeName) != null)
                    return Constants.Formats.Email;

                Attribute regexAttribute = GetAttributeByName(property, DataTypeAttributeName);
                if (regexAttribute != null)
                {
                    ReflectionObject o = ReflectionObject.Create(regexAttribute.GetType(), "DataType");
                    string s = o.GetValue(regexAttribute, "DataType").ToString();
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
    }
}
