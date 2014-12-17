using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal abstract class SchemaScope : Scope
    {
        public JSchema4 Schema { get; set; }

        protected SchemaScope(Context context, Scope parent, int initialDepth, JSchema4 schema, bool raiseErrors)
            : base(context, parent, initialDepth, raiseErrors)
        {
            Schema = schema;
        }

        public static int CreateTokenScope(JsonToken token, JSchema4 schema, Context context, Scope parent, int depth, bool raiseErrors)
        {
            int startCount = context.Scopes.Count;
            SchemaScope scope;

            switch (token)
            {
                case JsonToken.StartObject:
                    scope = new ObjectScope(context, parent, depth, schema, raiseErrors);
                    break;
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                    scope = new ArrayScope(context, parent, depth, schema, raiseErrors);
                    break;
                default:
                    scope = new PrimativeScope(context, parent, depth, schema, raiseErrors);
                    break;
            }

            context.Scopes.Add(scope);

            if (schema._allOf != null && schema._allOf.Count > 0)
            {
                var allOfScope = new AllOfScope(scope, schema._allOf, context, depth, raiseErrors);
                context.Scopes.Add(allOfScope);

                allOfScope.InitializeScopes(token);
            }
            if (schema._anyOf != null && schema._anyOf.Count > 0)
            {
                var anyOfScope = new AnyOfScope(scope, schema._anyOf, context, depth, raiseErrors);
                context.Scopes.Add(anyOfScope);

                anyOfScope.InitializeScopes(token);
            }
            if (schema._oneOf != null && schema._oneOf.Count > 0)
            {
                var oneOfScope = new OneOfScope(scope, schema._oneOf, context, depth, raiseErrors);
                context.Scopes.Add(oneOfScope);

                oneOfScope.InitializeScopes(token);
            }
            if (schema.Not != null)
            {
                var notScope = new NotScope(scope, schema.Not, context, depth, raiseErrors);
                context.Scopes.Add(notScope);

                notScope.InitializeScopes(token);
            }

            return context.Scopes.Count - startCount;
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

                        RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, sw.ToString()), Schema);
                    }
                }
            }
        }

        protected bool TestType(JSchema4 currentSchema, JSchemaType currentType)
        {
            if (!JsonSchemaTypeHelpers.HasFlag(currentSchema.Type, currentType))
            {
                RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema);
                return false;
            }

            return true;
        }

        protected void CreateScopesAndEvaluateToken(JsonToken token, object value, int depth, JSchema4 schema)
        {
            int newScopeCount = CreateTokenScope(token, schema, Context, this, depth, RaiseErrors);

            int start = Context.Scopes.Count - 1;
            int end = Context.Scopes.Count - newScopeCount;

            for (int i = start; i >= end; i--)
            {
                Scope newScope = Context.Scopes[i];
                newScope.EvaluateToken(token, value, depth);
                if (newScope.Complete)
                {
                    if (!newScope.IsValid)
                        IsValid = false;
                }
            }
        }
    }
}