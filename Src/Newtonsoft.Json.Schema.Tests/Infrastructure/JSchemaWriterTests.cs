#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.V4;
using Newtonsoft.Json.Schema.V4.Infrastructure;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaWriterTests : TestFixtureBase
    {
        [Test]
        public void WriteTo_AdditionalProperties()
        {
            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            JSchema4 schema = JSchema4.Parse(@"{
  ""description"":""AdditionalProperties"",
  ""type"":[""string"", ""integer""],
  ""additionalProperties"":{""type"":[""object"", ""boolean""]}
}");

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""AdditionalProperties"",
  ""type"": [
    ""string"",
    ""integer""
  ],
  ""additionalProperties"": {
    ""type"": [
      ""boolean"",
      ""object""
    ]
  }
}", json);
        }

        [Test]
        public void WriteTo_Properties()
        {
            JSchema4 schema = JSchema4.Parse(@"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""},
    ""hobbies"":
    {
      ""type"":""array"",
      ""items"": {""type"":""string""}
    }
  }
}");

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""A person"",
  ""type"": ""object"",
  ""properties"": {
    ""name"": {
      ""type"": ""string""
    },
    ""hobbies"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""string""
      }
    }
  }
}", json);
        }

        [Test]
        public void WriteTo_Enum()
        {
            JSchema4 schema = JSchema4.Parse(@"{
  ""description"":""Type"",
  ""type"":[""string"",""array""],
  ""items"":{},
  ""enum"":[""string"",""object"",""array"",""boolean"",""number"",""integer"",""null"",""any""]
}");

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""Type"",
  ""type"": [
    ""string"",
    ""array""
  ],
  ""items"": {},
  ""enum"": [
    ""string"",
    ""object"",
    ""array"",
    ""boolean"",
    ""number"",
    ""integer"",
    ""null"",
    ""any""
  ]
}", json);
        }

        [Test]
        public void WriteTo_CircularReference()
        {
            string json = @"{
  ""id"":""CircularReferenceArray"",
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""CircularReferenceArray""}
}";

            JSchema4 schema = JSchema4.Parse(json);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string writtenJson = writer.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""CircularReferenceArray"",
  ""description"": ""CircularReference"",
  ""type"": ""array"",
  ""items"": {
    ""$ref"": ""CircularReferenceArray""
  }
}", writtenJson);
        }

        [Test]
        public void WriteTo_DisallowMultiple()
        {
            JSchema4 schema = JSchema4.Parse(@"{
  ""description"":""Type"",
  ""type"":[""string"",""array""],
  ""items"":{},
  ""disallow"":[""string"",""object"",""array""]
}");

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""Type"",
  ""type"": [
    ""string"",
    ""array""
  ],
  ""items"": {},
  ""not"": {
    ""type"": [
      ""string"",
      ""object"",
      ""array""
    ]
  }
}", json);
        }

        [Test]
        public void WriteTo_DisallowSingle()
        {
            JSchema4 schema = JSchema4.Parse(@"{
  ""description"":""Type"",
  ""type"":[""string"",""array""],
  ""items"":{},
  ""disallow"":""any""
}");

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""Type"",
  ""type"": [
    ""string"",
    ""array""
  ],
  ""items"": {},
  ""not"": {
    ""type"": ""any""
  }
}", json);
        }

        [Test]
        public void WriteTo_InnerSchemaOfIdInternalSchema()
        {
            JSchema4 nestedInId = new JSchema4
            {
                Type = JSchemaType.Boolean
            };
            JSchema4 hasId = new JSchema4
            {
                Id = new Uri("purpleMonkeyDishwasher", UriKind.RelativeOrAbsolute),
                Not = nestedInId
            };

            JSchema4 root = new JSchema4();
            root.Properties = new Dictionary<string, JSchema4>
            {
                { "prop1", hasId }
            };
            root.Not = nestedInId;

            Console.WriteLine(root);
        }

        [Test]
        public void WriteTo_InnerSchemaOfExternalResolvedReference()
        {
            JSchema4 nestedReference = new JSchema4()
            {
                Type = JSchemaType.Boolean
            };

            JSchema4 referenceSchema = new JSchema4
            {
                Id = new Uri("http://localhost/test"),
                Items = new List<JSchema4>
                {
                    nestedReference
                }
            };

            JSchema4 root = new JSchema4
            {
                Id = new Uri("#root", UriKind.RelativeOrAbsolute),
                Not = nestedReference
            };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            root.WriteTo(jsonWriter, new JSchema4WriteSettings
            {
                ExternalSchemas = new List<ExternalSchema>
                {
                    new ExternalSchema(referenceSchema)
                }
            });

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""#root"",
  ""not"": {
    ""$ref"": ""http://localhost/test#/items/0""
  }
}", json);
        }

        [Test]
        public void WriteTo_MultipleItems()
        {
            JSchema4 schema = JSchema4.Parse(@"{
  ""items"":[{},{}]
}");

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""items"": [
    {},
    {}
  ]
}", json);
        }

        [Test]
        public void WriteTo_Required()
        {
            JSchema4 schema = new JSchema4
            {
                Properties =
                {
                    { "prop1", new JSchema4() }
                },
                Required =
                {
                    "prop1"
                }
            };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""properties"": {
    ""prop1"": {}
  },
  ""required"": [
    ""prop1""
  ]
}", json);
        }

        [Test]
        public void WriteTo_ExclusiveMinimum_ExclusiveMaximum()
        {
            JSchema4 schema = new JSchema4();
            schema.ExclusiveMinimum = true;
            schema.ExclusiveMaximum = true;

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""exclusiveMinimum"": true,
  ""exclusiveMaximum"": true
}", json);
        }

        [Test]
        public void WriteTo_PatternProperties()
        {
            JSchema4 schema = new JSchema4();
            schema.PatternProperties = new Dictionary<string, JSchema4>
            {
                { "[abc]", new JSchema4() }
            };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""patternProperties"": {
    ""[abc]"": {}
  }
}", json);
        }

        [Test]
        public void ToString_AdditionalItems()
        {
            JSchema4 schema = JSchema4.Parse(@"{
    ""additionalItems"": {""type"": ""integer""}
}");

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""additionalItems"": {
    ""type"": ""integer""
  }
}", json);
        }

        [Test]
        public void WriteTo_PositionalItemsValidation_True()
        {
            JSchema4 schema = new JSchema4();
            schema.ItemsPositionValidation = true;

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""items"": []
}", json);
        }

        [Test]
        public void WriteTo_PositionalItemsValidation_TrueWithItemsSchema()
        {
            JSchema4 schema = new JSchema4();
            schema.ItemsPositionValidation = true;
            schema.Items = new List<JSchema4> { new JSchema4 { Type = JSchemaType.String } };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""items"": [
    {
      ""type"": ""string""
    }
  ]
}", json);
        }

        [Test]
        public void WriteTo_PositionalItemsValidation_FalseWithItemsSchema()
        {
            JSchema4 schema = new JSchema4();
            schema.Items = new List<JSchema4> { new JSchema4 { Type = JSchemaType.String } };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""items"": {
    ""type"": ""string""
  }
}", json);
        }

        [Test]
        public void WriteTo_ExternalResolvedReferenceInDefinition()
        {
            JSchema4 referenceSchema = new JSchema4
            {
                Id = new Uri("http://localhost/test")
            };

            JSchema4 root = new JSchema4
            {
                Id = new Uri("#root", UriKind.RelativeOrAbsolute),
                Not = referenceSchema,
                ExtensionData =
                {
                    {
                        "definitions",
                        new JObject
                        {
                            { "reference", referenceSchema }
                        }
                    }
                }
            };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            root.WriteTo(jsonWriter, new JSchema4WriteSettings
            {
                ExternalSchemas = new List<ExternalSchema>
                {
                    new ExternalSchema(referenceSchema)
                }
            });

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""#root"",
  ""definitions"": {
    ""reference"": {
      ""$ref"": ""http://localhost/test""
    }
  },
  ""not"": {
    ""$ref"": ""http://localhost/test""
  }
}", json);
        }

        [Test]
        public void WriteComplex()
        {
            JSchema4 schema = new JSchema4();
            schema.Id = new Uri("root", UriKind.RelativeOrAbsolute);
            schema.Type = JSchemaType.Boolean;

            JSchema4 file = new JSchema4();
            file.Id = new Uri("file", UriKind.RelativeOrAbsolute);
            file.Properties = new Dictionary<string, JSchema4>
            {
                { "blah", schema }
            };
            file.ExtensionData["definitions"] = new JObject
            {
                { "parent", schema }
            };

            schema.Properties = new Dictionary<string, JSchema4>
            {
                { "storage", file }
            };
            schema.ExtensionData["definitions"] = new JObject
            {
                { "file", file },
                { "file2", file }
            };
            schema.Items = new List<JSchema4>
            {
                new JSchema4
                {
                    Type = JSchemaType.Integer | JSchemaType.Null
                },
                file
            };
            schema.ItemsPositionValidation = true;
            schema.Not = file;
            schema.AllOf.Add(file);
            schema.AllOf.Add(new JSchema4
            {
                Type = JSchemaType.Integer | JSchemaType.Null
            });
            schema.AllOf.Add(file);
            schema.AnyOf = new List<JSchema4>
            {
                file,
                new JSchema4
                {
                    Type = JSchemaType.Integer | JSchemaType.Null
                },
                schema
            };
            schema.OneOf = new List<JSchema4>
            {
                file,
                new JSchema4
                {
                    Type = JSchemaType.Integer | JSchemaType.Null
                },
                schema
            };
            schema.Not = file;

            JSchema4 file2 = (JSchema4)schema.ExtensionData["definitions"]["file"];

            Assert.AreEqual(file, file2);

            string json = schema.ToString();

            Assert.AreEqual(@"{
  ""id"": ""root"",
  ""definitions"": {
    ""file"": {
      ""id"": ""file"",
      ""definitions"": {
        ""parent"": {
          ""$ref"": ""root""
        }
      },
      ""properties"": {
        ""blah"": {
          ""$ref"": ""root""
        }
      }
    },
    ""file2"": {
      ""$ref"": ""file""
    }
  },
  ""type"": ""boolean"",
  ""properties"": {
    ""storage"": {
      ""$ref"": ""file""
    }
  },
  ""items"": [
    {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    {
      ""$ref"": ""file""
    }
  ],
  ""allOf"": [
    {
      ""$ref"": ""file""
    },
    {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    {
      ""$ref"": ""file""
    }
  ],
  ""anyOf"": [
    {
      ""$ref"": ""file""
    },
    {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    {
      ""$ref"": ""root""
    }
  ],
  ""oneOf"": [
    {
      ""$ref"": ""file""
    },
    {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    {
      ""$ref"": ""root""
    }
  ],
  ""not"": {
    ""$ref"": ""file""
  }
}", json);
        }
    }
}