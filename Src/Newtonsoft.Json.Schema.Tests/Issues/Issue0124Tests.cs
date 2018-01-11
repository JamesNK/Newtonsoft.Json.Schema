#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class Issue0124Tests : TestFixtureBase
    {
        [Test]
        public void Test_Contains_Valid()
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            JObject o = JObject.Parse(@"{
  ""contexts"": [
    {
      ""name"": ""A_name""
    },
    {
      ""name"": ""B_name""
    }
  ]
}");

            bool isValid = o.IsValid(schema, out IList<ValidationError> messages);
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, messages.Count);
        }

        [Test]
        public void Test_Contains_Invalid()
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            JObject o = JObject.Parse(@"{
  ""contexts"": [
    {
      ""name"": ""A_name""
    }
  ]
}");

            bool isValid = o.IsValid(schema, out IList<ValidationError> messages);
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 1.", messages[0].Message);
            Assert.AreEqual(1, messages[0].ChildErrors.Count);
            Assert.AreEqual("No items match contains.", messages[0].ChildErrors[0].Message);
        }

        private const string SchemaJson = @"{
  ""properties"": {
    ""contexts"": {
      ""type"": ""array"",
      ""allOf"": [
        {
          ""contains"": {
            ""type"": ""object"",
            ""properties"": {
              ""name"": {
                ""enum"": [
                  ""A_name""
                ],
                ""type"": ""string""
              }
            }
          }
        },
        {
          ""contains"": {
            ""type"": ""object"",
            ""properties"": {
              ""name"": {
                ""enum"": [
                  ""B_name""
                ],
                ""type"": ""string""
              }
            }
          }
        }
      ]
    }
  }
}";
    }
}