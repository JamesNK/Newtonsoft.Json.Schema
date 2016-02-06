#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue16Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            var schema = JSchema.Parse(@"{
  ""description"" : ""Example Contact Information Array JSON Schema"",
  ""type"" : ""array"",
  ""items"" : {
    ""title"" : ""A Contact Information object"",
    ""type"" : ""object"",
    ""properties"" : {
      ""name"" : {
        ""type"" : ""string"",
        ""enum"" : [""home"", ""work"", ""other""]
      },
      ""email"" : {
        ""type"" : ""string"",
        ""optional"" : true,
        ""format"" : ""email""
      }
    },
    ""minItems"" : 1,
    ""maxItems"" : 5
  }
}");

            JArray a = JArray.Parse(@"[
  {
    ""name"": ""work"", 
    ""email"": ""{xyz@abc.gr""
  }
]");

            IList<string> errorMessages;
            Assert.IsTrue(a.IsValid(schema, out errorMessages));
        }
    }
}