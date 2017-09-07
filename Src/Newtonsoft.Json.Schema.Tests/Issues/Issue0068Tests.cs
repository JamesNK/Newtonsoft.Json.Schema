#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(NET20 || NET35 || PORTABLE)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class Issue0068Tests : TestFixtureBase
    {
        private string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""patternProperties"": {
    ""^[a-zA-Z0-9]+$"": {
      ""type"": ""object"",
      ""properties"": {
        ""pricing"": {
          ""patternProperties"": {
            ""^[a-zA-Z0-9]+$"": {
              ""type"": ""object"",
              ""properties"": {
                ""priceType"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""pricePer"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""price"": {
                  ""type"": ""number"",
                  ""multipleOf"": 0.01
                },
                ""matchType"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""currency"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    }
  },
  ""additionalProperties"": false
}";

        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(schemaJson);

            JObject o = JObject.Parse(@"{
  ""each"": {
    ""pricing"": {
      ""0"": {
        ""priceType"": ""1"",
        ""pricePer"": ""2"",
        ""price"": 2000000000000000000000000000,
        ""currency"": ""INR""
      },
      ""1"": {
        ""priceType"": ""1"",
        ""pricePer"": ""2"",
        ""price"": 200,
        ""currency"": ""INR""
      },
      ""2"": {
        ""priceType"": ""1"",
        ""pricePer"": ""2"",
        ""price"": 200,
        ""currency"": ""INR""
      }
    }
  }
}");

            IList<string> errors;
            Assert.IsFalse(o.IsValid(schema, out errors));

            Assert.AreEqual("Integer 2000000000000000000000000000 is not a multiple of 0.01. Path 'each.pricing.0.price', line 7, position 45.", errors[0]);
        }
    }
}
#endif