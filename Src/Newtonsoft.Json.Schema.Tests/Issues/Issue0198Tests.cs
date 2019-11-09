#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0198Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            string rootSchemaPath = TestHelpers.ResolveFilePath(@"resources\schemas\custom\issue0198\sample.json");

            // Act
            JSchema schema;
            using (Stream file = File.OpenRead(rootSchemaPath))
            using (JsonReader reader = new JsonTextReader(new StreamReader(file)))
            {
                schema = JSchema.Load(reader, new JSchemaReaderSettings
                {
                    Resolver = new JSchemaUrlResolver(),
                    BaseUri = new Uri(rootSchemaPath, UriKind.RelativeOrAbsolute)
                });
            }

            // Assert
            Assert.IsNotNull(schema);
            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(2, schema.AllOf.Count);
            Assert.AreEqual(JSchemaType.String, schema.AllOf[0].Properties["address"].Type);
        }
    }
}
