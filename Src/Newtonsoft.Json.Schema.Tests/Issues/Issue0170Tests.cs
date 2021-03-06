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
    public class Issue0170Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            JArray a = JArray.Parse(Json);

            bool result = a.IsValid(schema);

            Assert.IsTrue(result);
        }

        private const string SchemaJson = @"{
  ""type"": ""array"",
  ""items"": {
    ""oneOf"": [
      {
        ""type"": ""object"",
        ""propertyNames"": {
          ""type"": ""string"",
          ""enum"": [
            ""validName""
          ]
        },
        ""additionalProperties"": true
      },
      {
        ""type"": ""string"",
        ""enum"": [
          ""validName""
        ]
      }
    ]
  }
}";

        private const string Json = @"[
  {
    ""validName"": {
      ""a"": 1
    }
  }
]";
    }
}