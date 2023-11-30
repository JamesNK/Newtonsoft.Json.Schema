#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("Id = {Id}, DynamicScope = {DynamicScope}, IsRoot = {IsRoot}")]
    internal readonly struct KnownSchemaUriKey : IEquatable<KnownSchemaUriKey>
    {
        public readonly Uri Id;
        public readonly Uri? DynamicScope;
        public readonly bool IsRoot;

        public KnownSchemaUriKey(Uri id, Uri? dynamicScope, bool isRoot) : this()
        {
            Id = id;
            DynamicScope = dynamicScope;
            IsRoot = isRoot;
        }

        public static KnownSchemaUriKey Create(KnownSchema knownSchema)
        {
            return new KnownSchemaUriKey(knownSchema.Id, knownSchema.DynamicScope, knownSchema.IsRoot);
        }

        public bool Equals(KnownSchemaUriKey other)
        {
            bool compareFragments = !(IsRoot && other.IsRoot);
            if (!UriComparer.Instance.Equals(Id, other.Id, compareFragments))
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
