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
