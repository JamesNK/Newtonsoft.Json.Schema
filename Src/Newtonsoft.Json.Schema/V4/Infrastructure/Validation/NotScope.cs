using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class NotScope : ConditionalScope
    {
        private readonly JSchema4 _schema;

        public NotScope(SchemaScope parent, JSchema4 schema, ContextBase context, int depth)
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
                IEnumerable<SchemaScope> children = Context.Scopes.Where(s => s.Parent == this).OfType<SchemaScope>();
                if (children.Any(s => s.IsValid))
                {
                    RaiseError("Not", ParentSchemaScope.Schema, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}