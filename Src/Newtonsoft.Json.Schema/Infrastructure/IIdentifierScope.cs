#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal interface IIdentifierScope
    {
        Uri? ScopeId { get; }
        bool Root { get; }
        string? DynamicAnchor { get; }
        bool CouldBeDynamic { get; }
    }
}