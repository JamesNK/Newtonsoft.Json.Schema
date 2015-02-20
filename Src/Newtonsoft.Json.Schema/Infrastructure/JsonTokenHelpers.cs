#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class JsonTokenHelpers
    {
        internal static bool IsPrimitiveOrEndToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.EndConstructor:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsPrimitiveOrStartToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.StartObject:
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return true;
                default:
                    return false;
            }
        }
    }
}