#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal readonly struct StructMultiKey<T1, T2> : IEquatable<StructMultiKey<T1, T2>>
    {
        public readonly T1 Value1;
        public readonly T2 Value2;

        public StructMultiKey(T1 v1, T2 v2)
        {
            Value1 = v1;
            Value2 = v2;
        }

        public override int GetHashCode()
        {
            return (Value1?.GetHashCode() ?? 0) ^ (Value2?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StructMultiKey<T1, T2> key))
            {
                return false;
            }

            return Equals(key);
        }

        public bool Equals(StructMultiKey<T1, T2> other)
        {
            return (Equals(Value1, other.Value1) && Equals(Value2, other.Value2));
        }
    }
}
