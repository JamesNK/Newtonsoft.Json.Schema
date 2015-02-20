using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                throw new ArgumentNullException("values");

            if (separator == null)
                separator = string.Empty;

            using (IEnumerator<T> enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return string.Empty;

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