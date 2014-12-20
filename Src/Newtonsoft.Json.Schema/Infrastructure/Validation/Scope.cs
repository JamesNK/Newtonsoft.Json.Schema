using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class Scope
    {
        public int InitialDepth;
        public ContextBase Context;
        public Scope Parent;
        public bool Complete;

        protected Scope(ContextBase context, Scope parent, int initialDepth)
        {
            Context = context;
            Parent = parent;
            InitialDepth = initialDepth;
        }

        internal virtual void RaiseError(string message, JSchema schema, IList<ISchemaError> childErrors)
        {
            SchemaScope schemaParent = Parent as SchemaScope;
            if (schemaParent != null)
                schemaParent.IsValid = false;

            Context.RaiseError(message, schema, childErrors);
        }

        public void EvaluateToken(JsonToken token, object value, int depth)
        {
            if (EvaluateTokenCore(token, value, depth))
                Complete = true;
        }

        protected abstract bool EvaluateTokenCore(JsonToken token, object value, int depth);
    }
}