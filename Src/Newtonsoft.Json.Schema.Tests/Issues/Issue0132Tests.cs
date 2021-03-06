#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

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
    public class Issue0132Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(SchemaJson);

            JToken t = JToken.Parse(Json);

            Assert.IsTrue(t.IsValid(s));
        }

        #region JSON
        private const string Json = @"{
    ""foobar"" : ""foo"",
    ""baz"" : ""baz""
}";
        #endregion

        #region Schema
        private const string SchemaJson = @"{
  ""oneOf"": [
    {
      ""allOf"": [
        {
          ""properties"": {
            ""foobar"": {
              ""type"": ""string"",
              ""enum"": [
                ""foo""
              ]
            }
          }
        },
        {
          ""$ref"": ""#/definitions/bazquux""
        }
      ]
    },
    {
      ""allOf"": [
        {
          ""properties"": {
            ""foobar"": {
              ""type"": ""string"",
              ""enum"": [
                ""bar""
              ]
            }
          }
        },
        {
          ""$ref"": ""#/definitions/bazquux""
        }
      ]
    }
  ],
  ""definitions"": {
    ""bazquux"": {
      ""oneOf"": [
        {
          ""properties"": {
            ""baz"": {
              ""type"": ""string""
            }
          },
          ""required"": [
            ""baz""
          ]
        },
        {
          ""properties"": {
            ""quux"": {
              ""type"": ""string""
            }
          },
          ""required"": [
            ""quux""
          ]
        }
      ]
    }
  }
}";
        #endregion
    }
}