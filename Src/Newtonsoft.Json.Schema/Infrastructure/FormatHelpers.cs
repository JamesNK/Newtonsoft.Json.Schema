#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Globalization;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class FormatHelpers
    {
        private static readonly Regex UriTemplateRegex = new Regex(@"^(?:(?:[^\x00-\x20""'<>%\\^`{|}]|%[0-9a-f]{2})|\{[+#.\/;?&=,!@|]?(?:[a-z0-9_]|%[0-9a-f]{2})+(?:\:[1-9][0-9]{0,3}|\*)?(?:,(?:[a-z0-9_]|%[0-9a-f]{2})+(?:\:[1-9][0-9]{0,3}|\*)?)*\})*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex JsonPointerRegex = new Regex(@"^(?:\/(?:[^~/]|~0|~1)*)*$|^#(?:\/(?:[a-z0-9_\-.!$&'()*+,;:=@]|%[0-9a-f]{2}|~0|~1)+)+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex UriReferenceRegex = new Regex(@"^(?:[a-z][a-z0-9+\-.]*:)?(?:\/?\/(?:(?:[a-z0-9\-._~!$&'()*+,;=:]|%[0-9a-f]{2})*@)?(?:\[(?:(?:(?:(?:[0-9a-f]{1,4}:){6}|::(?:[0-9a-f]{1,4}:){5}|(?:[0-9a-f]{1,4})?::(?:[0-9a-f]{1,4}:){4}|(?:(?:[0-9a-f]{1,4}:){0,1}[0-9a-f]{1,4})?::(?:[0-9a-f]{1,4}:){3}|(?:(?:[0-9a-f]{1,4}:){0,2}[0-9a-f]{1,4})?::(?:[0-9a-f]{1,4}:){2}|(?:(?:[0-9a-f]{1,4}:){0,3}[0-9a-f]{1,4})?::[0-9a-f]{1,4}:|(?:(?:[0-9a-f]{1,4}:){0,4}[0-9a-f]{1,4})?::)(?:[0-9a-f]{1,4}:[0-9a-f]{1,4}|(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?))|(?:(?:[0-9a-f]{1,4}:){0,5}[0-9a-f]{1,4})?::[0-9a-f]{1,4}|(?:(?:[0-9a-f]{1,4}:){0,6}[0-9a-f]{1,4})?::)|[Vv][0-9a-f]+\.[a-z0-9\-._~!$&'()*+,;=:]+)\]|(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)|(?:[a-z0-9\-._~!$&'""()*+,;=]|%[0-9a-f]{2})*)(?::\d*)?(?:\/(?:[a-z0-9\-._~!$&'""()*+,;=:@]|%[0-9a-f]{2})*)*|\/(?:(?:[a-z0-9\-._~!$&'""()*+,;=:@]|%[0-9a-f]{2})+(?:\/(?:[a-z0-9\-._~!$&'""()*+,;=:@]|%[0-9a-f]{2})*)*)?|(?:[a-z0-9\-._~!$&'""()*+,;=:@]|%[0-9a-f]{2})+(?:\/(?:[a-z0-9\-._~!$&'""()*+,;=:@]|%[0-9a-f]{2})*)*)?(?:\?(?:[a-z0-9\-._~!$&'""()*+,;=:@\/?]|%[0-9a-f]{2})*)?(?:\#(?:[a-z0-9\-._~!$&'""()*+,;=:@\/?]|%[0-9a-f]{2})*)?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex Ipv6Regex = new Regex(@"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$", RegexOptions.CultureInvariant);
        private static readonly Regex Ipv6PlusIpv4Regex = new Regex(@"^(?:[0-9A-Fa-f]{0,4}:){1,6}((?:[0-9]{1,3}\.){3}[0-9]{1,3})$", RegexOptions.CultureInvariant);

        public static bool ValidateUriTemplate(string text)
        {
            return UriTemplateRegex.IsMatch(text);
        }

        public static bool ValidateJsonPointer(string text)
        {
            return JsonPointerRegex.IsMatch(text);
        }

        public static bool ValidateUriReference(string text)
        {
            return UriReferenceRegex.IsMatch(text);
        }

        public static bool ValidateDuration(string text)
        {
            int index = 0;
            if (!Require(text, ref index, 'P'))
            {
                return false;
            }

            // Weeks can't be used with years/months/days
            bool hasDays = SkipElement(text, ref index, 'W');
            if (!hasDays)
            {
                hasDays = SkipElement(text, ref index, 'Y');
                hasDays |= SkipElement(text, ref index, 'M');
                hasDays |= SkipElement(text, ref index, 'D');
            }

            if (!Require(text, ref index, 'T'))
            {
                if (!hasDays)
                {
                    // Invalid because no days or time
                    return false;
                }
            }
            else
            {
                bool hasTime = SkipElement(text, ref index, 'H');
                hasTime |= SkipElement(text, ref index, 'M');
                hasTime |= SkipElement(text, ref index, 'S');

                if (!hasTime)
                {
                    // Invalid because has T but no time
                    return false;
                }
            }

            if (index != text.Length)
            {
                // Invalid because additional content
                return false;
            }

            return true;

            static bool Require(string text, ref int index, char requiredChar)
            {
                if (index < text.Length && text[index] == requiredChar)
                {
                    index++;
                    return true;
                }

                return false;
            }

            static bool SkipElement(string text, ref int index, char expectedElementChar)
            {
                int localIndex = index;
                bool hasDigit = false;
                while (localIndex < text.Length)
                {
                    char c = text[localIndex];
                    if (char.IsDigit(c) && IsAscii(c))
                    {
                        hasDigit = true;
                        localIndex++;
                        continue;
                    }
                    else if (c == expectedElementChar)
                    {
                        if (hasDigit)
                        {
                            index = localIndex + 1;
                            return true;
                        }
                    }

                    break;
                }

                return false;
            }
        }

        private static bool IsAscii(char ch)
        {
            return (ch <= '\x007f');
        }

        public static bool ValidateIPv6(string value)
        {
            if (Ipv6Regex.IsMatch(value))
            {
                return true;
            }

            Match match = Ipv6PlusIpv4Regex.Match(value);
            if (match.Success)
            {
                return ValidateIPv4(match.Groups[1].Value);
            }

            return false;
        }

        public static bool ValidateIPv4(string value)
        {
            string[] parts = value.Split('.');
            if (parts.Length != 4)
            {
                return false;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (string.IsNullOrEmpty(part))
                {
                    return false;
                }
                if (part.Length >= 2 && part[0] == '0')
                {
                    // Reject leading zeros. Don't want octal values.
                    return false;
                }

                if (!int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out int num)
                    || (num < 0 || num > 255))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
