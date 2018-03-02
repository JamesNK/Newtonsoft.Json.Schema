#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0127Tests : TestFixtureBase
    {
        [Test]
        public void Test_IfThenElseNestedSchemas()
        {
            JObject o = JObject.Parse(@"{
  ""type"": ""generic2"",
  ""content"": [
    {
      ""type"": ""generic3""
    }
  ]
}");
            JSchema s = JSchema.Parse(IfThenElseNestedSchemas);

            Assert.IsFalse(o.IsValid(s, out IList<ValidationError> errors));
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errors[0].Message);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errors[0].ChildErrors[0].Message);

            Assert.AreEqual(2, errors[0].ChildErrors[0].ChildErrors.Count);

            Assert.AreEqual("JSON does not match schema from 'then'.", errors[0].ChildErrors[0].ChildErrors[0].Message);
            Assert.AreEqual(new Uri("foo#/definitions/generic2", UriKind.RelativeOrAbsolute), errors[0].ChildErrors[0].ChildErrors[0].SchemaId);

            Assert.AreEqual("JSON does not match schema from 'else'.", errors[0].ChildErrors[0].ChildErrors[1].Message);
            Assert.AreEqual(new Uri("foo#/definitions/generic/anyOf/0/else", UriKind.RelativeOrAbsolute), errors[0].ChildErrors[0].ChildErrors[1].SchemaId);
        }

        [Test]
        public void Test_NestedConditionalSchemas()
        {
            JObject o = JObject.Parse(@"{
  ""type"": ""generic2"",
  ""content"": [
    {
      ""type"": ""generic3""
    }
  ]
}");
            JSchema s = JSchema.Parse(NestedConditionalSchemas);

            Assert.IsFalse(o.IsValid(s, out IList<ValidationError> errors));

            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errors[0].Message);
            Assert.AreEqual("JSON does not match any schemas from 'anyOf'.", errors[0].ChildErrors[0].Message);

            Assert.AreEqual(2, errors[0].ChildErrors[0].ChildErrors.Count);
        }

        #region IfThenElseNestedSchemas
        private const string IfThenElseNestedSchemas = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""foo"",
  ""anyOf"": [
    { ""$ref"": ""#/definitions/generic"" }
  ],
  ""definitions"": {
    ""generic"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""type"": ""string""
        }
      },
      ""required"": [""type""],
      ""anyOf"": [
        {
          ""if"": {""properties"": {""type"": {""const"": ""generic1""}}},
          ""then"": { ""$ref"": ""#/definitions/generic1"" },
          ""else"": false
        },
        {
          ""if"": {""properties"": {""type"": {""const"": ""generic2""}}},
          ""then"": { ""$ref"": ""#/definitions/generic2"" },
          ""else"": false
        }
      ]
    },
    ""generic1"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""const"": ""generic1""
        },
        ""content"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/generic""
          },
          ""additionalItems"": false
        }
      },
      ""required"": [
        ""type""
      ]
    },
    ""generic2"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""const"": ""generic2""
        },
        ""content"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/generic""
          },
          ""additionalItems"": false
        }
      },
      ""required"": [
        ""type""
      ]
    }
  }
}";
        #endregion

        #region NestedConditionalSchemas
        private const string NestedConditionalSchemas = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""foo"",
  ""anyOf"": [
    { ""$ref"": ""#/definitions/generic"" }
  ],
  ""definitions"": {
    ""generic"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""type"": ""string""
        }
      },
      ""required"": [""type""],
      ""anyOf"": [
        {
          ""allOf"": [{""properties"": {""type"": {""const"": ""generic1""}}}, { ""$ref"": ""#/definitions/generic1"" }]
        },
        {
          ""allOf"": [{""properties"": {""type"": {""const"": ""generic2""}}}, { ""$ref"": ""#/definitions/generic2"" }]
        }
      ]
    },
    ""generic1"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""const"": ""generic1""
        },
        ""content"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/generic""
          },
          ""additionalItems"": false
        }
      },
      ""required"": [
        ""type""
      ]
    },
    ""generic2"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""properties"": {
        ""type"": {
          ""const"": ""generic2""
        },
        ""content"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/generic""
          },
          ""additionalItems"": false
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