#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class JSchemaConstants
    {
        public const string TypePropertyName = "type";
        public const string PropertiesPropertyName = "properties";
        public const string ItemsPropertyName = "items";
        public const string AdditionalItemsPropertyName = "additionalItems";
        public const string RequiredPropertyName = "required";
        public const string PatternPropertiesPropertyName = "patternProperties";
        public const string AdditionalPropertiesPropertyName = "additionalProperties";
        public const string RequiresPropertyName = "requires";
        public const string MinimumPropertyName = "minimum";
        public const string MaximumPropertyName = "maximum";
        public const string ExclusiveMinimumPropertyName = "exclusiveMinimum";
        public const string ExclusiveMaximumPropertyName = "exclusiveMaximum";
        public const string MinimumItemsPropertyName = "minItems";
        public const string MaximumItemsPropertyName = "maxItems";
        public const string PatternPropertyName = "pattern";
        public const string MaximumLengthPropertyName = "maxLength";
        public const string MinimumLengthPropertyName = "minLength";
        public const string EnumPropertyName = "enum";
        public const string ReadOnlyPropertyName = "readonly";
        public const string TitlePropertyName = "title";
        public const string DescriptionPropertyName = "description";
        public const string FormatPropertyName = "format";
        public const string DefaultPropertyName = "default";
        public const string TransientPropertyName = "transient";
        public const string DivisibleByPropertyName = "divisibleBy";
        public const string HiddenPropertyName = "hidden";
        public const string DisallowPropertyName = "disallow";
        public const string ExtendsPropertyName = "extends";
        public const string IdPropertyName = "id";
        public const string UniqueItemsPropertyName = "uniqueItems";

        public const string OptionValuePropertyName = "value";
        public const string OptionLabelPropertyName = "label";

        public static readonly IDictionary<string, JSchemaType> JsonSchemaTypeMapping = new Dictionary<string, JSchemaType>
        {
            { "string", JSchemaType.String },
            { "object", JSchemaType.Object },
            { "integer", JSchemaType.Integer },
            { "number", JSchemaType.Float },
            { "null", JSchemaType.Null },
            { "boolean", JSchemaType.Boolean },
            { "array", JSchemaType.Array },
            { "any", JSchemaType.Any }
        };
    }
}