#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal enum ScopeType
    {
        Object,
        Array,
        Primitive,
        AllOf,
        AnyOf,
        OneOf,
        Not
    }

    internal abstract class Scope
    {
        public int InitialDepth;
        public ContextBase Context;
        public Scope Parent;
        public ScopeType Type;
        public bool Complete;

        protected void Initialize(ContextBase context, Scope parent, int initialDepth, ScopeType type)
        {
            Context = context;
            Parent = parent;
            InitialDepth = initialDepth;
            Type = type;
            Complete = false;
        }

        internal virtual void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            // mark all parent SchemaScopes as invalid
            Scope current = this;
            SchemaScope parentSchemaScope;
            while ((parentSchemaScope = current.Parent as SchemaScope) != null)
            {
                if (!parentSchemaScope.IsValid)
                {
                    break;
                }

                parentSchemaScope.IsValid = false;
                current = parentSchemaScope;
            }

            Context.RaiseError(message, errorType, schema, value, childErrors);
        }

        public void EvaluateToken(JsonToken token, object value, int depth)
        {
            if (EvaluateTokenCore(token, value, depth))
            {
                Complete = true;
            }
        }

        protected abstract bool EvaluateTokenCore(JsonToken token, object value, int depth);
    }
}