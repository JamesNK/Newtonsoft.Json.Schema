#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema.Infrastructure;
#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Theory = Xunit.TheoryAttribute;
using TestCase = Xunit.InlineDataAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
using Theory = NUnit.Framework.TestAttribute;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public abstract class JSchemaValidatingReaderUnevaluatedTestsBase : TestFixtureBase
    {
        protected abstract JSchemaValidatingReader CreateValidatingReader(JsonReader reader);

        [Test]
        public void UnevaluatedProperties_NotAllowed_NoMatch()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""unevaluatedProperties"": false
            }";

            string json = "{'bar':true}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Boolean, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Property 'bar' has not been successfully evaluated and the schema does not allow unevaluated properties. Path '', line 1, position 12.", validationEventArgs.Message);
            Assert.AreEqual(ErrorType.UnevaluatedProperties, validationEventArgs.ValidationError.ErrorType);
        }

        [Test]
        public void UnevaluatedProperties_AnyOf_NoMatch()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""anyOf"": [
                    {
                        ""properties"": {
                            ""bar"": { ""const"": ""bar"" }
                        },
                        ""required"": [""bar""]
                    },
                    {
                        ""properties"": {
                            ""baz"": { ""const"": ""baz"" }
                        },
                        ""required"": [""baz""]
                    },
                    {
                        ""properties"": {
                            ""quux"": { ""const"": ""quux"" }
                        },
                        ""required"": [""quux""]
                    }
                ],
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar"",
                    ""baz"": ""not-baz""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Property 'baz' has not been successfully evaluated and the schema does not allow unevaluated properties. Path '', line 5, position 17.", validationEventArgs.Message);
            Assert.AreEqual(ErrorType.UnevaluatedProperties, validationEventArgs.ValidationError.ErrorType);
        }

        [Test]
        public void UnevaluatedProperties_TrueWithUnevaluatedProperties_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""unevaluatedProperties"": true
            }";

            string json = @"{
                    ""foo"": ""foo""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_DependentSchemas_NoUnevaluatedProperties()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""dependentSchemas"": {
                    ""foo"": {
                        ""properties"": {
                            ""bar"": { ""const"": ""bar"" }
                        },
                        ""required"": [""bar""]
                    }
                },
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_HasUnevaluatedProperty_AllowAllSchema()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""anyOf"": [
                    {
                        ""properties"": {
                            ""bar"": { ""const"": ""bar"" }
                        },
                        ""required"": [""bar""]
                    },
                    {
                        ""properties"": {
                            ""baz"": { ""const"": ""baz"" }
                        },
                        ""required"": [""baz""]
                    },
                    {
                        ""properties"": {
                            ""quux"": { ""const"": ""quux"" }
                        },
                        ""required"": [""quux""]
                    }
                ],
                ""unevaluatedProperties"": {}
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar"",
                    ""baz"": ""not-baz""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_HasUnevaluatedProperty_AdditionalPropertiesSchema()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""additionalProperties"": true,
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_HasUnevaluatedProperty_Not()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" },
                    ""baz"": { ""type"": ""string"" }
                },
                ""not"": {
                    ""not"": {
                        ""properties"": {
                            ""bar"": { ""const"": ""bar"" }
                        },
                        ""required"": [""bar""]
                    }
                },
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar"",
                    ""baz"": ""baz""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Property 'bar' has not been successfully evaluated and the schema does not allow unevaluated properties. Path '', line 5, position 17.", validationEventArgs.Message);
            Assert.AreEqual(ErrorType.UnevaluatedProperties, validationEventArgs.ValidationError.ErrorType);
        }

        [Test]
        public void UnevaluatedProperties_NotAllowed_AllOfProperties_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""allOf"": [
                    {
                        ""properties"": {
                            ""bar"": { ""type"": ""string"" }
                        }
                    }
                ],
                ""unevaluatedProperties"": false
            }";

            string json = "{'bar':'value'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_AnyOfProperties_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""anyOf"": [
                    {
                        ""properties"": {
                            ""bar"": { ""const"": ""bar"" }
                        },
                        ""required"": [""bar""]
                    },
                    {
                        ""properties"": {
                            ""baz"": { ""const"": ""baz"" }
                        },
                        ""required"": [""baz""]
                    },
                    {
                        ""properties"": {
                            ""quux"": { ""const"": ""quux"" }
                        },
                        ""required"": [""quux""]
                    }
                ],
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_NestedAdditionalProperties_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""allOf"": [
                    {
                        ""additionalProperties"": true
                    }
                ],
                ""unevaluatedProperties"": false
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_NestedUnevaluatedProperties_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""allOf"": [
                    {
                        ""unevaluatedProperties"": true
                    }
                ],
                ""unevaluatedProperties"": {
                    ""type"": ""string"",
                    ""maxLength"": 2
                }
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_NestedUnevaluatedPropertiesSchema_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""allOf"": [
                    {
                        ""unevaluatedProperties"": { ""type"": ""string"" }
                    }
                ],
                ""unevaluatedProperties"": {
                    ""type"": ""string"",
                    ""maxLength"": 2
                }
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_Schema_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""unevaluatedProperties"": {
                    ""type"": ""string"",
                    ""minLength"": 3
                }
            }";

            string json = @"{
                    ""foo"": ""foo""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedItems_Schema_Match()
        {
            string schemaJson = @"{
                ""type"": ""array"",
                ""unevaluatedItems"": {
                    ""type"": ""string"",
                    ""minLength"": 3
                }
            }";

            string json = @"[ ""foo"" ]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedItems_Schema_NoMatch()
        {
            string schemaJson = @"{
                ""type"": ""array"",
                ""unevaluatedItems"": {
                    ""type"": ""string"",
                    ""minLength"": 3
                }
            }";

            string json = @"[ ""fo"" ]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Item at index 0 has not been successfully evaluated and the schema does not allow unevaluated items. Path '', line 1, position 8.", validationEventArgs.Message);
            Assert.AreEqual(ErrorType.UnevaluatedItems, validationEventArgs.ValidationError.ErrorType);
        }

        [Test]
        public void UnevaluatedProperties_WithRef_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""$ref"": ""#/$defs/bar"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""unevaluatedProperties"": false,
                ""$defs"": {
                    ""bar"": {
                        ""properties"": {
                            ""bar"": { ""type"": ""string"" }
                        }
                    }
                }
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedProperties_WithRefAndAllOf_Match()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""$ref"": ""#/$defs/bar"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""unevaluatedProperties"": false,
                ""$defs"": {
                    ""bar"": {
                        ""allOf"": [
                            {
                                ""properties"": {
                                    ""bar"": { ""type"": ""string"" }
                                }
                            }
                        ]
                    }
                }
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void SchemaReused_UnevaluatedProperties()
        {
            string schemaJson = @"{
  ""type"": ""object"",
  ""definitions"": {
    ""allRegionProperties"": {
      ""properties"": {
        ""Type"": true,
        ""Slug"": true
      }
    }
  },
  ""required"": [
    ""Type""
  ],
  ""properties"": {
    ""Type"": {
      ""enum"": [
        ""region"",
        ""geo"",
        ""non-regional""
      ]
    }
  },
  ""oneOf"": [
    {
      ""if"": {
        ""properties"": {
          ""Type"": {
            ""enum"": [
              ""geo""
            ]
          }
        }
      },
      ""then"": {
        ""$ref"": ""#/definitions/allRegionProperties"",
        ""unevaluatedProperties"": false
      },
      ""else"": false
    },
    {
      ""if"": {
        ""properties"": {
          ""Type"": {
            ""enum"": [
              ""non-regional""
            ]
          }
        }
      },
      ""then"": {
        ""allOf"": [
          {
            ""$ref"": ""#/definitions/allRegionProperties"",
            ""unevaluatedProperties"": false
          }
        ]
      },
      ""else"": false
    }
  ]
}";

            string json = @"{
  ""Type"": ""non-regional"",
  ""Unknown"": ""2 spooky""
}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", validationEventArgs.ValidationError.Message);
            Assert.AreEqual("JSON does not match schema from 'then'.", validationEventArgs.ValidationError.ChildErrors[0].Message);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0.", validationEventArgs.ValidationError.ChildErrors[0].ChildErrors[0].Message);
            Assert.AreEqual("Property 'Unknown' has not been successfully evaluated and the schema does not allow unevaluated properties.", validationEventArgs.ValidationError.ChildErrors[0].ChildErrors[0].ChildErrors[0].Message);
        }

        [Test]
        public void UnevaluatedItems_ConditionalSchema_Match()
        {
            string schemaJson = @"{
                ""type"": ""array"",
                ""items"": [
                    { ""type"": ""string"" }
                ],
                ""allOf"": [
                    {
                        ""items"": [
                            true,
                            { ""allOf"": [ true ] }
                        ]
                    }
                ],
                ""unevaluatedItems"": false
            }";

            string json = @"[""foo"", 42]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void UnevaluatedItems_ConditionalSchema_NoMatch()
        {
            string schemaJson = @"{
                ""type"": ""array"",
                ""items"": [
                    { ""type"": ""string"" }
                ],
                ""allOf"": [
                    {
                        ""items"": [
                            true,
                            { ""allOf"": [ false ] }
                        ]
                    }
                ],
                ""unevaluatedItems"": false
            }";

            string json = @"[""foo"", 42]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Item at index 1 has not been successfully evaluated and the schema does not allow unevaluated items. Path '', line 1, position 11.", validationEventArgs.Message);
        }

        [Test]
        public void UnevaluatedProperties_SchemaEvaluatedOnInvalidProperty_ErrorDisplayed()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""unevaluatedProperties"": {
                    ""type"": ""string"",
                    ""maxLength"": 2
                }
            }";

            string json = @"{
                    ""foo"": ""foo"",
                    ""bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Property 'bar' has not been successfully evaluated and the schema does not allow unevaluated properties.", validationEventArgs.ValidationError.Message);
            Assert.AreEqual(ErrorType.UnevaluatedProperties, validationEventArgs.ValidationError.ErrorType);
            Assert.AreEqual("String 'bar' exceeds maximum length of 2.", validationEventArgs.ValidationError.ChildErrors[0].Message);
        }

        [Test]
        public void UnevaluatedItems_SchemaEvaluatedOnInvalidItem_ErrorDisplayed()
        {
            string schemaJson = @"{
                ""type"": ""array"",
                ""unevaluatedItems"": {
                    ""type"": ""string"",
                    ""minLength"": 3
                }
            }";

            string json = @"[ ""foo"", ""fo"" ]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = CreateValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            while (reader.Read())
            {
            }

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Item at index 1 has not been successfully evaluated and the schema does not allow unevaluated items.", validationEventArgs.ValidationError.Message);
            Assert.AreEqual(ErrorType.UnevaluatedItems, validationEventArgs.ValidationError.ErrorType);
            Assert.AreEqual("String 'fo' is less than minimum length of 3.", validationEventArgs.ValidationError.ChildErrors[0].Message);
        }

        [Test]
        public void UnevaluatedPropertiesWithDynamicRef()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://example.com/derived"",

                ""$ref"": ""/baseSchema"",

                ""$defs"": {
                    ""derived"": {
                        ""$dynamicAnchor"": ""addons"",
                        ""properties"": {
                            ""bar"": { ""type"": ""string"" }
                        }
                    },
                    ""baseSchema"": {
                        ""$id"": ""/baseSchema"",

                        ""$comment"": ""unevaluatedProperties comes first so it's more likely to catch bugs with implementations that are sensitive to keyword ordering"",
                        ""unevaluatedProperties"": false,
                        ""type"": ""object"",
                        ""properties"": {
                            ""foo"": { ""type"": ""string"" }
                        },
                        ""$dynamicRef"": ""#addons"",

                        ""$defs"": {
                            ""defaultAddons"": {
                                ""$comment"": ""Needed to satisfy the bookending requirement"",
                                ""$dynamicAnchor"": ""addons""
                            }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
            JSchema addonSchema = s.Ref.Ref;
            Assert.AreEqual(JSchemaType.String, addonSchema.Properties["bar"].Type);

            JObject o = JObject.Parse(@"{
                ""foo"": ""foo"",
                ""bar"": ""bar""
            }");
            var result = o.IsValid(s, out IList<ValidationError> errorMessages);
            PrintErrorsRecursive(errorMessages, 0);
            Assert.IsTrue(result, string.Join(", ", errorMessages.Select(e => e.ToString()).ToArray()));
        }

        [Test]
        public void UnevaluatedPropertiesWithIfThenElse()
        {
            string json = @"{
              ""$schema"": ""http://json-schema.org/draft-07/schema#"",
              ""$id"": ""https://json.schemastore.org/minecraft-recipe.json"",
              ""$comment"": ""https://minecraft.wiki/w/Recipe#JSON_format"",
              ""allOf"": [
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:blasting""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a recipe in a blast furnace."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllCookingRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/cookingRecipeCategory""
                      }
                    ]
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:campfire_cooking""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a recipe in a campfire."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllCookingRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/cookingRecipeCategory""
                      }
                    ]
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:crafting_shaped""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a shaped crafting recipe in a crafting table."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/commonRecipeCategory""
                      }
                    ],
                    ""properties"": {
                      ""pattern"": {
                        ""description"": ""A list of single-character keys used to describe a pattern for shaped crafting."",
                        ""type"": ""array"",
                        ""items"": {
                          ""type"": ""string"",
                          ""maxLength"": 3
                        },
                        ""maxItems"": 3
                      },
                      ""key"": {
                        ""title"": ""key"",
                        ""description"": ""All keys used for this shaped crafting recipe."",
                        ""additionalProperties"": {
                          ""type"": [""object"", ""array""],
                          ""description"": ""The ingredient corresponding to this key."",
                          ""properties"": {
                            ""item"": {
                              ""description"": ""An item ID."",
                              ""type"": ""string""
                            },
                            ""tag"": {
                              ""description"": ""An item tag."",
                              ""type"": ""string""
                            }
                          },
                          ""items"": {
                            ""properties"": {
                              ""item"": {
                                ""description"": ""An item ID."",
                                ""type"": ""string""
                              },
                              ""tag"": {
                                ""description"": ""An item tag."",
                                ""type"": ""string""
                              }
                            }
                          }
                        }
                      },
                      ""result"": {
                        ""$ref"": ""#/definitions/result""
                      }
                    }
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:crafting_shapeless""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a shapeless crafting recipe in a crafting table."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/commonRecipeCategory""
                      }
                    ],
                    ""properties"": {
                      ""ingredients"": {
                        ""description"": ""A list of entries for this shapeless crafting recipe."",
                        ""type"": ""array"",
                        ""items"": {
                          ""oneOf"": [
                            {
                              ""$ref"": ""#/definitions/ingredient"",
                              ""title"": ""ingredient"",
                              ""description"": ""An entry made of a single ingredient."",
                              ""type"": ""object""
                            },
                            {
                              ""description"": ""An entry made of a list of acceptable ingredients."",
                              ""type"": ""array"",
                              ""minItems"": 1,
                              ""maxItems"": 9,
                              ""items"": {
                                ""$ref"": ""#/definitions/ingredient""
                              }
                            }
                          ]
                        }
                      },
                      ""result"": {
                        ""$ref"": ""#/definitions/result""
                      }
                    }
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:smelting""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a recipe in a furnace."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllCookingRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/cookingRecipeCategory""
                      }
                    ]
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:smithing""
                      }
                    }
                  },
                  ""then"": {
                    ""$ref"": ""#/definitions/tagsCommonToAllRecipes"",
                    ""description"": ""Represents a recipe in a smithing table."",
                    ""properties"": {
                      ""base"": {
                        ""$ref"": ""#/definitions/item"",
                        ""title"": ""base"",
                        ""description"": ""Ingredient specifying an item to be upgraded.""
                      },
                      ""addition"": {
                        ""$ref"": ""#/definitions/item"",
                        ""title"": ""addition""
                      },
                      ""result"": {
                        ""title"": ""result"",
                        ""type"": ""object""
                      }
                    }
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:smoking""
                      }
                    }
                  },
                  ""then"": {
                    ""description"": ""Represents a recipe in a smoker."",
                    ""anyOf"": [
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/tagsCommonToAllCookingRecipes""
                      },
                      {
                        ""$ref"": ""#/definitions/cookingRecipeCategory""
                      }
                    ]
                  }
                },
                {
                  ""if"": {
                    ""properties"": {
                      ""type"": {
                        ""const"": ""minecraft:stonecutting""
                      }
                    }
                  },
                  ""then"": {
                    ""$ref"": ""#/definitions/tagsCommonToAllRecipes"",
                    ""description"": ""Represents a recipe in a stonecutter."",
                    ""oneOf"": [
                      {
                        ""$ref"": ""#/definitions/item"",
                        ""title"": ""ingredient"",
                        ""description"": ""The ingredient for the recipe.""
                      },
                      {
                        ""title"": ""ingredient"",
                        ""description"": ""The list of ingredients for the recipe."",
                        ""type"": ""array"",
                        ""items"": {
                          ""$ref"": ""#/definitions/item"",
                          ""title"": ""ingredient""
                        }
                      }
                    ],
                    ""properties"": {
                      ""result"": {
                        ""description"": ""An item ID. The output item of the recipe."",
                        ""type"": ""string""
                      },
                      ""count"": {
                        ""description"": ""The amount of the output item."",
                        ""type"": ""integer""
                      }
                    }
                  }
                }
              ],
              ""definitions"": {
                ""item"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""item"": {
                      ""description"": ""An item ID."",
                      ""type"": ""string""
                    },
                    ""tag"": {
                      ""description"": ""An item tag."",
                      ""type"": ""string""
                    }
                  }
                },
                ""ingredient"": {
                  ""$ref"": ""#/definitions/item"",
                  ""title"": ""ingredient"",
                  ""description"": ""An acceptable ingredient."",
                  ""type"": ""object""
                },
                ""tagsCommonToAllRecipes"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""type"": {
                      ""description"": ""A namespaced ID indicating the type of serializer of the recipe."",
                      ""type"": ""string""
                    },
                    ""group"": {
                      ""description"": ""A string identifier. Used to group multiple recipes together in the recipe book."",
                      ""type"": ""string""
                    },
                    ""show_notification"": {
                      ""description"": ""If a notification is shown when the recipe is unlocked."",
                      ""type"": ""boolean""
                    }
                  }
                },
                ""tagsCommonToAllCookingRecipes"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""ingredient"": {
                      ""$ref"": ""#/definitions/ingredient"",
                      ""title"": ""ingredients"",
                      ""description"": ""The ingredients."",
                      ""type"": [""object"", ""array""],
                      ""items"": {
                        ""$ref"": ""#/definitions/ingredient""
                      }
                    },
                    ""result"": {
                      ""description"": ""An item ID. The output item of the recipe."",
                      ""type"": ""string""
                    },
                    ""experience"": {
                      ""description"": ""The output experience of the recipe."",
                      ""type"": ""number""
                    },
                    ""cookingtime"": {
                      ""description"": ""The cook time of the recipe in ticks."",
                      ""type"": ""integer""
                    }
                  }
                },
                ""commonRecipeCategory"": {
                  ""title"": ""category"",
                  ""description"": ""Category of common recipes (in recipe book)."",
                  ""type"": ""object"",
                  ""properties"": {
                    ""category"": {
                      ""type"": ""string"",
                      ""enum"": [""building"", ""redstone"", ""equipment"", ""misc""]
                    }
                  }
                },
                ""cookingRecipeCategory"": {
                  ""title"": ""category"",
                  ""description"": ""Category of cooking recipes (in recipe book)."",
                  ""type"": ""object"",
                  ""properties"": {
                    ""category"": {
                      ""type"": ""string"",
                      ""enum"": [""food"", ""blocks"", ""misc ""]
                    }
                  }
                },
                ""result"": {
                  ""title"": ""result"",
                  ""description"": ""The output item of the recipe."",
                  ""type"": ""object"",
                  ""properties"": {
                    ""count"": {
                      ""description"": ""The amount of the item."",
                      ""type"": ""integer"",
                      ""default"": 1
                    },
                    ""item"": {
                      ""description"": ""An item ID."",
                      ""type"": ""string""
                    }
                  }
                }
              },
              ""description"": ""Configuration file defining a recipe for a data pack for Minecraft."",
              ""properties"": {
                ""type"": {
                  ""description"": ""The type of recipe."",
                  ""type"": ""string"",
                  ""enum"": [
                    ""minecraft:blasting"",
                    ""minecraft:campfire_cooking"",
                    ""minecraft:crafting_shaped"",
                    ""minecraft:crafting_shapeless"",
                    ""minecraft:smelting"",
                    ""minecraft:smithing"",
                    ""minecraft:smoking"",
                    ""minecraft:stonecutting""
                  ]
                }
              },
              ""title"": ""minecraft data pack recipe"",
              ""type"": ""object""
            }";

            JSchema s = JSchema.Parse(json);

            JObject o = JObject.Parse(@"{}");
            var result = o.IsValid(s, out IList<ValidationError> errorMessages);
            PrintErrorsRecursive(errorMessages, 0);
            Assert.IsTrue(result, string.Join(", ", errorMessages.Select(e => e.ToString()).ToArray()));
        }

        private void PrintErrorsRecursive(IList<ValidationError> errors, int depth)
        {
            foreach (ValidationError validationError in errors)
            {
                string prefix = new string(' ', depth);

                Console.WriteLine(prefix + validationError.GetExtendedMessage() + " - " + validationError.SchemaId + " - " + validationError.SchemaBaseUri);

                PrintErrorsRecursive(validationError.ChildErrors, depth + 2);
            }
        }
    }
}