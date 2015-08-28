using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class SerializationTests : TestFixtureBase
    {
        [Test]
        public void SerializeSchema()
        {
            JSchema schema = new JSchema
            {
                Properties =
                {
                    { "first", new JSchema { Type = JSchemaType.String | JSchemaType.Null } }
                }
            };

            string s1 = schema.ToString();
            string s2 = JsonConvert.SerializeObject(schema, Formatting.Indented);

            Assert.AreEqual(s1, s2);
        }

        public class DummyLineInfo : IJsonLineInfo
        {
            public bool HasLineInfo()
            {
                return true;
            }

            public int LineNumber
            {
                get { return 11; }
            }

            public int LinePosition
            {
                get { return 5; }
            }
        }

        [Test]
        public void SerializeError()
        {
            ValidationError error = ValidationError.CreateValidationError(
                    message: $"A message!",
                    errorType: ErrorType.MinimumLength,
                    schema: new JSchema
                    {
                        BaseUri = new Uri("test.xml", UriKind.RelativeOrAbsolute),
                        Type = JSchemaType.Number
                    },
                    schemaId: new Uri("test.xml", UriKind.RelativeOrAbsolute),
                    value: "A value!",
                    childErrors: new List<ValidationError>
                    {
                        ValidationError.CreateValidationError($"Child message!", ErrorType.None, new JSchema(), null, null, null, null, null)
                    },
                    lineInfo: new DummyLineInfo(),
                    path: "sdf.sdf");

            string json = JsonConvert.SerializeObject(error, Formatting.Indented);

            StringAssert.AreEqual(@"{
  ""Message"": ""A message!"",
  ""LineNumber"": 11,
  ""LinePosition"": 5,
  ""Path"": ""sdf.sdf"",
  ""Value"": ""A value!"",
  ""SchemaId"": ""test.xml"",
  ""SchemaBaseUri"": ""test.xml"",
  ""ErrorType"": ""minLength"",
  ""ChildErrors"": [
    {
      ""Message"": ""Child message!"",
      ""LineNumber"": 0,
      ""LinePosition"": 0,
      ""Path"": null,
      ""Value"": null,
      ""SchemaId"": null,
      ""SchemaBaseUri"": null,
      ""ErrorType"": ""none"",
      ""ChildErrors"": []
    }
  ]
}", json);
        }
    }
}