#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    internal abstract class ConditionalScope : Scope
    {
        protected ConditionalContext ConditionalContext = default!;
        protected SchemaScope ParentSchemaScope = default!;
        protected readonly List<SchemaScope> ChildScopes;

        protected ConditionalScope()
        {
            ChildScopes = new List<SchemaScope>();
        }

        internal string DebuggerDisplay() => GetType().Name + " - Complete=" + Complete
#if DEBUG
                                           + " - ScopeId=" + DebugId
#endif
        ;

        public override void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, ScopeType type)
        {
            ValidationUtils.Assert(parent != null);

            base.Initialize(context, parent, initialDepth, type);

            ChildScopes.Clear();
            ParentSchemaScope = parent;
            ConditionalContext = ConditionalContext.Create(context, parent.ShouldValidateUnevaluated());
#if DEBUG
            ConditionalContext.Scope = this;
#endif
        }

        public List<SchemaScope>? EvaluatedSchemas => ConditionalContext.ResolveEvaluatedSchemas();

        public void InitializeScopes(JsonToken token, List<JSchema> schemas, int scopeIndex)
        {
            foreach (JSchema schema in schemas)
            {
                InitializeScope(token, scopeIndex, schema, ConditionalContext);
            }
        }

        public void InitializeScope(JsonToken token, int scopeIndex, JSchema schema, ContextBase context)
        {
            // cache this for performance
            int scopeCurrentIndex = scopeIndex;

            // check to see whether a scope with the same schema exists
            SchemaScope? childScope = GetExistingSchemaScope(schema, ref scopeCurrentIndex);

            if (childScope == null)
            {
                childScope = SchemaScope.CreateTokenScope(token, schema, context, null, InitialDepth);
            }
            else
            {
                if (childScope.Context != context)
                {
                    // The schema scope needs to be part of a different conditional contexts.
                    // We need to create a composite so that errors are raised to both.
                    CompositeContext? compositeContext = childScope.Context as CompositeContext;
                    if (compositeContext == null)
                    {
                        compositeContext = new CompositeContext(context.Validator);
                        compositeContext.Contexts.Add(childScope.Context);
                        compositeContext.Contexts.Add(context);

                        childScope.Context = compositeContext;
                    }
                    else
                    {
                        if (!compositeContext.Contexts.Contains(context))
                        {
                            compositeContext.Contexts.Add(context);
                        }
                    }
                }
            }

#if DEBUG
            childScope.ConditionalParents.Add(this);
#endif

            ChildScopes.Add(childScope);
        }

        protected SchemaScope? GetExistingSchemaScope(JSchema schema, ref int scopeCurrentIndex)
        {
            for (int i = Context.Scopes.Count - 1; i >= 0; i--)
            {
                if (Context.Scopes[i] is SchemaScope scope)
                {
                    if (scope.InitialDepth == InitialDepth)
                    {
                        if (scope.Complete != CompleteState.Completed && scope.Schema == schema)
                        {
                            if (i < scopeCurrentIndex)
                            {
                                // existing schema is before conditional scope
                                // move it so conditional scope is evaluated after existing schema
                                Context.Scopes.RemoveAt(i);
                                Context.Scopes.Insert(scopeCurrentIndex, scope);

#if DEBUG
                                // sanity check that moving the scope won't cause order of evaluation errors
                                for (int j = scopeCurrentIndex - 1; j >= 0; j--)
                                {
                                    if (Context.Scopes[j].Parent == scope)
                                    {
                                        throw new Exception("Child will be evaluated after parent.");
                                    }
                                }
#endif

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

        protected bool TryGetChildrenValidCount(JsonToken token, object? value, int depth, out int validCount)
        {
            validCount = 0;
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];
                if (!AssertScopeComplete(schemaScope, token, value, depth))
                {
                    return false;
                }

                if (schemaScope.IsValid)
                {
                    validCount++;
                }
            }

            return true;
        }

        protected bool TryGetChildrenAnyValid(JsonToken token, object? value, int depth, out bool anyValid)
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];
                if (!AssertScopeComplete(schemaScope, token, value, depth))
                {
                    anyValid = default;
                    return false;
                }

                if (schemaScope.IsValid)
                {
                    anyValid = true;
                    return true;
                }
            }

            anyValid = false;
            return true;
        }

        protected bool TryGetChildrenAllValid(JsonToken token, object? value, int depth, out bool allValid)
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                SchemaScope schemaScope = ChildScopes[i];
                if (!AssertScopeComplete(schemaScope, token, value, depth))
                {
                    allValid = default;
                    return false;
                }

                if (!schemaScope.IsValid)
                {
                    allValid = false;
                    return true;
                }
            }

            allValid = true;
            return true;
        }

        protected bool TryGetSchemaScopeBySchema(JSchema schema, JsonToken token, object? value, int depth, [NotNullWhen(true)] out SchemaScope? schemaScope)
        {
            for (int i = 0; i < ChildScopes.Count; i++)
            {
                schemaScope = ChildScopes[i];

                if (schemaScope.Schema == schema)
                {
                    if (!AssertScopeComplete(schemaScope, token, value, depth))
                    {
                        schemaScope = null;
                        return false;
                    }

                    return true;
                }
            }

            throw new InvalidOperationException("Expected to find schema scope for schema.");
        }

        private bool AssertScopeComplete(SchemaScope schemaScope, JsonToken token, object? value, int depth)
        {
            switch (schemaScope.Complete)
            {
                case CompleteState.Incomplete:
                    // the schema scope that the conditional scope depends on may not be complete because it has be re-ordered
                    // schema scope will be at the same depth at the conditional so evaluate it immediately
                    schemaScope.Complete = CompleteState.Completing;
                    schemaScope.EvaluateToken(token, value, depth);

#if DEBUG
                    if (schemaScope.Complete != CompleteState.Completed)
                    {
                        throw new Exception("Schema scope {0} is not complete.".FormatWith(CultureInfo.InvariantCulture, schemaScope.DebugId));
                    }
#endif
                    return true;
                case CompleteState.Completing:
                    return false;
                case CompleteState.Completed:
                    return true;
                default:
                    throw new InvalidOperationException("Unexpected complete state.");
            }
        }

        protected void RaiseCircularDependencyError(ErrorType errorType)
        {
            RaiseError($"Conditional schema has a circular dependency and can't be evaluated.", errorType, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
        }
    }
}