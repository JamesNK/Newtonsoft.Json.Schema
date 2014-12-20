using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class DummyJSchemaResolver : JSchemaResolver
    {
        public static readonly DummyJSchemaResolver Instance = new DummyJSchemaResolver();

        public override JSchema GetSchema(Uri uri)
        {
            return null;
        }
    }
}