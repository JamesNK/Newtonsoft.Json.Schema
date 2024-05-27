#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    [DebuggerDisplay("ScopeId = {ScopeId}, Root = {Root}, DynamicAnchor = {DynamicAnchor}, CouldBeDynamic = {CouldBeDynamic}")]
    internal class JsonIdentifierScope : IIdentifierScope
    {
        public static readonly JsonIdentifierScope Empty = new JsonIdentifierScope(null, false, null, false);

        public Uri? ScopeId { get; }
        public bool Root { get; }
        public string? DynamicAnchor { get; }
        public bool CouldBeDynamic { get; }

        public JsonIdentifierScope(Uri? id, bool root, string? dynamicAnchor, bool couldBeDynamic)
        {
            ScopeId = id;
            Root = root;
            DynamicAnchor = dynamicAnchor;
            CouldBeDynamic = couldBeDynamic;
        }
    }
}