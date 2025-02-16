using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Issue0364Tests
    {
        private const string Schema = @"{
  ""definitions"": {
    ""typeA"": {
      ""type"": ""object"",
      ""properties"": {
        ""method"": {
          ""type"": ""string"",
          ""minLength"": 4,
          ""maxLength"": 4
        }
      }
    },
    ""typeB"": {
      ""type"": ""object"",
      ""unevaluatedProperties"": false,
      ""allOf"": [
        {
          ""properties"": {
            ""id"": {
              ""type"": ""integer""
            }
          }
        },
        {
          ""$ref"": ""#/definitions/typeA""
        }
      ]
    },
    ""typeC"": {
      ""type"": ""object"",
      ""properties"": {
        ""name"": {
          ""type"": ""string""
        }
      }
    },
    ""typeD"": {
      ""type"": ""object"",
      ""properties"": {
        ""value"": {
          ""$ref"": ""#/definitions/typeC""
        }
      }
    },
    ""typeE"": {
      ""type"": ""object"",
      ""properties"": {
        ""value"": {
          ""$ref"": ""#/definitions/typeD""
        }
      }
    },
  },
  ""properties"": {
    ""property1"": {
      ""$ref"": ""#/definitions/typeB""
    },
    ""property2"": {
      ""$ref"": ""#/definitions/typeE""
    }
  }
}";

        [Test]
        public void Test()
        {
            // property1.method is invalid (too long) but this should only be an issue with typeB

            string json = @"{
              ""property1"": {
                ""id"": 1,
                ""method"": ""longstring""
              },
              ""property2"": {
                ""value"": {
                  ""value"": {
                    ""name"": ""xxx""
                  }
                }
              }
            }";

            JSchema schema = JSchema.Parse(Schema);
            JObject obj = JObject.Parse(json);
            Assert.IsFalse(obj.IsValid(schema, out IList<ValidationError> errorMessages));
            Assert.IsNotNull(errorMessages);
            Assert.AreNotEqual(0, errorMessages.Count);

            foreach (var error in errorMessages)
                Assert.AreEqual("#/definitions/typeB", error.SchemaId.OriginalString);
        }
    }
}