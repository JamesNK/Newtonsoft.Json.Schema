#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// JSON Schema error types.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// A 'multipleOf' error.
        /// </summary>
        MultipleOf,
        /// <summary>
        /// A 'maximum' error.
        /// </summary>
        Maximum,
        /// <summary>
        /// A 'minimum' error.
        /// </summary>
        Minimum,
        /// <summary>
        /// A 'maxLength' error.
        /// </summary>
        MaximumLength,
        /// <summary>
        /// A 'minLength' error.
        /// </summary>
        MinimumLength,
        /// <summary>
        /// A 'pattern' error.
        /// </summary>
        Pattern,
        /// <summary>
        /// An 'additionalItems' error.
        /// </summary>
        AdditionalItems,
        /// <summary>
        /// A 'items' error.
        /// </summary>
        Items,
        /// <summary>
        /// A 'maxItems' error.
        /// </summary>
        MaximumItems,
        /// <summary>
        /// A 'minItems' error.
        /// </summary>
        MinimumItems,
        /// <summary>
        /// A 'uniqueItems' error.
        /// </summary>
        UniqueItems,
        /// <summary>
        /// A 'maxProperties' error.
        /// </summary>
        MaximumProperties,
        /// <summary>
        /// A 'minProperties' error.
        /// </summary>
        MinimumProperties,
        /// <summary>
        /// A 'required' error.
        /// </summary>
        Required,
        /// <summary>
        /// An 'additionalProperties' error.
        /// </summary>
        AdditionalProperties,
        /// <summary>
        /// A 'dependencies' error.
        /// </summary>
        Dependencies,
        /// <summary>
        /// A 'enum' error.
        /// </summary>
        Enum,
        /// <summary>
        /// A 'type' error.
        /// </summary>
        Type,
        /// <summary>
        /// An 'allOf' error.
        /// </summary>
        AllOf,
        /// <summary>
        /// An 'anyOf' error.
        /// </summary>
        AnyOf,
        /// <summary>
        /// A 'oneOf' error.
        /// </summary>
        OneOf,
        /// <summary>
        /// A 'not' error.
        /// </summary>
        Not,
        /// <summary>
        /// A 'format' error.
        /// </summary>
        Format
    }
}