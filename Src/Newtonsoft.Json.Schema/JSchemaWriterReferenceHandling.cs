#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies reference handling options when writing schemas.
    /// </summary>
    public enum JSchemaWriterReferenceHandling
    {
        /// <summary>
        /// Always write references to previously encountered schemas.
        /// </summary>
        Always,
        /// <summary>
        /// Never write references to previously encountered schemas. Writing a schema with a recursive relationship will error.
        /// </summary>
        Never,
        /// <summary>
        /// Never write references to previously encountered schemas unless there is a recursive relationship,
        /// in which case a reference will be written.
        /// </summary>
        Auto
    }
}