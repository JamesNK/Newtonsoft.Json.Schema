#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class DeferedSchemaCollection : KeyedCollection<Uri, DeferedSchema>
    {
        public DeferedSchemaCollection() : base(UriComparer.Instance)
        {
        }

        protected override Uri GetKeyForItem(DeferedSchema item)
        {
            return item.ResolvedReference;
        }

        public bool TryGetValue(Uri key, [NotNullWhen(true)] out DeferedSchema? value)
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