#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class EnumHelpers
    {
        public static List<T> GetAllEnums<T>() where T : struct, Enum
        {
            List<T> result = new List<T>();
#if NET5_0_OR_GREATER
            int[] values = Enum.GetValues<T>().Cast<int>().ToArray();
#else
            int[] values = Enum.GetValues(typeof(T)).Cast<int>().ToArray();
#endif
            int[] valuesInverted = values.Select(v => ~v).ToArray();
            int max = 0;
            for (int i = 0; i < values.Length; i++)
            {
                max |= values[i];
            }

            for (int i = 0; i <= max; i++)
            {
                int unaccountedBits = i;
                for (int j = 0; j < valuesInverted.Length; j++)
                {
                    unaccountedBits &= valuesInverted[j];
                    if (unaccountedBits == 0)
                    {
                        result.Add((T)(object)i);
                        break;
                    }
                }
            }

            return result;
        }
    }
}