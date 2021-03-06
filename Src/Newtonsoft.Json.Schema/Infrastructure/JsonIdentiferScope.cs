#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JsonIdentiferScope : IIdentiferScope
    {
        public static readonly JsonIdentiferScope Empty = new(null, false, null);

        public Uri? ScopeId { get; }
        public bool Root { get; }
        public string? DynamicAnchor { get; }

        public JsonIdentiferScope(Uri? id, bool root, string? dynamicAnchor)
        {
            ScopeId = id;
            Root = root;
            DynamicAnchor = dynamicAnchor;
        }
    }
}