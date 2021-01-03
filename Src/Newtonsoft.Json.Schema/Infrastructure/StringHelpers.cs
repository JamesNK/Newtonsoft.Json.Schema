﻿#region License
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

        public static StringBuilder? TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0)
            {
                return sb;
            }

            int i = sb.Length - 1;
            for (; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    break;
                }
            }

            if (i < sb.Length - 1)
            {
                sb.Length = i + 1;
            }

            return sb;
        }

        public static bool IsBase64String(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length % 4 != 0)
            {
                return false;
            }
            int index = value.Length - 1;
            if (value[index] == '=')
            {
                index--;
            }

            if (value[index] == '=')
            {
                index--;
            }
            for (int i = 0; i <= index; i++)
            {
                if (IsInvalid(value[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsInvalid(char value)
        {
            if (value >= 48 && value <= 57)
            {
                return false;
            }

            if (value >= 65 && value <= 90)
            {
                return false;
            }

            if (value >= 97 && value <= 122)
            {
                return false;
            }

            return value != 43 && value != 47;
        }
    }
}

#if !(NETSTANDARD1_3 || NETSTANDARD2_0)
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

        public string Format => _format;

        public object[] GetArguments()
        {
            return _args;
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
#endif