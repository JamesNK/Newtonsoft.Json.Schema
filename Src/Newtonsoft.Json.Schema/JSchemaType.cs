#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// The value types allowed by the <see cref="JSchema"/>.
    /// </summary>
    [Flags]
    public enum JSchemaType
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// String type.
        /// </summary>
        String = 1,

        /// <summary>
        /// Number type.
        /// </summary>
        Number = 2,

        /// <summary>
        /// Integer type.
        /// </summary>
        Integer = 4,

        /// <summary>
        /// Boolean type.
        /// </summary>
        Boolean = 8,

        /// <summary>
        /// Object type.
        /// </summary>
        Object = 16,

        /// <summary>
        /// Array type.
        /// </summary>
        Array = 32,

        /// <summary>
        /// Null type.
        /// </summary>
        Null = 64
    }
}