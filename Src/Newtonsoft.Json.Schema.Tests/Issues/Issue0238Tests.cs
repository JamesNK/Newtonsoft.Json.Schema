#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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
    public class Issue0238Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            var schemaUrlResolver = new JSchemaUrlResolver();
            var schemaReaderSettings = new JSchemaReaderSettings
            {
                Resolver = schemaUrlResolver,
                Validators = new List<JsonValidator> { new CustomJsonValidator() { } }
            };
            JSchema schema = JSchema.Parse(@"{
                              ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                              ""$id"": ""schema.json"",
                              ""title"": ""schema"",
                              ""description"": ""Schema of Object"",
                              ""type"": ""object"",
                              ""properties"": {
                                ""id"": {
                                  ""type"": ""integer""
                                }
                              },
                              ""additionalProperties"": false,
                              ""required"": [
                                ""id""
                              ]
                            }", schemaReaderSettings);

            var model = new Model<string>();
            JObject obj = new JObject();
            obj["id"] = model.Value;

            var result = obj.IsValid(schema, out IList<ValidationError> errors);
            Assert.IsFalse(result);
            Assert.AreEqual("Invalid type. Expected Integer but got Null.", errors[0].Message);
        }

        public class CustomJsonValidator : JsonValidator
        {
            public override bool CanValidate(JSchema schema)
            {
                return true;
            }

            public override void Validate(JToken value, JsonValidatorContext context)
            {
                // do anything
            }
        }

        public class Model<T>
        {
            #region Public

            /// <summary>   Gets or sets the validation result. </summary>
            /// <value> The validation result. </value>
            public T Value { get; set; }

            #endregion
        }
    }
}
