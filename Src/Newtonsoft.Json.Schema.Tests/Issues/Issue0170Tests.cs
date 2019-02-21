#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
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
            string schemaJson = @"{
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

            JSchema schema = JSchema.Parse(schemaJson);

            JArray a = JArray.Parse(Json);

            bool result = a.IsValid(schema);

            Assert.IsTrue(result);
        }

        private const string Json = @"[
  {
    ""validName"": {
      ""a"": 1
    }
  }
]";

        private void OnValidate(object sender, SchemaValidationEventArgs e)
        {
        }
    }
}