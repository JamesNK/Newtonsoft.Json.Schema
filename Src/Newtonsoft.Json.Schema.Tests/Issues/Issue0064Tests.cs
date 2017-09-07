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
    public class Issue0064Tests : TestFixtureBase
    {
        private string schemaJson = @"{
  ""type"": ""object"",
  ""properties"": {
    ""metadata"": {
      ""type"": ""object"",
      ""properties"": {
        ""attributes"": {
          ""type"": ""object"",
          ""properties"": {},
          ""additionalProperties"": false,
          ""minProperties"": 1,
          ""patternProperties"": {
            ""^[A-Za-z]+([._-]*[A-Za-z0-9]+)*$"": {
              ""id"":""#itemId"",
              ""type"": ""object"",
              ""uniqueItems"": true,
              ""properties"": {
                ""displayName"": {
                  ""id"": ""displayName"",
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""type"": {
                  ""id"": ""type"",
                  ""type"": ""string"",
                  ""enum"": [
                    ""text"",
                    ""number""
                  ]
                },
                ""format"": {
                  ""id"": ""format"",
                  ""type"": ""object"",
                  ""properties"": {
                    ""prefix"": {
                      ""id"": ""prefix"",
                      ""type"": ""string""
                    },
                    ""suffix"": {
                      ""id"": ""suffix"",
                      ""type"": ""string""
                    },
                    ""precision"": {
                      ""id"": ""precision"",
                      ""type"": ""number"",
                      ""minimum"": 0,
                      ""exclusiveMinimum"": false
                    }
                  },
                  ""anyOf"": [
                    {
                      ""required"": [
                        ""prefix""
                      ]
                    },
                    {
                      ""required"": [
                        ""suffix""
                      ]
                    },
                    {
                      ""required"": [
                        ""precision""
                      ]
                    }
                  ],
                  ""additionalProperties"": false
                },
                ""style"": {
                  ""id"": ""style"",
                  ""type"": ""object"",
                  ""properties"": {
                    ""align"": {
                      ""id"": ""align"",
                      ""type"": ""string"",
                      ""enum"": [
                        ""left"",
                        ""center"",
                        ""right""
                      ]
                    }
                  },
                  ""additionalProperties"": false,
                  ""required"": [
                    ""align""
                  ]
                }
              },
              ""required"": [
                ""displayName"",
                ""type""
              ],
              ""additionalProperties"": false
            }
          }
        }/* attributes */,
        ""categoryAndValuePositions"": {
          ""type"": ""object"",
          ""minProperties"": 1,
          ""additionalProperties"": false,
          ""properties"": {},
          ""patternProperties"": {
            ""level_([0-9]+)"": {
              ""type"": ""object"",
              ""properties"": {
                ""node"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""valuePosition"": {
                  ""type"": ""array""
                }
              }/*properties*/,
              ""required"": [
              	""node"", ""valuePosition""
              ]
            }/*Level..*/
          }/*patternProperties */
        }/*categoryAndValuePositions*/,
        ""chart"": {
          ""type"": ""object"",
          ""additionalProperties"": false,
          ""properties"": {},
          ""patternProperties"": {
            ""level_([0-9]+)"": {
              ""type"": ""object"",
              ""properties"": {
                ""xAxisLabel"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""yAxisLabel"": {
                  ""type"": ""string"",
                  ""minLength"": 1
                },
                ""xAxis"":{
                  ""type"": ""string"",
                  ""minLength"": 1,
                  ""oneOf"":[
                    {	""$ref"":""#/properties/metadata/properties/attributes/patternProperties/^[A-Za-z]+([._-]*[A-Za-z0-9]+)*$""
                    }
                  ]
                },
                ""yAxis"":{
                  ""type"": ""array"",
                  ""minLength"": 1
                },
                ""title"":{
                  ""type"": ""string""
                }
              }/*properties*/,
              ""additionalProperties"": false,
              ""required"":[""xAxis"", ""yAxis""]
            }/*Level..*/
          }/*patternProperties */
        } /*chart*/,
        ""grid"": {}
      },
      ""additionalProperties"": false
    }/*metadata*/
  }/*global properties*/,
  ""additionalProperties"": false
}/* global */";

        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(schemaJson);

            JSchema patternSchema = s.Properties["metadata"].Properties["attributes"].PatternProperties["^[A-Za-z]+([._-]*[A-Za-z0-9]+)*$"];

            JSchema oneOf = s.Properties["metadata"].Properties["chart"].PatternProperties["level_([0-9]+)"].Properties["xAxis"].OneOf[0];

            Assert.AreEqual(patternSchema, oneOf);
        }
    }
}