#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue03Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string json = @"{
   ""location"" : {
      ""country_code"" : ""GB""
   }
}";

            JSchema schema = JSchema.Parse(Schema);
            JObject o = JObject.Parse(json);

            IList<string> messages;
            bool valid = o.IsValid(schema, out messages);

            Assert.IsTrue(valid);
        }

        private const string Schema = @"{
   ""$schema"" : ""http://json-schema.org/draft-04/schema#"",
   ""title"" : ""branch/update"",
   ""type"" : ""object"",
   ""id"" : ""http://realtime-listings.webservices.xxx.co.uk/docs/v0.1/schemas/branch/update.json"",
   ""oneOf"" : [
      { ""$ref"" : ""#/constraints/gb"" },
      { ""$ref"" : ""#/constraints/overseas"" }
   ],
   ""constraints"" : {
      ""gb"" : {
      },
      ""overseas"" : {
         ""properties"" : {
            ""location"" : {
               ""properties"" : {
                  ""country_code"" : {
                     ""not"" : {
                        ""pattern"" : ""^GB""
                     }
                  }
               }
            }
         }
      }
   }
}";    
    }
}