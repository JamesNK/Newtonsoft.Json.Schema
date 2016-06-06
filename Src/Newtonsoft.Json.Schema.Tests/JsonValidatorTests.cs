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
            Assert.AreEqual("#/properties/values/items/0", errors[0].SchemaId.OriginalString);

            Assert.AreEqual(@"String is not JSON: Unexpected character encountered while parsing value: b. Path 'prop1', line 1, position 9.", errors[1].Message);
            Assert.AreEqual(ErrorType.Validator, errors[1].ErrorType);
            Assert.AreEqual("#/properties/values/items/0", errors[1].SchemaId.OriginalString);
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
    }
}