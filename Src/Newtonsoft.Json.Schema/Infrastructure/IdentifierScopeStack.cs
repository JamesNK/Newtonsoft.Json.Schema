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
    [DebuggerTypeProxy(typeof(IdentifierScopeStackDebugView))]
    internal sealed class IdentifierScopeStack : List<IIdentifierScope>
    {
        public IdentifierScopeStack() { }

        public IdentifierScopeStack(IdentifierScopeStack scopes) : base(scopes) { }

        private sealed class IdentifierScopeStackDebugView
        {
            private readonly IdentifierScopeStack _stack;

            public IdentifierScopeStackDebugView(IdentifierScopeStack stack)
            {
                _stack = stack;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public JsonIdentifierScope[] Items => _stack.Select(s => new JsonIdentifierScope(s.ScopeId, s.Root, s.DynamicAnchor, s.CouldBeDynamic)).ToArray();
        }

        public IdentifierScopeStack Clone()
        {
            return new IdentifierScopeStack(this);
        }
    }
}