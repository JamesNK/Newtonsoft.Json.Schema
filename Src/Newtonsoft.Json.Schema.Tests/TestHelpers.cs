using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema.Tests
{
    public static class TestHelpers
    {
        public static Stream OpenResource(string name)
        {
            return typeof(TestHelpers).Assembly.GetManifestResourceStream("Newtonsoft.Json.Schema.Tests.Resources." + name);
        }

        public static JSchema OpenSchemaResource(string name)
        {
            return OpenSchemaResource(name, JSchemaDummyResolver.Instance);
        }

        public static JSchema OpenSchemaResource(string name, JSchemaResolver resolver)
        {
            Stream s = OpenResource("Schemas." + name);

            using (JsonReader reader = new JsonTextReader(new StreamReader(s)))
            {
                JSchemaReader schemaReader = new JSchemaReader(resolver);
                return schemaReader.ReadRoot(reader);
            }
        }
    }
}