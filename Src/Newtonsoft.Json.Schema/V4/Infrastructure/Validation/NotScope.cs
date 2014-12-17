using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class NotScope : Scope
    {
        private readonly JSchema4 _schema;

        public NotScope(Scope parent, JSchema4 schema, Context context, int depth, bool raiseErrors)
            : base(context, parent, depth, raiseErrors)
        {
            _schema = schema;
        }

        public void InitializeScopes(JsonToken token)
        {
            SchemaScope.CreateTokenScope(token, _schema, Context, this, InitialDepth, false);
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && (JsonWriter.IsEndToken(token) || JsonReader.IsPrimitiveToken(token)))
            {
                if (Context.Scopes.Where(s => s.Parent == this).Any(s => s.IsValid))
                {
                    RaiseError("Not", null);
                }

                return true;
            }

            return false;
        }
    }
}