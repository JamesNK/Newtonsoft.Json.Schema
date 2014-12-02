#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema.Tests.TestObjects;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema.Tests;
using Newtonsoft.Json.Utilities;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Schema;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using StringAssert = NUnit.Framework.StringAssert;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaGeneratorTests : TestFixtureBase
    {
        [Test]
        public void Generate_GenericDictionary()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Dictionary<string, List<string>>));

            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": {
    ""type"": [
      ""array"",
      ""null""
    ],
    ""items"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    }
  }
}", json);

            Dictionary<string, List<string>> value = new Dictionary<string, List<string>>
            {
                { "HasValue", new List<string>() { "first", "second", null } },
                { "NoValue", null }
            };

            string valueJson = JsonConvert.SerializeObject(value, Formatting.Indented);
            JObject o = JObject.Parse(valueJson);

            Assert.IsTrue(o.IsValid(schema));
        }

        [Test]
        public void Generate_DefaultValueAttributeTestClass()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(DefaultValueAttributeTestClass));

            string json = schema.ToString();

            Console.WriteLine(json);

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""TestField1"": {
      ""required"": true,
      ""type"": ""integer"",
      ""default"": 21
    },
    ""TestProperty1"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""TestProperty1Value""
    }
  }
}", json);
        }

        [Test]
        public void Generate_Person()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Person));

            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""Person"",
  ""title"": ""Title!"",
  ""description"": ""JsonObjectAttribute description!"",
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""BirthDate"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastModified"": {
      ""required"": true,
      ""type"": ""string""
    }
  }
}", json);
        }

        [Test]
        public void Generate_UserNullable()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(UserNullable));

            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""Id"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""FName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""LName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""RoleId"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""NullableRoleId"": {
      ""required"": true,
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""NullRoleId"": {
      ""required"": true,
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""Active"": {
      ""required"": true,
      ""type"": [
        ""boolean"",
        ""null""
      ]
    }
  }
}", json);
        }

        [Test]
        public void Generate_RequiredMembersClass()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(RequiredMembersClass));

            Assert.AreEqual(JSchemaType.String, schema.Properties["FirstName"].Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["MiddleName"].Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["LastName"].Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["BirthDate"].Type);
        }

        [Test]
        public void Generate_Store()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Store));

            Assert.AreEqual(11, schema.Properties.Count);

            JSchema productArraySchema = schema.Properties["product"];
            JSchema productSchema = productArraySchema.Items[0];

            Assert.AreEqual(4, productSchema.Properties.Count);
        }

        [Test]
        public void MissingSchemaIdHandlingTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(Store));
            Assert.AreEqual(null, schema.Id);

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(typeof(Store).FullName, schema.Id);

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseAssemblyQualifiedName;
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(typeof(Store).AssemblyQualifiedName, schema.Id);
        }

        [Test]
        public void CircularReferenceError()
        {
            ExceptionAssert.Throws<Exception>(() =>
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                generator.Generate(typeof(CircularReferenceClass));
            }, @"Unresolved circular reference for type 'Newtonsoft.Json.Schema.Tests.TestObjects.CircularReferenceClass'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.");
        }

        [Test]
        public void CircularReferenceWithTypeNameId()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(CircularReferenceClass), true);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Name"].Type);
            Assert.AreEqual(typeof(CircularReferenceClass).FullName, schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreEqual(schema, schema.Properties["Child"]);
        }

        [Test]
        public void CircularReferenceWithExplicitId()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(CircularReferenceWithIdClass));

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Name"].Type);
            Assert.AreEqual("MyExplicitId", schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreEqual(schema, schema.Properties["Child"]);
        }

        [Test]
        public void GenerateSchemaForType()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(Type));

            Assert.AreEqual(JSchemaType.String, schema.Type);

            string json = JsonConvert.SerializeObject(typeof(Version), Formatting.Indented);

            JValue v = new JValue(json);
            Assert.IsTrue(v.IsValid(schema));
        }

        [Test]
        public void GenerateSchemaForISerializable()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(Exception));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(true, schema.AllowAdditionalProperties);
            Assert.AreEqual(null, schema.Properties);
        }

        [Test]
        public void GenerateSchemaForDBNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(DBNull));

            Assert.AreEqual(JSchemaType.Null, schema.Type);
        }

        public class CustomDirectoryInfoMapper : DefaultContractResolver
        {
            public CustomDirectoryInfoMapper()
                : base(true)
            {
            }

            protected override JsonContract CreateContract(Type objectType)
            {
                if (objectType == typeof(DirectoryInfo))
                    return base.CreateObjectContract(objectType);

                return base.CreateContract(objectType);
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

                JsonPropertyCollection c = new JsonPropertyCollection(type);
                foreach (JsonProperty property in properties.Where(m => m.PropertyName != "Root"))
                {
                    c.Add(property);
                }

                return c;
            }
        }

        [Test]
        public void GenerateSchemaForDirectoryInfo()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            generator.ContractResolver = new CustomDirectoryInfoMapper
            {
#if !(NETFX_CORE || PORTABLE || ASPNETCORE50)
                IgnoreSerializableAttribute = true
#endif
            };

            JSchema schema = generator.Generate(typeof(DirectoryInfo), true);

            string json = schema.ToString();

            Console.WriteLine(json);

            StringAssert.AreEqual(@"{
  ""id"": ""System.IO.DirectoryInfo"",
  ""required"": true,
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Parent"": {
      ""$ref"": ""System.IO.DirectoryInfo""
    },
    ""Exists"": {
      ""required"": true,
      ""type"": ""boolean""
    },
    ""FullName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Extension"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""CreationTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""CreationTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastAccessTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastAccessTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastWriteTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastWriteTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""Attributes"": {
      ""required"": true,
      ""type"": ""integer""
    }
  }
}", json);

            DirectoryInfo temp = new DirectoryInfo(@"c:\temp");

            JTokenWriter jsonWriter = new JTokenWriter();
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.ContractResolver = new CustomDirectoryInfoMapper
            {
#if !(NETFX_CORE || PORTABLE || ASPNETCORE50)
                IgnoreSerializableInterface = true
#endif
            };
            serializer.Serialize(jsonWriter, temp);

            List<string> errors = new List<string>();
            jsonWriter.Token.Validate(schema, (sender, args) => errors.Add(args.Message));

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void GenerateSchemaCamelCase()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
#if !(NETFX_CORE || PORTABLE || ASPNETCORE50 || PORTABLE40)
                IgnoreSerializableAttribute = true
#endif
            };

            JSchema schema = generator.Generate(typeof(Version), true);

            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""System.Version"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""major"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""minor"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""build"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""revision"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""majorRevision"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""minorRevision"": {
      ""required"": true,
      ""type"": ""integer""
    }
  }
}", json);
        }

        [Test]
        public void GenerateSchemaSerializable()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = false
            };
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(Version), true);

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""System.Version"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""_Major"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""_Minor"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""_Build"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""_Revision"": {
      ""required"": true,
      ""type"": ""integer""
    }
  }
}", json);

            JTokenWriter jsonWriter = new JTokenWriter();
            JsonSerializer serializer = new JsonSerializer();
            serializer.ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = false
            };
            serializer.Serialize(jsonWriter, new Version(1, 2, 3, 4));

            List<string> errors = new List<string>();
            jsonWriter.Token.Validate(schema, (sender, args) => errors.Add(args.Message));

            Assert.AreEqual(0, errors.Count);

            StringAssert.AreEqual(@"{
  ""_Major"": 1,
  ""_Minor"": 2,
  ""_Build"": 3,
  ""_Revision"": 4
}", jsonWriter.Token.ToString());

            Version version = jsonWriter.Token.ToObject<Version>(serializer);
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Build);
            Assert.AreEqual(4, version.Revision);
        }

        public enum SortTypeFlag
        {
            No = 0,
            Asc = 1,
            Desc = -1
        }

        public class X
        {
            public SortTypeFlag x;
        }

        [Test]
        public void GenerateSchemaWithNegativeEnum()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(X));

            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""x"": {
      ""required"": true,
      ""type"": ""integer"",
      ""enum"": [
        0,
        1,
        -1
      ]
    }
  }
}", json);
        }

        [Test]
        public void CircularCollectionReferences()
        {
            Type type = typeof(Workspace);
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema jsonSchema = generator.Generate(type);

            // should succeed
            Assert.IsNotNull(jsonSchema);
        }

        [Test]
        public void CircularReferenceWithMixedRequires()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema jsonSchema = generator.Generate(typeof(CircularReferenceClass));
            string json = jsonSchema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json.Schema.Tests.TestObjects.CircularReferenceClass"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""Child"": {
      ""$ref"": ""Newtonsoft.Json.Schema.Tests.TestObjects.CircularReferenceClass""
    }
  }
}", json);
        }

        [Test]
        public void JsonPropertyWithHandlingValues()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema jsonSchema = generator.Generate(typeof(JsonPropertyWithHandlingValues));
            string json = jsonSchema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json.Schema.Tests.TestObjects.JsonPropertyWithHandlingValues"",
  ""required"": true,
  ""type"": [
    ""object"",
    ""null""
  ],
  ""properties"": {
    ""DefaultValueHandlingIgnoreProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingIncludeProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingPopulateProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingIgnoreAndPopulateProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""NullValueHandlingIgnoreProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""NullValueHandlingIncludeProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""ReferenceLoopHandlingErrorProperty"": {
      ""$ref"": ""Newtonsoft.Json.Schema.Tests.TestObjects.JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingIgnoreProperty"": {
      ""$ref"": ""Newtonsoft.Json.Schema.Tests.TestObjects.JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingSerializeProperty"": {
      ""$ref"": ""Newtonsoft.Json.Schema.Tests.TestObjects.JsonPropertyWithHandlingValues""
    }
  }
}", json);
        }

        [Test]
        public void GenerateForNullableInt32()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema jsonSchema = generator.Generate(typeof(NullableInt32TestClass));
            string json = jsonSchema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""Value"": {
      ""required"": true,
      ""type"": [
        ""integer"",
        ""null""
      ]
    }
  }
}", json);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SortTypeFlagAsString
        {
            No = 0,
            Asc = 1,
            Desc = -1
        }

        public class Y
        {
            public SortTypeFlagAsString y;
        }

#if !ASPNETCORE50
        [Test]
        [Ignore]
        public void GenerateSchemaWithStringEnum()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Y));

            string json = schema.ToString();

            // NOTE: This fails because the enum is serialized as an integer and not a string.
            // NOTE: There should exist a way to serialize the enum as lowercase strings.
            Assert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""y"": {
      ""required"": true,
      ""type"": ""string"",
      ""enum"": [
        ""no"",
        ""asc"",
        ""desc""
      ]
    }
  }
}", json);
        }
#endif
    }

    public class NullableInt32TestClass
    {
        public int? Value { get; set; }
    }

    public class DMDSLBase
    {
        public String Comment;
    }

    public class Workspace : DMDSLBase
    {
        public ControlFlowItemCollection Jobs = new ControlFlowItemCollection();
    }

    public class ControlFlowItemBase : DMDSLBase
    {
        public String Name;
    }

    public class ControlFlowItem : ControlFlowItemBase //A Job
    {
        public TaskCollection Tasks = new TaskCollection();
        public ContainerCollection Containers = new ContainerCollection();
    }

    public class ControlFlowItemCollection : List<ControlFlowItem>
    {
    }

    public class Task : ControlFlowItemBase
    {
        public DataFlowTaskCollection DataFlowTasks = new DataFlowTaskCollection();
        public BulkInsertTaskCollection BulkInsertTask = new BulkInsertTaskCollection();
    }

    public class TaskCollection : List<Task>
    {
    }

    public class Container : ControlFlowItemBase
    {
        public ControlFlowItemCollection ContainerJobs = new ControlFlowItemCollection();
    }

    public class ContainerCollection : List<Container>
    {
    }

    public class DataFlowTask_DSL : ControlFlowItemBase
    {
    }

    public class DataFlowTaskCollection : List<DataFlowTask_DSL>
    {
    }

    public class SequenceContainer_DSL : Container
    {
    }

    public class BulkInsertTaskCollection : List<BulkInsertTask_DSL>
    {
    }

    public class BulkInsertTask_DSL
    {
    }
}