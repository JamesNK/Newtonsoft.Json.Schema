#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class CollectionHelpers
    {
        public static readonly object[] EmptyArray = new object[0];

        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this List<T>? values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IList<T>? values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<TKey, TValue>([NotNullWhen(false)] this Dictionary<TKey, TValue>? values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool IsNullOrEmpty<TKey, TValue>([NotNullWhen(false)] this IDictionary<TKey, TValue>? values)
        {
            return (values == null || values.Count == 0);
        }

        public static bool ContainsKey<TValue>(Dictionary<string, TValue> dictionary, string key, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return dictionary.ContainsKey(key);
            }

            foreach (var kvp in dictionary)
            {
                if (string.Equals(key, kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetValue<TValue>(Dictionary<string, TValue> dictionary, string key, [NotNullWhen(true)] out TValue? value, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return dictionary.TryGetValue(key, out value);
            }

            foreach (var kvp in dictionary)
            {
                if (string.Equals(key, kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    value = kvp.Value!;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}