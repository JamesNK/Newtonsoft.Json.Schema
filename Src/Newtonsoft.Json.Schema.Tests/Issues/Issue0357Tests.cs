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
    public class Issue0357Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""query"": {
      ""type"": ""object"",
      ""oneOf"": [
        { ""$ref"": ""#/definitions/and"" },
        { ""$ref"": ""#/definitions/not"" }
      ]
    }
  },
  ""definitions"": {
    ""and"": {
      ""type"": ""object"",
      ""properties"": {
        ""and"": {
          ""type"": ""array"",
          ""items"": { ""$ref"": ""#/definitions/queryObject"" }
        }
      },
      ""required"": [""and""]
    },
    ""not"": {
      ""$ref"": ""#/definitions/queryObject""
    },
    ""queryObject"": {
      ""type"": ""object"",
      ""oneOf"": [
        { ""$ref"": ""#/definitions/not"" },
        { ""$ref"": ""#/definitions/and"" }
      ]
    }
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);

            string json = @"{
              ""query"": {
                ""and"": [
                  {
                  }
                ]
              }
            }";
            JObject o = JObject.Parse(json);

            bool isValid = o.IsValid(schema, out IList<ValidationError> errorMessages);
            Assert.AreEqual(false, isValid);

            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorMessages[0].Message);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].ChildErrors[0].Message);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].ChildErrors[1].Message);
        }
    }
}
