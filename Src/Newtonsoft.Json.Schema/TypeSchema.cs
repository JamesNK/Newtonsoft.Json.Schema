using System;

namespace Newtonsoft.Json.Schema
{
    internal class TypeSchema
    {
        public Type Type;
        public Required Required;
        public JSchema Schema;

        public TypeSchema(Type type, Required required, JSchema schema)
        {
            Type = type;
            Required = required;
            Schema = schema;
        }
    }
}