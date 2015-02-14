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
                return false;

            if (!x.IsAbsoluteUri)
                return true;

            return string.Equals(x.Fragment, y.Fragment, StringComparison.Ordinal);
        }

        public int GetHashCode(Uri obj)
        {
            if (!obj.IsAbsoluteUri || string.IsNullOrEmpty(obj.Fragment))
                return obj.GetHashCode();

            return obj.GetHashCode() ^ obj.Fragment.GetHashCode();
        }
    }
}