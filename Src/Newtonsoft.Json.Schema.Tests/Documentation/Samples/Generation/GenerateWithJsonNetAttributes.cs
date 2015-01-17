using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Generation
{
    [TestFixture]
    public class GenerateWithJsonNetAttributes : TestFixtureBase
    {
        #region Types
        public class User
        {
            // always require a string value
            [JsonProperty("name", Required = Required.Always)]
            public string Name { get; set; }

            // don't require any value
            [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
            public string Role { get; set; }

            // property is ignored
            [JsonIgnore]
            public string Password { get; set; }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(User));
            // {
            //   "type": "object",
            //   "properties": {
            //     "name": {
            //       "type": "string"
            //     },
            //     "role": {
            //       "type": [
            //         "string",
            //         "null"
            //       ]
            //     }
            //   },
            //   "required": [
            //     "name"
            //   ]
            // }
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
        }
    }
}