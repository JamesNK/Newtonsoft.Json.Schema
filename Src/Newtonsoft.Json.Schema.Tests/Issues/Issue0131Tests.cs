#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
    public class Issue0131Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(SchemaJson);

            JToken t = JToken.Parse(Json);

            Assert.AreEqual(false, t.IsValid(s, out IList<ValidationError> errorMessages));

            Assert.AreEqual(1, errorMessages.Count);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 1. Path '', line 1, position 1.", errorMessages[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("#/definitions/Bundle", UriKind.RelativeOrAbsolute), errorMessages[0].SchemaId);

            Assert.AreEqual(1, errorMessages[0].ChildErrors.Count);
            Assert.AreEqual("JSON does not match all schemas from 'allOf'. Invalid schema indexes: 1. Path 'entry[0]', line 5, position 5.", errorMessages[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("#/definitions/Bundle_Entry", UriKind.RelativeOrAbsolute), errorMessages[0].ChildErrors[0].SchemaId);

            Assert.AreEqual(1, errorMessages[0].ChildErrors[0].ChildErrors.Count);
            Assert.AreEqual("JSON is valid against more than one schema from 'oneOf'. Valid schema indexes: 38, 80, 81, 87, 101. Path 'entry[0].resource', line 7, position 19.", errorMessages[0].ChildErrors[0].ChildErrors[0].GetExtendedMessage());
            Assert.AreEqual(new Uri("#/definitions/ResourceList", UriKind.RelativeOrAbsolute), errorMessages[0].ChildErrors[0].ChildErrors[0].SchemaId);
        }

        #region JSON
        private const string Json = @"{
  ""resourceType"": ""Bundle"",
  ""type"": ""transaction"",
  ""entry"": [
    {
      ""fullUrl"": ""urn:uuid:3e1af5b90d9c468b9e98a92d370b4aa0"",
      ""resource"": {
        ""resourceType"": ""Practitioner"",
        ""id"": ""3e1af5b90d9c468b9e98a92d370b4aa0"",
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/"",
                  ""code"": ""PP"",
                  ""display"": ""Role-ROL""
                }
              ],
              ""text"": ""Role-ROL""
            },
            ""system"": ""http://global-health.com/iPM"",
            ""value"": ""PP""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""MAIN""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""LAA007""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""PROVN""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""095410EY""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""PROV""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/NSP"",
            ""value"": ""098765EY""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""LOCAL""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""LBB008""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""NATGP""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""LBB008""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""COMPOSITE""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""LBB008MORN001""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://global-health.com/iPM/prof_carers"",
                  ""code"": ""HEALTHLINK""
                }
              ]
            },
            ""system"": ""http://global-health.com/iPM/MORN001"",
            ""value"": ""MORNVILL_MORN001""
          }
        ],
        ""name"": [
          {
            ""use"": ""official"",
            ""family"": ""Lazaridis"",
            ""given"": [
              ""Alex""
            ],
            ""prefix"": [
              ""DR""
            ]
          }
        ],
        ""telecom"": [
          {
            ""system"": ""phone"",
            ""value"": ""5678 3600"",
            ""use"": ""work"",
            ""rank"": 1
          },
          {
            ""system"": ""fax"",
            ""value"": ""5678 3611"",
            ""use"": ""work""
          },
          {
            ""system"": ""phone"",
            ""value"": ""03 5678 3600"",
            ""use"": ""work""
          },
          {
            ""system"": ""fax"",
            ""value"": ""03 5678 3611"",
            ""use"": ""work""
          }
        ],
        ""address"": [
          {
            ""use"": ""work"",
            ""line"": [
              ""241 Mary St""
            ],
            ""city"": ""Mornington1"",
            ""state"": ""VIC"",
            ""postalCode"": ""3931"",
            ""country"": ""Australia""
          },
          {
            ""use"": ""work"",
            ""line"": [
              ""Shop 17"",
              ""241 Main Street""
            ],
            ""city"": ""MORNINGTON1"",
            ""state"": ""VIC"",
            ""postalCode"": ""3931"",
            ""country"": ""Australia""
          }
        ]
      },
      ""request"": {
        ""method"": ""PUT"",
        ""url"": ""Practitioner/?identifier=LBB008""
      }
    }
  ]
}";
        #endregion

        #region Schema
        private const string SchemaJson = @"{
	""$schema"": ""http://json-schema.org/draft-04/schema#"",
	""$ref"": ""#/definitions/Bundle"",
	""definitions"": {
		""Account"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Account""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""active"": {
							""$ref"": ""#/definitions/Period""
						},
						""balance"": {
							""$ref"": ""#/definitions/Money""
						},
						""coverage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Account_Coverage""
							}
						},
						""owner"": {
							""$ref"": ""#/definitions/Reference""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""guarantor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Account_Guarantor""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Account_Coverage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""coverage"": {
							""description"": ""The party(s) that are responsible for payment (or part of) of charges applied to this account (including self-pay).\n\nA coverage may only be resposible for specific types of charges, and the sequence of the coverages in the account could be important when processing billing."",
							""$ref"": ""#/definitions/Reference""
						},
						""priority"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""coverage""]
				}
			]
		},
		""Account_Guarantor"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""party"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onHold"": {
							""type"": ""boolean""
						},
						""_onHold"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""party""]
				}
			]
		},
		""ActivityDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ActivityDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contributor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contributor""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""library"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""kind"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""timingTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""timingDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_timingDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""timingRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ActivityDefinition_Participant""
							}
						},
						""productReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""productCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""dosage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Dosage""
							}
						},
						""bodySite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""transform"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dynamicValue"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ActivityDefinition_DynamicValue""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ActivityDefinition_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""ActivityDefinition_DynamicValue"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Address"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""use"": {
							""enum"": [""home"", ""work"", ""temp"", ""old""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""postal"", ""physical"", ""both""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""line"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_line"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""city"": {
							""type"": ""string""
						},
						""_city"": {
							""$ref"": ""#/definitions/Element""
						},
						""district"": {
							""type"": ""string""
						},
						""_district"": {
							""$ref"": ""#/definitions/Element""
						},
						""state"": {
							""type"": ""string""
						},
						""_state"": {
							""$ref"": ""#/definitions/Element""
						},
						""postalCode"": {
							""type"": ""string""
						},
						""_postalCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""country"": {
							""type"": ""string""
						},
						""_country"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""AdverseEvent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""AdverseEvent""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""category"": {
							""enum"": [""AE"", ""PAE""],
							""type"": ""string""
						},
						""_category"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""reaction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""seriousness"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""outcome"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""recorder"": {
							""$ref"": ""#/definitions/Reference""
						},
						""eventParticipant"": {
							""$ref"": ""#/definitions/Reference""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""suspectEntity"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/AdverseEvent_SuspectEntity""
							}
						},
						""subjectMedicalHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""referenceDocument"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""study"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""AdverseEvent_SuspectEntity"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""instance"": {
							""$ref"": ""#/definitions/Reference""
						},
						""causality"": {
							""enum"": [""causality1"", ""causality2""],
							""type"": ""string""
						},
						""_causality"": {
							""$ref"": ""#/definitions/Element""
						},
						""causalityAssessment"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""causalityProductRelatedness"": {
							""type"": ""string""
						},
						""_causalityProductRelatedness"": {
							""$ref"": ""#/definitions/Element""
						},
						""causalityMethod"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""causalityAuthor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""causalityResult"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""instance""]
				}
			]
		},
		""Age"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Quantity""
				}, {
					""properties"": {}
				}
			]
		},
		""AllergyIntolerance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""AllergyIntolerance""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""clinicalStatus"": {
							""enum"": [""active"", ""inactive"", ""resolved""],
							""type"": ""string""
						},
						""_clinicalStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""verificationStatus"": {
							""enum"": [""unconfirmed"", ""confirmed"", ""refuted"", ""entered-in-error""],
							""type"": ""string""
						},
						""_verificationStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""allergy"", ""intolerance""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""enum"": [""food"", ""medication"", ""environment"", ""biologic""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""criticality"": {
							""enum"": [""low"", ""high"", ""unable-to-assess""],
							""type"": ""string""
						},
						""_criticality"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""description"": ""Code for an allergy or intolerance statement (either a positive or a negated/excluded statement).  This may be a code for a substance or pharmaceutical product that is considered to be responsible for the adverse reaction risk (e.g., \""Latex\""), an allergy or intolerance condition (e.g., \""Latex allergy\""), or a negated/excluded code for a specific substance or class (e.g., \""No latex allergy\"") or a general or categorical negated statement (e.g.,  \""No known allergy\"", \""No known drug allergies\"")."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onsetDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_onsetDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""onsetAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""onsetPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""onsetRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""onsetString"": {
							""type"": ""string""
						},
						""_onsetString"": {
							""$ref"": ""#/definitions/Element""
						},
						""assertedDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_assertedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""recorder"": {
							""$ref"": ""#/definitions/Reference""
						},
						""asserter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""lastOccurrence"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_lastOccurrence"": {
							""$ref"": ""#/definitions/Element""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""reaction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/AllergyIntolerance_Reaction""
							}
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""AllergyIntolerance_Reaction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""substance"": {
							""description"": ""Identification of the specific substance (or pharmaceutical product) considered to be responsible for the Adverse Reaction event. Note: the substance for a specific reaction may be different from the substance identified as the cause of the risk, but it must be consistent with it. For instance, it may be a more specific substance (e.g. a brand medication) or a composite product that includes the identified substance. It must be clinically safe to only process the 'code' and ignore the 'reaction.substance'."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""manifestation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""onset"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_onset"": {
							""$ref"": ""#/definitions/Element""
						},
						""severity"": {
							""enum"": [""mild"", ""moderate"", ""severe""],
							""type"": ""string""
						},
						""_severity"": {
							""$ref"": ""#/definitions/Element""
						},
						""exposureRoute"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""manifestation""]
				}
			]
		},
		""Annotation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""authorReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""authorString"": {
							""type"": ""string""
						},
						""_authorString"": {
							""$ref"": ""#/definitions/Element""
						},
						""time"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_time"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Appointment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Appointment""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""proposed"", ""pending"", ""booked"", ""arrived"", ""fulfilled"", ""cancelled"", ""noshow"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""serviceCategory"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""serviceType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""appointmentType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""indication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""priority"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""supportingInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""start"": {
							""type"": ""string""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""string""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""minutesDuration"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_minutesDuration"": {
							""$ref"": ""#/definitions/Element""
						},
						""slot"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""incomingReferral"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Appointment_Participant""
							}
						},
						""requestedPeriod"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Period""
							}
						}
					},
					""required"": [""participant"", ""resourceType""]
				}
			]
		},
		""Appointment_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""required"": {
							""enum"": [""required"", ""optional"", ""information-only""],
							""type"": ""string""
						},
						""_required"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""accepted"", ""declined"", ""tentative"", ""needs-action""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""AppointmentResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""AppointmentResponse""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""appointment"": {
							""$ref"": ""#/definitions/Reference""
						},
						""start"": {
							""type"": ""string""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""string""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""participantType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""participantStatus"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_participantStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""appointment"", ""resourceType""]
				}
			]
		},
		""Attachment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""contentType"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_contentType"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""data"": {
							""type"": ""string""
						},
						""_data"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""size"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_size"": {
							""$ref"": ""#/definitions/Element""
						},
						""hash"": {
							""type"": ""string""
						},
						""_hash"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""creation"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_creation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""AuditEvent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""AuditEvent""]
						},
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""subtype"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""action"": {
							""enum"": [""C"", ""R"", ""U"", ""D"", ""E""],
							""type"": ""string""
						},
						""_action"": {
							""$ref"": ""#/definitions/Element""
						},
						""recorded"": {
							""type"": ""string""
						},
						""_recorded"": {
							""$ref"": ""#/definitions/Element""
						},
						""outcome"": {
							""enum"": [""0"", ""4"", ""8"", ""12""],
							""type"": ""string""
						},
						""_outcome"": {
							""$ref"": ""#/definitions/Element""
						},
						""outcomeDesc"": {
							""type"": ""string""
						},
						""_outcomeDesc"": {
							""$ref"": ""#/definitions/Element""
						},
						""purposeOfEvent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""agent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/AuditEvent_Agent""
							}
						},
						""source"": {
							""$ref"": ""#/definitions/AuditEvent_Source""
						},
						""entity"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/AuditEvent_Entity""
							}
						}
					},
					""required"": [""agent"", ""source"", ""type"", ""resourceType""]
				}
			]
		},
		""AuditEvent_Agent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""userId"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""altId"": {
							""type"": ""string""
						},
						""_altId"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestor"": {
							""type"": ""boolean""
						},
						""_requestor"": {
							""$ref"": ""#/definitions/Element""
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""policy"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_policy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""media"": {
							""$ref"": ""#/definitions/Coding""
						},
						""network"": {
							""$ref"": ""#/definitions/AuditEvent_Network""
						},
						""purposeOfUse"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					}
				}
			]
		},
		""AuditEvent_Network"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""address"": {
							""type"": ""string""
						},
						""_address"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""1"", ""2"", ""3"", ""4"", ""5""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""AuditEvent_Source"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""site"": {
							""type"": ""string""
						},
						""_site"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						}
					},
					""required"": [""identifier""]
				}
			]
		},
		""AuditEvent_Entity"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""role"": {
							""$ref"": ""#/definitions/Coding""
						},
						""lifecycle"": {
							""$ref"": ""#/definitions/Coding""
						},
						""securityLabel"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""query"": {
							""type"": ""string""
						},
						""_query"": {
							""$ref"": ""#/definitions/Element""
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/AuditEvent_Detail""
							}
						}
					}
				}
			]
		},
		""AuditEvent_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""BackboneElement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""modifierExtension"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Extension""
							}
						}
					}
				}
			]
		},
		""Basic"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Basic""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""description"": ""Identifies the patient, practitioner, device or any other resource that is the \""focus\"" of this resource."",
							""$ref"": ""#/definitions/Reference""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""code"", ""resourceType""]
				}
			]
		},
		""Binary"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Resource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Binary""]
						},
						""contentType"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_contentType"": {
							""$ref"": ""#/definitions/Element""
						},
						""securityContext"": {
							""$ref"": ""#/definitions/Reference""
						},
						""content"": {
							""type"": ""string""
						},
						""_content"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""BodySite"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""BodySite""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""qualifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""image"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""Bundle"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Resource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Bundle""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""type"": {
							""enum"": [""document"", ""message"", ""transaction"", ""transaction-response"", ""batch"", ""batch-response"", ""history"", ""searchset"", ""collection""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""total"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_total"": {
							""$ref"": ""#/definitions/Element""
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Bundle_Link""
							}
						},
						""entry"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Bundle_Entry""
							}
						},
						""signature"": {
							""$ref"": ""#/definitions/Signature""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Bundle_Link"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""relation"": {
							""description"": ""A name which details the functional use for this link - see [http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1](http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1)."",
							""type"": ""string""
						},
						""_relation"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Bundle_Entry"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Bundle_Link""
							}
						},
						""fullUrl"": {
							""description"": ""The Absolute URL for the resource.  The fullUrl SHALL not disagree with the id in the resource. The fullUrl is a version independent reference to the resource. The fullUrl element SHALL have a value except that: \n* fullUrl can be empty on a POST (although it does not need to when specifying a temporary id for reference in the bundle)\n* Results from operations might involve resources that are not identified."",
							""type"": ""string""
						},
						""_fullUrl"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""$ref"": ""#/definitions/ResourceList""
						},
						""search"": {
							""$ref"": ""#/definitions/Bundle_Search""
						},
						""request"": {
							""$ref"": ""#/definitions/Bundle_Request""
						},
						""response"": {
							""$ref"": ""#/definitions/Bundle_Response""
						}
					}
				}
			]
		},
		""Bundle_Search"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""mode"": {
							""enum"": [""match"", ""include"", ""outcome""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""score"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_score"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Bundle_Request"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""method"": {
							""enum"": [""GET"", ""POST"", ""PUT"", ""DELETE""],
							""type"": ""string""
						},
						""_method"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""ifNoneMatch"": {
							""description"": ""If the ETag values match, return a 304 Not Modified status. See the API documentation for [\""Conditional Read\""](http.html#cread)."",
							""type"": ""string""
						},
						""_ifNoneMatch"": {
							""$ref"": ""#/definitions/Element""
						},
						""ifModifiedSince"": {
							""description"": ""Only perform the operation if the last updated date matches. See the API documentation for [\""Conditional Read\""](http.html#cread)."",
							""type"": ""string""
						},
						""_ifModifiedSince"": {
							""$ref"": ""#/definitions/Element""
						},
						""ifMatch"": {
							""description"": ""Only perform the operation if the Etag value matches. For more information, see the API section [\""Managing Resource Contention\""](http.html#concurrency)."",
							""type"": ""string""
						},
						""_ifMatch"": {
							""$ref"": ""#/definitions/Element""
						},
						""ifNoneExist"": {
							""description"": ""Instruct the server not to perform the create if a specified resource already exists. For further information, see the API documentation for [\""Conditional Create\""](http.html#ccreate). This is just the query portion of the URL - what follows the \""?\"" (not including the \""?\"")."",
							""type"": ""string""
						},
						""_ifNoneExist"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Bundle_Response"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""status"": {
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""location"": {
							""type"": ""string""
						},
						""_location"": {
							""$ref"": ""#/definitions/Element""
						},
						""etag"": {
							""type"": ""string""
						},
						""_etag"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastModified"": {
							""type"": ""string""
						},
						""_lastModified"": {
							""$ref"": ""#/definitions/Element""
						},
						""outcome"": {
							""$ref"": ""#/definitions/ResourceList""
						}
					}
				}
			]
		},
		""CapabilityStatement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CapabilityStatement""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""kind"": {
							""enum"": [""instance"", ""capability"", ""requirements""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""instantiates"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_instantiates"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""software"": {
							""$ref"": ""#/definitions/CapabilityStatement_Software""
						},
						""implementation"": {
							""$ref"": ""#/definitions/CapabilityStatement_Implementation""
						},
						""fhirVersion"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_fhirVersion"": {
							""$ref"": ""#/definitions/Element""
						},
						""acceptUnknown"": {
							""enum"": [""no"", ""extensions"", ""elements"", ""both""],
							""type"": ""string""
						},
						""_acceptUnknown"": {
							""$ref"": ""#/definitions/Element""
						},
						""format"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_format"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""patchFormat"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_patchFormat"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""implementationGuide"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_implementationGuide"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""profile"": {
							""description"": ""A list of profiles that represent different use cases supported by the system. For a server, \""supported by the system\"" means the system hosts/produces a set of resources that are conformant to a particular profile, and allows clients that use its services to search using this profile and to find appropriate data. For a client, it means the system will search by this profile and process data according to the guidance implicit in the profile. See further discussion in [Using Profiles](profiling.html#profile-uses)."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""rest"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Rest""
							}
						},
						""messaging"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Messaging""
							}
						},
						""document"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Document""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""CapabilityStatement_Software"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""releaseDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_releaseDate"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_Implementation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_Rest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""mode"": {
							""enum"": [""client"", ""server""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""security"": {
							""$ref"": ""#/definitions/CapabilityStatement_Security""
						},
						""resource"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Resource""
							}
						},
						""interaction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Interaction1""
							}
						},
						""searchParam"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_SearchParam""
							}
						},
						""operation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Operation""
							}
						},
						""compartment"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_compartment"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""CapabilityStatement_Security"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""cors"": {
							""type"": ""boolean""
						},
						""_cors"": {
							""$ref"": ""#/definitions/Element""
						},
						""service"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""certificate"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Certificate""
							}
						}
					}
				}
			]
		},
		""CapabilityStatement_Certificate"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""blob"": {
							""type"": ""string""
						},
						""_blob"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_Resource"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""interaction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Interaction""
							}
						},
						""versioning"": {
							""enum"": [""no-version"", ""versioned"", ""versioned-update""],
							""type"": ""string""
						},
						""_versioning"": {
							""$ref"": ""#/definitions/Element""
						},
						""readHistory"": {
							""type"": ""boolean""
						},
						""_readHistory"": {
							""$ref"": ""#/definitions/Element""
						},
						""updateCreate"": {
							""type"": ""boolean""
						},
						""_updateCreate"": {
							""$ref"": ""#/definitions/Element""
						},
						""conditionalCreate"": {
							""type"": ""boolean""
						},
						""_conditionalCreate"": {
							""$ref"": ""#/definitions/Element""
						},
						""conditionalRead"": {
							""enum"": [""not-supported"", ""modified-since"", ""not-match"", ""full-support""],
							""type"": ""string""
						},
						""_conditionalRead"": {
							""$ref"": ""#/definitions/Element""
						},
						""conditionalUpdate"": {
							""type"": ""boolean""
						},
						""_conditionalUpdate"": {
							""$ref"": ""#/definitions/Element""
						},
						""conditionalDelete"": {
							""enum"": [""not-supported"", ""single"", ""multiple""],
							""type"": ""string""
						},
						""_conditionalDelete"": {
							""$ref"": ""#/definitions/Element""
						},
						""referencePolicy"": {
							""enum"": [""literal"", ""logical"", ""resolves"", ""enforced"", ""local""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_referencePolicy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""searchInclude"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_searchInclude"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""searchRevInclude"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_searchRevInclude"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""searchParam"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_SearchParam""
							}
						}
					},
					""required"": [""interaction""]
				}
			]
		},
		""CapabilityStatement_Interaction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""enum"": [""read"", ""vread"", ""update"", ""patch"", ""delete"", ""history-instance"", ""history-type"", ""create"", ""search-type""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_SearchParam"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""type"": ""string""
						},
						""_definition"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""number"", ""date"", ""string"", ""token"", ""reference"", ""composite"", ""quantity"", ""uri""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_Interaction1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""enum"": [""transaction"", ""batch"", ""search-system"", ""history-system""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CapabilityStatement_Operation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""definition""]
				}
			]
		},
		""CapabilityStatement_Messaging"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Endpoint""
							}
						},
						""reliableCache"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_reliableCache"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""supportedMessage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_SupportedMessage""
							}
						},
						""event"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CapabilityStatement_Event""
							}
						}
					}
				}
			]
		},
		""CapabilityStatement_Endpoint"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""protocol"": {
							""$ref"": ""#/definitions/Coding""
						},
						""address"": {
							""type"": ""string""
						},
						""_address"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""protocol""]
				}
			]
		},
		""CapabilityStatement_SupportedMessage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""mode"": {
							""enum"": [""sender"", ""receiver""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""definition""]
				}
			]
		},
		""CapabilityStatement_Event"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/Coding""
						},
						""category"": {
							""enum"": [""Consequence"", ""Currency"", ""Notification""],
							""type"": ""string""
						},
						""_category"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""sender"", ""receiver""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""focus"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_focus"": {
							""$ref"": ""#/definitions/Element""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""response"": {
							""$ref"": ""#/definitions/Reference""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""request"", ""code"", ""response""]
				}
			]
		},
		""CapabilityStatement_Document"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""mode"": {
							""enum"": [""producer"", ""consumer""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""profile""]
				}
			]
		},
		""CarePlan"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CarePlan""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""suspended"", ""completed"", ""entered-in-error"", ""cancelled"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""enum"": [""proposal"", ""plan"", ""order"", ""option""],
							""type"": ""string""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""description"": ""Identifies what \""kind\"" of plan this is to support differentiation between multiple co-existing plans; e.g. \""Home health\"", \""psychiatric\"", \""asthma\"", \""disease management\"", \""wellness plan\"", etc."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""author"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""careTeam"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""addresses"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""supportingInfo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""goal"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""activity"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CarePlan_Activity""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""CarePlan_Activity"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""outcomeCodeableConcept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""outcomeReference"": {
							""description"": ""Details of the outcome or action resulting from the activity.  The reference to an \""event\"" resource, such as Procedure or Encounter or Observation, is the result/outcome of the activity itself.  The activity can be conveyed using CarePlan.activity.detail OR using the CarePlan.activity.reference (a reference to a “request” resource)."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""progress"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""detail"": {
							""$ref"": ""#/definitions/CarePlan_Detail""
						}
					}
				}
			]
		},
		""CarePlan_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""definition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""goal"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""not-started"", ""scheduled"", ""in-progress"", ""on-hold"", ""completed"", ""cancelled"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""statusReason"": {
							""type"": ""string""
						},
						""_statusReason"": {
							""$ref"": ""#/definitions/Element""
						},
						""prohibited"": {
							""type"": ""boolean""
						},
						""_prohibited"": {
							""$ref"": ""#/definitions/Element""
						},
						""scheduledTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""scheduledPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""scheduledString"": {
							""type"": ""string""
						},
						""_scheduledString"": {
							""$ref"": ""#/definitions/Element""
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""productCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""productReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dailyAmount"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CareTeam"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CareTeam""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""proposed"", ""active"", ""suspended"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""name"": {
							""description"": ""A label for human use intended to distinguish like teams.  E.g. the \""red\"" vs. \""green\"" trauma teams."",
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CareTeam_Participant""
							}
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""managingOrganization"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""CareTeam_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""description"": ""Indicates specific responsibility of an individual within the care team, such as \""Primary care physician\"", \""Trained social worker counselor\"", \""Caregiver\"", etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""member"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""ChargeItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ChargeItem""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""status"": {
							""enum"": [""planned"", ""billable"", ""not-billable"", ""aborted"", ""billed"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""occurrenceTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ChargeItem_Participant""
							}
						},
						""performingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""bodysite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""factorOverride"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factorOverride"": {
							""$ref"": ""#/definitions/Element""
						},
						""priceOverride"": {
							""$ref"": ""#/definitions/Money""
						},
						""overrideReason"": {
							""type"": ""string""
						},
						""_overrideReason"": {
							""$ref"": ""#/definitions/Element""
						},
						""enterer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""enteredDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_enteredDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""service"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""account"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""supportingInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""code"", ""subject"", ""resourceType""]
				}
			]
		},
		""ChargeItem_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""Claim"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Claim""]
						},
						""identifier"": {
							""description"": ""The business identifier for the instance: claim number, pre-determination or pre-authorization number."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""use"": {
							""enum"": [""complete"", ""proposed"", ""exploratory"", ""other""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""billablePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""enterer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""priority"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""fundsReserve"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""related"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Related""
							}
						},
						""prescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""originalPrescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""payee"": {
							""$ref"": ""#/definitions/Claim_Payee""
						},
						""referral"": {
							""$ref"": ""#/definitions/Reference""
						},
						""facility"": {
							""$ref"": ""#/definitions/Reference""
						},
						""careTeam"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_CareTeam""
							}
						},
						""information"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Information""
							}
						},
						""diagnosis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Diagnosis""
							}
						},
						""procedure"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Procedure""
							}
						},
						""insurance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Insurance""
							}
						},
						""accident"": {
							""$ref"": ""#/definitions/Claim_Accident""
						},
						""employmentImpacted"": {
							""$ref"": ""#/definitions/Period""
						},
						""hospitalization"": {
							""$ref"": ""#/definitions/Period""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Item""
							}
						},
						""total"": {
							""$ref"": ""#/definitions/Money""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Claim_Related"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""claim"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relationship"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reference"": {
							""$ref"": ""#/definitions/Identifier""
						}
					}
				}
			]
		},
		""Claim_Payee"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""description"": ""Type of Party to be reimbursed: Subscriber, provider, other."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""resourceType"": {
							""$ref"": ""#/definitions/Coding""
						},
						""party"": {
							""description"": ""Party to be reimbursed: Subscriber, provider, other."",
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""Claim_CareTeam"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""responsible"": {
							""type"": ""boolean""
						},
						""_responsible"": {
							""$ref"": ""#/definitions/Element""
						},
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""qualification"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""provider""]
				}
			]
		},
		""Claim_Information"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""description"": ""The general class of the information supplied: information; exception; accident, employment; onset, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""timingDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_timingDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reason"": {
							""description"": ""For example, provides the reason for: the additional stay, or missing tooth or any other situation where a reason code is required in addition to the content."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""category""]
				}
			]
		},
		""Claim_Diagnosis"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""diagnosisCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""diagnosisReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""type"": {
							""description"": ""The type of the Diagnosis, for example: admitting, primary, secondary, discharge."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""packageCode"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""Claim_Procedure"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""procedureCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""procedureReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Claim_Insurance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""focal"": {
							""type"": ""boolean""
						},
						""_focal"": {
							""$ref"": ""#/definitions/Element""
						},
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						},
						""businessArrangement"": {
							""type"": ""string""
						},
						""_businessArrangement"": {
							""$ref"": ""#/definitions/Element""
						},
						""preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""claimResponse"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""coverage""]
				}
			]
		},
		""Claim_Accident"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""Type of accident: work, auto, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""locationAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""locationReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Claim_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""careTeamLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_careTeamLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""diagnosisLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_diagnosisLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""procedureLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_procedureLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""informationLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_informationLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""servicedDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_servicedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""servicedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""locationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""locationAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""locationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subSite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""encounter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_Detail""
							}
						}
					}
				}
			]
		},
		""Claim_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""subDetail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Claim_SubDetail""
							}
						}
					}
				}
			]
		},
		""Claim_SubDetail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""ClaimResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ClaimResponse""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""payeeType"": {
							""description"": ""Party to be reimbursed: Subscriber, provider, other."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Item""
							}
						},
						""addItem"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_AddItem""
							}
						},
						""error"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Error""
							}
						},
						""totalCost"": {
							""$ref"": ""#/definitions/Money""
						},
						""unallocDeductable"": {
							""$ref"": ""#/definitions/Money""
						},
						""totalBenefit"": {
							""$ref"": ""#/definitions/Money""
						},
						""payment"": {
							""$ref"": ""#/definitions/ClaimResponse_Payment""
						},
						""reserved"": {
							""$ref"": ""#/definitions/Coding""
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""processNote"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_ProcessNote""
							}
						},
						""communicationRequest"": {
							""description"": ""Request for additional supporting or authorizing information, such as: documents, images or resources."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""insurance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Insurance""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ClaimResponse_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Adjudication""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Detail""
							}
						}
					}
				}
			]
		},
		""ClaimResponse_Adjudication"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""category"": {
							""description"": ""Code indicating: Co-Pay, deductible, eligible, benefit, tax, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""amount"": {
							""$ref"": ""#/definitions/Money""
						},
						""value"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""category""]
				}
			]
		},
		""ClaimResponse_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Adjudication""
							}
						},
						""subDetail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_SubDetail""
							}
						}
					}
				}
			]
		},
		""ClaimResponse_SubDetail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Adjudication""
							}
						}
					}
				}
			]
		},
		""ClaimResponse_AddItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_sequenceLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""fee"": {
							""$ref"": ""#/definitions/Money""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Adjudication""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Detail1""
							}
						}
					}
				}
			]
		},
		""ClaimResponse_Detail1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""fee"": {
							""$ref"": ""#/definitions/Money""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClaimResponse_Adjudication""
							}
						}
					}
				}
			]
		},
		""ClaimResponse_Error"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""detailSequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_detailSequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""subdetailSequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_subdetailSequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""ClaimResponse_Payment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""adjustment"": {
							""$ref"": ""#/definitions/Money""
						},
						""adjustmentReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""amount"": {
							""$ref"": ""#/definitions/Money""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						}
					}
				}
			]
		},
		""ClaimResponse_ProcessNote"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""number"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_number"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""The note purpose: Print/Display."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""description"": ""The ISO-639-1 alpha 2 code in lower case for the language, optionally followed by a hyphen and the ISO-3166-1 alpha 2 code for the region in upper case; e.g. \""en\"" for English, or \""en-US\"" for American English versus \""en-EN\"" for England English."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""ClaimResponse_Insurance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""focal"": {
							""type"": ""boolean""
						},
						""_focal"": {
							""$ref"": ""#/definitions/Element""
						},
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						},
						""businessArrangement"": {
							""type"": ""string""
						},
						""_businessArrangement"": {
							""$ref"": ""#/definitions/Element""
						},
						""preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""claimResponse"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""coverage""]
				}
			]
		},
		""ClinicalImpression"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""A record of a clinical assessment performed to determine what problem(s) may affect the patient and before planning the treatments or management strategies that are best to manage a patient's condition. Assessments are often 1:1 with a clinical consultation / encounter,  but this varies greatly depending on the clinical workflow. This resource is called \""ClinicalImpression\"" rather than \""ClinicalAssessment\"" to avoid confusion with the recording of assessment tools such as Apgar score."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ClinicalImpression""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""draft"", ""completed"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""effectiveDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_effectiveDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""assessor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""previous"": {
							""$ref"": ""#/definitions/Reference""
						},
						""problem"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""investigation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClinicalImpression_Investigation""
							}
						},
						""protocol"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_protocol"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""summary"": {
							""type"": ""string""
						},
						""_summary"": {
							""$ref"": ""#/definitions/Element""
						},
						""finding"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ClinicalImpression_Finding""
							}
						},
						""prognosisCodeableConcept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""prognosisReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""ClinicalImpression_Investigation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a clinical assessment performed to determine what problem(s) may affect the patient and before planning the treatments or management strategies that are best to manage a patient's condition. Assessments are often 1:1 with a clinical consultation / encounter,  but this varies greatly depending on the clinical workflow. This resource is called \""ClinicalImpression\"" rather than \""ClinicalAssessment\"" to avoid confusion with the recording of assessment tools such as Apgar score."",
					""properties"": {
						""code"": {
							""description"": ""A name/code for the group (\""set\"") of investigations. Typically, this will be something like \""signs\"", \""symptoms\"", \""clinical\"", \""diagnostic\"", but the list is not constrained, and others such groups such as (exposure|family|travel|nutitirional) history may be used."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""code""]
				}
			]
		},
		""ClinicalImpression_Finding"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a clinical assessment performed to determine what problem(s) may affect the patient and before planning the treatments or management strategies that are best to manage a patient's condition. Assessments are often 1:1 with a clinical consultation / encounter,  but this varies greatly depending on the clinical workflow. This resource is called \""ClinicalImpression\"" rather than \""ClinicalAssessment\"" to avoid confusion with the recording of assessment tools such as Apgar score."",
					""properties"": {
						""itemCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""itemReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""basis"": {
							""type"": ""string""
						},
						""_basis"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CodeableConcept"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""coding"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Narrative"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""status"": {
							""enum"": [""generated"", ""extensions"", ""additional"", ""empty""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""div"": {
							""type"": ""string""
						}
					},
					""required"": [""div""]
				}
			]
		},
		""Extension"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBase64Binary"": {
							""type"": ""string""
						},
						""_valueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInstant"": {
							""type"": ""string""
						},
						""_valueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_valueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_valueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_valueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_valuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueMarkdown"": {
							""type"": ""string""
						},
						""_valueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""valueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""valueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""valueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""valueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""valueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""valueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""valueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""valueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""valueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""valueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""valueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""valueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""valueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""valueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""valueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""valueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""valueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""valueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""valueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""valueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""valueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						}
					}
				}
			]
		},
		""Element"": {
			""allOf"": [{
					""properties"": {
						""id"": {
							""type"": ""string""
						},
						""_id"": {
							""$ref"": ""#/definitions/Element""
						},
						""extension"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Extension""
							}
						}
					}
				}
			]
		},
		""Resource"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""id"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_id"": {
							""$ref"": ""#/definitions/Element""
						},
						""meta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""implicitRules"": {
							""type"": ""string""
						},
						""_implicitRules"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DomainResource"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Resource""
				}, {
					""properties"": {
						""text"": {
							""description"": ""A human-readable narrative that contains a summary of the resource, and may be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it \""clinically safe\"" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety."",
							""$ref"": ""#/definitions/Narrative""
						},
						""contained"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ResourceList""
							}
						},
						""extension"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Extension""
							}
						},
						""modifierExtension"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Extension""
							}
						}
					}
				}
			]
		},
		""CodeSystem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CodeSystem""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""caseSensitive"": {
							""type"": ""boolean""
						},
						""_caseSensitive"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSet"": {
							""type"": ""string""
						},
						""_valueSet"": {
							""$ref"": ""#/definitions/Element""
						},
						""hierarchyMeaning"": {
							""enum"": [""grouped-by"", ""is-a"", ""part-of"", ""classified-with""],
							""type"": ""string""
						},
						""_hierarchyMeaning"": {
							""$ref"": ""#/definitions/Element""
						},
						""compositional"": {
							""type"": ""boolean""
						},
						""_compositional"": {
							""$ref"": ""#/definitions/Element""
						},
						""versionNeeded"": {
							""type"": ""boolean""
						},
						""_versionNeeded"": {
							""$ref"": ""#/definitions/Element""
						},
						""content"": {
							""enum"": [""not-present"", ""example"", ""fragment"", ""complete""],
							""type"": ""string""
						},
						""_content"": {
							""$ref"": ""#/definitions/Element""
						},
						""count"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_count"": {
							""$ref"": ""#/definitions/Element""
						},
						""filter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Filter""
							}
						},
						""property"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Property""
							}
						},
						""concept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Concept""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""CodeSystem_Filter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""operator"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_operator"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CodeSystem_Property"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""The type of the property value. Properties of type \""code\"" contain a code defined by the code system (e.g. a reference to anotherr defined concept)."",
							""enum"": [""code"", ""Coding"", ""string"", ""integer"", ""boolean"", ""dateTime""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CodeSystem_Concept"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""type"": ""string""
						},
						""_definition"": {
							""$ref"": ""#/definitions/Element""
						},
						""designation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Designation""
							}
						},
						""property"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Property1""
							}
						},
						""concept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeSystem_Concept""
							}
						}
					}
				}
			]
		},
		""CodeSystem_Designation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""$ref"": ""#/definitions/Coding""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""CodeSystem_Property1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Identifier"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""use"": {
							""enum"": [""usual"", ""official"", ""temp"", ""secondary""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""assigner"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Coding"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""userSelected"": {
							""type"": ""boolean""
						},
						""_userSelected"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Quantity"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""value"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""comparator"": {
							""description"": ""How the value should be understood and represented - whether the actual value is greater or less than the stated value due to measurement issues; e.g. if the comparator is \""<\"" , then the real value is < stated value."",
							""enum"": [""<"", ""<="", "">="", "">""],
							""type"": ""string""
						},
						""_comparator"": {
							""$ref"": ""#/definitions/Element""
						},
						""unit"": {
							""type"": ""string""
						},
						""_unit"": {
							""$ref"": ""#/definitions/Element""
						},
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Distance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Quantity""
				}, {
					""properties"": {}
				}
			]
		},
		""Count"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Quantity""
				}, {
					""properties"": {}
				}
			]
		},
		""Money"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Quantity""
				}, {
					""properties"": {}
				}
			]
		},
		""Range"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""low"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""high"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""Period"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""start"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Ratio"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""numerator"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""denominator"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""Reference"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""reference"": {
							""type"": ""string""
						},
						""_reference"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""SampledData"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""origin"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""period"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_period"": {
							""$ref"": ""#/definitions/Element""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""lowerLimit"": {
							""description"": ""The lower limit of detection of the measured points. This is needed if any of the data points have the value \""L\"" (lower than detection limit)."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_lowerLimit"": {
							""$ref"": ""#/definitions/Element""
						},
						""upperLimit"": {
							""description"": ""The upper limit of detection of the measured points. This is needed if any of the data points have the value \""U\"" (higher than detection limit)."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_upperLimit"": {
							""$ref"": ""#/definitions/Element""
						},
						""dimensions"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_dimensions"": {
							""$ref"": ""#/definitions/Element""
						},
						""data"": {
							""description"": ""A series of data points which are decimal values separated by a single space (character u20). The special values \""E\"" (error), \""L\"" (below detection limit) and \""U\"" (above detection limit) can also be used in place of a decimal value."",
							""type"": ""string""
						},
						""_data"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""origin""]
				}
			]
		},
		""Signature"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""when"": {
							""type"": ""string""
						},
						""_when"": {
							""$ref"": ""#/definitions/Element""
						},
						""whoUri"": {
							""type"": ""string""
						},
						""_whoUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""whoReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOfUri"": {
							""type"": ""string""
						},
						""_onBehalfOfUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""onBehalfOfReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""contentType"": {
							""description"": ""A mime type that indicates the technical format of the signature. Important mime types are application/signature+xml for X ML DigSig, application/jwt for JWT, and image/* for a graphical image of a signature, etc."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_contentType"": {
							""$ref"": ""#/definitions/Element""
						},
						""blob"": {
							""type"": ""string""
						},
						""_blob"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""HumanName"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""use"": {
							""enum"": [""usual"", ""official"", ""temp"", ""nickname"", ""anonymous"", ""old"", ""maiden""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""family"": {
							""type"": ""string""
						},
						""_family"": {
							""$ref"": ""#/definitions/Element""
						},
						""given"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_given"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""prefix"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_prefix"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""suffix"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_suffix"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""Timing"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""event"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
							}
						},
						""_event"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""repeat"": {
							""$ref"": ""#/definitions/Timing_Repeat""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""Timing_Repeat"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""boundsDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""boundsRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""boundsPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""count"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_count"": {
							""$ref"": ""#/definitions/Element""
						},
						""countMax"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_countMax"": {
							""$ref"": ""#/definitions/Element""
						},
						""duration"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_duration"": {
							""$ref"": ""#/definitions/Element""
						},
						""durationMax"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_durationMax"": {
							""$ref"": ""#/definitions/Element""
						},
						""durationUnit"": {
							""enum"": [""s"", ""min"", ""h"", ""d"", ""wk"", ""mo"", ""a""],
							""type"": ""string""
						},
						""_durationUnit"": {
							""$ref"": ""#/definitions/Element""
						},
						""frequency"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_frequency"": {
							""$ref"": ""#/definitions/Element""
						},
						""frequencyMax"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_frequencyMax"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""description"": ""Indicates the duration of time over which repetitions are to occur; e.g. to express \""3 times per day\"", 3 would be the frequency and \""1 day\"" would be the period."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_period"": {
							""$ref"": ""#/definitions/Element""
						},
						""periodMax"": {
							""description"": ""If present, indicates that the period is a range from [period] to [periodMax], allowing expressing concepts such as \""do this once every 3-5 days."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_periodMax"": {
							""$ref"": ""#/definitions/Element""
						},
						""periodUnit"": {
							""enum"": [""s"", ""min"", ""h"", ""d"", ""wk"", ""mo"", ""a""],
							""type"": ""string""
						},
						""_periodUnit"": {
							""$ref"": ""#/definitions/Element""
						},
						""dayOfWeek"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_dayOfWeek"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""timeOfDay"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?""
							}
						},
						""_timeOfDay"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""when"": {
							""enum"": [""MORN"", ""AFT"", ""EVE"", ""NIGHT"", ""PHS"", ""HS"", ""WAKE"", ""C"", ""CM"", ""CD"", ""CV"", ""AC"", ""ACM"", ""ACD"", ""ACV"", ""PC"", ""PCM"", ""PCD"", ""PCV""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_when"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""offset"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_offset"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Meta"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""versionId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_versionId"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastUpdated"": {
							""type"": ""string""
						},
						""_lastUpdated"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_profile"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""security"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""tag"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						}
					}
				}
			]
		},
		""ElementDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""path"": {
							""description"": ""The path identifies the element and is expressed as a \"".\""-separated list of ancestor elements, beginning with the name of the resource or extension."",
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""representation"": {
							""enum"": [""xmlAttr"", ""xmlText"", ""typeAttr"", ""cdaText"", ""xhtml""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_representation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""sliceName"": {
							""type"": ""string""
						},
						""_sliceName"": {
							""$ref"": ""#/definitions/Element""
						},
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""slicing"": {
							""description"": ""Indicates that the element is sliced into a set of alternative definitions (i.e. in a structure definition, there are multiple different constraints on a single element in the base resource). Slicing can be used in any resource that has cardinality ..* on the base resource, or any resource with a choice of types. The set of slices is any elements that come after this in the element sequence that have the same path, until a shorter path occurs (the shorter path terminates the set)."",
							""$ref"": ""#/definitions/ElementDefinition_Slicing""
						},
						""short"": {
							""type"": ""string""
						},
						""_short"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""type"": ""string""
						},
						""_definition"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""requirements"": {
							""type"": ""string""
						},
						""_requirements"": {
							""$ref"": ""#/definitions/Element""
						},
						""alias"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_alias"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						},
						""base"": {
							""$ref"": ""#/definitions/ElementDefinition_Base""
						},
						""contentReference"": {
							""type"": ""string""
						},
						""_contentReference"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition_Type""
							}
						},
						""defaultValueBoolean"": {
							""type"": ""boolean""
						},
						""_defaultValueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_defaultValueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_defaultValueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueBase64Binary"": {
							""type"": ""string""
						},
						""_defaultValueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueInstant"": {
							""type"": ""string""
						},
						""_defaultValueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueString"": {
							""type"": ""string""
						},
						""_defaultValueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUri"": {
							""type"": ""string""
						},
						""_defaultValueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_defaultValueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_defaultValueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_defaultValueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_defaultValueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_defaultValueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_defaultValueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_defaultValueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_defaultValueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_defaultValuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueMarkdown"": {
							""type"": ""string""
						},
						""_defaultValueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""defaultValueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""defaultValueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""defaultValueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""defaultValueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""defaultValueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""defaultValueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""defaultValueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""defaultValueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""defaultValueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""defaultValueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""defaultValueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""defaultValueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""defaultValueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""defaultValueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""defaultValueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""defaultValuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""defaultValueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""defaultValueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""defaultValueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""defaultValueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""defaultValueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""defaultValueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""defaultValueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""defaultValueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""defaultValueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""defaultValueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""defaultValueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""defaultValueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""defaultValueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""defaultValueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""defaultValueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""defaultValueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""defaultValueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""defaultValueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						},
						""meaningWhenMissing"": {
							""type"": ""string""
						},
						""_meaningWhenMissing"": {
							""$ref"": ""#/definitions/Element""
						},
						""orderMeaning"": {
							""type"": ""string""
						},
						""_orderMeaning"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedBoolean"": {
							""type"": ""boolean""
						},
						""_fixedBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_fixedInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_fixedDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedBase64Binary"": {
							""type"": ""string""
						},
						""_fixedBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedInstant"": {
							""type"": ""string""
						},
						""_fixedInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedString"": {
							""type"": ""string""
						},
						""_fixedString"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedUri"": {
							""type"": ""string""
						},
						""_fixedUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_fixedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_fixedDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_fixedTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_fixedCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_fixedOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_fixedUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_fixedId"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_fixedUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedPositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_fixedPositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedMarkdown"": {
							""type"": ""string""
						},
						""_fixedMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""fixedExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""fixedBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""fixedNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""fixedAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""fixedAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""fixedIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""fixedCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""fixedCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""fixedQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""fixedDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""fixedSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""fixedDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""fixedCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""fixedMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""fixedAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""fixedRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""fixedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""fixedRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""fixedReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""fixedSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""fixedSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""fixedHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""fixedAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""fixedContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""fixedTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""fixedMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""fixedElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""fixedContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""fixedContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""fixedDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""fixedRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""fixedUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""fixedDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""fixedParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""fixedTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						},
						""patternBoolean"": {
							""type"": ""boolean""
						},
						""_patternBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_patternInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_patternDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternBase64Binary"": {
							""type"": ""string""
						},
						""_patternBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternInstant"": {
							""type"": ""string""
						},
						""_patternInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternString"": {
							""type"": ""string""
						},
						""_patternString"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternUri"": {
							""type"": ""string""
						},
						""_patternUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_patternDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_patternDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_patternTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_patternCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_patternOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_patternUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_patternId"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_patternUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternPositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_patternPositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternMarkdown"": {
							""type"": ""string""
						},
						""_patternMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""patternExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""patternBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""patternNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""patternAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""patternAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""patternIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""patternCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""patternCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""patternQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""patternDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""patternSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""patternDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""patternCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""patternMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""patternAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""patternRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""patternPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""patternRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""patternReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""patternSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""patternSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""patternHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""patternAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""patternContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""patternTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""patternMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""patternElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""patternContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""patternContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""patternDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""patternRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""patternUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""patternDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""patternParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""patternTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						},
						""example"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition_Example""
							}
						},
						""minValueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_minValueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_minValueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueInstant"": {
							""type"": ""string""
						},
						""_minValueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_minValueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_minValueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_minValueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_minValuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_minValueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""minValueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""maxValueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_maxValueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_maxValueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueInstant"": {
							""type"": ""string""
						},
						""_maxValueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_maxValueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_maxValueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_maxValueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_maxValuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_maxValueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxValueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""maxLength"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_maxLength"": {
							""$ref"": ""#/definitions/Element""
						},
						""condition"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
							}
						},
						""_condition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""constraint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition_Constraint""
							}
						},
						""mustSupport"": {
							""description"": ""If true, implementations that produce or consume resources SHALL provide \""support\"" for the element in some meaningful way.  If false, the element may be ignored and not supported."",
							""type"": ""boolean""
						},
						""_mustSupport"": {
							""$ref"": ""#/definitions/Element""
						},
						""isModifier"": {
							""description"": ""If true, the value of this element affects the interpretation of the element or resource that contains it, and the value of the element cannot be ignored. Typically, this is used for status, negation and qualification codes. The effect of this is that the element cannot be ignored by systems: they SHALL either recognize the element and process it, and/or a pre-determination has been made that it is not relevant to their particular system."",
							""type"": ""boolean""
						},
						""_isModifier"": {
							""$ref"": ""#/definitions/Element""
						},
						""isSummary"": {
							""type"": ""boolean""
						},
						""_isSummary"": {
							""$ref"": ""#/definitions/Element""
						},
						""binding"": {
							""$ref"": ""#/definitions/ElementDefinition_Binding""
						},
						""mapping"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition_Mapping""
							}
						}
					}
				}
			]
		},
		""ElementDefinition_Slicing"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""discriminator"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition_Discriminator""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""ordered"": {
							""type"": ""boolean""
						},
						""_ordered"": {
							""$ref"": ""#/definitions/Element""
						},
						""rules"": {
							""enum"": [""closed"", ""open"", ""openAtEnd""],
							""type"": ""string""
						},
						""_rules"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ElementDefinition_Discriminator"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""value"", ""exists"", ""pattern"", ""type"", ""profile""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ElementDefinition_Base"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ElementDefinition_Type"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""description"": ""URL of Data type or Resource that is a(or the) type used for this element. References are URLs that are relative to http://hl7.org/fhir/StructureDefinition e.g. \""string\"" is a reference to http://hl7.org/fhir/StructureDefinition/string. Absolute URLs are only allowed in logical models."",
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""type"": ""string""
						},
						""_profile"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetProfile"": {
							""type"": ""string""
						},
						""_targetProfile"": {
							""$ref"": ""#/definitions/Element""
						},
						""aggregation"": {
							""enum"": [""contained"", ""referenced"", ""bundled""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_aggregation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""versioning"": {
							""enum"": [""either"", ""independent"", ""specific""],
							""type"": ""string""
						},
						""_versioning"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ElementDefinition_Example"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBase64Binary"": {
							""type"": ""string""
						},
						""_valueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInstant"": {
							""type"": ""string""
						},
						""_valueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_valueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_valueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_valueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_valuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueMarkdown"": {
							""type"": ""string""
						},
						""_valueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""valueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""valueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""valueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""valueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""valueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""valueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""valueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""valueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""valueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""valueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""valueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""valueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""valueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""valueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""valueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""valueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""valueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""valueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""valueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""valueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""valueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						}
					}
				}
			]
		},
		""ElementDefinition_Constraint"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""key"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_key"": {
							""$ref"": ""#/definitions/Element""
						},
						""requirements"": {
							""type"": ""string""
						},
						""_requirements"": {
							""$ref"": ""#/definitions/Element""
						},
						""severity"": {
							""enum"": [""error"", ""warning""],
							""type"": ""string""
						},
						""_severity"": {
							""$ref"": ""#/definitions/Element""
						},
						""human"": {
							""type"": ""string""
						},
						""_human"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""description"": ""A [FHIRPath](http://hl7.org/fluentpath) expression of constraint that can be executed to see if this constraint is met."",
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						},
						""xpath"": {
							""type"": ""string""
						},
						""_xpath"": {
							""$ref"": ""#/definitions/Element""
						},
						""source"": {
							""type"": ""string""
						},
						""_source"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ElementDefinition_Binding"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""strength"": {
							""enum"": [""required"", ""extensible"", ""preferred"", ""example""],
							""type"": ""string""
						},
						""_strength"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetUri"": {
							""type"": ""string""
						},
						""_valueSetUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""ElementDefinition_Mapping"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identity"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_identity"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""map"": {
							""type"": ""string""
						},
						""_map"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ContactDetail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						}
					}
				}
			]
		},
		""Contributor"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""author"", ""editor"", ""reviewer"", ""endorser""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						}
					}
				}
			]
		},
		""Dosage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""additionalInstruction"": {
							""description"": ""Supplemental instruction - e.g. \""with meals\""."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""patientInstruction"": {
							""type"": ""string""
						},
						""_patientInstruction"": {
							""$ref"": ""#/definitions/Element""
						},
						""timing"": {
							""$ref"": ""#/definitions/Timing""
						},
						""asNeededBoolean"": {
							""type"": ""boolean""
						},
						""_asNeededBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""asNeededCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""site"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""route"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""doseRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""doseSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""maxDosePerPeriod"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""maxDosePerAdministration"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""maxDosePerLifetime"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""rateRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""rateRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""rateSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""RelatedArtifact"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""documentation"", ""justification"", ""citation"", ""predecessor"", ""successor"", ""derived-from"", ""depends-on"", ""composed-of""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""citation"": {
							""type"": ""string""
						},
						""_citation"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""document"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""DataRequirement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_profile"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""mustSupport"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_mustSupport"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""codeFilter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement_CodeFilter""
							}
						},
						""dateFilter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement_DateFilter""
							}
						}
					}
				}
			]
		},
		""DataRequirement_CodeFilter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetString"": {
							""type"": ""string""
						},
						""_valueSetString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueCode"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_valueCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""valueCoding"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""valueCodeableConcept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					}
				}
			]
		},
		""DataRequirement_DateFilter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						}
					}
				}
			]
		},
		""ParameterDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""TriggerDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""named-event"", ""periodic"", ""data-added"", ""data-modified"", ""data-removed"", ""data-accessed"", ""data-access-ended""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""eventName"": {
							""type"": ""string""
						},
						""_eventName"": {
							""$ref"": ""#/definitions/Element""
						},
						""eventTimingTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""eventTimingReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""eventTimingDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_eventTimingDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""eventTimingDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_eventTimingDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""eventData"": {
							""$ref"": ""#/definitions/DataRequirement""
						}
					}
				}
			]
		},
		""UsageContext"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""ResourceList"": {
			""oneOf"": [{
					""$ref"": ""#/definitions/Account""
				}, {
					""$ref"": ""#/definitions/ActivityDefinition""
				}, {
					""$ref"": ""#/definitions/AdverseEvent""
				}, {
					""$ref"": ""#/definitions/AllergyIntolerance""
				}, {
					""$ref"": ""#/definitions/Appointment""
				}, {
					""$ref"": ""#/definitions/AppointmentResponse""
				}, {
					""$ref"": ""#/definitions/AuditEvent""
				}, {
					""$ref"": ""#/definitions/Basic""
				}, {
					""$ref"": ""#/definitions/Binary""
				}, {
					""$ref"": ""#/definitions/BodySite""
				}, {
					""$ref"": ""#/definitions/Bundle""
				}, {
					""$ref"": ""#/definitions/CapabilityStatement""
				}, {
					""$ref"": ""#/definitions/CarePlan""
				}, {
					""$ref"": ""#/definitions/CareTeam""
				}, {
					""$ref"": ""#/definitions/ChargeItem""
				}, {
					""$ref"": ""#/definitions/Claim""
				}, {
					""$ref"": ""#/definitions/ClaimResponse""
				}, {
					""$ref"": ""#/definitions/ClinicalImpression""
				}, {
					""$ref"": ""#/definitions/CodeSystem""
				}, {
					""$ref"": ""#/definitions/Communication""
				}, {
					""$ref"": ""#/definitions/CommunicationRequest""
				}, {
					""$ref"": ""#/definitions/CompartmentDefinition""
				}, {
					""$ref"": ""#/definitions/Composition""
				}, {
					""$ref"": ""#/definitions/ConceptMap""
				}, {
					""$ref"": ""#/definitions/Condition""
				}, {
					""$ref"": ""#/definitions/Consent""
				}, {
					""$ref"": ""#/definitions/Contract""
				}, {
					""$ref"": ""#/definitions/Coverage""
				}, {
					""$ref"": ""#/definitions/DataElement""
				}, {
					""$ref"": ""#/definitions/DetectedIssue""
				}, {
					""$ref"": ""#/definitions/Device""
				}, {
					""$ref"": ""#/definitions/DeviceComponent""
				}, {
					""$ref"": ""#/definitions/DeviceMetric""
				}, {
					""$ref"": ""#/definitions/DeviceRequest""
				}, {
					""$ref"": ""#/definitions/DeviceUseStatement""
				}, {
					""$ref"": ""#/definitions/DiagnosticReport""
				}, {
					""$ref"": ""#/definitions/DocumentManifest""
				}, {
					""$ref"": ""#/definitions/DocumentReference""
				}, {
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""$ref"": ""#/definitions/EligibilityRequest""
				}, {
					""$ref"": ""#/definitions/EligibilityResponse""
				}, {
					""$ref"": ""#/definitions/Encounter""
				}, {
					""$ref"": ""#/definitions/Endpoint""
				}, {
					""$ref"": ""#/definitions/EnrollmentRequest""
				}, {
					""$ref"": ""#/definitions/EnrollmentResponse""
				}, {
					""$ref"": ""#/definitions/EpisodeOfCare""
				}, {
					""$ref"": ""#/definitions/ExpansionProfile""
				}, {
					""$ref"": ""#/definitions/ExplanationOfBenefit""
				}, {
					""$ref"": ""#/definitions/FamilyMemberHistory""
				}, {
					""$ref"": ""#/definitions/Flag""
				}, {
					""$ref"": ""#/definitions/Goal""
				}, {
					""$ref"": ""#/definitions/GraphDefinition""
				}, {
					""$ref"": ""#/definitions/Group""
				}, {
					""$ref"": ""#/definitions/GuidanceResponse""
				}, {
					""$ref"": ""#/definitions/HealthcareService""
				}, {
					""$ref"": ""#/definitions/ImagingManifest""
				}, {
					""$ref"": ""#/definitions/ImagingStudy""
				}, {
					""$ref"": ""#/definitions/Immunization""
				}, {
					""$ref"": ""#/definitions/ImmunizationRecommendation""
				}, {
					""$ref"": ""#/definitions/ImplementationGuide""
				}, {
					""$ref"": ""#/definitions/Library""
				}, {
					""$ref"": ""#/definitions/Linkage""
				}, {
					""$ref"": ""#/definitions/List""
				}, {
					""$ref"": ""#/definitions/Location""
				}, {
					""$ref"": ""#/definitions/Measure""
				}, {
					""$ref"": ""#/definitions/MeasureReport""
				}, {
					""$ref"": ""#/definitions/Media""
				}, {
					""$ref"": ""#/definitions/Medication""
				}, {
					""$ref"": ""#/definitions/MedicationAdministration""
				}, {
					""$ref"": ""#/definitions/MedicationDispense""
				}, {
					""$ref"": ""#/definitions/MedicationRequest""
				}, {
					""$ref"": ""#/definitions/MedicationStatement""
				}, {
					""$ref"": ""#/definitions/MessageDefinition""
				}, {
					""$ref"": ""#/definitions/MessageHeader""
				}, {
					""$ref"": ""#/definitions/NamingSystem""
				}, {
					""$ref"": ""#/definitions/NutritionOrder""
				}, {
					""$ref"": ""#/definitions/Observation""
				}, {
					""$ref"": ""#/definitions/OperationDefinition""
				}, {
					""$ref"": ""#/definitions/OperationOutcome""
				}, {
					""$ref"": ""#/definitions/Organization""
				}, {
					""$ref"": ""#/definitions/Parameters""
				}, {
					""$ref"": ""#/definitions/Parameters""
				}, {
					""$ref"": ""#/definitions/Patient""
				}, {
					""$ref"": ""#/definitions/PaymentNotice""
				}, {
					""$ref"": ""#/definitions/PaymentReconciliation""
				}, {
					""$ref"": ""#/definitions/Person""
				}, {
					""$ref"": ""#/definitions/PlanDefinition""
				}, {
					""$ref"": ""#/definitions/Practitioner""
				}, {
					""$ref"": ""#/definitions/PractitionerRole""
				}, {
					""$ref"": ""#/definitions/Procedure""
				}, {
					""$ref"": ""#/definitions/ProcedureRequest""
				}, {
					""$ref"": ""#/definitions/ProcessRequest""
				}, {
					""$ref"": ""#/definitions/ProcessResponse""
				}, {
					""$ref"": ""#/definitions/Provenance""
				}, {
					""$ref"": ""#/definitions/Questionnaire""
				}, {
					""$ref"": ""#/definitions/QuestionnaireResponse""
				}, {
					""$ref"": ""#/definitions/ReferralRequest""
				}, {
					""$ref"": ""#/definitions/RelatedPerson""
				}, {
					""$ref"": ""#/definitions/RequestGroup""
				}, {
					""$ref"": ""#/definitions/ResearchStudy""
				}, {
					""$ref"": ""#/definitions/ResearchSubject""
				}, {
					""$ref"": ""#/definitions/Resource""
				}, {
					""$ref"": ""#/definitions/RiskAssessment""
				}, {
					""$ref"": ""#/definitions/Schedule""
				}, {
					""$ref"": ""#/definitions/SearchParameter""
				}, {
					""$ref"": ""#/definitions/Sequence""
				}, {
					""$ref"": ""#/definitions/ServiceDefinition""
				}, {
					""$ref"": ""#/definitions/Slot""
				}, {
					""$ref"": ""#/definitions/Specimen""
				}, {
					""$ref"": ""#/definitions/StructureDefinition""
				}, {
					""$ref"": ""#/definitions/StructureMap""
				}, {
					""$ref"": ""#/definitions/Subscription""
				}, {
					""$ref"": ""#/definitions/Substance""
				}, {
					""$ref"": ""#/definitions/SupplyDelivery""
				}, {
					""$ref"": ""#/definitions/SupplyRequest""
				}, {
					""$ref"": ""#/definitions/Task""
				}, {
					""$ref"": ""#/definitions/TestReport""
				}, {
					""$ref"": ""#/definitions/TestScript""
				}, {
					""$ref"": ""#/definitions/ValueSet""
				}, {
					""$ref"": ""#/definitions/VisionPrescription""
				}
			]
		},
		""ContactPoint"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Element""
				}, {
					""properties"": {
						""system"": {
							""enum"": [""phone"", ""fax"", ""email"", ""pager"", ""url"", ""sms"", ""other""],
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""enum"": [""home"", ""work"", ""temp"", ""old"", ""mobile""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""rank"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_rank"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""Communication"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Communication""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDone"": {
							""type"": ""boolean""
						},
						""_notDone"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDoneReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""medium"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""recipient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""sent"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_sent"": {
							""$ref"": ""#/definitions/Element""
						},
						""received"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_received"": {
							""$ref"": ""#/definitions/Element""
						},
						""sender"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""payload"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Communication_Payload""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Communication_Payload"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""contentString"": {
							""type"": ""string""
						},
						""_contentString"": {
							""$ref"": ""#/definitions/Element""
						},
						""contentAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""contentReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""CommunicationRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CommunicationRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""medium"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""recipient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""payload"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CommunicationRequest_Payload""
							}
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""sender"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requester"": {
							""$ref"": ""#/definitions/CommunicationRequest_Requester""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""CommunicationRequest_Payload"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""contentString"": {
							""type"": ""string""
						},
						""_contentString"": {
							""$ref"": ""#/definitions/Element""
						},
						""contentAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""contentReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""CommunicationRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""CompartmentDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""CompartmentDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""code"": {
							""enum"": [""Patient"", ""Encounter"", ""RelatedPerson"", ""Practitioner"", ""Device""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""search"": {
							""type"": ""boolean""
						},
						""_search"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CompartmentDefinition_Resource""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""CompartmentDefinition_Resource"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""param"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_param"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Composition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""A set of healthcare-related information that is assembled together into a single logical document that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. While a Composition defines the structure, it does not actually contain the content: rather the full content of a document is contained in a Bundle, of which the Composition is the first resource contained."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Composition""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""preliminary"", ""final"", ""amended"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""class"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""confidentiality"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_confidentiality"": {
							""$ref"": ""#/definitions/Element""
						},
						""attester"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Composition_Attester""
							}
						},
						""custodian"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relatesTo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Composition_RelatesTo""
							}
						},
						""event"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Composition_Event""
							}
						},
						""section"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Composition_Section""
							}
						}
					},
					""required"": [""subject"", ""author"", ""type"", ""resourceType""]
				}
			]
		},
		""Composition_Attester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A set of healthcare-related information that is assembled together into a single logical document that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. While a Composition defines the structure, it does not actually contain the content: rather the full content of a document is contained in a Bundle, of which the Composition is the first resource contained."",
					""properties"": {
						""mode"": {
							""enum"": [""personal"", ""professional"", ""legal"", ""official""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_mode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""time"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_time"": {
							""$ref"": ""#/definitions/Element""
						},
						""party"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Composition_RelatesTo"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A set of healthcare-related information that is assembled together into a single logical document that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. While a Composition defines the structure, it does not actually contain the content: rather the full content of a document is contained in a Bundle, of which the Composition is the first resource contained."",
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""targetReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Composition_Event"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A set of healthcare-related information that is assembled together into a single logical document that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. While a Composition defines the structure, it does not actually contain the content: rather the full content of a document is contained in a Bundle, of which the Composition is the first resource contained."",
					""properties"": {
						""code"": {
							""description"": ""This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the typeCode, such as a \""History and Physical Report\"" in which the procedure being documented is necessarily a \""History and Physical\"" act."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""Composition_Section"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A set of healthcare-related information that is assembled together into a single logical document that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. While a Composition defines the structure, it does not actually contain the content: rather the full content of a document is contained in a Bundle, of which the Composition is the first resource contained."",
					""properties"": {
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""text"": {
							""description"": ""A human-readable narrative that contains the attested content of the section, used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it \""clinically safe\"" for a human to just read the narrative."",
							""$ref"": ""#/definitions/Narrative""
						},
						""mode"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""orderedBy"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""entry"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""emptyReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""section"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Composition_Section""
							}
						}
					}
				}
			]
		},
		""ConceptMap"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ConceptMap""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceUri"": {
							""type"": ""string""
						},
						""_sourceUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""targetUri"": {
							""type"": ""string""
						},
						""_targetUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""group"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ConceptMap_Group""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ConceptMap_Group"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""source"": {
							""type"": ""string""
						},
						""_source"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceVersion"": {
							""type"": ""string""
						},
						""_sourceVersion"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""type"": ""string""
						},
						""_target"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetVersion"": {
							""type"": ""string""
						},
						""_targetVersion"": {
							""$ref"": ""#/definitions/Element""
						},
						""element"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ConceptMap_Element""
							}
						},
						""unmapped"": {
							""$ref"": ""#/definitions/ConceptMap_Unmapped""
						}
					},
					""required"": [""element""]
				}
			]
		},
		""ConceptMap_Element"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ConceptMap_Target""
							}
						}
					}
				}
			]
		},
		""ConceptMap_Target"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""equivalence"": {
							""enum"": [""relatedto"", ""equivalent"", ""equal"", ""wider"", ""subsumes"", ""narrower"", ""specializes"", ""inexact"", ""unmatched"", ""disjoint""],
							""type"": ""string""
						},
						""_equivalence"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""dependsOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ConceptMap_DependsOn""
							}
						},
						""product"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ConceptMap_DependsOn""
							}
						}
					}
				}
			]
		},
		""ConceptMap_DependsOn"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""property"": {
							""type"": ""string""
						},
						""_property"": {
							""$ref"": ""#/definitions/Element""
						},
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ConceptMap_Unmapped"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""mode"": {
							""description"": ""Defines which action to take if there is no match in the group. One of 3 actions is possible: use the unmapped code (this is useful when doing a mapping between versions, and only a few codes have changed), use a fixed code (a default code), or alternatively, a reference to a different concept map can be provided (by canonical URL)."",
							""enum"": [""provided"", ""fixed"", ""other-map""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""description"": ""The fixed code to use when the mode = 'fixed'  - all unmapped codes are mapped to a single fixed code."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Condition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Condition""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""clinicalStatus"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_clinicalStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""verificationStatus"": {
							""enum"": [""provisional"", ""differential"", ""confirmed"", ""refuted"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_verificationStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""severity"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""bodySite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onsetDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_onsetDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""onsetAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""onsetPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""onsetRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""onsetString"": {
							""type"": ""string""
						},
						""_onsetString"": {
							""$ref"": ""#/definitions/Element""
						},
						""abatementDateTime"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_abatementDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""abatementAge"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""$ref"": ""#/definitions/Age""
						},
						""abatementBoolean"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""type"": ""boolean""
						},
						""_abatementBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""abatementPeriod"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""$ref"": ""#/definitions/Period""
						},
						""abatementRange"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""$ref"": ""#/definitions/Range""
						},
						""abatementString"": {
							""description"": ""The date or estimated date that the condition resolved or went into remission. This is called \""abatement\"" because of the many overloaded connotations associated with \""remission\"" or \""resolution\"" - Conditions are never really resolved, but they can abate."",
							""type"": ""string""
						},
						""_abatementString"": {
							""$ref"": ""#/definitions/Element""
						},
						""assertedDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_assertedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""asserter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""stage"": {
							""$ref"": ""#/definitions/Condition_Stage""
						},
						""evidence"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Condition_Evidence""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""Condition_Stage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""summary"": {
							""description"": ""A simple summary of the stage such as \""Stage 3\"". The determination of the stage is disease-specific."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""assessment"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""Condition_Evidence"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""Consent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Consent""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""draft"", ""proposed"", ""active"", ""rejected"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""dateTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_dateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""consentingParty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""actor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Actor""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""organization"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""sourceAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""sourceIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""sourceReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""policy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Policy""
							}
						},
						""policyRule"": {
							""type"": ""string""
						},
						""_policyRule"": {
							""$ref"": ""#/definitions/Element""
						},
						""securityLabel"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""purpose"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""dataPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""data"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Data""
							}
						},
						""except"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Except""
							}
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""Consent_Actor"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""reference"", ""role""]
				}
			]
		},
		""Consent_Policy"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""authority"": {
							""type"": ""string""
						},
						""_authority"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Consent_Data"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""meaning"": {
							""enum"": [""instance"", ""related"", ""dependents"", ""authoredby""],
							""type"": ""string""
						},
						""_meaning"": {
							""$ref"": ""#/definitions/Element""
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""reference""]
				}
			]
		},
		""Consent_Except"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""type"": {
							""enum"": [""deny"", ""permit""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""actor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Actor1""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""securityLabel"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""purpose"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""class"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""dataPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""data"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Consent_Data1""
							}
						}
					}
				}
			]
		},
		""Consent_Actor1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""reference"", ""role""]
				}
			]
		},
		""Consent_Data1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A record of a healthcare consumer’s policy choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time."",
					""properties"": {
						""meaning"": {
							""enum"": [""instance"", ""related"", ""dependents"", ""authoredby""],
							""type"": ""string""
						},
						""_meaning"": {
							""$ref"": ""#/definitions/Element""
						},
						""reference"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""reference""]
				}
			]
		},
		""Contract"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Contract""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""issued"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_issued"": {
							""$ref"": ""#/definitions/Element""
						},
						""applies"": {
							""$ref"": ""#/definitions/Period""
						},
						""subject"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""authority"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""domain"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""actionReason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""decisionType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""contentDerivative"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""securityLabel"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""agent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Agent""
							}
						},
						""signer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Signer""
							}
						},
						""valuedItem"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_ValuedItem""
							}
						},
						""term"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Term""
							}
						},
						""bindingAttachment"": {
							""description"": ""Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the \""source of truth\"" and which would be the basis for legal action related to enforcement of this Contract."",
							""$ref"": ""#/definitions/Attachment""
						},
						""bindingReference"": {
							""description"": ""Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the \""source of truth\"" and which would be the basis for legal action related to enforcement of this Contract."",
							""$ref"": ""#/definitions/Reference""
						},
						""friendly"": {
							""description"": ""The \""patient friendly language\"" versionof the Contract in whole or in parts. \""Patient friendly language\"" means the representation of the Contract and Contract Provisions in a manner that is readily accessible and understandable by a layperson in accordance with best practices for communication styles that ensure that those agreeing to or signing the Contract understand the roles, actions, obligations, responsibilities, and implication of the agreement."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Friendly""
							}
						},
						""legal"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Legal""
							}
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Rule""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Contract_Agent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""role"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""Contract_Signer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""party"": {
							""$ref"": ""#/definitions/Reference""
						},
						""signature"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Signature""
							}
						}
					},
					""required"": [""signature"", ""type"", ""party""]
				}
			]
		},
		""Contract_ValuedItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""entityCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""entityReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""effectiveTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_effectiveTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""points"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_points"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""Expresses the product of the Contract Valued Item unitQuantity and the unitPriceAmt. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						}
					}
				}
			]
		},
		""Contract_Term"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""issued"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_issued"": {
							""$ref"": ""#/definitions/Element""
						},
						""applies"": {
							""$ref"": ""#/definitions/Period""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""actionReason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""securityLabel"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""agent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Agent1""
							}
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuedItem"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_ValuedItem1""
							}
						},
						""group"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contract_Term""
							}
						}
					}
				}
			]
		},
		""Contract_Agent1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""role"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""Contract_ValuedItem1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""entityCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""entityReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""effectiveTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_effectiveTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""points"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_points"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""Expresses the product of the Contract Provision Valued Item unitQuantity and the unitPriceAmt. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						}
					}
				}
			]
		},
		""Contract_Friendly"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""contentAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""contentReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Contract_Legal"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""contentAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""contentReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Contract_Rule"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""contentAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""contentReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Coverage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Coverage""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""The type of coverage: social program, medical plan, accident coverage (workers compensation, auto), group health or payment by an individual or organization."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""policyHolder"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subscriber"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subscriberId"": {
							""type"": ""string""
						},
						""_subscriberId"": {
							""$ref"": ""#/definitions/Element""
						},
						""beneficiary"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relationship"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""payor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""grouping"": {
							""$ref"": ""#/definitions/Coverage_Grouping""
						},
						""dependent"": {
							""type"": ""string""
						},
						""_dependent"": {
							""$ref"": ""#/definitions/Element""
						},
						""sequence"": {
							""type"": ""string""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""order"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_order"": {
							""$ref"": ""#/definitions/Element""
						},
						""network"": {
							""type"": ""string""
						},
						""_network"": {
							""$ref"": ""#/definitions/Element""
						},
						""contract"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Coverage_Grouping"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""group"": {
							""type"": ""string""
						},
						""_group"": {
							""$ref"": ""#/definitions/Element""
						},
						""groupDisplay"": {
							""type"": ""string""
						},
						""_groupDisplay"": {
							""$ref"": ""#/definitions/Element""
						},
						""subGroup"": {
							""type"": ""string""
						},
						""_subGroup"": {
							""$ref"": ""#/definitions/Element""
						},
						""subGroupDisplay"": {
							""type"": ""string""
						},
						""_subGroupDisplay"": {
							""$ref"": ""#/definitions/Element""
						},
						""plan"": {
							""type"": ""string""
						},
						""_plan"": {
							""$ref"": ""#/definitions/Element""
						},
						""planDisplay"": {
							""type"": ""string""
						},
						""_planDisplay"": {
							""$ref"": ""#/definitions/Element""
						},
						""subPlan"": {
							""type"": ""string""
						},
						""_subPlan"": {
							""$ref"": ""#/definitions/Element""
						},
						""subPlanDisplay"": {
							""type"": ""string""
						},
						""_subPlanDisplay"": {
							""$ref"": ""#/definitions/Element""
						},
						""class"": {
							""type"": ""string""
						},
						""_class"": {
							""$ref"": ""#/definitions/Element""
						},
						""classDisplay"": {
							""type"": ""string""
						},
						""_classDisplay"": {
							""$ref"": ""#/definitions/Element""
						},
						""subClass"": {
							""type"": ""string""
						},
						""_subClass"": {
							""$ref"": ""#/definitions/Element""
						},
						""subClassDisplay"": {
							""type"": ""string""
						},
						""_subClassDisplay"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DataElement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DataElement""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""stringency"": {
							""enum"": [""comparable"", ""fully-specified"", ""equivalent"", ""convertable"", ""scaleable"", ""flexible""],
							""type"": ""string""
						},
						""_stringency"": {
							""$ref"": ""#/definitions/Element""
						},
						""mapping"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataElement_Mapping""
							}
						},
						""element"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition""
							}
						}
					},
					""required"": [""resourceType"", ""element""]
				}
			]
		},
		""DataElement_Mapping"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identity"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_identity"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DetectedIssue"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DetectedIssue""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""severity"": {
							""enum"": [""high"", ""moderate"", ""low""],
							""type"": ""string""
						},
						""_severity"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""implicated"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""detail"": {
							""type"": ""string""
						},
						""_detail"": {
							""$ref"": ""#/definitions/Element""
						},
						""reference"": {
							""type"": ""string""
						},
						""_reference"": {
							""$ref"": ""#/definitions/Element""
						},
						""mitigation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DetectedIssue_Mitigation""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""DetectedIssue_Mitigation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""action""]
				}
			]
		},
		""Device"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Device""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""udi"": {
							""$ref"": ""#/definitions/Device_Udi""
						},
						""status"": {
							""enum"": [""active"", ""inactive"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""lotNumber"": {
							""type"": ""string""
						},
						""_lotNumber"": {
							""$ref"": ""#/definitions/Element""
						},
						""manufacturer"": {
							""type"": ""string""
						},
						""_manufacturer"": {
							""$ref"": ""#/definitions/Element""
						},
						""manufactureDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_manufactureDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""expirationDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_expirationDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""model"": {
							""description"": ""The \""model\"" is an identifier assigned by the manufacturer to identify the product by its type. This number is shared by the all devices sold as the same type."",
							""type"": ""string""
						},
						""_model"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""owner"": {
							""$ref"": ""#/definitions/Reference""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""safety"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Device_Udi"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""deviceIdentifier"": {
							""type"": ""string""
						},
						""_deviceIdentifier"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""jurisdiction"": {
							""description"": ""The identity of the authoritative source for UDI generation within a  jurisdiction.  All UDIs are globally unique within a single namespace. with the appropriate repository uri as the system.  For example,  UDIs of devices managed in the U.S. by the FDA, the value is  http://hl7.org/fhir/NamingSystem/fda-udi."",
							""type"": ""string""
						},
						""_jurisdiction"": {
							""$ref"": ""#/definitions/Element""
						},
						""carrierHRF"": {
							""type"": ""string""
						},
						""_carrierHRF"": {
							""$ref"": ""#/definitions/Element""
						},
						""carrierAIDC"": {
							""description"": ""The full UDI carrier of the Automatic Identification and Data Capture (AIDC) technology representation of the barcode string as printed on the packaging of the device - E.g a barcode or RFID.   Because of limitations on character sets in XML and the need to round-trip JSON data through XML, AIDC Formats *SHALL* be base64 encoded."",
							""type"": ""string""
						},
						""_carrierAIDC"": {
							""$ref"": ""#/definitions/Element""
						},
						""issuer"": {
							""description"": ""Organization that is charged with issuing UDIs for devices.  For example, the US FDA issuers include :\n1) GS1: \nhttp://hl7.org/fhir/NamingSystem/gs1-di, \n2) HIBCC:\nhttp://hl7.org/fhir/NamingSystem/hibcc-dI, \n3) ICCBBA for blood containers:\nhttp://hl7.org/fhir/NamingSystem/iccbba-blood-di, \n4) ICCBA for other devices:\nhttp://hl7.org/fhir/NamingSystem/iccbba-other-di."",
							""type"": ""string""
						},
						""_issuer"": {
							""$ref"": ""#/definitions/Element""
						},
						""entryType"": {
							""enum"": [""barcode"", ""rfid"", ""manual"", ""card"", ""self-reported"", ""unknown""],
							""type"": ""string""
						},
						""_entryType"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DeviceComponent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DeviceComponent""]
						},
						""identifier"": {
							""description"": ""The locally assigned unique identification by the software. For example: handle ID."",
							""$ref"": ""#/definitions/Identifier""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""lastSystemChange"": {
							""type"": ""string""
						},
						""_lastSystemChange"": {
							""$ref"": ""#/definitions/Element""
						},
						""source"": {
							""$ref"": ""#/definitions/Reference""
						},
						""parent"": {
							""description"": ""The link to the parent resource. For example: Channel is linked to its VMD parent."",
							""$ref"": ""#/definitions/Reference""
						},
						""operationalStatus"": {
							""description"": ""The current operational status of the device. For example: On, Off, Standby, etc."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""parameterGroup"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""measurementPrinciple"": {
							""description"": ""The physical principle of the measurement. For example: thermal, chemical, acoustical, etc."",
							""enum"": [""other"", ""chemical"", ""electrical"", ""impedance"", ""nuclear"", ""optical"", ""thermal"", ""biological"", ""mechanical"", ""acoustical"", ""manual""],
							""type"": ""string""
						},
						""_measurementPrinciple"": {
							""$ref"": ""#/definitions/Element""
						},
						""productionSpecification"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DeviceComponent_ProductionSpecification""
							}
						},
						""languageCode"": {
							""description"": ""The language code for the human-readable text string produced by the device. This language code will follow the IETF language tag. Example: en-US."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""identifier"", ""type"", ""resourceType""]
				}
			]
		},
		""DeviceComponent_ProductionSpecification"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""specType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""componentId"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""productionSpec"": {
							""type"": ""string""
						},
						""_productionSpec"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DeviceMetric"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DeviceMetric""]
						},
						""identifier"": {
							""description"": ""Describes the unique identification of this metric that has been assigned by the device or gateway software. For example: handle ID.  It should be noted that in order to make the identifier unique, the system element of the identifier should be set to the unique identifier of the device."",
							""$ref"": ""#/definitions/Identifier""
						},
						""type"": {
							""description"": ""Describes the type of the metric. For example: Heart Rate, PEEP Setting, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""unit"": {
							""description"": ""Describes the unit that an observed value determined for this metric will have. For example: Percent, Seconds, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""source"": {
							""$ref"": ""#/definitions/Reference""
						},
						""parent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""operationalStatus"": {
							""description"": ""Indicates current operational state of the device. For example: On, Off, Standby, etc."",
							""enum"": [""on"", ""off"", ""standby"", ""entered-in-error""],
							""type"": ""string""
						},
						""_operationalStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""color"": {
							""enum"": [""black"", ""red"", ""green"", ""yellow"", ""blue"", ""magenta"", ""cyan"", ""white""],
							""type"": ""string""
						},
						""_color"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""enum"": [""measurement"", ""setting"", ""calculation"", ""unspecified""],
							""type"": ""string""
						},
						""_category"": {
							""$ref"": ""#/definitions/Element""
						},
						""measurementPeriod"": {
							""$ref"": ""#/definitions/Timing""
						},
						""calibration"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DeviceMetric_Calibration""
							}
						}
					},
					""required"": [""identifier"", ""type"", ""resourceType""]
				}
			]
		},
		""DeviceMetric_Calibration"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""unspecified"", ""offset"", ""gain"", ""two-point""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""state"": {
							""enum"": [""not-calibrated"", ""calibration-required"", ""calibrated"", ""unspecified""],
							""type"": ""string""
						},
						""_state"": {
							""$ref"": ""#/definitions/Element""
						},
						""time"": {
							""type"": ""string""
						},
						""_time"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""DeviceRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DeviceRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""description"": ""Protocol or definition followed by this request. For example: The proposed act must be performed if the indicated conditions occur, e.g.., shortness of breath, SpO2 less than x%."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""priorRequest"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""codeReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""codeCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""description"": ""The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. \""Every 8 hours\""; \""Three times a day\""; \""1/2 an hour before breakfast for 10 days from 23-Dec 2011:\""; \""15 Oct 2013, 17 Oct 2013 and 1 Nov 2013\""."",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""description"": ""The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. \""Every 8 hours\""; \""Three times a day\""; \""1/2 an hour before breakfast for 10 days from 23-Dec 2011:\""; \""15 Oct 2013, 17 Oct 2013 and 1 Nov 2013\""."",
							""$ref"": ""#/definitions/Period""
						},
						""occurrenceTiming"": {
							""description"": ""The timing schedule for the use of the device. The Schedule data type allows many different expressions, for example. \""Every 8 hours\""; \""Three times a day\""; \""1/2 an hour before breakfast for 10 days from 23-Dec 2011:\""; \""15 Oct 2013, 17 Oct 2013 and 1 Nov 2013\""."",
							""$ref"": ""#/definitions/Timing""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/DeviceRequest_Requester""
						},
						""performerType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""supportingInfo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""relevantHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""subject"", ""intent"", ""resourceType""]
				}
			]
		},
		""DeviceRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""DeviceUseStatement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DeviceUseStatement""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""completed"", ""entered-in-error"", ""intended"", ""stopped"", ""on-hold""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""whenUsed"": {
							""$ref"": ""#/definitions/Period""
						},
						""timingTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""timingDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_timingDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""recordedOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_recordedOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""source"": {
							""$ref"": ""#/definitions/Reference""
						},
						""device"": {
							""$ref"": ""#/definitions/Reference""
						},
						""indication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""subject"", ""device"", ""resourceType""]
				}
			]
		},
		""DiagnosticReport"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DiagnosticReport""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""registered"", ""partial"", ""preliminary"", ""final"", ""amended"", ""corrected"", ""appended"", ""cancelled"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""effectiveDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_effectiveDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""issued"": {
							""type"": ""string""
						},
						""_issued"": {
							""$ref"": ""#/definitions/Element""
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DiagnosticReport_Performer""
							}
						},
						""specimen"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""result"": {
							""description"": ""Observations that are part of this diagnostic report. Observations can be simple name/value pairs (e.g. \""atomic\"" results), or they can be grouping observations that include references to other members of the group (e.g. \""panels\"")."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""imagingStudy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""image"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DiagnosticReport_Image""
							}
						},
						""conclusion"": {
							""type"": ""string""
						},
						""_conclusion"": {
							""$ref"": ""#/definitions/Element""
						},
						""codedDiagnosis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""presentedForm"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						}
					},
					""required"": [""code"", ""resourceType""]
				}
			]
		},
		""DiagnosticReport_Performer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""DiagnosticReport_Image"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""link"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""link""]
				}
			]
		},
		""DocumentManifest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DocumentManifest""]
						},
						""masterIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""current"", ""superseded"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""recipient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""source"": {
							""type"": ""string""
						},
						""_source"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""description"": ""Human-readable description of the source document. This is sometimes known as the \""title\""."",
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""content"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DocumentManifest_Content""
							}
						},
						""related"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DocumentManifest_Related""
							}
						}
					},
					""required"": [""content"", ""resourceType""]
				}
			]
		},
		""DocumentManifest_Content"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""pAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""pReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""DocumentManifest_Related"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""ref"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""DocumentReference"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""DocumentReference""]
						},
						""masterIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""current"", ""superseded"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""docStatus"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_docStatus"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""class"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""indexed"": {
							""type"": ""string""
						},
						""_indexed"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""authenticator"": {
							""$ref"": ""#/definitions/Reference""
						},
						""custodian"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relatesTo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DocumentReference_RelatesTo""
							}
						},
						""description"": {
							""description"": ""Human-readable description of the source document. This is sometimes known as the \""title\""."",
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""securityLabel"": {
							""description"": ""A set of Security-Tag codes specifying the level of privacy/security of the Document. Note that DocumentReference.meta.security contains the security labels of the \""reference\"" to the document, while DocumentReference.securityLabel contains a snapshot of the security labels on the document the reference refers to."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""content"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DocumentReference_Content""
							}
						},
						""context"": {
							""$ref"": ""#/definitions/DocumentReference_Context""
						}
					},
					""required"": [""type"", ""content"", ""resourceType""]
				}
			]
		},
		""DocumentReference_RelatesTo"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""enum"": [""replaces"", ""transforms"", ""signs"", ""appends""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""target""]
				}
			]
		},
		""DocumentReference_Content"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""attachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""format"": {
							""$ref"": ""#/definitions/Coding""
						}
					},
					""required"": [""attachment""]
				}
			]
		},
		""DocumentReference_Context"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""event"": {
							""description"": ""This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the typeCode, such as a \""History and Physical Report\"" in which the procedure being documented is necessarily a \""History and Physical\"" act."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""facilityType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""practiceSetting"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""sourcePatientInfo"": {
							""$ref"": ""#/definitions/Reference""
						},
						""related"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DocumentReference_Related""
							}
						}
					}
				}
			]
		},
		""DocumentReference_Related"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""ref"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Duration"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Quantity""
				}, {
					""properties"": {}
				}
			]
		},
		""schemaArray"": {
			""type"": ""array"",
			""minItems"": 1,
			""items"": {
				""$ref"": ""#""
			}
		},
		""EligibilityRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""EligibilityRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""priority"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""servicedDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_servicedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""servicedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""enterer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""facility"": {
							""$ref"": ""#/definitions/Reference""
						},
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						},
						""businessArrangement"": {
							""type"": ""string""
						},
						""_businessArrangement"": {
							""$ref"": ""#/definitions/Element""
						},
						""benefitCategory"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""benefitSubCategory"": {
							""description"": ""Dental: basic, major, ortho; Vision exam, glasses, contacts; etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""EligibilityResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""EligibilityResponse""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""description"": ""Transaction status: error, complete."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""inforce"": {
							""type"": ""boolean""
						},
						""_inforce"": {
							""$ref"": ""#/definitions/Element""
						},
						""insurance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EligibilityResponse_Insurance""
							}
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""error"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EligibilityResponse_Error""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""EligibilityResponse_Insurance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						},
						""contract"": {
							""$ref"": ""#/definitions/Reference""
						},
						""benefitBalance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EligibilityResponse_BenefitBalance""
							}
						}
					}
				}
			]
		},
		""EligibilityResponse_BenefitBalance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subCategory"": {
							""description"": ""Dental: basic, major, ortho; Vision exam, glasses, contacts; etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""excluded"": {
							""type"": ""boolean""
						},
						""_excluded"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""description"": ""A richer description of the benefit, for example 'DENT2 covers 100% of basic, 50% of major but exclused Ortho, Implants and Costmetic services'."",
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""network"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""unit"": {
							""description"": ""Unit designation: individual or family."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""term"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""financial"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EligibilityResponse_Financial""
							}
						}
					},
					""required"": [""category""]
				}
			]
		},
		""EligibilityResponse_Financial"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""allowedUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_allowedUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""allowedString"": {
							""type"": ""string""
						},
						""_allowedString"": {
							""$ref"": ""#/definitions/Element""
						},
						""allowedMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""usedUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_usedUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""usedMoney"": {
							""$ref"": ""#/definitions/Money""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""EligibilityResponse_Error"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""Encounter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Encounter""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""description"": ""planned | arrived | triaged | in-progress | onleave | finished | cancelled +."",
							""enum"": [""planned"", ""arrived"", ""triaged"", ""in-progress"", ""onleave"", ""finished"", ""cancelled"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""statusHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Encounter_StatusHistory""
							}
						},
						""class"": {
							""description"": ""inpatient | outpatient | ambulatory | emergency +."",
							""$ref"": ""#/definitions/Coding""
						},
						""classHistory"": {
							""description"": ""The class history permits the tracking of the encounters transitions without needing to go  through the resource history.\n\nThis would be used for a case where an admission starts of as an emergency encounter, then transisions into an inpatient scenario. Doing this and not restarting a new encounter ensures that any lab/diagnostic results can more easily follow the patient and not require re-processing and not get lost or cancelled during a kindof discharge from emergency to inpatient."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Encounter_ClassHistory""
							}
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""priority"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""episodeOfCare"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""incomingReferral"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Encounter_Participant""
							}
						},
						""appointment"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""length"": {
							""$ref"": ""#/definitions/Duration""
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""diagnosis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Encounter_Diagnosis""
							}
						},
						""account"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""hospitalization"": {
							""$ref"": ""#/definitions/Encounter_Hospitalization""
						},
						""location"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Encounter_Location""
							}
						},
						""serviceProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""partOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Encounter_StatusHistory"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""status"": {
							""description"": ""planned | arrived | triaged | in-progress | onleave | finished | cancelled +."",
							""enum"": [""planned"", ""arrived"", ""triaged"", ""in-progress"", ""onleave"", ""finished"", ""cancelled"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""period""]
				}
			]
		},
		""Encounter_ClassHistory"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""class"": {
							""description"": ""inpatient | outpatient | ambulatory | emergency +."",
							""$ref"": ""#/definitions/Coding""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""period"", ""class""]
				}
			]
		},
		""Encounter_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""individual"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Encounter_Diagnosis"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""condition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""role"": {
							""description"": ""Role that this diagnosis has within the encounter (e.g. admission, billing, discharge …)."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""rank"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_rank"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""condition""]
				}
			]
		},
		""Encounter_Hospitalization"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""preAdmissionIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""origin"": {
							""$ref"": ""#/definitions/Reference""
						},
						""admitSource"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reAdmission"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""dietPreference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialCourtesy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialArrangement"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""destination"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dischargeDisposition"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""Encounter_Location"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""enum"": [""planned"", ""active"", ""reserved"", ""completed""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""location""]
				}
			]
		},
		""Endpoint"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Endpoint""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""suspended"", ""error"", ""off"", ""entered-in-error"", ""test""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""connectionType"": {
							""$ref"": ""#/definitions/Coding""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""managingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""payloadType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""payloadMimeType"": {
							""description"": ""The mime type to send the payload in - e.g. application/fhir+xml, application/fhir+json. If the mime type is not specified, then the sender could send any content (including no content depending on the connectionType)."",
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_payloadMimeType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""address"": {
							""type"": ""string""
						},
						""_address"": {
							""$ref"": ""#/definitions/Element""
						},
						""header"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_header"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					},
					""required"": [""payloadType"", ""connectionType"", ""resourceType""]
				}
			]
		},
		""EnrollmentRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""EnrollmentRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""EnrollmentResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""EnrollmentResponse""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""description"": ""Processing status: error, complete."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestOrganization"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""EpisodeOfCare"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""EpisodeOfCare""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""planned"", ""waitlist"", ""active"", ""onhold"", ""finished"", ""cancelled"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""statusHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EpisodeOfCare_StatusHistory""
							}
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""diagnosis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/EpisodeOfCare_Diagnosis""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""managingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""referralRequest"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""careManager"": {
							""$ref"": ""#/definitions/Reference""
						},
						""team"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""account"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""EpisodeOfCare_StatusHistory"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""status"": {
							""enum"": [""planned"", ""waitlist"", ""active"", ""onhold"", ""finished"", ""cancelled"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""period""]
				}
			]
		},
		""EpisodeOfCare_Diagnosis"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""condition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""role"": {
							""description"": ""Role that this diagnosis has within the episode of care (e.g. admission, billing, discharge …)."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""rank"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_rank"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""condition""]
				}
			]
		},
		""ExpansionProfile"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ExpansionProfile""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""fixedVersion"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExpansionProfile_FixedVersion""
							}
						},
						""excludedSystem"": {
							""$ref"": ""#/definitions/ExpansionProfile_ExcludedSystem""
						},
						""includeDesignations"": {
							""type"": ""boolean""
						},
						""_includeDesignations"": {
							""$ref"": ""#/definitions/Element""
						},
						""designation"": {
							""$ref"": ""#/definitions/ExpansionProfile_Designation""
						},
						""includeDefinition"": {
							""type"": ""boolean""
						},
						""_includeDefinition"": {
							""$ref"": ""#/definitions/Element""
						},
						""activeOnly"": {
							""type"": ""boolean""
						},
						""_activeOnly"": {
							""$ref"": ""#/definitions/Element""
						},
						""excludeNested"": {
							""type"": ""boolean""
						},
						""_excludeNested"": {
							""$ref"": ""#/definitions/Element""
						},
						""excludeNotForUI"": {
							""type"": ""boolean""
						},
						""_excludeNotForUI"": {
							""$ref"": ""#/definitions/Element""
						},
						""excludePostCoordinated"": {
							""type"": ""boolean""
						},
						""_excludePostCoordinated"": {
							""$ref"": ""#/definitions/Element""
						},
						""displayLanguage"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_displayLanguage"": {
							""$ref"": ""#/definitions/Element""
						},
						""limitedExpansion"": {
							""description"": ""If the value set being expanded is incomplete (because it is too big to expand), return a limited expansion (a subset) with an indicator that expansion is incomplete, using the extension [http://hl7.org/fhir/StructureDefinition/valueset-toocostly](extension-valueset-toocostly.html)."",
							""type"": ""boolean""
						},
						""_limitedExpansion"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ExpansionProfile_FixedVersion"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""default"", ""check"", ""override""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ExpansionProfile_ExcludedSystem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ExpansionProfile_Designation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""include"": {
							""$ref"": ""#/definitions/ExpansionProfile_Include""
						},
						""exclude"": {
							""$ref"": ""#/definitions/ExpansionProfile_Exclude""
						}
					}
				}
			]
		},
		""ExpansionProfile_Include"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""designation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExpansionProfile_Designation1""
							}
						}
					}
				}
			]
		},
		""ExpansionProfile_Designation1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""$ref"": ""#/definitions/Coding""
						}
					}
				}
			]
		},
		""ExpansionProfile_Exclude"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""designation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExpansionProfile_Designation2""
							}
						}
					}
				}
			]
		},
		""ExpansionProfile_Designation2"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""$ref"": ""#/definitions/Coding""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ExplanationOfBenefit""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""cancelled"", ""draft"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""billablePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""enterer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""insurer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""referral"": {
							""$ref"": ""#/definitions/Reference""
						},
						""facility"": {
							""$ref"": ""#/definitions/Reference""
						},
						""claim"": {
							""description"": ""The business identifier for the instance: invoice number, claim number, pre-determination or pre-authorization number."",
							""$ref"": ""#/definitions/Reference""
						},
						""claimResponse"": {
							""description"": ""The business identifier for the instance: invoice number, claim number, pre-determination or pre-authorization number."",
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""related"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Related""
							}
						},
						""prescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""originalPrescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""payee"": {
							""$ref"": ""#/definitions/ExplanationOfBenefit_Payee""
						},
						""information"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Information""
							}
						},
						""careTeam"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_CareTeam""
							}
						},
						""diagnosis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Diagnosis""
							}
						},
						""procedure"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Procedure""
							}
						},
						""precedence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_precedence"": {
							""$ref"": ""#/definitions/Element""
						},
						""insurance"": {
							""$ref"": ""#/definitions/ExplanationOfBenefit_Insurance""
						},
						""accident"": {
							""$ref"": ""#/definitions/ExplanationOfBenefit_Accident""
						},
						""employmentImpacted"": {
							""$ref"": ""#/definitions/Period""
						},
						""hospitalization"": {
							""$ref"": ""#/definitions/Period""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Item""
							}
						},
						""addItem"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_AddItem""
							}
						},
						""totalCost"": {
							""$ref"": ""#/definitions/Money""
						},
						""unallocDeductable"": {
							""$ref"": ""#/definitions/Money""
						},
						""totalBenefit"": {
							""$ref"": ""#/definitions/Money""
						},
						""payment"": {
							""$ref"": ""#/definitions/ExplanationOfBenefit_Payment""
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""processNote"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_ProcessNote""
							}
						},
						""benefitBalance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_BenefitBalance""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ExplanationOfBenefit_Related"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""claim"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relationship"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reference"": {
							""$ref"": ""#/definitions/Identifier""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Payee"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""type"": {
							""description"": ""Type of Party to be reimbursed: Subscriber, provider, other."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""resourceType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""party"": {
							""description"": ""Party to be reimbursed: Subscriber, provider, other."",
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Information"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""description"": ""The general class of the information supplied: information; exception; accident, employment; onset, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""timingDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_timingDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reason"": {
							""description"": ""For example, provides the reason for: the additional stay, or missing tooth or any other situation where a reason code is required in addition to the content."",
							""$ref"": ""#/definitions/Coding""
						}
					},
					""required"": [""category""]
				}
			]
		},
		""ExplanationOfBenefit_CareTeam"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""responsible"": {
							""type"": ""boolean""
						},
						""_responsible"": {
							""$ref"": ""#/definitions/Element""
						},
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""qualification"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""provider""]
				}
			]
		},
		""ExplanationOfBenefit_Diagnosis"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""diagnosisCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""diagnosisReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""type"": {
							""description"": ""The type of the Diagnosis, for example: admitting, primary, secondary, discharge."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""packageCode"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Procedure"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""procedureCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""procedureReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Insurance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""coverage"": {
							""$ref"": ""#/definitions/Reference""
						},
						""preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_preAuthRef"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Accident"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""Type of accident: work, auto, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""locationAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""locationReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""careTeamLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_careTeamLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""diagnosisLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_diagnosisLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""procedureLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_procedureLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""informationLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_informationLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""servicedDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_servicedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""servicedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""locationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""locationAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""locationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subSite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""encounter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Adjudication""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Detail""
							}
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Adjudication"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""category"": {
							""description"": ""Code indicating: Co-Pay, deductable, elegible, benefit, tax, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""amount"": {
							""$ref"": ""#/definitions/Money""
						},
						""value"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""category""]
				}
			]
		},
		""ExplanationOfBenefit_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Adjudication""
							}
						},
						""subDetail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_SubDetail""
							}
						}
					},
					""required"": [""type""]
				}
			]
		},
		""ExplanationOfBenefit_SubDetail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_sequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""programCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""unitPrice"": {
							""$ref"": ""#/definitions/Money""
						},
						""factor"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_factor"": {
							""$ref"": ""#/definitions/Element""
						},
						""net"": {
							""description"": ""The quantity times the unit price for an addittional service or product or charge. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied."",
							""$ref"": ""#/definitions/Money""
						},
						""udi"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Adjudication""
							}
						}
					},
					""required"": [""type""]
				}
			]
		},
		""ExplanationOfBenefit_AddItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_sequenceLinkId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""fee"": {
							""$ref"": ""#/definitions/Money""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Adjudication""
							}
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Detail1""
							}
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Detail1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""revenue"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""service"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""fee"": {
							""$ref"": ""#/definitions/Money""
						},
						""noteNumber"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""[1-9][0-9]*""
							}
						},
						""_noteNumber"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""adjudication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Adjudication""
							}
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_Payment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""adjustment"": {
							""$ref"": ""#/definitions/Money""
						},
						""adjustmentReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""amount"": {
							""$ref"": ""#/definitions/Money""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_ProcessNote"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""number"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_number"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""description"": ""The note purpose: Print/Display."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""description"": ""The ISO-639-1 alpha 2 code in lower case for the language, optionally followed by a hyphen and the ISO-3166-1 alpha 2 code for the region in upper case; e.g. \""en\"" for English, or \""en-US\"" for American English versus \""en-EN\"" for England English."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""ExplanationOfBenefit_BenefitBalance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subCategory"": {
							""description"": ""Dental: basic, major, ortho; Vision exam, glasses, contacts; etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""excluded"": {
							""type"": ""boolean""
						},
						""_excluded"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""description"": ""A richer description of the benefit, for example 'DENT2 covers 100% of basic, 50% of major but exclused Ortho, Implants and Costmetic services'."",
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""network"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""unit"": {
							""description"": ""Unit designation: individual or family."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""term"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""financial"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ExplanationOfBenefit_Financial""
							}
						}
					},
					""required"": [""category""]
				}
			]
		},
		""ExplanationOfBenefit_Financial"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""This resource provides: the claim details; adjudication details from the processing of a Claim; and optionally account balance information, for informing the subscriber of the benefits provided."",
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""allowedUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_allowedUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""allowedString"": {
							""type"": ""string""
						},
						""_allowedString"": {
							""$ref"": ""#/definitions/Element""
						},
						""allowedMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""usedUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_usedUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""usedMoney"": {
							""$ref"": ""#/definitions/Money""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""FamilyMemberHistory"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""FamilyMemberHistory""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""partial"", ""completed"", ""entered-in-error"", ""health-unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDone"": {
							""type"": ""boolean""
						},
						""_notDone"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDoneReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""description"": ""This will either be a name or a description; e.g. \""Aunt Susan\"", \""my cousin with the red hair\""."",
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""relationship"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""bornPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""bornDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_bornDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""bornString"": {
							""type"": ""string""
						},
						""_bornString"": {
							""$ref"": ""#/definitions/Element""
						},
						""ageAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""ageRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""ageString"": {
							""type"": ""string""
						},
						""_ageString"": {
							""$ref"": ""#/definitions/Element""
						},
						""estimatedAge"": {
							""type"": ""boolean""
						},
						""_estimatedAge"": {
							""$ref"": ""#/definitions/Element""
						},
						""deceasedBoolean"": {
							""type"": ""boolean""
						},
						""_deceasedBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""deceasedAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""deceasedRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""deceasedDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_deceasedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""deceasedString"": {
							""type"": ""string""
						},
						""_deceasedString"": {
							""$ref"": ""#/definitions/Element""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""condition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/FamilyMemberHistory_Condition""
							}
						}
					},
					""required"": [""patient"", ""relationship"", ""resourceType""]
				}
			]
		},
		""FamilyMemberHistory_Condition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""outcome"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""onsetAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""onsetRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""onsetPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""onsetString"": {
							""type"": ""string""
						},
						""_onsetString"": {
							""$ref"": ""#/definitions/Element""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""code""]
				}
			]
		},
		""Flag"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Flag""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""code"", ""subject"", ""resourceType""]
				}
			]
		},
		""Goal"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Goal""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""proposed"", ""accepted"", ""planned"", ""in-progress"", ""on-target"", ""ahead-of-target"", ""behind-target"", ""sustaining"", ""achieved"", ""on-hold"", ""cancelled"", ""entered-in-error"", ""rejected""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""priority"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""description"": ""Human-readable and/or coded description of a specific desired objective of care, such as \""control blood pressure\"" or \""negotiate an obstacle course\"" or \""dance with child at wedding\""."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""startDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_startDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""startCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""target"": {
							""$ref"": ""#/definitions/Goal_Target""
						},
						""statusDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_statusDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""statusReason"": {
							""type"": ""string""
						},
						""_statusReason"": {
							""$ref"": ""#/definitions/Element""
						},
						""expressedBy"": {
							""$ref"": ""#/definitions/Reference""
						},
						""addresses"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""outcomeCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""outcomeReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""description"", ""resourceType""]
				}
			]
		},
		""Goal_Target"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""measure"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""detailQuantity"": {
							""description"": ""The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value."",
							""$ref"": ""#/definitions/Quantity""
						},
						""detailRange"": {
							""description"": ""The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value."",
							""$ref"": ""#/definitions/Range""
						},
						""detailCodeableConcept"": {
							""description"": ""The target value of the focus to be achieved to signify the fulfillment of the goal, e.g. 150 pounds, 7.0%. Either the high or low or both values of the range can be specified. When a low value is missing, it indicates that the goal is achieved at any focus value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any focus value at or above the low value."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""dueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_dueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""dueDuration"": {
							""$ref"": ""#/definitions/Duration""
						}
					}
				}
			]
		},
		""GraphDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""GraphDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""start"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""type"": ""string""
						},
						""_profile"": {
							""$ref"": ""#/definitions/Element""
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/GraphDefinition_Link""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""GraphDefinition_Link"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""sliceName"": {
							""type"": ""string""
						},
						""_sliceName"": {
							""$ref"": ""#/definitions/Element""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/GraphDefinition_Target""
							}
						}
					},
					""required"": [""target""]
				}
			]
		},
		""GraphDefinition_Target"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""type"": ""string""
						},
						""_profile"": {
							""$ref"": ""#/definitions/Element""
						},
						""compartment"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/GraphDefinition_Compartment""
							}
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/GraphDefinition_Link""
							}
						}
					}
				}
			]
		},
		""GraphDefinition_Compartment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""rule"": {
							""enum"": [""identical"", ""matching"", ""different"", ""custom""],
							""type"": ""string""
						},
						""_rule"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Group"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Group""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""person"", ""animal"", ""practitioner"", ""device"", ""medication"", ""substance""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""actual"": {
							""type"": ""boolean""
						},
						""_actual"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""description"": ""Provides a specific type of resource the group includes; e.g. \""cow\"", \""syringe\"", etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""quantity"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_quantity"": {
							""$ref"": ""#/definitions/Element""
						},
						""characteristic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Group_Characteristic""
							}
						},
						""member"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Group_Member""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Group_Characteristic"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""exclude"": {
							""type"": ""boolean""
						},
						""_exclude"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""Group_Member"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""entity"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""inactive"": {
							""type"": ""boolean""
						},
						""_inactive"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""entity""]
				}
			]
		},
		""GuidanceResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""GuidanceResponse""]
						},
						""requestId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_requestId"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""module"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""enum"": [""success"", ""data-requested"", ""data-required"", ""in-progress"", ""failure"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""evaluationMessage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""outputParameters"": {
							""$ref"": ""#/definitions/Reference""
						},
						""result"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dataRequirement"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement""
							}
						}
					},
					""required"": [""module"", ""resourceType""]
				}
			]
		},
		""HealthcareService"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""HealthcareService""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""providedBy"": {
							""$ref"": ""#/definitions/Reference""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""location"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""extraDetails"": {
							""type"": ""string""
						},
						""_extraDetails"": {
							""$ref"": ""#/definitions/Element""
						},
						""photo"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""coverageArea"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""serviceProvisionCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""eligibility"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""eligibilityNote"": {
							""type"": ""string""
						},
						""_eligibilityNote"": {
							""$ref"": ""#/definitions/Element""
						},
						""programName"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_programName"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""characteristic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""referralMethod"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""appointmentRequired"": {
							""type"": ""boolean""
						},
						""_appointmentRequired"": {
							""$ref"": ""#/definitions/Element""
						},
						""availableTime"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HealthcareService_AvailableTime""
							}
						},
						""notAvailable"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HealthcareService_NotAvailable""
							}
						},
						""availabilityExceptions"": {
							""type"": ""string""
						},
						""_availabilityExceptions"": {
							""$ref"": ""#/definitions/Element""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""HealthcareService_AvailableTime"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""daysOfWeek"": {
							""enum"": [""mon"", ""tue"", ""wed"", ""thu"", ""fri"", ""sat"", ""sun""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_daysOfWeek"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""allDay"": {
							""type"": ""boolean""
						},
						""_allDay"": {
							""$ref"": ""#/definitions/Element""
						},
						""availableStartTime"": {
							""description"": ""The opening time of day. Note: If the AllDay flag is set, then this time is ignored."",
							""type"": ""string"",
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?""
						},
						""_availableStartTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""availableEndTime"": {
							""description"": ""The closing time of day. Note: If the AllDay flag is set, then this time is ignored."",
							""type"": ""string"",
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?""
						},
						""_availableEndTime"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""HealthcareService_NotAvailable"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""during"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""ImagingManifest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ImagingManifest""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""authoringTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoringTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""description"": {
							""description"": ""Free text narrative description of the ImagingManifest.  \nThe value may be derived from the DICOM Standard Part 16, CID-7010 descriptions (e.g. Best in Set, Complete Study Content). Note that those values cover the wide range of uses of the DICOM Key Object Selection object, several of which are not supported by ImagingManifest. Specifically, there is no expected behavior associated with descriptions that suggest referenced images be removed or not used."",
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""study"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImagingManifest_Study""
							}
						}
					},
					""required"": [""study"", ""patient"", ""resourceType""]
				}
			]
		},
		""ImagingManifest_Study"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						},
						""imagingStudy"": {
							""$ref"": ""#/definitions/Reference""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""series"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImagingManifest_Series""
							}
						}
					},
					""required"": [""series""]
				}
			]
		},
		""ImagingManifest_Series"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""instance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImagingManifest_Instance""
							}
						}
					},
					""required"": [""instance""]
				}
			]
		},
		""ImagingManifest_Instance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sopClass"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_sopClass"": {
							""$ref"": ""#/definitions/Element""
						},
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ImagingStudy"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ImagingStudy""]
						},
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						},
						""accession"": {
							""description"": ""Accession Number is an identifier related to some aspect of imaging workflow and data management. Usage may vary across different institutions.  See for instance [IHE Radiology Technical Framework Volume 1 Appendix A](http://www.ihe.net/uploadedFiles/Documents/Radiology/IHE_RAD_TF_Rev13.0_Vol1_FT_2014-07-30.pdf)."",
							""$ref"": ""#/definitions/Identifier""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""availability"": {
							""enum"": [""ONLINE"", ""OFFLINE"", ""NEARLINE"", ""UNAVAILABLE""],
							""type"": ""string""
						},
						""_availability"": {
							""$ref"": ""#/definitions/Element""
						},
						""modalityList"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""started"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_started"": {
							""$ref"": ""#/definitions/Element""
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""referrer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""interpreter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""numberOfSeries"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_numberOfSeries"": {
							""$ref"": ""#/definitions/Element""
						},
						""numberOfInstances"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_numberOfInstances"": {
							""$ref"": ""#/definitions/Element""
						},
						""procedureReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""procedureCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""series"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImagingStudy_Series""
							}
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""ImagingStudy_Series"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						},
						""number"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_number"": {
							""$ref"": ""#/definitions/Element""
						},
						""modality"": {
							""$ref"": ""#/definitions/Coding""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""numberOfInstances"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_numberOfInstances"": {
							""$ref"": ""#/definitions/Element""
						},
						""availability"": {
							""enum"": [""ONLINE"", ""OFFLINE"", ""NEARLINE"", ""UNAVAILABLE""],
							""type"": ""string""
						},
						""_availability"": {
							""$ref"": ""#/definitions/Element""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""bodySite"": {
							""description"": ""The anatomic structures examined. See DICOM Part 16 Annex L (http://dicom.nema.org/medical/dicom/current/output/chtml/part16/chapter_L.html) for DICOM to SNOMED-CT mappings. The bodySite may indicate the laterality of body part imaged; if so, it shall be consistent with any content of ImagingStudy.series.laterality."",
							""$ref"": ""#/definitions/Coding""
						},
						""laterality"": {
							""$ref"": ""#/definitions/Coding""
						},
						""started"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_started"": {
							""$ref"": ""#/definitions/Element""
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""instance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImagingStudy_Instance""
							}
						}
					},
					""required"": [""modality""]
				}
			]
		},
		""ImagingStudy_Instance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""uid"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_uid"": {
							""$ref"": ""#/definitions/Element""
						},
						""number"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_number"": {
							""$ref"": ""#/definitions/Element""
						},
						""sopClass"": {
							""type"": ""string"",
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*""
						},
						""_sopClass"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Immunization"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Immunization""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""notGiven"": {
							""type"": ""boolean""
						},
						""_notGiven"": {
							""$ref"": ""#/definitions/Element""
						},
						""vaccineCode"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""primarySource"": {
							""type"": ""boolean""
						},
						""_primarySource"": {
							""$ref"": ""#/definitions/Element""
						},
						""reportOrigin"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""manufacturer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""lotNumber"": {
							""type"": ""string""
						},
						""_lotNumber"": {
							""$ref"": ""#/definitions/Element""
						},
						""expirationDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_expirationDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""site"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""route"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""doseQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""practitioner"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Immunization_Practitioner""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""explanation"": {
							""$ref"": ""#/definitions/Immunization_Explanation""
						},
						""reaction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Immunization_Reaction""
							}
						},
						""vaccinationProtocol"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Immunization_VaccinationProtocol""
							}
						}
					},
					""required"": [""patient"", ""vaccineCode"", ""resourceType""]
				}
			]
		},
		""Immunization_Practitioner"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""Immunization_Explanation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonNotGiven"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					}
				}
			]
		},
		""Immunization_Reaction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""detail"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reported"": {
							""type"": ""boolean""
						},
						""_reported"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Immunization_VaccinationProtocol"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""doseSequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_doseSequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""authority"": {
							""$ref"": ""#/definitions/Reference""
						},
						""series"": {
							""type"": ""string""
						},
						""_series"": {
							""$ref"": ""#/definitions/Element""
						},
						""seriesDoses"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_seriesDoses"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetDisease"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""doseStatus"": {
							""description"": ""Indicates if the immunization event should \""count\"" against  the protocol."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""doseStatusReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""doseStatus"", ""targetDisease""]
				}
			]
		},
		""ImmunizationRecommendation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ImmunizationRecommendation""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""recommendation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImmunizationRecommendation_Recommendation""
							}
						}
					},
					""required"": [""patient"", ""recommendation"", ""resourceType""]
				}
			]
		},
		""ImmunizationRecommendation_Recommendation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""vaccineCode"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""targetDisease"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""doseNumber"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_doseNumber"": {
							""$ref"": ""#/definitions/Element""
						},
						""forecastStatus"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""dateCriterion"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImmunizationRecommendation_DateCriterion""
							}
						},
						""protocol"": {
							""$ref"": ""#/definitions/ImmunizationRecommendation_Protocol""
						},
						""supportingImmunization"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""supportingPatientInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""forecastStatus""]
				}
			]
		},
		""ImmunizationRecommendation_DateCriterion"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""value"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""ImmunizationRecommendation_Protocol"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""doseSequence"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_doseSequence"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""authority"": {
							""$ref"": ""#/definitions/Reference""
						},
						""series"": {
							""type"": ""string""
						},
						""_series"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ImplementationGuide"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ImplementationGuide""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""fhirVersion"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_fhirVersion"": {
							""$ref"": ""#/definitions/Element""
						},
						""dependency"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImplementationGuide_Dependency""
							}
						},
						""package"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImplementationGuide_Package""
							}
						},
						""global"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImplementationGuide_Global""
							}
						},
						""binary"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_binary"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""page"": {
							""$ref"": ""#/definitions/ImplementationGuide_Page""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ImplementationGuide_Dependency"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""reference"", ""inclusion""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ImplementationGuide_Package"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImplementationGuide_Resource""
							}
						}
					},
					""required"": [""resource""]
				}
			]
		},
		""ImplementationGuide_Resource"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""example"": {
							""type"": ""boolean""
						},
						""_example"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""acronym"": {
							""type"": ""string""
						},
						""_acronym"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceUri"": {
							""type"": ""string""
						},
						""_sourceUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""exampleFor"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""ImplementationGuide_Global"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""profile""]
				}
			]
		},
		""ImplementationGuide_Page"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""source"": {
							""type"": ""string""
						},
						""_source"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""kind"": {
							""enum"": [""page"", ""example"", ""list"", ""include"", ""directory"", ""dictionary"", ""toc"", ""resource""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""package"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_package"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""format"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_format"": {
							""$ref"": ""#/definitions/Element""
						},
						""page"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ImplementationGuide_Page""
							}
						}
					}
				}
			]
		},
		""Library"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Library""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contributor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contributor""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""parameter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ParameterDefinition""
							}
						},
						""dataRequirement"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement""
							}
						},
						""content"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						}
					},
					""required"": [""type"", ""resourceType""]
				}
			]
		},
		""Linkage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""Identifies two or more records (resource instances) that are referring to the same real-world \""occurrence\""."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Linkage""]
						},
						""active"": {
							""description"": ""Indicates whether the asserted set of linkages are considered to be \""in effect\""."",
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Linkage_Item""
							}
						}
					},
					""required"": [""item"", ""resourceType""]
				}
			]
		},
		""Linkage_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""Identifies two or more records (resource instances) that are referring to the same real-world \""occurrence\""."",
					""properties"": {
						""type"": {
							""description"": ""Distinguishes which item is \""source of truth\"" (if any) and which items are no longer considered to be current representations."",
							""enum"": [""source"", ""alternate"", ""historical""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resource""]
				}
			]
		},
		""List"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""List""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""current"", ""retired"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""working"", ""snapshot"", ""changes""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""source"": {
							""$ref"": ""#/definitions/Reference""
						},
						""orderedBy"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""entry"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/List_Entry""
							}
						},
						""emptyReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""List_Entry"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""flag"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""deleted"": {
							""type"": ""boolean""
						},
						""_deleted"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""item"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""item""]
				}
			]
		},
		""Location"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Location""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""suspended"", ""inactive""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""operationalStatus"": {
							""$ref"": ""#/definitions/Coding""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""alias"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_alias"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""instance"", ""kind""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""address"": {
							""$ref"": ""#/definitions/Address""
						},
						""physicalType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""position"": {
							""$ref"": ""#/definitions/Location_Position""
						},
						""managingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""partOf"": {
							""$ref"": ""#/definitions/Reference""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Location_Position"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""longitude"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_longitude"": {
							""$ref"": ""#/definitions/Element""
						},
						""latitude"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_latitude"": {
							""$ref"": ""#/definitions/Element""
						},
						""altitude"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_altitude"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Measure"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Measure""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contributor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contributor""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""library"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""disclaimer"": {
							""type"": ""string""
						},
						""_disclaimer"": {
							""$ref"": ""#/definitions/Element""
						},
						""scoring"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""compositeScoring"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""riskAdjustment"": {
							""type"": ""string""
						},
						""_riskAdjustment"": {
							""$ref"": ""#/definitions/Element""
						},
						""rateAggregation"": {
							""type"": ""string""
						},
						""_rateAggregation"": {
							""$ref"": ""#/definitions/Element""
						},
						""rationale"": {
							""description"": ""Provides a succint statement of the need for the measure. Usually includes statements pertaining to importance criterion: impact, gap in care, and evidence."",
							""type"": ""string""
						},
						""_rationale"": {
							""$ref"": ""#/definitions/Element""
						},
						""clinicalRecommendationStatement"": {
							""type"": ""string""
						},
						""_clinicalRecommendationStatement"": {
							""$ref"": ""#/definitions/Element""
						},
						""improvementNotation"": {
							""type"": ""string""
						},
						""_improvementNotation"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""guidance"": {
							""type"": ""string""
						},
						""_guidance"": {
							""$ref"": ""#/definitions/Element""
						},
						""set"": {
							""type"": ""string""
						},
						""_set"": {
							""$ref"": ""#/definitions/Element""
						},
						""group"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Measure_Group""
							}
						},
						""supplementalData"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Measure_SupplementalData""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Measure_Group"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""population"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Measure_Population""
							}
						},
						""stratifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Measure_Stratifier""
							}
						}
					},
					""required"": [""identifier""]
				}
			]
		},
		""Measure_Population"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""criteria"": {
							""type"": ""string""
						},
						""_criteria"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Measure_Stratifier"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""criteria"": {
							""type"": ""string""
						},
						""_criteria"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Measure_SupplementalData"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""usage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""criteria"": {
							""type"": ""string""
						},
						""_criteria"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MeasureReport"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MeasureReport""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""complete"", ""pending"", ""error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""individual"", ""patient-list"", ""summary""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""measure"": {
							""$ref"": ""#/definitions/Reference""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""reportingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""group"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MeasureReport_Group""
							}
						},
						""evaluatedResources"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""period"", ""measure"", ""resourceType""]
				}
			]
		},
		""MeasureReport_Group"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""population"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MeasureReport_Population""
							}
						},
						""measureScore"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_measureScore"": {
							""$ref"": ""#/definitions/Element""
						},
						""stratifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MeasureReport_Stratifier""
							}
						}
					},
					""required"": [""identifier""]
				}
			]
		},
		""MeasureReport_Population"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""count"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_count"": {
							""$ref"": ""#/definitions/Element""
						},
						""patients"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""MeasureReport_Stratifier"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""stratum"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MeasureReport_Stratum""
							}
						}
					}
				}
			]
		},
		""MeasureReport_Stratum"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""population"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MeasureReport_Population1""
							}
						},
						""measureScore"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_measureScore"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MeasureReport_Population1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""count"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_count"": {
							""$ref"": ""#/definitions/Element""
						},
						""patients"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Media"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Media""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""type"": {
							""enum"": [""photo"", ""video"", ""audio""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""subtype"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""view"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""operator"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""device"": {
							""$ref"": ""#/definitions/Reference""
						},
						""height"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_height"": {
							""$ref"": ""#/definitions/Element""
						},
						""width"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_width"": {
							""$ref"": ""#/definitions/Element""
						},
						""frames"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_frames"": {
							""$ref"": ""#/definitions/Element""
						},
						""duration"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_duration"": {
							""$ref"": ""#/definitions/Element""
						},
						""content"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""content"", ""resourceType""]
				}
			]
		},
		""Medication"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Medication""]
						},
						""code"": {
							""description"": ""A code (or set of codes) that specify this medication, or a textual description if no code is available. Usage note: This could be a standard medication code such as a code from RxNorm, SNOMED CT, IDMP etc. It could also be a national or local formulary code, optionally with translations to other code systems."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""status"": {
							""enum"": [""active"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""isBrand"": {
							""type"": ""boolean""
						},
						""_isBrand"": {
							""$ref"": ""#/definitions/Element""
						},
						""isOverTheCounter"": {
							""type"": ""boolean""
						},
						""_isOverTheCounter"": {
							""$ref"": ""#/definitions/Element""
						},
						""manufacturer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""ingredient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Medication_Ingredient""
							}
						},
						""package"": {
							""$ref"": ""#/definitions/Medication_Package""
						},
						""image"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Medication_Ingredient"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""itemCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""itemReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""isActive"": {
							""type"": ""boolean""
						},
						""_isActive"": {
							""$ref"": ""#/definitions/Element""
						},
						""amount"": {
							""$ref"": ""#/definitions/Ratio""
						}
					}
				}
			]
		},
		""Medication_Package"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""container"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""content"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Medication_Content""
							}
						},
						""batch"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Medication_Batch""
							}
						}
					}
				}
			]
		},
		""Medication_Content"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""itemCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""itemReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""amount"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""Medication_Batch"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""lotNumber"": {
							""type"": ""string""
						},
						""_lotNumber"": {
							""$ref"": ""#/definitions/Element""
						},
						""expirationDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_expirationDate"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MedicationAdministration"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MedicationAdministration""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""in-progress"", ""on-hold"", ""completed"", ""entered-in-error"", ""stopped"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""supportingInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""effectiveDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_effectiveDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MedicationAdministration_Performer""
							}
						},
						""notGiven"": {
							""type"": ""boolean""
						},
						""_notGiven"": {
							""$ref"": ""#/definitions/Element""
						},
						""reasonNotGiven"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""prescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""device"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""dosage"": {
							""$ref"": ""#/definitions/MedicationAdministration_Dosage""
						},
						""eventHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""MedicationAdministration_Performer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""MedicationAdministration_Dosage"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""site"": {
							""description"": ""A coded specification of the anatomic site where the medication first entered the body.  For example, \""left arm\""."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""route"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""dose"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""rateRatio"": {
							""description"": ""Identifies the speed with which the medication was or will be introduced into the patient.  Typically the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours."",
							""$ref"": ""#/definitions/Ratio""
						},
						""rateSimpleQuantity"": {
							""description"": ""Identifies the speed with which the medication was or will be introduced into the patient.  Typically the rate for an infusion e.g. 100 ml per 1 hour or 100 ml/hr.  May also be expressed as a rate per unit of time e.g. 500 ml per 2 hours.  Other examples:  200 mcg/min or 200 mcg/1 minute; 1 liter/8 hours."",
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""MedicationDispense"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MedicationDispense""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""preparation"", ""in-progress"", ""on-hold"", ""completed"", ""entered-in-error"", ""stopped""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""supportingInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MedicationDispense_Performer""
							}
						},
						""authorizingPrescription"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""daysSupply"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""whenPrepared"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_whenPrepared"": {
							""$ref"": ""#/definitions/Element""
						},
						""whenHandedOver"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_whenHandedOver"": {
							""$ref"": ""#/definitions/Element""
						},
						""destination"": {
							""$ref"": ""#/definitions/Reference""
						},
						""receiver"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""dosageInstruction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Dosage""
							}
						},
						""substitution"": {
							""$ref"": ""#/definitions/MedicationDispense_Substitution""
						},
						""detectedIssue"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""notDone"": {
							""type"": ""boolean""
						},
						""_notDone"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDoneReasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""notDoneReasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""eventHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""MedicationDispense_Performer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""MedicationDispense_Substitution"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""wasSubstituted"": {
							""type"": ""boolean""
						},
						""_wasSubstituted"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""responsibleParty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""MedicationRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""An order or request for both supply of the medication and the instructions for administration of the medication to a patient. The resource is called \""MedicationRequest\"" rather than \""MedicationPrescription\"" or \""MedicationOrder\"" to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MedicationRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""active"", ""on-hold"", ""cancelled"", ""completed"", ""entered-in-error"", ""stopped"", ""draft"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""enum"": [""proposal"", ""plan"", ""order"", ""instance-order""],
							""type"": ""string""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""priority"": {
							""enum"": [""routine"", ""urgent"", ""stat"", ""asap""],
							""type"": ""string""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""medicationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""supportingInformation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/MedicationRequest_Requester""
						},
						""recorder"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""dosageInstruction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Dosage""
							}
						},
						""dispenseRequest"": {
							""$ref"": ""#/definitions/MedicationRequest_DispenseRequest""
						},
						""substitution"": {
							""$ref"": ""#/definitions/MedicationRequest_Substitution""
						},
						""priorPrescription"": {
							""$ref"": ""#/definitions/Reference""
						},
						""detectedIssue"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""eventHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""MedicationRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""An order or request for both supply of the medication and the instructions for administration of the medication to a patient. The resource is called \""MedicationRequest\"" rather than \""MedicationPrescription\"" or \""MedicationOrder\"" to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns."",
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""MedicationRequest_DispenseRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""An order or request for both supply of the medication and the instructions for administration of the medication to a patient. The resource is called \""MedicationRequest\"" rather than \""MedicationPrescription\"" or \""MedicationOrder\"" to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns."",
					""properties"": {
						""validityPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""numberOfRepeatsAllowed"": {
							""description"": ""An integer indicating the number of times, in addition to the original dispense, (aka refills or repeats) that the patient can receive the prescribed medication. Usage Notes: This integer does not include the original order dispense. This means that if an order indicates dispense 30 tablets plus \""3 repeats\"", then the order can be dispensed a total of 4 times and the patient can receive a total of 120 tablets."",
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_numberOfRepeatsAllowed"": {
							""$ref"": ""#/definitions/Element""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""expectedSupplyDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""MedicationRequest_Substitution"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""An order or request for both supply of the medication and the instructions for administration of the medication to a patient. The resource is called \""MedicationRequest\"" rather than \""MedicationPrescription\"" or \""MedicationOrder\"" to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns."",
					""properties"": {
						""allowed"": {
							""type"": ""boolean""
						},
						""_allowed"": {
							""$ref"": ""#/definitions/Element""
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""MedicationStatement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MedicationStatement""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""enum"": [""active"", ""completed"", ""entered-in-error"", ""intended"", ""stopped"", ""on-hold""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""medicationReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""effectiveDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_effectiveDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""dateAsserted"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_dateAsserted"": {
							""$ref"": ""#/definitions/Element""
						},
						""informationSource"": {
							""description"": ""The person or organization that provided the information about the taking of this medication. Note: Use derivedFrom when a MedicationStatement is derived from other resources, e.g Claim or MedicationRequest."",
							""$ref"": ""#/definitions/Reference""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""derivedFrom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""taken"": {
							""enum"": [""y"", ""n"", ""unk"", ""na""],
							""type"": ""string""
						},
						""_taken"": {
							""$ref"": ""#/definitions/Element""
						},
						""reasonNotTaken"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""dosage"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Dosage""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""MessageDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MessageDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""base"": {
							""$ref"": ""#/definitions/Reference""
						},
						""parent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""event"": {
							""$ref"": ""#/definitions/Coding""
						},
						""category"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_category"": {
							""$ref"": ""#/definitions/Element""
						},
						""focus"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MessageDefinition_Focus""
							}
						},
						""responseRequired"": {
							""type"": ""boolean""
						},
						""_responseRequired"": {
							""$ref"": ""#/definitions/Element""
						},
						""allowedResponse"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MessageDefinition_AllowedResponse""
							}
						}
					},
					""required"": [""event"", ""resourceType""]
				}
			]
		},
		""MessageDefinition_Focus"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""[0]|([1-9][0-9]*)""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MessageDefinition_AllowedResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""message"": {
							""$ref"": ""#/definitions/Reference""
						},
						""situation"": {
							""type"": ""string""
						},
						""_situation"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""message""]
				}
			]
		},
		""MessageHeader"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""MessageHeader""]
						},
						""event"": {
							""description"": ""Code that identifies the event this message represents and connects it with its definition. Events defined as part of the FHIR specification have the system value \""http://hl7.org/fhir/message-events\""."",
							""$ref"": ""#/definitions/Coding""
						},
						""destination"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/MessageHeader_Destination""
							}
						},
						""receiver"": {
							""$ref"": ""#/definitions/Reference""
						},
						""sender"": {
							""$ref"": ""#/definitions/Reference""
						},
						""timestamp"": {
							""type"": ""string""
						},
						""_timestamp"": {
							""$ref"": ""#/definitions/Element""
						},
						""enterer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""source"": {
							""$ref"": ""#/definitions/MessageHeader_Source""
						},
						""responsible"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""response"": {
							""$ref"": ""#/definitions/MessageHeader_Response""
						},
						""focus"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""source"", ""event"", ""resourceType""]
				}
			]
		},
		""MessageHeader_Destination"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""$ref"": ""#/definitions/Reference""
						},
						""endpoint"": {
							""type"": ""string""
						},
						""_endpoint"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MessageHeader_Source"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""software"": {
							""type"": ""string""
						},
						""_software"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""endpoint"": {
							""type"": ""string""
						},
						""_endpoint"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""MessageHeader_Response"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_identifier"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""enum"": [""ok"", ""transient-error"", ""fatal-error""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""details"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""NamingSystem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""A curated namespace that issues unique symbols within that namespace for the identification of concepts, people, devices, etc.  Represents a \""System\"" used within the Identifier and Coding data types."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""NamingSystem""]
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""kind"": {
							""description"": ""Indicates the purpose for the naming system - what kinds of things does it make unique?"",
							""enum"": [""codesystem"", ""identifier"", ""root""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""responsible"": {
							""type"": ""string""
						},
						""_responsible"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""uniqueId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/NamingSystem_UniqueId""
							}
						},
						""replacedBy"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""uniqueId"", ""resourceType""]
				}
			]
		},
		""NamingSystem_UniqueId"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A curated namespace that issues unique symbols within that namespace for the identification of concepts, people, devices, etc.  Represents a \""System\"" used within the Identifier and Coding data types."",
					""properties"": {
						""type"": {
							""enum"": [""oid"", ""uuid"", ""uri"", ""other""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""preferred"": {
							""description"": ""Indicates whether this identifier is the \""preferred\"" identifier of this type."",
							""type"": ""boolean""
						},
						""_preferred"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""NutritionOrder"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""NutritionOrder""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""proposed"", ""draft"", ""planned"", ""requested"", ""active"", ""on-hold"", ""completed"", ""cancelled"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dateTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_dateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""orderer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""allergyIntolerance"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""foodPreferenceModifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""excludeFoodModifier"": {
							""description"": ""This modifier is used to convey order-specific modifiers about the type of food that should NOT be given. These can be derived from patient allergies, intolerances, or preferences such as No Red Meat, No Soy or No Wheat or  Gluten-Free.  While it should not be necessary to repeat allergy or intolerance information captured in the referenced AllergyIntolerance resource in the excludeFoodModifier, this element may be used to convey additional specificity related to foods that should be eliminated from the patient’s diet for any reason.  This modifier applies to the entire nutrition order inclusive of the oral diet, nutritional supplements and enteral formula feedings."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""oralDiet"": {
							""$ref"": ""#/definitions/NutritionOrder_OralDiet""
						},
						""supplement"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/NutritionOrder_Supplement""
							}
						},
						""enteralFormula"": {
							""$ref"": ""#/definitions/NutritionOrder_EnteralFormula""
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""NutritionOrder_OralDiet"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""schedule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Timing""
							}
						},
						""nutrient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/NutritionOrder_Nutrient""
							}
						},
						""texture"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/NutritionOrder_Texture""
							}
						},
						""fluidConsistencyType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""instruction"": {
							""type"": ""string""
						},
						""_instruction"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""NutritionOrder_Nutrient"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""modifier"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""amount"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""NutritionOrder_Texture"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""modifier"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""foodType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""NutritionOrder_Supplement"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""productName"": {
							""description"": ""The product or brand name of the nutritional supplement such as \""Acme Protein Shake\""."",
							""type"": ""string""
						},
						""_productName"": {
							""$ref"": ""#/definitions/Element""
						},
						""schedule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Timing""
							}
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""instruction"": {
							""type"": ""string""
						},
						""_instruction"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""NutritionOrder_EnteralFormula"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""baseFormulaType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""baseFormulaProductName"": {
							""description"": ""The product or brand name of the enteral or infant formula product such as \""ACME Adult Standard Formula\""."",
							""type"": ""string""
						},
						""_baseFormulaProductName"": {
							""$ref"": ""#/definitions/Element""
						},
						""additiveType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""additiveProductName"": {
							""type"": ""string""
						},
						""_additiveProductName"": {
							""$ref"": ""#/definitions/Element""
						},
						""caloricDensity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""routeofAdministration"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""administration"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/NutritionOrder_Administration""
							}
						},
						""maxVolumeToDeliver"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""administrationInstruction"": {
							""type"": ""string""
						},
						""_administrationInstruction"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""NutritionOrder_Administration"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""schedule"": {
							""$ref"": ""#/definitions/Timing""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""rateSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""rateRatio"": {
							""$ref"": ""#/definitions/Ratio""
						}
					}
				}
			]
		},
		""Observation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Observation""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""registered"", ""preliminary"", ""final"", ""amended"", ""corrected"", ""cancelled"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""code"": {
							""description"": ""Describes what was observed. Sometimes this is called the observation \""name\""."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""description"": ""The patient, or group of patients, location, or device whose characteristics (direct or indirect) are described by the observation and into whose record the observation is placed.  Comments: Indirect characteristics may be those of a specimen, fetus, donor,  other observer (for example a relative or EMT), or any observation made about the subject."",
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""effectiveDateTime"": {
							""description"": ""The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the \""physiologically relevant time\"". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself."",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_effectiveDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""description"": ""The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the \""physiologically relevant time\"". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself."",
							""$ref"": ""#/definitions/Period""
						},
						""issued"": {
							""type"": ""string""
						},
						""_issued"": {
							""$ref"": ""#/definitions/Element""
						},
						""performer"": {
							""description"": ""Who was responsible for asserting the observed value as \""true\""."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""dataAbsentReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""interpretation"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""specimen"": {
							""$ref"": ""#/definitions/Reference""
						},
						""device"": {
							""$ref"": ""#/definitions/Reference""
						},
						""referenceRange"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Observation_ReferenceRange""
							}
						},
						""related"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Observation_Related""
							}
						},
						""component"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Observation_Component""
							}
						}
					},
					""required"": [""code"", ""resourceType""]
				}
			]
		},
		""Observation_ReferenceRange"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""low"": {
							""description"": ""The value of the low bound of the reference range.  The low bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9).   If the low bound is omitted,  it is assumed to be meaningless (e.g. reference range is <=2.3)."",
							""$ref"": ""#/definitions/Quantity""
						},
						""high"": {
							""description"": ""The value of the high bound of the reference range.  The high bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9).   If the high bound is omitted,  it is assumed to be meaningless (e.g. reference range is >= 2.3)."",
							""$ref"": ""#/definitions/Quantity""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""appliesTo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""age"": {
							""$ref"": ""#/definitions/Range""
						},
						""text"": {
							""description"": ""Text based reference range in an observation which may be used when a quantitative range is not appropriate for an observation.  An example would be a reference value of \""Negative\"" or a list or table of 'normals'."",
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Observation_Related"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""has-member"", ""derived-from"", ""sequel-to"", ""replaces"", ""qualified-by"", ""interfered-by""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""target""]
				}
			]
		},
		""Observation_Component"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""description"": ""Describes what was observed. Sometimes this is called the observation \""code\""."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""dataAbsentReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""interpretation"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""referenceRange"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Observation_ReferenceRange""
							}
						}
					},
					""required"": [""code""]
				}
			]
		},
		""OperationDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""OperationDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""kind"": {
							""enum"": [""operation"", ""query""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""idempotent"": {
							""description"": ""Operations that are idempotent (see [HTTP specification definition of idempotent](http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html)) may be invoked by performing an HTTP GET operation instead of a POST."",
							""type"": ""boolean""
						},
						""_idempotent"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						},
						""base"": {
							""$ref"": ""#/definitions/Reference""
						},
						""resource"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_resource"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""system"": {
							""type"": ""boolean""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""boolean""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""instance"": {
							""type"": ""boolean""
						},
						""_instance"": {
							""$ref"": ""#/definitions/Element""
						},
						""parameter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/OperationDefinition_Parameter""
							}
						},
						""overload"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/OperationDefinition_Overload""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""OperationDefinition_Parameter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""enum"": [""in"", ""out""],
							""type"": ""string""
						},
						""_use"": {
							""$ref"": ""#/definitions/Element""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""searchType"": {
							""enum"": [""number"", ""date"", ""string"", ""token"", ""reference"", ""composite"", ""quantity"", ""uri""],
							""type"": ""string""
						},
						""_searchType"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Reference""
						},
						""binding"": {
							""$ref"": ""#/definitions/OperationDefinition_Binding""
						},
						""part"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/OperationDefinition_Parameter""
							}
						}
					}
				}
			]
		},
		""OperationDefinition_Binding"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""strength"": {
							""enum"": [""required"", ""extensible"", ""preferred"", ""example""],
							""type"": ""string""
						},
						""_strength"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetUri"": {
							""type"": ""string""
						},
						""_valueSetUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueSetReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""OperationDefinition_Overload"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""parameterName"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_parameterName"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""OperationOutcome"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""OperationOutcome""]
						},
						""issue"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/OperationOutcome_Issue""
							}
						}
					},
					""required"": [""issue"", ""resourceType""]
				}
			]
		},
		""OperationOutcome_Issue"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""severity"": {
							""enum"": [""fatal"", ""error"", ""warning"", ""information""],
							""type"": ""string""
						},
						""_severity"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""enum"": [""invalid"", ""structure"", ""required"", ""value"", ""invariant"", ""security"", ""login"", ""unknown"", ""expired"", ""forbidden"", ""suppressed"", ""processing"", ""not-supported"", ""duplicate"", ""not-found"", ""too-long"", ""code-invalid"", ""extension"", ""too-costly"", ""business-rule"", ""conflict"", ""incomplete"", ""transient"", ""lock-error"", ""no-store"", ""exception"", ""timeout"", ""throttled"", ""informational""],
							""type"": ""string""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""details"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""diagnostics"": {
							""type"": ""string""
						},
						""_diagnostics"": {
							""$ref"": ""#/definitions/Element""
						},
						""location"": {
							""description"": ""For resource issues, this will be a simple XPath limited to element names, repetition indicators and the default child access that identifies one of the elements in the resource that caused this issue to be raised.  For HTTP errors, will be \""http.\"" + the parameter name."",
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_location"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""expression"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_expression"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""Organization"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Organization""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""alias"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_alias"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""address"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Address""
							}
						},
						""partOf"": {
							""$ref"": ""#/definitions/Reference""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Organization_Contact""
							}
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Organization_Contact"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""purpose"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""name"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""address"": {
							""$ref"": ""#/definitions/Address""
						}
					}
				}
			]
		},
		""Parameters"": {
			""allOf"": [{
					""$ref"": ""#/definitions/Resource""
				}, {
					""properties"": {
						""parameter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Parameters_Parameter""
							}
						}
					}
				}
			]
		},
		""Parameters_Parameter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBase64Binary"": {
							""type"": ""string""
						},
						""_valueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInstant"": {
							""type"": ""string""
						},
						""_valueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_valueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_valueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_valueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_valuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueMarkdown"": {
							""type"": ""string""
						},
						""_valueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""valueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""valueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""valueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""valueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""valueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""valueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""valueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""valueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""valueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""valueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""valueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""valueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""valueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""valueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""valueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""valueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""valueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""valueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""valueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""valueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""valueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						},
						""resource"": {
							""$ref"": ""#/definitions/ResourceList""
						},
						""part"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Parameters_Parameter""
							}
						}
					}
				}
			]
		},
		""Patient"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Patient""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HumanName""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""birthDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_birthDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""deceasedBoolean"": {
							""type"": ""boolean""
						},
						""_deceasedBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""deceasedDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_deceasedDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""address"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Address""
							}
						},
						""maritalStatus"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""multipleBirthBoolean"": {
							""type"": ""boolean""
						},
						""_multipleBirthBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""multipleBirthInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_multipleBirthInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""photo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Patient_Contact""
							}
						},
						""animal"": {
							""$ref"": ""#/definitions/Patient_Animal""
						},
						""communication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Patient_Communication""
							}
						},
						""generalPractitioner"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""managingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Patient_Link""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Patient_Contact"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""relationship"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""name"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""address"": {
							""$ref"": ""#/definitions/Address""
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""Patient_Animal"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""species"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""breed"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""genderStatus"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""species""]
				}
			]
		},
		""Patient_Communication"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""language"": {
							""description"": ""The ISO-639-1 alpha 2 code in lower case for the language, optionally followed by a hyphen and the ISO-3166-1 alpha 2 code for the region in upper case; e.g. \""en\"" for English, or \""en-US\"" for American English versus \""en-EN\"" for England English."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""preferred"": {
							""type"": ""boolean""
						},
						""_preferred"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""language""]
				}
			]
		},
		""Patient_Link"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""other"": {
							""$ref"": ""#/definitions/Reference""
						},
						""type"": {
							""enum"": [""replaced-by"", ""replaces"", ""refer"", ""seealso""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""other""]
				}
			]
		},
		""PaymentNotice"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""PaymentNotice""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""response"": {
							""$ref"": ""#/definitions/Reference""
						},
						""statusDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_statusDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""$ref"": ""#/definitions/Reference""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""paymentStatus"": {
							""description"": ""The payment status, typically paid: payment sent, cleared: payment received."",
							""$ref"": ""#/definitions/CodeableConcept""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""PaymentReconciliation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""PaymentReconciliation""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""description"": ""Transaction status: error, complete."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""detail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PaymentReconciliation_Detail""
							}
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""total"": {
							""$ref"": ""#/definitions/Money""
						},
						""processNote"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PaymentReconciliation_ProcessNote""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""PaymentReconciliation_Detail"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""response"": {
							""$ref"": ""#/definitions/Reference""
						},
						""submitter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""payee"": {
							""$ref"": ""#/definitions/Reference""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""amount"": {
							""$ref"": ""#/definitions/Money""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""PaymentReconciliation_ProcessNote"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""description"": ""The note purpose: Print/Display."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Person"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Person""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""name"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HumanName""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""birthDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_birthDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""address"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Address""
							}
						},
						""photo"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""managingOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Person_Link""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Person_Link"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""target"": {
							""$ref"": ""#/definitions/Reference""
						},
						""assurance"": {
							""enum"": [""level1"", ""level2"", ""level3"", ""level4""],
							""type"": ""string""
						},
						""_assurance"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""target""]
				}
			]
		},
		""PlanDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""PlanDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contributor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contributor""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""library"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""goal"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Goal""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Action""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""PlanDefinition_Goal"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""description"": ""Human-readable and/or coded description of a specific desired objective of care, such as \""control blood pressure\"" or \""negotiate an obstacle course\"" or \""dance with child at wedding\""."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""priority"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""start"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""addresses"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""documentation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Target""
							}
						}
					},
					""required"": [""description""]
				}
			]
		},
		""PlanDefinition_Target"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""measure"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""detailQuantity"": {
							""description"": ""The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. Whan a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value."",
							""$ref"": ""#/definitions/Quantity""
						},
						""detailRange"": {
							""description"": ""The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. Whan a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value."",
							""$ref"": ""#/definitions/Range""
						},
						""detailCodeableConcept"": {
							""description"": ""The target value of the measure to be achieved to signify fulfillment of the goal, e.g. 150 pounds or 7.0%. Either the high or low or both values of the range can be specified. Whan a low value is missing, it indicates that the goal is achieved at any value at or below the high value. Similarly, if the high value is missing, it indicates that the goal is achieved at any value at or above the low value."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""due"": {
							""$ref"": ""#/definitions/Duration""
						}
					}
				}
			]
		},
		""PlanDefinition_Action"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""textEquivalent"": {
							""type"": ""string""
						},
						""_textEquivalent"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""documentation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""goalId"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
							}
						},
						""_goalId"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""triggerDefinition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TriggerDefinition""
							}
						},
						""condition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Condition""
							}
						},
						""input"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement""
							}
						},
						""output"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement""
							}
						},
						""relatedAction"": {
							""description"": ""A relationship to another action such as \""before\"" or \""30-60 minutes after start of\""."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_RelatedAction""
							}
						},
						""timingDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_timingDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""timingDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""timingRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""timingTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Participant""
							}
						},
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""groupingBehavior"": {
							""enum"": [""visual-group"", ""logical-group"", ""sentence-group""],
							""type"": ""string""
						},
						""_groupingBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""selectionBehavior"": {
							""enum"": [""any"", ""all"", ""all-or-none"", ""exactly-one"", ""at-most-one"", ""one-or-more""],
							""type"": ""string""
						},
						""_selectionBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""requiredBehavior"": {
							""enum"": [""must"", ""could"", ""must-unless-documented""],
							""type"": ""string""
						},
						""_requiredBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""precheckBehavior"": {
							""enum"": [""yes"", ""no""],
							""type"": ""string""
						},
						""_precheckBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""cardinalityBehavior"": {
							""enum"": [""single"", ""multiple""],
							""type"": ""string""
						},
						""_cardinalityBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""transform"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dynamicValue"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_DynamicValue""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PlanDefinition_Action""
							}
						}
					}
				}
			]
		},
		""PlanDefinition_Condition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""kind"": {
							""enum"": [""applicability"", ""start"", ""stop""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""PlanDefinition_RelatedAction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""actionId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_actionId"": {
							""$ref"": ""#/definitions/Element""
						},
						""relationship"": {
							""enum"": [""before-start"", ""before"", ""before-end"", ""concurrent-with-start"", ""concurrent"", ""concurrent-with-end"", ""after-start"", ""after"", ""after-end""],
							""type"": ""string""
						},
						""_relationship"": {
							""$ref"": ""#/definitions/Element""
						},
						""offsetDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""offsetRange"": {
							""$ref"": ""#/definitions/Range""
						}
					}
				}
			]
		},
		""PlanDefinition_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""patient"", ""practitioner"", ""related-person""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""role"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""PlanDefinition_DynamicValue"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Practitioner"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Practitioner""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HumanName""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""address"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Address""
							}
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""birthDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_birthDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""photo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						},
						""qualification"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Practitioner_Qualification""
							}
						},
						""communication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Practitioner_Qualification"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""issuer"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""code""]
				}
			]
		},
		""PractitionerRole"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""PractitionerRole""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""practitioner"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""location"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""healthcareService"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""availableTime"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PractitionerRole_AvailableTime""
							}
						},
						""notAvailable"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/PractitionerRole_NotAvailable""
							}
						},
						""availabilityExceptions"": {
							""type"": ""string""
						},
						""_availabilityExceptions"": {
							""$ref"": ""#/definitions/Element""
						},
						""endpoint"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""PractitionerRole_AvailableTime"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""daysOfWeek"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_daysOfWeek"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""allDay"": {
							""type"": ""boolean""
						},
						""_allDay"": {
							""$ref"": ""#/definitions/Element""
						},
						""availableStartTime"": {
							""description"": ""The opening time of day. Note: If the AllDay flag is set, then this time is ignored."",
							""type"": ""string"",
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?""
						},
						""_availableStartTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""availableEndTime"": {
							""description"": ""The closing time of day. Note: If the AllDay flag is set, then this time is ignored."",
							""type"": ""string"",
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?""
						},
						""_availableEndTime"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""PractitionerRole_NotAvailable"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""during"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""Procedure"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Procedure""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDone"": {
							""type"": ""boolean""
						},
						""_notDone"": {
							""$ref"": ""#/definitions/Element""
						},
						""notDoneReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""category"": {
							""description"": ""A code that classifies the procedure for searching, sorting and display purposes (e.g. \""Surgical Procedure\"")."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""description"": ""The specific procedure that is performed. Use text if the exact nature of the procedure cannot be coded (e.g. \""Laparoscopic Appendectomy\"")."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""performedDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_performedDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""performedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""performer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Procedure_Performer""
							}
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""bodySite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""outcome"": {
							""description"": ""The outcome of the procedure - did it resolve reasons for the procedure being performed?"",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""report"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""complication"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""complicationDetail"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""followUp"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""focalDevice"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Procedure_FocalDevice""
							}
						},
						""usedReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""usedCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""Procedure_Performer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""description"": ""For example: surgeon, anaethetist, endoscopist."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""actor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""actor""]
				}
			]
		},
		""Procedure_FocalDevice"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""manipulated"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""manipulated""]
				}
			]
		},
		""ProcedureRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ProcedureRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""requisition"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""doNotPerform"": {
							""type"": ""boolean""
						},
						""_doNotPerform"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""description"": ""A code that classifies the procedure for searching, sorting and display purposes (e.g. \""Surgical Procedure\"")."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""occurrenceTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""asNeededBoolean"": {
							""description"": ""If a CodeableConcept is present, it indicates the pre-condition for performing the procedure.  For example \""pain\"", \""on flare-up\"", etc."",
							""type"": ""boolean""
						},
						""_asNeededBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""asNeededCodeableConcept"": {
							""description"": ""If a CodeableConcept is present, it indicates the pre-condition for performing the procedure.  For example \""pain\"", \""on flare-up\"", etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/ProcedureRequest_Requester""
						},
						""performerType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""supportingInfo"": {
							""description"": ""Additional clinical information about the patient or specimen that may influence the procedure or diagnostics or their interpretations.     This information includes diagnosis, clinical findings and other observations.  In laboratory ordering these are typically referred to as \""ask at order entry questions (AOEs)\"".  This includes observations explicitly requested by the producer (filler) to provide context or supporting information needed to complete the order. For example,  reporting the amount of inspired oxygen for blood gas measurements."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""specimen"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""bodySite"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""note"": {
							""description"": ""Any other notes and comments made about the service request. For example, letting provider know that \""patient hates needles\"" or other provider instructions."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""relevantHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""code"", ""subject"", ""resourceType""]
				}
			]
		},
		""ProcedureRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""ProcessRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ProcessRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""action"": {
							""enum"": [""cancel"", ""poll"", ""reprocess"", ""status""],
							""type"": ""string""
						},
						""_action"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""$ref"": ""#/definitions/Reference""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""provider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""response"": {
							""$ref"": ""#/definitions/Reference""
						},
						""nullify"": {
							""type"": ""boolean""
						},
						""_nullify"": {
							""$ref"": ""#/definitions/Element""
						},
						""reference"": {
							""type"": ""string""
						},
						""_reference"": {
							""$ref"": ""#/definitions/Element""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ProcessRequest_Item""
							}
						},
						""include"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_include"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""exclude"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_exclude"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ProcessRequest_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""sequenceLinkId"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_sequenceLinkId"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ProcessResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ProcessResponse""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""created"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_created"": {
							""$ref"": ""#/definitions/Element""
						},
						""organization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""request"": {
							""$ref"": ""#/definitions/Reference""
						},
						""outcome"": {
							""description"": ""Transaction status: error, complete, held."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""disposition"": {
							""type"": ""string""
						},
						""_disposition"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestProvider"": {
							""$ref"": ""#/definitions/Reference""
						},
						""requestOrganization"": {
							""$ref"": ""#/definitions/Reference""
						},
						""form"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""processNote"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ProcessResponse_ProcessNote""
							}
						},
						""error"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""communicationRequest"": {
							""description"": ""Request for additional supporting or authorizing information, such as: documents, images or resources."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ProcessResponse_ProcessNote"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""description"": ""The note purpose: Print/Display."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Provenance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Provenance""]
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""recorded"": {
							""type"": ""string""
						},
						""_recorded"": {
							""$ref"": ""#/definitions/Element""
						},
						""policy"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_policy"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""location"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reason"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""activity"": {
							""$ref"": ""#/definitions/Coding""
						},
						""agent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Provenance_Agent""
							}
						},
						""entity"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Provenance_Entity""
							}
						},
						""signature"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Signature""
							}
						}
					},
					""required"": [""agent"", ""resourceType"", ""target""]
				}
			]
		},
		""Provenance_Agent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""whoUri"": {
							""type"": ""string""
						},
						""_whoUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""whoReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOfUri"": {
							""type"": ""string""
						},
						""_onBehalfOfUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""onBehalfOfReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relatedAgentType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""Provenance_Entity"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""role"": {
							""enum"": [""derivation"", ""revision"", ""quotation"", ""source"", ""removal""],
							""type"": ""string""
						},
						""_role"": {
							""$ref"": ""#/definitions/Element""
						},
						""whatUri"": {
							""type"": ""string""
						},
						""_whatUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""whatReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""whatIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""agent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Provenance_Agent""
							}
						}
					}
				}
			]
		},
		""Questionnaire"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Questionnaire""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""subjectType"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_subjectType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Questionnaire_Item""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Questionnaire_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""linkId"": {
							""type"": ""string""
						},
						""_linkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""description"": ""A reference to an [[[ElementDefinition]]] that provides the details for the item. If a definition is provided, then the following element values can be inferred from the definition: \n\n* code (ElementDefinition.code)\n* type (ElementDefinition.type)\n* required (ElementDefinition.min)\n* repeats (ElementDefinition.max)\n* maxLength (ElementDefinition.maxLength)\n* options (ElementDefinition.binding)\n\nAny information provided in these elements on a Questionnaire Item overrides the information from the definition."",
							""type"": ""string""
						},
						""_definition"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""prefix"": {
							""type"": ""string""
						},
						""_prefix"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""enum"": [""group"", ""display"", ""boolean"", ""decimal"", ""integer"", ""date"", ""dateTime"", ""time"", ""string"", ""text"", ""url"", ""choice"", ""open-choice"", ""attachment"", ""reference"", ""quantity""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""enableWhen"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Questionnaire_EnableWhen""
							}
						},
						""required"": {
							""description"": ""An indication, if true, that the item must be present in a \""completed\"" QuestionnaireResponse.  If false, the item may be skipped when answering the questionnaire."",
							""type"": ""boolean""
						},
						""_required"": {
							""$ref"": ""#/definitions/Element""
						},
						""repeats"": {
							""type"": ""boolean""
						},
						""_repeats"": {
							""$ref"": ""#/definitions/Element""
						},
						""readOnly"": {
							""type"": ""boolean""
						},
						""_readOnly"": {
							""$ref"": ""#/definitions/Element""
						},
						""maxLength"": {
							""description"": ""The maximum number of characters that are permitted in the answer to be considered a \""valid\"" QuestionnaireResponse."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_maxLength"": {
							""$ref"": ""#/definitions/Element""
						},
						""options"": {
							""description"": ""A reference to a value set containing a list of codes representing permitted answers for a \""choice\"" or \""open-choice\"" question."",
							""$ref"": ""#/definitions/Reference""
						},
						""option"": {
							""description"": ""One of the permitted answers for a \""choice\"" or \""open-choice\"" question."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Questionnaire_Option""
							}
						},
						""initialBoolean"": {
							""type"": ""boolean""
						},
						""_initialBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_initialDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_initialInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_initialDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_initialDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_initialTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialString"": {
							""type"": ""string""
						},
						""_initialString"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialUri"": {
							""type"": ""string""
						},
						""_initialUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""initialAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""initialCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""initialQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""initialReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Questionnaire_Item""
							}
						}
					}
				}
			]
		},
		""Questionnaire_EnableWhen"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""question"": {
							""type"": ""string""
						},
						""_question"": {
							""$ref"": ""#/definitions/Element""
						},
						""hasAnswer"": {
							""type"": ""boolean""
						},
						""_hasAnswer"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerBoolean"": {
							""type"": ""boolean""
						},
						""_answerBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_answerDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_answerInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_answerDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_answerDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_answerTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerString"": {
							""type"": ""string""
						},
						""_answerString"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerUri"": {
							""type"": ""string""
						},
						""_answerUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""answerAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""answerCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""answerQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""answerReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Questionnaire_Option"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						}
					}
				}
			]
		},
		""QuestionnaireResponse"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""QuestionnaireResponse""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""parent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""questionnaire"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""enum"": [""in-progress"", ""completed"", ""amended"", ""entered-in-error"", ""stopped""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""authored"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authored"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""source"": {
							""$ref"": ""#/definitions/Reference""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/QuestionnaireResponse_Item""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""QuestionnaireResponse_Item"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""linkId"": {
							""type"": ""string""
						},
						""_linkId"": {
							""$ref"": ""#/definitions/Element""
						},
						""definition"": {
							""type"": ""string""
						},
						""_definition"": {
							""$ref"": ""#/definitions/Element""
						},
						""text"": {
							""type"": ""string""
						},
						""_text"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""answer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/QuestionnaireResponse_Answer""
							}
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/QuestionnaireResponse_Item""
							}
						}
					}
				}
			]
		},
		""QuestionnaireResponse_Answer"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""item"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/QuestionnaireResponse_Item""
							}
						}
					}
				}
			]
		},
		""ReferralRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ReferralRequest""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""description"": ""The business identifier of the logical \""grouping\"" request/order that this referral is a part of."",
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""description"": ""Distinguishes the \""level\"" of authorization/demand implicit in this request."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""serviceRequested"": {
							""description"": ""The service(s) that is/are requested to be provided to the patient.  For example: cardiac pacemaker insertion."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/ReferralRequest_Requester""
						},
						""specialty"": {
							""description"": ""Indication of the clinical domain or discipline to which the referral or transfer of care request is sent.  For example: Cardiology Gastroenterology Diabetology."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""recipient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""reasonCode"": {
							""description"": ""Description of clinical condition indicating why referral/transfer of care is requested.  For example:  Pathological Anomalies, Disabled (physical or mental),  Behavioral Management."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""reasonReference"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""supportingInfo"": {
							""description"": ""Any additional (administrative, financial or clinical) information required to support request for referral or transfer of care.  For example: Presenting problems/chief complaints Medical History Family History Alerts Allergy/Intolerance and Adverse Reactions Medications Observations/Assessments (may include cognitive and fundtional assessments) Diagnostic Reports Care Plan."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""relevantHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""ReferralRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""RelatedPerson"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""RelatedPerson""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""relationship"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""name"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/HumanName""
							}
						},
						""telecom"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""gender"": {
							""enum"": [""male"", ""female"", ""other"", ""unknown""],
							""type"": ""string""
						},
						""_gender"": {
							""$ref"": ""#/definitions/Element""
						},
						""birthDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_birthDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""address"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Address""
							}
						},
						""photo"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Attachment""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						}
					},
					""required"": [""patient"", ""resourceType""]
				}
			]
		},
		""RequestGroup"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""A group of related requests that can be used to capture intended activities that have inter-dependencies such as \""give this medication after that one\""."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""RequestGroup""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""replaces"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""intent"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""author"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RequestGroup_Action""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""RequestGroup_Action"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A group of related requests that can be used to capture intended activities that have inter-dependencies such as \""give this medication after that one\""."",
					""properties"": {
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""textEquivalent"": {
							""type"": ""string""
						},
						""_textEquivalent"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""documentation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""condition"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RequestGroup_Condition""
							}
						},
						""relatedAction"": {
							""description"": ""A relationship to another action such as \""before\"" or \""30-60 minutes after start of\""."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RequestGroup_RelatedAction""
							}
						},
						""timingDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_timingDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""timingPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""timingDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""timingRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""timingTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""groupingBehavior"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_groupingBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""selectionBehavior"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_selectionBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""requiredBehavior"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_requiredBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""precheckBehavior"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_precheckBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""cardinalityBehavior"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_cardinalityBehavior"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RequestGroup_Action""
							}
						}
					}
				}
			]
		},
		""RequestGroup_Condition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A group of related requests that can be used to capture intended activities that have inter-dependencies such as \""give this medication after that one\""."",
					""properties"": {
						""kind"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""language"": {
							""type"": ""string""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""RequestGroup_RelatedAction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""A group of related requests that can be used to capture intended activities that have inter-dependencies such as \""give this medication after that one\""."",
					""properties"": {
						""actionId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_actionId"": {
							""$ref"": ""#/definitions/Element""
						},
						""relationship"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_relationship"": {
							""$ref"": ""#/definitions/Element""
						},
						""offsetDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""offsetRange"": {
							""$ref"": ""#/definitions/Range""
						}
					}
				}
			]
		},
		""ResearchStudy"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ResearchStudy""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""protocol"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""draft"", ""in-progress"", ""suspended"", ""stopped"", ""completed"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""focus"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""keyword"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""enrollment"": {
							""description"": ""Reference to a Group that defines the criteria for and quantity of subjects participating in the study.  E.g. \"" 200 female Europeans between the ages of 20 and 45 with early onset diabetes\""."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""sponsor"": {
							""$ref"": ""#/definitions/Reference""
						},
						""principalInvestigator"": {
							""$ref"": ""#/definitions/Reference""
						},
						""site"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""reasonStopped"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""arm"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ResearchStudy_Arm""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ResearchStudy_Arm"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ResearchSubject"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ResearchSubject""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""candidate"", ""enrolled"", ""active"", ""suspended"", ""withdrawn"", ""completed""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""study"": {
							""$ref"": ""#/definitions/Reference""
						},
						""individual"": {
							""$ref"": ""#/definitions/Reference""
						},
						""assignedArm"": {
							""type"": ""string""
						},
						""_assignedArm"": {
							""$ref"": ""#/definitions/Element""
						},
						""actualArm"": {
							""type"": ""string""
						},
						""_actualArm"": {
							""$ref"": ""#/definitions/Element""
						},
						""consent"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""study"", ""individual"", ""resourceType""]
				}
			]
		},
		""RiskAssessment"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""RiskAssessment""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""basedOn"": {
							""$ref"": ""#/definitions/Reference""
						},
						""parent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""condition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""basis"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""prediction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RiskAssessment_Prediction""
							}
						},
						""mitigation"": {
							""type"": ""string""
						},
						""_mitigation"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""RiskAssessment_Prediction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""outcome"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""probabilityDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_probabilityDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""probabilityRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""qualitativeRisk"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""relativeRisk"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_relativeRisk"": {
							""$ref"": ""#/definitions/Element""
						},
						""whenPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""whenRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""rationale"": {
							""type"": ""string""
						},
						""_rationale"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""outcome""]
				}
			]
		},
		""Schedule"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Schedule""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""active"": {
							""type"": ""boolean""
						},
						""_active"": {
							""$ref"": ""#/definitions/Element""
						},
						""serviceCategory"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""serviceType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""actor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""planningHorizon"": {
							""description"": ""The period of time that the slots that are attached to this Schedule resource cover (even if none exist). These  cover the amount of time that an organization's planning horizon; the interval for which they are currently accepting appointments. This does not define a \""template\"" for planning outside these dates."",
							""$ref"": ""#/definitions/Period""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""actor"", ""resourceType""]
				}
			]
		},
		""positiveInteger"": {
			""type"": ""integer"",
			""minimum"": 0
		},
		""positiveIntegerDefault0"": {
			""allOf"": [{
					""$ref"": ""#/definitions/positiveInteger""
				}, {
					""default"": 0
				}
			]
		},
		""simpleTypes"": {
			""enum"": [""array"", ""boolean"", ""integer"", ""null"", ""number"", ""object"", ""string""]
		},
		""stringArray"": {
			""type"": ""array"",
			""items"": {
				""type"": ""string""
			},
			""minItems"": 1,
			""uniqueItems"": true
		},
		""SearchParameter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""SearchParameter""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""base"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_base"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""type"": {
							""enum"": [""number"", ""date"", ""string"", ""token"", ""reference"", ""composite"", ""quantity"", ""uri""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""derivedFrom"": {
							""type"": ""string""
						},
						""_derivedFrom"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						},
						""xpath"": {
							""type"": ""string""
						},
						""_xpath"": {
							""$ref"": ""#/definitions/Element""
						},
						""xpathUsage"": {
							""enum"": [""normal"", ""phonetic"", ""nearby"", ""distance"", ""other""],
							""type"": ""string""
						},
						""_xpathUsage"": {
							""$ref"": ""#/definitions/Element""
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string"",
								""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
							}
						},
						""_target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""comparator"": {
							""enum"": [""eq"", ""ne"", ""gt"", ""lt"", ""ge"", ""le"", ""sa"", ""eb"", ""ap""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_comparator"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""modifier"": {
							""enum"": [""missing"", ""exact"", ""contains"", ""not"", ""text"", ""in"", ""not-in"", ""below"", ""above"", ""type""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_modifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""chain"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_chain"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""component"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/SearchParameter_Component""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""SearchParameter_Component"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""definition"": {
							""$ref"": ""#/definitions/Reference""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""definition""]
				}
			]
		},
		""Sequence"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Sequence""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""type"": {
							""enum"": [""aa"", ""dna"", ""rna""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""coordinateSystem"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_coordinateSystem"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""specimen"": {
							""$ref"": ""#/definitions/Reference""
						},
						""device"": {
							""$ref"": ""#/definitions/Reference""
						},
						""performer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""referenceSeq"": {
							""$ref"": ""#/definitions/Sequence_ReferenceSeq""
						},
						""variant"": {
							""description"": ""The definition of variant here originates from Sequence ontology ([variant_of](http://www.sequenceontology.org/browser/current_svn/term/variant_of)). This element can represent amino acid or nucleic sequence change(including insertion,deletion,SNP,etc.)  It can represent some complex mutation or segment variation with the assist of CIGAR string."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Sequence_Variant""
							}
						},
						""observedSeq"": {
							""type"": ""string""
						},
						""_observedSeq"": {
							""$ref"": ""#/definitions/Element""
						},
						""quality"": {
							""description"": ""An experimental feature attribute that defines the quality of the feature in a quantitative way, such as a phred quality score ([SO:0001686](http://www.sequenceontology.org/browser/current_svn/term/SO:0001686))."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Sequence_Quality""
							}
						},
						""readCoverage"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_readCoverage"": {
							""$ref"": ""#/definitions/Element""
						},
						""repository"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Sequence_Repository""
							}
						},
						""pointer"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Sequence_ReferenceSeq"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""chromosome"": {
							""description"": ""Structural unit composed of a nucleic acid molecule which controls its own replication through the interaction of specific proteins at one or more origins of replication ([SO:0000340](http://www.sequenceontology.org/browser/current_svn/term/SO:0000340))."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""genomeBuild"": {
							""type"": ""string""
						},
						""_genomeBuild"": {
							""$ref"": ""#/definitions/Element""
						},
						""referenceSeqId"": {
							""description"": ""Reference identifier of reference sequence submitted to NCBI. It must match the type in the Sequence.type field. For example, the prefix, “NG_” identifies reference sequence for genes, “NM_” for messenger RNA transcripts, and “NP_” for amino acid sequences."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""referenceSeqPointer"": {
							""$ref"": ""#/definitions/Reference""
						},
						""referenceSeqString"": {
							""description"": ""A string like \""ACGT\""."",
							""type"": ""string""
						},
						""_referenceSeqString"": {
							""$ref"": ""#/definitions/Element""
						},
						""strand"": {
							""description"": ""Directionality of DNA sequence. Available values are \""1\"" for the plus strand (5' to 3')/Watson/Sense/positive  and \""-1\"" for the minus strand(3' to 5')/Crick/Antisense/negative."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_strand"": {
							""$ref"": ""#/definitions/Element""
						},
						""windowStart"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_windowStart"": {
							""$ref"": ""#/definitions/Element""
						},
						""windowEnd"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_windowEnd"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Sequence_Variant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""start"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""observedAllele"": {
							""description"": ""An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)).  Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the observed  sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end."",
							""type"": ""string""
						},
						""_observedAllele"": {
							""$ref"": ""#/definitions/Element""
						},
						""referenceAllele"": {
							""description"": ""An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)). Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the reference sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end."",
							""type"": ""string""
						},
						""_referenceAllele"": {
							""$ref"": ""#/definitions/Element""
						},
						""cigar"": {
							""description"": ""Extended CIGAR string for aligning the sequence with reference bases. See detailed documentation [here](http://support.illumina.com/help/SequencingAnalysisWorkflow/Content/Vault/Informatics/Sequencing_Analysis/CASAVA/swSEQ_mCA_ExtendedCIGARFormat.htm)."",
							""type"": ""string""
						},
						""_cigar"": {
							""$ref"": ""#/definitions/Element""
						},
						""variantPointer"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""Sequence_Quality"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""indel"", ""snp"", ""unknown""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""standardSequence"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""start"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""score"": {
							""description"": ""The score of an experimentally derived feature such as a p-value ([SO:0001685](http://www.sequenceontology.org/browser/current_svn/term/SO:0001685))."",
							""$ref"": ""#/definitions/Quantity""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""truthTP"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_truthTP"": {
							""$ref"": ""#/definitions/Element""
						},
						""queryTP"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_queryTP"": {
							""$ref"": ""#/definitions/Element""
						},
						""truthFN"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_truthFN"": {
							""$ref"": ""#/definitions/Element""
						},
						""queryFP"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_queryFP"": {
							""$ref"": ""#/definitions/Element""
						},
						""gtFP"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_gtFP"": {
							""$ref"": ""#/definitions/Element""
						},
						""precision"": {
							""description"": ""QUERY.TP / (QUERY.TP + QUERY.FP)."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_precision"": {
							""$ref"": ""#/definitions/Element""
						},
						""recall"": {
							""description"": ""TRUTH.TP / (TRUTH.TP + TRUTH.FN)."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_recall"": {
							""$ref"": ""#/definitions/Element""
						},
						""fScore"": {
							""description"": ""Harmonic mean of Recall and Precision, computed as: 2 * precision * recall / (precision + recall)."",
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_fScore"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""Sequence_Repository"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""directlink"", ""openapi"", ""login"", ""oauth"", ""other""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""datasetId"": {
							""type"": ""string""
						},
						""_datasetId"": {
							""$ref"": ""#/definitions/Element""
						},
						""variantsetId"": {
							""type"": ""string""
						},
						""_variantsetId"": {
							""$ref"": ""#/definitions/Element""
						},
						""readsetId"": {
							""type"": ""string""
						},
						""_readsetId"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ServiceDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ServiceDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""usage"": {
							""type"": ""string""
						},
						""_usage"": {
							""$ref"": ""#/definitions/Element""
						},
						""approvalDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_approvalDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastReviewDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lastReviewDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""effectivePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""topic"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""contributor"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Contributor""
							}
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""relatedArtifact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/RelatedArtifact""
							}
						},
						""trigger"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TriggerDefinition""
							}
						},
						""dataRequirement"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/DataRequirement""
							}
						},
						""operationDefinition"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Slot"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Slot""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""serviceCategory"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""serviceType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""specialty"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""appointmentType"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""schedule"": {
							""$ref"": ""#/definitions/Reference""
						},
						""status"": {
							""enum"": [""busy"", ""free"", ""busy-unavailable"", ""busy-tentative"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""start"": {
							""type"": ""string""
						},
						""_start"": {
							""$ref"": ""#/definitions/Element""
						},
						""end"": {
							""type"": ""string""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""overbooked"": {
							""type"": ""boolean""
						},
						""_overbooked"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""schedule"", ""resourceType""]
				}
			]
		},
		""Specimen"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Specimen""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""accessionIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""available"", ""unavailable"", ""unsatisfactory"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""subject"": {
							""$ref"": ""#/definitions/Reference""
						},
						""receivedTime"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_receivedTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""parent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""request"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""collection"": {
							""$ref"": ""#/definitions/Specimen_Collection""
						},
						""processing"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Specimen_Processing""
							}
						},
						""container"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Specimen_Container""
							}
						},
						""note"": {
							""description"": ""To communicate any details or issues about the specimen or during the specimen collection. (for example: broken vial, sent with patient, frozen)."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					},
					""required"": [""subject"", ""resourceType""]
				}
			]
		},
		""Specimen_Collection"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""collector"": {
							""$ref"": ""#/definitions/Reference""
						},
						""collectedDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_collectedDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""collectedPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""method"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""bodySite"": {
							""$ref"": ""#/definitions/CodeableConcept""
						}
					}
				}
			]
		},
		""Specimen_Processing"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""procedure"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""additive"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""timeDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_timeDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""timePeriod"": {
							""$ref"": ""#/definitions/Period""
						}
					}
				}
			]
		},
		""Specimen_Container"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""capacity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""specimenQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""additiveCodeableConcept"": {
							""description"": ""Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""additiveReference"": {
							""description"": ""Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA."",
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""StructureDefinition"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""StructureDefinition""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""keyword"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						},
						""fhirVersion"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_fhirVersion"": {
							""$ref"": ""#/definitions/Element""
						},
						""mapping"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureDefinition_Mapping""
							}
						},
						""kind"": {
							""enum"": [""primitive-type"", ""complex-type"", ""resource"", ""logical""],
							""type"": ""string""
						},
						""_kind"": {
							""$ref"": ""#/definitions/Element""
						},
						""abstract"": {
							""type"": ""boolean""
						},
						""_abstract"": {
							""$ref"": ""#/definitions/Element""
						},
						""contextType"": {
							""enum"": [""resource"", ""datatype"", ""extension""],
							""type"": ""string""
						},
						""_contextType"": {
							""$ref"": ""#/definitions/Element""
						},
						""context"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_context"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""contextInvariant"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_contextInvariant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""type"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""baseDefinition"": {
							""type"": ""string""
						},
						""_baseDefinition"": {
							""$ref"": ""#/definitions/Element""
						},
						""derivation"": {
							""enum"": [""specialization"", ""constraint""],
							""type"": ""string""
						},
						""_derivation"": {
							""$ref"": ""#/definitions/Element""
						},
						""snapshot"": {
							""$ref"": ""#/definitions/StructureDefinition_Snapshot""
						},
						""differential"": {
							""$ref"": ""#/definitions/StructureDefinition_Differential""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""StructureDefinition_Mapping"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identity"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_identity"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""comment"": {
							""type"": ""string""
						},
						""_comment"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""StructureDefinition_Snapshot"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""element"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition""
							}
						}
					},
					""required"": [""element""]
				}
			]
		},
		""StructureDefinition_Differential"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""element"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ElementDefinition""
							}
						}
					},
					""required"": [""element""]
				}
			]
		},
		""StructureMap"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""StructureMap""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""structure"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Structure""
							}
						},
						""import"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_import"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""group"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Group""
							}
						}
					},
					""required"": [""resourceType"", ""group""]
				}
			]
		},
		""StructureMap_Structure"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""source"", ""queried"", ""target"", ""produced""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""alias"": {
							""type"": ""string""
						},
						""_alias"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""StructureMap_Group"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""extends"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_extends"": {
							""$ref"": ""#/definitions/Element""
						},
						""typeMode"": {
							""enum"": [""none"", ""types"", ""type-and-types""],
							""type"": ""string""
						},
						""_typeMode"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						},
						""input"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Input""
							}
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Rule""
							}
						}
					},
					""required"": [""input"", ""rule""]
				}
			]
		},
		""StructureMap_Input"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""mode"": {
							""enum"": [""source"", ""target""],
							""type"": ""string""
						},
						""_mode"": {
							""$ref"": ""#/definitions/Element""
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""StructureMap_Rule"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""source"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Source""
							}
						},
						""target"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Target""
							}
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Rule""
							}
						},
						""dependent"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Dependent""
							}
						},
						""documentation"": {
							""type"": ""string""
						},
						""_documentation"": {
							""$ref"": ""#/definitions/Element""
						}
					},
					""required"": [""source""]
				}
			]
		},
		""StructureMap_Source"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""context"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_context"": {
							""$ref"": ""#/definitions/Element""
						},
						""min"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_min"": {
							""$ref"": ""#/definitions/Element""
						},
						""max"": {
							""description"": ""Specified maximum cardinality for the element - a number or a \""*\"". This is optional; if present, it acts an implicit check on the input content (* just serves as documentation; it's the default value)."",
							""type"": ""string""
						},
						""_max"": {
							""$ref"": ""#/definitions/Element""
						},
						""type"": {
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueBoolean"": {
							""type"": ""boolean""
						},
						""_defaultValueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_defaultValueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_defaultValueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueBase64Binary"": {
							""type"": ""string""
						},
						""_defaultValueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueInstant"": {
							""type"": ""string""
						},
						""_defaultValueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueString"": {
							""type"": ""string""
						},
						""_defaultValueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUri"": {
							""type"": ""string""
						},
						""_defaultValueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_defaultValueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_defaultValueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_defaultValueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_defaultValueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_defaultValueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_defaultValueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_defaultValueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_defaultValueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_defaultValuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueMarkdown"": {
							""type"": ""string""
						},
						""_defaultValueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""defaultValueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""defaultValueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""defaultValueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""defaultValueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""defaultValueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""defaultValueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""defaultValueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""defaultValueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""defaultValueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""defaultValueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""defaultValueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""defaultValueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""defaultValueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""defaultValueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""defaultValueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""defaultValuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""defaultValueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""defaultValueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""defaultValueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""defaultValueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""defaultValueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""defaultValueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""defaultValueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""defaultValueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""defaultValueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""defaultValueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""defaultValueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""defaultValueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""defaultValueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""defaultValueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""defaultValueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""defaultValueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""defaultValueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""defaultValueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						},
						""element"": {
							""type"": ""string""
						},
						""_element"": {
							""$ref"": ""#/definitions/Element""
						},
						""listMode"": {
							""enum"": [""first"", ""not_first"", ""last"", ""not_last"", ""only_one""],
							""type"": ""string""
						},
						""_listMode"": {
							""$ref"": ""#/definitions/Element""
						},
						""variable"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_variable"": {
							""$ref"": ""#/definitions/Element""
						},
						""condition"": {
							""type"": ""string""
						},
						""_condition"": {
							""$ref"": ""#/definitions/Element""
						},
						""check"": {
							""type"": ""string""
						},
						""_check"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""StructureMap_Target"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""context"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_context"": {
							""$ref"": ""#/definitions/Element""
						},
						""contextType"": {
							""enum"": [""type"", ""variable""],
							""type"": ""string""
						},
						""_contextType"": {
							""$ref"": ""#/definitions/Element""
						},
						""element"": {
							""type"": ""string""
						},
						""_element"": {
							""$ref"": ""#/definitions/Element""
						},
						""variable"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_variable"": {
							""$ref"": ""#/definitions/Element""
						},
						""listMode"": {
							""enum"": [""first"", ""share"", ""last"", ""collate""],
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_listMode"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""listRuleId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_listRuleId"": {
							""$ref"": ""#/definitions/Element""
						},
						""transform"": {
							""enum"": [""create"", ""copy"", ""truncate"", ""escape"", ""cast"", ""append"", ""translate"", ""reference"", ""dateOp"", ""uuid"", ""pointer"", ""evaluate"", ""cc"", ""c"", ""qty"", ""id"", ""cp""],
							""type"": ""string""
						},
						""_transform"": {
							""$ref"": ""#/definitions/Element""
						},
						""parameter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/StructureMap_Parameter""
							}
						}
					}
				}
			]
		},
		""StructureMap_Parameter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""StructureMap_Dependent"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""variable"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_variable"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""Subscription"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""description"": ""The subscription resource is used to define a push based subscription from a server to another system. Once a subscription is registered with the server, the server checks every resource that is created or updated, and if the resource matches the given criteria, it sends a message on the defined \""channel\"" so that another system is able to take an appropriate action."",
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Subscription""]
						},
						""status"": {
							""enum"": [""requested"", ""active"", ""error"", ""off""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactPoint""
							}
						},
						""end"": {
							""type"": ""string""
						},
						""_end"": {
							""$ref"": ""#/definitions/Element""
						},
						""reason"": {
							""type"": ""string""
						},
						""_reason"": {
							""$ref"": ""#/definitions/Element""
						},
						""criteria"": {
							""type"": ""string""
						},
						""_criteria"": {
							""$ref"": ""#/definitions/Element""
						},
						""error"": {
							""type"": ""string""
						},
						""_error"": {
							""$ref"": ""#/definitions/Element""
						},
						""channel"": {
							""$ref"": ""#/definitions/Subscription_Channel""
						},
						""tag"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Coding""
							}
						}
					},
					""required"": [""channel"", ""resourceType""]
				}
			]
		},
		""Subscription_Channel"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""description"": ""The subscription resource is used to define a push based subscription from a server to another system. Once a subscription is registered with the server, the server checks every resource that is created or updated, and if the resource matches the given criteria, it sends a message on the defined \""channel\"" so that another system is able to take an appropriate action."",
					""properties"": {
						""type"": {
							""enum"": [""rest-hook"", ""websocket"", ""email"", ""sms"", ""message""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""endpoint"": {
							""type"": ""string""
						},
						""_endpoint"": {
							""$ref"": ""#/definitions/Element""
						},
						""payload"": {
							""description"": ""The mime type to send the payload in - either application/fhir+xml, or application/fhir+json. If the payload is not present, then there is no payload in the notification, just a notification."",
							""type"": ""string""
						},
						""_payload"": {
							""$ref"": ""#/definitions/Element""
						},
						""header"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_header"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""Substance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Substance""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""enum"": [""active"", ""inactive"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""instance"": {
							""description"": ""Substance may be used to describe a kind of substance, or a specific package/container of the substance: an instance."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Substance_Instance""
							}
						},
						""ingredient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Substance_Ingredient""
							}
						}
					},
					""required"": [""code"", ""resourceType""]
				}
			]
		},
		""Substance_Instance"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""expiry"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_expiry"": {
							""$ref"": ""#/definitions/Element""
						},
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						}
					}
				}
			]
		},
		""Substance_Ingredient"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""quantity"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""substanceCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""substanceReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""SupplyDelivery"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""SupplyDelivery""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""basedOn"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""in-progress"", ""completed"", ""abandoned"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""type"": {
							""description"": ""Indicates the type of dispensing event that is performed. Examples include: Trial Fill, Completion of Trial, Partial Fill, Emergency Fill, Samples, etc."",
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""suppliedItem"": {
							""$ref"": ""#/definitions/SupplyDelivery_SuppliedItem""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""occurrenceTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""supplier"": {
							""$ref"": ""#/definitions/Reference""
						},
						""destination"": {
							""$ref"": ""#/definitions/Reference""
						},
						""receiver"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""SupplyDelivery_SuppliedItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""itemCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""itemReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""SupplyRequest"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""SupplyRequest""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""suspended"", ""cancelled"", ""completed"", ""entered-in-error"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""category"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""orderedItem"": {
							""$ref"": ""#/definitions/SupplyRequest_OrderedItem""
						},
						""occurrenceDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_occurrenceDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""occurrencePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""occurrenceTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/SupplyRequest_Requester""
						},
						""supplier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""reasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""deliverFrom"": {
							""$ref"": ""#/definitions/Reference""
						},
						""deliverTo"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""SupplyRequest_OrderedItem"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""quantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""itemCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""itemReference"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""quantity""]
				}
			]
		},
		""SupplyRequest_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""Task"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""Task""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""definitionUri"": {
							""type"": ""string""
						},
						""_definitionUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""definitionReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""basedOn"": {
							""description"": ""BasedOn refers to a higher-level authorization that triggered the creation of the task.  It references a \""request\"" resource such as a ProcedureRequest, MedicationRequest, ProcedureRequest, CarePlan, etc. which is distinct from the \""request\"" resource the task is seeking to fulfil.  This latter resource is referenced by FocusOn.  For example, based on a ProcedureRequest (= BasedOn), a task is created to fulfil a procedureRequest ( = FocusOn ) to collect a specimen from a patient."",
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""groupIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""partOf"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""status"": {
							""enum"": [""draft"", ""requested"", ""received"", ""accepted"", ""rejected"", ""ready"", ""cancelled"", ""in-progress"", ""on-hold"", ""failed"", ""completed"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""statusReason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""businessStatus"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""intent"": {
							""description"": ""Indicates the \""level\"" of actionability associated with the Task.  I.e. Is this a proposed task, a planned task, an actionable task, etc."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_intent"": {
							""$ref"": ""#/definitions/Element""
						},
						""priority"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_priority"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""focus"": {
							""$ref"": ""#/definitions/Reference""
						},
						""for"": {
							""$ref"": ""#/definitions/Reference""
						},
						""context"": {
							""$ref"": ""#/definitions/Reference""
						},
						""executionPeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""authoredOn"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_authoredOn"": {
							""$ref"": ""#/definitions/Element""
						},
						""lastModified"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_lastModified"": {
							""$ref"": ""#/definitions/Element""
						},
						""requester"": {
							""$ref"": ""#/definitions/Task_Requester""
						},
						""performerType"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""owner"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reason"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						},
						""relevantHistory"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""restriction"": {
							""$ref"": ""#/definitions/Task_Restriction""
						},
						""input"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Task_Input""
							}
						},
						""output"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Task_Output""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""Task_Requester"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""agent"": {
							""$ref"": ""#/definitions/Reference""
						},
						""onBehalfOf"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""agent""]
				}
			]
		},
		""Task_Restriction"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""repetitions"": {
							""type"": ""number"",
							""pattern"": ""[1-9][0-9]*""
						},
						""_repetitions"": {
							""$ref"": ""#/definitions/Element""
						},
						""period"": {
							""$ref"": ""#/definitions/Period""
						},
						""recipient"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						}
					}
				}
			]
		},
		""Task_Input"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBase64Binary"": {
							""type"": ""string""
						},
						""_valueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInstant"": {
							""type"": ""string""
						},
						""_valueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_valueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_valueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_valueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_valuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueMarkdown"": {
							""type"": ""string""
						},
						""_valueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""valueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""valueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""valueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""valueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""valueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""valueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""valueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""valueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""valueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""valueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""valueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""valueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""valueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""valueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""valueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""valueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""valueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""valueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""valueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""valueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""valueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""Task_Output"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBase64Binary"": {
							""type"": ""string""
						},
						""_valueBase64Binary"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInstant"": {
							""type"": ""string""
						},
						""_valueInstant"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDate"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?"",
							""type"": ""string""
						},
						""_valueDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDateTime"": {
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?"",
							""type"": ""string""
						},
						""_valueDateTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueTime"": {
							""pattern"": ""([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?"",
							""type"": ""string""
						},
						""_valueTime"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueOid"": {
							""pattern"": ""urn:oid:(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))*"",
							""type"": ""string""
						},
						""_valueOid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUuid"": {
							""pattern"": ""urn:uuid:[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"",
							""type"": ""string""
						},
						""_valueUuid"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueId"": {
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}"",
							""type"": ""string""
						},
						""_valueId"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUnsignedInt"": {
							""pattern"": ""[0]|([1-9][0-9]*)"",
							""type"": ""number""
						},
						""_valueUnsignedInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valuePositiveInt"": {
							""pattern"": ""[1-9][0-9]*"",
							""type"": ""number""
						},
						""_valuePositiveInt"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueMarkdown"": {
							""type"": ""string""
						},
						""_valueMarkdown"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueElement"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueExtension"": {
							""$ref"": ""#/definitions/Extension""
						},
						""valueBackboneElement"": {
							""$ref"": ""#/definitions/BackboneElement""
						},
						""valueNarrative"": {
							""$ref"": ""#/definitions/Narrative""
						},
						""valueAnnotation"": {
							""$ref"": ""#/definitions/Annotation""
						},
						""valueAttachment"": {
							""$ref"": ""#/definitions/Attachment""
						},
						""valueIdentifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""valueCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""valueCoding"": {
							""$ref"": ""#/definitions/Coding""
						},
						""valueQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDuration"": {
							""$ref"": ""#/definitions/Duration""
						},
						""valueSimpleQuantity"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""valueDistance"": {
							""$ref"": ""#/definitions/Distance""
						},
						""valueCount"": {
							""$ref"": ""#/definitions/Count""
						},
						""valueMoney"": {
							""$ref"": ""#/definitions/Money""
						},
						""valueAge"": {
							""$ref"": ""#/definitions/Age""
						},
						""valueRange"": {
							""$ref"": ""#/definitions/Range""
						},
						""valuePeriod"": {
							""$ref"": ""#/definitions/Period""
						},
						""valueRatio"": {
							""$ref"": ""#/definitions/Ratio""
						},
						""valueReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""valueSampledData"": {
							""$ref"": ""#/definitions/SampledData""
						},
						""valueSignature"": {
							""$ref"": ""#/definitions/Signature""
						},
						""valueHumanName"": {
							""$ref"": ""#/definitions/HumanName""
						},
						""valueAddress"": {
							""$ref"": ""#/definitions/Address""
						},
						""valueContactPoint"": {
							""$ref"": ""#/definitions/ContactPoint""
						},
						""valueTiming"": {
							""$ref"": ""#/definitions/Timing""
						},
						""valueMeta"": {
							""$ref"": ""#/definitions/Meta""
						},
						""valueElementDefinition"": {
							""$ref"": ""#/definitions/ElementDefinition""
						},
						""valueContactDetail"": {
							""$ref"": ""#/definitions/ContactDetail""
						},
						""valueContributor"": {
							""$ref"": ""#/definitions/Contributor""
						},
						""valueDosage"": {
							""$ref"": ""#/definitions/Dosage""
						},
						""valueRelatedArtifact"": {
							""$ref"": ""#/definitions/RelatedArtifact""
						},
						""valueUsageContext"": {
							""$ref"": ""#/definitions/UsageContext""
						},
						""valueDataRequirement"": {
							""$ref"": ""#/definitions/DataRequirement""
						},
						""valueParameterDefinition"": {
							""$ref"": ""#/definitions/ParameterDefinition""
						},
						""valueTriggerDefinition"": {
							""$ref"": ""#/definitions/TriggerDefinition""
						}
					},
					""required"": [""type""]
				}
			]
		},
		""TestReport"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""TestReport""]
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""completed"", ""in-progress"", ""waiting"", ""stopped"", ""entered-in-error""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""testScript"": {
							""description"": ""Ideally this is an absolute URL that is used to identify the version-specific TestScript that was executed, matching the `TestScript.url`."",
							""$ref"": ""#/definitions/Reference""
						},
						""result"": {
							""enum"": [""pass"", ""fail"", ""pending""],
							""type"": ""string""
						},
						""_result"": {
							""$ref"": ""#/definitions/Element""
						},
						""score"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_score"": {
							""$ref"": ""#/definitions/Element""
						},
						""tester"": {
							""type"": ""string""
						},
						""_tester"": {
							""$ref"": ""#/definitions/Element""
						},
						""issued"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_issued"": {
							""$ref"": ""#/definitions/Element""
						},
						""participant"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestReport_Participant""
							}
						},
						""setup"": {
							""$ref"": ""#/definitions/TestReport_Setup""
						},
						""test"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestReport_Test""
							}
						},
						""teardown"": {
							""$ref"": ""#/definitions/TestReport_Teardown""
						}
					},
					""required"": [""testScript"", ""resourceType""]
				}
			]
		},
		""TestReport_Participant"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""enum"": [""test-engine"", ""client"", ""server""],
							""type"": ""string""
						},
						""_type"": {
							""$ref"": ""#/definitions/Element""
						},
						""uri"": {
							""type"": ""string""
						},
						""_uri"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestReport_Setup"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestReport_Action""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestReport_Action"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestReport_Operation""
						},
						""assert"": {
							""$ref"": ""#/definitions/TestReport_Assert""
						}
					}
				}
			]
		},
		""TestReport_Operation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""result"": {
							""enum"": [""pass"", ""skip"", ""fail"", ""warning"", ""error""],
							""type"": ""string""
						},
						""_result"": {
							""$ref"": ""#/definitions/Element""
						},
						""message"": {
							""type"": ""string""
						},
						""_message"": {
							""$ref"": ""#/definitions/Element""
						},
						""detail"": {
							""type"": ""string""
						},
						""_detail"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestReport_Assert"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""result"": {
							""enum"": [""pass"", ""skip"", ""fail"", ""warning"", ""error""],
							""type"": ""string""
						},
						""_result"": {
							""$ref"": ""#/definitions/Element""
						},
						""message"": {
							""type"": ""string""
						},
						""_message"": {
							""$ref"": ""#/definitions/Element""
						},
						""detail"": {
							""type"": ""string""
						},
						""_detail"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestReport_Test"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestReport_Action1""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestReport_Action1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestReport_Operation""
						},
						""assert"": {
							""$ref"": ""#/definitions/TestReport_Assert""
						}
					}
				}
			]
		},
		""TestReport_Teardown"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestReport_Action2""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestReport_Action2"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestReport_Operation""
						}
					},
					""required"": [""operation""]
				}
			]
		},
		""TestScript"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""TestScript""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""$ref"": ""#/definitions/Identifier""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""origin"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Origin""
							}
						},
						""destination"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Destination""
							}
						},
						""metadata"": {
							""$ref"": ""#/definitions/TestScript_Metadata""
						},
						""fixture"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Fixture""
							}
						},
						""profile"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Reference""
							}
						},
						""variable"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Variable""
							}
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Rule""
							}
						},
						""ruleset"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Ruleset""
							}
						},
						""setup"": {
							""$ref"": ""#/definitions/TestScript_Setup""
						},
						""test"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Test""
							}
						},
						""teardown"": {
							""$ref"": ""#/definitions/TestScript_Teardown""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""TestScript_Origin"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""index"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_index"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Coding""
						}
					},
					""required"": [""profile""]
				}
			]
		},
		""TestScript_Destination"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""index"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_index"": {
							""$ref"": ""#/definitions/Element""
						},
						""profile"": {
							""$ref"": ""#/definitions/Coding""
						}
					},
					""required"": [""profile""]
				}
			]
		},
		""TestScript_Metadata"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Link""
							}
						},
						""capability"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Capability""
							}
						}
					},
					""required"": [""capability""]
				}
			]
		},
		""TestScript_Link"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Capability"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""required"": {
							""type"": ""boolean""
						},
						""_required"": {
							""$ref"": ""#/definitions/Element""
						},
						""validated"": {
							""type"": ""boolean""
						},
						""_validated"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""origin"": {
							""type"": ""array"",
							""items"": {
								""type"": ""number"",
								""pattern"": ""-?([0]|([1-9][0-9]*))""
							}
						},
						""_origin"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""destination"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_destination"": {
							""$ref"": ""#/definitions/Element""
						},
						""link"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_link"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						},
						""capabilities"": {
							""$ref"": ""#/definitions/Reference""
						}
					},
					""required"": [""capabilities""]
				}
			]
		},
		""TestScript_Fixture"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""autocreate"": {
							""type"": ""boolean""
						},
						""_autocreate"": {
							""$ref"": ""#/definitions/Element""
						},
						""autodelete"": {
							""type"": ""boolean""
						},
						""_autodelete"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						}
					}
				}
			]
		},
		""TestScript_Variable"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""defaultValue"": {
							""type"": ""string""
						},
						""_defaultValue"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						},
						""headerField"": {
							""type"": ""string""
						},
						""_headerField"": {
							""$ref"": ""#/definitions/Element""
						},
						""hint"": {
							""type"": ""string""
						},
						""_hint"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_sourceId"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Rule"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						},
						""param"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Param""
							}
						}
					},
					""required"": [""resource""]
				}
			]
		},
		""TestScript_Param"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Ruleset"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""resource"": {
							""$ref"": ""#/definitions/Reference""
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Rule1""
							}
						}
					},
					""required"": [""resource"", ""rule""]
				}
			]
		},
		""TestScript_Rule1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""ruleId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_ruleId"": {
							""$ref"": ""#/definitions/Element""
						},
						""param"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Param1""
							}
						}
					}
				}
			]
		},
		""TestScript_Param1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Setup"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Action""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestScript_Action"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestScript_Operation""
						},
						""assert"": {
							""$ref"": ""#/definitions/TestScript_Assert""
						}
					}
				}
			]
		},
		""TestScript_Operation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""type"": {
							""$ref"": ""#/definitions/Coding""
						},
						""resource"": {
							""description"": ""The type of the resource.  See http://build.fhir.org/resourcelist.html."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_resource"": {
							""$ref"": ""#/definitions/Element""
						},
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""accept"": {
							""enum"": [""xml"", ""json"", ""ttl"", ""none""],
							""type"": ""string""
						},
						""_accept"": {
							""$ref"": ""#/definitions/Element""
						},
						""contentType"": {
							""enum"": [""xml"", ""json"", ""ttl"", ""none""],
							""type"": ""string""
						},
						""_contentType"": {
							""$ref"": ""#/definitions/Element""
						},
						""destination"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_destination"": {
							""$ref"": ""#/definitions/Element""
						},
						""encodeRequestUrl"": {
							""type"": ""boolean""
						},
						""_encodeRequestUrl"": {
							""$ref"": ""#/definitions/Element""
						},
						""origin"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_origin"": {
							""$ref"": ""#/definitions/Element""
						},
						""params"": {
							""type"": ""string""
						},
						""_params"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestHeader"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_RequestHeader""
							}
						},
						""requestId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_requestId"": {
							""$ref"": ""#/definitions/Element""
						},
						""responseId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_responseId"": {
							""$ref"": ""#/definitions/Element""
						},
						""sourceId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_sourceId"": {
							""$ref"": ""#/definitions/Element""
						},
						""targetId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_targetId"": {
							""$ref"": ""#/definitions/Element""
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_RequestHeader"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""field"": {
							""description"": ""The HTTP header field e.g. \""Accept\""."",
							""type"": ""string""
						},
						""_field"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""description"": ""The value of the header e.g. \""application/fhir+xml\""."",
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Assert"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""label"": {
							""type"": ""string""
						},
						""_label"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""direction"": {
							""enum"": [""response"", ""request""],
							""type"": ""string""
						},
						""_direction"": {
							""$ref"": ""#/definitions/Element""
						},
						""compareToSourceId"": {
							""description"": ""Id of the source fixture used as the contents to be evaluated by either the \""source/expression\"" or \""sourceId/path\"" definition."",
							""type"": ""string""
						},
						""_compareToSourceId"": {
							""$ref"": ""#/definitions/Element""
						},
						""compareToSourceExpression"": {
							""type"": ""string""
						},
						""_compareToSourceExpression"": {
							""$ref"": ""#/definitions/Element""
						},
						""compareToSourcePath"": {
							""type"": ""string""
						},
						""_compareToSourcePath"": {
							""$ref"": ""#/definitions/Element""
						},
						""contentType"": {
							""enum"": [""xml"", ""json"", ""ttl"", ""none""],
							""type"": ""string""
						},
						""_contentType"": {
							""$ref"": ""#/definitions/Element""
						},
						""expression"": {
							""type"": ""string""
						},
						""_expression"": {
							""$ref"": ""#/definitions/Element""
						},
						""headerField"": {
							""type"": ""string""
						},
						""_headerField"": {
							""$ref"": ""#/definitions/Element""
						},
						""minimumId"": {
							""type"": ""string""
						},
						""_minimumId"": {
							""$ref"": ""#/definitions/Element""
						},
						""navigationLinks"": {
							""type"": ""boolean""
						},
						""_navigationLinks"": {
							""$ref"": ""#/definitions/Element""
						},
						""operator"": {
							""enum"": [""equals"", ""notEquals"", ""in"", ""notIn"", ""greaterThan"", ""lessThan"", ""empty"", ""notEmpty"", ""contains"", ""notContains"", ""eval""],
							""type"": ""string""
						},
						""_operator"": {
							""$ref"": ""#/definitions/Element""
						},
						""path"": {
							""type"": ""string""
						},
						""_path"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestMethod"": {
							""enum"": [""delete"", ""get"", ""options"", ""patch"", ""post"", ""put""],
							""type"": ""string""
						},
						""_requestMethod"": {
							""$ref"": ""#/definitions/Element""
						},
						""requestURL"": {
							""type"": ""string""
						},
						""_requestURL"": {
							""$ref"": ""#/definitions/Element""
						},
						""resource"": {
							""description"": ""The type of the resource.  See http://build.fhir.org/resourcelist.html."",
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_resource"": {
							""$ref"": ""#/definitions/Element""
						},
						""response"": {
							""enum"": [""okay"", ""created"", ""noContent"", ""notModified"", ""bad"", ""forbidden"", ""notFound"", ""methodNotAllowed"", ""conflict"", ""gone"", ""preconditionFailed"", ""unprocessable""],
							""type"": ""string""
						},
						""_response"": {
							""$ref"": ""#/definitions/Element""
						},
						""responseCode"": {
							""type"": ""string""
						},
						""_responseCode"": {
							""$ref"": ""#/definitions/Element""
						},
						""rule"": {
							""$ref"": ""#/definitions/TestScript_Rule2""
						},
						""ruleset"": {
							""$ref"": ""#/definitions/TestScript_Ruleset1""
						},
						""sourceId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_sourceId"": {
							""$ref"": ""#/definitions/Element""
						},
						""validateProfileId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_validateProfileId"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						},
						""warningOnly"": {
							""type"": ""boolean""
						},
						""_warningOnly"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Rule2"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""ruleId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_ruleId"": {
							""$ref"": ""#/definitions/Element""
						},
						""param"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Param2""
							}
						}
					}
				}
			]
		},
		""TestScript_Param2"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Ruleset1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""rulesetId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_rulesetId"": {
							""$ref"": ""#/definitions/Element""
						},
						""rule"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Rule3""
							}
						}
					}
				}
			]
		},
		""TestScript_Rule3"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""ruleId"": {
							""type"": ""string"",
							""pattern"": ""[A-Za-z0-9\\-\\.]{1,64}""
						},
						""_ruleId"": {
							""$ref"": ""#/definitions/Element""
						},
						""param"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Param3""
							}
						}
					}
				}
			]
		},
		""TestScript_Param3"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""TestScript_Test"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Action1""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestScript_Action1"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestScript_Operation""
						},
						""assert"": {
							""$ref"": ""#/definitions/TestScript_Assert""
						}
					}
				}
			]
		},
		""TestScript_Teardown"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""action"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/TestScript_Action2""
							}
						}
					},
					""required"": [""action""]
				}
			]
		},
		""TestScript_Action2"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""operation"": {
							""$ref"": ""#/definitions/TestScript_Operation""
						}
					},
					""required"": [""operation""]
				}
			]
		},
		""ValueSet"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""ValueSet""]
						},
						""url"": {
							""type"": ""string""
						},
						""_url"": {
							""$ref"": ""#/definitions/Element""
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""title"": {
							""type"": ""string""
						},
						""_title"": {
							""$ref"": ""#/definitions/Element""
						},
						""status"": {
							""enum"": [""draft"", ""active"", ""retired"", ""unknown""],
							""type"": ""string""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""experimental"": {
							""type"": ""boolean""
						},
						""_experimental"": {
							""$ref"": ""#/definitions/Element""
						},
						""date"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_date"": {
							""$ref"": ""#/definitions/Element""
						},
						""publisher"": {
							""type"": ""string""
						},
						""_publisher"": {
							""$ref"": ""#/definitions/Element""
						},
						""contact"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ContactDetail""
							}
						},
						""description"": {
							""type"": ""string""
						},
						""_description"": {
							""$ref"": ""#/definitions/Element""
						},
						""useContext"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/UsageContext""
							}
						},
						""jurisdiction"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/CodeableConcept""
							}
						},
						""immutable"": {
							""description"": ""If this is set to 'true', then no new versions of the content logical definition can be created.  Note: Other metadata might still change."",
							""type"": ""boolean""
						},
						""_immutable"": {
							""$ref"": ""#/definitions/Element""
						},
						""purpose"": {
							""type"": ""string""
						},
						""_purpose"": {
							""$ref"": ""#/definitions/Element""
						},
						""copyright"": {
							""type"": ""string""
						},
						""_copyright"": {
							""$ref"": ""#/definitions/Element""
						},
						""extensible"": {
							""type"": ""boolean""
						},
						""_extensible"": {
							""$ref"": ""#/definitions/Element""
						},
						""compose"": {
							""description"": ""A set of criteria that define the content logical definition of the value set by including or excluding codes from outside this value set. This I also known as the \""Content Logical Definition\"" (CLD)."",
							""$ref"": ""#/definitions/ValueSet_Compose""
						},
						""expansion"": {
							""description"": ""A value set can also be \""expanded\"", where the value set is turned into a simple collection of enumerated codes. This element holds the expansion, if it has been performed."",
							""$ref"": ""#/definitions/ValueSet_Expansion""
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""ValueSet_Compose"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""lockedDate"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?""
						},
						""_lockedDate"": {
							""$ref"": ""#/definitions/Element""
						},
						""inactive"": {
							""description"": ""Whether inactive codes - codes that are not approved for current use - are in the value set. If inactive = true, inactive codes are to be included in the expansion, if inactive = false, the inactive codes will not be included in the expansion. If absent, the behavior is determined by the implementation, or by the applicable ExpansionProfile (but generally, inactive codes would be expected to be included)."",
							""type"": ""boolean""
						},
						""_inactive"": {
							""$ref"": ""#/definitions/Element""
						},
						""include"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Include""
							}
						},
						""exclude"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Include""
							}
						}
					},
					""required"": [""include""]
				}
			]
		},
		""ValueSet_Include"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""concept"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Concept""
							}
						},
						""filter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Filter""
							}
						},
						""valueSet"": {
							""type"": ""array"",
							""items"": {
								""type"": ""string""
							}
						},
						""_valueSet"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Element""
							}
						}
					}
				}
			]
		},
		""ValueSet_Concept"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""designation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Designation""
							}
						}
					}
				}
			]
		},
		""ValueSet_Designation"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""language"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_language"": {
							""$ref"": ""#/definitions/Element""
						},
						""use"": {
							""$ref"": ""#/definitions/Coding""
						},
						""value"": {
							""type"": ""string""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ValueSet_Filter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""property"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_property"": {
							""$ref"": ""#/definitions/Element""
						},
						""op"": {
							""enum"": [""="", ""is-a"", ""descendent-of"", ""is-not-a"", ""regex"", ""in"", ""not-in"", ""generalizes"", ""exists""],
							""type"": ""string""
						},
						""_op"": {
							""$ref"": ""#/definitions/Element""
						},
						""value"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_value"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ValueSet_Expansion"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""identifier"": {
							""type"": ""string""
						},
						""_identifier"": {
							""$ref"": ""#/definitions/Element""
						},
						""timestamp"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_timestamp"": {
							""$ref"": ""#/definitions/Element""
						},
						""total"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_total"": {
							""$ref"": ""#/definitions/Element""
						},
						""offset"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_offset"": {
							""$ref"": ""#/definitions/Element""
						},
						""parameter"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Parameter""
							}
						},
						""contains"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Contains""
							}
						}
					}
				}
			]
		},
		""ValueSet_Parameter"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""name"": {
							""type"": ""string""
						},
						""_name"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueString"": {
							""type"": ""string""
						},
						""_valueString"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueBoolean"": {
							""type"": ""boolean""
						},
						""_valueBoolean"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueInteger"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))"",
							""type"": ""number""
						},
						""_valueInteger"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueDecimal"": {
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?"",
							""type"": ""number""
						},
						""_valueDecimal"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueUri"": {
							""type"": ""string""
						},
						""_valueUri"": {
							""$ref"": ""#/definitions/Element""
						},
						""valueCode"": {
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*"",
							""type"": ""string""
						},
						""_valueCode"": {
							""$ref"": ""#/definitions/Element""
						}
					}
				}
			]
		},
		""ValueSet_Contains"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""system"": {
							""type"": ""string""
						},
						""_system"": {
							""$ref"": ""#/definitions/Element""
						},
						""abstract"": {
							""type"": ""boolean""
						},
						""_abstract"": {
							""$ref"": ""#/definitions/Element""
						},
						""inactive"": {
							""type"": ""boolean""
						},
						""_inactive"": {
							""$ref"": ""#/definitions/Element""
						},
						""version"": {
							""type"": ""string""
						},
						""_version"": {
							""$ref"": ""#/definitions/Element""
						},
						""code"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_code"": {
							""$ref"": ""#/definitions/Element""
						},
						""display"": {
							""type"": ""string""
						},
						""_display"": {
							""$ref"": ""#/definitions/Element""
						},
						""designation"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Designation""
							}
						},
						""contains"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/ValueSet_Contains""
							}
						}
					}
				}
			]
		},
		""VisionPrescription"": {
			""allOf"": [{
					""$ref"": ""#/definitions/DomainResource""
				}, {
					""properties"": {
						""resourceType"": {
							""type"": ""string"",
							""enum"": [""VisionPrescription""]
						},
						""identifier"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Identifier""
							}
						},
						""status"": {
							""type"": ""string"",
							""pattern"": ""[^\\s]+([\\s]?[^\\s]+)*""
						},
						""_status"": {
							""$ref"": ""#/definitions/Element""
						},
						""patient"": {
							""$ref"": ""#/definitions/Reference""
						},
						""encounter"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dateWritten"": {
							""type"": ""string"",
							""pattern"": ""-?[0-9]{4}(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.[0-9]+)?(Z|(\\+|-)((0[0-9]|1[0-3]):[0-5][0-9]|14:00)))?)?)?""
						},
						""_dateWritten"": {
							""$ref"": ""#/definitions/Element""
						},
						""prescriber"": {
							""$ref"": ""#/definitions/Reference""
						},
						""reasonCodeableConcept"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""reasonReference"": {
							""$ref"": ""#/definitions/Reference""
						},
						""dispense"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/VisionPrescription_Dispense""
							}
						}
					},
					""required"": [""resourceType""]
				}
			]
		},
		""VisionPrescription_Dispense"": {
			""allOf"": [{
					""$ref"": ""#/definitions/BackboneElement""
				}, {
					""properties"": {
						""product"": {
							""$ref"": ""#/definitions/CodeableConcept""
						},
						""eye"": {
							""enum"": [""right"", ""left""],
							""type"": ""string""
						},
						""_eye"": {
							""$ref"": ""#/definitions/Element""
						},
						""sphere"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_sphere"": {
							""$ref"": ""#/definitions/Element""
						},
						""cylinder"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_cylinder"": {
							""$ref"": ""#/definitions/Element""
						},
						""axis"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))""
						},
						""_axis"": {
							""$ref"": ""#/definitions/Element""
						},
						""prism"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_prism"": {
							""$ref"": ""#/definitions/Element""
						},
						""base"": {
							""enum"": [""up"", ""down"", ""in"", ""out""],
							""type"": ""string""
						},
						""_base"": {
							""$ref"": ""#/definitions/Element""
						},
						""add"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_add"": {
							""$ref"": ""#/definitions/Element""
						},
						""power"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_power"": {
							""$ref"": ""#/definitions/Element""
						},
						""backCurve"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_backCurve"": {
							""$ref"": ""#/definitions/Element""
						},
						""diameter"": {
							""type"": ""number"",
							""pattern"": ""-?([0]|([1-9][0-9]*))(\\.[0-9]+)?""
						},
						""_diameter"": {
							""$ref"": ""#/definitions/Element""
						},
						""duration"": {
							""$ref"": ""#/definitions/Quantity""
						},
						""color"": {
							""type"": ""string""
						},
						""_color"": {
							""$ref"": ""#/definitions/Element""
						},
						""brand"": {
							""type"": ""string""
						},
						""_brand"": {
							""$ref"": ""#/definitions/Element""
						},
						""note"": {
							""type"": ""array"",
							""items"": {
								""$ref"": ""#/definitions/Annotation""
							}
						}
					}
				}
			]
		}
	}
}
";
        #endregion
    }
}