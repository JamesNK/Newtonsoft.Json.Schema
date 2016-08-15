#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class UriComparer : IEqualityComparer<Uri>
    {
        public static readonly UriComparer Instance = new UriComparer();

        public bool Equals(Uri x, Uri y)
        {
            if (x != y)
            {
                return false;
            }

            if (x == null && y == null)
            {
                return true;
            }

            if (!x.IsAbsoluteUri)
            {
                return true;
            }

            return string.Equals(ResolveFragment(x), ResolveFragment(y), StringComparison.Ordinal);
        }

        public int GetHashCode(Uri obj)
        {
            if (!obj.IsAbsoluteUri || string.IsNullOrEmpty(obj.Fragment))
            {
                return obj.GetHashCode();
            }

            return obj.GetHashCode() ^ obj.Fragment.GetHashCode();
        }

        private string ResolveFragment(Uri uri)
        {
            // an empty fragment '#' is the same as no fragment
            string resolvedFragment = string.Equals(uri.Fragment, "#", StringComparison.Ordinal)
                ? string.Empty
                : uri.Fragment;

            return resolvedFragment;
        }
    }
}