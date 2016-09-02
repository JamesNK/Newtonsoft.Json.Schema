#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
using System.Numerics;
#endif

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class MathHelpers
    {
        private static readonly double DecimalDoubleMaxValue;
        private static readonly double DecimalDoubleMinValue;

        private static readonly double DoubleEpsilon;
        private static readonly decimal DecimalEpsilon;
        private static readonly decimal DecimalTolerance;
        private static readonly double DoubleTolerance;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
        private static readonly BigInteger BigIntegerDoubleMaxValue;
        private static readonly BigInteger BigIntegerDoubleMinValue;
#endif

        static MathHelpers()
        {
            DoubleEpsilon = 2.2204460492503131e-016;
            DecimalEpsilon = Convert.ToDecimal(DoubleEpsilon);
            DoubleTolerance = DoubleEpsilon * 300;
            DecimalTolerance = DecimalEpsilon * 300;

            DecimalDoubleMaxValue = Convert.ToDouble(decimal.MaxValue);
            DecimalDoubleMinValue = Convert.ToDouble(decimal.MinValue);

#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
            BigIntegerDoubleMaxValue = new BigInteger(decimal.MaxValue);
            BigIntegerDoubleMinValue = new BigInteger(decimal.MinValue);
#endif
        }

        public static bool IsIntegerMultiple(object integer, double multipleOf)
        {
            // use decimal math if multipleOf fits inside decimal
            if (FitsInDecimal(multipleOf))
            {
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
                bool integerIsInRange;

                if (integer is BigInteger)
                {
                    BigInteger i = (BigInteger) integer;
                    integerIsInRange = (i < BigIntegerDoubleMaxValue && i > BigIntegerDoubleMinValue);
                }
                else
                {
                    integerIsInRange = true;
                }

                if (integerIsInRange)
#endif
                {
                    return IsIntegerMultiple(integer, Convert.ToDecimal(multipleOf));
                }
            }

            bool isMultiple;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
            if (integer is BigInteger)
            {
                BigInteger i = (BigInteger)integer;

                bool divisibleNonInteger = !IsZero(Math.Abs(multipleOf - Math.Truncate(multipleOf)));

                if (divisibleNonInteger)
                {
                    // biginteger only supports operations against other integers
                    // this will lose any decimal point on MultipleOf
                    // so raise an error if MultipleOf is not an integer and value is not zero
                    if (i <= BigIntegerDoubleMaxValue)
                    {
                        isMultiple = IsRemainderMultiple((double)i % multipleOf, multipleOf);
                    }
                    else
                    {
                        isMultiple = i == 0;
                    }
                }
                else
                {
                    isMultiple = i % new BigInteger(multipleOf) == 0;
                }
            }
            else
#endif
            {
                isMultiple = IsRemainderMultiple(Convert.ToInt64(integer, CultureInfo.InvariantCulture) % multipleOf, multipleOf);
            }

            return isMultiple;
        }

        private static bool IsIntegerMultiple(object integer, decimal multipleOf)
        {
            if (multipleOf == 0)
            {
                return true;
            }

            bool isMultiple;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
            if (integer is BigInteger)
            {
                BigInteger i = (BigInteger)integer;

                bool divisibleNonInteger = !IsZero(Math.Abs(multipleOf - Math.Truncate(multipleOf)));

                if (divisibleNonInteger)
                {
                    // biginteger only supports operations against other integers
                    // this will lose any decimal point on MultipleOf
                    // so raise an error if MultipleOf is not an integer and value is not zero
                    isMultiple = IsRemainderMultiple((decimal)i % multipleOf, multipleOf);
                }
                else
                {
                    isMultiple = i % new BigInteger(multipleOf) == 0;
                }
            }
            else
#endif
            {
                isMultiple = IsRemainderMultiple(Convert.ToInt64(integer, CultureInfo.InvariantCulture) % multipleOf, multipleOf);
            }

            return isMultiple;
        }

        public static bool IsDoubleMultiple(object value, double multipleOf)
        {
            double d = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            if ((FitsInDecimal(d) || value is decimal) && FitsInDecimal(multipleOf))
            {
                return IsDoubleMultiple(Convert.ToDecimal(value, CultureInfo.InvariantCulture), Convert.ToDecimal(multipleOf));
            }

            double remainder = d % multipleOf;

            return IsRemainderMultiple(remainder, multipleOf);
        }

        public static bool IsDoubleMultiple(decimal value, decimal multipleOf)
        {
            decimal remainder = value % multipleOf;

            return IsRemainderMultiple(remainder, multipleOf);
        }

        private static bool IsRemainderMultiple(decimal remainder, decimal multipleOf)
        {
            decimal absRemainder = Math.Abs(remainder);

            if (absRemainder <= DecimalTolerance)
            {
                return true;
            }
            if (Math.Abs(multipleOf) - absRemainder <= DecimalTolerance)
            {
                return true;
            }

            return false;
        }

        private static bool IsRemainderMultiple(double remainder, double multipleOf)
        {
            double absRemainder = Math.Abs(remainder);

            if (absRemainder <= DoubleTolerance)
            {
                return true;
            }
            if (Math.Abs(multipleOf) - absRemainder <= DoubleTolerance)
            {
                return true;
            }

            return false;
        }

        private static bool FitsInDecimal(double d)
        {
            return (d < DecimalDoubleMaxValue && d > DecimalDoubleMinValue);
        }

        public static bool IsZero(decimal value)
        {
            return Math.Abs(value) < 20 * DecimalEpsilon;
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 20.0 * DoubleEpsilon;
        }
    }
}