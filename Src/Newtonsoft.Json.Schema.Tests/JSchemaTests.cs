#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaTests : TestFixtureBase
    {
        [Test]
        public void Extends()
        {
            string json;
            JSchemaResolver resolver = new JSchemaResolver();

            json = @"{
  ""id"":""first"",
  ""type"":""object"",
  ""additionalProperties"":{}
}";

            JSchema first = JSchema.Parse(json, resolver);

            json =
                @"{
  ""id"":""second"",
  ""type"":""object"",
  ""extends"":{""$ref"":""first""},
  ""additionalProperties"":{""type"":""string""}
}";

            JSchema second = JSchema.Parse(json, resolver);
            Assert.AreEqual(first, second.Extends[0]);

            json =
                @"{
  ""id"":""third"",
  ""type"":""object"",
  ""extends"":{""$ref"":""second""},
  ""additionalProperties"":false
}";

            JSchema third = JSchema.Parse(json, resolver);
            Assert.AreEqual(second, third.Extends[0]);
            Assert.AreEqual(first, third.Extends[0].Extends[0]);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            third.WriteTo(jsonWriter, resolver);

            string writtenJson = writer.ToString();
            StringAssert.AreEqual(@"{
  ""id"": ""third"",
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""extends"": {
    ""$ref"": ""second""
  }
}", writtenJson);

            StringWriter writer1 = new StringWriter();
            JsonTextWriter jsonWriter1 = new JsonTextWriter(writer1);
            jsonWriter1.Formatting = Formatting.Indented;

            third.WriteTo(jsonWriter1);

            writtenJson = writer1.ToString();
            StringAssert.AreEqual(@"{
  ""id"": ""third"",
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""extends"": {
    ""id"": ""second"",
    ""type"": ""object"",
    ""additionalProperties"": {
      ""type"": ""string""
    },
    ""extends"": {
      ""id"": ""first"",
      ""type"": ""object"",
      ""additionalProperties"": {}
    }
  }
}", writtenJson);
        }

        [Test]
        public void Extends_Multiple()
        {
            string json = @"{
  ""type"":""object"",
  ""extends"":{""type"":""string""},
  ""additionalProperties"":{""type"":""string""}
}";

            JSchema s = JSchema.Parse(json);

            StringWriter writer = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            string newJson = s.ToString();

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": {
    ""type"": ""string""
  },
  ""extends"": {
    ""type"": ""string""
  }
}", newJson);


            json = @"{
  ""type"":""object"",
  ""extends"":[{""type"":""string""}],
  ""additionalProperties"":{""type"":""string""}
}";

            s = JSchema.Parse(json);

            writer = new StringWriter();
            jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            newJson = s.ToString();

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": {
    ""type"": ""string""
  },
  ""extends"": {
    ""type"": ""string""
  }
}", newJson);


            json = @"{
  ""type"":""object"",
  ""extends"":[{""type"":""string""},{""type"":""object""}],
  ""additionalProperties"":{""type"":""string""}
}";

            s = JSchema.Parse(json);

            writer = new StringWriter();
            jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;

            newJson = s.ToString();

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": {
    ""type"": ""string""
  },
  ""extends"": [
    {
      ""type"": ""string""
    },
    {
      ""type"": ""object""
    }
  ]
}", newJson);
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
  ""disallow"": [
    ""string"",
    ""object"",
    ""array""
  ]
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
  ""disallow"": ""any""
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
            JSchema schema = new JSchema();
            schema.PatternProperties = new Dictionary<string, JSchema>
            {
                { "[abc]", new JSchema() }
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
            schema.PositionalItemsValidation = true;

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
            schema.PositionalItemsValidation = true;
            schema.Items = new List<JSchema> { new JSchema { Type = JSchemaType.String } };

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
            schema.Items = new List<JSchema> { new JSchema { Type = JSchemaType.String } };

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
        public void IntegerValidatesAgainstFloatFlags()
        {
            JSchema schema = JSchema.Parse(@"{
  ""type"": ""object"",
  ""$schema"": ""http://json-schema.org/draft-03/schema"",
  ""required"": false,
  ""properties"": {
  ""NumberProperty"": {
    ""required"": false,
    ""type"": [
        ""number"",
        ""null""
      ]
    }
  }
}");

            JObject json = JObject.Parse(@"{
        ""NumberProperty"": 23
      }");

            Assert.IsTrue(json.IsValid(schema));
        }
    }
}