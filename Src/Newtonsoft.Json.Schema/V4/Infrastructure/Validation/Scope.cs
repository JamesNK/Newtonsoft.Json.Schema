namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal abstract class Scope
    {
        public int InitialDepth;
        public Context Context;
        public bool IsValid;
        public bool RaiseErrors;
        public Scope Parent;
        public bool Complete;

        protected Scope(Context context, Scope parent, int initialDepth, bool raiseErrors)
        {
            Context = context;
            Parent = parent;
            InitialDepth = initialDepth;
            RaiseErrors = raiseErrors;
            IsValid = true;
        }

        internal void RaiseError(string message, JSchema4 schema)
        {
            IsValid = false;

            if (Parent is SchemaScope)
                Parent.IsValid = false;

            if (RaiseErrors)
                Context.RaiseError(message, schema);
        }

        public void EvaluateToken(JsonToken token, object value, int depth)
        {
            if (EvaluateTokenCore(token, value, depth))
                Complete = true;
        }

        protected abstract bool EvaluateTokenCore(JsonToken token, object value, int depth);
    }
}