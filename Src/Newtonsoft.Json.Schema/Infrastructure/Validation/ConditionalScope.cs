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
    }
}