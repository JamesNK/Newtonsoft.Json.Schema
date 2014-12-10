using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.V4;
using Newtonsoft.Json.Schema.V4.Infrastructure;

namespace Newtonsoft.Json.Schema.Tests
{
    public static class TestHelpers
    {
        public static Stream OpenResource(string name)
        {
            return typeof(TestHelpers).Assembly.GetManifestResourceStream("Newtonsoft.Json.Schema.Tests.Resources." + name);
        }

        public static JSchema4 OpenSchemaResource(string name)
        {
            return OpenSchemaResource(name, DummyJSchema4Resolver.Instance);
        }

        public static JSchema4 OpenSchemaResource(string name, JSchema4Resolver resolver)
        {
            Stream s = OpenResource("Schemas." + name);

            using (JsonReader reader = new JsonTextReader(new StreamReader(s)))
            {
                JSchema4Reader schemaReader = new JSchema4Reader(resolver);
                return schemaReader.ReadRoot(reader);
            }
        }
    }
}