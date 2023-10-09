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
using System.Text;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaValidatingReaderCaseInsensitiveTests : TestFixtureBase
    {
        [Test]
        public void ValidateObjectWithProperty_CaseInsensitive()
        {
            JSchema schema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    { "testProp", new JSchema { Type = JSchemaType.Boolean } },
                    { "testProp2", new JSchema { Type = JSchemaType.Integer } }
                }
            };

            string json = "{'TestProp':5,'TestProp2':true}";

            List<SchemaValidationEventArgs> errors = new List<SchemaValidationEventArgs>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { errors.Add(args); };
            reader.Schema = schema;
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Invalid type. Expected Boolean but got Integer. Path 'TestProp', line 1, position 13.", errors[0].Message);
            Assert.AreEqual(5L, errors[0].ValidationError.Value);
            Assert.AreEqual("Invalid type. Expected Integer but got Boolean. Path 'TestProp2', line 1, position 30.", errors[1].Message);
            Assert.AreEqual(true, errors[1].ValidationError.Value);
        }

        [Test]
        public void RequiredProperties_Inline_CaseInsensitive()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string"",""required"":true},
    ""age"":{""type"":""integer"",""required"":false}
  }
}";

            string json = "{'NAME':'James'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("NAME", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void RequiredProperties_CaseInsensitive()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""},
    ""age"":{""type"":""integer""}
  },
  ""required"": [""name""]
}";

            string json = "{'NAME':'James'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("NAME", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void MissingDependency_Single_CaseInsensitive()
        {
            string schemaJson = @"{
                ""dependencies"": {""bar"": ""foo""}
            }";

            string json = @"{""Bar"": ""bar""}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Bar", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("bar", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            StringAssert.AreEqual(@"Dependencies for property 'Bar' failed. Missing required keys: foo. Path '', line 1, position 14.", validationEventArgs.Message);
        }

        [Test]
        public void MultipleDependency_CaseInsensitive()
        {
            string schemaJson = @"{
                ""dependencies"": {""bar"": [""foo"", ""baz""]}
            }";

            string json = @"{""Foo"": ""foo"", ""bar"": ""bar"", ""Baz"": ""baz""}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Foo", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("foo", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("bar", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("bar", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Baz", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("baz", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void MissingDependenctRequired_Single_CaseInsensitive()
        {
            string schemaJson = @"{
                ""dependentRequired"": {""bar"": [""foo""]}
            }";

            string json = @"{""Bar"": ""bar""}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Bar", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("bar", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            StringAssert.AreEqual(@"Dependencies for property 'Bar' failed. Missing required keys: foo. Path '', line 1, position 14.", validationEventArgs.Message);
        }

        [Test]
        public void DependenctRequired_Single_CaseInsensitive()
        {
            string schemaJson = @"{
                ""dependentRequired"": {""bar"": [""foo""]}
            }";

            string json = @"{""Bar"": ""bar"", ""Foo"": ""foo""}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Bar", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("bar", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Foo", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("foo", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void DisableAdditionalProperties_CaseInsensitive()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""}
  },
  ""additionalProperties"":false
}";

            string json = "{'NAME':'James','additionalProperty1':null,'additionalProperty2':null}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("NAME", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("additionalProperty1", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.AreEqual(null, reader.Value);

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Property 'additionalProperty1' has not been defined and the schema does not allow additional properties. Path 'additionalProperty1', line 1, position 38.", validationEventArgs.Message);
            Assert.AreEqual("additionalProperty1", validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("additionalProperty2", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.AreEqual(null, reader.Value);
            Assert.AreEqual("Property 'additionalProperty2' has not been defined and the schema does not allow additional properties. Path 'additionalProperty2', line 1, position 65.", validationEventArgs.Message);
            Assert.AreEqual("additionalProperty2", validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
        }

        [Test]
        public void UnevaluatedProperties_NotAllowed_Match_CaseInsensitive()
        {
            string schemaJson = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""type"": ""string"" }
                },
                ""unevaluatedProperties"": false
            }";

            string json = "{'FOO':'foo'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

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
        public void UnevaluatedProperties_DependentSchemas_NoUnevaluatedProperties_CaseInsensitive()
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
                    ""Foo"": ""foo"",
                    ""Bar"": ""bar""
                }";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void SchemaDraft4()
        {
            string schemaJson = TestHelpers.OpenFileText("Resources/Schemas/schema-draft-v4.json");

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(schemaJson)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);
            reader.PropertyNameCaseInsensitive = true;

            while (reader.Read())
            {
            }

            Assert.IsNull(validationEventArgs);
        }
    }
}
