#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
    public class Issue0028Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();

            resolver.Add(new Uri("http://example.com/schema1.json"), Schema1);
            resolver.Add(new Uri("http://example.com/schema2.json"), Schema2);

            JSchema schema = JSchema.Parse(Schema1, resolver);

            JSchema fooSchema = schema.Properties["foo"];
            Assert.AreEqual(JSchemaType.String, fooSchema.Properties["value"].Type);

            // because the root schema has no BaseUri this schema won't be set back
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema bazSchema = (JSchema)schema.ExtensionData["definitions"]["baz"];
                Assert.IsNotNull(bazSchema);
            }, "Cannot convert JToken to JSchema. No schema is associated with this token.");

            Console.WriteLine(schema.ToString());
        }

        [Test]
        public void Test_WithBaseUri()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();

            resolver.Add(new Uri("http://example.com/schema1.json"), Schema1);
            resolver.Add(new Uri("http://example.com/schema2.json"), Schema2);

            JSchema schema = JSchema.Parse(Schema1, new JSchemaReaderSettings
            {
                Resolver = resolver,
                BaseUri = new Uri("http://example.com/schema1.json")
            });

            JSchema fooSchema = schema.Properties["foo"];
            Assert.AreEqual(JSchemaType.String, fooSchema.Properties["value"].Type);

            JSchema bazSchema = (JSchema)schema.ExtensionData["definitions"]["baz"];
            Assert.AreEqual(JSchemaType.Number, bazSchema.Properties["value"].Type);
        }

        private const string Schema1 = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""id"": ""http://example.com/schema1.json"",
    ""title"": ""Schema 1"",
    ""description"": ""Demonstrating stack overflow"",
    ""properties"": {
        ""foo"": {
            ""$ref"": ""http://example.com/schema2.json#/definitions/juliet""
        }
    },
    ""definitions"": {
        ""baz"": {
            ""properties"": {
                ""value"": { ""type"": ""number"" }
            }
        }
    }
}";

        private const string Schema2 = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""id"": ""http://example.com/schema2.json"",
    ""title"": ""Schema 2"",
    ""description"": ""Demonstrating stack overflow"",
    ""properties"": {
        ""romeo"": {
            ""$ref"": ""http://example.com/schema1.json#/definitions/baz""
        }
    },
    ""definitions"": {
        ""juliet"": {
            ""properties"": {
                ""value"": { ""type"": ""string"" }
            }
        }
    }
}";
    }
}