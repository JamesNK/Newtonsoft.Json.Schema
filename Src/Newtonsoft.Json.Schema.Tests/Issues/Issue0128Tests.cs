#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.IO;
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
    public class Issue0128Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(@"{
  ""id"": ""http://site.somewhere/entry-schema#"",
  ""$schema"": ""http://json-schema.org/draft-06/schema#"",
  ""description"": ""Json Schema for DSP Data Catalog Data Set"",
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""required"": [
    ""IsProcessed""
  ],

  ""properties"": {
   ""IsProcessed"": { ""type"": ""boolean"" }
  }
}");

            string json = @"{
     ""IsProcessed"" : 5
}";

            Result r = Deserialize<Result>(new JsonTextReader(new StringReader(json)), s, out List<string> errors);

            Assert.IsTrue(r.IsProcessed);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Invalid type. Expected Boolean but got Integer. Path 'IsProcessed', line 2, position 22.", errors[0]);
        }

        public class Result
        {
            public bool IsProcessed { get; set; }
        }

        private static TObject Deserialize<TObject>(JsonReader jsonReader, JSchema schema, out List<string> errors)
        {
            using (JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(jsonReader))
            {
                List<string> validationErrors = new List<string>();

                validatingReader.Schema = schema;
                validatingReader.ValidationEventHandler += (o, error) => validationErrors.Add(error.Message);

                JsonSerializer serializer = new JsonSerializer();

                TObject entity = serializer.Deserialize<TObject>(validatingReader);

                errors = validationErrors;
                return entity;
            }
        }
    }
}