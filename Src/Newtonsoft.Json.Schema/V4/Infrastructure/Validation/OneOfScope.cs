using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class OneOfScope : ConditionalScope
    {
        private readonly IList<JSchema4> _schemas;

        public OneOfScope(SchemaScope parent, IList<JSchema4> schemas, ContextBase context, int depth)
            : base(context, parent, depth)
        {
            _schemas = schemas;
        }

        public void InitializeScopes(JsonToken token)
        {
            foreach (JSchema4 schema in _schemas)
            {
                SchemaScope.CreateTokenScope(token, schema, ConditionalContext, this, InitialDepth);
            }
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && (JsonWriter.IsEndToken(token) || JsonReader.IsPrimitiveToken(token)))
            {
                IEnumerable<Scope> children = Context.Scopes.Where(s => s.Parent == this);
                int validCount = children.OfType<SchemaScope>().Count(s => s.IsValid);

                if (validCount != 1)
                {
                    RaiseError("OneOf", ParentSchemaScope.Schema, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}