#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("Id = {ScopeId}, Path = {Path}")]
    internal class SchemaPath
    {
        public readonly Uri ScopeId;
        public readonly Uri? ReferencedAs;
        public readonly string Path;

        public SchemaPath(Uri scopeId, Uri? referencedAs, string path)
        {
            ValidationUtils.Assert(scopeId != null);
            ValidationUtils.Assert(path != null);

            ScopeId = scopeId;
            ReferencedAs = referencedAs;
            Path = path;
        }
    }
}