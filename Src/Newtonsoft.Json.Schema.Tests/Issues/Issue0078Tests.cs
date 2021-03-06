#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0078Tests : TestFixtureBase
    {
        private readonly string schemaJson = @"{
  ""$id"": ""MySchema"",
  ""$type"": ""schema"",
  ""definitions"": {
    ""myDefinition"": {
      ""title"": ""myDefinition"",
      ""type"": ""object"",
      ""properties"": {
        ""items"": {
          ""title"": ""items"",
          ""type"": ""object"",
          ""properties"": {
            ""value"": {
              ""title"": ""value"",
              ""type"": ""array"",
              ""items"": {
                ""oneOf"": [
                  {
                    ""$ref"": ""MySchema#/definitions/myDefinition/properties/items/properties/value""
                  },
                  {
                    ""$ref"": ""MySchema#/definitions/myDefinition""
                  }
                ]
              }
            }
          },
          ""$ref"": ""MySchema#/definitions/myRef""
        }
      }
    },
    ""myRef"": {
      ""maxProperties"": 3
    }
  },
  ""allOf"": [
    {
      ""properties"": {
        ""myProperty"": {
          ""$ref"": ""MySchema#/definitions/myDefinition/properties/items/properties/value""
        }
      }
    }
  ]
}";

        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(schemaJson);
            AssertSchema(schema);

            string json = schema.ToString();

            JSchema schema2 = JSchema.Parse(json);
            AssertSchema(schema2);

            StringAssert.AreEqual(schemaJson, json);
        }

        private void AssertSchema(JSchema schema)
        {
            JSchema myDefinitionSchema = (JSchema)schema.ExtensionData["definitions"]["myDefinition"];
            JSchema itemsSchema = myDefinitionSchema.Properties["items"];
            JSchema valueSchema = itemsSchema.Properties["value"];

            Assert.AreEqual(3, itemsSchema.Ref.MaximumProperties);

            Assert.AreEqual(myDefinitionSchema, valueSchema.Items[0].OneOf[1]);

            Assert.AreEqual(valueSchema, schema.AllOf[0].Properties["myProperty"]);
        }
    }
}