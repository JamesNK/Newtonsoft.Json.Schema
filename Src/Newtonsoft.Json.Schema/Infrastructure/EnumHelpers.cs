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
        public static List<JSchemaType> GetAllJSchemaTypeEnums()
        {
            List<JSchemaType> result = new List<JSchemaType>();

            // These values should be kept in sync with Src/Newtonsoft.Json.Schema/JSchemaType.cs
            JSchemaType[] allValues = new[]
            {
                JSchemaType.None,
                JSchemaType.String,
                JSchemaType.Integer,
                JSchemaType.Boolean,
                JSchemaType.Object,
                JSchemaType.Array,
                JSchemaType.Null,
            };
            int[] values = allValues.Cast<int>().ToArray();
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
                        result.Add((JSchemaType)i);
                        break;
                    }
                }
            }

            return result;
        }
    }
}