#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0075Tests : TestFixtureBase
    {
        private readonly string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""description"": ""schema for value referenced by multiple pointers"",
  ""type"": ""object"",
  ""required"": [
    ""parent""
  ],
  ""properties"": {
    ""parent"": {
      ""$ref"": ""#/def""
    }
  },
  ""def"": {
    ""type"": ""object"",
    ""properties"": {
      ""multipointer"": {
        ""type"": ""array"",
        ""$ref"": ""#/pointers/1""
      }
    },
    ""required"": [
      ""multipointer""
    ]
  },
  ""pointers"": [
    {
      ""1"": {
        ""pointer1"": {
          ""$ref"": ""#/numberValues""
        },
        ""pointer2"": {
          ""$ref"": ""#/numberValues""
        },
        ""pointer3"": {
          ""$ref"": ""#/numberValues""
        },
        ""pointer4"": {
          ""$ref"": ""#/numberValues""
        }
      }
    }
  ],
  ""numberValues"": {
    ""type"": ""number"",
    ""description"": ""float""
  }
}";

        [Test]
        public void Test()
        {
            ExceptionAssert.Throws<JSchemaReaderException>(
                () => JSchema.Parse(schemaJson),
                "Could not resolve schema reference '#/pointers/1'. Path 'def.properties.multipointer', line 16, position 23.");
        }
    }
}