using System;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class KnownSchema
    {
        public Uri Id;
        public JSchema4 Schema;
        public KnownSchemaState State;

        public KnownSchema(Uri id, JSchema4 schema, KnownSchemaState state)
        {
            Id = id;
            Schema = schema;
            State = state;
        }
    }
}