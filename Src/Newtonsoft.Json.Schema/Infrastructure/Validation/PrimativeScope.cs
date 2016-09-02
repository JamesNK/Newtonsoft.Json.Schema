#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;
#if !(NET35 || PORTABLE) || NETSTANDARD1_3
using System.Numerics;
#endif
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class PrimativeScope : SchemaScope
    {
        public void Initialize(ContextBase context, SchemaScope parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Primitive);
            InitializeSchema(schema);
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            switch (token)
            {
                case JsonToken.Integer:
                {
                    if (!ValidateInteger(Schema, value))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Float:
                {
                    if (!ValidateNumber(Schema, value))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.String:
                {
                    Uri uri = value as Uri;
                    string s = (uri != null) ? uri.OriginalString : value.ToString();

                    if (!ValidateString(Schema, s))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Boolean:
                {
                    if (!ValidateBoolean(Schema, value))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Null:
                {
                    if (!ValidateNull(Schema, value))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Bytes:
                {
                    byte[] data = value as byte[];
                    if (data == null)
                    {
                        data = ((Guid)value).ToByteArray();
                    }

                    string s = Convert.ToBase64String(data);
                    if (!ValidateString(Schema, s))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Undefined:
                {
                    JSchemaType schemaType = Schema.Type.GetValueOrDefault(JSchemaType.None);

                    if (schemaType != JSchemaType.None)
                    {
                        RaiseError($"Invalid type. Expected {schemaType.GetDisplayText()} but got {token}.", ErrorType.Type, Schema, value, null);
                    }
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
            {
                return false;
            }

            if (schema.Maximum != null)
            {
                object v;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
                v = (value is BigInteger) ? (double)(BigInteger)value : value;
#else
                v = value;
#endif

                if (JValue.Compare(JTokenType.Integer, v, schema.Maximum) > 0)
                {
                    RaiseError($"Integer {value} exceeds maximum value of {schema.Maximum}.", ErrorType.Maximum, schema, value, null);
                }
                if (schema.ExclusiveMaximum && JValue.Compare(JTokenType.Integer, v, schema.Maximum) == 0)
                {
                    RaiseError($"Integer {value} equals maximum value of {schema.Maximum} and exclusive maximum is true.", ErrorType.Maximum, schema, value, null);
                }
            }

            if (schema.Minimum != null)
            {
                object v;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
                v = (value is BigInteger) ? (double)(BigInteger)value : value;
#else
                v = value;
#endif

                if (JValue.Compare(JTokenType.Integer, v, schema.Minimum) < 0)
                {
                    RaiseError($"Integer {value} is less than minimum value of {schema.Minimum}.", ErrorType.Minimum, schema, value, null);
                }
                if (schema.ExclusiveMinimum && JValue.Compare(JTokenType.Integer, v, schema.Minimum) == 0)
                {
                    RaiseError($"Integer {value} equals minimum value of {schema.Minimum} and exclusive minimum is true.", ErrorType.Minimum, schema, value, null);
                }
            }

            if (schema.MultipleOf != null)
            {
                if (!MathHelpers.IsIntegerMultiple(value, schema.MultipleOf.Value))
                {
                    RaiseError($"Integer {JsonConvert.ToString(value)} is not a multiple of {schema.MultipleOf}.", ErrorType.MultipleOf, schema, value, null);
                }
            }

            return true;
        }

        private bool ValidateString(JSchema schema, string value)
        {
            if (!TestType(schema, JSchemaType.String, value))
            {
                return false;
            }

            if (schema.MaximumLength != null || schema.MinimumLength != null)
            {
                // want to test the character length and ignore unicode surrogates
                StringInfo stringInfo = new StringInfo(value);
                int textLength = stringInfo.LengthInTextElements;

                if (schema.MaximumLength != null && textLength > schema.MaximumLength)
                {
                    RaiseError($"String '{value}' exceeds maximum length of {schema.MaximumLength}.", ErrorType.MaximumLength, schema, value, null);
                }

                if (schema.MinimumLength != null && textLength < schema.MinimumLength)
                {
                    RaiseError($"String '{value}' is less than minimum length of {schema.MinimumLength}.", ErrorType.MinimumLength, schema, value, null);
                }
            }

            if (schema.Pattern != null)
            {
                Regex regex;
                string errorMessage;

                if (schema.TryGetPatternRegex(out regex, out errorMessage))
                {
                    if (!regex.IsMatch(value))
                    {
                        RaiseError($"String '{value}' does not match regex pattern '{schema.Pattern}'.", ErrorType.Pattern, schema, value, null);
                    }
                }
                else
                {
                    RaiseError($"Could not validate string with regex pattern '{schema.Pattern}'. There was an error parsing the regex: {errorMessage}", ErrorType.Pattern, schema, value, null);
                }
            }

            if (schema.Format != null)
            {
                bool valid = ValidateFormat(schema.Format, value);

                if (!valid)
                {
                    RaiseError($"String '{value}' does not validate against format '{schema.Format}'.", ErrorType.Format, schema, value, null);
                }
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
                    {
                        return false;
                    }

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

        private bool ValidateNumber(JSchema schema, object value)
        {
            if (!TestType(schema, JSchemaType.Number, value))
            {
                return false;
            }

            if (schema.Maximum != null)
            {
                double d = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (d > schema.Maximum)
                {
                    RaiseError($"Float {JsonConvert.ToString(value)} exceeds maximum value of {schema.Maximum}.", ErrorType.Maximum, schema, value, null);
                }
                if (schema.ExclusiveMaximum && d == schema.Maximum)
                {
                    RaiseError($"Float {JsonConvert.ToString(value)} equals maximum value of {schema.Maximum} and exclusive maximum is true.", ErrorType.Maximum, schema, value, null);
                }
            }

            if (schema.Minimum != null)
            {
                double d = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (d < schema.Minimum)
                {
                    RaiseError($"Float {JsonConvert.ToString(value)} is less than minimum value of {schema.Minimum}.", ErrorType.Minimum, schema, value, null);
                }
                if (schema.ExclusiveMinimum && d == schema.Minimum)
                {
                    RaiseError($"Float {JsonConvert.ToString(value)} equals minimum value of {schema.Minimum} and exclusive minimum is true.", ErrorType.Minimum, schema, value, null);
                }
            }

            if (schema.MultipleOf != null)
            {
                bool isMultiple = MathHelpers.IsDoubleMultiple(value, schema.MultipleOf.Value);

                if (!isMultiple)
                {
                    RaiseError($"Float {JsonConvert.ToString(value)} is not a multiple of {schema.MultipleOf}.", ErrorType.MultipleOf, schema, value, null);
                }
            }

            return true;
        }
    }
}