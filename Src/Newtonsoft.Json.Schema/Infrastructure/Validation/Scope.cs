#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class Scope
    {
        public int InitialDepth;
        public ContextBase Context;
        public Scope Parent;
        public bool Complete;

        protected Scope(ContextBase context, Scope parent, int initialDepth)
        {
            Context = context;
            Parent = parent;
            InitialDepth = initialDepth;
        }

        internal virtual void RaiseError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            // mark all parent SchemaScopes as invalid
            Scope current = this;
            SchemaScope parentSchemaScope;
            while ((parentSchemaScope = current.Parent as SchemaScope) != null)
            {
                if (!parentSchemaScope.IsValid)
                    break;

                parentSchemaScope.IsValid = false;
                current = parentSchemaScope;
            }

            Context.RaiseError(message, errorType, schema, value, childErrors);
        }

        public void EvaluateToken(JsonToken token, object value, int depth)
        {
            if (EvaluateTokenCore(token, value, depth))
                Complete = true;
        }

        protected abstract bool EvaluateTokenCore(JsonToken token, object value, int depth);
    }
}