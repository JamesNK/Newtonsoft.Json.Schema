#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class Issue0088Tests : TestFixtureBase
    {
        private string originalSchemaJson = @"{

  ""type"": ""object"",
  ""$ref"":""#"",
  ""definitions"": {
  }
}";

        private string originalSchemaJsonDraft7 = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""type"": ""object"",
  ""$ref"":""#"",
  ""definitions"": {
  }
}";

        [Test]
        public void Test_Draft7()
        {
            ExceptionAssert.Throws<JSchemaReaderException>(() =>
            {
                JSchema.Parse(originalSchemaJsonDraft7);
            }, "Could not resolve schema reference '#'. Path '', line 1, position 1.");
        }

        [Test]
        public void Test_DraftUnset_Match()
        {
            var s = JSchema.Parse(originalSchemaJson);

            var writtenJson = s.ToString();

            StringAssert.AreEqual(@"{
  ""definitions"": {},
  ""type"": ""object"",
  ""$ref"": ""#""
}", writtenJson);

            JToken t = JToken.Parse("{'test':'value'}");

            Assert.IsTrue(t.IsValid(s));
        }

        [Test]
        public void Test_DraftUnset_NoMatch()
        {
            var s = JSchema.Parse(originalSchemaJson);

            var writtenJson = s.ToString();

            StringAssert.AreEqual(@"{
  ""definitions"": {},
  ""type"": ""object"",
  ""$ref"": ""#""
}", writtenJson);

            JToken t = JToken.Parse("[]");

            Assert.IsFalse(t.IsValid(s));
        }
    }
}