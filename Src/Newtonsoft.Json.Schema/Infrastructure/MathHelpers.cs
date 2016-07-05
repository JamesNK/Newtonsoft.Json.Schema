#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class MathHelpers
    {
        public static bool IsMultiple(double value, double multipleOf)
        {
            const double epsilon = 2.2204460492503131e-016;
            const double tolerance = epsilon * 300;

            double remainder = value % multipleOf;
            double absRemainder = Math.Abs(remainder);

            if (absRemainder < tolerance)
            {
                return true;
            }
            if (Math.Abs(multipleOf) - absRemainder < tolerance)
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