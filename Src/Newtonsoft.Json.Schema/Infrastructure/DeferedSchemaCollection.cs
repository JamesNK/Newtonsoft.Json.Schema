#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal struct DeferedSchemaKey : IEquatable<DeferedSchemaKey>
    {
        public DeferedSchemaKey(Uri resolvedReference, Uri? dynamicScopeId)
        {
            ResolvedReference = resolvedReference;
            DynamicScopeId = dynamicScopeId;
        }

        public Uri ResolvedReference { get; }
        public Uri? DynamicScopeId { get; }

        public override bool Equals(object obj)
        {
            if (obj is DeferedSchemaKey key)
            {
                return Equals(key);
            }

            return false;
        }

        public bool Equals(DeferedSchemaKey other)
        {
            return UriComparer.Instance.Equals(ResolvedReference, other.ResolvedReference)
                && UriComparer.Instance.Equals(DynamicScopeId, other.DynamicScopeId);
        }

        public override int GetHashCode()
        {
            int hashCode = UriComparer.Instance.GetHashCode(ResolvedReference);
            if (DynamicScopeId != null)
            {
                hashCode = hashCode ^ UriComparer.Instance.GetHashCode(DynamicScopeId);
            }
            return hashCode;
        }
    }

    internal class DeferedSchemaCollection : KeyedCollection<DeferedSchemaKey, DeferedSchema>
    {
        protected override DeferedSchemaKey GetKeyForItem(DeferedSchema item)
        {
            return DeferedSchema.CreateKey(item);
        }

        public bool TryGetValue(DeferedSchemaKey key, [NotNullWhen(true)] out DeferedSchema? value)
        {
            if (Dictionary == null)
            {
                value = null;
                return false;
            }

            return Dictionary.TryGetValue(key, out value);
        }
    }
}