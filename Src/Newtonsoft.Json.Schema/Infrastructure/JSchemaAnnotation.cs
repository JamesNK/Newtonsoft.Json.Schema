namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaAnnotation
    {
        public readonly JSchema Schema;

        public JSchemaAnnotation(JSchema schema)
        {
            Schema = schema;
        }
    }
}