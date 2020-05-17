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
        public ConditionalContext ThenContext;
        public ConditionalContext ElseContext;

        public JSchema If;
        public JSchema Then;
        public JSchema Else;

        public override void Initialize(ContextBase context, SchemaScope parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            If = null;
            Then = null;
            Else = null;

            ThenContext = null;
            ElseContext = null;
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            SchemaScope ifScope = GetSchemaScopeBySchema(If, token, value, depth);

            if (ifScope.IsValid)
            {
                ConditionalContext.TrackEvaluatedSchema(ifScope.Schema);

                if (Then != null)
                {
                    SchemaScope thenScope = GetSchemaScopeBySchema(Then, token, value, depth);

                    if (!thenScope.IsValid)
                    {
                        ConditionalContext context = (ConditionalContext)thenScope.Context;
                        RaiseError($"JSON does not match schema from 'then'.", ErrorType.Then, Then, null, context.Errors);
                    }
                    else
                    {
                        ConditionalContext.TrackEvaluatedSchema(thenScope.Schema);
                    }
                }
            }
            else
            {
                if (Else != null)
                {
                    SchemaScope elseScope = GetSchemaScopeBySchema(Else, token, value, depth);

                    if (!elseScope.IsValid)
                    {
                        ConditionalContext context = (ConditionalContext)elseScope.Context;
                        RaiseError($"JSON does not match schema from 'else'.", ErrorType.Else, Else, null, context.Errors);
                    }
                    else
                    {
                        ConditionalContext.TrackEvaluatedSchema(elseScope.Schema);
                    }
                }
            }

            return true;
        }

        public void InitializeScopes(JsonToken token, int scopeIndex)
        {
            InitializeScope(token, scopeIndex, If, ConditionalContext);
            if (Then != null)
            {
                InitializeScope(token, scopeIndex, Then, ThenContext);
            }
            if (Else != null)
            {
                InitializeScope(token, scopeIndex, Else, ElseContext);
            }
        }

        private void InitializeScope(JsonToken token, int scopeIndex, JSchema schema, ConditionalContext context)
        {
            // cache this for performance
            int scopeCurrentIndex = scopeIndex;

            // check to see whether a scope with the same schema exists
            SchemaScope childScope = GetExistingSchemaScope(schema, ref scopeCurrentIndex);

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