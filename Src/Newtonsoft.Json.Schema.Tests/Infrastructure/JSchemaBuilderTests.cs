#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
#endif
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaBuilderTests : TestFixtureBase
    {
        [Test]
        public void Simple()
        {
            string json = @"
{
  ""description"": ""A person"",
  ""type"": ""object"",
  ""properties"":
  {
    ""name"": {""type"":""string""},
    ""hobbies"": {
      ""type"": ""array"",
      ""items"": {""type"":""string""}
    }
  }
}
";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("A person", schema.Description);
            Assert.AreEqual(JSchemaType.Object, schema.Type);

            Assert.AreEqual(2, schema.Properties.Count);

            Assert.AreEqual(JSchemaType.String, schema.Properties["name"].Type);
            Assert.AreEqual(JSchemaType.Array, schema.Properties["hobbies"].Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["hobbies"].Items[0].Type);
        }

        [Test]
        public void MultipleTypes()
        {
            string json = @"{
  ""description"":""Age"",
  ""type"":[""string"", ""integer""]
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Age", schema.Description);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Integer, schema.Type);
        }

        [Test]
        public void MultipleItems()
        {
            string json = @"{
  ""description"":""MultipleItems"",
  ""type"":""array"",
  ""items"": [{""type"":""string""},{""type"":""array""}]
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("MultipleItems", schema.Description);
            Assert.AreEqual(JSchemaType.String, schema.Items[0].Type);
            Assert.AreEqual(JSchemaType.Array, schema.Items[1].Type);
        }

        [Test]
        public void AdditionalProperties()
        {
            string json = @"{
  ""description"":""AdditionalProperties"",
  ""type"":[""string"", ""integer""],
  ""additionalProperties"":{""type"":[""object"", ""boolean""]}
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("AdditionalProperties", schema.Description);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Boolean, schema.AdditionalProperties.Type);
        }

        [Test]
        public void Required()
        {
            string json = @"{
  ""description"":""Required"",
  ""required"":true
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Required", schema.Description);
            Assert.AreEqual(true, schema.Required);
        }

        [Test]
        public void ExclusiveMinimum_ExclusiveMaximum()
        {
            string json = @"{
  ""exclusiveMinimum"":true,
  ""exclusiveMaximum"":true
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(true, schema.ExclusiveMinimum);
            Assert.AreEqual(true, schema.ExclusiveMaximum);
        }

        [Test]
        public void ReadOnly()
        {
            string json = @"{
  ""description"":""ReadOnly"",
  ""readonly"":true
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("ReadOnly", schema.Description);
            Assert.AreEqual(true, schema.ReadOnly);
        }

        [Test]
        public void Hidden()
        {
            string json = @"{
  ""description"":""Hidden"",
  ""hidden"":true
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Hidden", schema.Description);
            Assert.AreEqual(true, schema.Hidden);
        }

        [Test]
        public void Id()
        {
            string json = @"{
  ""description"":""Id"",
  ""id"":""testid""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Id", schema.Description);
            Assert.AreEqual("testid", schema.Id);
        }

        [Test]
        public void Title()
        {
            string json = @"{
  ""description"":""Title"",
  ""title"":""testtitle""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Title", schema.Description);
            Assert.AreEqual("testtitle", schema.Title);
        }

        [Test]
        public void Pattern()
        {
            string json = @"{
  ""description"":""Pattern"",
  ""pattern"":""testpattern""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Pattern", schema.Description);
            Assert.AreEqual("testpattern", schema.Pattern);
        }

        [Test]
        public void Format()
        {
            string json = @"{
  ""description"":""Format"",
  ""format"":""testformat""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Format", schema.Description);
            Assert.AreEqual("testformat", schema.Format);
        }

        [Test]
        public void Requires()
        {
            string json = @"{
  ""description"":""Requires"",
  ""requires"":""PurpleMonkeyDishwasher""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Requires", schema.Description);
            Assert.AreEqual("PurpleMonkeyDishwasher", schema.Requires);
        }

        [Test]
        public void MinimumMaximum()
        {
            string json = @"{
  ""description"":""MinimumMaximum"",
  ""minimum"":1.1,
  ""maximum"":1.2,
  ""minItems"":1,
  ""maxItems"":2,
  ""minLength"":5,
  ""maxLength"":50,
  ""divisibleBy"":3,
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("MinimumMaximum", schema.Description);
            Assert.AreEqual(1.1, schema.Minimum);
            Assert.AreEqual(1.2, schema.Maximum);
            Assert.AreEqual(1, schema.MinimumItems);
            Assert.AreEqual(2, schema.MaximumItems);
            Assert.AreEqual(5, schema.MinimumLength);
            Assert.AreEqual(50, schema.MaximumLength);
            Assert.AreEqual(3, schema.DivisibleBy);
        }

        [Test]
        public void DisallowSingleType()
        {
            string json = @"{
  ""description"":""DisallowSingleType"",
  ""disallow"":""string""
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("DisallowSingleType", schema.Description);
            Assert.AreEqual(JSchemaType.String, schema.Disallow);
        }

        [Test]
        public void DisallowMultipleTypes()
        {
            string json = @"{
  ""description"":""DisallowMultipleTypes"",
  ""disallow"":[""string"",""number""]
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("DisallowMultipleTypes", schema.Description);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Float, schema.Disallow);
        }

        [Test]
        public void DefaultPrimitiveType()
        {
            string json = @"{
  ""description"":""DefaultPrimitiveType"",
  ""default"":1.1
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("DefaultPrimitiveType", schema.Description);
            Assert.AreEqual(1.1, (double)schema.Default);
        }

        [Test]
        public void DefaultComplexType()
        {
            string json = @"{
  ""description"":""DefaultComplexType"",
  ""default"":{""pie"":true}
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("DefaultComplexType", schema.Description);
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse(@"{""pie"":true}"), schema.Default));
        }

        [Test]
        public void Enum()
        {
            string json = @"{
  ""description"":""Type"",
  ""type"":[""string"",""array""],
  ""enum"":[""string"",""object"",""array"",""boolean"",""number"",""integer"",""null"",""any""]
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("Type", schema.Description);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Array, schema.Type);

            Assert.AreEqual(8, schema.Enum.Count);
            Assert.AreEqual("string", (string)schema.Enum[0]);
            Assert.AreEqual("any", (string)schema.Enum[schema.Enum.Count - 1]);
        }

        [Test]
        public void CircularReference()
        {
            string json = @"{
  ""id"":""CircularReferenceArray"",
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""CircularReferenceArray""}
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("CircularReference", schema.Description);
            Assert.AreEqual("CircularReferenceArray", schema.Id);
            Assert.AreEqual(JSchemaType.Array, schema.Type);

            Assert.AreEqual(schema, schema.Items[0]);
        }

        [Test]
        public void UnresolvedReference()
        {
            ExceptionAssert.Throws<Exception>(() =>
            {
                string json = @"{
  ""id"":""CircularReferenceArray"",
  ""description"":""CircularReference"",
  ""type"":[""array""],
  ""items"":{""$ref"":""MyUnresolvedReference""}
}";

                JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
                JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));
            }, @"Could not resolve schema reference 'MyUnresolvedReference'.");
        }

        [Test]
        public void PatternProperties()
        {
            string json = @"{
  ""patternProperties"": {
    ""[abc]"": { ""id"":""Blah"" }
  }
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.IsNotNull(schema.PatternProperties);
            Assert.AreEqual(1, schema.PatternProperties.Count);
            Assert.AreEqual("Blah", schema.PatternProperties["[abc]"].Id);
        }

        [Test]
        public void AdditionalItems()
        {
            string json = @"{
    ""items"": [],
    ""additionalItems"": {""type"": ""integer""}
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.IsNotNull(schema.AdditionalItems);
            Assert.AreEqual(JSchemaType.Integer, schema.AdditionalItems.Type);
            Assert.AreEqual(true, schema.AllowAdditionalItems);
        }

        [Test]
        public void DisallowAdditionalItems()
        {
            string json = @"{
    ""items"": [],
    ""additionalItems"": false
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.IsNull(schema.AdditionalItems);
            Assert.AreEqual(false, schema.AllowAdditionalItems);
        }

        [Test]
        public void AllowAdditionalItems()
        {
            string json = @"{
    ""items"": {},
    ""additionalItems"": false
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.IsNull(schema.AdditionalItems);
            Assert.AreEqual(false, schema.AllowAdditionalItems);
        }

        [Test]
        public void Location()
        {
            string json = @"{
  ""properties"":{
    ""foo"":{
      ""type"":""array"",
      ""items"":[
        {
          ""type"":""integer""
        },
        {
          ""properties"":{
            ""foo"":{
              ""type"":""integer""
            }
          }
        }
      ]
    }
  }
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("#", schema.Location);
            Assert.AreEqual("#/properties/foo", schema.Properties["foo"].Location);
            Assert.AreEqual("#/properties/foo/items/1/properties/foo", schema.Properties["foo"].Items[1].Properties["foo"].Location);
        }

        [Test]
        public void Reference_BackwardsLocation()
        {
            string json = @"{
  ""properties"": {
    ""foo"": {""type"": ""integer""},
    ""bar"": {""$ref"": ""#/properties/foo""}
  }
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(schema.Properties["foo"], schema.Properties["bar"]);
        }

        [Test]
        public void Reference_ForwardsLocation()
        {
            string json = @"{
  ""properties"": {
    ""bar"": {""$ref"": ""#/properties/foo""},
    ""foo"": {""type"": ""integer""}
  }
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(schema.Properties["foo"], schema.Properties["bar"]);
        }

        [Test]
        public void Reference_NonStandardLocation()
        {
            string json = @"{
  ""properties"": {
    ""bar"": {""$ref"": ""#/common/foo""},
    ""foo"": {""$ref"": ""#/common/foo""}
  },
  ""common"": {
    ""foo"": {""type"": ""integer""}
  }
}";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(schema.Properties["foo"], schema.Properties["bar"]);
        }

        [Test]
        public void EscapedReferences()
        {
            string json = @"{
            ""tilda~field"": {""type"": ""integer""},
            ""slash/field"": {""type"": ""object""},
            ""percent%field"": {""type"": ""array""},
            ""properties"": {
                ""tilda"": {""$ref"": ""#/tilda~0field""},
                ""slash"": {""$ref"": ""#/slash~1field""},
                ""percent"": {""$ref"": ""#/percent%25field""}
            }
        }";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["tilda"].Type);
            Assert.AreEqual(JSchemaType.Object, schema.Properties["slash"].Type);
            Assert.AreEqual(JSchemaType.Array, schema.Properties["percent"].Type);
        }

        [Test]
        public void References_Array()
        {
            string json = @"{
            ""array"": [{""type"": ""integer""},{""prop"":{""type"": ""object""}}],
            ""properties"": {
                ""array"": {""$ref"": ""#/array/0""},
                ""arrayprop"": {""$ref"": ""#/array/1/prop""}
            }
        }";

            JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
            JSchema schema = builder.Read(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["array"].Type);
            Assert.AreEqual(JSchemaType.Object, schema.Properties["arrayprop"].Type);
        }

        [Test]
        public void References_IndexTooBig()
        {
            // JsonException : Could not resolve schema reference '#/array/10'.

            string json = @"{
            ""array"": [{""type"": ""integer""},{""prop"":{""type"": ""object""}}],
            ""properties"": {
                ""array"": {""$ref"": ""#/array/0""},
                ""arrayprop"": {""$ref"": ""#/array/10""}
            }
        }";

            ExceptionAssert.Throws<JsonException>(() =>
            {
                JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
                builder.Read(new JsonTextReader(new StringReader(json)));
            }, "Could not resolve schema reference '#/array/10'.");
        }

        [Test]
        public void References_IndexNegative()
        {
            string json = @"{
            ""array"": [{""type"": ""integer""},{""prop"":{""type"": ""object""}}],
            ""properties"": {
                ""array"": {""$ref"": ""#/array/0""},
                ""arrayprop"": {""$ref"": ""#/array/-1""}
            }
        }";

            ExceptionAssert.Throws<JsonException>(() =>
            {
                JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
                builder.Read(new JsonTextReader(new StringReader(json)));
            }, "Could not resolve schema reference '#/array/-1'.");
        }

        [Test]
        public void References_IndexNotInteger()
        {
            string json = @"{
            ""array"": [{""type"": ""integer""},{""prop"":{""type"": ""object""}}],
            ""properties"": {
                ""array"": {""$ref"": ""#/array/0""},
                ""arrayprop"": {""$ref"": ""#/array/one""}
            }
        }";

            ExceptionAssert.Throws<JsonException>(() =>
            {
                JSchemaBuilder builder = new JSchemaBuilder(new JSchemaResolver());
                builder.Read(new JsonTextReader(new StringReader(json)));
            }, "Could not resolve schema reference '#/array/one'.");
        }
    }
}