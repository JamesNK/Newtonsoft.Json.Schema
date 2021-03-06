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
    public class Issue0010Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            ExceptionAssert.Throws<JSchemaReaderException>(
                () =>
                {
                    string json = @"{
  ""type"" : ""object"",
  ""properties"" : {
      ""test"" : {""$ref"" : ""#/definitions/b"" }
  },

  ""definitions"": {

    ""a"" : {
      ""type"":""string""
    },
    ""b"": {
      ""type"": ""object"",
      ""properties"": {
        ""b2"" : { ""$ref"": ""#definitions/a"" }
      }
    }
  }
}";

                    JSchema.Parse(json);
                },
                "Could not resolve schema reference '#definitions/a'. Path 'definitions.b.properties.b2', line 15, position 16.");
        }
    }
}