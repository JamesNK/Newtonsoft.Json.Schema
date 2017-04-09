#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema.Tests
{
    public static class TestHelpers
    {
        public static Stream OpenResource(string name)
        {
            Assembly assembly;
#if !DNXCORE50
            assembly = typeof(TestHelpers).Assembly;
#else
            assembly = typeof(TestHelpers).GetTypeInfo().Assembly;
#endif

            name = "Newtonsoft.Json.Schema.Tests.Resources." + name;

            Stream stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                throw new InvalidOperationException("Could not find resource with name: " + name);
            }

            return stream;
        }

        public static Stream OpenFile(string name)
        {
            string path = ResolveFilePath(name);

            return File.OpenRead(path);
        }

        public static string ResolveFilePath(string name)
        {
#if !DNXCORE50
            string path = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, name);
#else
            string path = Path.GetFullPath(name);
#endif

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