#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class ConditionalScope : Scope
    {
        protected ConditionalContext ConditionalContext;
        protected SchemaScope ParentSchemaScope;
        protected readonly List<SchemaScope> ChildScopes;

        protected ConditionalScope()
        {
            ChildScopes = new List<SchemaScope>();
        }

        internal string DebuggerDisplay
        {
            get { return GetType().Name + " - IsValid=" + IsValid() + " - Complete=" + Complete; }
        }

        public override void Initialize(ContextBase context, SchemaScope parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            ChildScopes.Clear();
            ParentSchemaScope = parent;
            ConditionalContext = ConditionalContext.Create(context);
        }

        public void InitializeScopes(JsonToken token, List<JSchema> schemas, int scopeIndex)
        {
            foreach (JSchema schema in schemas)
            {
                // cache this for performance
                int scopeCurrentIndex = scopeIndex;

                // check to see whether a scope with the same schema exists
                SchemaScope childScope = GetExistingSchemaScope(schema, ref scopeCurrentIndex);

                if (childScope == null)
                {
                    childScope = SchemaScope.CreateTokenScope(token, schema, ConditionalContext, null, InitialDepth);
                }

                ChildScopes.Add(childScope);
            }
        }

        internal abstract bool? IsValid();

        private SchemaScope GetExistingSchemaScope(JSchema schema, ref int scopeCurrentIndex)
        {
            for (int i = Context.Scopes.Count - 1; i >= 0; i--)
            {
                if (Context.Scopes[i] is SchemaScope scope)
                {
                    if (scope.InitialDepth == InitialDepth)
                    {
                        if (!scope.Complete && scope.Schema == schema)
                        {
                            if (i < scopeCurrentIndex)
                            {
                                // existing schema is before conditional scope
                                // move it so conditional scope is evaluated after existing schema
                                Context.Scopes.RemoveAt(i);
                                Context.Scopes.Insert(scopeCurrentIndex, scope);

                                // decrement index because the schema before current scope has been moved to after
                                scopeCurrentIndex--;
                            }

                            return scope;
                        }
                    }
                    else if (scope.InitialDepth < InitialDepth)
                    {
                        break;
                    }
                }
            }

            return null;
        }

        protected int GetChildrenValidCount()
        {
            int count = 0;
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];

                if (schemaScope.IsValid)
                {
                    count++;
                }
            }

            return count;
        }

        protected bool GetChildrenAnyValid()
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];

                if (schemaScope.IsValid)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool GetChildrenAllValid()
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];

                if (!schemaScope.IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        protected SchemaScope GetSchemaScopeBySchema(JSchema schema)
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];

                if (schemaScope.Schema == schema)
                {
                    return schemaScope;
                }
            }

            return null;
        }
    }
}