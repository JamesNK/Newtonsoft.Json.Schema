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
    public class Issue0103Test : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            JSchema parms = schema.Properties["instanceParms"];
            Assert.AreEqual(8192, parms.MaximumLength);
        }

        private const string SchemaJson = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
        ""authorizedName"": {
            ""$ref"": ""#/definitions/authorizedName""
        },
        ""nodeID"": {
            ""$ref"": ""#/definitions/nodeID""
        },
        ""accessPoint"": {
            ""$ref"": ""#/definitions/accessPoint""
        },
        ""actualCount"": {
            ""$ref"": ""#/definitions/actualCount""
        },
        ""address"": {
            ""$ref"": ""#/definitions/address""
        },
        ""addressLine"": {
            ""$ref"": ""#/definitions/addressLine""
        },
        ""assertionStatusItem"": {
            ""$ref"": ""#/definitions/assertionStatusItem""
        },
        ""authInfo"": {
            ""$ref"": ""#/definitions/authInfo""
        },
        ""bindingKey"": {
            ""$ref"": ""#/definitions/bindingKey""
        },
        ""bindingTemplate"": {
            ""$ref"": ""#/definitions/bindingTemplate""
        },
        ""bindingTemplates"": {
            ""$ref"": ""#/definitions/bindingTemplates""
        },
        ""businessEntity"": {
            ""$ref"": ""#/definitions/businessEntity""
        },
        ""businessInfo"": {
            ""$ref"": ""#/definitions/businessInfo""
        },
        ""businessInfos"": {
            ""$ref"": ""#/definitions/businessInfos""
        },
        ""businessKey"": {
            ""$ref"": ""#/definitions/businessKey""
        },
        ""businessService"": {
            ""$ref"": ""#/definitions/businessService""
        },
        ""businessServices"": {
            ""$ref"": ""#/definitions/businessServices""
        },
        ""categoryBag"": {
            ""$ref"": ""#/definitions/categoryBag""
        },
        ""completionStatus"": {
            ""$ref"": ""#/definitions/completionStatus""
        },
        ""contact"": {
            ""$ref"": ""#/definitions/contact""
        },
        ""contacts"": {
            ""$ref"": ""#/definitions/contacts""
        },
        ""description"": {
            ""$ref"": ""#/definitions/description""
        },
        ""discoveryURL"": {
            ""$ref"": ""#/definitions/discoveryURL""
        },
        ""discoveryURLs"": {
            ""$ref"": ""#/definitions/discoveryURLs""
        },
        ""dispositionReport"": {
            ""$ref"": ""#/definitions/dispositionReport""
        },
        ""email"": {
            ""$ref"": ""#/definitions/email""
        },
        ""errInfo"": {
            ""$ref"": ""#/definitions/errInfo""
        },
        ""findQualifier"": {
            ""$ref"": ""#/definitions/findQualifier""
        },
        ""findQualifiers"": {
            ""$ref"": ""#/definitions/findQualifiers""
        },
        ""fromKey"": {
            ""$ref"": ""#/definitions/fromKey""
        },
        ""hostingRedirector"": {
            ""$ref"": ""#/definitions/hostingRedirector""
        },
        ""identifierBag"": {
            ""$ref"": ""#/definitions/identifierBag""
        },
        ""includeCount"": {
            ""$ref"": ""#/definitions/includeCount""
        },
        ""infoSelection"": {
            ""$ref"": ""#/definitions/infoSelection""
        },
        ""instanceDetails"": {
            ""$ref"": ""#/definitions/instanceDetails""
        },
        ""instanceParms"": {
            ""$ref"": ""#/definitions/instanceParms""
        },
        ""keyedReference"": {
            ""$ref"": ""#/definitions/keyedReference""
        },
        ""keyedReferenceGroup"": {
            ""$ref"": ""#/definitions/keyedReferenceGroup""
        },
        ""keysOwned"": {
            ""$ref"": ""#/definitions/keysOwned""
        },
        ""listDescription"": {
            ""$ref"": ""#/definitions/listDescription""
        },
        ""listHead"": {
            ""$ref"": ""#/definitions/listHead""
        },
        ""name"": {
            ""$ref"": ""#/definitions/name""
        },
        ""operationalInfo"": {
            ""$ref"": ""#/definitions/operationalInfo""
        },
        ""overviewDoc"": {
            ""$ref"": ""#/definitions/overviewDoc""
        },
        ""overviewURL"": {
            ""$ref"": ""#/definitions/overviewURL""
        },
        ""personName"": {
            ""$ref"": ""#/definitions/personName""
        },
        ""phone"": {
            ""$ref"": ""#/definitions/phone""
        },
        ""publisherAssertion"": {
            ""$ref"": ""#/definitions/publisherAssertion""
        },
        ""relatedBusinessInfo"": {
            ""$ref"": ""#/definitions/relatedBusinessInfo""
        },
        ""relatedBusinessInfos"": {
            ""$ref"": ""#/definitions/relatedBusinessInfos""
        },
        ""result"": {
            ""$ref"": ""#/definitions/result""
        },
        ""serviceInfo"": {
            ""$ref"": ""#/definitions/serviceInfo""
        },
        ""serviceInfos"": {
            ""$ref"": ""#/definitions/serviceInfos""
        },
        ""serviceKey"": {
            ""$ref"": ""#/definitions/serviceKey""
        },
        ""sharedRelationships"": {
            ""$ref"": ""#/definitions/sharedRelationships""
        },
        ""tModel"": {
            ""$ref"": ""#/definitions/tModel""
        },
        ""tModelBag"": {
            ""$ref"": ""#/definitions/tModelBag""
        },
        ""tModelInfo"": {
            ""$ref"": ""#/definitions/tModelInfo""
        },
        ""tModelInfos"": {
            ""$ref"": ""#/definitions/tModelInfos""
        },
        ""tModelInstanceDetails"": {
            ""$ref"": ""#/definitions/tModelInstanceDetails""
        },
        ""tModelInstanceInfo"": {
            ""$ref"": ""#/definitions/tModelInstanceInfo""
        },
        ""tModelKey"": {
            ""$ref"": ""#/definitions/tModelKey""
        },
        ""toKey"": {
            ""$ref"": ""#/definitions/toKey""
        },
        ""add_publisherAssertions"": {
            ""$ref"": ""#/definitions/add_publisherAssertions""
        },
        ""delete_binding"": {
            ""$ref"": ""#/definitions/delete_binding""
        },
        ""delete_business"": {
            ""$ref"": ""#/definitions/delete_business""
        },
        ""delete_publisherAssertions"": {
            ""$ref"": ""#/definitions/delete_publisherAssertions""
        },
        ""delete_service"": {
            ""$ref"": ""#/definitions/delete_service""
        },
        ""delete_tModel"": {
            ""$ref"": ""#/definitions/delete_tModel""
        },
        ""discard_authToken"": {
            ""$ref"": ""#/definitions/discard_authToken""
        },
        ""find_binding"": {
            ""$ref"": ""#/definitions/find_binding""
        },
        ""find_business"": {
            ""$ref"": ""#/definitions/find_business""
        },
        ""find_relatedBusinesses"": {
            ""$ref"": ""#/definitions/find_relatedBusinesses""
        },
        ""find_service"": {
            ""$ref"": ""#/definitions/find_service""
        },
        ""find_tModel"": {
            ""$ref"": ""#/definitions/find_tModel""
        },
        ""get_assertionStatusReport"": {
            ""$ref"": ""#/definitions/get_assertionStatusReport""
        },
        ""get_authToken"": {
            ""$ref"": ""#/definitions/get_authToken""
        },
        ""get_bindingDetail"": {
            ""$ref"": ""#/definitions/get_bindingDetail""
        },
        ""get_businessDetail"": {
            ""$ref"": ""#/definitions/get_businessDetail""
        },
        ""get_operationalInfo"": {
            ""$ref"": ""#/definitions/get_operationalInfo""
        },
        ""get_publisherAssertions"": {
            ""$ref"": ""#/definitions/get_publisherAssertions""
        },
        ""get_registeredInfo"": {
            ""$ref"": ""#/definitions/get_registeredInfo""
        },
        ""get_serviceDetail"": {
            ""$ref"": ""#/definitions/get_serviceDetail""
        },
        ""get_tModelDetail"": {
            ""$ref"": ""#/definitions/get_tModelDetail""
        },
        ""save_binding"": {
            ""$ref"": ""#/definitions/save_binding""
        },
        ""save_business"": {
            ""$ref"": ""#/definitions/save_business""
        },
        ""save_service"": {
            ""$ref"": ""#/definitions/save_service""
        },
        ""save_tModel"": {
            ""$ref"": ""#/definitions/save_tModel""
        },
        ""set_publisherAssertions"": {
            ""$ref"": ""#/definitions/set_publisherAssertions""
        },
        ""assertionStatusReport"": {
            ""$ref"": ""#/definitions/assertionStatusReport""
        },
        ""authToken"": {
            ""$ref"": ""#/definitions/authToken""
        },
        ""bindingDetail"": {
            ""$ref"": ""#/definitions/bindingDetail""
        },
        ""businessDetail"": {
            ""$ref"": ""#/definitions/businessDetail""
        },
        ""businessList"": {
            ""$ref"": ""#/definitions/businessList""
        },
        ""operationalInfos"": {
            ""$ref"": ""#/definitions/operationalInfos""
        },
        ""publisherAssertions"": {
            ""$ref"": ""#/definitions/publisherAssertions""
        },
        ""registeredInfo"": {
            ""$ref"": ""#/definitions/registeredInfo""
        },
        ""relatedBusinessesList"": {
            ""$ref"": ""#/definitions/relatedBusinessesList""
        },
        ""serviceDetail"": {
            ""$ref"": ""#/definitions/serviceDetail""
        },
        ""serviceList"": {
            ""$ref"": ""#/definitions/serviceList""
        },
        ""tModelDetail"": {
            ""$ref"": ""#/definitions/tModelDetail""
        },
        ""tModelList"": {
            ""$ref"": ""#/definitions/tModelList""
        },
        ""Signature"": {
            ""$ref"": ""#/definitions/Signature""
        },
        ""SignatureValue"": {
            ""$ref"": ""#/definitions/SignatureValue""
        },
        ""SignedInfo"": {
            ""$ref"": ""#/definitions/SignedInfo""
        },
        ""CanonicalizationMethod"": {
            ""$ref"": ""#/definitions/CanonicalizationMethod""
        },
        ""SignatureMethod"": {
            ""$ref"": ""#/definitions/SignatureMethod""
        },
        ""Reference"": {
            ""$ref"": ""#/definitions/Reference""
        },
        ""Transforms"": {
            ""$ref"": ""#/definitions/Transforms""
        },
        ""Transform"": {
            ""$ref"": ""#/definitions/Transform""
        },
        ""DigestMethod"": {
            ""$ref"": ""#/definitions/DigestMethod""
        },
        ""DigestValue"": {
            ""$ref"": ""#/definitions/DigestValue""
        },
        ""KeyInfo"": {
            ""$ref"": ""#/definitions/KeyInfo""
        },
        ""KeyName"": {
            ""$ref"": ""#/definitions/KeyName""
        },
        ""MgmtData"": {
            ""$ref"": ""#/definitions/MgmtData""
        },
        ""KeyValue"": {
            ""$ref"": ""#/definitions/KeyValue""
        },
        ""RetrievalMethod"": {
            ""$ref"": ""#/definitions/RetrievalMethod""
        },
        ""X509Data"": {
            ""$ref"": ""#/definitions/X509Data""
        },
        ""PGPData"": {
            ""$ref"": ""#/definitions/PGPData""
        },
        ""SPKIData"": {
            ""$ref"": ""#/definitions/SPKIData""
        },
        ""Object"": {
            ""$ref"": ""#/definitions/Object""
        },
        ""Manifest"": {
            ""$ref"": ""#/definitions/Manifest""
        },
        ""SignatureProperties"": {
            ""$ref"": ""#/definitions/SignatureProperties""
        },
        ""SignatureProperty"": {
            ""$ref"": ""#/definitions/SignatureProperty""
        },
        ""DSAKeyValue"": {
            ""$ref"": ""#/definitions/DSAKeyValue""
        },
        ""RSAKeyValue"": {
            ""$ref"": ""#/definitions/RSAKeyValue""
        }
    },
    ""definitions"": {
        ""authorizedName"": {
            ""$ref"": ""#/definitions/authorizedName1""
        },
        ""nodeID"": {
            ""$ref"": ""#/definitions/nodeID1""
        },
        ""accessPoint"": {
            ""$ref"": ""#/definitions/accessPoint1""
        },
        ""actualCount"": {
            ""type"": ""integer""
        },
        ""address"": {
            ""$ref"": ""#/definitions/address1""
        },
        ""addressLine"": {
            ""$ref"": ""#/definitions/addressLine1""
        },
        ""assertionStatusItem"": {
            ""$ref"": ""#/definitions/assertionStatusItem1""
        },
        ""authInfo"": {
            ""type"": ""string""
        },
        ""bindingKey"": {
            ""$ref"": ""#/definitions/bindingKey1""
        },
        ""bindingTemplate"": {
            ""$ref"": ""#/definitions/bindingTemplate1""
        },
        ""bindingTemplates"": {
            ""$ref"": ""#/definitions/bindingTemplates1""
        },
        ""businessEntity"": {
            ""$ref"": ""#/definitions/businessEntity1""
        },
        ""businessInfo"": {
            ""$ref"": ""#/definitions/businessInfo1""
        },
        ""businessInfos"": {
            ""$ref"": ""#/definitions/businessInfos1""
        },
        ""businessKey"": {
            ""$ref"": ""#/definitions/businessKey1""
        },
        ""businessService"": {
            ""$ref"": ""#/definitions/businessService1""
        },
        ""businessServices"": {
            ""$ref"": ""#/definitions/businessServices1""
        },
        ""categoryBag"": {
            ""$ref"": ""#/definitions/categoryBag1""
        },
        ""completionStatus"": {
            ""$ref"": ""#/definitions/completionStatus1""
        },
        ""contact"": {
            ""$ref"": ""#/definitions/contact1""
        },
        ""contacts"": {
            ""$ref"": ""#/definitions/contacts1""
        },
        ""description"": {
            ""$ref"": ""#/definitions/description1""
        },
        ""discoveryURL"": {
            ""$ref"": ""#/definitions/discoveryURL1""
        },
        ""discoveryURLs"": {
            ""$ref"": ""#/definitions/discoveryURLs1""
        },
        ""dispositionReport"": {
            ""$ref"": ""#/definitions/dispositionReport1""
        },
        ""email"": {
            ""$ref"": ""#/definitions/email1""
        },
        ""errInfo"": {
            ""$ref"": ""#/definitions/errInfo1""
        },
        ""findQualifier"": {
            ""$ref"": ""#/definitions/findQualifier1""
        },
        ""findQualifiers"": {
            ""$ref"": ""#/definitions/findQualifiers1""
        },
        ""fromKey"": {
            ""$ref"": ""#/definitions/businessKey1""
        },
        ""hostingRedirector"": {
            ""$ref"": ""#/definitions/hostingRedirector1""
        },
        ""identifierBag"": {
            ""$ref"": ""#/definitions/identifierBag1""
        },
        ""includeCount"": {
            ""type"": ""integer""
        },
        ""infoSelection"": {
            ""$ref"": ""#/definitions/infoSelection1""
        },
        ""instanceDetails"": {
            ""$ref"": ""#/definitions/instanceDetails1""
        },
        ""instanceParms"": {
            ""$ref"": ""#/definitions/instanceParms1""
        },
        ""keyedReference"": {
            ""$ref"": ""#/definitions/keyedReference1""
        },
        ""keyedReferenceGroup"": {
            ""$ref"": ""#/definitions/keyedReferenceGroup1""
        },
        ""keysOwned"": {
            ""$ref"": ""#/definitions/keysOwned1""
        },
        ""listDescription"": {
            ""$ref"": ""#/definitions/listDescription1""
        },
        ""listHead"": {
            ""type"": ""integer""
        },
        ""name"": {
            ""$ref"": ""#/definitions/name1""
        },
        ""operationalInfo"": {
            ""$ref"": ""#/definitions/operationalInfo1""
        },
        ""overviewDoc"": {
            ""$ref"": ""#/definitions/overviewDoc1""
        },
        ""overviewURL"": {
            ""$ref"": ""#/definitions/overviewURL1""
        },
        ""personName"": {
            ""$ref"": ""#/definitions/personName1""
        },
        ""phone"": {
            ""$ref"": ""#/definitions/phone1""
        },
        ""publisherAssertion"": {
            ""$ref"": ""#/definitions/publisherAssertion1""
        },
        ""relatedBusinessInfo"": {
            ""$ref"": ""#/definitions/relatedBusinessInfo1""
        },
        ""relatedBusinessInfos"": {
            ""$ref"": ""#/definitions/relatedBusinessInfos1""
        },
        ""result"": {
            ""$ref"": ""#/definitions/result1""
        },
        ""serviceInfo"": {
            ""$ref"": ""#/definitions/serviceInfo1""
        },
        ""serviceInfos"": {
            ""$ref"": ""#/definitions/serviceInfos1""
        },
        ""serviceKey"": {
            ""$ref"": ""#/definitions/serviceKey1""
        },
        ""sharedRelationships"": {
            ""$ref"": ""#/definitions/sharedRelationships1""
        },
        ""tModel"": {
            ""$ref"": ""#/definitions/tModel1""
        },
        ""tModelBag"": {
            ""$ref"": ""#/definitions/tModelBag1""
        },
        ""tModelInfo"": {
            ""$ref"": ""#/definitions/tModelInfo1""
        },
        ""tModelInfos"": {
            ""$ref"": ""#/definitions/tModelInfos1""
        },
        ""tModelInstanceDetails"": {
            ""$ref"": ""#/definitions/tModelInstanceDetails1""
        },
        ""tModelInstanceInfo"": {
            ""$ref"": ""#/definitions/tModelInstanceInfo1""
        },
        ""tModelKey"": {
            ""$ref"": ""#/definitions/tModelKey1""
        },
        ""toKey"": {
            ""$ref"": ""#/definitions/businessKey1""
        },
        ""add_publisherAssertions"": {
            ""$ref"": ""#/definitions/add_publisherAssertions1""
        },
        ""delete_binding"": {
            ""$ref"": ""#/definitions/delete_binding1""
        },
        ""delete_business"": {
            ""$ref"": ""#/definitions/delete_business1""
        },
        ""delete_publisherAssertions"": {
            ""$ref"": ""#/definitions/delete_publisherAssertions1""
        },
        ""delete_service"": {
            ""$ref"": ""#/definitions/delete_service1""
        },
        ""delete_tModel"": {
            ""$ref"": ""#/definitions/delete_tModel1""
        },
        ""discard_authToken"": {
            ""$ref"": ""#/definitions/discard_authToken1""
        },
        ""find_binding"": {
            ""$ref"": ""#/definitions/find_binding1""
        },
        ""find_business"": {
            ""$ref"": ""#/definitions/find_business1""
        },
        ""find_relatedBusinesses"": {
            ""$ref"": ""#/definitions/find_relatedBusinesses1""
        },
        ""find_service"": {
            ""$ref"": ""#/definitions/find_service1""
        },
        ""find_tModel"": {
            ""$ref"": ""#/definitions/find_tModel1""
        },
        ""get_assertionStatusReport"": {
            ""$ref"": ""#/definitions/get_assertionStatusReport1""
        },
        ""get_authToken"": {
            ""$ref"": ""#/definitions/get_authToken1""
        },
        ""get_bindingDetail"": {
            ""$ref"": ""#/definitions/get_bindingDetail1""
        },
        ""get_businessDetail"": {
            ""$ref"": ""#/definitions/get_businessDetail1""
        },
        ""get_operationalInfo"": {
            ""$ref"": ""#/definitions/get_operationalInfo1""
        },
        ""get_publisherAssertions"": {
            ""$ref"": ""#/definitions/get_publisherAssertions1""
        },
        ""get_registeredInfo"": {
            ""$ref"": ""#/definitions/get_registeredInfo1""
        },
        ""get_serviceDetail"": {
            ""$ref"": ""#/definitions/get_serviceDetail1""
        },
        ""get_tModelDetail"": {
            ""$ref"": ""#/definitions/get_tModelDetail1""
        },
        ""save_binding"": {
            ""$ref"": ""#/definitions/save_binding1""
        },
        ""save_business"": {
            ""$ref"": ""#/definitions/save_business1""
        },
        ""save_service"": {
            ""$ref"": ""#/definitions/save_service1""
        },
        ""save_tModel"": {
            ""$ref"": ""#/definitions/save_tModel1""
        },
        ""set_publisherAssertions"": {
            ""$ref"": ""#/definitions/set_publisherAssertions1""
        },
        ""assertionStatusReport"": {
            ""$ref"": ""#/definitions/assertionStatusReport1""
        },
        ""authToken"": {
            ""$ref"": ""#/definitions/authToken1""
        },
        ""bindingDetail"": {
            ""$ref"": ""#/definitions/bindingDetail1""
        },
        ""businessDetail"": {
            ""$ref"": ""#/definitions/businessDetail1""
        },
        ""businessList"": {
            ""$ref"": ""#/definitions/businessList1""
        },
        ""operationalInfos"": {
            ""$ref"": ""#/definitions/operationalInfos1""
        },
        ""publisherAssertions"": {
            ""$ref"": ""#/definitions/publisherAssertions1""
        },
        ""registeredInfo"": {
            ""$ref"": ""#/definitions/registeredInfo1""
        },
        ""relatedBusinessesList"": {
            ""$ref"": ""#/definitions/relatedBusinessesList1""
        },
        ""serviceDetail"": {
            ""$ref"": ""#/definitions/serviceDetail1""
        },
        ""serviceList"": {
            ""$ref"": ""#/definitions/serviceList1""
        },
        ""tModelDetail"": {
            ""$ref"": ""#/definitions/tModelDetail1""
        },
        ""tModelList"": {
            ""$ref"": ""#/definitions/tModelList1""
        },
        ""Signature"": {
            ""$ref"": ""#/definitions/SignatureType""
        },
        ""SignatureValue"": {
            ""$ref"": ""#/definitions/SignatureValueType""
        },
        ""SignedInfo"": {
            ""$ref"": ""#/definitions/SignedInfoType""
        },
        ""CanonicalizationMethod"": {
            ""$ref"": ""#/definitions/CanonicalizationMethodType""
        },
        ""SignatureMethod"": {
            ""$ref"": ""#/definitions/SignatureMethodType""
        },
        ""Reference"": {
            ""$ref"": ""#/definitions/ReferenceType""
        },
        ""Transforms"": {
            ""$ref"": ""#/definitions/TransformsType""
        },
        ""Transform"": {
            ""$ref"": ""#/definitions/TransformType""
        },
        ""DigestMethod"": {
            ""$ref"": ""#/definitions/DigestMethodType""
        },
        ""DigestValue"": {
            ""$ref"": ""#/definitions/DigestValueType""
        },
        ""KeyInfo"": {
            ""$ref"": ""#/definitions/KeyInfoType""
        },
        ""KeyName"": {
            ""type"": ""string""
        },
        ""MgmtData"": {
            ""type"": ""string""
        },
        ""KeyValue"": {
            ""$ref"": ""#/definitions/KeyValueType""
        },
        ""RetrievalMethod"": {
            ""$ref"": ""#/definitions/RetrievalMethodType""
        },
        ""X509Data"": {
            ""$ref"": ""#/definitions/X509DataType""
        },
        ""PGPData"": {
            ""$ref"": ""#/definitions/PGPDataType""
        },
        ""SPKIData"": {
            ""$ref"": ""#/definitions/SPKIDataType""
        },
        ""Object"": {
            ""$ref"": ""#/definitions/ObjectType""
        },
        ""Manifest"": {
            ""$ref"": ""#/definitions/ManifestType""
        },
        ""SignatureProperties"": {
            ""$ref"": ""#/definitions/SignaturePropertiesType""
        },
        ""SignatureProperty"": {
            ""$ref"": ""#/definitions/SignaturePropertyType""
        },
        ""DSAKeyValue"": {
            ""$ref"": ""#/definitions/DSAKeyValueType""
        },
        ""RSAKeyValue"": {
            ""$ref"": ""#/definitions/RSAKeyValueType""
        },
        ""accessPoint1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString4096""
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""address1"": {
            ""type"": ""object"",
            ""properties"": {
                ""lang"": {
                    ""type"": [
                        ""string"",
                        ""number"",
                        ""boolean""
                    ],
                    ""anyOf"": [
                        {
                            ""type"": ""string"",
                            ""description"": ""Attempting to install the relevant ISO 2- and 3-letter\n         codes as the enumerated possible values is probably never\n         going to be a realistic possibility.  See\n         RFC 3066 at http://www.ietf.org/rfc/rfc3066.txt and the IANA registry\n         at http://www.iana.org/assignments/lang-tag-apps.htm for\n         further information.\n\n         The union allows for the 'un-declaration' of xml:lang with\n         the empty string.""
                        },
                        {}
                    ]
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                },
                ""sortCode"": {
                    ""$ref"": ""#/definitions/sortCode""
                },
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""addressLine"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/addressLine""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""addressLine1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString80""
                },
                ""keyName"": {
                    ""$ref"": ""#/definitions/keyName1""
                },
                ""keyValue"": {
                    ""$ref"": ""#/definitions/keyValue1""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""assertionStatusItem1"": {
            ""type"": ""object"",
            ""properties"": {
                ""completionStatus"": {
                    ""$ref"": ""#/definitions/completionStatus1""
                },
                ""fromKey"": {
                    ""$ref"": ""#/definitions/fromKey""
                },
                ""toKey"": {
                    ""$ref"": ""#/definitions/toKey""
                },
                ""keyedReference"": {
                    ""$ref"": ""#/definitions/keyedReference""
                },
                ""keysOwned"": {
                    ""$ref"": ""#/definitions/keysOwned""
                }
            },
            ""required"": [
                ""fromKey"",
                ""toKey"",
                ""keyedReference"",
                ""keysOwned""
            ]
        },
        ""bindingTemplate1"": {
            ""type"": ""object"",
            ""properties"": {
                ""bindingKey"": {
                    ""$ref"": ""#/definitions/bindingKey1""
                },
                ""serviceKey"": {
                    ""$ref"": ""#/definitions/serviceKey1""
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""accessPoint"": {
                    ""$ref"": ""#/definitions/accessPoint""
                },
                ""hostingRedirector"": {
                    ""$ref"": ""#/definitions/hostingRedirector""
                },
                ""tModelInstanceDetails"": {
                    ""$ref"": ""#/definitions/tModelInstanceDetails""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""Signature"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Signature""
                    }
                }
            },
            ""required"": [
                ""accessPoint"",
                ""hostingRedirector""
            ]
        },
        ""bindingTemplates1"": {
            ""type"": ""object"",
            ""properties"": {
                ""bindingTemplate"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/bindingTemplate""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""businessEntity1"": {
            ""type"": ""object"",
            ""properties"": {
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey1""
                },
                ""discoveryURLs"": {
                    ""$ref"": ""#/definitions/discoveryURLs""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    },
                    ""minItems"": 1
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""contacts"": {
                    ""$ref"": ""#/definitions/contacts""
                },
                ""businessServices"": {
                    ""$ref"": ""#/definitions/businessServices""
                },
                ""identifierBag"": {
                    ""$ref"": ""#/definitions/identifierBag""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""Signature"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Signature""
                    }
                }
            }
        },
        ""businessInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey1""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    },
                    ""minItems"": 1
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""serviceInfos"": {
                    ""$ref"": ""#/definitions/serviceInfos""
                }
            }
        },
        ""businessInfos1"": {
            ""type"": ""object"",
            ""properties"": {
                ""businessInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessInfo""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""businessService1"": {
            ""type"": ""object"",
            ""properties"": {
                ""serviceKey"": {
                    ""$ref"": ""#/definitions/serviceKey1""
                },
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey1""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    }
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""bindingTemplates"": {
                    ""$ref"": ""#/definitions/bindingTemplates""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""Signature"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Signature""
                    }
                }
            }
        },
        ""businessServices1"": {
            ""type"": ""object"",
            ""properties"": {
                ""businessService"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessService""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""categoryBag1"": {
            ""type"": ""object"",
            ""properties"": {
                ""keyedReference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReference""
                    },
                    ""minItems"": 1
                },
                ""keyedReferenceGroup"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReferenceGroup""
                    }
                },
                ""keyedReferenceGroup1"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReferenceGroup""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""contact1"": {
            ""type"": ""object"",
            ""properties"": {
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""personName"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/personName""
                    },
                    ""minItems"": 1
                },
                ""phone"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/phone""
                    }
                },
                ""email"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/email""
                    }
                },
                ""address"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/address""
                    }
                }
            }
        },
        ""contacts1"": {
            ""type"": ""object"",
            ""properties"": {
                ""contact"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/contact""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""description1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString255""
                },
                ""lang"": {
                    ""type"": [
                        ""string"",
                        ""number"",
                        ""boolean""
                    ],
                    ""anyOf"": [
                        {
                            ""type"": ""string"",
                            ""description"": ""Attempting to install the relevant ISO 2- and 3-letter\n         codes as the enumerated possible values is probably never\n         going to be a realistic possibility.  See\n         RFC 3066 at http://www.ietf.org/rfc/rfc3066.txt and the IANA registry\n         at http://www.iana.org/assignments/lang-tag-apps.htm for\n         further information.\n\n         The union allows for the 'un-declaration' of xml:lang with\n         the empty string.""
                        },
                        {}
                    ]
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""discoveryURL1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeAnyURI4096""
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""discoveryURLs1"": {
            ""type"": ""object"",
            ""properties"": {
                ""discoveryURL"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/discoveryURL""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""dispositionReport1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""result"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/result""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""email1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString255""
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""errInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""type"": ""string""
                },
                ""errCode"": {
                    ""type"": ""string""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""findQualifiers1"": {
            ""type"": ""object"",
            ""properties"": {
                ""findQualifier"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/findQualifier""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""hostingRedirector1"": {
            ""type"": ""object"",
            ""properties"": {
                ""bindingKey"": {
                    ""$ref"": ""#/definitions/bindingKey1""
                }
            }
        },
        ""identifierBag1"": {
            ""type"": ""object"",
            ""properties"": {
                ""keyedReference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReference""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""instanceDetails1"": {
            ""type"": ""object"",
            ""properties"": {
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""overviewDoc"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/overviewDoc""
                    },
                    ""minItems"": 1
                },
                ""instanceParms"": {
                    ""$ref"": ""#/definitions/instanceParms""
                },
                ""instanceParms1"": {
                    ""$ref"": ""#/definitions/instanceParms""
                }
            },
            ""required"": [
                ""instanceParms""
            ]
        },
        ""keyedReference1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""keyName"": {
                    ""$ref"": ""#/definitions/keyName1""
                },
                ""keyValue"": {
                    ""$ref"": ""#/definitions/keyValue1""
                }
            }
        },
        ""keyedReferenceGroup1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""keyedReference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReference""
                    }
                }
            }
        },
        ""keysOwned1"": {
            ""type"": ""object"",
            ""properties"": {
                ""fromKey"": {
                    ""$ref"": ""#/definitions/fromKey""
                },
                ""toKey"": {
                    ""$ref"": ""#/definitions/toKey""
                },
                ""toKey1"": {
                    ""$ref"": ""#/definitions/toKey""
                }
            },
            ""required"": [
                ""fromKey"",
                ""toKey""
            ]
        },
        ""listDescription1"": {
            ""type"": ""object"",
            ""properties"": {
                ""includeCount"": {
                    ""$ref"": ""#/definitions/includeCount""
                },
                ""actualCount"": {
                    ""$ref"": ""#/definitions/actualCount""
                },
                ""listHead"": {
                    ""$ref"": ""#/definitions/listHead""
                }
            },
            ""required"": [
                ""includeCount"",
                ""actualCount"",
                ""listHead""
            ]
        },
        ""name1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString255""
                },
                ""lang"": {
                    ""type"": [
                        ""string"",
                        ""number"",
                        ""boolean""
                    ],
                    ""anyOf"": [
                        {
                            ""type"": ""string"",
                            ""description"": ""Attempting to install the relevant ISO 2- and 3-letter\n         codes as the enumerated possible values is probably never\n         going to be a realistic possibility.  See\n         RFC 3066 at http://www.ietf.org/rfc/rfc3066.txt and the IANA registry\n         at http://www.iana.org/assignments/lang-tag-apps.htm for\n         further information.\n\n         The union allows for the 'un-declaration' of xml:lang with\n         the empty string.""
                        },
                        {}
                    ]
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""operationalInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""entityKey"": {
                    ""$ref"": ""#/definitions/uddiKey""
                },
                ""created"": {
                    ""$ref"": ""#/definitions/timeInstant""
                },
                ""modified"": {
                    ""$ref"": ""#/definitions/timeInstant""
                },
                ""modifiedIncludingChildren"": {
                    ""$ref"": ""#/definitions/timeInstant""
                },
                ""nodeID"": {
                    ""$ref"": ""#/definitions/nodeID""
                },
                ""authorizedName"": {
                    ""$ref"": ""#/definitions/authorizedName""
                }
            }
        },
        ""overviewDoc1"": {
            ""type"": ""object"",
            ""properties"": {
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    },
                    ""minItems"": 1
                },
                ""overviewURL"": {
                    ""$ref"": ""#/definitions/overviewURL""
                },
                ""overviewURL1"": {
                    ""$ref"": ""#/definitions/overviewURL""
                }
            },
            ""required"": [
                ""overviewURL""
            ]
        },
        ""overviewURL1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeAnyURI4096""
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""personName1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString255""
                },
                ""lang"": {
                    ""type"": [
                        ""string"",
                        ""number"",
                        ""boolean""
                    ],
                    ""anyOf"": [
                        {
                            ""type"": ""string"",
                            ""description"": ""Attempting to install the relevant ISO 2- and 3-letter\n         codes as the enumerated possible values is probably never\n         going to be a realistic possibility.  See\n         RFC 3066 at http://www.ietf.org/rfc/rfc3066.txt and the IANA registry\n         at http://www.iana.org/assignments/lang-tag-apps.htm for\n         further information.\n\n         The union allows for the 'un-declaration' of xml:lang with\n         the empty string.""
                        },
                        {}
                    ]
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""phone1"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""$ref"": ""#/definitions/validationTypeString50""
                },
                ""useType"": {
                    ""$ref"": ""#/definitions/useType""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""publisherAssertion1"": {
            ""type"": ""object"",
            ""properties"": {
                ""fromKey"": {
                    ""$ref"": ""#/definitions/fromKey""
                },
                ""toKey"": {
                    ""$ref"": ""#/definitions/toKey""
                },
                ""keyedReference"": {
                    ""$ref"": ""#/definitions/keyedReference""
                },
                ""Signature"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Signature""
                    }
                }
            },
            ""required"": [
                ""fromKey"",
                ""toKey"",
                ""keyedReference""
            ]
        },
        ""relatedBusinessInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    },
                    ""minItems"": 1
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""sharedRelationships"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/sharedRelationships""
                    },
                    ""maxItems"": 2,
                    ""minItems"": 1
                }
            },
            ""required"": [
                ""businessKey""
            ]
        },
        ""relatedBusinessInfos1"": {
            ""type"": ""object"",
            ""properties"": {
                ""relatedBusinessInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/relatedBusinessInfo""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""result1"": {
            ""type"": ""object"",
            ""properties"": {
                ""keyType"": {
                    ""$ref"": ""#/definitions/keyType""
                },
                ""errno"": {
                    ""type"": ""integer""
                },
                ""errInfo"": {
                    ""$ref"": ""#/definitions/errInfo""
                }
            }
        },
        ""serviceInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""serviceKey"": {
                    ""$ref"": ""#/definitions/serviceKey1""
                },
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey1""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    }
                }
            }
        },
        ""serviceInfos1"": {
            ""type"": ""object"",
            ""properties"": {
                ""serviceInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/serviceInfo""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""sharedRelationships1"": {
            ""type"": ""object"",
            ""properties"": {
                ""direction"": {
                    ""$ref"": ""#/definitions/direction""
                },
                ""keyedReference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/keyedReference""
                    },
                    ""minItems"": 1
                },
                ""publisherAssertion"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/publisherAssertion""
                    }
                }
            }
        },
        ""tModel1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""deleted"": {
                    ""$ref"": ""#/definitions/deleted""
                },
                ""name"": {
                    ""$ref"": ""#/definitions/name""
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""overviewDoc"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/overviewDoc""
                    }
                },
                ""identifierBag"": {
                    ""$ref"": ""#/definitions/identifierBag""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""Signature"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Signature""
                    }
                }
            },
            ""required"": [
                ""name""
            ]
        },
        ""tModelBag1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModelKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""tModelInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""name"": {
                    ""$ref"": ""#/definitions/name""
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                }
            },
            ""required"": [
                ""name""
            ]
        },
        ""tModelInfos1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModelInfo""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""tModelInstanceDetails1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelInstanceInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModelInstanceInfo""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""tModelInstanceInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""tModelKey"": {
                    ""$ref"": ""#/definitions/tModelKey1""
                },
                ""description"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/description""
                    }
                },
                ""instanceDetails"": {
                    ""$ref"": ""#/definitions/instanceDetails""
                }
            }
        },
        ""add_publisherAssertions1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""publisherAssertion"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/publisherAssertion""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""delete_binding1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""bindingKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/bindingKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""delete_business1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""businessKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""delete_publisherAssertions1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""publisherAssertion"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/publisherAssertion""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""delete_service1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""serviceKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/serviceKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""delete_tModel1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""tModelKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModelKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""discard_authToken1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                }
            },
            ""required"": [
                ""authInfo""
            ]
        },
        ""find_binding1"": {
            ""type"": ""object"",
            ""properties"": {
                ""maxRows"": {
                    ""type"": ""integer""
                },
                ""serviceKey"": {
                    ""$ref"": ""#/definitions/serviceKey1""
                },
                ""listHead"": {
                    ""type"": ""integer""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""findQualifiers"": {
                    ""$ref"": ""#/definitions/findQualifiers""
                },
                ""tModelBag"": {
                    ""$ref"": ""#/definitions/tModelBag""
                },
                ""find_tModel"": {
                    ""$ref"": ""#/definitions/find_tModel""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                }
            }
        },
        ""find_business1"": {
            ""type"": ""object"",
            ""properties"": {
                ""maxRows"": {
                    ""type"": ""integer""
                },
                ""listHead"": {
                    ""type"": ""integer""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""findQualifiers"": {
                    ""$ref"": ""#/definitions/findQualifiers""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    }
                },
                ""identifierBag"": {
                    ""$ref"": ""#/definitions/identifierBag""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""tModelBag"": {
                    ""$ref"": ""#/definitions/tModelBag""
                },
                ""find_tModel"": {
                    ""$ref"": ""#/definitions/find_tModel""
                },
                ""discoveryURLs"": {
                    ""$ref"": ""#/definitions/discoveryURLs""
                },
                ""find_relatedBusinesses"": {
                    ""$ref"": ""#/definitions/find_relatedBusinesses""
                }
            }
        },
        ""find_relatedBusinesses1"": {
            ""type"": ""object"",
            ""properties"": {
                ""maxRows"": {
                    ""type"": ""integer""
                },
                ""listHead"": {
                    ""type"": ""integer""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""findQualifiers"": {
                    ""$ref"": ""#/definitions/findQualifiers""
                },
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey""
                },
                ""fromKey"": {
                    ""$ref"": ""#/definitions/fromKey""
                },
                ""toKey"": {
                    ""$ref"": ""#/definitions/toKey""
                },
                ""keyedReference"": {
                    ""$ref"": ""#/definitions/keyedReference""
                }
            },
            ""required"": [
                ""businessKey"",
                ""fromKey"",
                ""toKey""
            ]
        },
        ""find_service1"": {
            ""type"": ""object"",
            ""properties"": {
                ""maxRows"": {
                    ""type"": ""integer""
                },
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey1""
                },
                ""listHead"": {
                    ""type"": ""integer""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""findQualifiers"": {
                    ""$ref"": ""#/definitions/findQualifiers""
                },
                ""name"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/name""
                    }
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                },
                ""tModelBag"": {
                    ""$ref"": ""#/definitions/tModelBag""
                },
                ""find_tModel"": {
                    ""$ref"": ""#/definitions/find_tModel""
                }
            }
        },
        ""find_tModel1"": {
            ""type"": ""object"",
            ""properties"": {
                ""maxRows"": {
                    ""type"": ""integer""
                },
                ""listHead"": {
                    ""type"": ""integer""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""findQualifiers"": {
                    ""$ref"": ""#/definitions/findQualifiers""
                },
                ""name"": {
                    ""$ref"": ""#/definitions/name""
                },
                ""identifierBag"": {
                    ""$ref"": ""#/definitions/identifierBag""
                },
                ""categoryBag"": {
                    ""$ref"": ""#/definitions/categoryBag""
                }
            }
        },
        ""get_assertionStatusReport1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""completionStatus"": {
                    ""$ref"": ""#/definitions/completionStatus""
                }
            }
        },
        ""get_authToken1"": {
            ""type"": ""object"",
            ""properties"": {
                ""userID"": {
                    ""type"": ""string""
                },
                ""cred"": {
                    ""type"": ""string""
                }
            }
        },
        ""get_bindingDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""bindingKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/bindingKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""get_businessDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""businessKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""get_operationalInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""entityKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/uddiKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""get_publisherAssertions1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                }
            }
        },
        ""get_registeredInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""infoSelection"": {
                    ""$ref"": ""#/definitions/infoSelection1""
                },
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                }
            }
        },
        ""get_serviceDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""serviceKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/serviceKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""get_tModelDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""tModelKey"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModelKey""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""save_binding1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""bindingTemplate"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/bindingTemplate""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""save_business1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""businessEntity"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessEntity""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""save_service1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""businessService"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessService""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""save_tModel1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""tModel"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModel""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""set_publisherAssertions1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                },
                ""publisherAssertion"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/publisherAssertion""
                    }
                }
            }
        },
        ""assertionStatusReport1"": {
            ""type"": ""object"",
            ""properties"": {
                ""assertionStatusItem"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/assertionStatusItem""
                    }
                }
            }
        },
        ""authToken1"": {
            ""type"": ""object"",
            ""properties"": {
                ""authInfo"": {
                    ""$ref"": ""#/definitions/authInfo""
                }
            },
            ""required"": [
                ""authInfo""
            ]
        },
        ""bindingDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""listDescription"": {
                    ""$ref"": ""#/definitions/listDescription""
                },
                ""bindingTemplate"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/bindingTemplate""
                    }
                }
            }
        },
        ""businessDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""businessEntity"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessEntity""
                    }
                }
            }
        },
        ""businessList1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""listDescription"": {
                    ""$ref"": ""#/definitions/listDescription""
                },
                ""businessInfos"": {
                    ""$ref"": ""#/definitions/businessInfos""
                }
            }
        },
        ""operationalInfos1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""operationalInfo"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/operationalInfo""
                    }
                }
            }
        },
        ""publisherAssertions1"": {
            ""type"": ""object"",
            ""properties"": {
                ""publisherAssertion"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/publisherAssertion""
                    }
                }
            }
        },
        ""registeredInfo1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""businessInfos"": {
                    ""$ref"": ""#/definitions/businessInfos""
                },
                ""tModelInfos"": {
                    ""$ref"": ""#/definitions/tModelInfos""
                }
            }
        },
        ""relatedBusinessesList1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""listDescription"": {
                    ""$ref"": ""#/definitions/listDescription""
                },
                ""businessKey"": {
                    ""$ref"": ""#/definitions/businessKey""
                },
                ""relatedBusinessInfos"": {
                    ""$ref"": ""#/definitions/relatedBusinessInfos""
                }
            },
            ""required"": [
                ""businessKey""
            ]
        },
        ""serviceDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""businessService"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/businessService""
                    }
                }
            }
        },
        ""serviceList1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""listDescription"": {
                    ""$ref"": ""#/definitions/listDescription""
                },
                ""serviceInfos"": {
                    ""$ref"": ""#/definitions/serviceInfos""
                }
            }
        },
        ""tModelDetail1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""tModel"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/tModel""
                    }
                }
            }
        },
        ""tModelList1"": {
            ""type"": ""object"",
            ""properties"": {
                ""truncated"": {
                    ""$ref"": ""#/definitions/truncated""
                },
                ""listDescription"": {
                    ""$ref"": ""#/definitions/listDescription""
                },
                ""tModelInfos"": {
                    ""$ref"": ""#/definitions/tModelInfos""
                }
            }
        },
        ""SignatureType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""SignedInfo"": {
                    ""$ref"": ""#/definitions/SignedInfo""
                },
                ""SignatureValue"": {
                    ""$ref"": ""#/definitions/SignatureValue""
                },
                ""KeyInfo"": {
                    ""$ref"": ""#/definitions/KeyInfo""
                },
                ""Object"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Object""
                    }
                }
            },
            ""required"": [
                ""SignedInfo"",
                ""SignatureValue""
            ]
        },
        ""SignatureValueType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Value"": {
                    ""type"": ""string""
                },
                ""Id"": {
                    ""type"": ""string""
                }
            },
            ""required"": [
                ""Value""
            ]
        },
        ""SignedInfoType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""CanonicalizationMethod"": {
                    ""$ref"": ""#/definitions/CanonicalizationMethod""
                },
                ""SignatureMethod"": {
                    ""$ref"": ""#/definitions/SignatureMethod""
                },
                ""Reference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Reference""
                    },
                    ""minItems"": 1
                }
            },
            ""required"": [
                ""CanonicalizationMethod"",
                ""SignatureMethod""
            ]
        },
        ""CanonicalizationMethodType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Algorithm"": {
                    ""type"": ""string""
                }
            }
        },
        ""SignatureMethodType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Algorithm"": {
                    ""type"": ""string""
                },
                ""HMACOutputLength"": {
                    ""$ref"": ""#/definitions/HMACOutputLengthType""
                }
            }
        },
        ""ReferenceType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""URI"": {
                    ""type"": ""string""
                },
                ""Type"": {
                    ""type"": ""string""
                },
                ""Transforms"": {
                    ""$ref"": ""#/definitions/Transforms""
                },
                ""DigestMethod"": {
                    ""$ref"": ""#/definitions/DigestMethod""
                },
                ""DigestValue"": {
                    ""$ref"": ""#/definitions/DigestValue""
                }
            },
            ""required"": [
                ""DigestMethod"",
                ""DigestValue""
            ]
        },
        ""TransformsType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Transform"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Transform""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""TransformType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Algorithm"": {
                    ""type"": ""string""
                },
                ""XPath"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    }
                }
            }
        },
        ""DigestMethodType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Algorithm"": {
                    ""type"": ""string""
                }
            }
        },
        ""KeyInfoType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""KeyName"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/KeyName""
                    },
                    ""minItems"": 1
                },
                ""KeyValue"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/KeyValue""
                    },
                    ""minItems"": 1
                },
                ""RetrievalMethod"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/RetrievalMethod""
                    },
                    ""minItems"": 1
                },
                ""X509Data"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/X509Data""
                    },
                    ""minItems"": 1
                },
                ""PGPData"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/PGPData""
                    },
                    ""minItems"": 1
                },
                ""SPKIData"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/SPKIData""
                    },
                    ""minItems"": 1
                },
                ""MgmtData"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/MgmtData""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""KeyValueType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""DSAKeyValue"": {
                    ""$ref"": ""#/definitions/DSAKeyValue""
                },
                ""RSAKeyValue"": {
                    ""$ref"": ""#/definitions/RSAKeyValue""
                }
            },
            ""required"": [
                ""DSAKeyValue"",
                ""RSAKeyValue""
            ]
        },
        ""RetrievalMethodType"": {
            ""type"": ""object"",
            ""properties"": {
                ""URI"": {
                    ""type"": ""string""
                },
                ""Type"": {
                    ""type"": ""string""
                },
                ""Transforms"": {
                    ""$ref"": ""#/definitions/Transforms""
                }
            }
        },
        ""X509DataType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""X509IssuerSerial"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/X509IssuerSerialType""
                    },
                    ""minItems"": 1
                },
                ""X509SKI"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    },
                    ""minItems"": 1
                },
                ""X509SubjectName"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    },
                    ""minItems"": 1
                },
                ""X509Certificate"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    },
                    ""minItems"": 1
                },
                ""X509CRL"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""X509IssuerSerialType"": {
            ""type"": ""object"",
            ""properties"": {
                ""X509IssuerName"": {
                    ""type"": ""string""
                },
                ""X509SerialNumber"": {
                    ""type"": ""integer""
                }
            },
            ""required"": [
                ""X509IssuerName"",
                ""X509SerialNumber""
            ]
        },
        ""PGPDataType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""PGPKeyID"": {
                    ""type"": ""string""
                },
                ""PGPKeyPacket"": {
                    ""type"": ""string""
                },
                ""PGPKeyPacket1"": {
                    ""type"": ""string""
                }
            },
            ""required"": [
                ""PGPKeyID"",
                ""PGPKeyPacket""
            ]
        },
        ""SPKIDataType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""SPKISexp"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""type"": ""string""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""ObjectType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""MimeType"": {
                    ""type"": ""string""
                },
                ""Encoding"": {
                    ""type"": ""string""
                }
            }
        },
        ""ManifestType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""Reference"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/Reference""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""SignaturePropertiesType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Id"": {
                    ""type"": ""string""
                },
                ""SignatureProperty"": {
                    ""type"": ""array"",
                    ""additionalItems"": {
                        ""$ref"": ""#/definitions/SignatureProperty""
                    },
                    ""minItems"": 1
                }
            }
        },
        ""SignaturePropertyType"": {
            ""type"": ""object"",
            ""additionalProperties"": true,
            ""properties"": {
                ""Target"": {
                    ""type"": ""string""
                },
                ""Id"": {
                    ""type"": ""string""
                }
            }
        },
        ""DSAKeyValueType"": {
            ""type"": ""object"",
            ""properties"": {
                ""P"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""Q"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""G"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""Y"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""J"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""Seed"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""PgenCounter"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                }
            },
            ""required"": [
                ""Y""
            ]
        },
        ""RSAKeyValueType"": {
            ""type"": ""object"",
            ""properties"": {
                ""Modulus"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                },
                ""Exponent"": {
                    ""$ref"": ""#/definitions/CryptoBinary""
                }
            },
            ""required"": [
                ""Modulus"",
                ""Exponent""
            ]
        },
        ""uddiKey"": {
            ""type"": ""string"",
            ""maxLength"": 255
        },
        ""bindingKey1"": {
            ""$ref"": ""#/definitions/uddiKey""
        },
        ""businessKey1"": {
            ""$ref"": ""#/definitions/uddiKey""
        },
        ""findQualifier1"": {
            ""$ref"": ""#/definitions/validationTypeString255""
        },
        ""serviceKey1"": {
            ""$ref"": ""#/definitions/uddiKey""
        },
        ""tModelKey1"": {
            ""$ref"": ""#/definitions/uddiKey""
        },
        ""authorizedName1"": {
            ""type"": ""string"",
            ""maxLength"": 255
        },
        ""nodeID1"": {
            ""$ref"": ""#/definitions/uddiKey""
        },
        ""completionStatus1"": {
            ""type"": ""string"",
            ""enum"": [
                ""status:complete"",
                ""status:fromKey_incomplete"",
                ""status:toKey_incomplete"",
                ""status:both_incomplete""
            ],
            ""maxLength"": 32
        },
        ""direction"": {
            ""type"": ""string"",
            ""enum"": [
                ""fromKey"",
                ""toKey""
            ]
        },
        ""instanceParms1"": {
            ""$ref"": ""#/definitions/validationTypeString8192""
        },
        ""deleted"": {
            ""type"": ""boolean""
        },
        ""timeInstant"": {
            ""type"": ""string""
        },
        ""truncated"": {
            ""type"": ""boolean""
        },
        ""keyName1"": {
            ""type"": ""string"",
            ""maxLength"": 255
        },
        ""keyType"": {
            ""type"": ""string"",
            ""enum"": [
                ""businessKey"",
                ""tModelKey"",
                ""serviceKey"",
                ""bindingKey"",
                ""subscriptionKey""
            ]
        },
        ""keyValue1"": {
            ""type"": ""string"",
            ""maxLength"": 255
        },
        ""sortCode"": {
            ""type"": ""string"",
            ""maxLength"": 10
        },
        ""useType"": {
            ""type"": ""string"",
            ""maxLength"": 255
        },
        ""infoSelection1"": {
            ""type"": ""string"",
            ""enum"": [
                ""all"",
                ""hidden"",
                ""visible""
            ]
        },
        ""validationTypeAnyURI4096"": {
            ""type"": ""string"",
            ""maxLength"": 4096,
            ""minLength"": 1
        },
        ""validationTypeString50"": {
            ""type"": ""string"",
            ""maxLength"": 50,
            ""minLength"": 1
        },
        ""validationTypeString80"": {
            ""type"": ""string"",
            ""maxLength"": 80,
            ""minLength"": 1
        },
        ""validationTypeString255"": {
            ""type"": ""string"",
            ""maxLength"": 255,
            ""minLength"": 1
        },
        ""validationTypeString4096"": {
            ""type"": ""string"",
            ""maxLength"": 4096,
            ""minLength"": 1
        },
        ""validationTypeString8192"": {
            ""type"": ""string"",
            ""maxLength"": 8192,
            ""minLength"": 1
        },
        ""CryptoBinary"": {
            ""type"": ""string""
        },
        ""DigestValueType"": {
            ""type"": ""string""
        },
        ""HMACOutputLengthType"": {
            ""type"": ""integer""
        }
    }
}";
    }
}