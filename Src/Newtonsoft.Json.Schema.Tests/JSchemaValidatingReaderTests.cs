#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#endif
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
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaValidatingReaderTests : TestFixtureBase
    {
        [Test]
        public void Sample()
        {
            JSchema schema = JSchema.Parse(@"{
  'type': 'array',
  'items': {'type':'string'}
}");

            JsonTextReader reader = new JsonTextReader(new StringReader(@"[
  'Developer',
  'Administrator'
]"));
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = schema;
            validatingReader.ValidationEventHandler += (sender, args) => { throw new Exception(args.Message); };

            JsonSerializer serializer = new JsonSerializer();
            List<string> roles = serializer.Deserialize<List<string>>(validatingReader);
        }

        private JSchemaValidatingReader CreateReader(string json, JSchema schema, out IList<SchemaValidationEventArgs> errors)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));

            List<SchemaValidationEventArgs> localErrors = new List<SchemaValidationEventArgs>();

            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.ValidationEventHandler += (sender, args) => { localErrors.Add(args); };
            validatingReader.Schema = schema;

            errors = localErrors;
            return validatingReader;
        }

        [Test]
        public void ValidateInteger()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Integer;

            string json = "1";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader validatingReader = CreateReader(json, schema, out errors);

            Assert.IsTrue(validatingReader.Read());

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void ValidateObject()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Object;

            string json = "{}";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader validatingReader = CreateReader(json, schema, out errors);

            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsFalse(validatingReader.Read());

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void ValidateObjectWithProperty()
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

            string json = "{'testProp':5,'testProp2':true}";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader validatingReader = CreateReader(json, schema, out errors);

            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsFalse(validatingReader.Read());

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Invalid type. Expected Boolean but got Integer. Path 'testProp', line 1, position 13.", errors[0].Message);
            Assert.AreEqual(5L, errors[0].ValidationError.Value);
            Assert.AreEqual("Invalid type. Expected Integer but got Boolean. Path 'testProp2', line 1, position 30.", errors[1].Message);
            Assert.AreEqual(true, errors[1].ValidationError.Value);
        }

        [Test]
        public void ValidateArray()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema { Type = JSchemaType.Integer });

            string json = "[1,true,2,'hi']";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader validatingReader = CreateReader(json, schema, out errors);

            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsTrue(validatingReader.Read());
            Assert.IsFalse(validatingReader.Read());

            Assert.AreEqual(2, errors.Count);
            StringAssert.AreEqual(@"Invalid type. Expected Integer but got Boolean. Path '[1]', line 1, position 7.", errors[0].Message);
            Assert.AreEqual(true, errors[0].ValidationError.Value);
            StringAssert.AreEqual(@"Invalid type. Expected Integer but got String. Path '[3]', line 1, position 14.", errors[1].Message);
            Assert.AreEqual("hi", errors[1].ValidationError.Value);
        }

        [Test]
        public void ValidateInteger_AllOfFailure()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Integer;

            schema.AllOf.Add(new JSchema
            {
                Maximum = 10
            });
            schema.AllOf.Add(new JSchema
            {
                Minimum = 2
            });

            string json = "1";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader validatingReader = CreateReader(json, schema, out errors);

            Assert.IsTrue(validatingReader.Read());

            Assert.AreEqual(1, errors.Count);
        }

        [Test]
        public void ValidateInteger_BadTypeFailure()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Boolean;

            string json = "1";
            JsonReader reader = new JsonTextReader(new StringReader(json));

            SchemaValidationEventArgs validationEventArgs = null;
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            validatingReader.Schema = schema;

            Assert.IsTrue(validatingReader.Read());

            Assert.IsNotNull(validationEventArgs);
            StringAssert.AreEqual(@"Invalid type. Expected Boolean but got Integer. Path '', line 1, position 1.", validationEventArgs.Message);
            Assert.AreEqual(1L, validationEventArgs.ValidationError.Value);
        }

        [Test]
        public void CheckInnerReader()
        {
            string json = "{'name':'James','hobbies':['pie','cake']}";
            JsonReader reader = new JsonTextReader(new StringReader(json));

            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            Assert.AreEqual(reader, validatingReader.Reader);
        }

        [Test]
        public void ValidateTypes()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""},
    ""hobbies"":
    {
      ""type"":""array"",
      ""items"": {""type"":""string""}
    }
  }
}";

            string json = @"{'name':""James"",'hobbies':[""pie"",'cake']}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            JSchema schema = JSchema.Parse(schemaJson);
            reader.Schema = schema;
            Assert.AreEqual(schema, reader.Schema);
            Assert.AreEqual(0, reader.Depth);
            Assert.AreEqual("", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);
            Assert.AreEqual("", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("name", reader.Value.ToString());
            Assert.AreEqual("name", reader.Path);
            Assert.AreEqual(1, reader.Depth);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.AreEqual(typeof(string), reader.ValueType);
            Assert.AreEqual('"', reader.QuoteChar);
            Assert.AreEqual("name", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("hobbies", reader.Value.ToString());
            Assert.AreEqual('\'', reader.QuoteChar);
            Assert.AreEqual("hobbies", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.AreEqual("hobbies", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("pie", reader.Value.ToString());
            Assert.AreEqual('"', reader.QuoteChar);
            Assert.AreEqual("hobbies[0]", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("cake", reader.Value.ToString());
            Assert.AreEqual("hobbies[1]", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.AreEqual("hobbies", reader.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
            Assert.AreEqual("", reader.Path);

            Assert.IsFalse(reader.Read());

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void ValidateUnrestrictedArray()
        {
            string schemaJson = @"{
  ""type"":""array""
}";

            string json = "['pie','cake',['nested1','nested2'],{'nestedproperty1':1.1,'nestedproperty2':[null]}]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("pie", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("cake", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("nested1", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("nested2", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("nestedproperty1", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual(1.1, reader.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("nestedproperty2", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void StringLessThanMinimumLength()
        {
            string schemaJson = @"{
  ""type"":""string"",
  ""minLength"":5,
  ""maxLength"":50,
}";

            string json = "'pie'";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("String 'pie' is less than minimum length of 5. Path '', line 1, position 5.", validationEventArgs.Message);
            Assert.AreEqual("pie", validationEventArgs.ValidationError.Value);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void StringGreaterThanMaximumLength()
        {
            string schemaJson = @"{
  ""type"":""string"",
  ""minLength"":5,
  ""maxLength"":10
}";

            string json = "'The quick brown fox jumps over the lazy dog.'";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("String 'The quick brown fox jumps over the lazy dog.' exceeds maximum length of 10. Path '', line 1, position 46.", validationEventArgs.Message);
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", validationEventArgs.ValidationError.Value);
            Assert.AreEqual(null, validationEventArgs.ValidationError.SchemaBaseUri);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void StringIsNotInEnum()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""string"",
    ""enum"":[""one"",""two""]
  },
  ""maxItems"":3
}";

            string json = "['one','two','THREE']";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual(@"Value ""THREE"" is not defined in enum. Path '[2]', line 1, position 20.", validationEventArgs.Message);
            Assert.AreEqual("THREE", validationEventArgs.ValidationError.Value);
            Assert.AreEqual("[2]", validationEventArgs.Path);
        }

        [Test]
        public void StringDoesNotMatchPattern()
        {
            string schemaJson = @"{
  ""type"":""string"",
  ""pattern"":""foo""
}";

            string json = "'The quick brown fox jumps over the lazy dog.'";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("String 'The quick brown fox jumps over the lazy dog.' does not match regex pattern 'foo'. Path '', line 1, position 46.", validationEventArgs.Message);
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", validationEventArgs.ValidationError.Value);
            Assert.AreEqual("", validationEventArgs.Path);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void IntegerGreaterThanMaximumValue()
        {
            string schemaJson = @"{
  ""type"":""integer"",
  ""maximum"":5
}";

            string json = "10";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Integer 10 exceeds maximum value of 5. Path '', line 1, position 2.", validationEventArgs.Message);
            Assert.AreEqual(10L, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("", validationEventArgs.Path);

            Assert.IsNotNull(validationEventArgs);
        }

#if !(NET20 || NET35 || PORTABLE || PORTABLE40)
        [Test]
        public void IntegerGreaterThanMaximumValue_BigInteger()
        {
            string schemaJson = @"{
  ""type"":""integer"",
  ""maximum"":5
}";

            string json = "99999999999999999999999999999999999999999999999999999999999999999999";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Integer 99999999999999999999999999999999999999999999999999999999999999999999 exceeds maximum value of 5. Path '', line 1, position 68.", validationEventArgs.Message);
            Assert.AreEqual(BigInteger.Parse("99999999999999999999999999999999999999999999999999999999999999999999"), validationEventArgs.ValidationError.Value);
            Assert.AreEqual("", validationEventArgs.Path);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void IntegerLessThanMaximumValue_BigInteger()
        {
            string schemaJson = @"{
  ""type"":""integer"",
  ""minimum"":5
}";

            JValue v = new JValue(new BigInteger(1));

            SchemaValidationEventArgs validationEventArgs = null;

            v.Validate(JSchema.Parse(schemaJson), (sender, args) => { validationEventArgs = args; });

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Integer 1 is less than minimum value of 5. Path ''.", validationEventArgs.Message);
            Assert.AreEqual(new BigInteger(1), validationEventArgs.ValidationError.Value);
            Assert.AreEqual("", validationEventArgs.Path);
        }
#endif

        [Test]
        public void ThrowExceptionWhenNoValidationEventHandler()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                string schemaJson = @"{
  ""type"":""integer"",
  ""maximum"":5
}";

                JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader("10")));
                reader.Schema = JSchema.Parse(schemaJson);

                Assert.IsTrue(reader.Read());
            }, "Integer 10 exceeds maximum value of 5. Path '', line 1, position 2.");
        }

        [Test]
        public void IntegerLessThanMinimumValue()
        {
            string schemaJson = @"{
  ""type"":""integer"",
  ""minimum"":5
}";

            string json = "1";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Integer 1 is less than minimum value of 5. Path '', line 1, position 1.", validationEventArgs.Message);
            Assert.AreEqual(1L, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("#", validationEventArgs.ValidationError.SchemaId.ToString());
            Assert.AreEqual(ErrorType.Minimum, validationEventArgs.ValidationError.ErrorType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void IntegerIsNotInEnum()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""integer"",
    ""enum"":[1,2]
  },
  ""maxItems"":3
}";

            string json = "[1,2,3]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(@"Value 3 is not defined in enum. Path '[2]', line 1, position 6.", validationEventArgs.Message);
            Assert.AreEqual("[2]", validationEventArgs.Path);
            Assert.AreEqual(3L, validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void FloatGreaterThanMaximumValue()
        {
            string schemaJson = @"{
  ""type"":""number"",
  ""maximum"":5
}";

            string json = "10.0";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual("Float 10.0 exceeds maximum value of 5. Path '', line 1, position 4.", validationEventArgs.Message);
            Assert.AreEqual(10.0d, validationEventArgs.ValidationError.Value);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void FloatLessThanMinimumValue()
        {
            string schemaJson = @"{
  ""type"":""number"",
  ""minimum"":5
}";

            string json = "1.1";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual("Float 1.1 is less than minimum value of 5. Path '', line 1, position 3.", validationEventArgs.Message);
            Assert.AreEqual(1.1d, validationEventArgs.ValidationError.Value);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void FloatIsNotInEnum()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""enum"":[1.1,2.2]
  },
  ""maxItems"":3
}";

            string json = "[1.1,2.2,3.0]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual(@"Value 3.0 is not defined in enum. Path '[2]', line 1, position 12.", validationEventArgs.Message);
            Assert.AreEqual(3.0d, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("[2]", validationEventArgs.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void FloatDivisibleBy()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""divisibleBy"":0.1
  }
}";

            string json = "[1.1,2.2,4.001]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);
            Assert.AreEqual(@"Float 4.001 is not a multiple of 0.1. Path '[2]', line 1, position 14.", validationEventArgs.Message);
            Assert.AreEqual(4.001d, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("[2]", validationEventArgs.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

#if !(NET20 || NET35 || PORTABLE || PORTABLE40)
        [Test]
        public void BigIntegerDivisibleBy_Success()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""divisibleBy"":2
  }
}";

            string json = "[999999999999999999999999999999999999999999999999999999998]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNull(validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
        }

        [Test]
        public void BigIntegerDivisibleBy_Failure()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""divisibleBy"":2
  }
}";

            string json = "[999999999999999999999999999999999999999999999999999999999]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(@"Integer 999999999999999999999999999999999999999999999999999999999 is not a multiple of 2. Path '[0]', line 1, position 58.", validationEventArgs.Message);
            Assert.AreEqual(BigInteger.Parse("999999999999999999999999999999999999999999999999999999999"), validationEventArgs.ValidationError.Value);
            Assert.AreEqual("[0]", validationEventArgs.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void BigIntegerDivisibleBy_Fraction()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""divisibleBy"":1.1
  }
}";

            string json = "[999999999999999999999999999999999999999999999999999999999]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual(@"Integer 999999999999999999999999999999999999999999999999999999999 is not a multiple of 1.1. Path '[0]', line 1, position 58.", validationEventArgs.Message);
            Assert.AreEqual(BigInteger.Parse("999999999999999999999999999999999999999999999999999999999"), validationEventArgs.ValidationError.Value);
            Assert.AreEqual("[0]", validationEventArgs.Path);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
        }

        [Test]
        public void BigIntegerDivisibleBy_FractionWithZeroValue()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number"",
    ""divisibleBy"":1.1
  }
}";

            JArray a = new JArray(new JValue(new BigInteger(0)));

            SchemaValidationEventArgs validationEventArgs = null;

            a.Validate(JSchema.Parse(schemaJson), (sender, args) => { validationEventArgs = args; });

            Assert.IsNull(validationEventArgs);
        }
#endif

        [Test]
        public void ReadDateTimes()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""string"",
    ""minLength"":21
  }
}";

            string json = @"[
  ""2000-12-02T05:06:02+00:00"",
  ""2000-12-02T05:06:02Z"",
  1
]";

            SchemaValidationEventArgs a = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { a = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.IsNull(a);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNull(a);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual(@"String '2000-12-02T05:06:02Z' is less than minimum length of 21. Path '[1]', line 3, position 24.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02Z", a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Integer. Path '[2]', line 4, position 3.", a.Message);
            Assert.AreEqual(1L, a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.IsNull(a);
        }

        [Test]
        public void ReadDateTimes_DateParseHandling()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""string"",
    ""minLength"":21
  }
}";

            string json = @"[
  ""2000-12-02T05:06:02+00:00"",
  ""2000-12-02T05:06:02Z"",
  1 // hi
]";

            SchemaValidationEventArgs a = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json))
            {
                DateParseHandling = DateParseHandling.DateTimeOffset
            });
            reader.ValidationEventHandler += (sender, args) => { a = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.IsNull(a);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNull(a);

            reader.ReadAsString();
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual(@"String '2000-12-02T05:06:02Z' is less than minimum length of 21. Path '[1]', line 3, position 24.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02Z", a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Integer. Path '[2]', line 4, position 3.", a.Message);
            Assert.AreEqual(1L, a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Comment, reader.TokenType);
            Assert.IsNull(a);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.IsNull(a);
        }

        [Test]
        public void ReadDateTimes_ReadAsDateTime()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""string"",
    ""minLength"":21
  }
}";

            string json = @"[
  ""2000-12-02T05:06:02+00:00"",
  ""2000-12-02T05:06:02Z"",
  1
]";

            SchemaValidationEventArgs a = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { a = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.IsNull(a);

            reader.ReadAsDateTime();
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNull(a);

            reader.ReadAsDateTime();
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual(@"String '2000-12-02T05:06:02Z' is less than minimum length of 21. Path '[1]', line 3, position 24.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02Z", a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Integer. Path '[2]', line 4, position 3.", a.Message);
            Assert.AreEqual(1L, a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.IsNull(a);
        }

        [Test]
        public void ReadDateTimes_ReadAsDateTimeOffset()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""string"",
    ""minLength"":21
  }
}";

            string json = @"[
  ""2000-12-02T05:06:02+00:00"",
  ""2000-12-02T05:06:02Z"",
  1
]";

            SchemaValidationEventArgs a = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { a = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.IsNull(a);

            reader.ReadAsDateTimeOffset();
            Assert.AreEqual(JsonToken.Date, reader.TokenType);
            Assert.IsNull(a);

            reader.ReadAsString();
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual(@"String '2000-12-02T05:06:02Z' is less than minimum length of 21. Path '[1]', line 3, position 24.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02Z", a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Integer. Path '[2]', line 4, position 3.", a.Message);
            Assert.AreEqual(1L, a.ValidationError.Value);
            a = null;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.IsNull(a);
        }

        [Test]
        public void IntValidForNumber()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""number""
  }
}";

            string json = "[1]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void NullNotInEnum()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""null"",
    ""enum"":[1234567]
  },
  ""maxItems"":3
}";

            string json = "[null]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.AreEqual(@"Value null is not defined in enum. Path '[0]', line 1, position 5.", validationEventArgs.Message);
            Assert.AreEqual("[0]", validationEventArgs.Path);
            Assert.AreEqual(null, validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void BooleanNotInEnum()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""boolean"",
    ""enum"":[true]
  },
  ""maxItems"":3
}";

            string json = "[true,false]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Boolean, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Boolean, reader.TokenType);
            Assert.AreEqual(@"Value false is not defined in enum. Path '[1]', line 1, position 11.", validationEventArgs.Message);
            Assert.AreEqual("[1]", validationEventArgs.Path);
            Assert.AreEqual(false, validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void ArrayCountGreaterThanMaximumItems()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""minItems"":2,
  ""maxItems"":3
}";

            string json = "[null,null,null,null]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual("Array item count 4 exceeds maximum count of 3. Path '', line 1, position 21.", validationEventArgs.Message);
            Assert.AreEqual(4, validationEventArgs.ValidationError.Value);
        }

        [Test]
        public void ArrayCountLessThanMinimumItems()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""minItems"":2,
  ""maxItems"":3
}";

            string json = "[null]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Null, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.AreEqual("Array item count 1 is less than minimum count of 2. Path '', line 1, position 6.", validationEventArgs.Message);
            Assert.AreEqual(1, validationEventArgs.ValidationError.Value);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void InvalidDataType()
        {
            string schemaJson = @"{
  ""type"":""string"",
  ""minItems"":2,
  ""maxItems"":3,
  ""items"":{}
}";

            string json = "[null,null,null,null]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);
            Assert.AreEqual(@"Invalid type. Expected String but got Array. Path '', line 1, position 1.", validationEventArgs.Message);
            Assert.AreEqual(null, validationEventArgs.ValidationError.Value);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void StringDisallowed()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""disallow"":[""number""]
  },
  ""maxItems"":3
}";

            string json = "['pie',1.1]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Float, reader.TokenType);

            Assert.IsNotNull(validationEventArgs);
            Assert.AreEqual(@"JSON is valid against schema from 'not'. Path '[1]', line 1, position 10.", validationEventArgs.Message);
            Assert.AreEqual(null, validationEventArgs.ValidationError.Value);
            Assert.AreEqual(0, validationEventArgs.ValidationError.ChildErrors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
        }

        [Test]
        public void MissingRequiredProperties()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string""},
    ""hobbies"":{""type"":""string"",""required"":true},
    ""age"":{""type"":""integer"",""required"":true}
  }
}";

            string json = "{'name':'James'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("name", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
            Assert.AreEqual("Required properties are missing from object: hobbies, age. Path '', line 1, position 16.", validationEventArgs.Message);
            Assert.AreEqual("", validationEventArgs.Path);
            Assert.AreEqual("hobbies", ((IList<string>)validationEventArgs.ValidationError.Value)[0]);
            Assert.AreEqual("age", ((IList<string>)validationEventArgs.ValidationError.Value)[1]);

            Assert.IsNotNull(validationEventArgs);
        }

        [Test]
        public void MissingNonRequiredProperties()
        {
            string schemaJson = @"{
  ""description"":""A person"",
  ""type"":""object"",
  ""properties"":
  {
    ""name"":{""type"":""string"",""required"":true},
    ""hobbies"":{""type"":""string"",""required"":false},
    ""age"":{""type"":""integer""}
  }
}";

            string json = "{'name':'James'}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("name", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("James", reader.Value.ToString());
            Assert.IsNull(validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsNull(validationEventArgs);
        }

        [Test]
        public void DisableAdditionalProperties()
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

            string json = "{'name':'James','additionalProperty1':null,'additionalProperty2':null}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("name", reader.Value.ToString());

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
        public void ExtendsStringGreaterThanMaximumLength()
        {
            string schemaJson = @"{
  ""extends"":{
    ""type"":""string"",
    ""minLength"":5,
    ""maxLength"":10
  },
  ""maxLength"":9
}";

            List<ValidationError> errors = new List<ValidationError>();
            string json = "'The quick brown fox jumps over the lazy dog.'";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) =>
            {
                validationEventArgs = args;
                errors.Add(validationEventArgs.ValidationError);
            };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual(2, errors.Count);
            StringAssert.AreEqual(@"JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path '', line 1, position 46.", errors[0].GetExtendedMessage());
            Assert.AreEqual(null, errors[0].Value);
            Assert.AreEqual(1, errors[0].ChildErrors.Count);
            StringAssert.AreEqual(@"String 'The quick brown fox jumps over the lazy dog.' exceeds maximum length of 10. Path '', line 1, position 46.", errors[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", errors[0].ChildErrors[0].Value);
            Assert.AreEqual("String 'The quick brown fox jumps over the lazy dog.' exceeds maximum length of 9. Path '', line 1, position 46.", errors[1].GetExtendedMessage());
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", errors[1].Value);

            Assert.IsNotNull(validationEventArgs);
        }

        private JSchema GetExtendedSchema()
        {
            string first = @"{
  ""type"":""object"",
  ""properties"":
  {
    ""firstproperty"":{""type"":""string"",""required"":true}
  },
  ""additionalProperties"":{}
}";

            string second = @"{
  ""id"":""second"",
  ""type"":""object"",
  ""extends"":{""$ref"":""first""},
  ""properties"":
  {
    ""secondproperty"":{""type"":""string"",""required"":true}
  },
  ""additionalProperties"":false
}";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            JSchema firstSchema = JSchema.Parse(first);

            resolver.Add(new Uri("first", UriKind.RelativeOrAbsolute), firstSchema.ToString());

            JSchema secondSchema = JSchema.Parse(second, resolver);

            return secondSchema;
        }

        [Test]
        public void ExtendsDisallowAdditionProperties()
        {
            string json = "{'firstproperty':'blah','secondproperty':'blah2','additional':'blah3','additional2':'blah4'}";

            IList<SchemaValidationEventArgs> errors;
            JSchemaValidatingReader reader = CreateReader(json, GetExtendedSchema(), out errors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("firstproperty", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);
            StringAssert.AreEqual(@"Property 'firstproperty' has not been defined and the schema does not allow additional properties. Path 'firstproperty', line 1, position 17.", errors[0].Message);
            Assert.AreEqual("firstproperty", errors[0].ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("blah", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("secondproperty", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("blah2", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("additional", reader.Value.ToString());
            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Property 'additional' has not been defined and the schema does not allow additional properties. Path 'additional', line 1, position 62.", errors[1].Message);
            Assert.AreEqual("additional", errors[1].ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("blah3", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("additional2", reader.Value.ToString());
            Assert.AreEqual(3, errors.Count);
            Assert.AreEqual("Property 'additional2' has not been defined and the schema does not allow additional properties. Path 'additional2', line 1, position 84.", errors[2].Message);
            Assert.AreEqual("additional2", errors[2].ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("blah4", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void ExtendsMissingRequiredProperties()
        {
            string json = "{}";

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { errors.Add(args.ValidationError); };
            reader.Schema = GetExtendedSchema();

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.AreEqual(2, errors.Count);
            StringAssert.AreEqual(@"JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path '', line 1, position 2.", errors[0].GetExtendedMessage());
            Assert.AreEqual(null, errors[0].Value);

            Assert.AreEqual(1, errors[0].ChildErrors.Count);
            StringAssert.AreEqual(@"Required properties are missing from object: firstproperty. Path '', line 1, position 2.", errors[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual(new List<string> { "firstproperty" }, errors[0].ChildErrors[0].Value);

            StringAssert.AreEqual("Required properties are missing from object: secondproperty. Path '', line 1, position 2.", errors[1].GetExtendedMessage());
            Assert.AreEqual(new List<string> { "secondproperty" }, errors[1].Value);
            Assert.AreEqual("second", errors[1].SchemaId.ToString());
        }

        [Test]
        public void NoAdditionalItems()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"": [{""type"":""string""},{""type"":""integer""}],
  ""additionalItems"": false
}";

            string json = @"[1, 'a', 1234]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Invalid type. Expected String but got Integer. Path '[0]', line 1, position 2.", validationEventArgs.Message);
            Assert.AreEqual(1L, validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("Invalid type. Expected Integer but got String. Path '[1]', line 1, position 7.", validationEventArgs.Message);
            Assert.AreEqual("a", validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Index 3 has not been defined and the schema does not allow additional items. Path '[2]', line 1, position 13.", validationEventArgs.Message);
            Assert.AreEqual(1234L, validationEventArgs.ValidationError.Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);

            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void PatternPropertiesNoAdditionalProperties()
        {
            string schemaJson = @"{
  ""type"":""object"",
  ""patternProperties"": {
     ""hi"": {""type"":""string""},
     ""ho"": {""type"":""string""}
  },
  ""additionalProperties"": false
}";

            string json = @"{
  ""hi"": ""A string!"",
  ""hide"": ""A string!"",
  ""ho"": 1,
  ""hey"": ""A string!""
}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual("Invalid type. Expected String but got Integer. Path 'ho', line 4, position 9.", validationEventArgs.Message);
            Assert.AreEqual(1L, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("#/patternProperties/ho", validationEventArgs.ValidationError.SchemaId.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("Property 'hey' has not been defined and the schema does not allow additional properties. Path 'hey', line 5, position 8.", validationEventArgs.Message);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

            Assert.IsFalse(reader.Read());
        }

        [Test]
        public void ExtendedComplex()
        {
            string first = @"{
  ""id"":""first"",
  ""type"":""object"",
  ""properties"":
  {
    ""firstproperty"":{""type"":""string""},
    ""secondproperty"":{""type"":""string"",""maxLength"":10},
    ""thirdproperty"":{
      ""type"":""object"",
      ""properties"":
      {
        ""thirdproperty_firstproperty"":{""type"":""string"",""maxLength"":10,""minLength"":7}
      }
    }
  },
  ""additionalProperties"":{}
}";

            string second = @"{
  ""id"":""second"",
  ""type"":""object"",
  ""extends"":{""$ref"":""first""},
  ""properties"":
  {
    ""secondproperty"":{""type"":""any""},
    ""thirdproperty"":{
      ""extends"":{
        ""properties"":
        {
          ""thirdproperty_firstproperty"":{""maxLength"":9,""minLength"":6,""pattern"":""hi2u""}
        },
        ""additionalProperties"":{""maxLength"":9,""minLength"":6,""enum"":[""one"",""two""]}
      },
      ""type"":""object"",
      ""properties"":
      {
        ""thirdproperty_firstproperty"":{""pattern"":""hi""}
      },
      ""additionalProperties"":{""type"":""string"",""enum"":[""two"",""three""]}
    },
    ""fourthproperty"":{""type"":""string""}
  },
  ""additionalProperties"":false
}";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            JSchema firstSchema = JSchema.Parse(first);
            resolver.Add(firstSchema.Id, firstSchema.ToString());

            JSchema secondSchema = JSchema.Parse(second, resolver);

            string json = @"{
  'firstproperty':'blahblahblahblahblahblah',
  'secondproperty':'secasecasecasecaseca',
  'thirdproperty':{
    'thirdproperty_firstproperty':'aaa',
    'additional':'three'
  }
}";

            SchemaValidationEventArgs validationEventArgs = null;
            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) =>
            {
                validationEventArgs = args;
                errors.Add(validationEventArgs.ValidationError);
            };
            reader.Schema = secondSchema;

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("firstproperty", reader.Value.ToString());
            StringAssert.AreEqual(@"firstproperty", errors[0].Path);
            StringAssert.AreEqual(@"Property 'firstproperty' has not been defined and the schema does not allow additional properties. Path 'firstproperty', line 2, position 18.", errors[0].GetExtendedMessage());
            Assert.AreEqual("firstproperty", errors[0].Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("blahblahblahblahblahblah", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("secondproperty", reader.Value.ToString());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("secasecasecasecaseca", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("thirdproperty", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("thirdproperty_firstproperty", reader.Value.ToString());
            Assert.AreEqual(1, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("aaa", reader.Value.ToString());
            Assert.AreEqual(2, errors.Count);
            StringAssert.AreEqual(@"thirdproperty.thirdproperty_firstproperty", errors[1].Path);
            StringAssert.AreEqual(@"String 'aaa' does not match regex pattern 'hi'. Path 'thirdproperty.thirdproperty_firstproperty', line 5, position 39.", errors[1].GetExtendedMessage());
            Assert.AreEqual("aaa", errors[1].Value);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
            Assert.AreEqual("additional", reader.Value.ToString());
            Assert.AreEqual(2, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.String, reader.TokenType);
            Assert.AreEqual("three", reader.Value.ToString());
            Assert.AreEqual(2, errors.Count);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
            Assert.AreEqual(3, errors.Count);
            StringAssert.AreEqual(@"thirdproperty", errors[2].Path);
            StringAssert.AreEqual(@"JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path 'thirdproperty', line 7, position 3.", errors[2].GetExtendedMessage());
            Assert.AreEqual(null, errors[2].Value);
            Assert.AreEqual(4, errors[2].ChildErrors.Count);
            StringAssert.AreEqual(@"thirdproperty.thirdproperty_firstproperty", errors[2].ChildErrors[0].Path);
            StringAssert.AreEqual(@"String 'aaa' is less than minimum length of 6. Path 'thirdproperty.thirdproperty_firstproperty', line 5, position 39.", errors[2].ChildErrors[0].GetExtendedMessage());

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
            Assert.AreEqual(4, errors.Count);
            StringAssert.AreEqual(@"", errors[3].Path);
            StringAssert.AreEqual(@"JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path '', line 8, position 1.", errors[3].GetExtendedMessage());
            Assert.AreEqual(null, errors[3].Value);
            Assert.AreEqual(2, errors[3].ChildErrors.Count);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(4, errors.Count);
        }

        [Test]
        public void DuplicateErrorsTest()
        {
            string schema = @"{
  ""id"":""ErrorDemo.Database"",
  ""properties"":{
    ""ErrorDemoDatabase"":{
      ""type"":""object"",
      ""required"":true,
      ""properties"":{
        ""URL"":{
          ""type"":""string"",
          ""required"":true
        },
        ""Version"":{
          ""type"":""string"",
          ""required"":true
        },
        ""Date"":{
          ""type"":""string"",
          ""format"":""date-time"",
          ""required"":true
        },
        ""MACLevels"":{
          ""type"":""object"",
          ""required"":true,
          ""properties"":{
            ""MACLevel"":{
              ""type"":""array"",
              ""required"":true,
              ""items"":[
                {
                  ""required"":true,
                  ""properties"":{
                    ""IDName"":{
                      ""type"":""string"",
                      ""required"":true
                    },
                    ""Order"":{
                      ""type"":""string"",
                      ""required"":true
                    },
                    ""IDDesc"":{
                      ""type"":""string"",
                      ""required"":true
                    },
                    ""IsActive"":{
                      ""type"":""string"",
                      ""required"":true
                    }
                  }
                }
              ]
            }
          }
        }
      }
    }
  }
}";

            string json = @"{
  ""ErrorDemoDatabase"":{
    ""URL"":""localhost:3164"",
    ""Version"":""1.0"",
    ""Date"":""6.23.2010, 9:35:18.121"",
    ""MACLevels"":{
      ""MACLevel"":[
        {
          ""@IDName"":""Developer"",
          ""Order"":""0"",
          ""IDDesc"":""DeveloperDesc"",
          ""IsActive"":""True""
        },
        {
          ""IDName"":""Technician"",
          ""Order"":""1"",
          ""IDDesc"":""TechnicianDesc"",
          ""IsActive"":""True""
        },
        {
          ""IDName"":""Administrator"",
          ""Order"":""2"",
          ""IDDesc"":""AdministratorDesc"",
          ""IsActive"":""True""
        },
        {
          ""IDName"":""PowerUser"",
          ""Order"":""3"",
          ""IDDesc"":""PowerUserDesc"",
          ""IsActive"":""True""
        },
        {
          ""IDName"":""Operator"",
          ""Order"":""4"",
          ""IDDesc"":""OperatorDesc"",
          ""IsActive"":""True""
        }
      ]
    }
  }
}";

            IList<SchemaValidationEventArgs> validationEventArgs = new List<SchemaValidationEventArgs>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs.Add(args); };
            reader.Schema = JSchema.Parse(schema);

            while (reader.Read())
            {
            }

            Assert.AreEqual(2, validationEventArgs.Count);
        }

        [Test]
        public void ReadAsBytes()
        {
            JSchema s = new JSchemaGenerator().Generate(typeof(byte[]));

            byte[] data = Encoding.UTF8.GetBytes("Hello world");

            JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"""" + Convert.ToBase64String(data) + @"""")))
            {
                Schema = s
            };
            byte[] bytes = reader.ReadAsBytes();

            CollectionAssert.AreEquivalent(data, bytes);
        }

        [Test]
        public void ReadAsBoolean_Failure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema s = new JSchemaGenerator().Generate(typeof(bool));
                s.Enum.Add(new JValue(false));

                JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"true")))
                {
                    Schema = s
                };
                reader.ReadAsBoolean();
            }, "Value true is not defined in enum. Path '', line 1, position 4.");
        }

        [Test]
        public void ReadAsBoolean()
        {
            JSchema s = new JSchemaGenerator().Generate(typeof(bool));

            JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"true")))
            {
                Schema = s
            };
            bool? b = reader.ReadAsBoolean();

            Assert.AreEqual(true, b);
        }

        [Test]
        public void ReadAsInt32()
        {
            JSchema s = new JSchemaGenerator().Generate(typeof(int));

            JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"1")))
            {
                Schema = s
            };
            int? i = reader.ReadAsInt32();

            Assert.AreEqual(1, i);
        }

        [Test]
        public void ReadAsInt32Failure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema s = new JSchemaGenerator().Generate(typeof(int));
                s.Maximum = 2;

                JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"5")))
                {
                    Schema = s
                };
                reader.ReadAsInt32();
            }, "Integer 5 exceeds maximum value of 2. Path '', line 1, position 1.");
        }

        [Test]
        public void ReadAsDouble()
        {
            JSchema s = new JSchemaGenerator().Generate(typeof(double));

            JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"1.0")))
            {
                Schema = s
            };
            double? d = reader.ReadAsDouble();

            Assert.AreEqual(1d, d);
        }

        [Test]
        public void ReadAsDoubleFailure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema s = new JSchemaGenerator().Generate(typeof(double));
                s.Maximum = 2;

                JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"5.0")))
                {
                    Schema = s
                };
                reader.ReadAsDouble();
            }, "Float 5.0 exceeds maximum value of 2. Path '', line 1, position 3.");
        }

        [Test]
        public void ReadAsDecimal()
        {
            JSchema s = new JSchemaGenerator().Generate(typeof(decimal));

            JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"1.5")))
            {
                Schema = s
            };
            decimal? d = reader.ReadAsDecimal();

            Assert.AreEqual(1.5m, d);
        }

        [Test]
        public void ReadAsDecimalFailure()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema s = new JSchemaGenerator().Generate(typeof(decimal));
                s.MultipleOf = 1;

                JsonReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(@"5.5")))
                {
                    Schema = s
                };
                reader.ReadAsDecimal();
            }, "Float 5.5 is not a multiple of 1. Path '', line 1, position 3.");
        }

        [Test]
        public void ReadAsInt32FromSerializer()
        {
            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader("[1,2,3]")));
            reader.Schema = new JSchemaGenerator().Generate(typeof(int[]));
            int[] values = new JsonSerializer().Deserialize<int[]>(reader);

            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void ReadAsInt32InArray()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""integer""
  },
  ""maxItems"":1
}";

            string json = "[1,2]";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            reader.Read();
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.EndArray, reader.TokenType);
            Assert.AreEqual("Array item count 2 exceeds maximum count of 1. Path '', line 1, position 5.", validationEventArgs.Message);
            Assert.AreEqual(2, validationEventArgs.ValidationError.Value);
            Assert.AreEqual("", validationEventArgs.Path);
        }

        [Test]
        public void ReadAsInt32InArrayIncomplete()
        {
            string schemaJson = @"{
  ""type"":""array"",
  ""items"":{
    ""type"":""integer""
  },
  ""maxItems"":1
}";

            string json = "[1,2";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schemaJson);

            reader.Read();
            Assert.AreEqual(JsonToken.StartArray, reader.TokenType);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.Integer, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            reader.ReadAsInt32();
            Assert.AreEqual(JsonToken.None, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);
        }

        [Test]
        public void SchemaPath()
        {
            string schema = @"{
   ""$schema"" : ""http://json-schema.org/draft-04/schema#"",
   ""title"" : ""listing/update"",
   ""type"" : ""object"",
   ""id"" : ""http://test.com/update.json"",
   ""required"" : [""derp""]
}";

            SchemaValidationEventArgs validationEventArgs = null;

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader("{}")));
            reader.ValidationEventHandler += (sender, args) => { validationEventArgs = args; };
            reader.Schema = JSchema.Parse(schema);

            reader.Read();
            Assert.AreEqual(JsonToken.StartObject, reader.TokenType);
            Assert.AreEqual(null, validationEventArgs);

            reader.Read();
            Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
            Assert.AreNotEqual(null, validationEventArgs);
        }

        [Test]
        public void InvalidPatternPropertyRegex()
        {
            string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema"",
  ""definitions"": {
    ""pais"": {
      ""type"": ""object""
    }
  },
  ""properties"": {
    ""$schema"": {
      ""type"": ""string""
    },
    ""paises"": {
      ""patternProperties"": {
        ""[]"": {
          ""$ref"": ""#/definitions/pais""
        }
      }
    }
  }
}";

            string json = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema"",
  ""paises"": {
    ""name"": ""Test"",
    ""age"": 52
  }
}";

            JSchema schema = JSchema.Parse(schemaJson);

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (o, e) => errors.Add(e.ValidationError);
            reader.Schema = schema;

            while (reader.Read())
            {
            }

            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(
                errors[0].Message == @"Could not test property names with regex pattern '[]'. There was an error parsing the regex: parsing ""[]"" - Unterminated [] set." ||
                errors[0].Message == @"Could not test property names with regex pattern '[]'. There was an error parsing the regex: parsing '[]' - Unterminated [] set.", errors[0].Message);
            Assert.AreEqual(new Uri("#/properties/paises", UriKind.Relative), errors[0].SchemaId);
            Assert.AreEqual(ErrorType.PatternProperties, errors[0].ErrorType);
        }

#if !DNXCORE50
        [Test]
        [Ignore("WIP")]
        public void DateFormat()
        {
            string schemaJson = "{\"type\":\"object\",\"properties\":{\"DueDate\":{\"required\":true,\"type\":\"string\",\"format\":\"date\"},\"DateCompleted\":{\"required\":true,\"type\":\"string\",\"format\":\"date-time\"}}}";

            string json = "{\"DueDate\":\"2015-08-25\",\"DateCompleted\":\"2015-08-27T22:40:09.3749084-05:00\"}";

            IList<string> errors = new List<string>();
            var schema = JSchema.Parse(schemaJson);

            var jsonReader = new JsonTextReader(new StringReader(json));
            var validatingReader = new JSchemaValidatingReader(jsonReader);
            validatingReader.Schema = schema;
            validatingReader.ValidationEventHandler += (o, a) => errors.Add(a.Path + ": " + a.Message);

            var serializer = new JsonSerializer();
            var hw = serializer.Deserialize<Homework>(validatingReader);

            Assert.AreEqual(0, errors.Count);
        }
#endif

        public class Homework
        {
            public DateTime DueDate { get; set; }
            public DateTime DateCompleted { get; set; }
        }

        [Test]
        public void ValidateConstructorAndUndefinedWithNoType()
        {
            string schemaJson = @"{

""$schema"": ""http://json-schema.org/draft-04/schema#"",
""title"": ""car"",
""description"": ""Attributes of car"",
""type"": ""object"",
""required"": [ ""model"", ""color"" ],
""properties"": {
      
      
	""model"": {""type"": ""string""},
	""firstRegistration"": {""format"": ""date-time""},
	""model"": { ""type"": ""string""},
	""power"": {""allOf"":
        	[
		{""type"": ""number""},
		{""maxLength"": 4}
		]
	}
}
}";

            string json = @"{
	model: ""volkswagen"",
	color: ""blue"",
   firstRegistration:,
   firstRegistration:new Date(),
	power: 999
}";

            JSchema schema = JSchema.Parse(schemaJson);

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (o, e) => errors.Add(e.ValidationError);
            reader.Schema = schema;

            while (reader.Read())
            {
            }

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void ValidateUndefinedWithType()
        {
            string schemaJson = @"{

""$schema"": ""http://json-schema.org/draft-04/schema#"",
""title"": ""car"",
""description"": ""Attributes of car"",
""type"": ""object"",
""required"": [ ""model"", ""color"" ],
""properties"": {
      
      
	""model"": {""type"": ""string""},
	""firstRegistration"": {""format"": ""date-time"",""type"": ""string""},
	""model"": { ""type"": ""string""},
	""power"": {""allOf"":
        	[
		{""type"": ""number""},
		{""maxLength"": 4}
		]
	}
}
}";

            string json = @"{
	model: ""volkswagen"",
	color: ""blue"",
   firstRegistration:,
   firstRegistration:new Date(),
	power: 999
}";

            JSchema schema = JSchema.Parse(schemaJson);

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (o, e) => errors.Add(e.ValidationError);
            reader.Schema = schema;

            while (reader.Read())
            {
            }

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Invalid type. Expected String but got Undefined.", errors[0].Message);
            Assert.AreEqual(null, errors[0].Value);
            Assert.AreEqual("Invalid type. Expected String but got Constructor.", errors[1].Message);
            Assert.AreEqual("Date", errors[1].Value);
        }

        [Test]
        public void ValidateMaxPropertiesInArray()
        {
            string schemaJson = @"{
   ""$schema"":""http://json-schema.org/draft-04/schema#"",
   ""type"":""array"",
   ""maxItems"":5,
   ""items"":{
      ""$ref"":""#/definitions/test""
   },
   ""definitions"":{
      ""test"":{
         ""type"":""object"",
         ""additionalProperties"":false,
         ""maxProperties"":1,
         ""properties"":{
            ""a"":{
               ""type"":""string""
            },
            ""b"":{
               ""type"":""string""
            }
         }
      }
   }
}";

            string json = @"[
  {""a"":""a""},
  {""b"":""b""},
]";

            JSchema schema = JSchema.Parse(schemaJson);

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (o, e) => errors.Add(e.ValidationError);
            reader.Schema = schema;

            while (reader.Read())
            {
            }

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void DeeplyNestedConditionalScopes()
        {
            string schemaJson = TestHelpers.OpenFileText(@"resources\schemas\components-10definitions.schema.json");

            string json = TestHelpers.OpenFileText(@"resources\json\components-5levels.json");

            JSchema schema = JSchema.Parse(schemaJson);

            List<ValidationError> errors = new List<ValidationError>();

            JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            reader.ValidationEventHandler += (o, e) => errors.Add(e.ValidationError);
            reader.Schema = schema;

            while (reader.Read())
            {
            }

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void CloseAlsoClosesUnderlyingReader()
        {
            var underlyingReader = new JsonReaderStubWithIsClosed();
            var validatingReader = new JSchemaValidatingReader(underlyingReader) { CloseInput = true };

            validatingReader.Close();

            Assert.IsTrue(underlyingReader.IsClosed);
        }
    }

    public sealed class JsonReaderStubWithIsClosed : JsonReader
    {
        public bool IsClosed { get; private set; }

        public override void Close()
        {
            IsClosed = true;
        }

        public override bool Read()
        {
            throw new NotSupportedException();
        }
    }
}