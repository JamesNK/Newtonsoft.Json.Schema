#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Generation
{
    public class StringEnumGenerationProvider : JSchemaGenerationProvider
    {
        public bool CamelCaseText { get; set; }

        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            bool isNullable = ReflectionUtils.IsNullableType(context.ObjectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(context.ObjectType) : context.ObjectType;

            if (!t.IsEnum())
                return null;

            JSchema schema = new JSchema
            {
                Type = JSchemaType.String
            };

            if (isNullable && context.Required != Required.Always)
            {
                schema.Type |= JSchemaType.Null;
            }

            string[] names = Enum.GetNames(t);

            foreach (string name in names)
            {
                string finalName = EnumUtils.ToEnumName(t, name, CamelCaseText);

                schema.Enum.Add(JValue.CreateString(finalName));
            }

            return schema;
        }
    }
}