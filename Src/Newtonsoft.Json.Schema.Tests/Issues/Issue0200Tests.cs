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
    public class Issue0200Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            string schemaJson = @"{
  ""type"": ""object"",
  ""definitions"": {
    ""foo"": {
      ""type"": ""object"",
      ""properties"": {
        ""bar"": {
          ""$ref"": ""#/definitions/bar""
        },
        ""baz"": {
          ""$ref"": ""#/definitions/bar""
        }
      }
    },
    ""bar"": {
      ""type"": ""string"" 
    }
  },
  ""dependencies"": {
    ""foo"": {
      ""oneOf"": [
        {
          ""properties"": {
            ""foo"": {
              ""$ref"": ""#/definitions/foo"" 
            } 
          } 
        }
      ] 
    } 
  } 
}";

            JSchema schema = JSchema.Parse(schemaJson);

            // Act
            string serialized = schema.ToString();

            // Assert
            StringAssert.AreEqual(@"{
  ""definitions"": {
    ""foo"": {
      ""type"": ""object"",
      ""properties"": {
        ""bar"": {
          ""$ref"": ""#/definitions/bar""
        },
        ""baz"": {
          ""$ref"": ""#/definitions/bar""
        }
      }
    },
    ""bar"": {
      ""type"": ""string""
    }
  },
  ""type"": ""object"",
  ""dependencies"": {
    ""foo"": {
      ""oneOf"": [
        {
          ""properties"": {
            ""foo"": {
              ""$ref"": ""#/definitions/foo""
            }
          }
        }
      ]
    }
  }
}", serialized);
        }
    }
}
