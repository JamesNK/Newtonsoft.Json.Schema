#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class SchemaScope : Scope
    {
        public JSchema Schema = default!;
        public bool IsValid;
        public List<ConditionalScope>? ConditionalChildren;
#if DEBUG
        internal List<SchemaScope> SchemaChildren = new List<SchemaScope>();
        internal List<ConditionalScope> ConditionalParents = new List<ConditionalScope>();
#endif

        protected void InitializeSchema(JSchema schema)
        {
            Schema = schema;
            IsValid = true;
            ConditionalChildren?.Clear();
#if DEBUG
            SchemaChildren.Clear();
            ConditionalParents.Clear();
#endif
        }

#if DEBUG
        public override void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            if (parent != null)
            {
                parent.SchemaChildren.Add(this);
            }
        }
#endif

        public virtual bool ShouldValidateUnevaluated()
        {
            return false;
        }

        public ConditionalContext CreateConditionalContext()
        {
            return ConditionalContext.Create(Context, ShouldValidateUnevaluated());
        }

        private void AddChildScope(ConditionalScope scope)
        {
            if (ConditionalChildren == null)
            {
                ConditionalChildren = new List<ConditionalScope>();
            }

            ConditionalChildren.Add(scope);
        }

        internal string DebuggerDisplay => GetType().Name + " - IsValid=" + IsValid + " - Complete=" + Complete
#if DEBUG
                                           + " - SchemaId=" + Schema.DebugId
                                           + " - ScopeId=" + DebugId
#endif
        ;

        public static SchemaScope CreateTokenScope(JsonToken token, JSchema schema, ContextBase context, SchemaScope? parent, int depth)
        {
            SchemaScope scope;

            switch (token)
            {
                case JsonToken.StartObject:
                    ObjectScope? objectScope = context.Validator.GetCachedScope<ObjectScope>(ScopeType.Object);
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
                    ArrayScope? arrayScope = context.Validator.GetCachedScope<ArrayScope>(ScopeType.Array);
                    if (arrayScope == null)
                    {
                        arrayScope = new ArrayScope();
                    }
                    arrayScope.Initialize(context, parent, depth, schema);

                    context.Scopes.Add(arrayScope);
                    scope = arrayScope;
                    break;
                default:
                    PrimativeScope? primativeScope = context.Validator.GetCachedScope<PrimativeScope>(ScopeType.Primitive);
                    if (primativeScope == null)
                    {
                        primativeScope = new PrimativeScope();
                    }
                    primativeScope.Initialize(context, parent, depth, schema);

                    scope = primativeScope;
                    context.Scopes.Add(scope);
                    break;
            }

            if (schema.Ref != null)
            {
                RefScope? refScope = context.Validator.GetCachedScope<RefScope>(ScopeType.Ref);
                if (refScope == null)
                {
                    refScope = new RefScope();
                }
                refScope.Initialize(context, scope, depth, ScopeType.Ref);
                scope.AddChildScope(refScope);

                refScope.InitializeScopes(token, new List<JSchema> { schema.Ref }, context.Scopes.Count - 1);
            }
            if (!schema._allOf.IsNullOrEmpty())
            {
                AllOfScope? allOfScope = context.Validator.GetCachedScope<AllOfScope>(ScopeType.AllOf);
                if (allOfScope == null)
                {
                    allOfScope = new AllOfScope();
                }
                allOfScope.Initialize(context, scope, depth, ScopeType.AllOf);
                scope.AddChildScope(allOfScope);

                allOfScope.InitializeScopes(token, schema._allOf.GetInnerList(), context.Scopes.Count - 1);
            }
            if (!schema._anyOf.IsNullOrEmpty())
            {
                AnyOfScope? anyOfScope = context.Validator.GetCachedScope<AnyOfScope>(ScopeType.AnyOf);
                if (anyOfScope == null)
                {
                    anyOfScope = new AnyOfScope();
                }
                anyOfScope.Initialize(context, scope, depth, ScopeType.AnyOf);
                scope.AddChildScope(anyOfScope);

                anyOfScope.InitializeScopes(token, schema._anyOf.GetInnerList(), context.Scopes.Count - 1);
            }
            if (!schema._oneOf.IsNullOrEmpty())
            {
                OneOfScope? oneOfScope = context.Validator.GetCachedScope<OneOfScope>(ScopeType.OneOf);
                if (oneOfScope == null)
                {
                    oneOfScope = new OneOfScope();
                }
                oneOfScope.Initialize(context, scope, depth, ScopeType.OneOf);
                scope.AddChildScope(oneOfScope);

                oneOfScope.InitializeScopes(token, schema._oneOf.GetInnerList(), context.Scopes.Count - 1);
            }
            if (schema.Not != null)
            {
                NotScope? notScope = context.Validator.GetCachedScope<NotScope>(ScopeType.Not);
                if (notScope == null)
                {
                    notScope = new NotScope();
                }
                notScope.Initialize(context, scope, depth, ScopeType.Not);
                scope.AddChildScope(notScope);

                notScope.InitializeScopes(token, new List<JSchema> { schema.Not }, context.Scopes.Count - 1);
            }
            // only makes sense to eval if/then/else when there is an if and either a then or a else
            if (schema.If != null && (schema.Then != null || schema.Else != null))
            {
                IfThenElseScope? ifThenElseScope = context.Validator.GetCachedScope<IfThenElseScope>(ScopeType.IfThenElse);
                if (ifThenElseScope == null)
                {
                    ifThenElseScope = new IfThenElseScope();
                }
                ifThenElseScope.Initialize(context, scope, depth, ScopeType.IfThenElse);
                scope.AddChildScope(ifThenElseScope);

                ifThenElseScope.If = schema.If;
                if (schema.Then != null)
                {
                    ifThenElseScope.Then = schema.Then;
                    ifThenElseScope.ThenContext = scope.CreateConditionalContext();
                }
                if (schema.Else != null)
                {
                    ifThenElseScope.Else = schema.Else;
                    ifThenElseScope.ElseContext = scope.CreateConditionalContext();
                }

                ifThenElseScope.InitializeScopes(token, context.Scopes.Count - 1);
            }

            return scope;
        }

        protected void EnsureValid(object? value)
        {
            EnsureValid(this, Schema, value);
        }

        internal static void EnsureValid(SchemaScope scope, JSchema schema, object? value)
        {
            if (schema.HasReference)
            {
                throw new JSchemaException("Schema has unresolved reference '{0}'. All references must be resolved before a schema can be validated.".FormatWith(CultureInfo.InvariantCulture, schema.Reference!.OriginalString));
            }

            if (schema.Valid != null && !schema.Valid.Value)
            {
                scope.RaiseError($"Schema always fails validation.", ErrorType.Valid, schema, value, null);
            }
        }

        protected void ValidateConditionalChildren(JsonToken token, object? value, int depth)
        {
            if (!ConditionalChildren.IsNullOrEmpty())
            {
                for (int i = 0; i < ConditionalChildren.Count; i++)
                {
                    ConditionalScope conditionalScope = ConditionalChildren[i];

                    conditionalScope.EvaluateToken(token, value, depth);
                    OnConditionalScopeValidated(conditionalScope);

                    Context.Validator.ReturnScopeToCache(conditionalScope);
                }
            }
        }

        protected virtual void OnConditionalScopeValidated(ConditionalScope conditionalScope)
        {
        }

        protected void EnsureEnum(JsonToken token, object? value)
        {
            bool isEnum = !Schema._enum.IsNullOrEmpty();
            bool hasConst = Schema.Const != null;
            bool hasValidator = !Schema._validators.IsNullOrEmpty();

            if (isEnum || hasConst || hasValidator)
            {
                if (JsonTokenHelpers.IsPrimitiveOrStartToken(token))
                {
                    if (Context.TokenWriter == null)
                    {
                        Context.TokenWriter = new JTokenWriter();
                        Context.TokenWriter.WriteToken(token, value);
                    }
                }
                else if (token == JsonToken.PropertyName)
                {
                    if (Context.TokenWriter == null)
                    {
                        Context.TokenWriter = new JTokenWriter();
                        Context.TokenWriter.WriteToken(JsonToken.String, value);
                    }
                }

                if (JsonTokenHelpers.IsPrimitiveOrEndToken(token) || token == JsonToken.PropertyName)
                {
                    ValidationUtils.Assert(Context.TokenWriter != null);

                    JToken currentToken = Context.TokenWriter.CurrentToken!;

                    if (isEnum)
                    {
                        JToken resolvedToken = (currentToken is JProperty property)
                            ? new JValue(property.Name)
                            : currentToken;

                        bool defined = JsonTokenHelpers.Contains(Schema._enum!, resolvedToken);

                        if (!defined)
                        {
                            StringWriter sw = new(CultureInfo.InvariantCulture);
                            currentToken.WriteTo(new JsonTextWriter(sw));

                            RaiseError($"Value {sw.ToString()} is not defined in enum.", ErrorType.Enum, Schema, value, null);
                        }
                    }

                    if (hasConst)
                    {
                        bool defined = JsonTokenHelpers.ImplicitDeepEquals(Schema.Const!, currentToken);

                        if (!defined)
                        {
                            StringWriter sw = new(CultureInfo.InvariantCulture);
                            currentToken.WriteTo(new JsonTextWriter(sw));

                            RaiseError($"Value {sw.ToString()} does not match const.", ErrorType.Const, Schema, value, null);
                        }
                    }

                    if (hasValidator)
                    {
                        JsonValidatorContext context = new(this, Schema);

                        foreach (JsonValidator validator in Schema._validators!)
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

        protected bool TestType<T>(JSchema currentSchema, JSchemaType currentType, T? value)
        {
            return TestType(this, currentSchema, currentType, value);
        }

        internal static bool TestType<T>(SchemaScope scope, JSchema currentSchema, JSchemaType currentType, T? value)
        {
            if (!JSchemaTypeHelpers.HasFlag(currentSchema.Type, currentType))
            {
                scope.RaiseError($"Invalid type. Expected {currentSchema.Type.GetValueOrDefault().GetDisplayText()} but got {currentType.GetDisplayText()}.", ErrorType.Type, currentSchema, value, null);
                return false;
            }

            return true;
        }

        protected SchemaScope CreateScopesAndEvaluateToken(JsonToken token, object? value, int depth, JSchema schema)
        {
            return CreateScopesAndEvaluateToken(token, value, depth, schema, this, Context);
        }

        protected SchemaScope CreateScopesAndEvaluateToken(JsonToken token, object? value, int depth, JSchema schema, SchemaScope? parent, ContextBase context)
        {
            int startCount = Context.Scopes.Count;

            SchemaScope createdScope = CreateTokenScope(token, schema, context, parent, depth);

            int start = Context.Scopes.Count - 1;
            int end = startCount;

            for (int i = start; i >= end; i--)
            {
                Scope newScope = Context.Scopes[i];
                newScope.EvaluateToken(token, value, depth);
            }

            return createdScope;
        }

        internal override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
        {
            IsValid = false;

            base.RaiseError(message, errorType, schema, value, childErrors);
        }

        internal IList<ValidationError>? GetValidationErrors()
        {
            if (Context is ConditionalContext conditionalContext)
            {
                return conditionalContext.Errors;
            }
            else if (Context is CompositeContext compositeContext)
            {
                List<ValidationError> validationErrors = new();
                foreach (ContextBase? childContent in compositeContext.Contexts)
                {
                    if (childContent is ConditionalContext cc && cc.HasErrors)
                    {
                        foreach (ValidationError? e in cc.Errors)
                        {
                            if (!validationErrors.Contains(e))
                            {
                                validationErrors.Add(e);
                            }
                        }
                    }
                }

                return validationErrors;
            }
            else
            {
                return null;
            }
        }
    }
}