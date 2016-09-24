#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class CollectionHelpers
    {
        public static readonly object[] EmptyArray = new object[0];

        public static bool IsNullOrEmpty<T>(this List<T> values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> values)
        {
            return (values == null || values.Count == 0);
        }
    }
}