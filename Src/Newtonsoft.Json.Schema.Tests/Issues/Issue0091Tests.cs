#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue0091Tests : TestFixtureBase
    {
        public class Root
        {
            public List<ItemA> FirstArray { get; set; }
            public List<ItemB> SecondArray { get; set; }
        }

        public class ItemA
        {
            public List<CustomData> Data { get; set; }
        }

        public class ItemB
        {
            public List<CustomData> Data { get; set; }
        }

        public class CustomData
        {
            public string Key { get; set; }
            public string Text { get; set; }
        }

        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema1 = generator.Generate(new Root().GetType());
            string json1 = schema1.ToString();

            StringAssert.AreEqual(@"{
  ""definitions"": {
    ""CustomData"": {
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Key"": {
          ""type"": [
            ""string"",
            ""null""
          ]
        },
        ""Text"": {
          ""type"": [
            ""string"",
            ""null""
          ]
        }
      },
      ""required"": [
        ""Key"",
        ""Text""
      ]
    },
    ""ItemA"": {
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Data"": {
          ""type"": [
            ""array"",
            ""null""
          ],
          ""items"": {
            ""$ref"": ""#/definitions/CustomData""
          }
        }
      },
      ""required"": [
        ""Data""
      ]
    },
    ""ItemB"": {
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Data"": {
          ""type"": [
            ""array"",
            ""null""
          ],
          ""items"": {
            ""$ref"": ""#/definitions/CustomData""
          }
        }
      },
      ""required"": [
        ""Data""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""FirstArray"": {
      ""type"": [
        ""array"",
        ""null""
      ],
      ""items"": {
        ""$ref"": ""#/definitions/ItemA""
      }
    },
    ""SecondArray"": {
      ""type"": [
        ""array"",
        ""null""
      ],
      ""items"": {
        ""$ref"": ""#/definitions/ItemB""
      }
    }
  },
  ""required"": [
    ""FirstArray"",
    ""SecondArray""
  ]
}", json1);

            JSchema schema2 = JSchema.Parse(json1);
            string json2 = schema2.ToString();

            StringAssert.AreEqual(json1, json2);
        }
    }
}