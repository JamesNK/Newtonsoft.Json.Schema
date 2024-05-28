#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    [DebuggerDisplay("ResolvedReference = {ResolvedReference}, DynamicScopeId = {DynamicScopeId}")]
    internal readonly struct DeferredSchemaKey : IEquatable<DeferredSchemaKey>
    {
        public DeferredSchemaKey(Uri resolvedReference, Uri? dynamicScopeId)
        {
            ResolvedReference = resolvedReference;
            DynamicScopeId = dynamicScopeId;
        }

        public Uri ResolvedReference { get; }
        public Uri? DynamicScopeId { get; }

        public override bool Equals(object obj)
        {
            if (obj is DeferredSchemaKey key)
            {
                return Equals(key);
            }

            return false;
        }

        public bool Equals(DeferredSchemaKey other)
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

    internal class DeferredSchemaCollection : KeyedCollection<DeferredSchemaKey, DeferredSchema>
    {
        protected override DeferredSchemaKey GetKeyForItem(DeferredSchema item)
        {
            return DeferredSchema.CreateKey(item);
        }

#if !NETSTANDARD2_1_OR_GREATER
        public bool TryGetValue(DeferredSchemaKey key, [NotNullWhen(true)] out DeferredSchema? value)
        {
            if (Dictionary == null)
            {
                value = null;
                return false;
            }

            return Dictionary.TryGetValue(key, out value);
        }
#endif
    }
}