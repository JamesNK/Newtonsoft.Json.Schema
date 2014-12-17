#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class Constants
    {
        public static readonly List<JsonToken> NumberTokens = new List<JsonToken> { JsonToken.Integer, JsonToken.Float };
        public static readonly List<JsonToken> DependencyTokens = new List<JsonToken> { JsonToken.StartObject, JsonToken.StartArray, JsonToken.String };

        internal static class PropertyNames
        {
            public const string Type = "type";
            public const string Properties = "properties";
            public const string Items = "items";
            public const string AdditionalItems = "additionalItems";
            public const string Required = "required";
            public const string PatternProperties = "patternProperties";
            public const string AdditionalProperties = "additionalProperties";
            public const string Requires = "requires";
            public const string Dependencies = "dependencies";
            public const string Minimum = "minimum";
            public const string Maximum = "maximum";
            public const string ExclusiveMinimum = "exclusiveMinimum";
            public const string ExclusiveMaximum = "exclusiveMaximum";
            public const string MinimumItems = "minItems";
            public const string MaximumItems = "maxItems";
            public const string Pattern = "pattern";
            public const string MaximumLength = "maxLength";
            public const string MinimumLength = "minLength";
            public const string Enum = "enum";
            public const string ReadOnly = "readonly";
            public const string Title = "title";
            public const string Description = "description";
            public const string Format = "format";
            public const string Default = "default";
            public const string Transient = "transient";
            public const string DivisibleBy = "divisibleBy";
            public const string MultipleOf = "multipleOf";
            public const string Hidden = "hidden";
            public const string Disallow = "disallow";
            public const string Extends = "extends";
            public const string Id = "id";
            public const string UniqueItems = "uniqueItems";
            public const string MinimumProperties = "minProperties";
            public const string MaximumProperties = "maxProperties";

            public const string AnyOf = "anyOf";
            public const string AllOf = "allOf";
            public const string OneOf = "oneOf";
            public const string Not = "not";

            public const string Ref = "$ref";
        }

        public const string OptionValue = "value";
        public const string OptionLabel = "label";

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