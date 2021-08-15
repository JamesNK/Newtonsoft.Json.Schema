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
    public class Issue0275Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(Schema);
            JObject o = JObject.Parse(Data);

            Assert.IsTrue(o.IsValid(schema));
        }

        private const string Schema = @"{
    ""$schema"": ""http://json-schema.org/draft-07/schema"",
    ""type"": ""object"",
    ""title"": ""DiscontinueSKUSplit"",
    ""description"": ""name of object at Inter"",
    ""properties"": {
        ""contentSource"": {
            ""type"": ""string"",
            ""description"": ""The source system of the interface""
        },
        ""contentInterface"": {
            ""type"": ""string"",
            ""description"": ""The interface name of the integration in jira insight""
        },
        ""contents"": {
            ""type"": ""array"",
            ""description"": ""List of contents having metaDataHeader and dataObjectValues"",
            ""additionalItems"": false,
            ""minItems"": 1,
            ""items"": {
                ""$ref"": ""#/definitions/contentItem""
            }
        }
    },
    ""definitions"": {
        ""contentItem"": {
            ""type"": ""object"",
            ""properties"": {
                ""dataObjectValue"": {
                    ""$ref"": ""#/definitions/dataObjectValueType""
                }
            },
            ""additionalProperties"": false
        },
        ""dataObjectValueType"": {
            ""type"": ""object"",
            ""properties"": {
                ""lastOrderQTY"": {
                    ""type"": ""number"",
                    ""description"": ""It is DI Type plan arrivals."",
                    ""maximum"": 99999999999999999999.99,
                    ""multipleOf"": 0.00000000000000000000000000001
                }
            }
        }
    },
    ""additionalProperties"": false,
    ""required"": [
        ""contentSource"",
        ""contentInterface"",
        ""contents""
    ]
}";

        private const string Data = @"{
    ""contentSource"": ""Lorem"",
    ""contentInterface"": ""Lorem"",
    ""contents"": [
        {
            ""dataObjectValue"": {
                ""lastOrderQTY"": 99999999999999999999.099
            }
        }
    ]
}";
    }
}
