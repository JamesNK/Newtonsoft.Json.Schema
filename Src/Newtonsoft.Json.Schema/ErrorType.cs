#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Runtime.Serialization;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// JSON Schema error types.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Default ErrorType value.
        /// </summary>
        [EnumMember(Value = "none")]
        None = 0,

        /// <summary>
        /// A 'multipleOf' error.
        /// </summary>
        [EnumMember(Value = "multipleOf")]
        MultipleOf = 1,

        /// <summary>
        /// A 'maximum' error.
        /// </summary>
        [EnumMember(Value = "maximum")]
        Maximum = 2,

        /// <summary>
        /// A 'minimum' error.
        /// </summary>
        [EnumMember(Value = "minimum")]
        Minimum = 3,

        /// <summary>
        /// A 'maxLength' error.
        /// </summary>
        [EnumMember(Value = "maxLength")]
        MaximumLength = 4,

        /// <summary>
        /// A 'minLength' error.
        /// </summary>
        [EnumMember(Value = "minLength")]
        MinimumLength = 5,

        /// <summary>
        /// A 'pattern' error.
        /// </summary>
        [EnumMember(Value = "pattern")]
        Pattern = 6,

        /// <summary>
        /// An 'additionalItems' error.
        /// </summary>
        [EnumMember(Value = "additionalItems")]
        AdditionalItems = 7,

        /// <summary>
        /// A 'items' error.
        /// </summary>
        [EnumMember(Value = "items")]
        Items = 8,

        /// <summary>
        /// A 'maxItems' error.
        /// </summary>
        [EnumMember(Value = "maxItems")]
        MaximumItems = 9,

        /// <summary>
        /// A 'minItems' error.
        /// </summary>
        [EnumMember(Value = "minItems")]
        MinimumItems = 10,

        /// <summary>
        /// A 'uniqueItems' error.
        /// </summary>
        [EnumMember(Value = "uniqueItems")]
        UniqueItems = 11,

        /// <summary>
        /// A 'maxProperties' error.
        /// </summary>
        [EnumMember(Value = "maxProperties")]
        MaximumProperties = 12,

        /// <summary>
        /// A 'minProperties' error.
        /// </summary>
        [EnumMember(Value = "minProperties")]
        MinimumProperties = 13,

        /// <summary>
        /// A 'required' error.
        /// </summary>
        [EnumMember(Value = "required")]
        Required = 14,

        /// <summary>
        /// An 'additionalProperties' error.
        /// </summary>
        [EnumMember(Value = "additionalProperties")]
        AdditionalProperties = 15,

        /// <summary>
        /// A 'dependencies' error.
        /// </summary>
        [EnumMember(Value = "dependencies")]
        Dependencies = 16,

        /// <summary>
        /// A 'enum' error.
        /// </summary>
        [EnumMember(Value = "enum")]
        Enum = 17,

        /// <summary>
        /// A 'type' error.
        /// </summary>
        [EnumMember(Value = "type")]
        Type = 18,

        /// <summary>
        /// An 'allOf' error.
        /// </summary>
        [EnumMember(Value = "allOf")]
        AllOf = 19,

        /// <summary>
        /// An 'anyOf' error.
        /// </summary>
        [EnumMember(Value = "anyOf")]
        AnyOf = 20,

        /// <summary>
        /// A 'oneOf' error.
        /// </summary>
        [EnumMember(Value = "oneOf")]
        OneOf = 21,

        /// <summary>
        /// A 'not' error.
        /// </summary>
        [EnumMember(Value = "not")]
        Not = 22,

        /// <summary>
        /// A 'format' error.
        /// </summary>
        [EnumMember(Value = "format")]
        Format = 23,

        /// <summary>
        /// An 'id' error.
        /// </summary>
        [EnumMember(Value = "id")]
        Id = 24,

        /// <summary>
        /// A 'patternProperties' error.
        /// </summary>
        [EnumMember(Value = "patternProperties")]
        PatternProperties = 25,

        /// <summary>
        /// A 'validator' error.
        /// </summary>
        [EnumMember(Value = "validator")]
        Validator = 26,

        /// <summary>
        /// A 'valid' error.
        /// </summary>
        [EnumMember(Value = "valid")]
        Valid = 27,

        /// <summary>
        /// A 'const' error.
        /// </summary>
        [EnumMember(Value = "const")]
        Const = 28,

        /// <summary>
        /// A 'contains' error.
        /// </summary>
        [EnumMember(Value = "contains")]
        Contains = 29
    }
}