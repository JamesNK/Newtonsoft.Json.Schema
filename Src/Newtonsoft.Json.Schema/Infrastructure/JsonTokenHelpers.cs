#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class JsonTokenHelpers
    {
        internal static bool IsPrimitiveToken(JsonToken token)
        {
            switch (token)
            {
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

        internal static bool Contains(List<JToken> enums, JToken value)
        {
            for (int i = 0; i < enums.Count; i++)
            {
                if (ImplicitDeepEquals(enums[i], value))
                {
                    return true;
                }
            }

            return false;
        }

        // Basically JToken.DeepEquals except float and integer values can be equal
        internal static bool ImplicitDeepEquals(JToken t1, JToken t2)
        {
            if (t1 == t2)
            {
                return true;
            }

            if (t1 is JObject o1)
            {
                return t2 is JObject o2 && ContentsEqual(o1, o2);
            }
            else if (t1 is JArray a1)
            {
                return t2 is JArray a2 && ContentsEqual(a1, a2);
            }
            else if (t1 is JConstructor c1)
            {
                return t2 is JConstructor c2 && ContentsEqual(c1, c2);
            }
            else if (t1 is JValue v1)
            {
                return t2 is JValue v2 && ValueTypeEquals(v1, v2) && v1.CompareTo(v2) == 0;
            }
            else if (t1 is JProperty p1)
            {
                return t2 is JProperty p2 && p1.Name == p2.Name && ContentsEqual(p1, p2);
            }

            return false;
        }

        private static bool ValueTypeEquals(JValue v1, JValue v2)
        {
            if (v1.Type == v2.Type)
            {
                return true;
            }

            if (v1.Type == JTokenType.Integer && v2.Type == JTokenType.Float)
            {
                return true;
            }

            if (v1.Type == JTokenType.Float && v2.Type == JTokenType.Integer)
            {
                return true;
            }

            return false;
        }

        private static bool ContentsEqual(IList<JToken> t1, IList<JToken> t2)
        {
            if (t1.Count != t2.Count)
            {
                return false;
            }

            for (int i = 0; i < t1.Count; i++)
            {
                if (!ImplicitDeepEquals(t1[i], t2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ContentsEqual(JObject o1, JObject o2)
        {
            if (o1 == o2)
            {
                return true;
            }

            if (o1.Count != o2.Count)
            {
                return false;
            }

            // dictionaries in JavaScript aren't ordered
            // ignore order when comparing properties
            IList<JToken> l = o1;
            for (int i = 0; i < l.Count; i++)
            {
                JProperty p1 = (JProperty)l[i];

                if (!o2.TryGetValue(p1.Name, out JToken secondValue))
                {
                    return false;
                }

                if (p1.Value == null)
                {
                    return (secondValue == null);
                }

                if (!ImplicitDeepEquals(p1.Value, secondValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}