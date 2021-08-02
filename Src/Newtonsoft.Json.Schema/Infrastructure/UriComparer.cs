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

            return ResolveFragment(x).Equals(ResolveFragment(y));
        }

        public int GetHashCode(Uri obj)
        {
            if (!obj.IsAbsoluteUri)
            {
                return obj.GetHashCode();
            }

            StringSegment resolvedFragment = ResolveFragment(obj);
            if (resolvedFragment.IsEmpty)
            {
                return obj.GetHashCode();
            }

            return obj.GetHashCode() ^ resolvedFragment.GetHashCode();
        }

        private StringSegment ResolveFragment(Uri uri)
        {
#if NET5_0_OR_GREATER
            int fragmentIndex = uri.OriginalString.IndexOf('#', StringComparison.Ordinal);
#else
            int fragmentIndex = uri.OriginalString.IndexOf('#');
#endif
            if (fragmentIndex == -1)
            {
                return default;
            }

            int length = uri.OriginalString.Length - fragmentIndex;
            if (length == 1)
            {
                // an empty fragment '#' is the same as no fragment
                return default;
            }

            return new StringSegment(uri.OriginalString, fragmentIndex, length);
        }

        private readonly struct StringSegment : IEquatable<StringSegment>
        {
            private readonly string _s;
            private readonly int _startIndex;
            private readonly int _length;

            public StringSegment(string s, int startIndex, int length)
            {
                _s = s;
                _startIndex = startIndex;
                _length = length;
            }

            public bool IsEmpty => _s == null;

            public bool Equals(StringSegment other)
            {
                if (other._length != _length)
                {
                    return false;
                }

                for (int i = 0; i < _length; i++)
                {
                    if (_s[_startIndex + i] != other._s[other._startIndex + i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj is StringSegment other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode()
            {
                int hashCode = 257;

                int end = _startIndex + _length;
                for (int i = _startIndex; i < end; i++)
                {
                    hashCode = hashCode ^ _s[i];
                }

                return hashCode;
            }
        }
    }
}