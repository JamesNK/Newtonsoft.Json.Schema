#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class IfThenElseScope : ConditionalScope
    {
        public ConditionalContext? IfContext;
        public ConditionalContext? ThenContext;
        public ConditionalContext? ElseContext;

        public JSchema? If;
        public JSchema? Then;
        public JSchema? Else;

        public override void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            If = null;
            Then = null;
            Else = null;

            IfContext = null;
            ThenContext = null;
            ElseContext = null;
        }

        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            if (!TryGetSchemaScopeBySchema(If!, token, value, depth, out SchemaScope? ifScope))
            {
                RaiseCircularDependencyError(ErrorType.None);
                return true;
            }

            if (ifScope.IsValid)
            {
                ConditionalContext.TrackEvaluatedSchemaScope(ifScope);

                if (Then != null)
                {
                    if (!TryGetSchemaScopeBySchema(Then, token, value, depth, out SchemaScope? thenScope))
                    {
                        RaiseCircularDependencyError(ErrorType.Then);
                        return true;
                    }

                    if (!thenScope.IsValid)
                    {
                        RaiseError($"JSON does not match schema from 'then'.", ErrorType.Then, Then, null, thenScope.GetValidationErrors());
                    }
                    else
                    {
                        TrackScope(thenScope);
                    }
                }
            }
            else
            {
                if (Else != null)
                {
                    if (!TryGetSchemaScopeBySchema(Else, token, value, depth, out SchemaScope? elseScope))
                    {
                        RaiseCircularDependencyError(ErrorType.Else);
                        return true;
                    }

                    if (!elseScope.IsValid)
                    {
                        RaiseError($"JSON does not match schema from 'else'.", ErrorType.Else, Else, null, elseScope.GetValidationErrors());
                    }
                    else
                    {
                        TrackScope(elseScope);
                    }
                }
            }

            return true;
        }

        private void TrackScope(SchemaScope scope)
        {
            ConditionalContext.TrackEvaluatedSchemaScope(scope);
            if (scope.Context is ISchemaTracker tracker && !tracker.EvaluatedSchemas.IsNullOrEmpty())
            {
                foreach (var item in tracker.EvaluatedSchemas)
                {
                    ConditionalContext.TrackEvaluatedSchemaScope(item);
                }
            }
        }

        public void InitializeScopes(JsonToken token, int scopeIndex)
        {
            InitializeScope(token, scopeIndex, If!, IfContext!);
            if (Then != null)
            {
                InitializeScope(token, scopeIndex, Then, ThenContext!);
            }
            if (Else != null)
            {
                InitializeScope(token, scopeIndex, Else, ElseContext!);
            }
        }

        private void InitializeScope(JsonToken token, int scopeIndex, JSchema schema, ConditionalContext context)
        {
            // cache this for performance
            int scopeCurrentIndex = scopeIndex;

            // check to see whether a scope with the same schema exists
            SchemaScope? childScope = GetExistingSchemaScope(schema, ref scopeCurrentIndex);

            if (childScope == null)
            {
                childScope = SchemaScope.CreateTokenScope(token, schema, context, null, InitialDepth);
            }

#if DEBUG
            childScope.ConditionalParents.Add(this);
#endif

            ChildScopes.Add(childScope);
        }
    }
}