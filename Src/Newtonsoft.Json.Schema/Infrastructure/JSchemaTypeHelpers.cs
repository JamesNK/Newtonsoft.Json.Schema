#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class JSchemaTypeHelpers
    {
        internal static bool HasFlag(JSchemaType? value, JSchemaType flag)
        {
            // default value is Any
            if (value == null)
                return true;

            bool match = ((value & flag) == flag);
            if (match)
                return true;

            // integer is a subset of number
            if (flag == JSchemaType.Integer && (value & JSchemaType.Number) == JSchemaType.Number)
                return true;

            return false;
        }

        internal static string MapType(JSchemaType type)
        {
            return Constants.JSchemaTypeMapping.Single(kv => kv.Value == type).Key;
        }
    }
}