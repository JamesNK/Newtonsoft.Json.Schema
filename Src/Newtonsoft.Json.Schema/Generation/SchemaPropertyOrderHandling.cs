#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Specifies the schema property ordering for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    public enum SchemaPropertyOrderHandling
    {
        /// <summary>
        /// Use the default order of properties.
        /// </summary>
        Default,
        /// <summary>
        /// Alphabetical order of properties.
        /// </summary>
        Alphabetical
    }
}