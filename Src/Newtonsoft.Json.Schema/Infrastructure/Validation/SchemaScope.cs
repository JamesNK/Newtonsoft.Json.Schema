#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class SchemaScope : Scope
    {
        public JSchema Schema;
        public bool IsValid;

        protected SchemaScope(ContextBase context, Scope parent, int initialDepth, JSchema schema)
            : base(context, parent, initialDepth)
        {
            Schema = schema;
            IsValid = true;
        }

        public static SchemaScope CreateTokenScope(JsonToken token, JSchema schema, ContextBase context, Scope parent, int depth)
        {
            SchemaScope scope;

            switch (token)
            {
                case JsonToken.StartObject:
                    var objectScope = new ObjectScope(context, parent, depth, schema);
                    context.Scopes.Add(objectScope);

                    objectScope.InitializeScopes(token);

                    scope = objectScope;
                    break;
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                    scope = new ArrayScope(context, parent, depth, schema);
                    context.Scopes.Add(scope);
                    break;
                default:
                    scope = new PrimativeScope(context, parent, depth, schema);
                    context.Scopes.Add(scope);
                    break;
            }

            if (schema._allOf != null && schema._allOf.Count > 0)
            {
                AllOfScope allOfScope = new AllOfScope(scope, context, depth);
                context.Scopes.Add(allOfScope);

                allOfScope.InitializeScopes(token, schema._allOf);
            }
            if (schema._anyOf != null && schema._anyOf.Count > 0)
            {
                AnyOfScope anyOfScope = new AnyOfScope(scope, context, depth);
                context.Scopes.Add(anyOfScope);

                anyOfScope.InitializeScopes(token, schema._anyOf);
            }
            if (schema._oneOf != null && schema._oneOf.Count > 0)
            {
                OneOfScope oneOfScope = new OneOfScope(scope, context, depth);
                context.Scopes.Add(oneOfScope);

                oneOfScope.InitializeScopes(token, schema._oneOf);
            }
            if (schema.Not != null)
            {
                NotScope notScope = new NotScope(scope, context, depth);
                context.Scopes.Add(notScope);

                notScope.InitializeScopes(token, Enumerable.Repeat(schema.Not, 1));
            }

            return scope;
        }

        protected void EnsureEnum(JsonToken token, object value)
        {
            if (Schema._enum != null && Schema._enum.Count > 0)
            {
                if (JsonTokenHelpers.IsPrimitiveOrStartToken(token))
                {
                    if (Context.TokenWriter == null)
                    {
                        Context.TokenWriter = new JTokenWriter();
                        Context.TokenWriter.WriteToken(token, value);
                    }
                }

                if (JsonTokenHelpers.IsPrimitiveOrEndToken(token))
                {
                    if (!Schema._enum.ContainsValue(Context.TokenWriter.CurrentToken, JToken.EqualityComparer))
                    {
                        StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
                        Context.TokenWriter.CurrentToken.WriteTo(new JsonTextWriter(sw));

                        RaiseError($"Value {sw.ToString()} is not defined in enum.", ErrorType.Enum, Schema, value, null);
                    }
                }
            }
        }

        protected bool TestType(JSchema currentSchema, JSchemaType currentType, object value)
        {
            if (!JSchemaTypeHelpers.HasFlag(currentSchema.Type, currentType))
            {
                RaiseError($"Invalid type. Expected {currentSchema.Type} but got {currentType}.", ErrorType.Type, currentSchema, value, null);
                return false;
            }

            return true;
        }

        protected void CreateScopesAndEvaluateToken(JsonToken token, object value, int depth, JSchema schema)
        {
            int startCount = Context.Scopes.Count;

            CreateTokenScope(token, schema, Context, this, depth);

            int start = Context.Scopes.Count - 1;
            int end = startCount;

            for (int i = start; i >= end; i--)
            {
                Scope newScope = Context.Scopes[i];
                newScope.EvaluateToken(token, value, depth);
            }
        }

        internal override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            IsValid = false;

            base.RaiseError(message, errorType, schema, value, childErrors);
        }
    }
}