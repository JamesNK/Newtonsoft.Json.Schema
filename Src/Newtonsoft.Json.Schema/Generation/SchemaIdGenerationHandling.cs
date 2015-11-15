#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Specifies generated schema Id handling options for the <see cref="JSchemaGenerator"/>.
    /// </summary>
    public enum SchemaIdGenerationHandling
    {
        /// <summary>
        /// Do not generate a schema Id.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the .NET type name as the schema Id.
        /// </summary>
        TypeName = 1,

        /// <summary>
        /// Use the .NET full type name (namespace plus type name) as the schema Id.
        /// </summary>
        FullTypeName = 2,

        /// <summary>
        /// Use the assembly qualified .NET type name as the schema Id.
        /// </summary>
        AssemblyQualifiedName = 3,
    }
}