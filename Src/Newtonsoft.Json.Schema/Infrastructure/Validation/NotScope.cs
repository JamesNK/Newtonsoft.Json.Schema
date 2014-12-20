using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class NotScope : ConditionalScope
    {
        private readonly JSchema _schema;

        public NotScope(SchemaScope parent, JSchema schema, ContextBase context, int depth)
            : base(context, parent, depth)
        {
            _schema = schema;
        }

        public void InitializeScopes(JsonToken token)
        {
            SchemaScope.CreateTokenScope(token, _schema, ConditionalContext, this, InitialDepth);
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && (JsonWriter.IsEndToken(token) || JsonReader.IsPrimitiveToken(token)))
            {
                if (GetChildren().Any(IsValidPredicate))
                {
                    RaiseError("JSON is valid against schema from 'not'.", ParentSchemaScope.Schema, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}