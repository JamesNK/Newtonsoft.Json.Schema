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
    internal static class CompareUtils
    {
        internal static int CompareInteger(object? objA, object? objB)
        {
            if (objA == objB)
            {
                return 0;
            }
            if (objB == null)
            {
                return 1;
            }
            if (objA == null)
            {
                return -1;
            }

#if HAVE_BIG_INTEGER
            if (objA is BigInteger integerA)
            {
                return CompareBigInteger(integerA, objB);
            }
            if (objB is BigInteger integerB)
            {
                return -CompareBigInteger(integerB, objA);
            }
#endif
            if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
            {
                return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
            }
            else if (objA is float || objB is float || objA is double || objB is double)
            {
                return CompareFloat(objA, objB);
            }
            else
            {
                return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
            }
        }

        private static int CompareFloat(object objA, object objB)
        {
            double d1 = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
            double d2 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);

            // take into account possible floating point errors
            if (ApproxEquals(d1, d2))
            {
                return 0;
            }

            return d1.CompareTo(d2);
        }

        public static bool ApproxEquals(double d1, double d2)
        {
            const double epsilon = 2.2204460492503131E-16;

            if (d1 == d2)
            {
                return true;
            }

            double tolerance = ((Math.Abs(d1) + Math.Abs(d2)) + 10.0) * epsilon;
            double difference = d1 - d2;

            return (-tolerance < difference && tolerance > difference);
        }

#if HAVE_BIG_INTEGER
        private static int CompareBigInteger(BigInteger i1, object i2)
        {
            int result = i1.CompareTo(ToBigInteger(i2));

            if (result != 0)
            {
                return result;
            }

            // converting a fractional number to a BigInteger will lose the fraction
            // check for fraction if result is two numbers are equal
            if (i2 is decimal d1)
            {
                return (0m).CompareTo(Math.Abs(d1 - Math.Truncate(d1)));
            }
            else if (i2 is double || i2 is float)
            {
                double d = Convert.ToDouble(i2, CultureInfo.InvariantCulture);
                return (0d).CompareTo(Math.Abs(d - Math.Truncate(d)));
            }

            return result;
        }

        internal static BigInteger ToBigInteger(object value)
        {
            if (value is BigInteger integer)
            {
                return integer;
            }

            if (value is string s)
            {
                return BigInteger.Parse(s, CultureInfo.InvariantCulture);
            }

            if (value is float f)
            {
                return new BigInteger(f);
            }
            if (value is double d)
            {
                return new BigInteger(d);
            }
            if (value is decimal @decimal)
            {
                return new BigInteger(@decimal);
            }
            if (value is int i)
            {
                return new BigInteger(i);
            }
            if (value is long l)
            {
                return new BigInteger(l);
            }
            if (value is uint u)
            {
                return new BigInteger(u);
            }
            if (value is ulong @ulong)
            {
                return new BigInteger(@ulong);
            }

            if (value is byte[] bytes)
            {
                return new BigInteger(bytes);
            }

            throw new InvalidCastException("Cannot convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }
#endif
    }
}
