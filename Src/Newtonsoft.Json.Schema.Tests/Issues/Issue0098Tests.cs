#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
    public class Issue0098Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            JSchema schema = generator.Generate(typeof(TypeOne));

            // A process will generate the schema and cache it
            string schemaString = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""typeOne"",
  ""definitions"": {
    ""settings"": {
      ""id"": ""settings"",
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""a"": {
          ""type"": [
            ""string"",
            ""null""
          ]
        },
        ""b"": {
          ""type"": ""integer""
        }
      },
      ""required"": [
        ""a"",
        ""b""
      ]
    },
    ""typeTwo"": {
      ""id"": ""typeTwo"",
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""prop2"": {
          ""type"": [
            ""string"",
            ""null""
          ]
        },
        ""common"": {
          ""$ref"": ""settings""
        }
      },
      ""required"": [
        ""prop2"",
        ""common""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""prop1"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""prop2"": {
      ""$ref"": ""settings""
    },
    ""prop3"": {
      ""$ref"": ""typeTwo""
    }
  },
  ""required"": [
    ""prop1"",
    ""prop2"",
    ""prop3""
  ]
}", schemaString);

            JSchema parsedSchema = JSchema.Parse(schemaString);

            Assert.AreEqual(new Uri("typeOne", UriKind.RelativeOrAbsolute), parsedSchema.Id);
        }

        [JsonObject("settings")]
        public class CommonViewModel
        {
            [JsonProperty("a")]
            public string A { get; set; }

            [JsonProperty("b")]
            public int B { get; set; }
        }

        [JsonObject("typeOne")]
        public class TypeOne
        {
            [JsonProperty("prop1")]
            public string Prop1 { get; set; }

            [JsonProperty("prop2")]
            public CommonViewModel Common { get; set; }

            [JsonProperty("prop3")]
            public TypeTwo TypeTwo { get; set; }
        }

        [JsonObject("typeTwo")]
        public class TypeTwo
        {
            [JsonProperty("prop2")]
            public string Prop2 { get; set; }

            [JsonProperty("common")]
            public CommonViewModel Common { get; set; }
        }
    }
}