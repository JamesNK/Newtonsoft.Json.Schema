#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

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

        public static Stream OpenFile(string name)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(baseDirectory, name);

            return File.OpenRead(path);
        }

        public static string OpenFileText(string name)
        {
            using (var file = OpenFile(name))
            using (StreamReader sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
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

        public static JSchema OpenSchemaFile(string name, JSchemaResolver resolver = null)
        {
            using (Stream file = OpenFile(name))
            using (JsonReader reader = new JsonTextReader(new StreamReader(file)))
            {
                JSchema schema = (resolver != null) ? JSchema.Load(reader, resolver) : JSchema.Load(reader);
                return schema;
            }
        }
    }
}