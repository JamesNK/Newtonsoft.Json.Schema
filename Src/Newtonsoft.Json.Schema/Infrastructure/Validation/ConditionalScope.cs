#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class ConditionalScope : Scope
    {
        protected ConditionalContext ConditionalContext;
        protected SchemaScope ParentSchemaScope;
        protected static readonly Func<SchemaScope, bool> IsValidPredicate = IsValidPredicateInternal;

        public void Initialize(ContextBase context, SchemaScope parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            ParentSchemaScope = parent;
            ConditionalContext = ConditionalContext.Create(context);
        }

        public void InitializeScopes(JsonToken token, List<JSchema> schemas)
        {
            foreach (JSchema schema in schemas)
            {
                SchemaScope.CreateTokenScope(token, schema, ConditionalContext, this, InitialDepth);
            }
        }

        protected IEnumerable<SchemaScope> GetChildren()
        {
            foreach (Scope scope in Context.Scopes)
            {
                SchemaScope schemaScope = scope as SchemaScope;
                if (schemaScope != null)
                {
                    if (schemaScope.Parent == this)
                    {
                        yield return schemaScope;
                    }
                }
            }
        }

        protected IEnumerable<SchemaScope> GetValidChildren()
        {
            foreach (SchemaScope schemaScope in GetChildren())
            {
                if (schemaScope.IsValid)
                {
                    yield return schemaScope;
                }
            }
        }

        protected static bool IsValidPredicateInternal(SchemaScope s)
        {
            return s.IsValid;
        }
    }
}