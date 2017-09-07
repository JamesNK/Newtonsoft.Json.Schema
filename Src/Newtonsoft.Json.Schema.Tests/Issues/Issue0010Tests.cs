#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
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