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
    public class Issue78Tests : TestFixtureBase
    {
        private string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""MySchema"",
  ""$type"": ""schema"",
  ""definitions"": {
    ""myDefinition"": {
      ""type"": ""object"",
      ""properties"": {
        ""items"": {
          ""type"": ""object"",
          ""properties"": {
            ""value"": {
              ""type"": ""array"",
              ""items"": {
                ""oneOf"": [
                  {
                    ""$ref"": ""MySchema#/definitions/myDefinition/properties/items/properties/value""
                  },
                  {
                    ""$ref"": ""MySchema#/definitions/myDefinition""
                  }
                ]
              }
            }
          }
        }
      }
    }
  },
  ""allOf"": [
    {
      ""properties"": {
        ""myProperty"": {
          ""$ref"": ""MySchema#/definitions/myDefinition/properties/items/properties/value""
        }
      }
    }
  ]
}";

        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(schemaJson);
            string json = schema.ToString();

            StringAssert.AreEqual(schemaJson, schema.ToString());

            JSchema schema2 = JSchema.Parse(json);
            JSchema myDefinitionSchema = (JSchema)schema2.ExtensionData["definitions"]["myDefinition"];
            JSchema valueSchema = myDefinitionSchema.Properties["items"].Properties["value"];
            
            Assert.AreEqual(myDefinitionSchema, valueSchema.Items[0].OneOf[1]);

            Assert.AreEqual(valueSchema, schema2.AllOf[0].Properties["myProperty"]);
        }
    }
}