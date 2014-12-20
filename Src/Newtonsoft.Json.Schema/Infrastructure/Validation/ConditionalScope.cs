using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class ConditionalScope : Scope
    {
        protected readonly ConditionalContext ConditionalContext;
        protected readonly SchemaScope ParentSchemaScope;

        protected ConditionalScope(ContextBase context, SchemaScope parent, int initialDepth)
            : base(context, parent, initialDepth)
        {
            ParentSchemaScope = parent;
            ConditionalContext = ConditionalContext.Create(context);
        }

        public void InitializeScopes(JsonToken token, IEnumerable<JSchema> schemas)
        {
            foreach (JSchema schema in schemas)
            {
                SchemaScope.CreateTokenScope(token, schema, ConditionalContext, this, InitialDepth);
            }
        }

        protected IEnumerable<SchemaScope> GetChildren()
        {
            foreach (Scope scope in Context.Scopes)
            {
                SchemaScope schemaScope = scope as SchemaScope;
                if (schemaScope != null)
                {
                    if (schemaScope.Parent == this)
                        yield return schemaScope;
                }
            }
        }

        protected IEnumerable<SchemaScope> GetValidChildren()
        {
            foreach (SchemaScope schemaScope in GetChildren())
            {
                if (schemaScope.IsValid)
                    yield return schemaScope;
            }
        }

        protected static bool IsValidPredicate(SchemaScope s)
        {
            return s.IsValid;
        }
    }
}