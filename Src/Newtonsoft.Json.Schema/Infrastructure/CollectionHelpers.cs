#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

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
    }
}