#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
#if HAVE_BIG_INTEGER
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
#if HAVE_BIG_INTEGER
        private static readonly BigInteger BigIntegerDecimalMaxValue;
        private static readonly BigInteger BigIntegerDecimalMinValue;

        private static readonly BigInteger BigIntegerDoubleMaxValue;
#endif

        static MathHelpers()
        {
            DoubleEpsilon = 2.2204460492503131e-016;
            DecimalEpsilon = Convert.ToDecimal(DoubleEpsilon);
            DoubleTolerance = DoubleEpsilon * 300;
            DecimalTolerance = DecimalEpsilon * 300;

            DecimalDoubleMaxValue = Convert.ToDouble(decimal.MaxValue);
            DecimalDoubleMinValue = Convert.ToDouble(decimal.MinValue);

#if HAVE_BIG_INTEGER
            BigIntegerDecimalMaxValue = new BigInteger(decimal.MaxValue);
            BigIntegerDecimalMinValue = new BigInteger(decimal.MinValue);
            BigIntegerDoubleMaxValue = new BigInteger(double.MaxValue);
#endif
        }

        public static bool IsIntegerMultiple(object integer, double multipleOf)
        {
            // use decimal math if multipleOf fits inside decimal
            if (FitsInDecimal(multipleOf))
            {
                double integerAsDouble;
                bool integerIsInRange;

#if HAVE_BIG_INTEGER
                if (integer is BigInteger i1)
                {
                    integerIsInRange = (i1 < BigIntegerDecimalMaxValue && i1 > BigIntegerDecimalMinValue);

                    integerAsDouble = (double) i1;
                }
                else
                {
                    integerAsDouble = Convert.ToDouble(integer, CultureInfo.InvariantCulture);
                    integerIsInRange = true;
                }
#else
                integerAsDouble = Convert.ToDouble(integer, CultureInfo.InvariantCulture);
                integerIsInRange = true;
#endif

                if (integerIsInRange)
                {
                    decimal decimalMultipleOf = Convert.ToDecimal(multipleOf);
                    if (decimalMultipleOf == 0)
                    {
                        return true;
                    }

                    // check that the division result doesn't exceed a decimal so there is no overflow while calculating remainder
                    double division = integerAsDouble / multipleOf;
                    if (FitsInDecimal(division))
                    {
                        return IsIntegerMultiple(integer, decimalMultipleOf);
                    }
                }
            }

            bool isMultiple;
#if HAVE_BIG_INTEGER
            if (integer is BigInteger i2)
            {
                bool divisibleNonInteger = !IsZero(Math.Abs(multipleOf - Math.Truncate(multipleOf)));

                if (divisibleNonInteger)
                {
                    // biginteger only supports operations against other integers
                    // this will lose any decimal point on MultipleOf
                    // so raise an error if MultipleOf is not an integer and value is not zero
                    if (i2 <= BigIntegerDoubleMaxValue)
                    {
                        isMultiple = IsRemainderMultiple((double)i2 % multipleOf, multipleOf);
                    }
                    else
                    {
                        isMultiple = i2 == 0;
                    }
                }
                else
                {
                    isMultiple = i2 % new BigInteger(multipleOf) == 0;
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
            bool isMultiple;
#if HAVE_BIG_INTEGER
            if (integer is BigInteger i)
            {
                bool divisibleNonInteger = !IsZero(Math.Abs(multipleOf - Math.Truncate(multipleOf)));

                if (divisibleNonInteger)
                {
                    // biginteger only supports operations against other integers
                    // this will lose any decimal point on MultipleOf
                    // so raise an error if MultipleOf is not an integer and value is not zero
                    isMultiple = IsRemainderMultiple((decimal) i % multipleOf, multipleOf);
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
                decimal multipleOfD = Convert.ToDecimal(multipleOf);
                decimal valueD = Convert.ToDecimal(value, CultureInfo.InvariantCulture);

                // check if value divided by multipleOf is too big for decimal - the modulus will error if it is
                if (!FitsInDecimal(d / multipleOf))
                {
                    // check if remainder is between 0 and 1 and multipleOf fits into 0
                    // remove whole numbers from value to avoid overflow error
                    if (Math.Abs(multipleOfD) < 1 && 1 % multipleOfD == 0)
                    {
                        valueD = valueD % 1;
                        return IsDoubleMultiple(valueD, multipleOfD);
                    }
                }
                else
                {
                    return IsDoubleMultiple(valueD, multipleOfD);
                }
            }

            double remainder = d % multipleOf;

            return IsRemainderMultiple(remainder, multipleOf);
        }

        public static bool IsDoubleMultiple(decimal value, decimal multipleOf)
        {
            if (multipleOf == 0)
            {
                return false;
            }

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