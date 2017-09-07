#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
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

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0094Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(Schema);

            JObject o = JObject.Parse(Json);

            Assert.AreEqual(true, o.IsValid(s));
        }

        public const string Schema = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""lifeEventID"": {
      ""type"": ""string"",
      ""pattern"": ""^[0-9]{10}$""
    },
    ""transactionType"": {
      ""type"": ""string"",
      ""enum"": [
        ""Bonus"",
        ""Penalty""
      ]
    },
    ""bonusDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""duePeriodAmount"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""dueYTD"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""paidYTD"": {
          ""$ref"": ""#/definitions/decimalValue""
        }
      },
      ""required"": [
        ""duePeriodAmount"",
        ""dueYTD""
      ],
      ""additionalProperties"": false
    },
    ""claimDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""startPeriod"": {
          ""$ref"": ""#/definitions/dateString""
        },
        ""endPeriod"": {
          ""$ref"": ""#/definitions/dateString""
        },
        ""reason"": {
          ""type"": ""string"",
          ""enum"": [
            ""Life Event"",
            ""Regular Bonus""
          ]
        }
      },
      ""required"": [
        ""startPeriod"",
        ""endPeriod"",
        ""reason""
      ],
      ""additionalProperties"": false
    },
    ""transferDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""inAmountForPeriod"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""amountYTD"": {
          ""$ref"": ""#/definitions/decimalValue""
        }
      },
      ""additionalProperties"": false
    },
    ""paymentsDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""newSubsForPeriod"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""newSubsYTD"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""totalSubsForPeriod"": {
          ""$ref"": ""#/definitions/decimalValue""
        },
        ""totalSubsYTD"": {
          ""$ref"": ""#/definitions/decimalValue""
        }
      },
      ""required"": [
        ""newSubsYTD"",
        ""totalSubsForPeriod"",
        ""totalSubsYTD""
      ],
      ""additionalProperties"": false
    }
  },
  ""required"": [
    ""transactionType"",
    ""bonusDetail"",
    ""claimDetail"",
    ""paymentsDetail""
  ],
  ""additionalProperties"": false,
  ""definitions"": {
    ""dateString"": {
      ""type"": ""string"",
      ""pattern"": ""^[2][0][0-9][0-9][-][0-1][0-9][-][0-3][0-9]$"",
      ""description"": ""YYYY-MM-DD""
    },
    ""decimalValue"": {
      ""type"": ""number"",
      ""multipleOf"": 0.01
    }
  }
}";

        public const string Json = @"{
  ""lifeEventID"": ""1234567891"",
  ""transactionType"": ""Bonus"",
  ""bonusDetail"": {
    ""duePeriodAmount"": 5555555555555555555555555555.01,
    ""dueYTD"": 40000,
    ""paidYTD"": 40000
  },
  ""claimDetail"": {
    ""startPeriod"": ""2017-04-06"",
    ""endPeriod"": ""2017-04-06"",
    ""reason"": ""Life Event""
  },
  ""transferDetail"": {
    ""inAmountForPeriod"": 40000,
    ""amountYTD"": 40000
  },
  ""paymentsDetail"": {
    ""newSubsForPeriod"": 4000,
    ""newSubsYTD"": 4000,
    ""totalSubsForPeriod"": 4000,
    ""totalSubsYTD"": 4000
  }
}";
    }
}