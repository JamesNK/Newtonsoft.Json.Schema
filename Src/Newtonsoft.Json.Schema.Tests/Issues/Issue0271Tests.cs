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
    public class Issue0271Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(Schema);
            Assert.AreEqual("bodySection", schema.Properties["body"].Properties["blocks"].Items[0].Id.OriginalString);
            Assert.AreEqual("bodySection", schema.Properties["body"].Properties["blocks"].Items[0].Anchor);
        }

        private const string Schema = @"{
  ""$schema"": ""http://json-schema.org/draft-2019-09/schema"",
  ""$defs"": {
    ""bodyAsset"": {
      ""$anchor"": ""bodyAsset"",
      ""type"": ""object"",
      ""required"": [
        ""type""
      ],
      ""properties"": {
        ""id"": {
          ""type"": ""string""
        },
        ""type"": {
          ""const"": ""ASSET""
        },
        ""about"": {
          ""type"": ""string""
        },
        ""headline"": {
          ""type"": ""string""
        }
      }
    },
    ""bodySection"": {
      ""$anchor"": ""bodySection"",
      ""$id"": ""bodySection"",
      ""type"": ""object"",
      ""required"": [
        ""type""
      ],
      ""properties"": {
        ""name"": {
          ""type"": ""string""
        },
        ""type"": {
          ""const"": ""SECTION""
        },
        ""sections"": {
          ""type"": ""array"",
          ""items"": {
            ""anyOf"": [
              {
                ""$ref"": ""#bodyAsset""
              }
            ]
          }
        }
      },
      ""additionalProperties"": false
    },
    ""metroNewsletterBody"": {
      ""$anchor"": ""metroNewsletterBody"",
      ""type"": ""object"",
      ""required"": [
        ""blocks""
      ],
      ""properties"": {
        ""blocks"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""bodySection#bodySection""
          }
        }
      }
    }
  },
  ""properties"": {
    ""body"": {
      ""$ref"": ""#metroNewsletterBody""
    }
  },
  ""required"": [
  ],
  ""type"": ""object""
}";
    }
}
