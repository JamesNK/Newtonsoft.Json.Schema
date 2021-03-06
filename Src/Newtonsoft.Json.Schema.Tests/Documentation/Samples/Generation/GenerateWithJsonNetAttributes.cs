#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

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