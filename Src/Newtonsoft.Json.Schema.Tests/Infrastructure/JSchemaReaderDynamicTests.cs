#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using System.Text;
using System.Linq;

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaReaderDynamicTests : TestFixtureBase
    {
        [Test]
        public void RecursiveAnchor_RefcursiveRefToRoot()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://json-schema.org/draft/2019-09/meta/core"",
                ""$vocabulary"": {
                    ""https://json-schema.org/draft/2019-09/vocab/core"": true
                },
                ""$recursiveAnchor"": true,

                ""title"": ""Core vocabulary meta-schema"",
                ""type"": [""object"", ""boolean""],
                ""properties"": {
                    ""$defs"": {
                        ""type"": ""object"",
                        ""additionalProperties"": { ""$recursiveRef"": ""#"" },
                        ""default"": {}
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual(s, s.Properties["$defs"].AdditionalProperties);
        }

        [Test]
        public void RecursiveAnchor_Nested()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$recursiveAnchor"": true,

                ""allOf"": [
                    {""$ref"": ""meta/core""}
                ]
            }";

            string coreJson = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://json-schema.org/draft/2019-09/meta/core"",
                ""$recursiveAnchor"": true,

                ""properties"": {
                    ""$defs"": {
                        ""additionalProperties"": { ""$recursiveRef"": ""#"" },
                    }
                }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("https://json-schema.org/draft/2019-09/meta/core"), coreJson);

            JSchema s = JSchema.Parse(json, resolver);
            Assert.AreEqual(s, s.AllOf[0].Properties["$defs"].AdditionalProperties);
        }

        [Test]
        public void RecursiveRef()
        {
            string json = @"{
                ""allOf"": [{
                    ""$recursiveRef"": ""#""
                }]
            }";

            JSchema s = JSchema.Parse(json);

            Assert.AreEqual(s, s.AllOf[0]);
        }

        [Test]
        public void RecursiveAnchor()
        {
            string json = @"{
                ""$recursiveAnchor"": false
            }";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual(false, s.RecursiveAnchor);
        }

        [Test]
        public void RecursiveAnchor_OnBothLevels_OuterSchemaResolved()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://example.com/root"",
                ""$recursiveAnchor"": true,
                ""properties"": {
                    ""prop1"": { ""$ref"": ""tree"" }
                }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("https://example.com/tree", UriKind.RelativeOrAbsolute), @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://example.com/tree"",
                ""$recursiveAnchor"": true,

                ""type"": ""object"",
                ""properties"": {
                    ""data"": true,
                    ""children"": {
                        ""type"": ""array"",
                        ""items"": {
                            ""$recursiveRef"": ""#""
                        }
                    }
                }
            }");

            JSchema s = JSchema.Parse(json, resolver);
            Assert.AreEqual(true, s.RecursiveAnchor);

            JSchema prop1 = s.Properties["prop1"];

            Assert.AreEqual("https://example.com/tree", prop1.Id.OriginalString);
            Assert.AreEqual(true, prop1.RecursiveAnchor);

            JSchema items = prop1.Properties["children"].Items[0];
            Assert.AreEqual(s, items);
        }

        [Test]
        public void RecursiveAnchor_SingleLevel_InnerSchemaResolved()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://example.com/root"",

                ""properties"": {
                    ""prop1"": { ""$ref"": ""tree"" }
                }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("https://example.com/tree", UriKind.RelativeOrAbsolute), @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://example.com/tree"",
                ""$recursiveAnchor"": true,

                ""type"": ""object"",
                ""properties"": {
                    ""data"": true,
                    ""children"": {
                        ""type"": ""array"",
                        ""items"": {
                            ""$recursiveRef"": ""#""
                        }
                    }
                }
            }");

            JSchema s = JSchema.Parse(json, resolver);
            JSchema prop1 = s.Properties["prop1"];

            Assert.AreEqual("https://example.com/tree", prop1.Id.OriginalString);
            Assert.AreEqual(true, prop1.RecursiveAnchor);

            JSchema items = prop1.Properties["children"].Items[0];
            Assert.AreEqual(prop1, items);
        }

        [Test]
        public void RecursiveRef_RecursiveAnchorFalse()
        {
            string schemaJson = @"{
    ""$id"": ""http://localhost:4242/recursiveRef4/schema.json"",
    ""$recursiveAnchor"": false,
    ""$defs"": {
        ""myobject"": {
            ""$id"": ""myobject.json"",
            ""$recursiveAnchor"": false,
            ""anyOf"": [
                { ""type"": ""string"" },
                {
                    ""type"": ""object"",
                    ""additionalProperties"": { ""$recursiveRef"": ""#"" }
                }
            ]
        }
    },
    ""anyOf"": [
        { ""type"": ""integer"" },
        { ""$ref"": ""#/$defs/myobject"" }
    ]
}";

            JSchema s = JSchema.Parse(schemaJson);
            JSchema myObject = s.AnyOf[1];
            Assert.AreEqual("myobject.json", myObject.Id.OriginalString);
            Assert.AreEqual(false, myObject.RecursiveAnchor);
            Assert.AreEqual(myObject, myObject.AnyOf[1].AdditionalProperties);
        }

        [Test]
        public void RecursiveRef_DynamicSelection()
        {
            string schemaJson = @"{
    ""$id"": ""recursiveRef8_main.json"",
    ""$defs"": {
        ""inner"": {
            ""$id"": ""recursiveRef8_inner.json"",
            ""$recursiveAnchor"": true,
            ""title"": ""inner"",
            ""additionalProperties"": {
                ""$recursiveRef"": ""#""
            }
        }
    },
    ""if"": {
        ""propertyNames"": {
            ""pattern"": ""^[a-m]""
        }
    },
    ""then"": {
        ""title"": ""any type of node"",
        ""$id"": ""recursiveRef8_anyLeafNode.json"",
        ""$recursiveAnchor"": true,
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner""
    },
    ""else"": {
        ""title"": ""integer node"",
        ""$id"": ""recursiveRef8_integerNode.json"",
        ""$recursiveAnchor"": true,
        ""type"": [ ""object"", ""integer"" ],
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner""
    }
}";

            JSchema s = JSchema.Parse(schemaJson);
            Assert.AreEqual("recursiveRef8_main.json", s.Id.OriginalString);

            Assert.AreEqual("recursiveRef8_anyLeafNode.json", s.Then.Id.OriginalString);
            Assert.AreEqual("recursiveRef8_anyLeafNode.json", s.Then.Ref.AdditionalProperties.Id.OriginalString);

            Assert.AreEqual("recursiveRef8_integerNode.json", s.Else.Id.OriginalString);
            Assert.AreEqual("recursiveRef8_integerNode.json", s.Else.Ref.AdditionalProperties.Id.OriginalString);
        }

        [Test]
        public void RecursiveAnchor_Ref()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""$id"": ""https://json-schema.org/draft/2019-09/meta/applicator"",
                ""$recursiveAnchor"": true,

                ""title"": ""Applicator vocabulary meta-schema"",
                ""type"": [""object"", ""boolean""],
                ""properties"": {
                    ""items"": {
                        ""anyOf"": [
                            { ""$ref"": ""#/$defs/schemaArray"" }
                        ]
                    }
                },
                ""$defs"": {
                    ""schemaArray"": {
                        ""type"": ""array"",
                        ""minItems"": 1
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual(JSchemaType.Array, s.Properties["items"].AnyOf[0].Type);
        }

        [Test]
        public void DynamicAnchor()
        {
            string json = @"{
                ""$dynamicAnchor"": ""node""
            }";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual("node", s.DynamicAnchor);
            Assert.AreEqual(null, s.RecursiveAnchor);
        }

        [Test]
        public void DynamicAnchorRef()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""http://localhost:1234/draft2020-12/strict-tree.json"",
                ""$dynamicAnchor"": ""node"",

                ""$ref"": ""tree.json"",
                ""unevaluatedProperties"": false
            }";

            string treeJson = @"{
              ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
              ""$id"": ""https://example.com/tree"",
              ""$dynamicAnchor"": ""node"",
              ""type"": ""object"",
              ""properties"": {
                ""data"": true,
                ""children"": {
                  ""type"": ""array"",
                  ""items"": { ""$dynamicRef"": ""#node"" }
                }
              }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://localhost:1234/draft2020-12/tree.json"), treeJson);

            JSchema s = JSchema.Parse(json, resolver);

            Assert.AreEqual("http://localhost:1234/draft2020-12/strict-tree.json", s.Id.OriginalString);
            Assert.AreEqual("node", s.DynamicAnchor);
            Assert.AreEqual(false, s.AllowUnevaluatedProperties);

            Assert.AreEqual("https://example.com/tree", s.Ref.Id.OriginalString);
            Assert.AreEqual("node", s.Ref.DynamicAnchor);

            JSchema children = s.Ref.Properties["children"];
            Assert.AreEqual(JSchemaType.Array, children.Type);
            Assert.AreEqual("http://localhost:1234/draft2020-12/strict-tree.json", children.AdditionalItems.Id.OriginalString);
        }

        [Test]
        public void RecursiveAnchorRef()
        {
            string json = @"{
              ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
              ""$id"": ""http://localhost:1234/2019-09/strict-tree.json"",
              ""$recursiveAnchor"": true,
              ""$ref"": ""tree.json"",
              ""unevaluatedProperties"": false
            }";

            string treeJson = @"{
              ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
              ""$id"": ""https://example.com/tree"",
              ""$recursiveAnchor"": true,
              ""type"": ""object"",
              ""properties"": {
                ""data"": true,
                ""children"": {
                  ""type"": ""array"",
                  ""items"": { ""$recursiveRef"": ""#"" }
                }
              }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://localhost:1234/2019-09/tree.json"), treeJson);

            JSchema s = JSchema.Parse(json, resolver);

            Assert.AreEqual("http://localhost:1234/2019-09/strict-tree.json", s.Id.OriginalString);
            Assert.AreEqual(true, s.RecursiveAnchor);
            Assert.AreEqual(false, s.AllowUnevaluatedProperties);

            Assert.AreEqual("https://example.com/tree", s.Ref.Id.OriginalString);
            Assert.AreEqual(true, s.Ref.RecursiveAnchor);

            JSchema children = s.Ref.Properties["children"];
            Assert.AreEqual(JSchemaType.Array, children.Type);
            Assert.AreEqual("http://localhost:1234/2019-09/strict-tree.json", children.Items[0].Id.OriginalString);
        }

        [Test]
        public void RefToDynamicRefFindsDetachedDynamicAnchor()
        {
            string json = @"{
              ""$ref"": ""http://localhost:1234/draft2020-12/detached-dynamicref.json#/$defs/foo""
            }";

            string detachedJson = @"{
              ""$id"": ""http://localhost:1234/draft2020-12/detached-dynamicref.json"",
              ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
              ""$defs"": {
                ""foo"": {
                  ""$dynamicRef"": ""#detached""
                },
                ""detached"": {
                  ""$dynamicAnchor"": ""detached"",
                  ""type"": ""integer""
                }
              }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://localhost:1234/draft2020-12/detached-dynamicref.json"), detachedJson);

            JSchema s = JSchema.Parse(json, resolver);
            Assert.AreEqual("detached", s.DynamicAnchor);
            Assert.AreEqual(JSchemaType.Integer, s.Type);
        }

        [Test]
        public void DynamicRef_ResolvesToTheFirstDynamicAnchorInTheDynamicScope()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/relative-dynamic-reference/root"",
                ""$dynamicAnchor"": ""meta"",
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""const"": ""pass"" }
                },
                ""$ref"": ""extended"",
                ""$defs"": {
                    ""extended"": {
                        ""$id"": ""extended"",
                        ""$dynamicAnchor"": ""meta"",
                        ""type"": ""object"",
                        ""properties"": {
                            ""bar"": { ""$ref"": ""bar"" }
                        }
                    },
                    ""bar"": {
                        ""$id"": ""bar"",
                        ""type"": ""object"",
                        ""properties"": {
                            ""baz"": { ""$dynamicRef"": ""extended#meta"" }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
        }

        [Test]
        public void AnchorRoot_RefPath()
        {
            string json = @"{
    ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
    ""$id"": ""https://json-schema.org/draft/2020-12/meta/core"",
    ""$anchor"": ""meta"",
    ""properties"": {
        ""prop1"": {
            ""$ref"": ""#/$defs/uriReferenceString""
        }
    },
    ""$defs"": {
        ""uriReferenceString"": {
            ""type"": ""string"",
            ""format"": ""uri-reference""
        }
    }
}";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual(JSchemaType.String, s.Properties["prop1"].Type);
        }

        [Test]
        public void UseFirstDynamicAnchor()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/typical-dynamic-resolution/root"",
                ""$ref"": ""list"",
                ""$defs"": {
                    ""foo"": {
                        ""$dynamicAnchor"": ""items"",
                        ""type"": ""string""
                    },
                    ""list"": {
                        ""$id"": ""list"",
                        ""type"": ""array"",
                        ""items"": { ""$dynamicRef"": ""#items"" },
                        ""$defs"": {
                          ""items"": {
                              ""$comment"": ""This is only needed to satisfy the bookending requirement"",
                              ""$dynamicAnchor"": ""items""
                          }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
            Assert.AreEqual(JSchemaType.String, s.Ref.AdditionalItems.Type);

            JArray a = JArray.Parse(@"[""foo"", 42]");
            Assert.IsFalse(a.IsValid(s));
        }

        [Test]
        public void DynamicRefWithNonMatchingDynamicAnchor()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/unmatched-dynamic-anchor/root"",
                ""$ref"": ""list"",
                ""$defs"": {
                    ""foo"": {
                        ""$dynamicAnchor"": ""items"",
                        ""type"": ""string""
                    },
                    ""list"": {
                        ""$id"": ""list"",
                        ""type"": ""array"",
                        ""items"": { ""$dynamicRef"": ""#items"" },
                        ""$defs"": {
                            ""items"": {
                                ""$comment"": ""This is only needed to give the reference somewhere to resolve to when it behaves like $ref"",
                                ""$anchor"": ""items"",
                                ""$dynamicAnchor"": ""foo""
                            }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            JArray a = JArray.Parse(@"[""foo"", 42]");

            bool valid = a.IsValid(s, out IList<string> errorMessages);
            Assert.IsTrue(valid, "Should be valid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefAnchorWithSameNameAsDynamicAnchorNotUsed()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/dynamic-resolution-ignores-anchors/root"",
                ""$ref"": ""list"",
                ""$defs"": {
                    ""foo"": {
                        ""$anchor"": ""items"",
                        ""type"": ""string""
                    },
                    ""list"": {
                        ""$id"": ""list"",
                        ""type"": ""array"",
                        ""items"": { ""$dynamicRef"": ""#items"" },
                        ""$defs"": {
                          ""items"": {
                              ""$comment"": ""This is only needed to satisfy the bookending requirement"",
                              ""$dynamicAnchor"": ""items""
                          }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            JArray a = JArray.Parse(@"[""foo"", 42]");

            bool valid = a.IsValid(s, out IList<string> errorMessages);
            Assert.IsTrue(valid, "Should be valid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefAfterLeavingDynamicScope()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/dynamic-ref-leaving-dynamic-scope/main"",
                ""if"": {
                    ""$id"": ""first_scope"",
                    ""$defs"": {
                        ""thingy"": {
                            ""$comment"": ""this is first_scope#thingy"",
                            ""$dynamicAnchor"": ""thingy"",
                            ""type"": ""number""
                        }
                    }
                },
                ""then"": {
                    ""$id"": ""second_scope"",
                    ""$ref"": ""start"",
                    ""$defs"": {
                        ""thingy"": {
                            ""$comment"": ""this is second_scope#thingy, the final destination of the $dynamicRef"",
                            ""$dynamicAnchor"": ""thingy"",
                            ""type"": ""null""
                        }
                    }
                },
                ""$defs"": {
                    ""start"": {
                        ""$comment"": ""this is the landing spot from $ref"",
                        ""$id"": ""start"",
                        ""$dynamicRef"": ""inner_scope#thingy""
                    },
                    ""thingy"": {
                        ""$comment"": ""this is the first stop for the $dynamicRef"",
                        ""$id"": ""inner_scope"",
                        ""$dynamicAnchor"": ""thingy"",
                        ""type"": ""string""
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            JToken stringToken = new JValue("a string");

            bool valid = stringToken.IsValid(s, out IList<string> errorMessages);
            Assert.IsFalse(valid, "Should be invalid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefMultipleDynamicPaths()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/dynamic-ref-with-multiple-paths/main"",
                ""if"": {
                    ""properties"": {
                        ""kindOfList"": { ""const"": ""numbers"" }
                    },
                    ""required"": [""kindOfList""]
                },
                ""then"": { ""$ref"": ""numberList"" },
                ""else"": { ""$ref"": ""stringList"" },

                ""$defs"": {
                    ""genericList"": {
                        ""$id"": ""genericList"",
                        ""properties"": {
                            ""list"": {
                                ""items"": { ""$dynamicRef"": ""#itemType"" }
                            }
                        },
                        ""$defs"": {
                            ""defaultItemType"": {
                                ""$comment"": ""Only needed to satisfy bookending requirement"",
                                ""$dynamicAnchor"": ""itemType""
                            }
                        }
                    },
                    ""numberList"": {
                        ""$id"": ""numberList"",
                        ""$defs"": {
                            ""itemType"": {
                                ""$dynamicAnchor"": ""itemType"",
                                ""type"": ""number""
                            }
                        },
                        ""$ref"": ""genericList""
                    },
                    ""stringList"": {
                        ""$id"": ""stringList"",
                        ""$defs"": {
                            ""itemType"": {
                                ""$dynamicAnchor"": ""itemType"",
                                ""type"": ""string""
                            }
                        },
                        ""$ref"": ""genericList""
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            Assert.AreNotSame(s.Then.Ref, s.Else.Ref);
            Assert.AreEqual(JSchemaType.Number, s.Then.Ref.Properties["list"].AdditionalItems.Type);
            Assert.AreEqual(JSchemaType.String, s.Else.Ref.Properties["list"].AdditionalItems.Type);

            JObject data = JObject.Parse(@"{
                ""kindOfList"": ""strings"",
                ""list"": [1.1]
            }");

            bool valid = data.IsValid(s, out IList<string> errorMessages);
            Assert.IsFalse(valid, "Should be invalid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefWithAnchorInAllOfRefs()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""http://localhost:1234/draft2020-12/strict-extendible-allof-defs-first.json"",
                ""allOf"": [
                    {
                        ""$ref"": ""extendible-dynamic-ref.json""
                    },
                    {
                        ""$defs"": {
                            ""elements"": {
                                ""$dynamicAnchor"": ""elements"",
                                ""properties"": {
                                    ""a"": true
                                },
                                ""required"": [""a""],
                                ""additionalProperties"": false
                            }
                        }
                    }
                ]
            }";

            string referencedJson = @"{
                ""description"": ""extendible array"",
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""http://localhost:1234/draft2020-12/extendible-dynamic-ref.json"",
                ""type"": ""object"",
                ""properties"": {
                    ""elements"": {
                        ""type"": ""array"",
                        ""items"": {
                            ""$dynamicRef"": ""#elements""
                        }
                    }
                },
                ""required"": [""elements""],
                ""additionalProperties"": false,
                ""$defs"": {
                    ""elements"": {
                        ""$dynamicAnchor"": ""elements""
                    }
                }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://localhost:1234/draft2020-12/extendible-dynamic-ref.json"), referencedJson);

            JSchema s = JSchema.Parse(json, resolver);

            JObject data = JObject.Parse(@"{
                ""elements"": [
                    { ""b"": 1 }
                ]
            }");

            bool valid = data.IsValid(s, out IList<string> errorMessages);
            Assert.IsFalse(valid, "Should be invalid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefLookToRootBecauseOfDynamicAnchor()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/relative-dynamic-reference/root"",
                ""$dynamicAnchor"": ""meta"",
                ""type"": ""object"",
                ""properties"": {
                    ""foo"": { ""const"": ""pass"" }
                },
                ""$ref"": ""extended"",
                ""$defs"": {
                    ""extended"": {
                        ""$id"": ""extended"",
                        ""$dynamicAnchor"": ""meta"",
                        ""type"": ""object"",
                        ""properties"": {
                            ""bar"": { ""$ref"": ""bar"" }
                        }
                    },
                    ""bar"": {
                        ""$id"": ""bar"",
                        ""type"": ""object"",
                        ""properties"": {
                            ""baz"": { ""$dynamicRef"": ""extended#meta"" }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);
            JSchema bazSchema = s.Ref.Properties["bar"].Properties["baz"];
            Assert.AreEqual("pass", (string)bazSchema.Properties["foo"].Const);

            JObject data = JObject.Parse(@"{
                ""foo"": ""pass"",
                ""bar"": {
                    ""baz"": { ""foo"": ""fail"" }
                }
            }");

            bool valid = data.IsValid(s, out IList<string> errorMessages);
            Assert.IsFalse(valid, "Should be invalid. Errors: " + string.Join(", ", errorMessages.ToArray()));
        }

        [Test]
        public void DynamicRefSkipOverIntermediateResources_DirectReference()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                ""$id"": ""https://test.json-schema.org/dynamic-ref-skips-intermediate-resource/main"",
                ""type"": ""object"",
                ""properties"": {
                    ""bar-item"": {
                        ""$ref"": ""item""
                    }
                },
                ""$defs"": {
                    ""bar"": {
                        ""$id"": ""bar"",
                        ""type"": ""array"",
                        ""items"": {
                            ""$ref"": ""item""
                        },
                        ""$defs"": {
                            ""item"": {
                                ""$id"": ""item"",
                                ""type"": ""object"",
                                ""properties"": {
                                    ""content"": {
                                        ""$dynamicRef"": ""#content""
                                    }
                                },
                                ""$defs"": {
                                    ""defaultContent"": {
                                        ""$dynamicAnchor"": ""content"",
                                        ""type"": ""integer""
                                    }
                                }
                            },
                            ""content"": {
                                ""$dynamicAnchor"": ""content"",
                                ""type"": ""string""
                            }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            JObject data1 = JObject.Parse(@"{ ""bar-item"": { ""content"": 42 } }");
            bool valid1 = data1.IsValid(s, out IList<string> errorMessages1);
            Assert.IsTrue(valid1, "Should be valid. Errors: " + string.Join(", ", errorMessages1.ToArray()));

            JObject data2 = JObject.Parse(@"{ ""bar-item"": { ""content"": ""value"" } }");
            bool valid2 = data2.IsValid(s, out IList<string> errorMessages2);
            Assert.IsFalse(valid2, "Should be invalid. Errors: " + string.Join(", ", errorMessages2.ToArray()));
        }

        [Test]
        public void DynamicRefSkipOverIntermediateResources_PointerReferenceAcrossResourceBoundary()
        {
            string json = @"{
                ""$schema"": ""https://json-schema.org/draft/next/schema"",
                ""$id"": ""https://test.json-schema.org/dynamic-ref-skips-intermediate-resource/optional/main"",
                ""type"": ""object"",
                ""properties"": {
                    ""bar-item"": {
                        ""$ref"": ""bar#/$defs/item""
                    }
                },
                ""$defs"": {
                    ""bar"": {
                        ""$id"": ""bar"",
                        ""type"": ""array"",
                        ""items"": {
                            ""$ref"": ""item""
                        },
                        ""$defs"": {
                            ""item"": {
                                ""$id"": ""item"",
                                ""type"": ""object"",
                                ""properties"": {
                                    ""content"": {
                                        ""$dynamicRef"": ""#content""
                                    }
                                },
                                ""$defs"": {
                                    ""defaultContent"": {
                                        ""$dynamicAnchor"": ""content"",
                                        ""type"": ""integer""
                                    }
                                }
                            },
                            ""content"": {
                                ""$dynamicAnchor"": ""content"",
                                ""type"": ""string""
                            }
                        }
                    }
                }
            }";

            JSchema s = JSchema.Parse(json);

            JObject data1 = JObject.Parse(@"{ ""bar-item"": { ""content"": 42 } }");
            bool valid1 = data1.IsValid(s, out IList<string> errorMessages1);
            Assert.IsTrue(valid1, "Should be valid. Errors: " + string.Join(", ", errorMessages1.ToArray()));

            JObject data2 = JObject.Parse(@"{ ""bar-item"": { ""content"": ""value"" } }");
            bool valid2 = data2.IsValid(s, out IList<string> errorMessages2);
            Assert.IsFalse(valid2, "Should be invalid. Errors: " + string.Join(", ", errorMessages2.ToArray()));
        }
    }
}