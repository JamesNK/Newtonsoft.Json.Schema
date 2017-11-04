#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JsonValidatorTests : TestFixtureBase
    {
        [Test]
        public void ValidateComplexType()
        {
            string json = @"{
  ""someDictionary"": [
    {
      ""key"":
      {
        ""field1"": ""value1"",
        ""field2"": ""value2"",
        ""field3"": ""value3"",
      },
      ""value"": {}
    },
    {
      ""key"":
      {
        ""field1"": ""value1a"",
        ""field2"": ""value2a"",
        ""field3"": ""value3a"",
      },
      ""value"": {}
    },
    {
      ""key"":
      {
        ""field1"": ""value1"",
        ""field2"": ""value2"",
        ""field3"": ""value3"",
      },
      ""value"": ""invalid!""
    }   
  ]
}";

            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                Validators = new List<JsonValidator> { new UniqueKeyValidator() }
            };

            JSchema schema = JSchema.Parse(@"{
  ""type"": ""object"",
  ""properties"": {
    ""someDictionary"": {
      ""type"": ""array"",
      ""uniqueDictionaryKey"": true,
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""key"": { ""type"": ""object"" },
          ""value"": { ""type"": ""object"" }
        }
      }
    }
  }
}", settings);

            JObject o = JObject.Parse(json);

            IList<string> errors;
            Assert.IsFalse(o.IsValid(schema, out errors));

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual(@"Invalid type. Expected Object but got String. Path 'someDictionary[2].value', line 28, position 25.", errors[0]);
            Assert.AreEqual(@"Duplicate key: {""field1"":""value1"",""field2"":""value2"",""field3"":""value3""}. Path 'someDictionary', line 2, position 21.", errors[1]);
        }

        public class UniqueKeyValidator : JsonValidator
        {
            public override void Validate(JToken value, JsonValidatorContext context)
            {
                var groupedValues = value.GroupBy(v => v["key"], v => v["value"], JToken.EqualityComparer);

                foreach (var groupedValue in groupedValues)
                {
                    if (groupedValue.Count() > 1)
                    {
                        context.RaiseError($"Duplicate key: {groupedValue.Key.ToString(Formatting.None)}");
                    }
                }
            }

            public override bool CanValidate(JSchema schema)
            {
                return schema.ExtensionData.ContainsKey("uniqueDictionaryKey");
            }
        }

        [Test]
        public void ValidateSimpleType()
        {
            string json = @"{
  ""values"": [
    ""1"",
    ""[1]"",
    ""\""A string!\"""",
    123,
    ""{\""prop1\"":bad!}""
  ]
}";

            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                Validators = new List<JsonValidator> { new JsonFormatValidator() }
            };

            JSchema schema = JSchema.Parse(@"{
  ""type"": ""object"",
  ""properties"": {
    ""values"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""string"",
        ""format"": ""json""
      }
    }
  }
}", settings);

            JObject o = JObject.Parse(json);

            IList<ValidationError> errors;
            Assert.IsFalse(o.IsValid(schema, out errors));

            Assert.AreEqual(2, errors.Count);

            Assert.AreEqual(@"Invalid type. Expected String but got Integer.", errors[0].Message);
            Assert.AreEqual(ErrorType.Type, errors[0].ErrorType);
            Assert.AreEqual("#/properties/values/items", errors[0].SchemaId.OriginalString);

            Assert.AreEqual(@"String is not JSON: Unexpected character encountered while parsing value: b. Path 'prop1', line 1, position 9.", errors[1].Message);
            Assert.AreEqual(ErrorType.Validator, errors[1].ErrorType);
            Assert.AreEqual("#/properties/values/items", errors[1].SchemaId.OriginalString);
        }

        public class JsonFormatValidator : JsonValidator
        {
            public override void Validate(JToken value, JsonValidatorContext context)
            {
                if (value.Type == JTokenType.String)
                {
                    string s = value.ToString();

                    try
                    {
                        JToken.Parse(s);
                    }
                    catch (Exception ex)
                    {
                        context.RaiseError($"String is not JSON: {ex.Message}");
                    }
                }
            }

            public override bool CanValidate(JSchema schema)
            {
                return (schema.Format == "json");
            }
        }

        [Test]
        public void ValidateRefSchemaSimpleType()
        {
            string json = @"{
                              ""values"": [
                                            ""1"",
                                            ""[1]"",
                                            ""\""A string!\"""",
                                            123,
                                            ""{\""prop1\"":bad!}""
                                          ],
                              ""refSchema"": {
                                              ""values"": [
                                                            ""1"",
                                                            ""[1]"",
                                                            ""\""A string!\"""",
                                                            123,
                                                            ""{\""prop1\"":bad!}""
                                                          ]}
                            }";

            string refSchema = @"{
                                  ""type"": ""object"",
                                  ""properties"": {
                                                    ""values"": {
                                                        ""type"": ""array"",
                                                        ""items"": {
                                                                    ""type"": ""string"",
                                                                    ""format"": ""json""
                                                                   }
                                                                }
                                                  }
                                }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("refSchema.json", UriKind.RelativeOrAbsolute), refSchema);


            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                Validators = new List<JsonValidator> { new JsonFormatValidator() },
                Resolver = resolver
            };

            JSchema schema = JSchema.Parse(@"{
                                              ""type"": ""object"",
                                              ""properties"": {
                                                    ""values"": {
                                                                  ""type"": ""array"",
                                                                  ""items"": {
                                                                    ""type"": ""string"",
                                                                    ""format"": ""json""
                                                                  }
                                                                },
                                                    ""refSchema"": { ""$ref"": ""refSchema.json"" }
                                              }
                                            }", settings);

            JObject o = JObject.Parse(json);

            IList<ValidationError> errors;
            Assert.IsFalse(o.IsValid(schema, out errors));

            Assert.AreEqual(4, errors.Count);

            Assert.AreEqual(@"Invalid type. Expected String but got Integer.", errors[0].Message);
            Assert.AreEqual(ErrorType.Type, errors[0].ErrorType);
            Assert.AreEqual("#/properties/values/items", errors[0].SchemaId.OriginalString);

            Assert.AreEqual(@"String is not JSON: Unexpected character encountered while parsing value: b. Path 'prop1', line 1, position 9.", errors[1].Message);
            Assert.AreEqual(ErrorType.Validator, errors[1].ErrorType);
            Assert.AreEqual("#/properties/values/items", errors[1].SchemaId.OriginalString);

            Assert.AreEqual(@"Invalid type. Expected String but got Integer.", errors[2].Message);
            Assert.AreEqual(ErrorType.Type, errors[2].ErrorType);
            Assert.AreEqual("#/properties/refSchema/properties/values/items", errors[2].SchemaId.OriginalString);

            Assert.AreEqual(@"String is not JSON: Unexpected character encountered while parsing value: b. Path 'prop1', line 1, position 9.", errors[3].Message);
            Assert.AreEqual(ErrorType.Validator, errors[3].ErrorType);
            Assert.AreEqual("#/properties/refSchema/properties/values/items", errors[3].SchemaId.OriginalString);
        }

        [Test]
        public void ValidateRefSchemaSimpleType_WithBaseUri()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();

            resolver.Add(new Uri("http://example.com/schema1.json"), Schema1);
            resolver.Add(new Uri("http://example.com/schema2.json"), Schema2);

            JSchema schema = JSchema.Parse(Schema1, new JSchemaReaderSettings
            {
                Resolver = resolver,
                BaseUri = new Uri("http://example.com/schema1.json"),
                Validators = new List<JsonValidator> { new JsonFormatValidator() }
            });

            JSchema fooSchema = schema.Properties["foo"];
            Assert.AreEqual(1, fooSchema.Properties["value"].Validators.Count);

            JSchema bazSchema = (JSchema)schema.ExtensionData["definitions"]["baz"];
            Assert.AreEqual(1, bazSchema.Properties["value"].Validators.Count);
        }

        private const string Schema1 = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""id"": ""http://example.com/schema1.json"",
    ""title"": ""Schema 1"",
    ""description"": ""Demonstrating stack overflow"",
    ""properties"": {
        ""foo"": {
            ""$ref"": ""http://example.com/schema2.json#/definitions/juliet""
        }
    },
    ""definitions"": {
        ""baz"": {
            ""properties"": {
                ""value"": { ""format"": ""json"" }
            }
        }
    }
}";

        private const string Schema2 = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""id"": ""http://example.com/schema2.json"",
    ""title"": ""Schema 2"",
    ""description"": ""Demonstrating stack overflow"",
    ""properties"": {
        ""romeo"": {
            ""$ref"": ""http://example.com/schema1.json#/definitions/baz""
        }
    },
    ""definitions"": {
        ""juliet"": {
            ""properties"": {
                ""value"": { ""format"": ""json"" }
            }
        }
    }
}";
    }
}