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
    public class Issue0187Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            JSchema schema = new JSchema()
            {
                Type = JSchemaType.Object,
                SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#")
            };

            schema.Properties.Add("ShowField", new JSchema() { Type = JSchemaType.Boolean });

            JSchema oneOf1 = new JSchema();
            oneOf1.Properties.Add("ShowField", new JSchema() { Enum = { false } });

            JSchema oneOf2 = new JSchema();
            oneOf2.Properties.Add("ShowField", new JSchema() { Enum = { true } });
            oneOf2.Properties.Add("ExtraField", new JSchema() { Type = JSchemaType.String });

            JSchema oneOf = new JSchema();
            oneOf.OneOf.Add(oneOf1);
            oneOf.OneOf.Add(oneOf2);

            schema.Dependencies.Add("ShowField", oneOf);

            Console.WriteLine();

            // Act
            string result = schema.ToString();

            // Assert
            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""ShowField"": {
      ""type"": ""boolean""
    }
  },
  ""dependencies"": {
    ""ShowField"": {
      ""oneOf"": [
        {
          ""properties"": {
            ""ShowField"": {
              ""enum"": [
                false
              ]
            }
          }
        },
        {
          ""properties"": {
            ""ShowField"": {
              ""enum"": [
                true
              ]
            },
            ""ExtraField"": {
              ""type"": ""string""
            }
          }
        }
      ]
    }
  }
}", result);
        }

        [Test]
        public void RoundtripArray()
        {
            // Arrange
            const string schemaJson = @"{
  ""dependencies"": {
    ""quux"": [
      ""foo"",
      ""bar""
    ]
  }
}";

            // Act
            JSchema schema = JSchema.Parse(schemaJson);
            string json = schema.ToString();

            // Assert
            Assert.AreEqual(schemaJson, json);
        }
    }
}
