#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(IdentiferScopeStackDebugView))]
    internal sealed class IdentiferScopeStack : List<IIdentiferScope>
    {
        private sealed class IdentiferScopeStackDebugView
        {
            private readonly IdentiferScopeStack _stack;

            public IdentiferScopeStackDebugView(IdentiferScopeStack stack)
            {
                _stack = stack;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public JsonIdentiferScope[] Items => _stack.Select(s => new JsonIdentiferScope(s.ScopeId, s.Root, s.DynamicAnchor)).ToArray();
        }
    }
}