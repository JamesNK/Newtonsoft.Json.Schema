#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class EnumInfo
    {
        public EnumInfo(bool isFlags, ulong[] values, string[] names, string[] resolvedNames)
        {
            IsFlags = isFlags;
            Values = values;
            Names = names;
            ResolvedNames = resolvedNames;
        }

        public readonly bool IsFlags;
        public readonly ulong[] Values;
        public readonly string[] Names;
        public readonly string[] ResolvedNames;
    }
}