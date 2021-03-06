#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Linq;
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
    public class Issue0042Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""http://DTCP05IFCENT01.qa.local:8080/identity-management/v1/authenticated.json"",
  ""definitions"": {
    ""event"": {
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""event"": {
      ""$ref"": ""#/definitions/event""
    }
  },
  ""required"": [
    ""event""
  ]
}";

            JSchema schema = JSchema.Parse(schemaJson);
            JSchema eventSchema = schema.ExtensionData["definitions"]["event"].Annotations<JSchemaAnnotation>().Single().GetSchema(null);

            Assert.AreEqual(eventSchema, schema.Properties["event"]);
        }

        [Test]
        public void Test2()
        {
            string json = @"{
  ""id"": ""TestComplexClass1"",
  ""definitions"": {
    ""http://DTCP05IFCENT01.qa.local:8080/identity-management/v1/authenticated.json"": {
      ""id"": ""http://DTCP05IFCENT01.qa.local:8080/identity-management/v1/authenticated.json""
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""TestProperty"": {
      ""$ref"": ""http://dtcp05ifcent01.qa.local:8080/identity-management/v1/authenticated.json""
    }
  },
  ""required"": [
    ""TestProperty""
  ]
}";

            JSchema schema = JSchema.Parse(json);
            JSchema definitionSchema = schema.ExtensionData["definitions"]["http://DTCP05IFCENT01.qa.local:8080/identity-management/v1/authenticated.json"].Annotations<JSchemaAnnotation>().Single().GetSchema(null);

            Assert.AreEqual(definitionSchema, schema.Properties["TestProperty"]);
        }
    }
}