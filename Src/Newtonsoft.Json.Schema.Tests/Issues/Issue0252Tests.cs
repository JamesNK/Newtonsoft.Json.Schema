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
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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
    public class Issue0252Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JObject clientJson = JObject.Parse(Data);
            JSchema schema = JSchema.Parse(Schema);

            var subSchema = schema.Properties["testEnum"].Items[0].Properties["testEnum1"];
            Assert.AreEqual("test", subSchema.Description);

            var subSchemaRef = subSchema.Ref;

            Assert.AreEqual(JSchemaType.Object, subSchemaRef.Type);

            var enums = subSchemaRef.Properties["value"].Enum;

            Assert.AreEqual("1", (string)enums[0]);
            Assert.AreEqual("2", (string)enums[1]);
            Assert.AreEqual("3", (string)enums[2]);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("JSON does not match schema from '$ref'.", errorMessages[0].Message);
            Assert.AreEqual(@"Value ""21"" is not defined in enum.", errorMessages[0].ChildErrors[0].Message);
        }

        private const string Data = @"{
  ""testEnum"": [
    {
      ""testEnum1"": {
        ""value"": ""21""
      }
    }
  ]
}";

        private const string Schema = @"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""testEnum"": {
      ""type"": ""array"",
      ""minItems"": 1,
      ""items"": {
        ""$ref"": ""#/components/schemas/funTest""
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""funTest"": {
        ""type"": ""object"",
        ""additionalProperties"": false,

        ""properties"": {
          ""testEnum1"": {
            ""description"": ""test"",
            ""$ref"": ""#/components/schemas/testEnum""
          }
        }
      },
      ""testEnum"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {
          ""value"": {
            ""type"": ""string"",
            ""enum"": [
              ""1"",
              ""2"",
              ""3""
            ]
          }
        }
      }
    }
  }
}";
    }
}
