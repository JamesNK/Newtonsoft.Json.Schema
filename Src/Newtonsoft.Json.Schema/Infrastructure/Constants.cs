#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class Constants
    {
        public static readonly List<JsonToken> NumberTokens = new List<JsonToken> { JsonToken.Integer, JsonToken.Float };
        public static readonly List<JsonToken> SchemaTokens = new List<JsonToken> { JsonToken.StartObject, JsonToken.Boolean };
        public static readonly List<JsonToken> ItemsDraft2020_12Tokens = new List<JsonToken> { JsonToken.StartArray };
        public static readonly List<JsonToken> ItemsDraft6Tokens = new List<JsonToken> { JsonToken.StartArray, JsonToken.StartObject, JsonToken.Boolean };
        public static readonly List<JsonToken> ItemsDraft4Tokens = new List<JsonToken> { JsonToken.StartArray, JsonToken.StartObject };
        public static readonly List<JsonToken> DependencyTokens = new List<JsonToken> { JsonToken.StartObject, JsonToken.StartArray, JsonToken.String, JsonToken.Boolean };
        public static readonly List<JsonToken> DependencyDraft4Tokens = new List<JsonToken> { JsonToken.StartObject, JsonToken.StartArray, JsonToken.String };
        public static readonly List<JsonToken> MaximumMinimumTokens = new List<JsonToken> { JsonToken.Integer, JsonToken.Float, JsonToken.Boolean };

        public static class PropertyNames
        {
            public const string Type = "type";
            public const string Properties = "properties";
            public const string Items = "items";
            public const string AdditionalItems = "additionalItems";
            public const string UnevaluatedItems = "unevaluatedItems";
            public const string Required = "required";
            public const string PatternProperties = "patternProperties";
            public const string AdditionalProperties = "additionalProperties";
            public const string UnevaluatedProperties = "unevaluatedProperties";
            public const string Requires = "requires";
            public const string Dependencies = "dependencies";
            public const string DependentSchemas = "dependentSchemas";
            public const string DependentRequired = "dependentRequired";
            public const string Minimum = "minimum";
            public const string Maximum = "maximum";
            public const string ExclusiveMinimum = "exclusiveMinimum";
            public const string ExclusiveMaximum = "exclusiveMaximum";
            public const string MinimumItems = "minItems";
            public const string MaximumItems = "maxItems";
            public const string MinimumContains = "minContains";
            public const string MaximumContains = "maxContains";
            public const string Pattern = "pattern";
            public const string MaximumLength = "maxLength";
            public const string MinimumLength = "minLength";
            public const string Enum = "enum";
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
            public const string Id = "$id";
            public const string Anchor = "$anchor";
            public const string IdDraft3 = "id";
            public const string UniqueItems = "uniqueItems";
            public const string MinimumProperties = "minProperties";
            public const string MaximumProperties = "maxProperties";
            // todo - rename class and this prop
            public const string PropertyNamesSchema = "propertyNames";
            public const string Const = "const";
            public const string Contains = "contains";
            public const string ContentEncoding = "contentEncoding";
            public const string ContentMediaType = "contentMediaType";
            public const string ReadOnly = "readOnly";
            public const string WriteOnly = "writeOnly";
            public const string PrefixItems = "prefixItems";

            public const string AnyOf = "anyOf";
            public const string AllOf = "allOf";
            public const string OneOf = "oneOf";
            public const string Not = "not";
            public const string If = "if";
            public const string Then = "then";
            public const string Else = "else";

            public const string Ref = "$ref";
            public const string RecursiveRef = "$recursiveRef";
            public const string RecursiveAnchor = "$recursiveAnchor";
            public const string DynamicRef = "$dynamicRef";
            public const string DynamicAnchor = "$dynamicAnchor";
            public const string Schema = "$schema";

            public const string Definitions = "definitions";
            public const string Defs = "$defs";

            public static bool IsDefinition(string name)
            {
                return string.Equals(name, Definitions, StringComparison.Ordinal)
                    || string.Equals(name, Defs, StringComparison.Ordinal);
            }
        }

        public static class Formats
        {
            public const string Draft3Hostname = "host-name";
            public const string Draft3IPv4 = "ip-address";
            public const string Hostname = "hostname";
            public const string DateTime = "date-time";
            public const string Date = "date";
            public const string Time = "time";
            public const string UtcMilliseconds = "utc-millisec";
            public const string Regex = "regex";
            public const string Color = "color";
            public const string Style = "style";
            public const string Phone = "phone";
            public const string Uri = "uri";
            public const string UriReference = "uri-reference";
            public const string UriTemplate = "uri-template";
            public const string JsonPointer = "json-pointer";
            public const string IPv6 = "ipv6";
            public const string IPv4 = "ipv4";
            public const string Email = "email";
            public const string Duration = "duration";
            public const string Uuid = "uuid";
        }

        public static class Types
        {
            public const string String = "string";
            public const string Object = "object";
            public const string Integer = "integer";
            public const string Number = "number";
            public const string Null = "null";
            public const string Boolean = "boolean";
            public const string Array = "array";

            public const string Any = "any";
        }

        public static class ContentEncodings
        {
            public const string Base64 = "base64";
        }

        public static readonly IDictionary<string, JSchemaType> JSchemaTypeMapping = new Dictionary<string, JSchemaType>(StringComparer.Ordinal)
        {
            { Types.String, JSchemaType.String },
            { Types.Object, JSchemaType.Object },
            { Types.Integer, JSchemaType.Integer },
            { Types.Number, JSchemaType.Number },
            { Types.Null, JSchemaType.Null },
            { Types.Boolean, JSchemaType.Boolean },
            { Types.Array, JSchemaType.Array }
        };

        public const JSchemaType AnyType = JSchemaType.Array | JSchemaType.Object | JSchemaType.Boolean | JSchemaType.Integer | JSchemaType.Null | JSchemaType.Number | JSchemaType.String;

        public static class SchemaVersions
        {
            public static readonly Uri Draft3 = new Uri("http://json-schema.org/draft-03/schema#");
            public static readonly Uri Draft4 = new Uri("http://json-schema.org/draft-04/schema#");
            public static readonly Uri Draft6 = new Uri("http://json-schema.org/draft-06/schema#");
            public static readonly Uri Draft7 = new Uri("http://json-schema.org/draft-07/schema#");
            public static readonly Uri Draft2019_09 = new Uri("https://json-schema.org/draft/2019-09/schema");
            public static readonly Uri Draft2020_12 = new Uri("https://json-schema.org/draft/2020-12/schema");
        }
    }
}