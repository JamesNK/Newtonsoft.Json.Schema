using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class PrimativeScope : SchemaScope
    {
        public PrimativeScope(ContextBase context, Scope scope, int initialDepth, JSchema schema)
            : base(context, scope, initialDepth, schema)
        {
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            switch (token)
            {
                case JsonToken.Integer:
                    if (!ValidateInteger(Schema, value))
                        return true;
                    break;
                case JsonToken.Float:
                    if (!ValidateFloat(Schema, Convert.ToDouble(value, CultureInfo.InvariantCulture)))
                        return true;
                    break;
                case JsonToken.String:
                    if (!ValidateString(Schema, (string)value))
                        return true;
                    break;
                case JsonToken.Boolean:
                    if (!ValidateBoolean(Schema))
                        return true;
                    break;
                case JsonToken.Null:
                    if (!ValidateNull(Schema))
                        return true;
                    break;
                case JsonToken.Bytes:
                case JsonToken.Date:
                case JsonToken.Undefined:
                    // these have no equivalent in JSON schema
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unexpected token: " + token);
            }

            EnsureEnum(token, value);

            return true;
        }

        private bool ValidateNull(JSchema schema)
        {
            return TestType(schema, JSchemaType.Null);
        }

        private bool ValidateBoolean(JSchema schema)
        {
            return TestType(schema, JSchemaType.Boolean);
        }

        private bool ValidateInteger(JSchema schema, object value)
        {
            if (!TestType(schema, JSchemaType.Integer))
                return false;

            if (schema.Maximum != null)
            {
                if (JValue.Compare(JTokenType.Integer, value, schema.Maximum) > 0)
                    RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema, null);
                if (schema.ExclusiveMaximum && JValue.Compare(JTokenType.Integer, value, schema.Maximum) == 0)
                    RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema, null);
            }

            if (schema.Minimum != null)
            {
                if (JValue.Compare(JTokenType.Integer, value, schema.Minimum) < 0)
                    RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema, null);
                if (schema.ExclusiveMinimum && JValue.Compare(JTokenType.Integer, value, schema.Minimum) == 0)
                    RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema, null);
            }

            if (schema.MultipleOf != null)
            {
                bool notDivisible;
#if !(NET20 || NET35 || PORTABLE40 || PORTABLE)
                if (value is BigInteger)
                {
                    // not that this will lose any decimal point on DivisibleBy
                    // so manually raise an error if DivisibleBy is not an integer and value is not zero
                    BigInteger i = (BigInteger)value;
                    bool divisibleNonInteger = !Math.Abs(schema.DivisibleBy.Value - Math.Truncate(schema.DivisibleBy.Value)).Equals(0);
                    if (divisibleNonInteger)
                        notDivisible = i != 0;
                    else
                        notDivisible = i % new BigInteger(schema.DivisibleBy.Value) != 0;
                }
                else
#endif
                notDivisible = !MathHelpers.IsZero(Convert.ToInt64(value, CultureInfo.InvariantCulture) % schema.MultipleOf.Value);

                if (notDivisible)
                    RaiseError("Integer {0} is not a multiple of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.MultipleOf), schema, null);
            }

            return true;
        }

        private bool ValidateString(JSchema schema, string value)
        {
            if (!TestType(schema, JSchemaType.String))
                return false;

            if (schema.MaximumLength != null || schema.MinimumLength != null)
            {
                // want to test the character length and ignore unicode surrogates
                StringInfo stringInfo = new StringInfo(value);
                int textLength = stringInfo.LengthInTextElements;

                if (schema.MaximumLength != null && textLength > schema.MaximumLength)
                    RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MaximumLength), schema, null);

                if (schema.MinimumLength != null && textLength < schema.MinimumLength)
                    RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.MinimumLength), schema, null);
            }

            if (schema.Pattern != null)
            {
                if (!Regex.IsMatch(value, schema.Pattern))
                    RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, value, schema.Pattern), schema, null);
            }

            return true;
        }

        private bool ValidateFloat(JSchema schema, double value)
        {
            if (!TestType(schema, JSchemaType.Float))
                return false;

            if (schema.Maximum != null)
            {
                if (value > schema.Maximum)
                    RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), schema, null);
                if (schema.ExclusiveMaximum && value == schema.Maximum)
                    RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Maximum), schema, null);
            }

            if (schema.Minimum != null)
            {
                if (value < schema.Minimum)
                    RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), schema, null);
                if (schema.ExclusiveMinimum && value == schema.Minimum)
                    RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.Minimum), schema, null);
            }

            if (schema.MultipleOf != null)
            {
                double remainder = MathHelpers.FloatingPointRemainder(value, schema.MultipleOf.Value);

                if (!MathHelpers.IsZero(remainder))
                    RaiseError("Float {0} is not a multiple of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.MultipleOf), schema, null);
            }

            return true;
        }
    }
}