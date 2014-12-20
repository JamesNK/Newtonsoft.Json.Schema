using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class JsonSchemaTypeHelpers
    {
        internal static bool HasFlag(JSchemaType? value, JSchemaType flag)
        {
            // default value is Any
            if (value == null)
                return true;

            bool match = ((value & flag) == flag);
            if (match)
                return true;

            // integer is a subset of float
            if (flag == JSchemaType.Integer && (value & JSchemaType.Float) == JSchemaType.Float)
                return true;

            return false;
        }

        internal static JSchemaType MapType(string type)
        {
            JSchemaType mappedType;
            if (!Constants.JsonSchemaTypeMapping.TryGetValue(type, out mappedType))
                throw new JsonException("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));

            return mappedType;
        }

        internal static string MapType(JSchemaType type)
        {
            return Constants.JsonSchemaTypeMapping.Single(kv => kv.Value == type).Key;
        }
    }
}