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
    public class Issue0260Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JObject clientJson = JObject.Parse(Json);
            JSchema schema = JSchema.Parse(Schema);

            bool valid = clientJson.IsValid(schema, out IList<ValidationError> errorMessages);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, errorMessages.Count);

            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0, 1.", errorMessages[0].Message);
            Assert.AreEqual(2, errorMessages[0].ChildErrors.Count);
            Assert.AreEqual("Required properties are missing from object: baseUrl, redirectUris.", errorMessages[0].ChildErrors[0].Message);
            Assert.AreEqual("JSON does not match schema from 'then'.", errorMessages[0].ChildErrors[1].Message);
            Assert.AreEqual(1, errorMessages[0].ChildErrors[1].ChildErrors.Count);
            Assert.AreEqual("Required properties are missing from object: baseUrl, redirectUris.", errorMessages[0].ChildErrors[1].ChildErrors[0].Message);
        }

        private const string Schema = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""type"": ""object"",
  ""definitions"": {
    ""type"": {
      ""enum"": [
        ""CLIENT_SIDE_FRONTEND"",
        ""SERVER_SIDE_FRONT_END"",
        ""CONFIDENTIAL_SERVICE"",
        ""BEARER_ONLY_SERVICE""
      ],
    },
    ""clientRoles"": {
      ""type"": ""object"",
      ""minProperties"": 1,
      ""additionalProperties"": {
        ""type"": ""array"",
        ""items"": {
          ""type"": ""string""
        },
        ""uniqueItems"": true,
        ""minItems"": 1
      }
    },
    ""realmRoles"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""string""
      },
      ""uniqueItems"": true,
      ""minItems"": 1
    },
    ""role"": {
      ""type"": ""object"",
      ""required"": [
        ""name""
      ],
      ""properties"": {
        ""name"": {
          ""type"": ""string"",
          ""minLength"": 2,
          ""maxLength"": 255
        },
        ""description"": {
          ""type"": ""string"",
          ""minLength"": 1,
          ""maxLength"": 255
        },
        ""defaultRole"": {
          ""type"": ""boolean"",
          ""default"": false
        },
        ""compositeRoles"": {
          ""type"": ""object"",
          ""anyOf"": [
            {
              ""required"": [
                ""clientRoles""
              ]
            },
            {
              ""required"": [
                ""realmRoles""
              ]
            }
          ],
          ""properties"": {
            ""clientRoles"": {
              ""$ref"": ""#/definitions/clientRoles""
            },
            ""realmRoles"": {
              ""$ref"": ""#/definitions/realmRoles""
            }
          },
          ""additionalProperties"": false
        }
      },
      ""additionalProperties"": false
    },
    ""group"": {
      ""type"": ""object"",
      ""title"": ""Defines a group"",
      ""required"": [
        ""name""
      ],
      ""properties"": {
        ""name"": {
          ""type"": ""string"",
          ""minLength"": 2,
          ""maxLength"": 255
        },
        ""description"": {
          ""type"": ""string"",
          ""minLength"": 1,
          ""maxLength"": 255
        },
        ""clientRoles"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""string""
          },
          ""uniqueItems"": true,
          ""minItems"": 1
        },
        ""realmRoles"": {
          ""$ref"": ""#/definitions/realmRoles""
        },
        ""defaultGroup"": {
          ""type"": ""boolean""
        },
        ""groups"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/group""
          },
          ""uniqueItems"": true,
          ""minItems"": 1
        }
      },
      ""additionalProperties"": false
    },
    ""clientProperties"": {
      ""clientId"": {
        ""type"": ""string"",
        ""minLength"": 2,
        ""maxLength"": 255
      },
      ""name"": {
        ""type"": ""string"",
        ""minLength"": 2,
        ""maxLength"": 255
      },
      ""enabled"": {
        ""type"": ""boolean"",
        ""default"": true
      },
      ""secret"": {
        ""type"": ""string"",
        ""minLength"": 2,
        ""maxLength"": 255
      },
      ""baseUrl"": {
        ""type"": ""string"",
        ""minLength"": 8,
        ""maxLength"": 255
      },
      ""redirectUris"": {
        ""type"": ""array"",
        ""items"": {
          ""type"": ""string""
        },
        ""minItems"": 1
      },
      ""webOrigins"": {
        ""type"": ""array"",
        ""additionalItems"": true,
        ""items"": {
          ""type"": ""string""
        },
        ""minItems"": 1
      },
      ""roles"": {
        ""type"": ""array"",
        ""uniqueItems"": true,
        ""minItems"": 1,
        ""items"": [
          {
            ""$ref"": ""#/definitions/role""
          }
        ]
      },
      ""serviceAccount"": {
        ""type"": ""object"",
        ""anyOf"": [
          {
            ""required"": [
              ""clientRoles""
            ]
          },
          {
            ""required"": [
              ""realmRoles""
            ]
          }
        ],
        ""properties"": {
          ""clientRoles"": {
            ""$ref"": ""#/definitions/clientRoles""
          },
          ""realmRoles"": {
            ""$ref"": ""#/definitions/realmRoles""
          }
        },
        ""additionalProperties"": false
      },
      ""claims"": {
        ""type"": ""object"",
        ""required"": [
          ""hardcoded""
        ],
        ""properties"": {
          ""hardcoded"": {
            ""type"": ""object"",
            ""minProperties"": 1,
            ""additionalProperties"": {
              ""type"": ""object"",
              ""$ref"": ""#/definitions/hardcodedClaim""
            }
          }
        }
      }
    },
    ""hardcodedClaim"": {
      ""type"": ""object"",
      ""required"": [
        ""value"",
        ""type""
      ],
      ""properties"": {
        ""value"": {
          ""type"": ""string""
        },
        ""type"": {
          ""enum"": [
            ""String"",
            ""long"",
            ""int"",
            ""boolean"",
            ""JSON""
          ]
        }
      }
    },
    ""publicClient"": {
      ""type"": ""object"",
      ""required"": [
        ""type"",
        ""clientId"",
        ""name"",
        ""baseUrl"",
        ""redirectUris""
      ],
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/type""
        },
        ""clientId"": {
          ""$ref"": ""#/definitions/clientProperties/clientId""
        },
        ""name"": {
          ""$ref"": ""#/definitions/clientProperties/name""
        },
        ""enabled"": {
          ""$ref"": ""#/definitions/clientProperties/enabled""
        },
        ""secret"": {
          ""$ref"": ""#/definitions/clientProperties/secret""
        },
        ""baseUrl"": {
          ""$ref"": ""#/definitions/clientProperties/baseUrl""
        },
        ""redirectUris"": {
          ""$ref"": ""#/definitions/clientProperties/redirectUris""
        },
        ""webOrigins"": {
          ""$ref"": ""#/definitions/clientProperties/webOrigins""
        },
        ""roles"": {
          ""$ref"": ""#/definitions/clientProperties/roles""
        },
        ""claims"": {
          ""$ref"": ""#/definitions/clientProperties/claims""
        }
      },
      ""additionalProperties"": false
    },
    ""bearerClient"": {
      ""type"": ""object"",
      ""required"": [
        ""type"",
        ""clientId"",
        ""name""
      ],
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/type""
        },
        ""clientId"": {
          ""$ref"": ""#/definitions/clientProperties/clientId""
        },
        ""name"": {
          ""$ref"": ""#/definitions/clientProperties/name""
        },
        ""enabled"": {
          ""$ref"": ""#/definitions/clientProperties/enabled""
        },
        ""secret"": {
          ""$ref"": ""#/definitions/clientProperties/secret""
        },
        ""roles"": {
          ""$ref"": ""#/definitions/clientProperties/roles""
        },
        ""claims"": {
          ""$ref"": ""#/definitions/clientProperties/claims""
        }
      },
      ""additionalProperties"": false
    },
    ""confidentialClient"": {
      ""type"": ""object"",
      ""required"": [
        ""type"",
        ""clientId"",
        ""name"",
        ""baseUrl"",
        ""redirectUris""
      ],
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/type""
        },
        ""clientId"": {
          ""$ref"": ""#/definitions/clientProperties/clientId""
        },
        ""name"": {
          ""$ref"": ""#/definitions/clientProperties/name""
        },
        ""enabled"": {
          ""$ref"": ""#/definitions/clientProperties/enabled""
        },
        ""secret"": {
          ""$ref"": ""#/definitions/clientProperties/secret""
        },
        ""baseUrl"": {
          ""$ref"": ""#/definitions/clientProperties/baseUrl""
        },
        ""redirectUris"": {
          ""$ref"": ""#/definitions/clientProperties/redirectUris""
        },
        ""webOrigins"": {
          ""$ref"": ""#/definitions/clientProperties/webOrigins""
        },
        ""roles"": {
          ""$ref"": ""#/definitions/clientProperties/roles""
        },
        ""serviceAccount"": {
          ""$ref"": ""#/definitions/clientProperties/serviceAccount""
        },
        ""claims"": {
          ""$ref"": ""#/definitions/clientProperties/claims""
        }
      },
      ""additionalProperties"": false
    },
    ""client"": {
      ""allOf"": [
        {
          ""$ref"": ""#/definitions/confidentialClient""
        },
        {
          ""if"": {
            ""properties"": {
              ""type"": {
                ""const"": ""CONFIDENTIAL_SERVICE""
              }
            }
          },
          ""then"": {
            ""$ref"": ""#/definitions/confidentialClient""
          }
        }
      ]
    }
  },
  ""required"": [
    ""version"",
    ""service""
  ],
  ""properties"": {
    ""version"": {
      ""type"": ""string"",
      ""minLength"": 1
    },
    ""service"": {
      ""required"": [
        ""name"",
        ""realmName"",
        ""clients""
      ],
      ""properties"": {
        ""name"": {
          ""type"": ""string"",
          ""minLength"": 2,
          ""maxLength"": 255
        },
        ""realmName"": {
          ""type"": ""string"",
          ""minLength"": 2,
          ""maxLength"": 255
        },
        ""description"": {
          ""type"": ""string"",
          ""minLength"": 1,
          ""maxLength"": 255
        },
        ""clients"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/client"",
            ""uniqueItems"": true,
            ""minItems"": 1
          }
        },
        ""realm"": {
          ""type"": ""object"",
          ""required"": [
            ""groups""
          ],
          ""properties"": {
            ""groups"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/group""
              },
              ""uniqueItems"": true,
              ""minItems"": 1
            }
          },
          ""additionalProperties"": false
        }
      },
      ""additionalProperties"": false
    }
  },
  ""additionalProperties"": false
}";

        private const string Json = @"{
  ""version"": ""2"",
  ""service"": {
    ""name"": ""My New Service"",
    ""realmName"": ""application-services"",
    ""description"": ""something"",
    ""clients"": [
      {
      ""clientId"": ""test"",
      ""type"": ""CONFIDENTIAL_SERVICE"",
      ""name"": ""test"",
      ""enabled"": true,
      ""secret"": ""A_SECRET"",
      ""claims"": {
        ""hardcoded"": {
          ""claim1"": {
            ""value"": ""aaaa"",
            ""type"": ""String""
          }
        }
      }
      }
    ]
  }
}";
    }
}
