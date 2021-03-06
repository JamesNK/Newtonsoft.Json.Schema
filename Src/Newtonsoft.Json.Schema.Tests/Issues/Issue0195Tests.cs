#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
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
    public class Issue0195Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            string rootSchemaPath = TestHelpers.ResolveFilePath(@"resources\schemas\custom\issue0195\A.json");

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
            Assert.AreEqual(1, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["a"].Items[0].Properties["p"].Type);
        }

        [Test]
        public void Test_MultipleRootReplacements()
        {
            // Arrange
            string rootSchemaPath = TestHelpers.ResolveFilePath(@"resources\schemas\custom\issue0195\A2.json");

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
            Assert.AreEqual(1, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["p"].Type);
        }
    }
}
