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
    public class Issue0316Tests : TestFixtureBase
    {
        [Test]
        public void TestDateTime()
        {
            JSchema schema = JSchema.Parse(SchemaDateTime);
            JObject o = JObject.Parse(@"{
  ""aaa"" : ""2023-04-17T10:40:55.0341019069999999999999999999999999999999999999999999999999999999999999999999999999Z""
}");

            Assert.IsTrue(o.IsValid(schema));
        }

        [Test]
        public void TestTime()
        {
            JSchema schema = JSchema.Parse(SchemaTime);
            JObject o = JObject.Parse(@"{
  ""aaa"" : ""10:40:55.0341019069999999999999999999999999999999999999999999999999999999999999999999999999Z""
}");

            Assert.IsTrue(o.IsValid(schema));
        }

        private const string SchemaDateTime = @"{
  ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
  ""type"": ""object"",
  ""properties"": {
    ""aaa"": {
      ""type"": ""string"",
      ""format"": ""date-time""
    }
  }
}";

        private const string SchemaTime = @"{
  ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
  ""type"": ""object"",
  ""properties"": {
    ""aaa"": {
      ""type"": ""string"",
      ""format"": ""time""
    }
  }
}";
    }
}
