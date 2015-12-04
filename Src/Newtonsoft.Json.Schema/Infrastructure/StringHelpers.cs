#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class StringHelpers
    {
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if !NET35
            return string.Join(separator, values);
#else
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (separator == null)
            {
                separator = string.Empty;
            }

            using (IEnumerator<T> enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();
                if (enumerator.Current != null)
                {
                    string s = enumerator.Current.ToString();
                    sb.Append(s);
                }
                while (enumerator.MoveNext())
                {
                    sb.Append(separator);
                    if (enumerator.Current != null)
                    {
                        string s = enumerator.Current.ToString();
                        sb.Append(s);
                    }
                }

                return sb.ToString();
            }
#endif
        }
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class FormattableStringFactory
    {
        public static FormattableString Create(string s, params object[] args)
        {
            return new FormattableString(s, args);
        }
    }

    internal class FormattableString : IFormattable
    {
        private readonly string _format;
        private readonly object[] _args;

        public FormattableString(string format, object[] args)
        {
            _format = format;
            _args = args;
        }

        public string ToString(string ignored, IFormatProvider formatProvider)
        {
            if (_args.IsNullOrEmpty())
            {
                return _format;
            }
            return string.Format(formatProvider, _format, _args);
        }
    }
}