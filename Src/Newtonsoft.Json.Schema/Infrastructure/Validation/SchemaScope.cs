using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class SchemaScope : Scope
    {
        public JSchema Schema { get; set; }
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
                var allOfScope = new AllOfScope(scope, schema._allOf, context, depth);
                context.Scopes.Add(allOfScope);

                allOfScope.InitializeScopes(token);
            }
            if (schema._anyOf != null && schema._anyOf.Count > 0)
            {
                var anyOfScope = new AnyOfScope(scope, schema._anyOf, context, depth);
                context.Scopes.Add(anyOfScope);

                anyOfScope.InitializeScopes(token);
            }
            if (schema._oneOf != null && schema._oneOf.Count > 0)
            {
                var oneOfScope = new OneOfScope(scope, schema._oneOf, context, depth);
                context.Scopes.Add(oneOfScope);

                oneOfScope.InitializeScopes(token);
            }
            if (schema.Not != null)
            {
                var notScope = new NotScope(scope, schema.Not, context, depth);
                context.Scopes.Add(notScope);

                notScope.InitializeScopes(token);
            }

            return scope;
        }

        protected void EnsureEnum(JsonToken token, object value)
        {
            if (Schema.Enum != null)
            {
                if (JsonReader.IsPrimitiveToken(token) || JsonReader.IsStartToken(token))
                {
                    if (Context.TokenWriter == null)
                    {
                        Context.TokenWriter = new JTokenWriter();
                        Context.TokenWriter.WriteToken(token, value);
                    }
                }

                if (JsonReader.IsPrimitiveToken(token) || JsonWriter.IsEndToken(token))
                {
                    if (!Schema.Enum.ContainsValue(Context.TokenWriter.CurrentToken, JToken.EqualityComparer))
                    {
                        StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
                        Context.TokenWriter.CurrentToken.WriteTo(new JsonTextWriter(sw));

                        RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, sw.ToString()), Schema, null);
                    }
                }
            }
        }

        protected bool TestType(JSchema currentSchema, JSchemaType currentType)
        {
            if (!JsonSchemaTypeHelpers.HasFlag(currentSchema.Type, currentType))
            {
                RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema, null);
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

        internal override void RaiseError(string message, JSchema schema, IList<ISchemaError> childErrors)
        {
            IsValid = false;

            base.RaiseError(message, schema, childErrors);
        }
    }
}