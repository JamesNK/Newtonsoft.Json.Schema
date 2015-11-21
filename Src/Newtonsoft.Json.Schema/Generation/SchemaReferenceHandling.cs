#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Specifies whether generated schemas can be referenced for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    [Flags]
    public enum SchemaReferenceHandling
    {
        /// <summary>
        /// No schemas can be referenced.
        /// </summary>
        None,
        /// <summary>
        /// Object schemas can be referenced.
        /// </summary>
        Objects = 1,
        /// <summary>
        /// Array schemas can be referenced.
        /// </summary>
        Arrays = 2,
        /// <summary>
        /// Dictionary schemas can be referenced.
        /// </summary>
        Dictionaries = 3,
        /// <summary>
        /// All schemas can be referenced.
        /// </summary>
        All = Objects | Arrays | Dictionaries
    }
}