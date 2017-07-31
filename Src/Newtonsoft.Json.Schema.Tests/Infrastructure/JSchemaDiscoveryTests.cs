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
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaDiscoveryTests : TestFixtureBase
    {
        [Test]
        public void SimpleTest()
        {
            JSchema prop = new JSchema();
            JSchema root = new JSchema
            {
                Properties =
                {
                    { "prop1", prop },
                    { "prop2", prop }
                }
            };

            JSchemaDiscovery discovery = new JSchemaDiscovery(root);
            discovery.Discover(root, null);

            Assert.AreEqual(2, discovery.KnownSchemas.Count);
            Assert.AreEqual(root, discovery.KnownSchemas[0].Schema);
            Assert.AreEqual(new Uri("#", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[0].Id);
            Assert.AreEqual(prop, discovery.KnownSchemas[1].Schema);
            Assert.AreEqual(new Uri("#/properties/prop1", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[1].Id);
        }

        [Test]
        public void SimpleTest_RootId()
        {
            JSchema prop = new JSchema();
            JSchema root = new JSchema
            {
                Id = new Uri("http://localhost/"),
                Properties =
                {
                    { "prop1", prop },
                    { "prop2", prop }
                }
            };

            JSchemaDiscovery discovery = new JSchemaDiscovery(root);
            discovery.Discover(root, null);

            Assert.AreEqual(2, discovery.KnownSchemas.Count);
            Assert.AreEqual(root, discovery.KnownSchemas[0].Schema);
            Assert.AreEqual(new Uri("http://localhost/#", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[0].Id);
            Assert.AreEqual(prop, discovery.KnownSchemas[1].Schema);
            Assert.AreEqual(new Uri("http://localhost/#/properties/prop1", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[1].Id);
        }

        [Test]
        public void RootId_NestedId()
        {
            JSchema prop1 = new JSchema
            {
                Id = new Uri("test.json/", UriKind.RelativeOrAbsolute),
                Items =
                {
                    new JSchema(),
                    new JSchema
                    {
                        Id = new Uri("#fragmentItem2", UriKind.RelativeOrAbsolute),
                        Items =
                        {
                            new JSchema(),
                            new JSchema { Id = new Uri("#fragmentItem2Item2", UriKind.RelativeOrAbsolute) },
                            new JSchema { Id = new Uri("file.json", UriKind.RelativeOrAbsolute) },
                            new JSchema { Id = new Uri("/file1.json", UriKind.RelativeOrAbsolute) }
                        },
                        ItemsPositionValidation = true
                    }
                },
                ItemsPositionValidation = true
            };
            JSchema prop2 = new JSchema
            {
                Id = new Uri("#fragment", UriKind.RelativeOrAbsolute),
                Not = new JSchema()
            };
            JSchema root = new JSchema
            {
                Id = new Uri("http://localhost/", UriKind.RelativeOrAbsolute),
                Properties =
                {
                    { "prop1", prop1 },
                    { "prop2", prop2 }
                },
                ExtensionData =
                {
                    {
                        "definitions",
                        new JObject
                        {
                            { "def1", new JSchema() },
                            { "def2", new JSchema { Id = new Uri("def2.json", UriKind.RelativeOrAbsolute) } },
                            {
                                "defn",
                                new JArray
                                {
                                    new JValue(5),
                                    new JSchema()
                                }
                            }
                        }
                    }
                }
            };

            JSchemaDiscovery discovery = new JSchemaDiscovery(root);
            discovery.Discover(root, null);

            int i = 0;

            Assert.AreEqual(new Uri("http://localhost/#", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root, discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/#/definitions/def1", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual((JSchema)root.ExtensionData["definitions"]["def1"], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/def2.json", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual((JSchema)root.ExtensionData["definitions"]["def2"], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/#/definitions/defn/1", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual((JSchema)root.ExtensionData["definitions"]["defn"][1], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/#/items/0", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[0], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/#fragmentItem2", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[1], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/#fragmentItem2/items/0", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[1].Items[0], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/#fragmentItem2Item2", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[1].Items[1], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/test.json/file.json", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[1].Items[2], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/file1.json", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop1"].Items[1].Items[3], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/#fragment", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop2"], discovery.KnownSchemas[i++].Schema);

            Assert.AreEqual(new Uri("http://localhost/#fragment/not", UriKind.RelativeOrAbsolute), discovery.KnownSchemas[i].Id);
            Assert.AreEqual(root.Properties["prop2"].Not, discovery.KnownSchemas[i++].Schema);
        }

        [Test]
        public void Draft4Example()
        {
            JSchema schema1 = new JSchema
            {
                Id = new Uri("#foo", UriKind.RelativeOrAbsolute),
                Title = "schema1"
            };
            JSchema schema2 = new JSchema
            {
                Id = new Uri("otherschema.json", UriKind.RelativeOrAbsolute),
                Title = "schema2",
                ExtensionData =
                {
                    {
                        "nested",
                        new JSchema
                        {
                            Title = "nested",
                            Id = new Uri("#bar", UriKind.RelativeOrAbsolute)
                        }
                    },
                    {
                        "alsonested",
                        new JSchema
                        {
                            Title = "alsonested",
                            Id = new Uri("t/inner.json#a", UriKind.RelativeOrAbsolute),
                            ExtensionData =
                            {
                                {
                                    "nestedmore",
                                    new JSchema { Title = "nestedmore" }
                                }
                            }
                        }
                    }
                }
            };
            JSchema schema3 = new JSchema
            {
                Title = "schema3",
                Id = new Uri("some://where.else/completely#", UriKind.RelativeOrAbsolute)
            };

            JSchema root = new JSchema
            {
                Id = new Uri("http://x.y.z/rootschema.json#", UriKind.RelativeOrAbsolute),
                ExtensionData =
                {
                    { "schema1", schema1 },
                    { "schema2", schema2 },
                    { "schema3", schema3 }
                }
            };

            JSchemaDiscovery discovery = new JSchemaDiscovery(root);
            discovery.Discover(root, null);

            Assert.AreEqual(7, discovery.KnownSchemas.Count);
            Assert.AreEqual("http://x.y.z/rootschema.json#", discovery.KnownSchemas[0].Id.ToString());
            Assert.AreEqual("http://x.y.z/rootschema.json#foo", discovery.KnownSchemas[1].Id.ToString());
            Assert.AreEqual("http://x.y.z/otherschema.json", discovery.KnownSchemas[2].Id.ToString());
            Assert.AreEqual("http://x.y.z/otherschema.json#bar", discovery.KnownSchemas[3].Id.ToString());
            Assert.AreEqual("http://x.y.z/t/inner.json#a", discovery.KnownSchemas[4].Id.ToString());
            Assert.AreEqual("http://x.y.z/t/inner.json#/nestedmore", discovery.KnownSchemas[5].Id.ToString());
            Assert.AreEqual("some://where.else/completely#", discovery.KnownSchemas[6].Id.ToString());
        }

        [Test]
        public void ComplexPath()
        {
            string path = TestHelpers.ResolveFilePath(@"resources\schemas\custom\validator1.json");

            string schemaJson = File.ReadAllText(path);
            JSchema schema = JSchema.Parse(schemaJson);

            JSchemaDiscovery discovery = new JSchemaDiscovery(schema);
            discovery.Discover(schema, null);

            // ensure the path does not contain multiple #'s
            Assert.AreEqual("http://www.example.org/IntegralLifeProduct#/definitions/ProductType/allOf/0", discovery.KnownSchemas[3].Id.OriginalString);
        }

        [Test]
        public void MultipleNestedPaths()
        {
            string schemaJson = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""type"": ""object"",
    ""title"": ""Quotes Configurator Schema"",
    ""description"": ""Defines the constraints for an acceptable JSON object required to produce a package"",
    ""properties"": {
       ""working_directory"" : { ""$ref"": ""#/definitions/working_directory"" },
       ""environment"" : { ""$ref"": ""#/definitions/environment"" },
        ""timezone"" : {
          ""description"": ""Timezone in in which processes are scheduled, does control the timezone in which they run"",
          ""type"": ""string""
       },
       ""cleanup_directories"" : {
          ""description"": ""Directories (and corresponding file types) which will be cleaned twice daily"",
          ""type"": ""object""
       },
       ""holiday_key"" : {
          ""description"": ""Holiday key which your package will follow"",
          ""type"": ""string""
       },
       ""cpu_alerting_threshold"" : {
          ""description"": ""Alerting threshold for high CPU usage"",
          ""type"": ""integer"",
          ""minimum"": 75,
          ""maximum"" : 250,
          ""multipleOf"" : 1
       },
       ""processes"" : { ""$ref"": ""#/definitions/processes"" },
       ""schedule"" : { ""$ref"": ""#/definitions/schedule"" }
    },
    ""additionalProperties"": false,
    ""required"": [
        ""working_directory"",
        ""timezone"",
        ""environment"",
        ""cleanup_directories"",
        ""processes"",
        ""schedule""
    ],
    ""definitions"": {
        ""nonEmptyWord"" : {
           ""type"" : ""string"",
           ""pattern"" : ""(^\\S+$)""
        },
        ""nonEmptyString"" : {
           ""type"" : ""string"",
           ""pattern"": ""(^\\S+(\\S|\\s)+$)""
        },
        ""dayShorthand"" : {
           ""type"" : ""string"",
           ""enum"" : [ ""Sun"", ""Mon"", ""Tue"", ""Wed"", ""Thu"", ""Fri"", ""Sat"" ]
        },
        ""twentyFourHourTime"" : {
           ""type"" : ""integer"",
           ""minimum"": 0,
           ""maximum"" : 235959,
           ""multipleOf"" : 1
        },
        ""environment"" : {
           ""description"": ""Global environment settings for each of the processes"",
           ""type"": ""object""
        },
        ""working_directory"" : {
           ""description"": ""Working directory for each process. Usually either root directory of your package or its etc/ directory"",
           ""type"": ""string"",
           ""pattern"": ""(^@\\w+@$)""
        },
        ""process"": {
           ""id"": ""process"",
           ""type"" : ""object"",
           ""properties"": {
              ""working_directory"" : { ""$ref"": ""../#/definitions/working_directory"" },
              ""environment"" : { ""$ref"": ""../#/definitions/environment"" },
              ""name"" : { ""$ref"": ""../#/definitions/nonEmptyWord"" },
              ""description"" : { ""$ref"": ""../#/definitions/nonEmptyString"" },
              ""ulimit"" : {
                 ""type"" : ""integer"",
                 ""multipleOf"": 256,
                 ""minimum"" : 256
              },
              ""alert_on_downtime"" : { ""type"" : ""boolean"" },
              ""starts_on_holiday"" : { ""type"" : ""boolean"" },
              ""check_for_completion"" : { ""type"" : ""boolean"" },
              ""arguments"" : {
                 ""type"" : ""array"",
                 ""items"" : { ""$ref"": ""../#/definitions/nonEmptyString"" },
                 ""minItems"": 1
              },
              ""stats_store_groups"" : {
                 ""type"" : ""array"",
                 ""items"" : { ""$ref"": ""../#/definitions/nonEmptyWord"" },
                 ""uniqueItems"" : true
              }
           },
           ""patternProperties"": {

              ""(^binary$|^module$|^external$|^bash$|^python$|^perl$)"" : { ""$ref"": ""../#/definitions/nonEmptyWord"" },
           },
           ""additionalProperties"": false,
           ""minProperties"": 5,
           ""maxProperties"": 12,
           ""allOf"" : [
             { ""required"": [ ""name"", ""ulimit"", ""alert_on_downtime"", ""starts_on_holiday"" ] },
             { ""oneOf"": [
               { ""required"" :  [ ""binary"" ] },
               { ""required"" :  [ ""module"" ] },
               { ""required"" :  [ ""external"" ] },
               { ""required"" :  [ ""bash"" ] },
               { ""required"" :  [ ""python"" ] },
               { ""required"" :  [ ""perl"" ] }
                ]
             }
           ]
        },
        ""processes"": {
          ""description"": ""All processes that are allowed to be run"",
          ""type"" : ""array"",
          ""items"" : { ""$ref"": ""#/definitions/process"" }
        },
        ""task"": {
          ""id"" : ""task"",
          ""type"" : ""object"",
          ""properties"" : {
             ""task_name"" : { ""$ref"": ""../#/definitions/nonEmptyWord"" },
             ""processes"" : {
                ""type"" : ""array""
             },
             ""runs_on""   : {
                ""type"" : ""array"",
                ""items"" : { ""$ref"": ""../#/definitions/dayShorthand"" },
                ""uniqueItems"" : true
             }
          },
          ""patternProperties"": {
             ""(^start_at$|^end_at$)"" : { ""$ref"": ""../#/definitions/twentyFourHourTime"" },
          },
          ""allOf"" : [
             { ""required"": [ ""task_name"", ""processes"", ""runs_on"" ] },
             { ""oneOf"" : [
                   {
                      ""anyOf"": [
                         { ""required"" :  [ ""start_at"" ] }
                      ]
                   },
                   {
                      ""anyOf"": [
                         { ""required"" :  [ ""end_at"" ] }
                      ]
                   }
                ]
             },
           ],
          ""additionalProperties"": false
        },
        ""schedule"": {
           ""description"": ""Separate tasks which consist of one or more processes and will run under your package"",
           ""type"" : ""array"",
           ""items"" : { ""$ref"": ""#/definitions/task"" }
        }
    }
}";

            JSchema schema = JSchema.Parse(schemaJson);

            JSchemaDiscovery discovery = new JSchemaDiscovery(schema);
            discovery.Discover(schema, null);

            //for (int i = 0; i < discovery.KnownSchemas.Count; i++)
            //{
            //    KnownSchema knownSchema = discovery.KnownSchemas[i];

            //    Console.WriteLine(string.Format(@"Assert.AreEqual(""{0}"", discovery.KnownSchemas[{1}].Id.OriginalString);", knownSchema.Id, i));
            //}

            Assert.AreEqual("#", discovery.KnownSchemas[0].Id.OriginalString);
            Assert.AreEqual("#/definitions/nonEmptyWord", discovery.KnownSchemas[1].Id.OriginalString);
            Assert.AreEqual("#/definitions/nonEmptyString", discovery.KnownSchemas[2].Id.OriginalString);
            Assert.AreEqual("#/definitions/dayShorthand", discovery.KnownSchemas[3].Id.OriginalString);
            Assert.AreEqual("#/definitions/twentyFourHourTime", discovery.KnownSchemas[4].Id.OriginalString);
            Assert.AreEqual("#/definitions/environment", discovery.KnownSchemas[5].Id.OriginalString);
            Assert.AreEqual("#/definitions/working_directory", discovery.KnownSchemas[6].Id.OriginalString);
            Assert.AreEqual("process", discovery.KnownSchemas[7].Id.OriginalString);
            Assert.AreEqual("process#/properties/ulimit", discovery.KnownSchemas[8].Id.OriginalString);
            Assert.AreEqual("process#/properties/alert_on_downtime", discovery.KnownSchemas[9].Id.OriginalString);
            Assert.AreEqual("process#/properties/starts_on_holiday", discovery.KnownSchemas[10].Id.OriginalString);
            Assert.AreEqual("process#/properties/check_for_completion", discovery.KnownSchemas[11].Id.OriginalString);
            Assert.AreEqual("process#/properties/arguments", discovery.KnownSchemas[12].Id.OriginalString);
            Assert.AreEqual("process#/properties/stats_store_groups", discovery.KnownSchemas[13].Id.OriginalString);
            Assert.AreEqual("process#/allOf/0", discovery.KnownSchemas[14].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1", discovery.KnownSchemas[15].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/0", discovery.KnownSchemas[16].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/1", discovery.KnownSchemas[17].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/2", discovery.KnownSchemas[18].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/3", discovery.KnownSchemas[19].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/4", discovery.KnownSchemas[20].Id.OriginalString);
            Assert.AreEqual("process#/allOf/1/oneOf/5", discovery.KnownSchemas[21].Id.OriginalString);
            Assert.AreEqual("#/definitions/processes", discovery.KnownSchemas[22].Id.OriginalString);
            Assert.AreEqual("task", discovery.KnownSchemas[23].Id.OriginalString);
            Assert.AreEqual("task#/properties/processes", discovery.KnownSchemas[24].Id.OriginalString);
            Assert.AreEqual("task#/properties/runs_on", discovery.KnownSchemas[25].Id.OriginalString);
            Assert.AreEqual("task#/allOf/0", discovery.KnownSchemas[26].Id.OriginalString);
            Assert.AreEqual("task#/allOf/1", discovery.KnownSchemas[27].Id.OriginalString);
            Assert.AreEqual("task#/allOf/1/oneOf/0", discovery.KnownSchemas[28].Id.OriginalString);
            Assert.AreEqual("task#/allOf/1/oneOf/0/anyOf/0", discovery.KnownSchemas[29].Id.OriginalString);
            Assert.AreEqual("task#/allOf/1/oneOf/1", discovery.KnownSchemas[30].Id.OriginalString);
            Assert.AreEqual("task#/allOf/1/oneOf/1/anyOf/0", discovery.KnownSchemas[31].Id.OriginalString);
            Assert.AreEqual("#/definitions/schedule", discovery.KnownSchemas[32].Id.OriginalString);
            Assert.AreEqual("#/properties/timezone", discovery.KnownSchemas[33].Id.OriginalString);
            Assert.AreEqual("#/properties/cleanup_directories", discovery.KnownSchemas[34].Id.OriginalString);
            Assert.AreEqual("#/properties/holiday_key", discovery.KnownSchemas[35].Id.OriginalString);
            Assert.AreEqual("#/properties/cpu_alerting_threshold", discovery.KnownSchemas[36].Id.OriginalString);
        }

        [Test]
        public void DuplicateIds()
        {
            JSchema root = new JSchema
            {
                Properties =
                {
                    {
                        "prop1", new JSchema
                        {
                            Id = new Uri("duplicate", UriKind.RelativeOrAbsolute),
                            Properties =
                            {
                                { "test", new JSchema() }
                            }
                        }
                    },
                    {
                        "prop2", new JSchema
                        {
                            Id = new Uri("duplicate", UriKind.RelativeOrAbsolute),
                            Properties =
                            {
                                { "test", new JSchema() }
                            }
                        }
                    }
                }
            };

            JSchemaDiscovery discovery = new JSchemaDiscovery(root);
            discovery.Discover(root, null);

            Assert.AreEqual("#", discovery.KnownSchemas[0].Id.OriginalString);
            Assert.AreEqual("duplicate", discovery.KnownSchemas[1].Id.OriginalString);
            Assert.AreEqual("duplicate#/properties/test", discovery.KnownSchemas[2].Id.OriginalString);
            Assert.AreEqual("duplicate", discovery.KnownSchemas[3].Id.OriginalString);
            Assert.AreEqual("duplicate#/properties/test", discovery.KnownSchemas[4].Id.OriginalString);
        }

        [Test]
        public void InvalidSchemaId()
        {
            string schemaJson = @"{
  ""id"": ""#root"",
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""title"": ""command"",
  ""type"": ""object"",
  ""oneOf"": [
    {
      ""$ref"": ""#/definitions/registerCommand""
    },
    {
      ""$ref"": ""#/definitions/unregisterCommand""
    },
    {
      ""$ref"": ""#/definitions/loginCommand""
    },
    {
      ""$ref"": ""#/definitions/logoutCommand""
    },
    {
      ""$ref"": ""#/definitions/syncCommand""
    },
    {
      ""$ref"": ""#/definitions/sendmsgCommand""
    }
  ],
  ""required"": [
    ""cmd""
  ],
  ""definitions"": {
    ""registerCommand"": {
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""register""
          ]
        },
        ""pms"": {
          ""$ref"": ""#/definitions/authParams""
        }
      },
      ""required"": [
        ""pms""
      ]
    },
    ""unregisterCommand"": {
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""unregister""
          ]
        }
      }
    },
    ""loginCommand"": {
      ""title"": ""log in"",
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""login""
          ]
        },
        ""pms"": {
          ""$ref"": ""#/definitions/authParams""
        }
      },
      ""required"": [
        ""pms""
      ]
    },
    ""logoutCommand"": {
      ""title"": ""log out"",
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""logout""
          ]
        }
      }
    },
    ""syncCommand"": {
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""sync""
          ]
        },
        ""pms"": {
          ""$ref"": ""#/definitions/syncParams""
        }
      },
      ""required"": [
        ""pms""
      ]
    },
    ""sendmsgCommand"": {
      ""properties"": {
        ""cmd"": {
          ""enum"": [
            ""sendmsg""
          ]
        },
        ""pms"": {
        }
      },
      ""required"": [
        ""pms""
      ]
    },
    ""authParams"": {
      ""type"": ""object"",
      ""properties"": {
        ""username"": {
        },
        ""password"": {
        }
      },
      ""required"": [
        ""username"",
        ""password""
      ]
    },
    ""syncParams"": {
      ""type"": ""object"",
      ""properties"": {
        ""inbox"": {
        },
        ""contacts"": {
        }
      },
      ""required"": [
        ""inbox"",
        ""contacts""
      ]
    }
  }
}";

            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings());
            schemaReader.ReadRoot(new JsonTextReader(new StringReader(schemaJson)));

            JSchemaDiscovery discovery = schemaReader._schemaDiscovery;

            foreach (KnownSchema knownSchema in discovery.KnownSchemas)
            {
                Assert.IsFalse(knownSchema.Id.OriginalString.StartsWith("#/#/", StringComparison.Ordinal));
            }
        }
    }
}