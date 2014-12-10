#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema.Tests.TestObjects;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
#if !(NETFX_CORE || ASPNETCORE50)
using System.Data;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class ExtensionsTests : TestFixtureBase
    {
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
            Assert.AreEqual("Invalid type. Expected Integer but got String.", errorMessages[0]);
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

            Assert.AreEqual("String 'pie' does not match regex pattern 'lol'.", errors[0]);
        }

        [Test]
        public void ValidateWithOutEventHandlerFailure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = JSchema.Parse("{'pattern':'lol'}");
                JToken stringToken = JToken.FromObject("pie");
                stringToken.Validate(schema);
            }, @"String 'pie' does not match regex pattern 'lol'.");
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

            Assert.AreEqual("Required properties are missing from object: lol. Line 1, position 1.", errors[0]);
            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void ValidateRequiredFieldsWithLineInfo()
        {
            JSchema schema = JSchema.Parse("{'properties':{'lol':{'type':'string'}}}");
            JObject o = JObject.Parse("{'lol':1}");

            List<string> errors = new List<string>();
            o.Validate(schema, (sender, args) => errors.Add(args.Path + " - " + args.Message));

            Assert.AreEqual("lol - Invalid type. Expected String but got Integer. Line 1, position 8.", errors[0]);
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

            //JsonSchema schema;

            //using (JsonTextReader reader = new JsonTextReader(new StringReader(schemaJson)))
            //{
            //  JsonSchemaBuilder builder = new JsonSchemaBuilder(new JsonSchemaResolver());
            //  schema = builder.Parse(reader);
            //}

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
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseAssemblyQualifiedName;
            JSchema typeSchema = generator.Generate(typeof(T));
            string schema = typeSchema.ToString();

            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            JToken token = JToken.ReadFrom(new JsonTextReader(new StringReader(json)));

            List<string> errors = new List<string>();

            token.Validate(typeSchema, (sender, args) => { errors.Add(args.Message); });

            if (errors.Count > 0)
                Assert.Fail("Schema generated for type '{0}' is not valid." + Environment.NewLine + string.Join(Environment.NewLine, errors.ToArray()), typeof(T));
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
#if !(NETFX_CORE || PORTABLE || ASPNETCORE50 || PORTABLE40)
            GenerateSchemaAndSerializeFromType(new DataSet());
#endif
            GenerateSchemaAndSerializeFromType(new object());
            GenerateSchemaAndSerializeFromType(1);
            GenerateSchemaAndSerializeFromType("Hi");
            GenerateSchemaAndSerializeFromType(new DateTime(2000, 12, 29, 23, 59, 0, DateTimeKind.Utc));
            GenerateSchemaAndSerializeFromType(TimeSpan.FromTicks(1000000));
#if !(NETFX_CORE || PORTABLE || ASPNETCORE50 || PORTABLE40)
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
            Assert.AreEqual("Property 'g' has not been defined and the schema does not allow additional properties. Line 1, position 5.", errors[0]);
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
            }, "Integer 10 equals maximum value of 10 and exclusive maximum is true.");
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
            }, "Float 10.1 equals maximum value of 10.1 and exclusive maximum is true.");
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
            }, "Integer 10 equals minimum value of 10 and exclusive minimum is true.");
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
            }, "Float 10.1 equals minimum value of 10.1 and exclusive minimum is true.");
        }

        [Test]
        public void DivisibleBy_Int()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema schema = new JSchema();
                schema.DivisibleBy = 3;

                JValue v = new JValue(10);
                v.Validate(schema);
            }, "Integer 10 is not evenly divisible by 3.");
        }

        [Test]
        public void DivisibleBy_Approx()
        {
            JSchema schema = new JSchema();
            schema.DivisibleBy = 0.01;

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
            Assert.AreEqual("Non-unique array item at index 3.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 4.", errorMessages[1]);
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
            Assert.AreEqual("Non-unique array item at index 4.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 6.", errorMessages[1]);
            Assert.AreEqual("Non-unique array item at index 7.", errorMessages[2]);
        }

        [Test]
        public void UniqueItems_NestedDuplicate()
        {
            JSchema schema = new JSchema();
            schema.UniqueItems = true;
            schema.Items = new List<JSchema>
            {
                new JSchema
                {
                    UniqueItems = true
                }
            };
            schema.PositionalItemsValidation = false;

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
            Assert.AreEqual("Non-unique array item at index 1.", errorMessages[0]);
            Assert.AreEqual("Non-unique array item at index 3.", errorMessages[1]);
            Assert.AreEqual("Non-unique array item at index 1.", errorMessages[2]);
            Assert.AreEqual("Non-unique array item at index 4.", errorMessages[3]);
        }

        [Test]
        public void Enum_Properties()
        {
            JSchema schema = new JSchema();
            schema.Properties = new Dictionary<string, JSchema>
            {
                {
                    "bar",
                    new JSchema
                    {
                        Enum = new List<JToken>
                        {
                            new JValue(1),
                            new JValue(2)
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
            JSchema schema = new JSchema();
            schema.Properties = new Dictionary<string, JSchema>
            {
                {
                    "bar",
                    new JSchema
                    {
                        UniqueItems = true
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
            JSchema schema = new JSchema();
            schema.Items = new List<JSchema>
            {
                new JSchema { Type = JSchemaType.Object },
                new JSchema { Type = JSchemaType.Integer }
            };
            schema.PositionalItemsValidation = true;

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
    }
}