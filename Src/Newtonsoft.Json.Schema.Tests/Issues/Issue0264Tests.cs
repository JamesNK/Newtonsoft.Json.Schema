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
    public class Issue0264Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JObject clientJson = JObject.Parse(Json);
            JSchema schema = JSchema.Parse(SchemaRecursiveRef);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].Message);
            Assert.AreEqual(ErrorType.Dependencies, errorMessages[0].ErrorType);
        }

        [Test]
        public void Test_Ref()
        {
            JObject clientJson = JObject.Parse(Json);
            JSchema schema = JSchema.Parse(SchemaRef);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].Message);
            Assert.AreEqual(ErrorType.Dependencies, errorMessages[0].ErrorType);
        }

        [Test]
        public void Test_WithProperties_Ref()
        {
            JObject clientJson = JObject.Parse(Json);
            JSchema schema = JSchema.Parse(SchemaRefWithProperties);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual("Conditional schema has a circular dependency and can't be evaluated.", errorMessages[0].Message);
            Assert.AreEqual(ErrorType.Dependencies, errorMessages[0].ErrorType);
        }

        private const string SchemaRef = @"{
  ""dependencies"": {
    ""Lorem_862"": {
      ""$ref"": ""#""
    }
  }
}";

        private const string SchemaRefWithProperties = @"{
  ""dependencies"": {
    ""Lorem_862"": {
      ""$ref"": ""#""
    }
  },
  ""properties"": {
    ""prop1"": false
  }
}";

        private const string SchemaRecursiveRef = @"{
  ""dependencies"": {
    ""Lorem_862"": {
      ""$recursiveRef"": ""#""
    }
  }
}";

        private const string Json = @"{""Lorem_862"":true}";

        private const string JsonWithProp1 = @"{""Lorem_862"":true, ""prop1"":1}";
    }
}
