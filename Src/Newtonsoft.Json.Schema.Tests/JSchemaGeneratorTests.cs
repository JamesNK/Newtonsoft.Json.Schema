#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema.Generation;
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
        public class GeneratorTestClass
        {
            [Required]
            public string RequiredProperty { get; set; }
        }

        [Test]
        public void RequiredPropertyTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(GeneratorTestClass));

            JSchema propertySchema = schema.Properties["RequiredProperty"];

            Assert.AreEqual(JSchemaType.String, propertySchema.Type);
        }

        public class EnumTestClass
        {
            public StringComparison EnumProperty { get; set; }
        }

        public class EnumWithAttributeTestClass
        {
            public StringComparison EnumProperty1 { get; set; }
            [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
            public StringComparison EnumProperty2 { get; set; }
            [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
            public StringComparison? EnumProperty3 { get; set; }
        }

        [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
        public enum EnumWithAttribute
        {
            One,
            Two,
            Three
        }

        [Test]
        public void ProviderAttributeOnProperty()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(EnumWithAttributeTestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);

            JSchema enumProp1 = schema.Properties["EnumProperty1"];
            Assert.AreEqual(JSchemaType.Integer, enumProp1.Type);
            Assert.AreEqual(6, enumProp1.Enum.Count);

            JSchema enumProp2 = schema.Properties["EnumProperty2"];
            Assert.AreEqual(JSchemaType.String, enumProp2.Type);
            Assert.AreEqual(6, enumProp2.Enum.Count);

            JSchema enumProp3 = schema.Properties["EnumProperty3"];
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, enumProp3.Type);
            Assert.AreEqual(6, enumProp3.Enum.Count);
        }

        [Test]
        public void EnumOnType()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(EnumWithAttribute));

            Assert.AreEqual(JSchemaType.String, schema.Type);
            Assert.AreEqual(3, schema.Enum.Count);
            Assert.AreEqual("One", (string)schema.Enum[0]);
            Assert.AreEqual("Two", (string)schema.Enum[1]);
            Assert.AreEqual("Three", (string)schema.Enum[2]);
        }

        [Test]
        public void EnumOnTypeNullable()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(EnumWithAttribute?));

            Assert.AreEqual(JSchemaType.String, schema.Type);
            Assert.AreEqual(3, schema.Enum.Count);
            Assert.AreEqual("One", (string)schema.Enum[0]);
            Assert.AreEqual("Two", (string)schema.Enum[1]);
            Assert.AreEqual("Three", (string)schema.Enum[2]);
        }

        [Test]
        public void EnumOnTypeNullable_AllowNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(EnumWithAttribute?), true);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Type);
            Assert.AreEqual(3, schema.Enum.Count);
            Assert.AreEqual("One", (string)schema.Enum[0]);
            Assert.AreEqual("Two", (string)schema.Enum[1]);
            Assert.AreEqual("Three", (string)schema.Enum[2]);
        }

        [Test]
        public void NullableTypeAtRoot()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(StringComparison?));

            Assert.AreEqual(JSchemaType.Integer, schema.Type);
            Assert.AreEqual(6, schema.Enum.Count);
            Assert.AreEqual(0, (int)schema.Enum[0]);
            Assert.AreEqual(1, (int)schema.Enum[1]);
            Assert.AreEqual(2, (int)schema.Enum[2]);
            Assert.AreEqual(3, (int)schema.Enum[3]);
            Assert.AreEqual(4, (int)schema.Enum[4]);
            Assert.AreEqual(5, (int)schema.Enum[5]);
        }

        [Test]
        public void NullableTypeAtRoot_AllowNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(StringComparison?), true);

            Assert.AreEqual(JSchemaType.Integer | JSchemaType.Null, schema.Type);
            Assert.AreEqual(6, schema.Enum.Count);
            Assert.AreEqual(0, (int)schema.Enum[0]);
            Assert.AreEqual(1, (int)schema.Enum[1]);
            Assert.AreEqual(2, (int)schema.Enum[2]);
            Assert.AreEqual(3, (int)schema.Enum[3]);
            Assert.AreEqual(4, (int)schema.Enum[4]);
            Assert.AreEqual(5, (int)schema.Enum[5]);
        }

        [Test]
        public void ProvidersCollection()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema schema = generator.Generate(typeof(EnumTestClass));

            JSchema propertySchema = schema.Properties["EnumProperty"];

            Assert.AreEqual(JSchemaType.String, propertySchema.Type);
            Assert.AreEqual(6, propertySchema.Enum.Count);
            Assert.AreEqual("CurrentCulture", (string)propertySchema.Enum[0]);
            Assert.AreEqual("CurrentCultureIgnoreCase", (string)propertySchema.Enum[1]);
            Assert.AreEqual("InvariantCulture", (string)propertySchema.Enum[2]);
            Assert.AreEqual("InvariantCultureIgnoreCase", (string)propertySchema.Enum[3]);
            Assert.AreEqual("Ordinal", (string)propertySchema.Enum[4]);
            Assert.AreEqual("OrdinalIgnoreCase", (string)propertySchema.Enum[5]);
        }

        public class DictionaryWithMinAndMaxLength
        {
            [MinLength(5)]
            [MaxLength(10)]
            public IDictionary<string, int> Dictionary1 { get; set; }
            public IDictionary<string, int> Dictionary2 { get; set; }
            [MinLength(5)]
            [MaxLength(10)]
            public IDictionary<string, int> Dictionary3 { get; set; }
        }

        [Test]
        public void DictionaryWithLength()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(DictionaryWithMinAndMaxLength));

            JSchema dictionary1 = schema.Properties["Dictionary1"];
            JSchema dictionary2 = schema.Properties["Dictionary2"];
            JSchema dictionary3 = schema.Properties["Dictionary3"];

            Assert.AreNotEqual(dictionary1, dictionary2);
            Assert.AreEqual(dictionary1, dictionary3);

            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, dictionary1.Type);
            Assert.AreEqual(5, dictionary1.MinimumProperties);
            Assert.AreEqual(10, dictionary1.MaximumProperties);

            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, dictionary2.Type);
            Assert.AreEqual(null, dictionary2.MinimumProperties);
            Assert.AreEqual(null, dictionary2.MaximumProperties);
        }

        public class ListWithMinAndMaxLength
        {
            [MinLength(5)]
            [MaxLength(10)]
            public IList<int> List1 { get; set; }
            public IList<int> List2 { get; set; }
            [MinLength(5)]
            [MaxLength(10)]
            public IList<int> List3 { get; set; }
        }

        [Test]
        public void ListWithLength()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ListWithMinAndMaxLength));

            JSchema list1 = schema.Properties["List1"];
            JSchema list2 = schema.Properties["List2"];
            JSchema list3 = schema.Properties["List3"];

            Assert.AreNotEqual(list1, list2);
            Assert.AreEqual(list1, list3);

            Assert.AreEqual(JSchemaType.Array | JSchemaType.Null, list1.Type);
            Assert.AreEqual(5, list1.MinimumItems);
            Assert.AreEqual(10, list1.MaximumItems);

            Assert.AreEqual(JSchemaType.Array | JSchemaType.Null, list2.Type);
            Assert.AreEqual(null, list2.MinimumItems);
            Assert.AreEqual(null, list2.MaximumItems);
        }

        public class NumberWithRange
        {
            [System.ComponentModel.DataAnnotations.RangeAttribute(5, 10)]
            public int IntegerProperty { get; set; }
            [System.ComponentModel.DataAnnotations.RangeAttribute(5.5, 10.5)]
            public decimal DecimalProperty { get; set; }
            [System.ComponentModel.DataAnnotations.RangeAttribute(0.5, 1.5)]
            public double DoubleProperty { get; set; }
        }

        [Test]
        public void NumberWithRangeTests()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(NumberWithRange));

            JSchema integerProperty = schema.Properties["IntegerProperty"];
            JSchema decimalProperty = schema.Properties["DecimalProperty"];
            JSchema doubleProperty = schema.Properties["DoubleProperty"];

            Assert.AreEqual(JSchemaType.Integer, integerProperty.Type);
            Assert.AreEqual(5, integerProperty.Minimum);
            Assert.AreEqual(10, integerProperty.Maximum);

            Assert.AreEqual(JSchemaType.Float, decimalProperty.Type);
            Assert.AreEqual(5.5, decimalProperty.Minimum);
            Assert.AreEqual(10.5, decimalProperty.Maximum);

            Assert.AreEqual(JSchemaType.Float, doubleProperty.Type);
            Assert.AreEqual(0.5, doubleProperty.Minimum);
            Assert.AreEqual(1.5, doubleProperty.Maximum);

            Console.WriteLine(schema.ToString());
        }

        public class StringWithMinAndMaxLength
        {
            [MinLength(5)]
            [MaxLength(10)]
            public string String1 { get; set; }
            public string String2 { get; set; }
            [MinLength(5)]
            [MaxLength(10)]
            public string String3 { get; set; }
        }

        [Test]
        public void StringWithLength()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(StringWithMinAndMaxLength));

            JSchema string1 = schema.Properties["String1"];
            JSchema string2 = schema.Properties["String2"];
            JSchema string3 = schema.Properties["String3"];

            Assert.AreNotEqual(string1, string2);
            Assert.AreNotEqual(string1, string3);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, string1.Type);
            Assert.AreEqual(5, string1.MinimumLength);
            Assert.AreEqual(10, string1.MaximumLength);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, string2.Type);
            Assert.AreEqual(null, string2.MinimumLength);
            Assert.AreEqual(null, string2.MaximumLength);

            Console.WriteLine(schema.ToString());
        }

        public class EnumWithEnumDataType
        {
            [EnumDataType(typeof(StringComparison))]
            public string String1 { get; set; }
            public string String2 { get; set; }
            [EnumDataType(typeof(StringComparison))]
            public int Integer1 { get; set; }
            public int Integer2 { get; set; }
        }

        [Test]
        public void EnumWithEnumDataTypeTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(EnumWithEnumDataType));

            JSchema string1 = schema.Properties["String1"];
            JSchema integer = schema.Properties["Integer1"];

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, string1.Type);
            Assert.AreEqual(6, string1.Enum.Count);
            Assert.AreEqual("CurrentCulture", (string)string1.Enum[0]);

            Assert.AreEqual(JSchemaType.Integer, integer.Type);
            Assert.AreEqual(6, integer.Enum.Count);
            Assert.AreEqual(0, (int)integer.Enum[0]);

            Console.WriteLine(schema.ToString());
        }

        public class StringAttributeOptions
        {
            [RegularExpression("[A-Z]")]
            [Required]
            public string String1 { get; set; }
            [DataType(DataType.Date)]
            [Required]
            public string String2 { get; set; }
            [Url]
            [Required]
            public string String3 { get; set; }
            [DataType(DataType.DateTime)]
            [Required]
            public string String4 { get; set; }
            [DataType(DataType.Time)]
            [Required]
            public string String5 { get; set; }
            [DataType(DataType.EmailAddress)]
            [Required]
            public string String6 { get; set; }
            [DataType(DataType.Url)]
            [Required]
            public string String7 { get; set; }
            [DataType(DataType.PhoneNumber)]
            [Required]
            public string String8 { get; set; }
            [Phone]
            [Required]
            public string String9 { get; set; }
            [EmailAddress]
            [Required]
            public string String10 { get; set; }
            [StringLength(50)]
            [Required]
            public string String11 { get; set; }
        }

        [Test]
        public void StringAttributeOptionsTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(StringAttributeOptions));

            JSchema string1 = schema.Properties["String1"];
            JSchema string2 = schema.Properties["String2"];
            JSchema string3 = schema.Properties["String3"];

            Assert.AreEqual(JSchemaType.String, string1.Type);
            Assert.AreEqual("[A-Z]", string1.Pattern);

            Assert.AreEqual(JSchemaType.String, string2.Type);
            Assert.AreEqual("date", string2.Format);

            Assert.AreEqual(JSchemaType.String, string3.Type);
            Assert.AreEqual("uri", string3.Format);

            Assert.AreEqual("date-time", schema.Properties["String4"].Format);
            Assert.AreEqual("time", schema.Properties["String5"].Format);
            Assert.AreEqual("email", schema.Properties["String6"].Format);
            Assert.AreEqual("uri", schema.Properties["String7"].Format);
            Assert.AreEqual("phone", schema.Properties["String8"].Format);
            Assert.AreEqual("phone", schema.Properties["String9"].Format);
            Assert.AreEqual("email", schema.Properties["String10"].Format);
            Assert.AreEqual(0, schema.Properties["String11"].MinimumLength);
            Assert.AreEqual(50, schema.Properties["String11"].MaximumLength);

            Console.WriteLine(schema.ToString());
        }

        [Test]
        public void MixedRequired()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.None;

            JSchema schema = generator.Generate(typeof(CircularReferenceWithIdClass));

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""id"": ""MyExplicitId"",
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Child"": {
      ""id"": ""MyExplicitId-1"",
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Name"": {
          ""type"": [
            ""string"",
            ""null""
          ]
        },
        ""Child"": {
          ""$ref"": ""MyExplicitId-1""
        }
      },
      ""required"": [
        ""Name"",
        ""Child""
      ]
    }
  },
  ""required"": [
    ""Name"",
    ""Child""
  ]
}", json);

            CircularReferenceWithIdClass c = new CircularReferenceWithIdClass();
            JToken t = JToken.Parse(JsonConvert.SerializeObject(c, Formatting.Indented));

            Assert.IsTrue(t.IsValid(schema));
        }

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
      ""type"": ""integer"",
      ""default"": 21
    },
    ""TestProperty1"": {
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""TestProperty1Value""
    }
  },
  ""required"": [
    ""TestField1"",
    ""TestProperty1""
  ]
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
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""BirthDate"": {
      ""type"": ""string""
    },
    ""LastModified"": {
      ""type"": ""string""
    }
  },
  ""required"": [
    ""Name"",
    ""BirthDate"",
    ""LastModified""
  ]
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
      ""type"": ""string""
    },
    ""FName"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""LName"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""RoleId"": {
      ""type"": ""integer""
    },
    ""NullableRoleId"": {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""NullRoleId"": {
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""Active"": {
      ""type"": [
        ""boolean"",
        ""null""
      ]
    }
  },
  ""required"": [
    ""Id"",
    ""FName"",
    ""LName"",
    ""RoleId"",
    ""NullableRoleId"",
    ""NullRoleId"",
    ""Active""
  ]
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

            generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(new Uri(typeof(Store).FullName, UriKind.RelativeOrAbsolute), schema.Id);

            generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseAssemblyQualifiedName;
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(new Uri(typeof(Store).AssemblyQualifiedName, UriKind.RelativeOrAbsolute), schema.Id);
        }

        [Test]
        public void CircularReferenceWithTypeNameId()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(CircularReferenceClass), true);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Name"].Type);
            Assert.AreEqual(new Uri(typeof(CircularReferenceClass).FullName, UriKind.RelativeOrAbsolute), schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreEqual(schema, schema.Properties["Child"]);
        }

        [Test]
        public void CircularReferenceWithExplicitId()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(CircularReferenceWithIdClass));

            Console.WriteLine(schema.ToString());

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Name"].Type);
            Assert.AreEqual(new Uri("MyExplicitId", UriKind.RelativeOrAbsolute), schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreNotEqual(schema, schema.Properties["Child"]); // child allow's null
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
            Assert.AreEqual(0, schema.Properties.Count);
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
#if !(NETFX_CORE || PORTABLE || PORTABLE40 || ASPNETCORE50)
                IgnoreSerializableAttribute = true
#endif
            };

            JSchema schema = generator.Generate(typeof(DirectoryInfo), true);

            string json = schema.ToString();

            Console.WriteLine(json);

            StringAssert.AreEqual(@"{
  ""id"": ""System.IO.DirectoryInfo"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""Name"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Parent"": {
      ""$ref"": ""System.IO.DirectoryInfo""
    },
    ""Exists"": {
      ""type"": ""boolean""
    },
    ""FullName"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Extension"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""CreationTime"": {
      ""type"": ""string""
    },
    ""CreationTimeUtc"": {
      ""type"": ""string""
    },
    ""LastAccessTime"": {
      ""type"": ""string""
    },
    ""LastAccessTimeUtc"": {
      ""type"": ""string""
    },
    ""LastWriteTime"": {
      ""type"": ""string""
    },
    ""LastWriteTimeUtc"": {
      ""type"": ""string""
    },
    ""Attributes"": {
      ""type"": ""integer""
    }
  },
  ""required"": [
    ""Name"",
    ""Parent"",
    ""Exists"",
    ""FullName"",
    ""Extension"",
    ""CreationTime"",
    ""CreationTimeUtc"",
    ""LastAccessTime"",
    ""LastAccessTimeUtc"",
    ""LastWriteTime"",
    ""LastWriteTimeUtc"",
    ""Attributes""
  ]
}", json);

            DirectoryInfo temp = new DirectoryInfo(@"c:\temp");

            JTokenWriter jsonWriter = new JTokenWriter();
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.ContractResolver = new CustomDirectoryInfoMapper
            {
#if !(NETFX_CORE || PORTABLE || PORTABLE40 || ASPNETCORE50)
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
      ""type"": ""integer""
    },
    ""minor"": {
      ""type"": ""integer""
    },
    ""build"": {
      ""type"": ""integer""
    },
    ""revision"": {
      ""type"": ""integer""
    },
    ""majorRevision"": {
      ""type"": ""integer""
    },
    ""minorRevision"": {
      ""type"": ""integer""
    }
  },
  ""required"": [
    ""major"",
    ""minor"",
    ""build"",
    ""revision"",
    ""majorRevision"",
    ""minorRevision""
  ]
}", json);
        }

#if !PORTABLE40
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
      ""type"": ""integer""
    },
    ""_Minor"": {
      ""type"": ""integer""
    },
    ""_Build"": {
      ""type"": ""integer""
    },
    ""_Revision"": {
      ""type"": ""integer""
    }
  },
  ""required"": [
    ""_Major"",
    ""_Minor"",
    ""_Build"",
    ""_Revision""
  ]
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
#endif

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
      ""type"": ""integer"",
      ""enum"": [
        0,
        1,
        -1
      ]
    }
  },
  ""required"": [
    ""x""
  ]
}", json);
        }

        [Test]
        public void CircularCollectionReferences()
        {
            Type type = typeof(Workspace);
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema schema = generator.Generate(type);

            // should succeed
            Assert.IsNotNull(schema);
        }

        [Test]
        public void CircularReferenceWithMixedRequires()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema schema = generator.Generate(typeof(CircularReferenceClass), true);
            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json.Schema.Tests.TestObjects.CircularReferenceClass"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""properties"": {
    ""Name"": {
      ""type"": ""string""
    },
    ""Child"": {
      ""$ref"": ""Newtonsoft.Json.Schema.Tests.TestObjects.CircularReferenceClass""
    }
  },
  ""required"": [
    ""Name""
  ]
}", json);
        }

        [Test]
        public void JsonPropertyWithHandlingValues()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            generator.UndefinedSchemaIdHandling = JSchemaUndefinedIdHandling.UseTypeName;
            JSchema schema = generator.Generate(typeof(JsonPropertyWithHandlingValues), true);
            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json.Schema.Tests.TestObjects.JsonPropertyWithHandlingValues"",
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
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingPopulateProperty"": {
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
  },
  ""required"": [
    ""DefaultValueHandlingIncludeProperty"",
    ""DefaultValueHandlingPopulateProperty"",
    ""NullValueHandlingIncludeProperty"",
    ""ReferenceLoopHandlingErrorProperty"",
    ""ReferenceLoopHandlingIgnoreProperty"",
    ""ReferenceLoopHandlingSerializeProperty""
  ]
}", json);
        }

        [Test]
        public void GenerateForNullableInt32()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(NullableInt32TestClass));
            string json = schema.ToString();

            Tests.StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""Value"": {
      ""type"": [
        ""integer"",
        ""null""
      ]
    }
  },
  ""required"": [
    ""Value""
  ]
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
        public void GenerateSchemaWithStringEnum()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider
            {
                CamelCaseText = true
            });
            JSchema schema = generator.Generate(typeof(Y));

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""y"": {
      ""type"": ""string"",
      ""enum"": [
        ""no"",
        ""asc"",
        ""desc""
      ]
    }
  },
  ""required"": [
    ""y""
  ]
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