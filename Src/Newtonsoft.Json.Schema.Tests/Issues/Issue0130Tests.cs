#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

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
    public class Issue0130Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(SchemaJson);

            JToken t = JToken.Parse(Json);

            Assert.AreEqual(false, t.IsValid(s, out IList<ValidationError> errorMessages));

            Assert.AreEqual(3, errorMessages.Count);
            Assert.AreEqual("JSON does not match schema from 'then'. Path 'pages[0].sections.default[12].widgetContainerOptions', line 261, position 39.", errorMessages[2].GetExtendedMessage());
            Assert.AreEqual("Required properties are missing from object: collapsableOptions. Path 'pages[0].sections.default[12].widgetContainerOptions', line 261, position 39.", errorMessages[2].ChildErrors[0].GetExtendedMessage());
        }

        private const string Json = @"{
  ""masterPages"": [
    {
      ""id"": ""master"",
      ""properties"": {},
      ""sections"": {
        ""default"": {
          ""prefixComponents"": [
            {
              ""componentType"": ""siteHeader"",
              ""siteHeaderOptions"": {}
            }
          ],
          ""suffixComponents"": [
            {
              ""componentType"": ""siteFooter"",
              ""siteFooterOptions"": {}
            }
          ]
        }
      }
    }
  ],
  ""pages"": [
    {
      ""id"": ""non-owner-auto-insurance"",
      ""properties"": {
        ""title"": ""Non-Owner Auto Insurance"",
        ""path"": ""/our-products/non-owner-auto-insurance"",
        ""masterPageId"": ""master"",
        ""meta"": {
          ""description"": {
            ""content"": ""Non owner car insurance policies are designed to protect individuals who don't drive regularly, but require coverage for the times they do drive a car. Acceptance can help explain all your options and provide a no-hassle, no-obligation non owner car insurance quote, so you can decide whether this type of policy is right for you.""
          }
        }
      },
      ""sections"": {
        ""default"": [
          {
            ""componentType"": ""heroImage"",
            ""heroImageOptions"": {
              ""image"": ""images/products/non-owner-\ncar-insurance.jpg"",
              ""title"": ""Non-Owner Car Insurance""
            }
          },
          {
            ""componentType"": ""widgetContainer"",
            ""widgetContainerOptions"": {
              ""title"": """",
              ""collapsable"": true,
              ""collapsableOptions"": {
                ""actionTitle"": ""Get A Quote""
              },
              ""widgets"": [
                ""get-a-quote""
              ]
            }
          },
          {
            ""componentType"": ""titledParagraph"",
            ""titledParagraphOptions"": {
              ""title"": ""About Non-Owner Auto Insurance"",
              ""body"": ""Non owner car insurance policies are designed to protect individuals who don't drive regularly, but require coverage for the times they do drive a car. Acceptance can help explain all your options and provide a no-hassle, no-obligation non owner car insurance quote, so you can decide whether this type of policy is right for you.""
            }
          },
          {
            ""componentType"": ""anchorIndex"",
            ""anchorIndexOptions"": {
              ""links"": [
                {
                  ""title"": ""Is It Right For Me?"",
                  ""anchorName"": ""is-it-right-for-me""
                },
                {
                  ""title"": ""Types of Insurance"",
                  ""anchorName"": ""types-of-insurance""
                },
                {
                  ""title"": ""State By State"",
                  ""anchorName"": ""state-by-state""
                },
                {
                  ""title"": ""Talk To Us"",
                  ""anchorName"": ""talk-to-us""
                }
              ]
            }
          },
          {
            ""componentType"": ""anchorHeader"",
            ""anchorHeaderOptions"": {
              ""id"": ""is-it-right-for-me"",
              ""title"": ""Is It Right For Me?""
            }
          },
          {
            ""componentType"": ""titledParagraph"",
            ""titledParagraphOptions"": {
              ""title"": ""Who Should Consider Non Owner Insurance?"",
              ""body"": ""Insurance experts recommend non-owners auto insurance for anyone who doesn't own a vehicle. The policy will provide the basic liability coverage you need to protect yourself and your assets after an accident for which you were at fault.  If you don’t own a car but still need liability insurance coverage, it’s well worth considering. ""
            }
          },
          {
            ""componentType"": ""titledParagraph"",
            ""titledParagraphOptions"": {
              ""title"": ""The Facts about Non Owner Insurance"",
              ""body"": ""Although there are several insurance companies that offer non owner coverage, it isn't usually advertised to the extent that other policies are, so most people don't even know it exists. Non owner car insurance policies can be a good alternative to full coverage. Getting a quote through Acceptance is easy, and this type of insurance can save a lot of money in the end compared to the cost of paying for the medical bills and property damage associated with an accident.""
            }
          },
          {
            ""componentType"": ""titledParagraph"",
            ""titledParagraphOptions"": {
              ""title"": ""How Much Does it Cost?"",
              ""body"": ""There are several factors that may determine the cost of your non owner insurance policy. The amount of coverage you desire will, of course, affect how much you pay. Your driving history and the estimated frequency with which you plan on using a car will also influence the cost of your monthly payment. Contact us today to learn more and get a free non owner car insurance quote. One of our highly knowledgeable insurance specialists can explain how this type of policy works, what it might cover, what kinds of coverage options there are and anything else you're concerned with.""
            }
          },
          {
            ""componentType"": ""anchorHeader"",
            ""anchorHeaderOptions"": {
              ""id"": ""types-of-insurance"",
              ""title"": ""Types of Insurance""
            }
          },
          {
            ""componentType"": ""linkList"",
            ""linkListOptions"": {
              ""listType"": ""text"",
              ""links"": [
                {
                  ""title"": ""High Risk"",
                  ""url"": ""/our-products/high-risk-insurance""
                },
                {
                  ""title"": ""SR22"",
                  ""url"": ""/our-products/sr22-insurance""
                },
                {
                  ""title"": ""Non-Owner"",
                  ""url"": ""/our-products/non-owner-insurance""
                },
                {
                  ""title"": ""Liability"",
                  ""url"": ""/our-products/liability-insurance""
                },
                {
                  ""title"": ""Teenager"",
                  ""url"": ""/our-products/teenager-insurance""
                },
                {
                  ""title"": ""Senior"",
                  ""url"": ""/our-products/senior-insurance""
                },
                {
                  ""title"": ""Non-Standard"",
                  ""url"": ""/our-products/non-standard-insurance""
                }
              ]
            }
          },
          {
            ""componentType"": ""linkList"",
            ""linkListOptions"": {
              ""title"": ""All Insurances We Offer"",
              ""listType"": ""icon"",
              ""links"": [
                {
                  ""title"": ""Auto"",
                  ""url"": ""/our-products/auto-insurance"",
                  ""icon"": ""ai-product-auto""
                },
                {
                  ""title"": ""Motorcycle"",
                  ""url"": ""/our-products/motorcycle-insurance"",
                  ""icon"": ""ai-product-motorcycle""
                },
                {
                  ""title"": ""Roadside Assistance"",
                  ""url"": ""/our-products/roadside-assitance"",
                  ""icon"": ""ai-product-roadside-assistance""
                },
                {
                  ""title"": ""Renters"",
                  ""url"": ""/our-products/renters-insurance"",
                  ""icon"": ""ai-product-renters""
                },
                {
                  ""title"": ""Homeowners"",
                  ""url"": ""/our-products/homeowners-insurance"",
                  ""icon"": ""ai-product-homeowners""
                },
                {
                  ""title"": ""Commercial"",
                  ""url"": ""/our-products/commercial-insurance"",
                  ""icon"": ""ai-product-commercial""
                },
                {
                  ""title"": ""Pet"",
                  ""url"": ""/our-products/pet-insurance"",
                  ""icon"": ""ai-product-pet""
                },
                {
                  ""title"": ""Health"",
                  ""url"": ""/our-products/health-insurance"",
                  ""icon"": ""ai-product-health""
                },
                {
                  ""title"": ""Life"",
                  ""url"": ""/our-products/life-insurance"",
                  ""icon"": ""ai-product-life""
                }
              ]
            }
          },
          {
            ""componentType"": ""linkList"",
            ""linkListOptions"": {
              ""title"": ""State By State"",
              ""listType"": ""chooser"",
              ""links"": [
                {
                  ""title"": ""Alabama"",
                  ""url"": ""/states/alabama""
                },
                {
                  ""title"": ""Arizona"",
                  ""url"": ""/states/arizona""
                },
                {
                  ""title"": ""California"",
                  ""url"": ""/states/california""
                },
                {
                  ""title"": ""Florida"",
                  ""url"": ""/states/florida""
                },
                {
                  ""title"": ""Georgia"",
                  ""url"": ""/states/georgia""
                },
                {
                  ""title"": ""Illinois"",
                  ""url"": ""/states/illinois""
                },
                {
                  ""title"": ""Indiana"",
                  ""url"": ""/indiana""
                },
                {
                  ""title"": ""Mississippi"",
                  ""url"": ""/states/mississippi""
                },
                {
                  ""title"": ""Missouri"",
                  ""url"": ""/states/missouri""
                }
              ]
            }
          },
          {
            ""componentType"": ""widgetContainer"",
            ""widgetContainerOptions"": {
              ""title"": ""Talk To Us"",
              ""collapsable"": true,
              ""widgets"": [
                ""call-us""
              ]
            }
          }
        ]
      }
    }
  ]
}";

        private const string SchemaJson = @"{
  ""id"": ""http://pdl-schema.org/draft-01/schema#"",
  ""title"": ""Page Definition Language Schema"",
  ""definitions"": {
    ""componentContainerArray"": {
      ""type"": ""array"",
      ""items"": {
        ""$ref"": ""#/definitions/componentContainer""
      }
    },
    ""componentContainer"": {
      ""type"": ""object"",
      ""properties"": {
        ""componentType"": {
          ""$ref"": ""#/definitions/components/definitions/componentTypes""
        },
        ""heroImageOptions"": {
          ""$ref"": ""#/definitions/components/definitions/heroImage""
        },
        ""titledParagraphOptions"": {
          ""$ref"": ""#/definitions/components/definitions/titledParagraph""
        },
        ""widgetContainerOptions"": {
          ""$ref"": ""#/definitions/components/definitions/widgetContainer""
        },
        ""linkListOptions"": {
          ""$ref"": ""#/definitions/components/definitions/linkList""
        },
        ""siteHeaderOptions"": {
          ""$ref"": ""#/definitions/components/definitions/siteHeader""
        },
        ""siteFooterOptions"": {
          ""$ref"": ""#/definitions/components/definitions/siteFooter""
        },
        ""anchorIndexOptions"": {
          ""$ref"": ""#/definitions/components/definitions/anchorIndex""
        },
        ""anchorHeaderOptions"": {
          ""$ref"": ""#/definitions/components/definitions/anchorHeader""
        }
      },
      ""oneOf"": [
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""heroImage""
              ]
            }
          },
          ""required"": [
            ""heroImageOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""titledParagraph""
              ]
            }
          },
          ""required"": [
            ""titledParagraphOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""widgetContainer""
              ]
            }
          },
          ""required"": [
            ""widgetContainerOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""linkList""
              ]
            }
          },
          ""required"": [
            ""linkListOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""anchorIndex""
              ]
            }
          },
          ""required"": [
            ""anchorIndexOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""anchorHeader""
              ]
            }
          },
          ""required"": [
            ""anchorHeaderOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""siteHeader""
              ]
            }
          },
          ""required"": [
            ""siteHeaderOptions""
          ]
        },
        {
          ""properties"": {
            ""componentType"": {
              ""enum"": [
                ""siteFooter""
              ]
            }
          },
          ""required"": [
            ""siteFooterOptions""
          ]
        }
      ],
      ""additionalProperties"": false,
      ""required"": [
        ""componentType""
      ]
    },
    ""components"": {
      ""id"": ""components"",
      ""definitions"": {
        ""componentTypes"": {
          ""enum"": [
            ""heroImage"",
            ""titledParagraph"",
            ""widgetContainer"",
            ""linkList"",
            ""anchorIndex"",
            ""anchorHeader"",
            ""siteHeader"",
            ""siteFooter""
          ]
        },
        ""siteHeader"": {
          ""id"": ""siteHeader"",
          ""type"": ""object"",
          ""properties"": {},
          ""additionalProperties"": false,
          ""required"": []
        },
        ""siteFooter"": {
          ""id"": ""siteFooter"",
          ""type"": ""object"",
          ""properties"": {},
          ""required"": [],
          ""additionalProperties"": false
        },
        ""heroImage"": {
          ""id"": ""heroImage"",
          ""type"": ""object"",
          ""properties"": {
            ""image"": {
              ""type"": ""string"",
              ""description"": """"
            },
            ""title"": {
              ""type"": ""string"",
              ""description"": ""Overlay title text""
            }
          },
          ""required"": [
            ""image"",
            ""title""
          ],
          ""additionalProperties"": false
        },
        ""titledParagraph"": {
          ""id"": ""titledParagraph"",
          ""type"": ""object"",
          ""properties"": {
            ""title"": {
              ""type"": ""string"",
              ""description"": """"
            },
            ""body"": {
              ""type"": ""string"",
              ""description"": """"
            }
          },
          ""required"": [
            ""title"",
            ""body""
          ],
          ""additionalProperties"": false
        },
        ""anchorIndex"": {
          ""id"": ""anchorIndex"",
          ""type"": ""object"",
          ""definitions"": {
            ""anchorIndexItem"": {
              ""type"": ""object"",
              ""properties"": {
                ""title"": {
                  ""type"": ""string""
                },
                ""anchorName"": {
                  ""type"": ""string""
                }
              },
              ""required"": [
                ""title"",
                ""anchorName""
              ],
              ""additionalProperties"": false
            }
          },
          ""properties"": {
            ""links"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/anchorIndexItem""
              }
            }
          },
          ""required"": [
            ""links""
          ],
          ""additionalProperties"": false
        },
        ""anchorHeader"": {
          ""id"": ""anchorHeader"",
          ""type"": ""object"",
          ""properties"": {
            ""id"": {
              ""type"": ""string"",
              ""pattern"": ""^[A-Za-z0-9\\-_]*$""
            },
            ""title"": {
              ""type"": ""string"",
              ""description"": """"
            }
          },
          ""required"": [
            ""id"",
            ""title""
          ],
          ""additionalProperties"": false
        },
        ""linkList"": {
          ""id"": ""linkList"",
          ""type"": ""object"",
          ""definitions"": {
            ""iconType"": {
              ""enum"": [
                ""ai-product-auto"",
                ""ai-product-motorcycle"",
                ""ai-product-roadside-assistance"",
                ""ai-product-renters"",
                ""ai-product-homeowners"",
                ""ai-product-commercial"",
                ""ai-product-pet"",
                ""ai-product-health"",
                ""ai-product-life""
              ]
            },
            ""linkOptions"": {
              ""type"": ""object"",
              ""properties"": {
                ""title"": {
                  ""type"": ""string""
                },
                ""url"": {
                  ""type"": ""string""
                },
                ""icon"": {
                  ""$ref"": ""#/definitions/iconType""
                }
              },
              ""required"": [
                ""title"",
                ""url""
              ],
              ""additionalProperties"": false
            }
          },
          ""properties"": {
            ""title"": {
              ""type"": ""string""
            },
            ""listType"": {
              ""enum"": [
                ""text"",
                ""icon"",
                ""chooser""
              ]
            },
            ""links"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/linkOptions""
              }
            }
          },
          ""required"": [
            ""listType"",
            ""links""
          ],
          ""additionalProperties"": false
        },
        ""widgetContainer"": {
          ""id"": ""widgetContainer"",
          ""type"": ""object"",
          ""definitions"": {
            ""collapsableOptions"": {
              ""type"": ""object"",
              ""properties"": {
                ""collapsed"": {
                  ""type"": ""boolean""
                },
                ""actionTitle"": {
                  ""type"": ""string""
                }
              },
              ""required"": [
                ""actionTitle""
              ],
              ""additionalProperties"": false
            }
          },
          ""properties"": {
            ""title"": {
              ""type"": ""string""
            },
            ""containerType"": {
              ""enum"": [
                ""collapsable"",
                ""container""
              ]
            },
            ""collapsableOptions"": {
              ""$ref"": ""#/definitions/collapsableOptions""
            },
            ""widgets"": {
              ""type"": ""array"",
              ""minItems"": 1,
              ""items"": {
                ""enum"": [
                  ""call-us"",
                  ""find-an-agent"",
                  ""find-a-location"",
                  ""get-a-quote"",
                  ""make-a-claim""
                ]
              }
            }
          },
          ""if"": {
            ""properties"": {
              """": {
                ""enum"": [
                  ""collapsable""
                ]
              }
            }
          },
          ""then"": {
            ""required"": [
              ""collapsableOptions""
            ]
          },
          ""required"": [
            ""widgets""
          ],
          ""additionalProperties"": false
        }
      }
    },
    ""screenTypes"": {
      ""enum"": [
        ""mobile"",
        ""desktop""
      ]
    },
    ""screenTypeOptions"": {
      ""type"": ""object"",
      ""properties"": {
        ""viewportType"": {
          ""$ref"": ""#/definitions/screenTypeOptions""
        },
        ""hidden"": {
          ""type"": ""boolean""
        },
        ""classes"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""string""
          }
        }
      }
    },
    ""metaTag"": {
      ""id"": ""metaTag"",
      ""type"": ""object"",
      ""properties"": {
        ""content"": {
          ""type"": ""string""
        }
      },
      ""patternProperties"": {
        ""^[A-Za-z:\\s0-9\\-]*$"": {
          ""type"": ""string""
        }
      },
      ""additionalProperties"": false,
      ""required"": [
        ""content""
      ]
    },
    ""masterPageOptions"": {
      ""type"": ""object"",
      ""properties"": {
        ""meta"": {
          ""type"": ""object""
        }
      },
      ""required"": [],
      ""additionalProperties"": false
    },
    ""pageOptions"": {
      ""type"": ""object"",
      ""properties"": {
        ""title"": {
          ""type"": ""string""
        },
        ""path"": {
          ""type"": ""string""
        },
        ""masterPageId"": {
          ""type"": ""string""
        },
        ""meta"": {
          ""type"": ""object"",
          ""patternProperties"": {
            ""^[A-Za-z:\\s0-9\\-]*$"": {
              ""$ref"": ""#/definitions/metaTag""
            }
          }
        }
      },
      ""required"": [
        ""title"",
        ""path"",
        ""masterPageId""
      ],
      ""additionalProperties"": false
    },
    ""page"": {
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""type"": ""string""
        },
        ""properties"": {
          ""$ref"": ""#/definitions/pageOptions""
        },
        ""sections"": {
          ""type"": ""object"",
          ""properties"": {
            ""default"": {
              ""$ref"": ""#/definitions/componentContainerArray""
            }
          },
          ""required"": [
            ""default""
          ]
        }
      },
      ""additionalProperties"": true,
      ""required"": [
        ""id"",
        ""properties"",
        ""sections""
      ]
    },
    ""masterPageSection"": {
      ""type"": ""object"",
      ""properties"": {
        ""prefixComponents"": {
          ""$ref"": ""#/definitions/componentContainerArray""
        },
        ""suffixComponents"": {
          ""$ref"": ""#/definitions/componentContainerArray""
        }
      },
      ""required"": [
        ""prefixComponents"",
        ""suffixComponents""
      ],
      ""additionalProperties"": false
    },
    ""masterPage"": {
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""type"": ""string""
        },
        ""properties"": {
          ""$ref"": ""#/definitions/masterPageOptions""
        },
        ""sections"": {
          ""type"": ""object"",
          ""properties"": {
            ""default"": {
              ""$ref"": ""#/definitions/masterPageSection""
            }
          },
          ""required"": [
            ""default""
          ]
        }
      },
      ""additionalProperties"": true,
      ""required"": [
        ""id"",
        ""properties"",
        ""sections""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""masterPages"": {
      ""type"": ""array"",
      ""items"": {
        ""$ref"": ""#/definitions/masterPage""
      },
      ""minItems"": 1
    },
    ""pages"": {
      ""type"": ""array"",
      ""items"": {
        ""$ref"": ""#/definitions/page""
      }
    },
    ""additionalProperties"": false
  },
  ""required"": [
    ""masterPages"",
    ""pages""
  ]
}";
    }
}