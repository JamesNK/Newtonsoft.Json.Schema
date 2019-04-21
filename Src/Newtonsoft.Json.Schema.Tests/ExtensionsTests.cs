﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using Newtonsoft.Json.Schema.Tests.TestObjects;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json.Schema.Generation;
#if !(NETFX_CORE || DNXCORE50)
using System.Data;

#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class ExtensionsTests : TestFixtureBase
    {
#if !(PORTABLE || NET35)
        [Test]
        public void BigNum()
        {
            ValidationError error = null;

            JSchema s = JSchema.Parse(@"{""maximum"": 18446744073709551615}");

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader("18446744073709551600")));
            reader.Schema = s;
            reader.ValidationEventHandler += (sender, args) => { error = args.ValidationError; };

            Assert.IsTrue(reader.Read());
            Assert.IsNull(error);
        }
#endif

        [Test]
        public void ValidationErrorPath()
        {
            string schemaJson = TestHelpers.OpenFileText(@"resources\schemas\schema-draft-v4.json");
            JSchema s = JSchema.Parse(schemaJson);

            JObject o = JObject.Parse(@"{ ""additionalItems"": 5 }");

            IList<ValidationError> validationErrors;
            o.IsValid(s, out validationErrors);

            Assert.AreEqual(1, validationErrors.Count);
            Assert.AreEqual("#/properties/additionalItems", validationErrors[0].SchemaId.ToString());
            Assert.AreEqual(2, validationErrors[0].ChildErrors.Count);

            Assert.AreEqual("#", validationErrors[0].ChildErrors[0].SchemaId.ToString());
            Assert.AreEqual("#/properties/additionalItems/anyOf/0", validationErrors[0].ChildErrors[1].SchemaId.ToString());
        }

        [Test]
        public void IsValid()
        {
            JSchema schema = JSchema.Parse("{'type':'integer'}");
            JToken stringToken = JToken.FromObject("pie");
            JToken integerToken = JToken.FromObject(1);

            IList<string> errorMessages;
            Assert.AreEqual(true, integerToken.IsValid(schema));
            Assert.AreEqual(true, integerToken.IsValid(schema, out errorMessages));
            Assert.AreEqual(0, errorMessages.Count);

            Assert.AreEqual(false, stringToken.IsValid(schema));
            Assert.AreEqual(false, stringToken.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            Assert.AreEqual("Invalid type. Expected Integer but got String. Path ''.", errorMessages[0]);
        }

        [Test]
        public void ValidateWithEventHandler()
        {
            JSchema schema = JSchema.Parse("{'pattern':'lol'}");
            JToken stringToken = JToken.FromObject("pie lol");

            List<string> errors = new List<string>();
            stringToken.Validate(schema, (sender, args) => errors.Add(args.Message));
            Assert.AreEqual(0, errors.Count);

            stringToken = JToken.FromObject("pie");

            stringToken.Validate(schema, (sender, args) => errors.Add(args.Message));
            Assert.AreEqual(1, errors.Count);

            Assert.AreEqual("String 'pie' does not match regex pattern 'lol'. Path ''.", errors[0]);
        }

        [Test]
        public void ValidateWithOutEventHandlerFailure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = JSchema.Parse("{'pattern':'lol'}");
                JToken stringToken = JToken.FromObject("pie");
                stringToken.Validate(schema);
            }, @"String 'pie' does not match regex pattern 'lol'. Path ''.");
        }

        [Test]
        public void ValidateWithOutEventHandlerSuccess()
        {
            JSchema schema = JSchema.Parse("{'pattern':'lol'}");
            JToken stringToken = JToken.FromObject("pie lol");
            stringToken.Validate(schema);
        }

        [Test]
        public void ValidateFailureWithOutLineInfoBecauseOfEndToken()
        {
            // changed in 6.0.6 to now include line info!
            JSchema schema = JSchema.Parse("{'properties':{'lol':{'required':true}}}");
            JObject o = JObject.Parse("{}");

            List<string> errors = new List<string>();
            o.Validate(schema, (sender, args) => errors.Add(args.Message));

            Assert.AreEqual("Required properties are missing from object: lol. Path '', line 1, position 1.", errors[0]);
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void ValidateRequiredFieldsWithLineInfo()
        {
            JSchema schema = JSchema.Parse("{'properties':{'lol':{'type':'string'}}}");
            JObject o = JObject.Parse("{'lol':1}");

            List<string> errors = new List<string>();
            o.Validate(schema, (sender, args) => errors.Add(args.Path + " - " + args.Message));

            Assert.AreEqual("lol - Invalid type. Expected String but got Integer. Path 'lol', line 1, position 8.", errors[0]);
            Assert.AreEqual("1", o.SelectToken("lol").ToString());
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void Blog()
        {
            string schemaJson = @"
{
  ""description"": ""A person schema"",
  ""type"": ""object"",
  ""properties"":
  {
    ""name"": {""type"":""string""},
    ""hobbies"": {
      ""type"": ""array"",
      ""items"": {""type"":""string""}
    }
  }
}
";

            JSchema schema = JSchema.Parse(schemaJson);

            JObject person = JObject.Parse(@"{
        ""name"": ""James"",
        ""hobbies"": ["".NET"", ""Blogging"", ""Reading"", ""Xbox"", ""LOLCATS""]
      }");

            bool valid = person.IsValid(schema);
            // true
        }

        private void GenerateSchemaAndSerializeFromType<T>(T value)
        {
            LicenseHelpers.ResetCounts(null);

            JSchemaGenerator generator = new JSchemaGenerator();
            generator.SchemaIdGenerationHandling = SchemaIdGenerationHandling.AssemblyQualifiedName;
            JSchema typeSchema = generator.Generate(typeof(T));
            string schema = typeSchema.ToString();

            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            JToken token = JToken.ReadFrom(new JsonTextReader(new StringReader(json)));

            List<string> errors = new List<string>();

            token.Validate(typeSchema, (sender, args) => { errors.Add(args.Message); });

            if (errors.Count > 0)
            {
                Assert.Fail("Schema generated for type '{0}' is not valid." + Environment.NewLine + string.Join(Environment.NewLine, errors.ToArray()), typeof(T));
            }
        }

        [Test]
        public void GenerateSchemaAndSerializeFromTypeTests()
        {
            GenerateSchemaAndSerializeFromType(new List<string> { "1", "Two", "III" });
            GenerateSchemaAndSerializeFromType(new List<int> { 1 });
            GenerateSchemaAndSerializeFromType(new Version("1.2.3.4"));
            GenerateSchemaAndSerializeFromType(new Store());
            GenerateSchemaAndSerializeFromType(new Person());
            GenerateSchemaAndSerializeFromType(new PersonRaw());
            GenerateSchemaAndSerializeFromType(new CircularReferenceClass() { Name = "I'm required" });
            GenerateSchemaAndSerializeFromType(new CircularReferenceWithIdClass());
            GenerateSchemaAndSerializeFromType(new ClassWithArray());
            GenerateSchemaAndSerializeFromType(new ClassWithGuid());
#if !NET20
            GenerateSchemaAndSerializeFromType(new NullableDateTimeTestClass());
#endif
#if !(NETFX_CORE || PORTABLE || DNXCORE50 || PORTABLE40)
            GenerateSchemaAndSerializeFromType(new DataSet());
#endif
            GenerateSchemaAndSerializeFromType(new object());
            GenerateSchemaAndSerializeFromType(1);
            GenerateSchemaAndSerializeFromType("Hi");
            GenerateSchemaAndSerializeFromType(new DateTime(2000, 12, 29, 23, 59, 0, DateTimeKind.Utc));
            GenerateSchemaAndSerializeFromType(TimeSpan.FromTicks(1000000));
#if !(NETFX_CORE || PORTABLE || DNXCORE50 || PORTABLE40)
            GenerateSchemaAndSerializeFromType(DBNull.Value);
#endif
            GenerateSchemaAndSerializeFromType(new JsonPropertyWithHandlingValues());
        }

        [Test]
        public void UndefinedPropertyOnNoPropertySchema()
        {
            JSchema schema = JSchema.Parse(@"{
  ""description"": ""test"",
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
  }
}");

            JObject o = JObject.Parse("{'g':1}");

            List<string> errors = new List<string>();
            o.Validate(schema, (sender, args) => errors.Add(args.Message));

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Property 'g' has not been defined and the schema does not allow additional properties. Path 'g', line 1, position 5.", errors[0]);
        }

        [Test]
        public void ExclusiveMaximum_Int()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.Maximum = 10;
                schema.ExclusiveMaximum = true;

                JValue v = new JValue(10);
                v.Validate(schema);
            }, "Integer 10 equals maximum value of 10 and exclusive maximum is true. Path ''.");
        }

        [Test]
        public void ExclusiveMaximum_Float()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.Maximum = 10.1;
                schema.ExclusiveMaximum = true;

                JValue v = new JValue(10.1);
                v.Validate(schema);
            }, "Float 10.1 equals maximum value of 10.1 and exclusive maximum is true. Path ''.");
        }

        [Test]
        public void ExclusiveMinimum_Int()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.Minimum = 10;
                schema.ExclusiveMinimum = true;

                JValue v = new JValue(10);
                v.Validate(schema);
            }, "Integer 10 equals minimum value of 10 and exclusive minimum is true. Path ''.");
        }

        [Test]
        public void ExclusiveMinimum_Float()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.Minimum = 10.1;
                schema.ExclusiveMinimum = true;

                JValue v = new JValue(10.1);
                v.Validate(schema);
            }, "Float 10.1 equals minimum value of 10.1 and exclusive minimum is true. Path ''.");
        }

        [Test]
        public void DivisibleBy_Int()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.MultipleOf = 3;

                JValue v = new JValue(10);
                v.Validate(schema);
            }, "Integer 10 is not a multiple of 3. Path ''.");
        }

        [Test]
        public void DivisibleBy_Approx()
        {
            JSchema schema = new JSchema();
            schema.MultipleOf = 0.01;

            JValue v = new JValue(20.49);
            v.Validate(schema);
        }

        [Test]
        public void UniqueItems_SimpleUnique()
        {
            JSchema schema = new JSchema();
            schema.UniqueItems = true;

            JArray a = new JArray(1, 2, 3);
            Assert.IsTrue(a.IsValid(schema));
        }

        [Test]
        public void UniqueItems_SimpleDuplicate()
        {
            JSchema schema = new JSchema();
            schema.UniqueItems = true;

            JArray a = new JArray(1, 2, 3, 2, 2);
            IList<string> errorMessages;
            Assert.IsFalse(a.IsValid(schema, out errorMessages));
            Assert.AreEqual(2, errorMessages.Count);
            Assert.AreEqual("Non-unique array item at index 3. Path '[3]'.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 4. Path '[4]'.", errorMessages[1]);
        }

        [Test]
        public void UniqueItems_ComplexDuplicate()
        {
            JSchema schema = new JSchema();
            schema.UniqueItems = true;

            JArray a = new JArray(1, new JObject(new JProperty("value", "value!")), 3, 2, new JObject(new JProperty("value", "value!")), 4, 2, new JObject(new JProperty("value", "value!")));
            IList<string> errorMessages;
            Assert.IsFalse(a.IsValid(schema, out errorMessages));
            Assert.AreEqual(3, errorMessages.Count);
            Assert.AreEqual("Non-unique array item at index 4. Path '[4]'.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 6. Path '[6]'.", errorMessages[1]);
            Assert.AreEqual("Non-unique array item at index 7. Path '[7]'.", errorMessages[2]);
        }

        [Test]
        public void UniqueItems_NestedDuplicate()
        {
            JSchema schema = new JSchema
            {
                UniqueItems = true,
                Items =
                {
                    new JSchema
                    {
                        UniqueItems = true
                    }
                }
            };
            schema.ItemsPositionValidation = false;

            JArray a = new JArray(
                new JArray(1, 2),
                new JArray(1, 1),
                new JArray(3, 4),
                new JArray(1, 2),
                new JArray(1, 1)
                );
            IList<string> errorMessages;
            Assert.IsFalse(a.IsValid(schema, out errorMessages));
            Assert.AreEqual(4, errorMessages.Count);
            Assert.AreEqual("Non-unique array item at index 1. Path '[1][1]'.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 3. Path '[3]'.", errorMessages[1]);
            Assert.AreEqual("Non-unique array item at index 1. Path '[4][1]'.", errorMessages[2]);
            Assert.AreEqual("Non-unique array item at index 4. Path '[4]'.", errorMessages[3]);
        }

        [Test]
        public void Enum_Properties()
        {
            JSchema schema = new JSchema
            {
                Properties =
                {
                    {
                        "bar",
                        new JSchema
                        {
                            Enum =
                            {
                                new JValue(1),
                                new JValue(2)
                            }
                        }
                    }
                }
            };

            JObject o = new JObject(
                new JProperty("bar", 1)
                );
            IList<string> errorMessages;
            Assert.IsTrue(o.IsValid(schema, out errorMessages));
            Assert.AreEqual(0, errorMessages.Count);

            o = new JObject(
                new JProperty("bar", 3)
                );
            Assert.IsFalse(o.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
        }

        [Test]
        public void UniqueItems_Property()
        {
            JSchema schema = new JSchema
            {
                Properties =
                {
                    {
                        "bar",
                        new JSchema
                        {
                            UniqueItems = true
                        }
                    }
                }
            };

            JObject o = new JObject(
                new JProperty("bar", new JArray(1, 2, 3, 3))
                );
            IList<string> errorMessages;
            Assert.IsFalse(o.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
        }

        [Test]
        public void Items_Positional()
        {
            JSchema schema = new JSchema
            {
                Items =
                {
                    new JSchema { Type = JSchemaType.Object },
                    new JSchema { Type = JSchemaType.Integer }
                }
            };
            schema.ItemsPositionValidation = true;

            JArray a = new JArray(new JObject(), 1);
            IList<string> errorMessages;
            Assert.IsTrue(a.IsValid(schema, out errorMessages));
            Assert.AreEqual(0, errorMessages.Count);
        }

        [Test]
        public void IntegerValidatesAgainstFloatFlags()
        {
            JSchema schema = JSchema.Parse(@"{
  ""type"": ""object"",
  ""$schema"": ""http://json-schema.org/draft-03/schema"",
  ""required"": false,
  ""properties"": {
  ""NumberProperty"": {
    ""required"": false,
    ""type"": [
        ""number"",
        ""null""
      ]
    }
  }
}");

            JObject json = JObject.Parse(@"{
  ""NumberProperty"": 23
}");

            Assert.IsTrue(json.IsValid(schema));
        }

        [Test]
        public void ComplexEnum()
        {
            JSchema schema = JSchema.Parse(@"{""enum"": [6, ""foo"", [], true, {""foo"": 12}]}");

            JObject json = JObject.Parse(@"{""foo"": false}");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Value {""foo"":false} is not defined in enum. Path '', line 1, position 1.", errorMessages[0]);
        }

        [Test]
        public void AdditionalItemsSchema()
        {
            JSchema schema = JSchema.Parse(@"{
                ""items"": [],
                ""additionalItems"": {""type"": ""integer""}
            }");

            JArray json = JArray.Parse(@"[ 1, 2, 3, ""foo"" ]");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Invalid type. Expected Integer but got String. Path '[3]', line 1, position 16.", errorMessages[0]);
        }

        [Test]
        public void AdditionalPropertiesSchema()
        {
            JSchema schema = JSchema.Parse(@"{
                ""properties"": {""foo"": {}, ""bar"": {}},
                ""additionalProperties"": {""type"": ""boolean""}
            }");

            JObject json = JObject.Parse(@"{""foo"" : 1, ""bar"" : 2, ""quux"" : 12}");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Invalid type. Expected Boolean but got Integer. Path 'quux', line 1, position 34.", errorMessages[0]);
        }

        [Test]
        public void MultipleDisallowSubschema_Pass()
        {
            JSchema schema = JSchema.Parse(@"{
                ""disallow"":
                    [""string"",
                     {
                        ""type"": ""object"",
                        ""properties"": {
                            ""foo"": {
                                ""type"": ""string""
                            }
                        }
                     }]
            }");

            StringAssert.AreEqual(@"{
  ""not"": {
    ""anyOf"": [
      {
        ""type"": ""string""
      },
      {
        ""type"": ""object"",
        ""properties"": {
          ""foo"": {
            ""type"": ""string""
          }
        }
      }
    ]
  }
}", schema.ToString());

            JToken json = JToken.Parse(@"true");

            IList<string> errorMessages;
            Assert.IsTrue(json.IsValid(schema, out errorMessages));
        }

        [Test]
        public void MultipleDisallowSubschema_Fail()
        {
            JSchema schema = JSchema.Parse(@"{
                ""disallow"":
                    [""string"",
                     {
                        ""type"": ""object"",
                        ""properties"": {
                            ""foo"": {
                                ""type"": ""string""
                            }
                        }
                     }]
            }");

            JObject json = JObject.Parse(@"{""foo"": ""bar""}");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual("JSON is valid against schema from 'not'. Path '', line 1, position 1.", errorMessages[0]);
        }

        [Test]
        public void OneOf_MultipleValid()
        {
            JSchema schema = JSchema.Parse(@"{
                ""type"": ""string"",
                ""oneOf"" : [
                    {
                        ""minLength"": 2
                    },
                    {
                        ""maxLength"": 4
                    }
                ]
            }");

            JToken json = JToken.Parse(@"""foo""");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);

            StringAssert.AreEqual("JSON is valid against more than one schema from 'oneOf'. Valid schema indexes: 0, 1. Path '', line 1, position 5.", errorMessages[0]);
        }

        [Test]
        public void OneOf_NoneValid()
        {
            JSchema schema = JSchema.Parse(@"{
                ""type"": ""string"",
                ""oneOf"" : [
                    {
                        ""type"": ""object""
                    },
                    {
                        ""maxLength"": 4
                    }
                ]
            }");

            JToken json = JToken.Parse(@"""foo foo""");

            IList<ValidationError> errors;
            Assert.IsFalse(json.IsValid(schema, out errors));
            Assert.AreEqual(1, errors.Count);

            ValidationError error = errors.Single();
            StringAssert.AreEqual("JSON is valid against no schemas from 'oneOf'. Path '', line 1, position 9.", error.GetExtendedMessage());
            Assert.AreEqual(2, error.ChildErrors.Count);
            StringAssert.AreEqual(@"String 'foo foo' exceeds maximum length of 4. Path '', line 1, position 9.", error.ChildErrors[0].GetExtendedMessage());
            StringAssert.AreEqual(@"Invalid type. Expected Object but got String. Path '', line 1, position 9.", error.ChildErrors[1].GetExtendedMessage());
        }

        [Test]
        public void SupCodePoints()
        {
            JSchema schema = JSchema.Parse(@"{""maxLength"": 2}");

            JToken json = JToken.Parse(@"""\uD83D\uDCA9\uD83D\uDCA9""");

            IList<string> errorMessages;
            Assert.IsTrue(json.IsValid(schema, out errorMessages));
        }

        [Test]
        public void ResolutionScope()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();

            JSchema subSchema = JSchema.Parse(@"{
                ""type"": ""integer""
            }");

            resolver.Add(new Uri("http://localhost:1234/folder/folderInteger.json"), subSchema.ToString());

            JSchema schema = JSchema.Parse(@"{
                ""id"": ""http://localhost:1234/"",
                ""items"": {
                    ""id"": ""folder/"",
                    ""items"": {""$ref"": ""folderInteger.json""}
                }
            }", resolver);

            JToken json = JToken.Parse(@"[[""a""]]");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
        }

        [Test]
        public void MissingDependency_Single()
        {
            JSchema schema = JSchema.Parse(@"{
                ""dependencies"": {""bar"": ""foo""}
            }");

            JToken json = JToken.Parse(@"{""bar"": 2}");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Dependencies for property 'bar' failed. Missing required keys: foo. Path '', line 1, position 1.", errorMessages[0]);
        }

        [Test]
        public void MissingDependency_Multiple()
        {
            JSchema schema = JSchema.Parse(@"{
                ""dependencies"": {""quux"": [""foo"", ""bar""]}
            }");

            JToken json = JToken.Parse(@"{""foo"": 1, ""quux"": 2}");

            IList<string> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Dependencies for property 'quux' failed. Missing required keys: bar. Path '', line 1, position 1.", errorMessages[0]);
        }

        [Test]
        public void MissingDependency_Schema()
        {
            JSchema schema = JSchema.Parse(@"{
                ""dependencies"": {
                    ""bar"": {
                        ""properties"": {
                            ""foo"": {""type"": ""integer""},
                            ""bar"": {""type"": ""integer""}
                        }
                    }
                }
            }");

            JToken json = JToken.Parse(@"{""foo"": ""quux"", ""bar"": 2}");

            IList<ValidationError> errorMessages;
            Assert.IsFalse(json.IsValid(schema, out errorMessages));
            Assert.AreEqual(1, errorMessages.Count);
            StringAssert.AreEqual(@"Dependencies for property 'bar' failed. Path '', line 1, position 1.", errorMessages[0].GetExtendedMessage());
            Assert.AreEqual(1, errorMessages[0].ChildErrors.Count);
            StringAssert.AreEqual(@"Invalid type. Expected Integer but got String. Path 'foo', line 1, position 14.", errorMessages[0].ChildErrors[0].GetExtendedMessage());
        }

        [Test]
        public void UnusedFailingDependencySchema_InsideAllOf()
        {
            JSchema schema = JSchema.Parse(@"{
                ""dependencies"": {
                    ""bar"": {
                        ""properties"": {
                            ""foo"": {""type"": ""integer""},
                            ""bar"": {""type"": ""integer""}
                        }
                    }
                }
            }");

            JSchema root = new JSchema();
            root.AllOf.Add(schema);

            JToken json = JToken.Parse(@"{""foo"":""quux""}");

            IList<ValidationError> errors;
            bool isValid = json.IsValid(root, out errors);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Format_IPv4_Hex()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""ipv4""}");
            JToken t = JToken.Parse("'0x7f000001'");

            Assert.IsFalse(t.IsValid(s));
        }

        [Test]
        public void Format_Time_24Hour()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""time""}");
            JToken t = JToken.Parse("'18:50:00'");

            Assert.IsTrue(t.IsValid(s));
        }

        [Test]
        public void Format_Time_Milliseconds()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""time""}");
            JToken t = JToken.Parse("'11:50:00.123'");

            Assert.IsTrue(t.IsValid(s));
        }

        [Test]
        public void Format_Time_UTC()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""time""}");
            JToken t = JToken.Parse("'18:55:00Z'");

            Assert.IsTrue(t.IsValid(s));
        }

        [Test]
        public void Format_Time_TimeZoneOffset()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""time""}");
            JToken t = JToken.Parse("'18:55:00-04:00'");

            Assert.IsTrue(t.IsValid(s));
        }

        [Test]
        public void Format_Hostname_InvalidCharacters()
        {
            JSchema s = JSchema.Parse(@"{""format"": ""hostname""}");
            JToken t = JToken.Parse(@"""not_a_valid_host_name""");

            Assert.IsFalse(t.IsValid(s));
        }

        [Test]
        public void IfThenElse_ThenChildErrors()
        {
            JSchema s = JSchema.Parse(@"{
            ""if"": {
                ""exclusiveMaximum"": 0
            },
            ""then"": {
                ""minimum"": -10
            },
            ""else"": {
                ""type"": ""string""
            }
        }");

            JToken t = JToken.Parse("-100");

            Assert.IsFalse(t.IsValid(s, out IList<ValidationError> validationErrors));

            Assert.AreEqual(1, validationErrors.Count);
            Assert.AreEqual("JSON does not match schema from 'then'.", validationErrors[0].Message);
            Assert.AreEqual(ErrorType.Then, validationErrors[0].ErrorType);
            Assert.AreEqual(1, validationErrors[0].ChildErrors.Count);
            Assert.AreEqual("Integer -100 is less than minimum value of -10.", validationErrors[0].ChildErrors[0].Message);
        }

        [Test]
        public void IfThenElse_ComplexThenChildErrors()
        {
            JSchema s = JSchema.Parse(@"{
  ""if"": {
    ""properties"": {
      ""value"": {
        ""type"": ""integer""
      }
    }
  },
  ""then"": {
    ""properties"": {
      ""value"": {
        ""maximum"": -10
      }
    }
  },
  ""else"": {
    ""type"": ""null""
  }
}");

            JToken t = JToken.Parse(@"{""value"":1}");

            Assert.IsFalse(t.IsValid(s, out IList<ValidationError> validationErrors));

            Assert.AreEqual(1, validationErrors.Count);
            Assert.AreEqual("JSON does not match schema from 'then'.", validationErrors[0].Message);
            Assert.AreEqual(ErrorType.Then, validationErrors[0].ErrorType);
            Assert.AreEqual(1, validationErrors[0].ChildErrors.Count);
            Assert.AreEqual("Integer 1 exceeds maximum value of -10.", validationErrors[0].ChildErrors[0].Message);
        }

        [Test]
        public void IfThenElse_ElseChildErrors()
        {
            JSchema s = JSchema.Parse(@"{
            ""if"": {
                ""exclusiveMaximum"": 0
            },
            ""then"": {
                ""minimum"": -10
            },
            ""else"": {
                ""type"": ""string""
            }
        }");

            JToken t = JToken.Parse("99");

            Assert.IsFalse(t.IsValid(s, out IList<ValidationError> validationErrors));

            Assert.AreEqual(1, validationErrors.Count);
            Assert.AreEqual("JSON does not match schema from 'else'.", validationErrors[0].Message);
            Assert.AreEqual(ErrorType.Else, validationErrors[0].ErrorType);
            Assert.AreEqual(1, validationErrors[0].ChildErrors.Count);
            Assert.AreEqual("Invalid type. Expected String but got Integer.", validationErrors[0].ChildErrors[0].Message);
        }

        [Test]
        public void TV4_Issue_86()
        {
            string schemaJson = @"{
			""type"": ""object"",
			""properties"": {
				""shape"": {
					""oneOf"": [
						{ ""$ref"": ""#/definitions/squareSchema"" },
						{ ""$ref"": ""#/definitions/circleSchema"" }
					]
				}
			},
			""definitions"": {
				""squareSchema"": {
					""type"": ""object"",
					""properties"": {
						""thetype"": {
							""type"": ""string"",
							""enum"": [""square""]
						},
						""colour"": {},
						""shade"": {},
						""boxname"": {
							""type"": ""string""
						}
					},
					""oneOf"": [
						{ ""$ref"": ""#/definitions/colourSchema"" },
						{ ""$ref"": ""#/definitions/shadeSchema"" }
					],
					""required"": [""thetype"", ""boxname""],
					""additionalProperties"": false
				},
				""circleSchema"": {
					""type"": ""object"",
					""properties"": {
						""thetype"": {
							""type"": ""string"",
							""enum"": [""circle""]
						},
						""colour"": {},
						""shade"": {}
					},
					""oneOf"": [
						{ ""$ref"": ""#/definitions/colourSchema"" },
						{ ""$ref"": ""#/definitions/shadeSchema"" }
					],
					""additionalProperties"": false
				},
				""colourSchema"": {
					""type"": ""object"",
					""properties"": {
						""colour"": {
							""type"": ""string""
						},
						""shade"": {
							""type"": ""null""
						}
					}
				},
				""shadeSchema"": {
					""type"": ""object"",
					""properties"": {
						""shade"": {
							""type"": ""string""
						},
						""colour"": {
							""type"": ""null""
						}
					}
				}
			}
		}";

            JObject o = JObject.Parse(@"{
			""shape"": {
				""thetype"": ""circle"",
				""shade"": ""red""
			}
		}");

            JSchema schema = JSchema.Parse(schemaJson);

            bool isValid = o.IsValid(schema);
            Assert.IsTrue(isValid);
        }
    }
}