using System;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaDummyResolver : JSchemaResolver
    {
        public static readonly JSchemaDummyResolver Instance = new JSchemaDummyResolver();

        public override JSchema GetSchema(Uri uri)
        {
            return null;
        }
    }
}