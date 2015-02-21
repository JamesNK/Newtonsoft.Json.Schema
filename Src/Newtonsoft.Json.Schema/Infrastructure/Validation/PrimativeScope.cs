#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;
#if !(NET35 || PORTABLE)
using System.Numerics;
#endif
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class PrimativeScope : SchemaScope
    {
        public PrimativeScope(ContextBase context, Scope parent, int initialDepth, JSchema schema)
            : base(context, parent, initialDepth, schema)
        {
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            switch (token)
            {
                case JsonToken.Integer:
                {
                    if (!ValidateInteger(Schema, value))
                        return true;
                    break;
                }
                case JsonToken.Float:
                {
                    if (!ValidateNumber(Schema, Convert.ToDouble(value, CultureInfo.InvariantCulture)))
                        return true;
                    break;
                }
                case JsonToken.String:
                {
                    Uri uri = value as Uri;
                    string s = (uri != null) ? uri.OriginalString : value.ToString();

                    if (!ValidateString(Schema, s))
                        return true;
                    break;
                }
                case JsonToken.Boolean:
                {
                    if (!ValidateBoolean(Schema, value))
                        return true;
                    break;
                }
                case JsonToken.Null:
                {
                    if (!ValidateNull(Schema, value))
                        return true;
                    break;
                }
                case JsonToken.Bytes:
                {
                    byte[] data = value as byte[];
                    if (data == null)
                        data = ((Guid)value).ToByteArray();

                    string s = Convert.ToBase64String(data);
                    if (!ValidateString(Schema, s))
                        return true;
                    break;
                }
                case JsonToken.Undefined:
                {
                    RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, Schema.Type, token), ErrorType.Type, Schema, value, null);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("Unexpected token: " + token);
                }
            }

            EnsureEnum(token, value);

            return true;
        }

        private bool ValidateNull(JSchema schema, object value)
        {
            return TestType(schema, JSchemaType.Null, value);
        }

        private bool ValidateBoolean(JSchema schema, object value)
        {
            return TestType(schema, JSchemaType.Boolean, value);
        }

        private bool ValidateInteger(JSchema schema, object value)
        {
            if (!TestType(schema, JSchemaType.Integer, value))
                return false;

            if (schema.Maximum != null)
            {
                if (JValue.Compare(JTokenType.Integer, value, schema.Maximum) > 0)
                    RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), ErrorType.Maximum, schema, value, null);
                if (schema.ExclusiveMaximum && JValue.Compare(JTokenType.Integer, value, schema.Maximum) == 0)
                    RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), ErrorType.Maximum, schema, value, null);
            }

            if (schema.Minimum != null)
            {
                if (JValue.Compare(JTokenType.Integer, value, schema.Minimum) < 0)
                    RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), ErrorType.Minimum, schema, value, null);
                if (schema.ExclusiveMinimum && JValue.Compare(JTokenType.Integer, value, schema.Minimum) == 0)
                    RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), ErrorType.Minimum, schema, value, null);
            }

            if (schema.MultipleOf != null)
            {
                bool notDivisible;
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
                if (value is BigInteger)
                {
                    double multipleOf = schema.MultipleOf.Value;

                    // not that this will lose any decimal point on MultipleOf
                    // so manually raise an error if MultipleOf is not an integer and value is not zero
                    BigInteger i = (BigInteger)value;
                    bool divisibleNonInteger = !Math.Abs(multipleOf - Math.Truncate(multipleOf)).Equals(0);
                    if (divisibleNonInteger)
                        notDivisible = i != 0;
                    else
                        notDivisible = i % new BigInteger(multipleOf) != 0;
                }
                else
#endif
                notDivisible = !MathHelpers.IsZero(Convert.ToInt64(value, CultureInfo.InvariantCulture) % schema.MultipleOf.Value);

                if (notDivisible)
                    RaiseError("Integer {0} is not a multiple of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.MultipleOf), ErrorType.MultipleOf, schema, value, null);
            }

            return true;
        }

        private bool ValidateString(JSchema schema, string value)
        {
            if (!TestType(schema, JSchemaType.String, value))
                return false;

            if (schema.MaximumLength != null || schema.MinimumLength != null)
            {
                // want to test the character length and ignore unicode surrogates
                StringInfo stringInfo = new StringInfo(value);
                int textLength = stringInfo.LengthInTextElements;

                if (schema.MaximumLength != null && textLength > schema.MaximumLength)
                    RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MaximumLength), ErrorType.MaximumLength, schema, value, null);

                if (schema.MinimumLength != null && textLength < schema.MinimumLength)
                    RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MinimumLength), ErrorType.MinimumLength, schema, value, null);
            }

            if (schema.Pattern != null)
            {
                if (!Regex.IsMatch(value, schema.Pattern))
                    RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, value, schema.Pattern), ErrorType.Pattern, schema, value, null);
            }

            if (schema.Format != null)
            {
                bool valid = ValidateFormat(schema.Format, value);

                if (!valid)
                    RaiseError("String '{0}' does not validate against format '{1}'.".FormatWith(CultureInfo.InvariantCulture, value, schema.Format), ErrorType.Format, schema, value, null);
            }

            return true;
        }

        private static bool ValidateFormat(string format, string value)
        {
            switch (format)
            {
                case Constants.Formats.Color:
                {
                    return ColorHelpers.IsValid(value);
                }
                case Constants.Formats.Hostname:
                case Constants.Formats.Draft3Hostname:
                {
                    // http://stackoverflow.com/questions/1418423/the-hostname-regex
                    return Regex.IsMatch(value, @"^(?=.{1,255}$)[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?(?:\.[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?)*\.?$", RegexOptions.CultureInvariant);
                }
                case Constants.Formats.IPv4:
                case Constants.Formats.Draft3IPv4:
                {
                    string[] parts = value.Split('.');
                    if (parts.Length != 4)
                        return false;

                    for (int i = 0; i < parts.Length; i++)
                    {
                        int num;
                        if (!int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out num)
                            || (num < 0 || num > 255))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                case Constants.Formats.IPv6:
                {
                    return (Uri.CheckHostName(value) == UriHostNameType.IPv6);
                }
                case Constants.Formats.Email:
                {
                    return EmailHelpers.Validate(value, true);
                }
                case Constants.Formats.Uri:
                {
                    return Uri.IsWellFormedUriString(value, UriKind.Absolute);
                }
                case Constants.Formats.Date:
                {
                    DateTime temp;
                    return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                }
                case Constants.Formats.Time:
                {
                    DateTime temp;
                    return DateTime.TryParseExact(value, "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                }
                case Constants.Formats.DateTime:
                {
                    DateTime temp;
                    return DateTime.TryParseExact(value, @"yyyy-MM-dd\THH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp);
                }
                case Constants.Formats.UtcMilliseconds:
                {
                    double temp;
                    return Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out temp);
                }
                case Constants.Formats.Regex:
                {
                    try
                    {
                        new Regex(value);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                default:
                {
                    return true;
                }
            }
        }

        private bool ValidateNumber(JSchema schema, double value)
        {
            if (!TestType(schema, JSchemaType.Number, value))
                return false;

            if (schema.Maximum != null)
            {
                if (value > schema.Maximum)
                    RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), ErrorType.Maximum, schema, value, null);
                if (schema.ExclusiveMaximum && value == schema.Maximum)
                    RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), ErrorType.Maximum, schema, value, null);
            }

            if (schema.Minimum != null)
            {
                if (value < schema.Minimum)
                    RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), ErrorType.Minimum, schema, value, null);
                if (schema.ExclusiveMinimum && value == schema.Minimum)
                    RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), ErrorType.Minimum, schema, value, null);
            }

            if (schema.MultipleOf != null)
            {
                double remainder = MathHelpers.FloatingPointRemainder(value, schema.MultipleOf.Value);

                if (!MathHelpers.IsZero(remainder))
                    RaiseError("Float {0} is not a multiple of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.MultipleOf), ErrorType.MultipleOf, schema, value, null);
            }

            return true;
        }
    }
}