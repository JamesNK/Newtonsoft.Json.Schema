using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class AllOfScope : ConditionalScope
    {
        private readonly IList<JSchema> _schemas;

        public AllOfScope(SchemaScope parent, IList<JSchema> schemas, ContextBase context, int depth)
            : base(context, parent, depth)
        {
            _schemas = schemas;
        }

        public void InitializeScopes(JsonToken token)
        {
            foreach (JSchema schema in _schemas)
            {
                SchemaScope.CreateTokenScope(token, schema, ConditionalContext, this, InitialDepth);
            }
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && (JsonWriter.IsEndToken(token) || JsonReader.IsPrimitiveToken(token)))
            {
                if (!GetChildren().All(IsValidPredicate))
                {
                    List<int> invalidIndexes = new List<int>();
                    int index = 0;
                    foreach (SchemaScope schemaScope in GetChildren())
                    {
                        if (!schemaScope.IsValid)
                            invalidIndexes.Add(index);

                        index++;
                    }

                    string message = "JSON does not match all schemas from 'allOf'. Invalid schema indexes: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", invalidIndexes));
                    RaiseError(message, ParentSchemaScope.Schema, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}