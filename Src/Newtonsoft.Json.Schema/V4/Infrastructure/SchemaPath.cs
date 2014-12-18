using System;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class SchemaPath
    {
        public readonly Uri Id;
        public readonly string Path;

        public SchemaPath(Uri id, string path)
        {
            Id = id;
            Path = path;
        }
    }
}