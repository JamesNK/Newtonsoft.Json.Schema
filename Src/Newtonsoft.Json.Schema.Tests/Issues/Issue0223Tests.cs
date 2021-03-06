#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
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
    public class Issue0223Tests : TestFixtureBase
    {
        [Test]
        public void Test_Object_Success()
        {
            // Arrange
            JSchema schema = JSchema.Parse(JsonSchema_Object);
            JArray a = JArray.Parse(@"[
  {
    ""module_id"": ""001"",
    ""module_name"": ""Reklamacje"",
    ""module_body"": [
      {
        ""id"": ""001"",
        ""class"": ""middle"",
        ""to"": [
          {
            ""id"": ""002"",
            ""class"": ""simple"",
            ""value"": ""DUMMY""
          }
        ]
      }
    ]
  }
]");

            // Act
            bool valid = a.IsValid(schema, out IList<ValidationError> errors);

            // Assert
            Assert.IsTrue(valid);
            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void Test_Object_Failure()
        {
            // Arrange
            JSchema schema = JSchema.Parse(JsonSchema_Object);
            JArray a = JArray.Parse(@"[
  {
    ""module_id"": ""001"",
    ""module_name"": ""Reklamacje"",
    ""module_body"": [
      {
        ""id"": ""!!!"",
        ""class"": ""middle"",
        ""to"": [
          {
            ""id"": ""002"",
            ""class"": ""simple"",
            ""value"": ""DUMMY""
          }
        ]
      }
    ]
  }
]");

            // Act
            bool valid = a.IsValid(schema, out IList<ValidationError> errors);

            // Assert
            Assert.IsFalse(valid);
            Assert.AreEqual(4, errors.Count);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0.", errors[0].Message);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0.", errors[0].ChildErrors[0].Message);
            Assert.AreEqual(@"String '!!!' does not match regex pattern '^[\da-z-_]+$'.", errors[0].ChildErrors[0].ChildErrors[0].Message);
            Assert.AreEqual("Property 'id' has not been successfully evaluated and the schema does not allow unevaluated properties.", errors[1].Message);
            Assert.AreEqual("Property 'class' has not been successfully evaluated and the schema does not allow unevaluated properties.", errors[2].Message);
            Assert.AreEqual("Property 'to' has not been successfully evaluated and the schema does not allow unevaluated properties.", errors[3].Message);
        }

        private const string JsonSchema_Object = @"{
  ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
  ""definitions"": {
    ""def-type-id"": {
      ""$id"": ""#root/definitions/def-type-id"",
      ""type"": ""string"",
      ""default"": """",
      ""pattern"": ""^[\\da-z-_]+$""
    },
    ""def-node-base"": {
      ""$id"": ""#root/definitions/def-node-base"",
      ""type"": ""object"",
      ""unevaluatedProperties"": true,
      ""properties"": {
        ""id"": {
          ""allOf"": [
            {
              ""$ref"": ""#root/definitions/def-type-id""
            },
            {
              ""$id"": ""#root/definitions/def-node-base/id"",
              ""title"": ""Id""
            }
          ]
        }
      }
    }
  },
  ""title"": ""Root"",
  ""type"": ""array"",
  ""default"": [],
  ""items"": {
    ""$id"": ""#root/items"",
    ""title"": ""Items"",
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
      ""module_id"": {
        ""allOf"": [
          {
            ""$ref"": ""#root/definitions/def-type-id""
          },
          {
            ""$id"": ""#root/items/module_id"",
            ""title"": ""Module_id""
          }
        ]
      },
      ""module_name"": {
        ""$id"": ""#root/items/module_name"",
        ""title"": ""Module_name"",
        ""type"": ""string"",
        ""default"": """",
        ""pattern"": ""^.*$""
      },
      ""module_body"": {
        ""$id"": ""#root/items/module_body"",
        ""title"": ""Module_body"",
        ""type"": ""array"",
        ""default"": [],
        ""items"": {
          ""unevaluatedProperties"": false,
          ""allOf"": [
            {
              ""$ref"": ""#root/definitions/def-node-base""
            },
            {
              ""$id"": ""#root/items/module_body/items"",
              ""title"": ""Items""
            }
          ]
        }
      }
    }
  }
}";
    }
}
