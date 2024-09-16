#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Threading;

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
        Not,
        IfThenElse,
        Ref,
        DependentSchema
    }

    internal enum CompleteState
    {
        Incomplete,
        Completing,
        Completed
    }

    internal abstract class Scope
    {
#if DEBUG
        internal static long LastDebugId;
        internal long DebugId { get; set; }
#endif

        public int InitialDepth;
        public ContextBase Context = default!;
        public SchemaScope? Parent;
        public ScopeType Type;
        public CompleteState Complete;

        protected bool HasCircularReference;

        public virtual void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, ScopeType type)
        {
            Context = context;
            Parent = parent;
            InitialDepth = initialDepth;
            Type = type;
            Complete = CompleteState.Incomplete;

#if DEBUG
            Interlocked.Increment(ref LastDebugId);
            DebugId = LastDebugId;
#endif
        }

        internal virtual void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
        {
            if (HasCircularReference)
            {
                // Don't record errors after a circular reference is detected. They could create circular references between themselves.
                return;
            }

            // mark all parent SchemaScopes as invalid
            Scope current = this;
            SchemaScope? parentSchemaScope;
            while ((parentSchemaScope = current.Parent) != null)
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

        public void EvaluateToken(JsonToken token, object? value, int depth)
        {
            if (EvaluateTokenCore(token, value, depth))
            {
                Complete = CompleteState.Completed;
            }
        }

        protected abstract bool EvaluateTokenCore(JsonToken token, object? value, int depth);
    }
}