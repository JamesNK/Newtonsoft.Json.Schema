#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Customizes <see cref="JSchema"/> generation for <see cref="Enum"/> to be a string value.
    /// </summary>
    public class StringEnumGenerationProvider : JSchemaGenerationProvider
    {
        /// <summary>
        /// Gets or sets a value indicating whether the written enum text should be camel case.
        /// </summary>
        /// <value><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</value>
        public bool CamelCaseText { get; set; }

        /// <summary>
        /// Gets a <see cref="JSchema"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="context">The <see cref="Type"/> and associated information used to generate a <see cref="JSchema"/>.</param>
        /// <returns>The generated <see cref="JSchema"/>.</returns>
        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            bool isNullable = ReflectionUtils.IsNullableType(context.ObjectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(context.ObjectType) : context.ObjectType;

            JSchema schema = new JSchema
            {
                Title = context.SchemaTitle,
                Description = context.SchemaDescription,
                Type = JSchemaType.String
            };

            if (isNullable && context.Required != Required.Always && context.Required != Required.DisallowNull)
            {
                schema.Type |= JSchemaType.Null;
                schema.Enum.Add(JValue.CreateNull());
            }

            string[] names = Enum.GetNames(t);

            foreach (string name in names)
            {
                string finalName = EnumUtils.ToEnumName(t, name, CamelCaseText);

                schema.Enum.Add(JValue.CreateString(finalName));
            }

            return schema;
        }

        /// <summary>
        /// Determines whether this instance can generate a <see cref="JSchema"/> for the specified object type.
        /// </summary>
        /// <param name="context">The <see cref="Type"/> and associated information.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanGenerateSchema(JSchemaTypeGenerationContext context)
        {
            bool isNullable = ReflectionUtils.IsNullableType(context.ObjectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(context.ObjectType) : context.ObjectType;

            return t.IsEnum();
        }
    }
}