namespace Newtonsoft.Json.Schema.Generation
{
    public abstract class JSchemaGenerationProvider
    {
        public abstract JSchema GetSchema(JSchemaTypeGenerationContext context);
    }
}