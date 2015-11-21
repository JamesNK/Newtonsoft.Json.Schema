#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    public class JsonValidatorTests
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
                Validators = new List<JsonValidator> {new UniqueKeyValidator()}
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

            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }

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
    }
}