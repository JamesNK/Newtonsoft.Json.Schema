using System;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class KnownSchema
    {
        public Uri Id;
        public JSchema Schema;
        public KnownSchemaState State;

        public KnownSchema(Uri id, JSchema schema, KnownSchemaState state)
        {
            Id = id;
            Schema = schema;
            State = state;
        }
    }
}