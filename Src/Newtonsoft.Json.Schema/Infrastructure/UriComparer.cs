#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class UriComparer : IEqualityComparer<Uri>
    {
        public static readonly UriComparer Instance = new UriComparer();

        public bool Equals(Uri? x, Uri? y)
        {
            if (x != y)
            {
                return false;
            }

            if (x == null && y == null)
            {
                return true;
            }

            ValidationUtils.Assert(x != null);
            ValidationUtils.Assert(y != null);

            if (!x.IsAbsoluteUri)
            {
                return true;
            }

            return string.Equals(ResolveFragment(x), ResolveFragment(y), StringComparison.Ordinal);
        }

        public int GetHashCode(Uri obj)
        {
            if (!obj.IsAbsoluteUri)
            {
                return obj.GetHashCode();
            }

            string resolvedFragment = ResolveFragment(obj);
            if (string.IsNullOrEmpty(resolvedFragment))
            {
                return obj.GetHashCode();
            }

            return obj.GetHashCode() ^ resolvedFragment.GetHashCode();
        }

        private string ResolveFragment(Uri uri)
        {
            string resolvedFragment = uri.Fragment;

            // an empty fragment '#' is the same as no fragment
            if (string.Equals(resolvedFragment, "#", StringComparison.Ordinal))
            {
                resolvedFragment = string.Empty;
            }

            return resolvedFragment;
        }
    }
}