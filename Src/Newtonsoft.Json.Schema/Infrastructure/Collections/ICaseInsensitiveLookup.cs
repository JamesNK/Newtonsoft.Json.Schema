#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal interface ICaseInsensitiveLookup<TValue>
    {
        bool ContainsKey(string key, bool ignoreCase);
        public bool TryGetValue(string key, [NotNullWhen(true)] out TValue? value, bool ignoreCase);
    }
}