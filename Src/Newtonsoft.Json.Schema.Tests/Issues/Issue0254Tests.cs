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
    public class Issue0254Tests : TestFixtureBase
    {
        [Test]
        public void Test_TypeMismatch()
        {
            JObject clientJson = JObject.Parse("{}");
            JSchema schema = JSchema.Parse(Schema);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("Invalid type. Expected Integer but got Object.", errorMessages[0].Message);
        }

        [Test]
        public void Test_TypeMatch()
        {
            JToken clientJson = JToken.Parse("1");
            JSchema schema = JSchema.Parse(Schema);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("JSON is valid against schema from 'not'.", errorMessages[0].Message);
        }

        [Test]
        public void Test_AdditionalConditionals()
        {
            JToken clientJson = JToken.Parse("1");
            JSchema schema = JSchema.Parse(SchemaMultipleConditionals);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errorMessages[0].Message);
        }

        private const string Schema = @"{
  ""type"": ""integer"",
  ""not"": {
    ""$ref"": ""#""
  }
}";

        private const string SchemaMultipleConditionals = @"{
  ""not"": {
    ""$ref"": ""#""
  },
  ""oneOf"": [
    {
      ""type"": ""boolean""
    }
  ]
}";
    }
}
