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
    public class Issue0354Tests : TestFixtureBase
    {
        [Test]
        public void Test_DotStart()
        {
            string schemaJson = @"{
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""$ref"": ""./a"",
  ""$defs"": {
    ""a"": { ""$id"": ""./a"", ""$ref"": ""./b"", ""required"": [""prop1""] },
    ""b"": { ""$id"": ""./b"", ""$ref"": ""./a"", ""required"": [""prop2""] }
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);

            Assert.AreEqual("./a", schema.Ref.Id.OriginalString);
            Assert.AreEqual("./b", schema.Ref.Ref.Id.OriginalString);
            Assert.AreSame(schema.Ref, schema.Ref.Ref.Ref);
        }

        [Test]
        public void Test_SlashStart()
        {
            string schemaJson = @"{
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""$ref"": ""/a"",
  ""$defs"": {
    ""a"": { ""$id"": ""/a"", ""$ref"": ""/b"" },
    ""b"": { ""$id"": ""/b"", ""$ref"": ""/a"" }
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);
        }

        [Test]
        public void Test_ValidateRecursive()
        {
            string schemaJson = @"{
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""$id"": ""https://example.com/"",
  ""$ref"": ""./a"",
  ""$defs"": {
    ""a"": { ""$id"": ""./a"", ""$ref"": ""./b"", ""required"": [""prop1""] },
    ""b"": { ""$id"": ""./b"", ""$ref"": ""./a"", ""required"": [""prop2""] }
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);

            JObject o1 = new JObject();
            Assert.IsFalse(o1.IsValid(schema, out IList<ValidationError> errorMessages1));

            JObject o2 = new JObject
            {
                ["prop1"] = 1,
                ["prop2"] = 2
            };
            Assert.IsTrue(o2.IsValid(schema, out IList<ValidationError> errorMessages2));
        }
    }
}
