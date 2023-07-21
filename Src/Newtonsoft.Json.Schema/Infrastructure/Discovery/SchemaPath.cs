#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    [DebuggerDisplay("ScopeId = {ScopeId}, ReferencedAs = {ReferencedAs}, Path = {Path}, DynamicScope = {DynamicScope}")]
    internal class SchemaPath
    {
        public readonly Uri ScopeId;
        public readonly Uri? ReferencedAs;
        public readonly string Path;
        public readonly Uri? DynamicScope;

        public SchemaPath(Uri scopeId, Uri? referencedAs, string path, Uri? dynamicScope)
        {
            ValidationUtils.Assert(scopeId != null);
            ValidationUtils.Assert(path != null);

            ScopeId = scopeId;
            ReferencedAs = referencedAs;
            Path = path;
            DynamicScope = dynamicScope;
        }
    }
}