#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.IO;
#if HAVE_BIG_INTEGER
using System.Numerics;
#endif
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class PrimativeScope : SchemaScope
    {
        private static readonly Regex HostnameRegex = new Regex(@"^(?=.{1,255}$)[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?(?:\.[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?)*\.?$", RegexOptions.CultureInvariant);
#if NET35
        private static readonly Regex UuidRegex = new Regex("^[0-9A-Fa-f]{8}-(?:[0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}$", RegexOptions.CultureInvariant);
#endif

        public void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Primitive);
            InitializeSchema(schema);
        }

        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            EnsureValid(value);

            switch (token)
            {
                case JsonToken.Integer:
                {
                    if (!ValidateInteger(Schema, value!))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.Float:
                {
                    if (!ValidateNumber(Schema, value!))
                    {
                        return true;
                    }
                    break;
                }
                case JsonToken.String:
                case JsonToken.PropertyName:
                {
                    if (value == null)
                    {
                        // This can happen with a JTokenReader when a JValue has a String type
                        // and a null value
                        goto case JsonToken.Null;
                    }

                    string s = (value is Uri uri) ? uri.OriginalString : value.ToString();

                    if (!ValidateString(this, Schema, s))
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
                    byte[]? data = value as byte[];
                    if (data == null)
                    {
                        data = ((Guid) value!).ToByteArray();
                    }

                    string s = Convert.ToBase64String(data);
                    if (!ValidateString(this, Schema, s))
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
            ValidateConditionalChildren(token, value, depth);

            return true;
        }

        private bool ValidateNull(JSchema schema, object? value)
        {
            return TestType(schema, JSchemaType.Null, value);
        }

        private bool ValidateBoolean(JSchema schema, object? value)
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
#if HAVE_BIG_INTEGER
                v = (value is BigInteger d) ? (double)d : value;
#else
                v = value;
#endif

                if (CompareUtils.CompareInteger(v, schema.Maximum) > 0)
                {
                    RaiseError($"Integer {value} exceeds maximum value of {schema.Maximum}.", ErrorType.Maximum, schema, value, null);
                }
                if (schema.ExclusiveMaximum && CompareUtils.CompareInteger(v, schema.Maximum) == 0)
                {
                    RaiseError($"Integer {value} equals maximum value of {schema.Maximum} and exclusive maximum is true.", ErrorType.Maximum, schema, value, null);
                }
            }

            if (schema.Minimum != null)
            {
                object v;
#if HAVE_BIG_INTEGER
                v = (value is BigInteger d) ? (double)d : value;
#else
                v = value;
#endif

                if (CompareUtils.CompareInteger(v, schema.Minimum) < 0)
                {
                    RaiseError($"Integer {value} is less than minimum value of {schema.Minimum}.", ErrorType.Minimum, schema, value, null);
                }
                if (schema.ExclusiveMinimum && CompareUtils.CompareInteger(v, schema.Minimum) == 0)
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

        internal static bool ValidateString(SchemaScope scope, JSchema schema, string value)
        {
            if (!TestType(scope, schema, JSchemaType.String, value))
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
                    scope.RaiseError($"String '{value}' exceeds maximum length of {schema.MaximumLength}.", ErrorType.MaximumLength, schema, value, null);
                }

                if (schema.MinimumLength != null && textLength < schema.MinimumLength)
                {
                    scope.RaiseError($"String '{value}' is less than minimum length of {schema.MinimumLength}.", ErrorType.MinimumLength, schema, value, null);
                }
            }

            if (schema.Pattern != null)
            {
                if (schema.TryGetPatternRegex(
#if !(NET35 || NET40)
                    scope.Context.Validator.RegexMatchTimeout,
#endif
                    out Regex? regex,
                    out string? errorMessage))
                {
                    if (!RegexHelpers.IsMatch(regex, schema.Pattern, value))
                    {
                        scope.RaiseError($"String '{value}' does not match regex pattern '{schema.Pattern}'.", ErrorType.Pattern, schema, value, null);
                    }
                }
                else
                {
                    scope.RaiseError($"Could not validate string with regex pattern '{schema.Pattern}'. There was an error parsing the regex: {errorMessage}", ErrorType.Pattern, schema, value, null);
                }
            }

            if (schema.ContentEncoding != null)
            {
                if (schema.ContentEncoding == Constants.ContentEncodings.Base64)
                {
                    bool valid = StringHelpers.IsBase64String(value);

                    if (!valid)
                    {
                        scope.RaiseError($"String '{value}' does not validate against content encoding '{schema.ContentEncoding}'.", ErrorType.ContentEncoding, schema, value, null);
                    }
                }
            }

            if (schema.Format != null)
            {
                bool valid = ValidateFormat(schema.Format, value);

                if (!valid)
                {
                    scope.RaiseError($"String '{value}' does not validate against format '{schema.Format}'.", ErrorType.Format, schema, value, null);
                }
            }

            return true;
        }

        private static readonly char[] CaseInsensitiveDateTimeChars = new[] { 't', 'z' };

        private static bool ValidateFormat(string format, string value)
        {
            switch (format)
            {
                case Constants.Formats.Uuid:
                    {
#if NET35
                        return UuidRegex.IsMatch(value);
#else
                        return Guid.TryParseExact(value, "D", out _);
#endif
                    }
                case Constants.Formats.Color:
                    {
                        return ColorHelpers.IsValid(value);
                    }
                case Constants.Formats.Hostname:
                case Constants.Formats.Draft3Hostname:
                    {
                        // http://stackoverflow.com/questions/1418423/the-hostname-regex
                        return HostnameRegex.IsMatch(value);
                    }
                case Constants.Formats.IPv4:
                case Constants.Formats.Draft3IPv4:
                    {
                        return FormatHelpers.ValidateIPv4(value);
                    }
                case Constants.Formats.IPv6:
                    {
                        return FormatHelpers.ValidateIPv6(value);
                    }
                case Constants.Formats.Email:
                    {
                        return EmailHelpers.Validate(value, true);
                    }
                case Constants.Formats.Uri:
                    {
                        return Uri.IsWellFormedUriString(value, UriKind.Absolute);
                    }
                case Constants.Formats.UriReference:
                    {
                        return FormatHelpers.ValidateUriReference(value);
                    }
                case Constants.Formats.Duration:
                    {
                        return FormatHelpers.ValidateDuration(value);
                    }
                case Constants.Formats.UriTemplate:
                    {
                        return FormatHelpers.ValidateUriTemplate(value);
                    }
                case Constants.Formats.JsonPointer:
                    {
                        return FormatHelpers.ValidateJsonPointer(value);
                    }
                case Constants.Formats.Date:
                    {
                        return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
                    }
                case Constants.Formats.Time:
                    {
                        var parser = new DateTimeParser();
                        return parser.ParseTime(value, 0, value.Length);
                    }
                case Constants.Formats.DateTime:
                    {
                        var parser = new DateTimeParser();
                        return parser.ParseDateTime(value, 0, value.Length);
                    }
                case Constants.Formats.UtcMilliseconds:
                    {
                        return double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out double _);
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
            if (JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Integer)
                && !JSchemaTypeHelpers.HasFlag(schema.Type, JSchemaType.Number)
                && MathHelpers.IsDoubleMultiple(value, 1)
                && SchemaVersionHelpers.EnsureVersion(Context.Validator.SchemaVersion, SchemaVersion.Draft6))
            {
                // accept values like 1.0 in draft6+
            }
            else if (!TestType(schema, JSchemaType.Number, value))
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