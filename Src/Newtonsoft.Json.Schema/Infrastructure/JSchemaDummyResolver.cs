#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaDummyResolver : JSchemaResolver
    {
        public static readonly JSchemaDummyResolver Instance = new JSchemaDummyResolver();

        public override Stream? GetSchemaResource(ResolveSchemaContext context, SchemaReference reference)
        {
            return null;
        }
    }
}