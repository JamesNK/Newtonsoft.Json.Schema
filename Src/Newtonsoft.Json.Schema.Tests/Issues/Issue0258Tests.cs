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
    public class Issue0258Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JObject clientJson = JObject.Parse(Json);
            JSchema schema = JSchema.Parse(Schema);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, errorMessages.Count);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errorMessages[0].Message);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].ChildErrors[2].Message);
            Assert.AreEqual(ErrorType.AnyOf, errorMessages[0].ChildErrors[2].ErrorType);
        }

        private const string Schema = @"{
  ""definitions"": {
    ""oneMethod"": {
      ""additionalProperties"": false,
      ""type"": ""object"",
      ""properties"": {
        ""method"": {
          ""type"": ""string""
        },
        ""method_ref"": {
          ""type"": ""string""
        }
      },
      ""not"": {
        ""required"": [
          ""method"",
          ""method_ref""
        ]
      },
       ""anyOf"": [ { ""$ref"": ""#/definitions/oneNursery"" } ]
    },
    ""oneNursery"": {
      ""additionalProperties"": false,
      ""type"": ""object"",
      ""properties"": {
        ""nursery"": {
          ""type"": ""string""
        },
        ""nursery_ref"": {
          ""type"": ""string""
        },
        
      },
      ""not"": {
        ""required"": [
          ""nursery"",
          ""nursery_ref""
        ]
      },
      ""anyOf"": [ { ""$ref"": ""#/definitions/oneMethod"" } ]
    }
  },
  ""anyOf"": [
    {
      ""$ref"": ""#/definitions/oneMethod""
    },
    {
      ""$ref"": ""#/definitions/oneNursery""
    }
  ]
}";

        private const string Json = @"{
    ""method"": ""2"",
    ""nursery"": ""2"" 
}";
    }
}
