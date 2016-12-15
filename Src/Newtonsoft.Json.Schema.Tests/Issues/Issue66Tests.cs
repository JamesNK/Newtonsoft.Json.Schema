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
    public class Issue66Tests : TestFixtureBase
    {
        private string schemaJson = @"{
  ""id"": ""schemas/events/Clicked.json"",
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""description"": ""The user interacted with the page by clicking on something interactable."",
  ""type"": ""object"",
  ""allOf"": [
    {
      ""id"": ""file:base.json"",
      ""$schema"": ""http://json-schema.org/draft-04/schema#"",
      ""type"": ""object"",
      ""properties"": {
        ""schemaVersion"": {
          ""type"": ""string"",
          ""pattern"": ""^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)""
        },
        ""eventType"": {
          ""type"": ""string"",
          ""enum"": [
            ""Clicked"",
            ""EntityViewed"",
            ""FormSubmitted"",
            ""PageViewed"",
            ""Searched"",
            ""SurveyCreated"",
            ""SurveySaved""
          ]
        },
        ""eventVersion"": {
          ""type"": ""string"",
          ""pattern"": ""^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)""
        },
        ""eventPayload"": {
          ""type"": ""object""
        },
        ""headers"": {
          ""type"": ""object""
        },
        ""query"": {
          ""type"": ""object""
        },
        ""url"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""property"": {
          ""type"": ""string"",
          ""default"": ""realmassive""
        },
        ""domain"": {
          ""type"": ""string""
        },
        ""path"": {
          ""type"": ""string""
        },
        ""thirdParty"": {
          ""type"": ""boolean""
        },
        ""referrer"": {
          ""type"": ""string""
        },
        ""timestamp"": {
          ""type"": [
            ""number"",
            ""string""
          ]
        },
        ""performance"": {
          ""type"": ""object""
        },
        ""client"": {
          ""type"": ""object"",
          ""properties"": {
            ""userAgent"": {
              ""type"": ""string""
            },
            ""deviceId"": {
              ""type"": ""string""
            },
            ""screenHeight"": {
              ""type"": ""number""
            },
            ""screenWidth"": {
              ""type"": ""number""
            },
            ""devicePixelRatio"": {
              ""type"": ""number""
            },
            ""ipAddress"": {
              ""type"": ""string""
            }
          },
          ""required"": [
            ""userAgent"",
            ""deviceId"",
            ""ipAddress""
          ]
        },
        ""campaign"": {
          ""type"": ""string""
        },
        ""slice"": {
          ""type"": ""string""
        },
        ""features"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""string""
          }
        }
      },
      ""required"": [
        ""schemaVersion"",
        ""eventType"",
        ""eventVersion"",
        ""url"",
        ""property"",
        ""domain"",
        ""path"",
        ""timestamp"",
        ""client""
      ]
    }
  ],
  ""properties"": {
    ""eventType"": {
      ""type"": ""string"",
      ""pattern"": ""^Clicked$""
    },
    ""eventPayload"": {
      ""type"": ""object"",
      ""properties"": {
        ""items"": {
          ""id"": ""file:components/EntityItems.json"",
          ""$schema"": ""http://json-schema.org/draft-04/schema#"",
          ""description"": ""an array containing Entity components associated with the current event"",
          ""type"": ""array"",
          ""properties"": {
            ""items"": {
              ""id"": ""file:components/Entity.json"",
              ""$schema"": ""http://json-schema.org/draft-04/schema#"",
              ""type"": ""object"",
              ""properties"": {
                ""id"": {
                  ""type"": [
                    ""string"",
                    ""number""
                  ]
                },
                ""type"": {
                  ""type"": ""string""
                },
                ""attributes"": {
                  ""type"": ""object""
                },
                ""relationships"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""id"": ""file:components/EntityReference.json"",
                    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
                    ""type"": ""object"",
                    ""properties"": {
                      ""id"": {
                        ""type"": [
                          ""string"",
                          ""number""
                        ]
                      },
                      ""type"": {
                        ""type"": ""string""
                      }
                    }
                  }
                }
              }
            }
          }
        },
        ""eventTarget"": {
          ""description"": ""a string derived from the nearest data-analytics-target attribute defined on an element"",
          ""type"": ""string""
        },
        ""targetText"": {
          ""description"": ""the inner text of the element clicked on"",
          ""type"": ""string""
        }
      },
      ""anyOf"": [
        {
          ""required"": [
            ""items""
          ]
        },
        {
          ""required"": [
            ""eventTarget""
          ]
        },
        {
          ""required"": [
            ""targetText""
          ]
        }
      ]
    }
  },
  ""required"": [
    ""eventPayload""
  ]
}";

        [Test]
        public void Test()
        {
            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                JSchema s = JSchema.Parse(schemaJson);

                JObject o = JObject.Parse(@"{
  ""lineItems"": [
    {
      ""rate"": 1.222
    },
    {
      ""placementName"": ""test""
    }
  ]
}");

                IList<string> errors;
                o.IsValid(s, out errors);
            }, "Invalid URI error while resolving schema ID. Scope URI: 'schemas/events/schemas/events/Clicked.json', schema URI: 'file:components/EntityItems.json'");
        }
    }
}