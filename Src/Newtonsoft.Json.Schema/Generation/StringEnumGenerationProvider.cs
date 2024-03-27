#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Customizes <see cref="JSchema"/> generation for <see cref="Enum"/> to be a string value.
    /// </summary>
    public class StringEnumGenerationProvider : JSchemaGenerationProvider
    {
        private static readonly CamelCaseNamingStrategy _camelCaseNamingStrategy = new CamelCaseNamingStrategy();

        private bool _camelCaseText;
        private NamingStrategy? _nameStrategy;

        /// <summary>
        /// Gets or sets a value indicating whether the written enum text should be camel case.
        /// </summary>
        /// <value><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</value>
        [Obsolete("This property is obsolete and has been replaced by the NamingStrategy property.")]
        public bool CamelCaseText
        {
            get => _camelCaseText;
            set
            {
                _camelCaseText = value;
                // we use this to ensure compatibility with the old property.
                NamingStrategy = value ? _camelCaseNamingStrategy : null;
            }
        }

        /// <summary>
        /// Gets or sets the naming strategy used to resolve enum values.
        /// </summary>
        public NamingStrategy? NamingStrategy
        {
            get => _nameStrategy;
            set
            {
                _nameStrategy = value;
                // we use this to ensure compatibility with the old property.
                _camelCaseText = (typeof(CamelCaseNamingStrategy) == value?.GetType());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringEnumGenerationProvider"/> class.
        /// </summary>
        public StringEnumGenerationProvider() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringEnumGenerationProvider"/> class 
        /// with a specified naming strategy.
        /// </summary>
        public StringEnumGenerationProvider(NamingStrategy namingStrategy)
        {
            NamingStrategy = namingStrategy;
        }

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

            object? defaultValue = context.MemberProperty?.DefaultValue;
            if (defaultValue != null)
            {
                EnumUtils.TryToString(t, defaultValue, NamingStrategy, out string? finalName);

                schema.Default = JToken.FromObject(finalName ?? defaultValue.ToString());
            }

            EnumInfo enumValues = EnumUtils.GetEnumValuesAndNames(t, NamingStrategy);

            for (int i = 0; i < enumValues.Values.Length; i++)
            {
                string finalName = enumValues.ResolvedNames[i];

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