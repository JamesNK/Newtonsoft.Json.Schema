#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
    public class Issue0134Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(SchemaJson);

            JToken t = JToken.Parse(Json);

            Assert.AreEqual(false, t.IsValid(s, out IList<ValidationError> errorMessages));

            Assert.AreEqual(1, errorMessages.Count);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'. Path '', line 1, position 1.", errorMessages[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("http://example.com/schemas/example/1.0/schema.json", UriKind.RelativeOrAbsolute), errorMessages[0].SchemaId);

            Assert.AreEqual(1, errorMessages[0].ChildErrors.Count);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'. Path '', line 1, position 1.", errorMessages[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("http://example.com/schemas/example/1.0/schema.json#/definitions/anyObject", UriKind.RelativeOrAbsolute), errorMessages[0].ChildErrors[0].SchemaId);

            Assert.AreEqual(2, errorMessages[0].ChildErrors[0].ChildErrors.Count);
            Assert.AreEqual("JSON does not match schema from 'then'. Path '', line 1, position 1.", errorMessages[0].ChildErrors[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("http://example.com/schemas/example/1.0/schema.json#/definitions/typeB", UriKind.RelativeOrAbsolute), errorMessages[0].ChildErrors[0].ChildErrors[0].SchemaId);

            Assert.AreEqual("JSON does not match schema from 'else'. Path '', line 1, position 1.", errorMessages[0].ChildErrors[0].ChildErrors[1].GetExtendedMessage());
            Assert.AreEqual(new Uri("http://example.com/schemas/example/1.0/schema.json#/definitions/anyObject/anyOf/0/else", UriKind.RelativeOrAbsolute), errorMessages[0].ChildErrors[0].ChildErrors[1].SchemaId);
        }

        #region JSON
        private const string Json = @"{
  ""type"": ""typeB"",
  ""body"":{
     ""$computed"":""first"",
     ""values"":[]
  }
}";
        #endregion

        #region Schema
        private const string SchemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""http://example.com/schemas/example/1.0/schema.json"",
  ""anyOf"": [
    {
      ""$ref"": ""#/definitions/anyObject""
    }
  ],
  ""definitions"": {
    ""anyObject"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""type"": ""string""
        }
      },
      ""required"": [
        ""type""
      ],
      ""anyOf"": [
        {
          ""if"": {
            ""properties"": {
              ""type"": {
                ""const"": ""typeA""
              }
            }
          },
          ""then"": {
            ""$ref"": ""#/definitions/typeA""
          },
          ""else"": false
        },
        {
          ""if"": {
            ""properties"": {
              ""type"": {
                ""const"": ""typeB""
              }
            }
          },
          ""then"": {
            ""$ref"": ""#/definitions/typeB""
          },
          ""else"": false
        }
      ]
    },
    ""bodyDefinition"": {
      ""oneOf"": [
        {
          ""if"": {
            ""properties"": {
              ""$computed"": {
                ""type"": ""string""
              }
            },
            ""required"": [
              ""$computed""
            ]
          },
          ""then"": {
            ""$ref"": ""#/definitions/computedBody""
          },
          ""else"": {
            ""$ref"": ""#/definitions/wildcardBody""
          }
        }
      ]
    },
    ""wildcardBody"": {
      ""type"": ""object"",
      ""additionalProperties"": {
        ""$ref"": ""#/definitions/bodyDefinition""
      }
    },
    ""firstComputedValue"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""$computed"": {
          ""const"": ""first""
        },
        ""values"": {
          ""type"": ""array"",
          ""minItems"": 1,
          ""items"": {
            ""$ref"": ""#/definitions/bodyDefinition""
          }
        }
      },
      ""required"": [
        ""$computed"",
        ""values""
      ]
    },
    ""computedBody"": {
      ""if"": {
        ""properties"": {
          ""$computed"": {
            ""const"": ""first""
          }
        }
      },
      ""then"": false,
      ""else"": false
    },
    ""typeA"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""type"": ""string"",
          ""const"": ""typeA""
        },
        ""body"": {
          ""$ref"": ""#/definitions/bodyDefinition""
        }
      },
      ""required"": [
        ""type""
      ]
    },
    ""typeB"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""type"": ""string"",
          ""const"": ""typeB""
        },
        ""body"": {
          ""$ref"": ""#/definitions/bodyDefinition""
        }
      },
      ""required"": [
        ""type""
      ]
    }
  }
}";
        #endregion
    }
}