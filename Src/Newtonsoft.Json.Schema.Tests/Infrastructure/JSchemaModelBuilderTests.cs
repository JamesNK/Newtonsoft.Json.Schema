#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Infrastructure;
using NUnit.Framework;
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

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaModelBuilderTests : TestFixtureBase
    {
        [Test]
        public void ExtendedComplex()
        {
            string first = @"{
  ""id"":""first"",
  ""type"":""object"",
  ""properties"":
  {
    ""firstproperty"":{""type"":""string""},
    ""secondproperty"":{""type"":""string"",""maxLength"":10},
    ""thirdproperty"":{
      ""type"":""object"",
      ""properties"":
      {
        ""thirdproperty_firstproperty"":{""type"":""string"",""maxLength"":10,""minLength"":7}
      }
    }
  },
  ""additionalProperties"":{}
}";

            string second = @"{
  ""id"":""second"",
  ""type"":""object"",
  ""extends"":{""$ref"":""first""},
  ""properties"":
  {
    ""secondproperty"":{""type"":""any""},
    ""thirdproperty"":{
      ""extends"":{
        ""properties"":
        {
          ""thirdproperty_firstproperty"":{""maxLength"":9,""minLength"":6,""pattern"":""hi2u""}
        },
        ""additionalProperties"":{""maxLength"":9,""minLength"":6,""enum"":[""one"",""two""]}
      },
      ""type"":""object"",
      ""properties"":
      {
        ""thirdproperty_firstproperty"":{""pattern"":""hi""}
      },
      ""additionalProperties"":{""type"":""string"",""enum"":[""two"",""three""]}
    },
    ""fourthproperty"":{""type"":""string""}
  },
  ""additionalProperties"":false
}";

            JSchemaResolver resolver = new JSchemaResolver();
            JSchema firstSchema = JSchema.Parse(first, resolver);
            JSchema secondSchema = JSchema.Parse(second, resolver);

            JSchemaModelBuilder modelBuilder = new JSchemaModelBuilder();

            JSchemaModel model = modelBuilder.Build(secondSchema);

            Assert.AreEqual(4, model.Properties.Count);

            Assert.AreEqual(JSchemaType.String, model.Properties["firstproperty"].Type);

            Assert.AreEqual(JSchemaType.String, model.Properties["secondproperty"].Type);
            Assert.AreEqual(10, model.Properties["secondproperty"].MaximumLength);
            Assert.AreEqual(null, model.Properties["secondproperty"].Enum);
            Assert.AreEqual(null, model.Properties["secondproperty"].Patterns);

            Assert.AreEqual(JSchemaType.Object, model.Properties["thirdproperty"].Type);
            Assert.AreEqual(3, model.Properties["thirdproperty"].AdditionalProperties.Enum.Count);
            Assert.AreEqual("two", (string)model.Properties["thirdproperty"].AdditionalProperties.Enum[0]);
            Assert.AreEqual("three", (string)model.Properties["thirdproperty"].AdditionalProperties.Enum[1]);
            Assert.AreEqual("one", (string)model.Properties["thirdproperty"].AdditionalProperties.Enum[2]);

            Assert.AreEqual(JSchemaType.String, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Type);
            Assert.AreEqual(9, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].MaximumLength);
            Assert.AreEqual(7, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].MinimumLength);
            Assert.AreEqual(2, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Patterns.Count);
            Assert.AreEqual("hi", model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Patterns[0]);
            Assert.AreEqual("hi2u", model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Patterns[1]);
            Assert.AreEqual(null, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Properties);
            Assert.AreEqual(null, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].Items);
            Assert.AreEqual(null, model.Properties["thirdproperty"].Properties["thirdproperty_firstproperty"].AdditionalProperties);
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

            JSchema schema = JSchema.Parse(json);

            JSchemaModelBuilder modelBuilder = new JSchemaModelBuilder();

            JSchemaModel model = modelBuilder.Build(schema);

            Assert.AreEqual(JSchemaType.Array, model.Type);

            Assert.AreEqual(model, model.Items[0]);
        }

        [Test]
        public void Required()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""},
    ""hobbies"":{""type"":""string"",required:true},
    ""age"":{""type"":""integer"",required:true}
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);
            JSchemaModelBuilder modelBuilder = new JSchemaModelBuilder();
            JSchemaModel model = modelBuilder.Build(schema);

            Assert.AreEqual(JSchemaType.Object, model.Type);
            Assert.AreEqual(3, model.Properties.Count);
            Assert.AreEqual(false, model.Properties["name"].Required);
            Assert.AreEqual(true, model.Properties["hobbies"].Required);
            Assert.AreEqual(true, model.Properties["age"].Required);
        }
    }
}