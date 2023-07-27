#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("Id = {Id}, DynamicScope = {DynamicScope}")]
    internal readonly struct KnownSchemaKey : IEquatable<KnownSchemaKey>
    {
        public readonly JSchema Id;
        public readonly Uri? DynamicScope;

        public KnownSchemaKey(JSchema id, Uri? dynamicScope) : this()
        {
            Id = id;
            DynamicScope = dynamicScope;
        }

        public static KnownSchemaKey Create(KnownSchema knownSchema)
        {
            return new KnownSchemaKey(knownSchema.Schema, knownSchema.DynamicScope);
        }

        public bool Equals(KnownSchemaKey other)
        {
            if (!Equals(Id, other.Id))
            {
                return false;
            }
            if (!UriComparer.Instance.Equals(DynamicScope, other.DynamicScope))
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is KnownSchemaUriKey key)
            {
                return Equals(key);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 234325025;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            if (DynamicScope != null)
            {
                hashCode = hashCode * -1521134295 + UriComparer.Instance.GetHashCode(DynamicScope);
            }
            return hashCode;
        }
    }
}
