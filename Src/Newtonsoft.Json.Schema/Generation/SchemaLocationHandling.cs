#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Specifies the location of referenced schemas for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    public enum SchemaLocationHandling
    {
        /// <summary>
        /// Referenced schemas are placed in the root schema's definitions collection.
        /// </summary>
        Definitions,
        /// <summary>
        /// Referenced schemas are inline.
        /// </summary>
        Inline
    }
}