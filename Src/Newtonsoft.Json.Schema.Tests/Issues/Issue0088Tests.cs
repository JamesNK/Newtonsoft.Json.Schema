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
    public class Issue0088Tests : TestFixtureBase
    {
        private readonly string originalSchemaJson = @"{

  ""type"": ""object"",
  ""$ref"":""#"",
  ""definitions"": {
  }
}";

        private readonly string originalSchemaJsonDraft7 = @"{
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
            JSchema s = JSchema.Parse(originalSchemaJson);

            string writtenJson = s.ToString();

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
            JSchema s = JSchema.Parse(originalSchemaJson);

            string writtenJson = s.ToString();

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