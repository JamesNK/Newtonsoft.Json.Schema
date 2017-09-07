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
    public class Issue0063Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            #region Schema
            JSchema s = JSchema.Parse(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""definitions"": {
    ""GameResults.RoundMeasureContest"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""ConStatement"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""EffectOfAbstain"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""FullText"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""InfoUri"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""PassageThreshold"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""ProStatement"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""SummaryText"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.RoundMeasureType""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""RoundSubTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""HasRotation"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""OtherPointVariation"": {
          ""type"": ""string""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""PointVariation"": {
          ""$ref"": ""#/definitions/GameResults.PointVariation""
        },
        ""RoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RoundMeasureContest""
          ]
        }
      }
    },
    ""GameResults.RoundMeasureSelection"": {
      ""required"": [
        ""Selection""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Selection"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PointCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PointCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RoundMeasureSelection""
          ]
        }
      }
    },
    ""GameResults.RoundSelection"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PointCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PointCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RoundSelection""
          ]
        }
      }
    },
    ""GameResults.RoundStyle"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""ImageUris"": {
          ""items"": {
            ""type"": ""string"",
            ""format"": ""uri""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Parties"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.Team""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GpUnits"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ReportingDevice""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ReportingUnit""
              }
            ]
          },
          ""minItems"": 1,
          ""type"": ""array""
        },
        ""OrderedContests"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RoundStyle""
          ]
        }
      }
    },
    ""GameResults.Player"": {
      ""required"": [
        ""RoundName""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""RoundName"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""FileDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""IsIncumbent"": {
          ""type"": ""boolean""
        },
        ""IsTopTicket"": {
          ""type"": ""boolean""
        },
        ""Clan"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.Team""
            }
          ]
        },
        ""Person"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PostGameStatus"": {
          ""$ref"": ""#/definitions/GameResults.PlayerPostGameStatus""
        },
        ""PreGameStatus"": {
          ""$ref"": ""#/definitions/GameResults.PlayerPreGameStatus""
        },
        ""PlayerSelection"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerSelection""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RetentionContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RetentionContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Player""
          ]
        }
      }
    },
    ""GameResults.PlayerContest"": {
      ""required"": [
        ""PointsAllowed""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""NumberElected"": {
          ""type"": ""integer""
        },
        ""Offices"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PrimaryClan"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.Team""
            }
          ]
        },
        ""PointsAllowed"": {
          ""type"": ""integer""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""RoundSubTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""HasRotation"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""OtherPointVariation"": {
          ""type"": ""string""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""PointVariation"": {
          ""$ref"": ""#/definitions/GameResults.PointVariation""
        },
        ""RoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PlayerContest""
          ]
        }
      }
    },
    ""GameResults.PlayerSelection"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Players"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""EndorsementParties"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.Team""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""IsWriteIn"": {
          ""type"": ""boolean""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PointCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PointCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PlayerSelection""
          ]
        }
      }
    },
    ""GameResults.Team"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Contests"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ClanContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.PlayerContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RetentionContest""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Parties"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.Team""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""Color"": {
          ""type"": ""string""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""LogoUri"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""Name"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Player"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PlayerContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PlayerSelection"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerSelection""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ClanRegistration"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ClanRegistration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ClanSelection"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ClanSelection""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Person"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Team""
          ]
        }
      }
    },
    ""GameResults.ContactInformation"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""AddressLines"": {
          ""items"": {
            ""type"": ""string""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Directions"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Emails"": {
          ""items"": {
            ""type"": ""string""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Faxes"": {
          ""items"": {
            ""type"": ""string""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""Phones"": {
          ""items"": {
            ""type"": ""string""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Uris"": {
          ""items"": {
            ""type"": ""string"",
            ""format"": ""uri""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameAdministration"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameAdministration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""LatLng"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.LatLng""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Office"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Person"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ReportingUnit"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Schedules"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Schedule""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ContactInformation""
          ]
        }
      }
    },
    ""GameResults.Contest"": {
      ""required"": [
        ""Name""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""RoundSubTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""HasRotation"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""OtherPointVariation"": {
          ""type"": ""string""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""PointVariation"": {
          ""$ref"": ""#/definitions/GameResults.PointVariation""
        },
        ""RoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Contest""
          ]
        }
      }
    },
    ""GameResults.CountStatus"": {
      ""required"": [
        ""Status"",
        ""Type""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""OtherType"": {
          ""type"": ""string""
        },
        ""Status"": {
          ""$ref"": ""#/definitions/GameResults.CountItemStatus""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.CountItemType""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.CountStatus""
          ]
        }
      }
    },
    ""GameResults.Counts"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Device"": {
          ""$ref"": ""#/definitions/GameResults.Device""
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""IsSuppressedForPrivacy"": {
          ""type"": ""boolean""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.CountItemType""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Counts""
          ]
        }
      }
    },
    ""GameResults.Device"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Manufacturer"": {
          ""type"": ""string""
        },
        ""Model"": {
          ""type"": ""string""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.DeviceType""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Device""
          ]
        }
      }
    },
    ""GameResults.Game"": {
      ""required"": [
        ""EndDate"",
        ""Name"",
        ""StartDate"",
        ""Type""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameScope"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""EndDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""ExternalIdenitfiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""Name"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""StartDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.GameType""
        },
        ""RoundStyles"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Players"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Contests"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ClanContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.PlayerContest""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RetentionContest""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Game""
          ]
        }
      }
    },
    ""GameResults.GameAdministration"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""GameOfficialPeople"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ReportingUnit"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.GameAdministration""
          ]
        }
      }
    },
    ""GameResults.GameReport"": {
      ""required"": [
        ""Format"",
        ""GeneratedDate"",
        ""Issuer"",
        ""IssuerAbbreviation"",
        ""SequenceEnd"",
        ""SequenceStart"",
        ""Status"",
        ""VendorApplicationId""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""Format"": {
          ""$ref"": ""#/definitions/GameResults.ReportDetailLevel""
        },
        ""GeneratedDate"": {
          ""type"": ""string"",
          ""format"": ""date-time""
        },
        ""IsTest"": {
          ""type"": ""boolean""
        },
        ""Issuer"": {
          ""type"": ""string""
        },
        ""IssuerAbbreviation"": {
          ""type"": ""string""
        },
        ""Notes"": {
          ""type"": ""string""
        },
        ""People"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""SequenceEnd"": {
          ""type"": ""integer""
        },
        ""SequenceStart"": {
          ""type"": ""integer""
        },
        ""Signature"": {
          ""$ref"": ""#/definitions/Xmldsig.Signature""
        },
        ""Status"": {
          ""$ref"": ""#/definitions/GameResults.ResultsStatus""
        },
        ""TestType"": {
          ""type"": ""string""
        },
        ""VendorApplicationId"": {
          ""type"": ""string""
        },
        ""Games"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GpUnits"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ReportingDevice""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ReportingUnit""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OfficeGroups"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OfficeGroup""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Offices"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Parties"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.Team""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.GameReport""
          ]
        }
      }
    },
    ""GameResults.ExternalIdentifier"": {
      ""required"": [
        ""Type"",
        ""Value""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""OtherType"": {
          ""type"": ""string""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.IdentifierType""
        },
        ""Value"": {
          ""type"": ""string""
        },
        ""ExternalIdentifiers"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ExternalIdentifier""
          ]
        }
      }
    },
    ""GameResults.ExternalIdentifiers"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""ExternalIdentifiers"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ExternalIdentifier""
          },
          ""minItems"": 1,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ExternalIdentifiers""
          ]
        }
      }
    },
    ""GameResults.GpUnit"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""ComposingGpUnits"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ReportingDevice""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ReportingUnit""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Counts"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.SummaryCounts""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PointCounts""
            }
          ]
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.GpUnit""
          ]
        }
      }
    },
    ""GameResults.Hours"": {
      ""required"": [
        ""EndTime"",
        ""StartTime""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Day"": {
          ""$ref"": ""#/definitions/GameResults.DayType""
        },
        ""EndTime"": {
          ""type"": ""string"",
          ""format"": ""time""
        },
        ""StartTime"": {
          ""type"": ""string"",
          ""format"": ""time""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""Schedule"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Schedule""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Hours""
          ]
        }
      }
    },
    ""GameResults.InternationalizedText"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Texts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.LanguageString""
          },
          ""minItems"": 1,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.InternationalizedText""
          ]
        }
      }
    },
    ""GameResults.LanguageString"": {
      ""required"": [
        ""Content"",
        ""Language""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Content"": {
          ""type"": ""string""
        },
        ""InternationalizedText"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.InternationalizedText""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Language"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.LanguageString""
          ]
        }
      }
    },
    ""GameResults.LatLng"": {
      ""required"": [
        ""Latitude"",
        ""Longitude""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Latitude"": {
          ""type"": ""number""
        },
        ""Longitude"": {
          ""type"": ""number""
        },
        ""Source"": {
          ""type"": ""string""
        },
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.LatLng""
          ]
        }
      }
    },
    ""GameResults.Office"": {
      ""required"": [
        ""Name""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Contest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""FilingDeadline"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""IsPartisan"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""OfficeHolderPeople"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OfficeGroup"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OfficeGroup""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RetentionContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RetentionContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Term"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Term""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Office""
          ]
        }
      }
    },
    ""GameResults.OfficeGroup"": {
      ""required"": [
        ""Name""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Name"": {
          ""type"": ""string""
        },
        ""SubOfficeGroups"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OfficeGroup""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""OfficeGroup"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OfficeGroup""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Offices"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.OfficeGroup""
          ]
        }
      }
    },
    ""GameResults.OrderedContest"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""OrderedRoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.OrderedContest""
          ]
        }
      }
    },
    ""GameResults.Clan"": {
      ""required"": [
        ""Name""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""Color"": {
          ""type"": ""string""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""LogoUri"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""Name"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Player"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PlayerContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PlayerSelection"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PlayerSelection""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ClanRegistration"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ClanRegistration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ClanSelection"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ClanSelection""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Person"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Clan""
          ]
        }
      }
    },
    ""GameResults.ClanContest"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""@id"": {
          ""type"": ""integer""
        },
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""RoundSubTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""HasRotation"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""OtherPointVariation"": {
          ""type"": ""string""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""PointVariation"": {
          ""$ref"": ""#/definitions/GameResults.PointVariation""
        },
        ""RoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ClanContest""
          ]
        }
      }
    },
    ""GameResults.ClanRegistration"": {
      ""required"": [
        ""Count""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Count"": {
          ""type"": ""integer""
        },
        ""Clan"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.Team""
            }
          ]
        },
        ""ReportingUnit"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ClanRegistration""
          ]
        }
      }
    },
    ""GameResults.ClanSelection"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Parties"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.Team""
              }
            ]
          },
          ""minItems"": 1,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""PointCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.PointCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ClanSelection""
          ]
        }
      }
    },
    ""GameResults.Person"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""DateOfBirth"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""FirstName"": {
          ""type"": ""string""
        },
        ""FullName"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Gender"": {
          ""type"": ""string""
        },
        ""LastName"": {
          ""type"": ""string""
        },
        ""MiddleNames"": {
          ""items"": {
            ""type"": ""string""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Nickname"": {
          ""type"": ""string""
        },
        ""Clan"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.Team""
            }
          ]
        },
        ""Prefix"": {
          ""type"": ""string""
        },
        ""Profession"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Suffix"": {
          ""type"": ""string""
        },
        ""Title"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Player"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ContactInformations"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameAdministration"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameAdministration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Office"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ReportingUnit"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Person""
          ]
        }
      }
    },
    ""GameResults.ReportingDevice"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Device"": {
          ""$ref"": ""#/definitions/GameResults.Device""
        },
        ""SerialNumber"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""ComposingGpUnits"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ReportingDevice""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ReportingUnit""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Counts"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.SummaryCounts""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PointCounts""
            }
          ]
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ReportingDevice""
          ]
        }
      }
    },
    ""GameResults.ReportingUnit"": {
      ""required"": [
        ""Type""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Authorities"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Person""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""IsDistricted"": {
          ""type"": ""boolean""
        },
        ""IsMailOnly"": {
          ""type"": ""boolean""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnitType""
        },
        ""PointrsParticipated"": {
          ""type"": ""integer""
        },
        ""PointrsRegistered"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GameAdministration"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameAdministration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Office"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ClanRegistrations"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ClanRegistration""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""SpatialDimension"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SpatialDimension""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""ComposingGpUnits"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.ReportingDevice""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ReportingUnit""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""RoundStyle"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.RoundStyle""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Counts"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.SummaryCounts""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PointCounts""
            }
          ]
        },
        ""GameReport"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.GameReport""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ReportingUnit""
          ]
        }
      }
    },
    ""GameResults.RetentionContest"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Player"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Player""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Office"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""ConStatement"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""EffectOfAbstain"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""FullText"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""InfoUri"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""PassageThreshold"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""ProStatement"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""SummaryText"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.RoundMeasureType""
        },
        ""Abbreviation"": {
          ""type"": ""string""
        },
        ""RoundSubTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""RoundTitle"": {
          ""$ref"": ""#/definitions/GameResults.InternationalizedText""
        },
        ""CountStatuses"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.CountStatus""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""ElectoralDistrict"": {
          ""$ref"": ""#/definitions/GameResults.ReportingUnit""
        },
        ""ExternalIdentifiers"": {
          ""$ref"": ""#/definitions/GameResults.ExternalIdentifiers""
        },
        ""HasRotation"": {
          ""type"": ""boolean""
        },
        ""Name"": {
          ""type"": ""string""
        },
        ""OtherPointVariation"": {
          ""type"": ""string""
        },
        ""SequenceOrder"": {
          ""type"": ""integer""
        },
        ""SubUnitsReported"": {
          ""type"": ""integer""
        },
        ""SummaryCounts"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SummaryCounts""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""TotalSubUnits"": {
          ""type"": ""integer""
        },
        ""PointVariation"": {
          ""$ref"": ""#/definitions/GameResults.PointVariation""
        },
        ""RoundSelections"": {
          ""items"": {
            ""oneOf"": [
              {
                ""$ref"": ""#/definitions/GameResults.PlayerSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.ClanSelection""
              },
              {
                ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
              }
            ]
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Team"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Team""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Game"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Game""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""OrderedContest"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.OrderedContest""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RetentionContest""
          ]
        }
      }
    },
    ""GameResults.Schedule"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""EndDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""IsOnlyByAppointment"": {
          ""type"": ""boolean""
        },
        ""IsOrByAppointment"": {
          ""type"": ""boolean""
        },
        ""IsSubjectToChange"": {
          ""type"": ""boolean""
        },
        ""StartDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""ContactInformation"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ContactInformation""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Hours"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Hours""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""Label"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Schedule""
          ]
        }
      }
    },
    ""GameResults.SpatialDimension"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""MapUri"": {
          ""type"": ""string"",
          ""format"": ""uri""
        },
        ""ReportingUnit"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.ReportingUnit""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""SpatialExtent"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SpatialExtent""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.SpatialDimension""
          ]
        }
      }
    },
    ""GameResults.SpatialExtent"": {
      ""required"": [
        ""Coordinates"",
        ""Format""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Coordinates"": {
          ""type"": ""string""
        },
        ""Format"": {
          ""$ref"": ""#/definitions/GameResults.GeoSpatialFormat""
        },
        ""SpatialDimension"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.SpatialDimension""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.SpatialExtent""
          ]
        }
      }
    },
    ""GameResults.SummaryCounts"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""RoundsCast"": {
          ""type"": ""integer""
        },
        ""RoundsOutstanding"": {
          ""type"": ""integer""
        },
        ""RoundsRejected"": {
          ""type"": ""integer""
        },
        ""Overvotes"": {
          ""type"": ""integer""
        },
        ""Undervotes"": {
          ""type"": ""integer""
        },
        ""WriteIns"": {
          ""type"": ""integer""
        },
        ""Contest"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ClanContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.PlayerContest""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RetentionContest""
            }
          ]
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""Device"": {
          ""$ref"": ""#/definitions/GameResults.Device""
        },
        ""IsSuppressedForPrivacy"": {
          ""type"": ""boolean""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.CountItemType""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.SummaryCounts""
          ]
        }
      }
    },
    ""GameResults.Term"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""EndDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""StartDate"": {
          ""type"": ""string"",
          ""format"": ""date""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.OfficeTermType""
        },
        ""Office"": {
          ""items"": {
            ""$ref"": ""#/definitions/GameResults.Office""
          },
          ""minItems"": 0,
          ""type"": ""array""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.Term""
          ]
        }
      }
    },
    ""GameResults.PointCounts"": {
      ""required"": [
        ""Count""
      ],
      ""additionalProperties"": false,
      ""properties"": {
        ""Count"": {
          ""type"": ""number""
        },
        ""RoundSelection"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.PlayerSelection""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ClanSelection""
            },
            {
              ""$ref"": ""#/definitions/GameResults.RoundMeasureSelection""
            }
          ]
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""Device"": {
          ""$ref"": ""#/definitions/GameResults.Device""
        },
        ""GpUnit"": {
          ""oneOf"": [
            {
              ""$ref"": ""#/definitions/GameResults.ReportingDevice""
            },
            {
              ""$ref"": ""#/definitions/GameResults.ReportingUnit""
            }
          ]
        },
        ""IsSuppressedForPrivacy"": {
          ""type"": ""boolean""
        },
        ""OtherType"": {
          ""type"": ""string""
        },
        ""Type"": {
          ""$ref"": ""#/definitions/GameResults.CountItemType""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PointCounts""
          ]
        }
      }
    },
    ""GameResults.RoundMeasureType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.RoundMeasureType""
          ]
        }
      }
    },
    ""GameResults.PlayerPostGameStatus"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PlayerPostGameStatus""
          ]
        }
      }
    },
    ""GameResults.PlayerPreGameStatus"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PlayerPreGameStatus""
          ]
        }
      }
    },
    ""GameResults.CountItemStatus"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.CountItemStatus""
          ]
        }
      }
    },
    ""GameResults.CountItemType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.CountItemType""
          ]
        }
      }
    },
    ""GameResults.DayType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.DayType""
          ]
        }
      }
    },
    ""GameResults.DeviceType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.DeviceType""
          ]
        }
      }
    },
    ""GameResults.GameType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.GameType""
          ]
        }
      }
    },
    ""GameResults.GeoSpatialFormat"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.GeoSpatialFormat""
          ]
        }
      }
    },
    ""GameResults.IdentifierType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.IdentifierType""
          ]
        }
      }
    },
    ""GameResults.OfficeTermType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.OfficeTermType""
          ]
        }
      }
    },
    ""GameResults.ReportDetailLevel"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ReportDetailLevel""
          ]
        }
      }
    },
    ""GameResults.ReportingUnitType"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ReportingUnitType""
          ]
        }
      }
    },
    ""GameResults.ResultsStatus"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.ResultsStatus""
          ]
        }
      }
    },
    ""GameResults.PointVariation"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Value"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.PointVariation""
          ]
        }
      }
    },
    ""GameResults.HtmlColorString"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Pattern"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.HtmlColorString""
          ]
        }
      }
    },
    ""GameResults.TimeWithZone"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""Pattern"": {
          ""type"": ""string""
        },
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""GameResults.TimeWithZone""
          ]
        }
      }
    },
    ""Xmldsig.Signature"": {
      ""additionalProperties"": false,
      ""properties"": {
        ""@id"": {
          ""type"": ""integer""
        },
        ""@type"": {
          ""enum"": [
            ""Xmldsig.Signature""
          ]
        }
      }
    }
  },
  ""oneOf"": [
    {
      ""$ref"": ""#/definitions/GameResults.GameReport""
    }
  ]
}");
            #endregion

            #region Json
            string json = @"{
  ""@type"": ""GameResults.GameReport"",
  ""@id"": 1,
  ""Games"": [
    {
      ""@type"": ""GameResults.Game"",
      ""@id"": 2,
      ""Contests"": [
        {
          ""@type"": ""GameResults.PlayerContest"",
          ""@id"": 3,
          ""RoundSelections"": [
            {
              ""@type"": ""GameResults.PlayerSelection"",
              ""@id"": 4,
              ""PointCounts"": [
                {
                  ""@type"": ""GameResults.PointCounts"",
                  ""@id"": 5,
                  ""Type"": ""Total"",
                  ""Count"": 1000
                }
              ],
              ""Players"": [
                {
                  ""@type"": ""GameResults.Player"",
                  ""@id"": 6,
                  ""RoundName"": {
                    ""@type"": ""GameResults.InternationalizedText"",
                    ""@id"": 7,
                    ""Texts"": [
                      {
                        ""@type"": ""GameResults.LanguageString"",
                        ""@id"": 8,
                        ""Content"": ""West"",
                        ""Language"": ""en""
                      }
                    ]
                  }
                }
              ]
            },
            {
              ""@type"": ""GameResults.PlayerSelection"",
              ""@id"": 9,
              ""PointCounts"": [
                {
                  ""@type"": ""GameResults.PointCounts"",
                  ""@id"": 10,
                  ""Type"": ""Total"",
                  ""Count"": 100
                }
              ],
              ""Players"": [
                {
                  ""@type"": ""GameResults.Player"",
                  ""@id"": 11,
                  ""RoundName"": {
                    ""@type"": ""GameResults.InternationalizedText"",
                    ""@id"": 12,
                    ""Texts"": [
                      {
                        ""@type"": ""GameResults.LanguageString"",
                        ""@id"": 13,
                        ""Content"": ""East"",
                        ""Language"": ""en""
                      }
                    ]
                  }
                }
              ]
            }
          ],
          ""SummaryCounts"": [
            {
              ""@type"": ""GameResults.SummaryCounts"",
              ""@id"": 53
            }
          ],
          ""Name"": ""Overwatch Match"",
          ""Offices"": [
            {
              ""@type"": ""GameResults.Office"",
              ""@id"": 14,
              ""Name"": {
                ""@type"": ""GameResults.InternationalizedText"",
                ""@id"": 15,
                ""Texts"": [
                  {
                    ""@type"": ""GameResults.LanguageString"",
                    ""@id"": 16,
                    ""Content"": ""Mayor"",
                    ""Language"": ""en""
                  }
                ]
              }
            }
          ],
          ""PointsAllowed"": 1
        },
        {
          ""@type"": ""GameResults.PlayerContest"",
          ""@id"": 17,
          ""RoundSelections"": [
            {
              ""@type"": ""GameResults.PlayerSelection"",
              ""@id"": 18,
              ""PointCounts"": [
                {
                  ""@type"": ""GameResults.PointCounts"",
                  ""@id"": 19,
                  ""Type"": ""Total"",
                  ""Count"": 1000
                }
              ],
              ""Players"": [
                {
                  ""@type"": ""GameResults.Player"",
                  ""@id"": 20,
                  ""RoundName"": {
                    ""@type"": ""GameResults.InternationalizedText"",
                    ""@id"": 21,
                    ""Texts"": [
                      {
                        ""@type"": ""GameResults.LanguageString"",
                        ""@id"": 22,
                        ""Content"": ""North"",
                        ""Language"": ""en""
                      }
                    ]
                  }
                }
              ]
            }
          ],
          ""Name"": ""Controller Contest"",
          ""NumberElected"": 2,
          ""PointsAllowed"": 4,
          ""Offices"": [
            {
              ""@type"": ""GameResults.Office"",
              ""@id"": 23,
              ""Name"": {
                ""@type"": ""GameResults.InternationalizedText"",
                ""@id"": 24,
                ""Texts"": [
                  {
                    ""@type"": ""GameResults.LanguageString"",
                    ""@id"": 25,
                    ""Content"": ""Comptroller"",
                    ""Language"": ""en""
                  }
                ]
              }
            }
          ]
        }
      ],
      ""RoundStyles"": [
        {
          ""@type"": ""GameResults.RoundStyle"",
          ""@id"": 47,
          ""OrderedContests"": [
            {
              ""@type"": ""GameResults.OrderedContest"",
              ""@id"": 48
            }
          ]
        }
      ],
      ""Name"": {
        ""@type"": ""GameResults.InternationalizedText"",
        ""@id"": 26,
        ""Texts"": [
          {
            ""@type"": ""GameResults.LanguageString"",
            ""@id"": 27,
            ""Content"": ""Special Game"",
            ""Language"": ""en""
          },
          {
            ""@type"": ""GameResults.LanguageString"",
            ""@id"": 28,
            ""Content"": ""Elección Especial"",
            ""Language"": ""es""
          }
        ]
      },
      ""Type"": ""dgdgdfgd"",
      ""EndDate"": ""2016-10-12"",
      ""StartDate"": ""2016-10-13""
    }
  ],
  ""People"": [
    {
      ""@type"": ""GameResults.Person"",
      ""@id"": 29,
      ""ContactInformations"": [
        {
          ""@type"": ""GameResults.ContactInformation"",
          ""@id"": 30,
          ""Schedules"": [
            {
              ""@type"": ""GameResults.Schedule"",
              ""@id"": 31,
              ""Hours"": [
                {
                  ""@type"": ""GameResults.Hours"",
                  ""@id"": 32,
                  ""StartTime"": ""18:50:00-04:00"",
                  ""EndTime"": ""18:55:00-04:00""
                }
              ]
            }
          ],
          ""LatLng"": [
            {
              ""@type"": ""GameResults.LatLng"",
              ""@id"": 32,
              ""Latitude"": 12.2,
              ""Longitude"": 13.6
            }
          ]
        }
      ]
    }
  ],
  ""Offices"": [
    {
      ""@type"": ""GameResults.Office"",
      ""@id"": 35,
      ""Name"": ""xyz"",
      ""ContactInformation"": [
        {
          ""@type"": ""GameResults.ContactInformation"",
          ""@id"": 36,
          ""Schedules"": [
            {
              ""@type"": ""GameResults.Schedule"",
              ""@id"": 37,
              ""Hours"": [
                {
                  ""@type"": ""GameResults.Hours"",
                  ""@id"": 38,
                  ""StartTime"": ""18:50:00-04:00"",
                  ""EndTime"": ""18:55:00-04:00""
                }
              ]
            }
          ],
          ""LatLng"": [
            {
              ""@type"": ""GameResults.LatLng"",
              ""@id"": 32,
              ""Latitude"": 12.2,
              ""Longitude"": 13.6
            }
          ]
        }
      ],
      ""ElectoralDistrict"": [
        {
          ""@type"": ""GameResults.ReportingUnit"",
          ""@id"": 49,
          ""Type"": ""xyz"",
          ""SpatialDimension"": [
            {
              ""@type"": ""GameResults.SpatialDimension"",
              ""@id"": 51,
              ""SpatialExtent"": [
                {
                  ""@type"": ""GameResults.SpatialExtent"",
                  ""@id"": 52,
                  ""Coordinates"": ""xyz"",
                  ""Format"": ""xyz""
                }
              ]
            }
          ],
          ""ClanRegistrations"": [
            {
              ""@type"": ""GameResults.ClanRegistration"",
              ""@id"": 51,
              ""Count"": 12
            }
          ],
          ""GameAdministration"": [
            {
              ""@type"": ""GameResults.GameAdministration"",
              ""@id"": 53
            }
          ]
        }
      ],
      ""Term"": [
        {
          ""@type"": ""GameResults.Term"",
          ""@id"": 49
        }
      ],
      ""RetentionContest"": [
        {
          ""@type"": ""GameResults.RetentionContest"",
          ""@id"": 54
        }
      ]
    }
  ],
  ""OfficeGroups"": [
    {
      ""@type"": ""GameResults.OfficeGroup"",
      ""@id"": 39,
      ""Name"": ""dffdg"",
      ""SubOfficeGroups"": [
        {
          ""@type"": ""GameResults.OfficeGroup"",
          ""@id"": 40,
          ""Name"": ""xyz""
        }
      ]
    }
  ],
  ""GpUnits"": [
    {
      ""@type"": ""GameResults.ReportingDevice"",
      ""@id"": 41
    }
  ],
  ""Parties"": [
    {
      ""@type"": ""GameResults.Team"",
      ""@id"": 41,
      ""Name"": ""xyz""
    }
  ],
  ""GeneratedDate"": ""1996-12-19T16:39:57-08:00"",
  ""Issuer"": ""xyz"",
  ""IssuerAbbreviation"": ""xyz"",
  ""SequenceEnd"": 2,
  ""SequenceStart"": 4,
  ""Status"": ""GameResults.ResultsStatus"",
  ""VendorApplicationId"": ""xyz"",
  ""Format"": ""SummaryContest""
}";
            #endregion

            JObject o = JObject.Parse(json);

            IList<ValidationError> errors = new List<ValidationError>();
            o.IsValid(s, out errors);

            Assert.AreEqual(0, errors.Count);
        }
    }
}