using System;

namespace Newtonsoft.Json.Schema.V4
{
    internal class TypeSchema
    {
        public Type Type;
        public Required Required;
        public JSchema4 Schema;

        public TypeSchema(Type type, Required required, JSchema4 schema)
        {
            Type = type;
            Required = required;
            Schema = schema;
        }
    }
}