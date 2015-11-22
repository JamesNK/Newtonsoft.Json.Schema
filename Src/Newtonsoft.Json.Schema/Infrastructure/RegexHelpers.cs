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
        public static bool TryGetPatternRegex(string pattern, ref Regex regex, ref string errorMessage)
        {
            if (regex == null)
            {
                if (errorMessage != null)
                {
                    regex = null;
                    return false;
                }

                if (pattern == null)
                    throw new InvalidOperationException("Cannot get pattern regex, pattern has not been set.");

                try
                {
                    regex = new Regex(pattern);
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
