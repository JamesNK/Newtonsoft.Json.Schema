#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
    public class Issue0107Test : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaReaderSettings settings = new JSchemaReaderSettings();
            settings.ValidationEventHandler += (sender, args) => { };
            JSchema schema = JSchema.Parse(SchemaJson, settings);

            JSchema propSchema = schema.Properties["UPVersion"];
            Assert.AreEqual(new Uri("//UP/", UriKind.RelativeOrAbsolute), propSchema.Id);
        }

        private const string SchemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-06/schema#"",
  ""title"": ""SRS"",
  ""description"": ""A test of SRS"",
  ""type"": ""object"",
  ""properties"": {
    ""UPVersion"": {
      ""id"": ""//UP/"",
      ""type"": ""integer""
    },
    ""flowTechId"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""issuerApp"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 40
    },
    ""srcSysVersNum"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 40
    },
    ""closingstring"": {
      ""id"": ""//UP/"",
      ""type"": ""string""
    },
    ""accountingstring"": {
      ""id"": ""//UP/"",
      ""type"": ""string""
    },
    ""profile"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 3
    },
    ""sequence"": {
      ""id"": ""//UP/"",
      ""type"": ""integer""
    },
    ""creationstring"": {
      ""id"": ""//UP/"",
      ""type"": ""string""
    },
    ""issuerAppContactName"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""issuerAppContactMail"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""issuerAppBackupName"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""issuerAppBackupMail"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""rulesSetting"": {
      ""id"": ""//UP/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""entityChksm"": {
      ""id"": ""//UP/"",
      ""type"": ""integer""
    },
    ""entities"": {
      ""entityId"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 6
      },
      ""repCurr"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 3
      },
      ""regCtry"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 2
      },
      ""typIssuer"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 2
      },
      ""typCompany"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""entName"": {
        ""id"": ""//UP/entities/entity/"",
        ""type"": ""string"",
        ""maxLength"": 100
      }
    },
    ""acquisstring"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string""
    },
    ""RMPMInd"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""integer""
    },
    ""entityContactName"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""entityContactMail"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""entityBackupName"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""entityBackupMail"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""paramFU"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 36
    },
    ""riad"": {
      ""id"": ""//UP/entities/entity/"",
      ""type"": ""string"",
      ""maxLength"": 30
    },
    ""settingsAccountingApp"": {
      ""treatment"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 10
      },
      ""language"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 2
      },
      ""phase"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 100
      },
      ""groupentity"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""groupphase"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""groupwarning"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""groupaudit"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 100
      },
      ""bali"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""baliauto"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""wac"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""monitoring"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      },
      ""rulecode"": {
        ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
        ""type"": ""string"",
        ""maxLength"": 5
      }
    },
    ""platform"": {
      ""id"": ""//UP/entities/entity/settingsAccountingApp/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""positions"": {
      ""cnt"": {
        ""id"": ""//UP/entities/entity/positions/pos/"",
        ""type"": ""integer""
      }
    },
    ""holdApp"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""trCurr"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 3
    },
    ""amntTC"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""number""
    },
    ""amntRC"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""number""
    },
    ""posId"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""mastId"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 100
    },
    ""univAcc"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""locGaapAcc"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""locGaap"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""locGaapAcc2"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""locGaap2"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""locGaapAcc3"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""locGaap3"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""locAcc"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""locProd"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""locBus"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""locSect"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""custSeg"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""pma"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 15
    },
    ""matisseGestNom"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""amntType"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 1
    },
    ""annVolum"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""liquidity"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""dureeres"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""amntNat"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 10
    },
    ""nettingNat"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""reasonClass"": {
        ""id"": ""//UP/entities/entity/positions/pos/"",
        ""type"": ""string"",
        ""maxLength"": 2
      }
    },
    ""guaranteeAcquisitionInd"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""cntpCtry"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""guarantorCtry"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""issuerCtry"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    },
    ""forbSt"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 3
    },
    ""arrierespmt"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""securCode"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 30
    },
    ""flow"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""financt"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 4
    },
    ""strategieias"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""douteux"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""npe"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""sousprod"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""typOp"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""sousjac"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""stratcouv"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""marche"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""derivative"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""detention"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""grossAmntRC"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""number""
    },
    ""tieremett"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""tierctpa"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""socemett"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""partner"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""donnord"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""tiergarant"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""correspondant"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""sectact"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""dureerespens"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""dureeresgar"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""dureeresemett"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 12
    },
    ""crgarant"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""cotation"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""matstring"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string""
    },
    ""strategiefr"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""fbe"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""fbeWProb"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""npeStRestruct"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 1
    },
    ""lmonctpa"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""lmonemett"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""dureeini"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""socemett_nconso"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""partner_nconso"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""donnord_nconso"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 8
    },
    ""garantrc"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""garantTyp"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""natopecb"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""eligib"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""particip"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""secnumber"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""number""
    },
    ""level"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""netting"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""clearinghouse"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""allow"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""garantee"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 6
    },
    ""weightingfactor"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""careng"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""engfin"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 5
    },
    ""custRateDiffInd"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""integer""
    },
    ""valuestring"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string""
    },
    ""insur1"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 20
    },
    ""flowType"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 1
    },
    ""baliId"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 35
    },
    ""observedAgentId"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 35
    },
    ""stage"": {
      ""id"": ""//UP/entities/entity/positions/pos/"",
      ""type"": ""string"",
      ""maxLength"": 2
    }
  },
  ""creditImpairedInd"": {
    ""id"": ""//UP/entities/entity/positions/pos/"",
    ""type"": ""integer""
  }
}";
    }
}