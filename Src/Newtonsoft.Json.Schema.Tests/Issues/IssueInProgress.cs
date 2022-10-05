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
    public class IssueInProgress : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(Schema);
            JObject o = JObject.Parse(Data);

            Assert.IsTrue(o.IsValid(schema));
           
            Console.WriteLine(schema.ToString());

            string expectedOutput = @"{
  ""$comment"": ""Root type."",
  ""$defs"": {
    ""contentItem"": {
      ""$comment"": ""some definition."",
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""dataObjectValue"": {
          ""type"": ""string""
        }
      }
    }
  },
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""contents"": {
      ""$comment"": ""some definition."",
      ""$ref"": ""#/$defs/contentItem""
    },
    ""contents2"": {
      ""$ref"": ""#/$defs/contentItem""
    }
  },
  ""required"": [
    ""contents""
  ]
}";

            StringAssert.AreEqual(expectedOutput, schema.ToString());
        }

        private const string Schema = @"{
    ""$comment"": ""Root type."",
    ""type"": ""object"",
    ""properties"": {
        ""contents"": {
            ""$ref"": ""#/$defs/contentItem"",
            ""$comment"": ""some definition.""
        },
        ""contents2"": {
            ""$ref"": ""#/$defs/contentItem""
        }
    },
    ""$defs"": {
        ""contentItem"": {
            ""type"": ""object"",
            ""properties"": {
                ""dataObjectValue"": {
                    ""type"": ""string""
                }
            },
            ""additionalProperties"": false
        }
    },
    ""additionalProperties"": false,
    ""required"": [
        ""contents""
    ]
}";

        private const string Data = @"{
    ""contents"": 
    {
        ""dataObjectValue"": ""lastOrderQTY""
    }
}";
    }
}
