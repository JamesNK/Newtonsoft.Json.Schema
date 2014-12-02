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
    public class JSchemaNodeTests : TestFixtureBase
    {
        [Test]
        public void AddSchema()
        {
            string first = @"{
  ""id"":""first"",
  ""type"":""object"",
  ""properties"":
  {
    ""firstproperty"":{""type"":""string"",""maxLength"":10},
    ""secondproperty"":{
      ""type"":""object"",
      ""properties"":
      {
        ""secondproperty_firstproperty"":{""type"":""string"",""maxLength"":10,""minLength"":7}
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
    ""firstproperty"":{""type"":""string""},
    ""secondproperty"":{
      ""extends"":{
        ""properties"":
        {
          ""secondproperty_firstproperty"":{""maxLength"":9,""minLength"":6}
        }
      },
      ""type"":""object"",
      ""properties"":
      {
        ""secondproperty_firstproperty"":{}
      }
    },
    ""thirdproperty"":{""type"":""string""}
  },
  ""additionalProperties"":false
}";

            JSchemaResolver resolver = new JSchemaResolver();
            JSchema firstSchema = JSchema.Parse(first, resolver);
            JSchema secondSchema = JSchema.Parse(second, resolver);

            JSchemaModelBuilder modelBuilder = new JSchemaModelBuilder();

            JSchemaNode node = modelBuilder.AddSchema(null, secondSchema);

            Assert.AreEqual(2, node.Schemas.Count);
            Assert.AreEqual(2, node.Properties["firstproperty"].Schemas.Count);
            Assert.AreEqual(3, node.Properties["secondproperty"].Schemas.Count);
            Assert.AreEqual(3, node.Properties["secondproperty"].Properties["secondproperty_firstproperty"].Schemas.Count);
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

            JSchemaNode node = modelBuilder.AddSchema(null, schema);

            Assert.AreEqual(1, node.Schemas.Count);

            Assert.AreEqual(node, node.Items[0]);
        }
    }
}