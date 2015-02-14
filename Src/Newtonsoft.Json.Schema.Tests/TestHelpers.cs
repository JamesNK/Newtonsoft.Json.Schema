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
            string path = ResolveFilePath(name);

            return File.OpenRead(path);
        }

        private static string ResolveFilePath(string name)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(baseDirectory, name);
            return path;
        }

        public static string OpenFileText(string name)
        {
            using (var file = OpenFile(name))
            using (StreamReader sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        public static JSchema OpenSchemaFile(string name, JSchemaResolver resolver = null)
        {
            string path = ResolveFilePath(name);

            using (Stream file = File.OpenRead(path))
            using (JsonReader reader = new JsonTextReader(new StreamReader(file)))
            {
                JSchema schema = JSchema.Load(reader, new JSchemaReaderSettings
                {
                    Resolver = resolver,
                    BaseUri = new Uri(path, UriKind.RelativeOrAbsolute)
                });
                return schema;
            }
        }
    }
}