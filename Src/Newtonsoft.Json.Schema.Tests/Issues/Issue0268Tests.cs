#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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
    public class Issue0268Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            var resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("https://a.b.c/schemas/definitions.schema.json"), SchemaRef);

            JSchema schema = JSchema.Parse(Schema, resolver);

            Assert.AreEqual("https://a.b.c/schemas/message-base.schema.json", schema.Id.OriginalString);
            Assert.AreEqual("https://a.b.c/schemas/definitions.schema.json", schema.Properties["gauges"].BaseUri.OriginalString);
            Assert.AreEqual("https://a.b.c/schemas/definitions.schema.json", schema.Properties["gauges"].PatternProperties[".*"].OneOf[1].BaseUri.OriginalString);
        }

        private const string Schema = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""https://a.b.c/schemas/message-base.schema.json"",
  ""type"": ""object"",
  ""properties"": {
    ""gauges"": {
      ""$ref"": ""https://a.b.c/schemas/definitions.schema.json#string-gauge-dict""
    }
  },
  ""required"": [
    ""type""
  ]
}";

        private const string SchemaRef = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""$id"": ""https://a.b.c/schemas/definitions.schema.json"",
  ""title"": ""Definitions"",
  ""definitions"": {
    ""loglevel"": {
      ""$id"": ""#loglevel"",
      ""type"": ""string"",
      ""enum"": [
        ""verbose"",
        ""debug"",
        ""info"",
        ""warn"",
        ""error"",
        ""fatal""
      ]
    },
    ""span-id"": {
      ""$id"": ""#span-id"",
      ""type"": ""string""
    },
    ""trace-id"": {
      ""$id"": ""#trace-id"",
      ""type"": ""string""
    },
    ""gauge-regular"": {
      ""$id"": ""#gauge-regular"",
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""type"": ""string"",
          ""const"": ""gauge""
        },
        ""value"": {
          ""$ref"": ""#value""
        },
        ""unit"": {
          ""$ref"": ""#unit""
        }
      },
      ""required"": [
        ""type"",
        ""value"",
        ""unit""
      ]
    },
    ""gauge"": {
      ""$id"": ""#gauge"",
      ""oneOf"": [
        {
          ""$ref"": ""#gauge-regular""
        },
        {
          ""$ref"": ""#money-plain""
        }
      ]
    },
    ""unit"": {
      ""$id"": ""#unit"",
      ""oneOf"": [
        {
          ""type"": ""string""
        },
        {
          ""type"": ""object"",
          ""properties"": {
            ""type"": {
              ""type"": ""string""
            }
          }
        }
      ]
    },
    ""value"": {
      ""$id"": ""#value"",
      ""anyOf"": [
        {
          ""$ref"": ""#value-str""
        },
        {
          ""$ref"": ""#value-fraction""
        },
        {
          ""$ref"": ""#value-number""
        },
        {
          ""$ref"": ""#value-bool""
        }
      ]
    },
    ""value-str"": {
      ""$id"": ""#value-str"",
      ""type"": ""string"",
      ""maxLength"": 1024
    },
    ""value-fraction"": {
      ""$id"": ""#value-fraction"",
      ""type"": ""object"",
      ""required"": [
        ""type"",
        ""numerator"",
        ""denominator""
      ]
    },
    ""value-number"": {
      ""$id"": ""#value-number"",
      ""type"": ""number""
    },
    ""value-bool"": {
      ""$id"": ""#value-bool"",
      ""type"": ""boolean""
    },
    ""currency"": {
      ""$id"": ""#currency"",
      ""type"": ""string"",
      ""minLength"": 3,
      ""maxLength"": 3,
      ""examples"": [
        ""USD"",
        ""EUR"",
        ""SEK"",
        ""BTC""
      ]
    },
    ""money"": {
      ""$id"": ""#money"",
      ""oneOf"": [
        {
          ""$ref"": ""#/definitions/money-plain""
        },
        {
          ""$ref"": ""#/definitions/money-gauge""
        }
      ]
    },
    ""money-plain"": {
      ""$id"": ""#money-plain"",
      ""type"": ""object"",
      ""properties"": {
        ""currency"": {
          ""type"": ""string"",
          ""$ref"": ""#/definitions/currency""
        }
      },
      ""required"": [
        ""amount"",
        ""currency""
      ]
    },
    ""money-gauge"": {
      ""properties"": {
        ""type"": {
          ""type"": ""string"",
          ""const"": ""gauge""
        },
        ""value"": {
          ""type"": ""number""
        },
        ""unit"": {
          ""$ref"": ""#unit""
        }
      },
      ""required"": [
        ""type"",
        ""value"",
        ""unit""
      ]
    },
    ""error-info"": {
      ""$id"": ""#error-info"",
      ""properties"": {
        ""stackTrace"": {
          ""$ref"": ""#stack-trace""
        },
        ""inner"": {
          ""$ref"": ""#error-info""
        }
      },
      ""required"": [
        ""stackTrace""
      ]
    },
    ""stack-trace"": {
      ""$id"": ""#stack-trace"",
      ""properties"": {
        ""frames"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#stack-frame""
          }
        }
      }
    },
    ""stack-frame"": {
      ""$id"": ""#stack-frame"",
      ""type"": ""object"",
      ""properties"": {
        ""loadModule"": {
          ""$ref"": ""#/definitions/load-module""
        }
      }
    },
    ""load-module"": {
      ""$id"": ""#load-module"",
      ""properties"": {
        ""name"": {
          ""type"": ""string""
        },
        ""buildId"": {
          ""type"": ""string""
        }
      }
    },
    ""string-string-dict"": {
      ""$id"": ""#string-string-dict"",
      ""type"": ""object"",
      ""patternProperties"": {
        "".*"": {
          ""type"": ""string""
        }
      }
    },
    ""string-value-dict"": {
      ""$id"": ""#string-value-dict"",
      ""type"": ""object"",
      ""patternProperties"": {
        "".*"": {
          ""$ref"": ""#value""
        }
      }
    },
    ""string-gauge-dict"": {
      ""$id"": ""#string-gauge-dict"",
      ""type"": ""object"",
      ""patternProperties"": {
        "".*"": {
          ""$ref"": ""#gauge""
        }
      }
    },
    ""float-float-dict"": {
      ""$id"": ""#float-float-dict"",
      ""type"": ""object"",
      ""patternProperties"": {
        "".*"": {
          ""type"": ""number""
        }
      }
    }
  }
}";
    }
}
