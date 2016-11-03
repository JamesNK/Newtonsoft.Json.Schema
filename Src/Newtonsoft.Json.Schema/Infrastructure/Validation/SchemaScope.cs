#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class SchemaScope : Scope
    {
        public JSchema Schema;
        public bool IsValid;

        protected void InitializeSchema(JSchema schema)
        {
            Schema = schema;
            IsValid = true;
        }

        internal string DebuggerDisplay
        {
            get { return GetType().Name + " - IsValid=" + IsValid + " - Complete=" + Complete; }
        }

        public static SchemaScope CreateTokenScope(JsonToken token, JSchema schema, ContextBase context, SchemaScope parent, int depth)
        {
            SchemaScope scope;

            switch (token)
            {
                case JsonToken.StartObject:
                    ObjectScope objectScope = context.Validator.GetCachedScope<ObjectScope>(ScopeType.Object);
                    if (objectScope == null)
                    {
                        objectScope = new ObjectScope();
                    }
                    objectScope.Initialize(context, parent, depth, schema);
                    context.Scopes.Add(objectScope);

                    objectScope.InitializeScopes(token);

                    scope = objectScope;
                    break;
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                    ArrayScope arrayScope = context.Validator.GetCachedScope<ArrayScope>(ScopeType.Array);
                    if (arrayScope == null)
                    {
                        arrayScope = new ArrayScope();
                    }
                    arrayScope.Initialize(context, parent, depth, schema);

                    context.Scopes.Add(arrayScope);
                    scope = arrayScope;
                    break;
                default:
                    PrimativeScope primativeScope = context.Validator.GetCachedScope<PrimativeScope>(ScopeType.Primitive);
                    if (primativeScope == null)
                    {
                        primativeScope = new PrimativeScope();
                    }
                    primativeScope.Initialize(context, parent, depth, schema);

                    scope = primativeScope;
                    context.Scopes.Add(scope);
                    break;
            }

            if (!schema._allOf.IsNullOrEmpty())
            {
                AllOfScope allOfScope = context.Validator.GetCachedScope<AllOfScope>(ScopeType.AllOf);
                if (allOfScope == null)
                {
                    allOfScope = new AllOfScope();
                }
                allOfScope.Initialize(context, scope, depth, ScopeType.AllOf);
                context.Scopes.Add(allOfScope);

                allOfScope.InitializeScopes(token, schema._allOf.GetInnerList());
            }
            if (!schema._anyOf.IsNullOrEmpty())
            {
                AnyOfScope anyOfScope = context.Validator.GetCachedScope<AnyOfScope>(ScopeType.AnyOf);
                if (anyOfScope == null)
                {
                    anyOfScope = new AnyOfScope();
                }
                anyOfScope.Initialize(context, scope, depth, ScopeType.AnyOf);
                context.Scopes.Add(anyOfScope);

                anyOfScope.InitializeScopes(token, schema._anyOf.GetInnerList());
            }
            if (!schema._oneOf.IsNullOrEmpty())
            {
                OneOfScope oneOfScope = context.Validator.GetCachedScope<OneOfScope>(ScopeType.OneOf);
                if (oneOfScope == null)
                {
                    oneOfScope = new OneOfScope();
                }
                oneOfScope.Initialize(context, scope, depth, ScopeType.OneOf);
                context.Scopes.Add(oneOfScope);

                oneOfScope.InitializeScopes(token, schema._oneOf.GetInnerList());
            }
            if (schema.Not != null)
            {
                NotScope notScope = context.Validator.GetCachedScope<NotScope>(ScopeType.Not);
                if (notScope == null)
                {
                    notScope = new NotScope();
                }
                notScope.Initialize(context, scope, depth, ScopeType.Not);
                context.Scopes.Add(notScope);

                notScope.InitializeScopes(token, new List<JSchema> { schema.Not });
            }

            return scope;
        }

        protected void EnsureEnum(JsonToken token, object value)
        {
            bool isEnum = !Schema._enum.IsNullOrEmpty();
            bool hasValidator = !Schema._validators.IsNullOrEmpty();

            if (isEnum || hasValidator)
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
                    JToken currentToken = Context.TokenWriter.CurrentToken;

                    if (isEnum)
                    {
                        bool defined = JsonTokenHelpers.Contains(Schema._enum, currentToken);

                        if (!defined)
                        {
                            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
                            currentToken.WriteTo(new JsonTextWriter(sw));

                            RaiseError($"Value {sw.ToString()} is not defined in enum.", ErrorType.Enum, Schema, value, null);
                        }
                    }

                    if (hasValidator)
                    {
                        JsonValidatorContext context = new JsonValidatorContext(this, Schema);

                        foreach (JsonValidator validator in Schema._validators)
                        {
                            validator.Validate(currentToken, context);
                        }
                    }
                }
            }
        }

        protected bool TestType(JSchema currentSchema, JSchemaType currentType)
        {
            return TestType<object>(currentSchema, currentType, null);
        }

        protected bool TestType<T>(JSchema currentSchema, JSchemaType currentType, T value)
        {
            if (!JSchemaTypeHelpers.HasFlag(currentSchema.Type, currentType))
            {
                RaiseError($"Invalid type. Expected {currentSchema.Type.Value.GetDisplayText()} but got {currentType.GetDisplayText()}.", ErrorType.Type, currentSchema, value, null);
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