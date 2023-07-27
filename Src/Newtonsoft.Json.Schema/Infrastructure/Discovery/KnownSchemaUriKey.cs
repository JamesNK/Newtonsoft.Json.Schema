#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("Id = {Id}, DynamicScope = {DynamicScope}")]
    internal readonly struct KnownSchemaUriKey : IEquatable<KnownSchemaUriKey>
    {
        public readonly Uri Id;
        public readonly Uri? DynamicScope;

        public KnownSchemaUriKey(Uri id, Uri? dynamicScope) : this()
        {
            Id = id;
            DynamicScope = dynamicScope;
        }

        public static KnownSchemaUriKey Create(KnownSchema knownSchema)
        {
            return new KnownSchemaUriKey(knownSchema.Id, knownSchema.DynamicScope);
        }

        public bool Equals(KnownSchemaUriKey other)
        {
            if (!UriComparer.Instance.Equals(Id, other.Id))
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
            hashCode = hashCode * -1521134295 + UriComparer.Instance.GetHashCode(Id);
            if (DynamicScope != null)
            {
                hashCode = hashCode * -1521134295 + UriComparer.Instance.GetHashCode(DynamicScope);
            }
            return hashCode;
        }
    }
}
