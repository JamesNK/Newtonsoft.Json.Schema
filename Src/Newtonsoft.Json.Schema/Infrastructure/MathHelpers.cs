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
        private const double Epsilon = 2.2204460492503131e-016;
        private const double Tolerance = Epsilon * 300;
#if !(NET20 || NET35 || PORTABLE) || NETSTANDARD1_3
        private static readonly BigInteger DoubleMaxValue = new BigInteger(double.MaxValue);
#endif

        public static bool IsIntegerMultiple(object integer, double multipleOf)
        {
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
                    if (i <= DoubleMaxValue)
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

        public static bool IsDoubleMultiple(double value, double multipleOf)
        {
            double remainder = value % multipleOf;

            return IsRemainderMultiple(remainder, multipleOf);
        }

        private static bool IsRemainderMultiple(double remainder, double multipleOf)
        {
            double absRemainder = Math.Abs(remainder);

            if (absRemainder < Tolerance)
            {
                return true;
            }
            if (Math.Abs(multipleOf) - absRemainder < Tolerance)
            {
                return true;
            }

            return false;
        }

        public static bool IsZero(double value)
        {
            const double epsilon = 2.2204460492503131e-016;

            return Math.Abs(value) < 20.0 * epsilon;
        }
    }
}