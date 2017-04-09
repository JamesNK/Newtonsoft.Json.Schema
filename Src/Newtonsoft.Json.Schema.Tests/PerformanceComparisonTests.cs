#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !DNXCORE50
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

#pragma warning disable 0618
namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class PerformanceComparisonTests : TestFixtureBase
    {
        private const int ValidationCount = 1000;

        [Test]
        public void IsValidPerformance()
        {
            JArray a = JArray.Parse(Json);
            a.IsValid(Schema);
            a.IsValid(SchemaV3);

            using (new PerformanceTester("Raw"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    JsonTextReader reader = new JsonTextReader(new StringReader(Json));

                    while (reader.Read())
                    {

                    }
                }
            }

            GC.Collect();

            using (new PerformanceTester("JSchema"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    JsonTextReader reader = new JsonTextReader(new StringReader(Json));

                    JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
                    validatingReader.Schema = Schema;
                    while (validatingReader.Read())
                    {

                    }
                }
            }

            GC.Collect();

            using (new PerformanceTester("JsonSchema"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    JsonTextReader reader = new JsonTextReader(new StringReader(Json));

                    JsonValidatingReader vr = new JsonValidatingReader(reader);
                    vr.Schema = SchemaV3;
                    while (vr.Read())
                    {

                    }
                }
            }

            XmlSchema schema = XmlSchema.Read(new StringReader(SchemaXml), (sender, args) =>
            {
                throw new Exception(args.Message);
            });
            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(schema);
            set.Compile();

            GC.Collect();

            using (new PerformanceTester("IsValid_XmlSchema"))
            {
                for (int i = 1; i < ValidationCount; i++)
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas = set;
                    settings.ValidationEventHandler += ValidatingReader_ValidationEventHandler;

                    XmlReader reader = XmlReader.Create(new StringReader(Xml), settings);

                    while (reader.Read())
                    {

                    }
                }
            }
        }

        private void ValidatingReader_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            throw new Exception(e.Message);
        }

        private readonly JSchema Schema = JSchema.Parse(@"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""title"": ""Product set"",
    ""type"": ""array"",
    ""items"": {
        ""title"": ""Product"",
        ""type"": ""object"",
        ""properties"": {
            ""id"": {
                ""description"": ""The unique identifier for a product"",
                ""type"": ""number""
            },
            ""name"": {
                ""type"": ""string""
            },
            ""price"": {
                ""type"": ""number"",
                ""minimum"": 0,
                ""exclusiveMinimum"": true
            },
            ""tags"": {
                ""type"": ""array"",
                ""items"": {
                    ""type"": ""string""
                },
                ""minItems"": 1,
                ""uniqueItems"": true
            },
            ""dimensions"": {
                ""type"": ""object"",
                ""properties"": {
                    ""length"": {""type"": ""number""},
                    ""width"": {""type"": ""number""},
                    ""height"": {""type"": ""number""}
                },
                ""required"": [""length"", ""width"", ""height""]
            },
            ""warehouseLocation"": {
                ""description"": ""A geographical coordinate"",
                ""type"": ""object"",
                ""properties"": {
                    ""latitude"": { ""type"": ""number"" },
                    ""longitude"": { ""type"": ""number"" }
                }
            }
        },
        ""required"": [""id"", ""name"", ""price""]
    }
}");

        private readonly JsonSchema SchemaV3 = JsonSchema.Parse(@"{
    ""title"": ""Product set"",
    ""type"": ""array"",
    ""items"": {
        ""title"": ""Product"",
        ""type"": ""object"",
        ""properties"": {
            ""id"": {
                ""description"": ""The unique identifier for a product"",
                ""type"": ""number"",
                ""required"": true
            },
            ""name"": {
                ""type"": ""string"",
                ""required"": true
            },
            ""price"": {
                ""type"": ""number"",
                ""minimum"": 0,
                ""exclusiveMinimum"": true,
                ""required"": true
            },
            ""tags"": {
                ""type"": ""array"",
                ""items"": {
                    ""type"": ""string""
                },
                ""minItems"": 1,
                ""uniqueItems"": true
            },
            ""dimensions"": {
                ""type"": ""object"",
                ""properties"": {
                    ""length"": {""type"": ""number"", ""required"": true},
                    ""width"": {""type"": ""number"", ""required"": true},
                    ""height"": {""type"": ""number"", ""required"": true}
                }
            },
            ""warehouseLocation"": {
                ""description"": ""A geographical coordinate"",
                ""type"": ""object"",
                ""properties"": {
                    ""latitude"": { ""type"": ""number"" },
                    ""longitude"": { ""type"": ""number"" }
                }
            }
        }
    }
}");

        private const string Json = @"[
    {
        ""id"": 2,
        ""name"": ""An ice sculpture"",
        ""price"": 12.50,
        ""tags"": [""cold"", ""ice""],
        ""dimensions"": {
            ""length"": 7.0,
            ""width"": 12.0,
            ""height"": 9.5
        },
        ""warehouseLocation"": {
            ""latitude"": -78.75,
            ""longitude"": 20.4
        }
    },
    {
        ""id"": 3,
        ""name"": ""A blue mouse"",
        ""price"": 25.50,
        ""dimensions"": {
            ""length"": 3.1,
            ""width"": 1.0,
            ""height"": 1.0
        },
        ""warehouseLocation"": {
            ""latitude"": 54.4,
            ""longitude"": -32.7
        }
    }
]";

        private const string SchemaXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" elementFormDefault=""qualified"" attributeFormDefault=""unqualified"">
  <xs:element name=""products"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""product"" maxOccurs=""unbounded"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""id"" type=""xs:int""></xs:element>
              <xs:element name=""name"" type=""xs:string""></xs:element>
              <xs:element name=""price"">
                <xs:simpleType>
                  <xs:restriction base=""xs:decimal"">
                    <xs:minExclusive value=""0"" />
                  </xs:restriction>
                </xs:simpleType>
              </xs:element>
              <xs:element name=""tags"" minOccurs=""0"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""tag"" maxOccurs=""unbounded"" type=""xs:string""></xs:element>
                  </xs:sequence>
                </xs:complexType>
                <xs:unique name=""TagUnique"">
                  <xs:selector xpath=""tag"" />
                  <xs:field xpath=""."" />
                </xs:unique>
              </xs:element>
              <xs:element name=""dimensions"" minOccurs=""0"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""length"" type=""xs:double""></xs:element>
                    <xs:element name=""width"" type=""xs:double""></xs:element>
                    <xs:element name=""height"" type=""xs:double""></xs:element>
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
              <xs:element name=""warehouseLocation"" minOccurs=""0"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""latitude"" type=""xs:double"" minOccurs=""0""></xs:element>
                    <xs:element name=""longitude"" type=""xs:double"" minOccurs=""0""></xs:element>
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";

        private const string Xml = @"<products>
  <product>
    <id>2</id>
    <name>An ice sculpture</name>
    <price>12.5</price>
    <tags>
      <tag>cold</tag>
      <tag>ice</tag>
    </tags>
    <dimensions>
      <length>7</length>
      <width>12</width>
      <height>9.5</height>
    </dimensions>
    <warehouseLocation>
      <latitude>-78.75</latitude>
      <longitude>20.4</longitude>
    </warehouseLocation>
  </product>
  <product>
    <id>3</id>
    <name>A blue mouse</name>
    <price>25.5</price>
    <dimensions>
      <length>3.1</length>
      <width>1</width>
      <height>1</height>
    </dimensions>
    <warehouseLocation>
      <latitude>54.4</latitude>
      <longitude>-32.7</longitude>
    </warehouseLocation>
  </product>
</products>";
    }
}
#pragma warning restore 0618
#endif