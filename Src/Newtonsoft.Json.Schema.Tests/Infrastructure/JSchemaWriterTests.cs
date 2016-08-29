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
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaWriterTests : TestFixtureBase
    {
        [Test]
        public void WriteTo_Schema_Root()
        {
            JSchema s = new JSchema
            {
                SchemaVersion = new Uri("http://www.google.com")
            };

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://www.google.com""
}", json);
        }

        [Test]
        public void WriteTo_Schema_NonRoot()
        {
            JSchema s = new JSchema
            {
                Not = new JSchema
                {
                    SchemaVersion = new Uri("http://www.google.com")
                }
            };

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""not"": {}
}", json);
        }

        [Test]
        public void WriteTo_MaximumLength_Large()
        {
            JSchema s = new JSchema
            {
                MaximumLength = long.MaxValue
            };

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""maxLength"": 9223372036854775807
}", json);
        }

        [Test]
        public void WriteTo_UniqueItems()
        {
            JSchema s = new JSchema
            {
                UniqueItems = true
            };

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""uniqueItems"": true
}", json);
        }

        [Test]
        public void WriteTo_ReferenceWithRootId()
        {
            JSchema nested = new JSchema
            {
                Type = JSchemaType.Object
            };

            JSchema s = new JSchema();
            s.Id = new Uri("http://www.jnk.com/");
            s.Items.Add(nested);
            s.Properties["test"] = nested;

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""http://www.jnk.com/"",
  ""properties"": {
    ""test"": {
      ""type"": ""object""
    }
  },
  ""items"": {
    ""$ref"": ""#/properties/test""
  }
}", json);
        }

        [Test]
        public void WriteTo_ReferenceToPatternChild()
        {
            JSchema nested = new JSchema
            {
                Type = JSchemaType.Object
            };

            JSchema s = new JSchema();
            s.Id = new Uri("http://www.jnk.com/");
            s.Properties["pattern_parent"] = new JSchema
            {
                PatternProperties =
                {
                    { "///~~~test~/~/~", nested }
                }
            };
            s.Properties["ref_parent"] = new JSchema
            {
                Items =
                {
                    nested
                }
            };

            string json = s.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""http://www.jnk.com/"",
  ""properties"": {
    ""pattern_parent"": {
      ""patternProperties"": {
        ""///~~~test~/~/~"": {
          ""type"": ""object""
        }
      }
    },
    ""ref_parent"": {
      ""items"": {
        ""$ref"": ""#/properties/pattern_parent/patternProperties/~1~1~1~0~0~0test~0~1~0~1~0""
      }
    }
  }
}", json);
        }

        [Test]
        public void WriteTo_AdditionalProperties()
        {
            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            JSchema schema = JSchema.Parse(@"{
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
            JSchema schema = JSchema.Parse(@"{
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
            JSchema schema = JSchema.Parse(@"{
  ""description"":""Type"",
  ""type"":[""string"",""array""],
  ""items"":{},
  ""enum"":[""string"",""object"",""array"",""boolean"",""number"",""integer"",""null""]
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
    ""null""
  ]
}", json);
        }

        [Test]
        public void WriteTo_CircularReference()
        {
            string json = @"{
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""#""}
}";

            JSchema schema = JSchema.Parse(json);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter);

            string writtenJson = writer.ToString();

            StringAssert.AreEqual(@"{
  ""description"": ""CircularReference"",
  ""type"": ""array"",
  ""items"": {
    ""$ref"": ""#""
  }
}", writtenJson);
        }

        [Test]
        public void WriteTo_CircularReference_WithRootId()
        {
            string json = @"{
  ""id"":""CircularReferenceArray"",
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""CircularReferenceArray""}
}";

            JSchema schema = JSchema.Parse(json);

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
            JSchema schema = JSchema.Parse(@"{
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
            JSchema schema = JSchema.Parse(@"{
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
  ""not"": {}
}", json);
        }

        [Test]
        public void WriteTo_InnerSchemaOfIdInternalSchema()
        {
            JSchema nestedInId = new JSchema
            {
                Type = JSchemaType.Boolean
            };
            JSchema hasId = new JSchema
            {
                Id = new Uri("purpleMonkeyDishwasher", UriKind.RelativeOrAbsolute),
                Not = nestedInId
            };

            JSchema root = new JSchema
            {
                Properties =
                {
                    { "prop1", hasId }
                }
            };
            root.Not = nestedInId;
        }

        [Test]
        public void WriteTo_InnerSchemaOfExternalResolvedReference()
        {
            JSchema nestedReference = new JSchema()
            {
                Type = JSchemaType.Boolean
            };

            JSchema referenceSchema = new JSchema
            {
                Id = new Uri("http://localhost/test"),
                Items =
                {
                    nestedReference
                }
            };

            JSchema root = new JSchema
            {
                Id = new Uri("#root", UriKind.RelativeOrAbsolute),
                Not = nestedReference
            };

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            root.WriteTo(jsonWriter, new JSchemaWriterSettings
            {
                ExternalSchemas =
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
            JSchema schema = JSchema.Parse(@"{
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
            JSchema schema = new JSchema
            {
                Properties =
                {
                    { "prop1", new JSchema() }
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
            JSchema schema = new JSchema();
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
            JSchema schema = new JSchema
            {
                PatternProperties =
                {
                    { "[abc]", new JSchema() }
                }
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
            JSchema schema = JSchema.Parse(@"{
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
            JSchema schema = new JSchema();
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
            JSchema schema = new JSchema();
            schema.ItemsPositionValidation = true;
            schema.Items.Add(new JSchema { Type = JSchemaType.String });

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
            JSchema schema = new JSchema();
            schema.Items.Add(new JSchema { Type = JSchemaType.String });

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
            JSchema referenceSchema = new JSchema
            {
                Id = new Uri("http://localhost/test")
            };

            JSchema root = new JSchema
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

            root.WriteTo(jsonWriter, new JSchemaWriterSettings
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
            JSchema schema = new JSchema();
            schema.Id = new Uri("root", UriKind.RelativeOrAbsolute);
            schema.Type = JSchemaType.Boolean;

            JSchema file = new JSchema();
            file.Id = new Uri("file", UriKind.RelativeOrAbsolute);
            file.Properties.Add("blah", schema);
            file.ExtensionData["definitions"] = new JObject
            {
                { "parent", schema }
            };

            schema.Properties.Add("storage", file);
            schema.ExtensionData["definitions"] = new JObject
            {
                { "file", file },
                { "file2", file }
            };
            schema.Items.Add(new JSchema
            {
                Type = JSchemaType.Integer | JSchemaType.Null
            });
            schema.Items.Add(file);
            schema.ItemsPositionValidation = true;
            schema.Not = file;
            schema.AllOf.Add(file);
            schema.AllOf.Add(new JSchema
            {
                Type = JSchemaType.Integer | JSchemaType.Null
            });
            schema.AllOf.Add(file);
            schema.AnyOf.Add(file);
            schema.AnyOf.Add(new JSchema
            {
                Type = JSchemaType.Integer | JSchemaType.Null
            });
            schema.AnyOf.Add(schema);
            schema.OneOf.Add(file);
            schema.OneOf.Add(new JSchema
            {
                Type = JSchemaType.Integer | JSchemaType.Null
            });
            schema.OneOf.Add(schema);
            schema.Not = file;

            JSchema file2 = (JSchema) schema.ExtensionData["definitions"]["file"];

            Assert.AreEqual(file, file2);

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
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

        [Test]
        public void WriteTo_Format()
        {
            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            JSchema schema = new JSchema
            {
                Format = "a-format"
            };

            schema.WriteTo(jsonWriter);

            string json = writer.ToString();

            StringAssert.AreEqual(@"{
  ""format"": ""a-format""
}", json);
        }

        [Test]
        public void WriteTo_CircularReference_ReferenceHandling_Never()
        {
            string json = @"{
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""#""}
}";

            JSchema schema = JSchema.Parse(json);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                schema.WriteTo(jsonWriter, new JSchemaWriterSettings
                {
                    ReferenceHandling = JSchemaWriterReferenceHandling.Never
                });
            }, "Error writing schema because writing schema references has been disabled and the schema contains a circular reference.");
        }

        [Test]
        public void WriteTo_CircularReference_ReferenceHandling_Auto()
        {
            string json = @"{
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""#""}
}";

            JSchema schema = JSchema.Parse(json);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter, new JSchemaWriterSettings
            {
                ReferenceHandling = JSchemaWriterReferenceHandling.Auto
            });

            StringAssert.AreEqual(@"{
  ""description"": ""CircularReference"",
  ""type"": ""array"",
  ""items"": {
    ""$ref"": ""#""
  }
}", writer.ToString());
        }

        [Test]
        public void WriteTo_MultipleUsages_ReferenceHandling_Auto()
        {
            JSchema numberSchema = new JSchema
            {
                Type = JSchemaType.Number | JSchemaType.Integer
            };
            JSchema schema = new JSchema();
            schema.Properties["prop1"] = numberSchema;
            schema.Properties["prop2"] = numberSchema;

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            schema.WriteTo(jsonWriter, new JSchemaWriterSettings
            {
                ReferenceHandling = JSchemaWriterReferenceHandling.Auto
            });

            Console.WriteLine(writer.ToString());

            StringAssert.AreEqual(@"{
  ""properties"": {
    ""prop1"": {
      ""type"": [
        ""number"",
        ""integer""
      ]
    },
    ""prop2"": {
      ""type"": [
        ""number"",
        ""integer""
      ]
    }
  }
}", writer.ToString());
        }
    }
}