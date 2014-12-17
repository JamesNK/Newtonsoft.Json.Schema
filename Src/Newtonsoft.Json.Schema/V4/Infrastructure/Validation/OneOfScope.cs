using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class OneOfScope : Scope
    {
        private readonly IList<JSchema4> _schemas;

        public OneOfScope(Scope parent, IList<JSchema4> schemas, Context context, int depth, bool raiseErrors)
            : base(context, parent, depth, raiseErrors)
        {
            _schemas = schemas;
        }

        public void InitializeScopes(JsonToken token)
        {
            foreach (JSchema4 schema in _schemas)
            {
                SchemaScope.CreateTokenScope(token, schema, Context, this, InitialDepth, false);
            }
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && (JsonWriter.IsEndToken(token) || JsonReader.IsPrimitiveToken(token)))
            {
                IEnumerable<Scope> children = Context.Scopes.Where(s => s.Parent == this);
                int validCount = children.Count(s => s.IsValid);

                if (validCount != 1)
                {
                    RaiseError("OneOf", null);
                }

                return true;
            }

            return false;
        }
    }
}