using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue10Tests : TestFixtureBase
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
                "Could not resolve schema reference '#definitions/a'. Path 'definitions.b.properties.b2', line 15, position 17.");
        }
    }
}