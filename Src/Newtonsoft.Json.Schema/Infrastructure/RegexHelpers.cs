#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class RegexHelpers
    {
        public static bool IsMatch(Regex regex, string pattern, string value)
        {
#if !(NET35 || NET40)
            try
#endif
            {
                return regex.IsMatch(value);
            }
#if !(NET35 || NET40)
            catch (RegexMatchTimeoutException ex)
            {
                throw new JSchemaException($"Timeout when matching regex pattern '{pattern}'.", ex);
            }
#endif
        }

        public static bool TryGetPatternRegex(
            string pattern,
#if !(NET35 || NET40)
            TimeSpan? matchTimeout,
#endif
            ref Regex regex,
            ref string errorMessage)
        {
            if (regex == null
#if !(NET35 || NET40)
                || regex.MatchTimeout != (matchTimeout ?? TimeSpan.FromMilliseconds(-1))
#endif
                )
            {
                if (errorMessage != null)
                {
                    regex = null;
                    return false;
                }

                if (pattern == null)
                {
                    throw new InvalidOperationException("Cannot get pattern regex, pattern has not been set.");
                }

                try
                {
                    regex =
#if !(NET35 || NET40)
                        (matchTimeout != null) ? new Regex(pattern, RegexOptions.None, matchTimeout.Value) :
#endif
                        new Regex(pattern, RegexOptions.None);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    regex = null;
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}