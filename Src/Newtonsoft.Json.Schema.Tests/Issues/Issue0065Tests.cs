#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0065Tests : TestFixtureBase
    {
        private readonly string schemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""oneOf"": [
    {
      ""$ref"": ""#/definitions/JsonRTC_Frame_Request_and_Message""
    },
    {
      ""$ref"": ""#/definitions/JsonRTC_Frame_Response""
    },
    {
      ""$ref"": ""#/definitions/JsonRTC_Frame_Ack""
    },
    {
      ""$ref"": ""#/definitions/JsonRTC_Frame_Error""
    }
  ],
  ""definitions"": {
    ""JsonRTC_Frame_Request_and_Message"": {
      ""type"": ""object"",
      ""properties"": {
        ""control"": {
          ""$ref"": ""#/definitions/control_request_and_message""
        },
        ""header"": {
          ""allOf"": [
            {
              ""$ref"": ""#/definitions/header_common""
            },
            {
              ""$ref"": ""#/definitions/header_message_notification""
            },
            {
              ""$ref"": ""#/definitions/header_conversation""
            },
            {
              ""$ref"": ""#/definitions/header_connect""
            }
          ]
        },
        ""payload"": {
          ""$ref"": ""#/definitions/payload""
        }
      },
      ""required"": [
        ""control""
      ],
      ""additionalProperties"": false
    },
    ""JsonRTC_Frame_Response"": {
      ""type"": ""object"",
      ""properties"": {
        ""control"": {
          ""$ref"": ""#/definitions/control_response""
        },
        ""header"": {
          ""allOf"": [
            {
              ""$ref"": ""#/definitions/header_common""
            },
            {
              ""$ref"": ""#/definitions/header_message_notification""
            },
            {
              ""$ref"": ""#/definitions/header_conversation""
            },
            {
              ""$ref"": ""#/definitions/header_response""
            },
            {
              ""$ref"": ""#/definitions/header_connect""
            }
          ]
        },
        ""payload"": {
          ""$ref"": ""#/definitions/payload""
        }
      },
      ""required"": [
        ""control""
      ],
      ""additionalProperties"": false
    },
    ""JsonRTC_Frame_Ack"": {
      ""type"": ""object"",
      ""properties"": {
        ""control"": {
          ""$ref"": ""#/definitions/control_ack""
        },
        ""header"": {
          ""allOf"": [
            {
              ""$ref"": ""#/definitions/header_common""
            },
            {
              ""$ref"": ""#/definitions/header_message_notification""
            },
            {
              ""$ref"": ""#/definitions/header_conversation""
            },
            {
              ""$ref"": ""#/definitions/header_connect""
            }
          ]
        },
        ""payload"": {
          ""description"": ""payload should not exist in acknowledgement according to https://docs.oracle.com/cd/E69505_01/doc.72/e69518/xt_wse_protocol_ref.htm#WSEXT237, add it in in order to get integration tests passed."",
          ""$ref"": ""#/definitions/payload""
        }
      },
      ""required"": [
        ""control""
      ],
      ""additionalProperties"": false
    },
    ""JsonRTC_Frame_Error"": {
      ""type"": ""object"",
      ""properties"": {
        ""control"": {
          ""$ref"": ""#/definitions/control_error""
        },
        ""header"": {
          ""allOf"": [
            {
              ""$ref"": ""#/definitions/header_common""
            },
            {
              ""$ref"": ""#/definitions/header_error""
            }
          ]
        },
        ""payload"": {
          ""description"": ""payload should not exist in error according to https://docs.oracle.com/cd/E69505_01/doc.72/e69518/xt_wse_protocol_ref.htm#WSEXT237, add it in in order to get integration tests passed."",
          ""$ref"": ""#/definitions/payload""
        }
      },
      ""required"": [
        ""control""
      ],
      ""additionalProperties"": false
    },
    ""header_common"": {
      ""type"": ""object"",
      ""properties"": {
        ""action"": {
          ""$ref"": ""#/definitions/message_action""
        },
        ""initiator"": {
          ""$ref"": ""#/definitions/uri""
        },
        ""target"": {
          ""$ref"": ""#/definitions/uri""
        },
        ""participant"": {
          ""$ref"": ""#/definitions/uri""
        },
        ""enquiry_data"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/enquiry_data""
            },
            {
              ""$ref"": ""#/definitions/free_text""
            }
          ]
        },
        ""stream_id"": {
          ""$ref"": ""#/definitions/stream_id""
        },
        ""stream_type"": {
          ""$ref"": ""#/definitions/stream_type""
        },
        ""stream_options"": {
          ""$ref"": ""#/definitions/stream_options""
        },
        ""authenticate"": {
          ""type"": ""object""
        },
        ""authorization"": {
          ""type"": ""object""
        },
        ""instructions"": {
          ""$ref"": ""#/definitions/instructions""
        },
        ""event"": {
          ""$ref"": ""#/definitions/event""
        },
        ""instruction_name"": {
          ""$ref"": ""#/definitions/instruction_name""
        },
        ""web_context"": {
          ""$ref"": ""#/definitions/webContext""
        }
      }
    },
    ""header_error"": {
      ""type"": ""object"",
      ""properties"": {
        ""error_code"": {
          ""type"": ""integer"",
          ""minimum"": 300
        },
        ""reason"": {
          ""type"": ""string""
        }
      }
    },
    ""header_response"": {
      ""type"": ""object"",
      ""properties"": {
        ""response_code"": {
          ""type"": ""integer"",
          ""minimum"": 100,
          ""maximum"": 700
        },
        ""wsc_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      }
    },
    ""header_connect"": {
      ""type"": ""object"",
      ""properties"": {
        ""sslr"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""cslr"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""cslw"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""csuw"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""conversation_channel"": {
          ""$ref"": ""#/definitions/conversation_channel""
        }
      }
    },
    ""header_conversation"": {
      ""type"": ""object"",
      ""properties"": {
        ""conversation_key"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""conversation_topic"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""conversation_channel"": {
          ""$ref"": ""#/definitions/conversation_channel""
        }
      }
    },
    ""header_message_notification"": {
      ""type"": ""object"",
      ""properties"": {
        ""expiry"": {
          ""type"": ""string"",
          ""format"": ""date-time""
        }
      }
    },
    ""control_request_and_message"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/msg-request_and_message""
        },
        ""package_type"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/package_type_common""
            },
            {
              ""$ref"": ""#/definitions/package_type_conversation""
            },
            {
              ""$ref"": ""#/definitions/package_type_capability""
            },
            {
              ""$ref"": ""#/definitions/package_type_message_notification""
            }
          ]
        },
        ""session_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""subsession_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""sequence"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""ack_sequence"": {
          ""type"": ""integer"",
          ""minimum"": 0
        },
        ""correlation_id"": {
          ""$ref"": ""#/definitions/correlation_id""
        },
        ""version"": {
          ""$ref"": ""#/definitions/version""
        },
        ""wsc_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      },
      ""required"": [
        ""type"",
        ""version""
      ],
      ""additionalProperties"": false
    },
    ""control_response"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/msg-response""
        },
        ""package_type"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/package_type_common""
            },
            {
              ""$ref"": ""#/definitions/package_type_conversation""
            },
            {
              ""$ref"": ""#/definitions/package_type_capability""
            },
            {
              ""$ref"": ""#/definitions/package_type_message_notification""
            }
          ]
        },
        ""session_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""subsession_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""sequence"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""ack_sequence"": {
          ""type"": ""integer"",
          ""minimum"": 0
        },
        ""correlation_id"": {
          ""$ref"": ""#/definitions/correlation_id""
        },
        ""message_state"": {
          ""type"": ""string"",
          ""enum"": [
            ""final"",
            ""subsequent""
          ]
        },
        ""version"": {
          ""$ref"": ""#/definitions/version""
        },
        ""wsc_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      },
      ""required"": [
        ""type"",
        ""version""
      ],
      ""additionalProperties"": false
    },
    ""control_ack"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/msg-ack""
        },
        ""package_type"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/package_type_common""
            },
            {
              ""$ref"": ""#/definitions/package_type_conversation""
            },
            {
              ""$ref"": ""#/definitions/package_type_capability""
            },
            {
              ""$ref"": ""#/definitions/package_type_message_notification""
            }
          ]
        },
        ""session_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""subsession_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""sequence"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""correlation_id"": {
          ""$ref"": ""#/definitions/correlation_id""
        },
        ""version"": {
          ""$ref"": ""#/definitions/version""
        },
        ""wsc_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      },
      ""required"": [
        ""type"",
        ""version""
      ],
      ""additionalProperties"": false
    },
    ""control_error"": {
      ""type"": ""object"",
      ""properties"": {
        ""type"": {
          ""$ref"": ""#/definitions/msg-error""
        },
        ""package_type"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/package_type_common""
            },
            {
              ""$ref"": ""#/definitions/package_type_conversation""
            },
            {
              ""$ref"": ""#/definitions/package_type_capability""
            },
            {
              ""$ref"": ""#/definitions/package_type_message_notification""
            }
          ]
        },
        ""session_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""subsession_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""sequence"": {
          ""type"": ""integer"",
          ""minimum"": 1
        },
        ""ack_sequence"": {
          ""type"": ""integer"",
          ""minimum"": 0
        },
        ""correlation_id"": {
          ""$ref"": ""#/definitions/correlation_id""
        },
        ""version"": {
          ""$ref"": ""#/definitions/version""
        },
        ""wsc_id"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      },
      ""required"": [
        ""type"",
        ""version""
      ],
      ""additionalProperties"": false
    },
    ""msg-request_and_message"": {
      ""type"": ""string"",
      ""enum"": [
        ""request"",
        ""message""
      ]
    },
    ""msg-response"": {
      ""type"": ""string"",
      ""enum"": [
        ""response""
      ]
    },
    ""msg-ack"": {
      ""type"": ""string"",
      ""enum"": [
        ""acknowledgement""
      ]
    },
    ""msg-error"": {
      ""type"": ""string"",
      ""enum"": [
        ""error""
      ]
    },
    ""payload"": {
      ""type"": ""object"",
      ""properties"": {
        ""sdp"": {
          ""type"": ""string""
        },
        ""candidates"": {
          ""type"": ""string""
        },
        ""capability"": {
          ""type"": ""object"",
          ""properties"": {
            ""family"": {
              ""type"": ""string"",
              ""enum"": [
                ""android"",
                ""ios""
              ]
            },
            ""version"": {
              ""$ref"": ""#/definitions/version""
            },
            ""appid"": {
              ""$ref"": ""#/definitions/restricted_string_64""
            }
          },
          ""required"": [
            ""family"",
            ""version"",
            ""appid""
          ],
          ""additionalProperties"": false
        },
        ""devicetoken"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        }
      },
      ""additionalProperties"": false
    },
    ""enquiry_data"": {
      ""type"": ""object"",
      ""properties"": {
        ""target"": {
          ""$ref"": ""#/definitions/uri""
        },
        ""channel"": {
          ""$ref"": ""#/definitions/conversation_channel""
        },
        ""webContext"": {
          ""$ref"": ""#/definitions/webContext""
        },
        ""web_context"": {
          ""description"": ""added in order to pass integration tests"",
          ""$ref"": ""#/definitions/webContext""
        }
      },
      ""additionalProperties"": false
    },
    ""instructions"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""instruction_name"": {
            ""$ref"": ""#/definitions/instruction_name""
          },
          ""participants"": {
            ""$ref"": ""#/definitions/participants""
          },
          ""recording_style"": {
            ""type"": ""string"",
            ""enum"": [
              ""single"",
              ""participant""
            ]
          },
          ""include_associations"": {
            ""type"": ""array"",
            ""items"": {
              ""type"": ""string"",
              ""enum"": [
                ""CREATOR"",
                ""MEMBER""
              ]
            }
          }
        },
        ""required"": [
          ""instruction_name""
        ],
        ""additionalProperties"": false
      }
    },
    ""conversation_info"": {
      ""type"": ""object"",
      ""properties"": {
        ""recording_status"": {
          ""$ref"": ""#/definitions/recording_status""
        },
        ""participants"": {
          ""$ref"": ""#/definitions/participants""
        }
      },
      ""required"": [
        ""recording_status"",
        ""participants""
      ],
      ""additionalProperties"": false
    },
    ""event"": {
      ""type"": ""object"",
      ""properties"": {
        ""event_name"": {
          ""type"": ""string"",
          ""enum"": [
            ""stream_added"",
            ""stream_removed"",
            ""stream_rejected"",
            ""stream_joined"",
            ""stream_started"",
            ""stream_left"",
            ""stream_changed"",
            ""recording_started"",
            ""recording_ended""
          ]
        },
        ""participants"": {
          ""$ref"": ""#/definitions/participants""
        }
      },
      ""required"": [
        ""event_name"",
        ""participants""
      ],
      ""additionalProperties"": false
    },
    ""participants"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""streams"": {
            ""type"": ""array"",
            ""items"": {
              ""type"": ""object"",
              ""properties"": {
                ""size"": {
                  ""type"": [
                    ""integer"",
                    ""null""
                  ],
                  ""minimum"": 0
                },
                ""stream_id"": {
                  ""$ref"": ""#/definitions/stream_id""
                },
                ""stream_type"": {
                  ""$ref"": ""#/definitions/stream_type""
                },
                ""recording_status"": {
                  ""$ref"": ""#/definitions/recording_status""
                },
                ""stream_options"": {
                  ""$ref"": ""#/definitions/stream_options""
                }
              },
              ""additionalProperties"": false
            }
          },
          ""channel"": {
            ""$ref"": ""#/definitions/conversation_channel""
          },
          ""active_stream_id"": {
            ""$ref"": ""#/definitions/stream_id""
          },
          ""active_stream_type"": {
            ""$ref"": ""#/definitions/stream_type""
          },
          ""uri"": {
            ""$ref"": ""#/definitions/uri""
          },
          ""stream_id"": {
            ""$ref"": ""#/definitions/stream_id""
          },
          ""stream_type"": {
            ""$ref"": ""#/definitions/stream_type""
          },
          ""stream_options"": {
            ""$ref"": ""#/definitions/stream_options""
          },
          ""size"": {
            ""type"": ""string"",
            ""enum"": [
              ""maximize""
            ]
          }
        },
        ""additionalProperties"": false
      }
    },
    ""stream_options"": {
      ""type"": ""object"",
      ""properties"": {
        ""audio"": {
          ""description"": ""From MediaDirection enum"",
          ""type"": ""string"",
          ""enum"": [
            ""inactive"",
            ""recvonly"",
            ""sendonly"",
            ""sendrecv""
          ]
        },
        ""video"": {
          ""description"": ""From MediaDirection enum"",
          ""type"": ""string"",
          ""enum"": [
            ""inactive"",
            ""recvonly"",
            ""sendonly"",
            ""sendrecv""
          ]
        }
      },
      ""additionalProperties"": false
    },
    ""stream_type"": {
      ""type"": ""string"",
      ""enum"": [
        ""screenshare"",
        ""audiovideo""
      ]
    },
    ""stream_id"": {
      ""type"": ""string"",
      ""pattern"": ""^[a-zA-Z0-9-]+$"",
      ""minLength"": 1,
      ""maxLength"": 32
    },
    ""uri"": {
      ""$ref"": ""#/definitions/restricted_string_1024""
    },
    ""package_type_common"": {
      ""type"": ""string"",
      ""enum"": [
        ""register"",
        ""call"",
        ""flash"",
        ""messaging"",
        ""chat"",
        ""file_transfer""
      ]
    },
    ""package_type_message_notification"": {
      ""type"": ""string"",
      ""enum"": [
        ""message_notification""
      ]
    },
    ""package_type_conversation"": {
      ""type"": ""string"",
      ""enum"": [
        ""conversation""
      ]
    },
    ""package_type_capability"": {
      ""type"": ""string"",
      ""enum"": [
        ""capability""
      ]
    },
    ""message_action"": {
      ""type"": ""string"",
      ""enum"": [
        ""connect"",
        ""start"",
        ""complete"",
        ""hibernate"",
        ""notify"",
        ""shutdown"",
        ""prack"",
        ""enquiry"",
        ""send"",
        ""trickle"",
        ""iceEnquiry"",
        ""modify""
      ]
    },
    ""instruction_name"": {
      ""type"": ""string"",
      ""enum"": [
        ""add_stream"",
        ""ADD_STREAM"",
        ""remove_stream"",
        ""REMOVE_STREAM"",
        ""begin_record"",
        ""BEGIN_RECORD"",
        ""end_record"",
        ""END_RECORD"",
        ""pause_record"",
        ""PAUSE_RECORD"",
        ""resume_record"",
        ""RESUME_RECORD"",
        ""begin_play"",
        ""BEGIN_PLAY"",
        ""pause_play"",
        ""PAUSE_PLAY"",
        ""resume_play"",
        ""RESUME_PLAY"",
        ""end_play"",
        ""END_PLAY"",
        ""media_direction"",
        ""MEDIA_DIRECTION"",
        ""video_layout"",
        ""VIDEO_LAYOUT"",
        ""list_participants"",
        ""LIST_PARTICIPANTS"",
        ""recording_rules"",
        ""RECORDING_RULES""
      ]
    },
    ""webContext"": {
      ""type"": ""object"",
      ""properties"": {
        ""info"": {
          ""$ref"": ""#/definitions/restricted_string_64""
        },
        ""url"": {
          ""$ref"": ""#/definitions/restricted_string_1024""
        }
      },
      ""required"": [
        ""url""
      ],
      ""additionalProperties"": false
    },
    ""recording_status"": {
      ""type"": ""boolean""
    },
    ""conversation_channel"": {
      ""type"": ""string"",
      ""enum"": [
        ""video"",
        ""audio""
      ]
    },
    ""uuid"": {
      ""type"": ""string"",
      ""pattern"": ""^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$""
    },
    ""id_derived_from_session"": {
      ""type"": ""string"",
      ""pattern"": ""^s-register-[a-zA-Z0-9_+/=-]{36}$""
    },
    ""correlation_id"": {
      ""type"": ""string"",
      ""pattern"": ""^[c|s][a-zA-Z0-9-]+$"",
      ""minLength"": 2,
      ""maxLength"": 32
    },
    ""restricted_string_64"": {
      ""type"": ""string"",
      ""minLength"": 0,
      ""maxLength"": 64,
      ""pattern"": ""^[a-zA-Z0-9@./_+=#*&: -]*$""
    },
    ""restricted_string_1024"": {
      ""type"": ""string"",
      ""minLength"": 0,
      ""maxLength"": 1024,
      ""pattern"": ""^[a-zA-Z0-9@./_+=#*&: -]*$""
    },
    ""free_text"": {
      ""type"": ""string""
    },
    ""version"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""maxLength"": 4,
      ""pattern"": ""^[0-9.]+$""
    }
  }
}";

        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(schemaJson);

            JSchema uriSchema = (JSchema)s.ExtensionData["definitions"]["uri"];
            JSchema restrictedString1024Schema = (JSchema)s.ExtensionData["definitions"]["restricted_string_1024"];

            Assert.AreEqual(restrictedString1024Schema, uriSchema);
        }
    }
}