#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#if !NET35
using System.Dynamic;
#endif
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Tests.TestObjects;
using Newtonsoft.Json.Serialization;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaGeneratorTests : TestFixtureBase
    {
#if !PORTABLE
        [JsonObject(Title = "Title!", Description = "Description!")]
        public class DescriptionTestClass
        {
            [System.ComponentModel.Description("This is prop1!")]
            [System.ComponentModel.DisplayName("This is prop1 title!")]
            public string Prop1 { get; set; }
            [System.ComponentModel.Description("This is prop2!")]
            [System.ComponentModel.DisplayName("This is prop2 title!")]
            public string Prop2 { get; set; }
            public string Prop3 { get; set; }
            [System.ComponentModel.Description("This is prop4!")]
            [System.ComponentModel.DisplayName("This is prop4 title!")]
            public DescriptionChild Prop4 { get; set; }
            public DescriptionChild Prop5 { get; set; }
        }

        [System.ComponentModel.Description("This is child!")]
        [System.ComponentModel.DisplayName("This is child title!")]
        public class DescriptionChild
        {

        }

        [Test]
        public void TitleAndDescriptionTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(DescriptionTestClass));

            Console.WriteLine(schema.ToString());

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual("Title!", schema.Title);
            Assert.AreEqual("Description!", schema.Description);
            Assert.AreEqual(5, schema.Properties.Count);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Prop1"].Type);
            Assert.AreEqual("This is prop1!", schema.Properties["Prop1"].Description);
            Assert.AreEqual("This is prop1 title!", schema.Properties["Prop1"].Title);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Prop2"].Type);
            Assert.AreEqual("This is prop2!", schema.Properties["Prop2"].Description);
            Assert.AreEqual("This is prop2 title!", schema.Properties["Prop2"].Title);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Prop3"].Type);
            Assert.AreEqual(null, schema.Properties["Prop3"].Description);

            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Prop4"].Type);
            Assert.AreEqual("This is prop4!", schema.Properties["Prop4"].Description);
            Assert.AreEqual("This is prop4 title!", schema.Properties["Prop4"].Title);

            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Prop5"].Type);
            Assert.AreEqual("This is child!", schema.Properties["Prop5"].Description);
            Assert.AreEqual("This is child title!", schema.Properties["Prop5"].Title);

            DescriptionTestClass testObject = new DescriptionTestClass();
            JObject jObject = JObject.FromObject(testObject);
            jObject.Validate(schema);
        }

        [JsonObject(Title = "Title!", Description = "Description!")]
        public class DescriptionWithSchemaProviderTestClass
        {
            [System.ComponentModel.Description("This is prop1!")]
            [System.ComponentModel.DisplayName("This is prop1 title!")]
            [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
            public StringComparison Prop1 { get; set; }
            [System.ComponentModel.Description("This is prop2!")]
            [System.ComponentModel.DisplayName("This is prop2 title!")]
            [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
            public StringComparison Prop2 { get; set; }
            public StringComparison Prop3 { get; set; }
            [System.ComponentModel.Description("This is prop4!")]
            [System.ComponentModel.DisplayName("This is prop4 title!")]
            [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
            public DescriptionChildWithSchemaProvider Prop4 { get; set; }
            public DescriptionChildWithSchemaProvider Prop5 { get; set; }
        }

        [System.ComponentModel.Description("This is child!")]
        [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
        public enum DescriptionChildWithSchemaProvider
        {
            One,
            Two
        }

        [Test]
        public void TitleAndDescriptionWithSchemaProviderTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(DescriptionWithSchemaProviderTestClass));

            Console.WriteLine(schema.ToString());

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual("Title!", schema.Title);
            Assert.AreEqual("Description!", schema.Description);
            Assert.AreEqual(5, schema.Properties.Count);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Prop1"].Type);
            Assert.AreEqual("This is prop1!", schema.Properties["Prop1"].Description);
            Assert.AreEqual("This is prop1 title!", schema.Properties["Prop1"].Title);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Prop2"].Type);
            Assert.AreEqual("This is prop2!", schema.Properties["Prop2"].Description);
            Assert.AreEqual("This is prop2 title!", schema.Properties["Prop2"].Title);

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["Prop3"].Type);
            Assert.AreEqual(null, schema.Properties["Prop3"].Description);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Prop4"].Type);
            Assert.AreEqual("This is prop4!", schema.Properties["Prop4"].Description);
            Assert.AreEqual("This is prop4 title!", schema.Properties["Prop4"].Title);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Prop5"].Type);
            Assert.AreEqual("This is child!", schema.Properties["Prop5"].Description);
            Assert.AreEqual(null, schema.Properties["Prop5"].Title);

            JObject jObject = new JObject
            {
                ["Prop1"] = "InvariantCulture",
                ["Prop2"] = "CurrentCultureIgnoreCase",
                ["Prop3"] = 5,
                ["Prop4"] = "One",
                ["Prop5"] = "Two"
            };

            jObject.Validate(schema);
        }
#endif

        public class MixNullableRootClass
        {
            [JsonProperty(Required = Required.DisallowNull)]
            public MixNullablePropClass NotNull { get; set; }
            public MixNullablePropClass Null { get; set; }
        }

        public class MixNullablePropClass
        {
            public string Name { get; set; }
        }

        [Test]
        public void Generate_TypeMixedRequired_BothSchemasInDefinitions()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaReferenceHandling = SchemaReferenceHandling.All
            };
            JSchema schema = generator.Generate(typeof(MixNullableRootClass));

            JObject definitions = (JObject)schema.ExtensionData["definitions"];

            Assert.AreEqual(2, definitions.Count);

            Assert.IsTrue(definitions.Properties().Any(p => schema.Properties["NotNull"] == (JSchema)p.Value));
            Assert.IsTrue(definitions.Properties().Any(p => schema.Properties["Null"] == (JSchema)p.Value));
        }

        public class FormatTestClass
        {
            public Uri UriProp { get; set; }
            public Guid GuidProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
        }

        [Test]
        public void DeriveFormatTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.AllowNull
            };
            JSchema schema = generator.Generate(typeof(FormatTestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(4, schema.Properties.Count);

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["UriProp"].Type);
            Assert.AreEqual("uri", schema.Properties["UriProp"].Format);

            Assert.AreEqual(JSchemaType.String, schema.Properties["GuidProp"].Type);
            Assert.AreEqual(null, schema.Properties["GuidProp"].Format);

            Assert.AreEqual(JSchemaType.String, schema.Properties["DateTimeProp"].Type);
            Assert.AreEqual("date-time", schema.Properties["DateTimeProp"].Format);

            Assert.AreEqual(JSchemaType.String, schema.Properties["DateTimeOffsetProp"].Type);
            Assert.AreEqual("date-time", schema.Properties["DateTimeOffsetProp"].Format);

            FormatTestClass testObject = new FormatTestClass();
            JObject jObject = JObject.FromObject(testObject);
            jObject.Validate(schema);
        }

        public class TestClass
        {
            public int Counter { get; set; }
            public TestEnum? TestProperty { get; set; }
        }

        public enum TestEnum
        {
            none, one, two
        }

        [Test]
        public void NullableEnumProperty_AllowNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.AllowNull
            };
            JSchema schema = generator.Generate(typeof(TestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(2, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.Integer | JSchemaType.Null, schema.Properties["TestProperty"].Type);
            Assert.AreEqual(4, schema.Properties["TestProperty"].Enum.Count);
            Assert.AreEqual(null, ((JValue)schema.Properties["TestProperty"].Enum[0]).Value);
            Assert.AreEqual(0, (int)schema.Properties["TestProperty"].Enum[1]);
            Assert.AreEqual(1, (int)schema.Properties["TestProperty"].Enum[2]);
            Assert.AreEqual(2, (int)schema.Properties["TestProperty"].Enum[3]);

            TestClass testObject = new TestClass();
            JObject jObject = JObject.FromObject(testObject);
            jObject.Validate(schema);
        }

        [Test]
        public void NullableEnumProperty_DisallowNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.DisallowNull
            };
            JSchema schema = generator.Generate(typeof(TestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(2, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.Integer, schema.Properties["TestProperty"].Type);
            Assert.AreEqual(3, schema.Properties["TestProperty"].Enum.Count);
            Assert.AreEqual(0, (int)schema.Properties["TestProperty"].Enum[0]);
            Assert.AreEqual(1, (int)schema.Properties["TestProperty"].Enum[1]);
            Assert.AreEqual(2, (int)schema.Properties["TestProperty"].Enum[2]);

            TestClass testObject = new TestClass
            {
                TestProperty = TestEnum.none
            };
            JObject jObject = JObject.FromObject(testObject);
            jObject.Validate(schema);
        }

        [Test]
        public void NullableEnumProperty_AllowNull_StringEnumGenerationProvider()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.AllowNull,
                GenerationProviders = { new StringEnumGenerationProvider() }
            };
            JSchema schema = generator.Generate(typeof(TestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(2, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["TestProperty"].Type);
            Assert.AreEqual(4, schema.Properties["TestProperty"].Enum.Count);
            Assert.AreEqual(null, ((JValue)schema.Properties["TestProperty"].Enum[0]).Value);
            Assert.AreEqual("none", (string)schema.Properties["TestProperty"].Enum[1]);
            Assert.AreEqual("one", (string)schema.Properties["TestProperty"].Enum[2]);
            Assert.AreEqual("two", (string)schema.Properties["TestProperty"].Enum[3]);

            TestClass testObject = new TestClass();
            JObject jObject = JObject.FromObject(testObject, new JsonSerializer
            {
                Converters = { new StringEnumConverter() }
            });
            jObject.Validate(schema);
        }

        [Test]
        public void NullableEnumProperty_DisallowNull_StringEnumGenerationProvider()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.DisallowNull,
                GenerationProviders = { new StringEnumGenerationProvider() }
            };
            JSchema schema = generator.Generate(typeof(TestClass));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(2, schema.Properties.Count);
            Assert.AreEqual(JSchemaType.String, schema.Properties["TestProperty"].Type);
            Assert.AreEqual(3, schema.Properties["TestProperty"].Enum.Count);
            Assert.AreEqual("none", (string)schema.Properties["TestProperty"].Enum[0]);
            Assert.AreEqual("one", (string)schema.Properties["TestProperty"].Enum[1]);
            Assert.AreEqual("two", (string)schema.Properties["TestProperty"].Enum[2]);

            TestClass testObject = new TestClass
            {
                TestProperty = TestEnum.none
            };
            JObject jObject = JObject.FromObject(testObject, new JsonSerializer
            {
                Converters = { new StringEnumConverter() }
            });
            jObject.Validate(schema);
        }

#if !(NET40 || NET35)
        public class Account
        {
            [EmailAddress]
            [JsonProperty("email", Required = Required.Always)]
            public string Email;
        }

        [Test]
        public void Sample()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Account));
            // {
            //   "type": "object",
            //   "properties": {
            //     "Email": { "type": "string", "format": "email" }
            //   },
            //   "required": [ "Email" ]
            // }

            Console.WriteLine(schema.ToString());
        }
#endif

#if !NET35
        [Test]
        public void DynamicTest()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            dynamic person = new
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            JSchema schema = generator.Generate(person.GetType());

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["Id"].Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["FirstName"].Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["LastName"].Type);

            Assert.AreEqual(false, schema.AllowAdditionalProperties);

            Assert.AreEqual(true, schema.Required.Contains("Id"));
            Assert.AreEqual(true, schema.Required.Contains("FirstName"));
            Assert.AreEqual(true, schema.Required.Contains("LastName"));
        }

        [Test]
        public void DynamicTest_DefaultRequired()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.DisallowNull
            };

            dynamic person = new
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            JSchema schema = generator.Generate(person.GetType());

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["Id"].Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["FirstName"].Type);
            Assert.AreEqual(JSchemaType.String, schema.Properties["LastName"].Type);

            Assert.AreEqual(false, schema.AllowAdditionalProperties);

            Assert.AreEqual(0, schema.Required.Count);
        }
#endif

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

            Assert.AreEqual(1, schema.Required.Count);
            Assert.AreEqual("RequiredProperty", schema.Required[0]);

            JSchema propertySchema = schema.Properties["RequiredProperty"];

            Assert.AreEqual(JSchemaType.String, propertySchema.Type);
        }

        public class EnumTestClass
        {
            public StringComparison EnumProperty { get; set; }
            public int IntProperty { get; set; }
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
            Assert.AreEqual(7, enumProp3.Enum.Count);
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
            Assert.AreEqual(4, schema.Enum.Count);
            Assert.AreEqual(null, ((JValue)schema.Enum[0]).Value);
            Assert.AreEqual("One", (string)schema.Enum[1]);
            Assert.AreEqual("Two", (string)schema.Enum[2]);
            Assert.AreEqual("Three", (string)schema.Enum[3]);
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
            Assert.AreEqual(7, schema.Enum.Count);
            Assert.AreEqual(null, ((JValue)schema.Enum[0]).Value);
            Assert.AreEqual(0, (int)schema.Enum[1]);
            Assert.AreEqual(1, (int)schema.Enum[2]);
            Assert.AreEqual(2, (int)schema.Enum[3]);
            Assert.AreEqual(3, (int)schema.Enum[4]);
            Assert.AreEqual(4, (int)schema.Enum[5]);
            Assert.AreEqual(5, (int)schema.Enum[6]);
        }

        public class IntegerGenerationProvider : JSchemaGenerationProvider
        {
            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                return new JSchema
                {
                    Type = JSchemaType.Integer,
                    Maximum = int.MaxValue,
                    Minimum = int.MinValue
                };
            }

            public override bool CanGenerateSchema(JSchemaTypeGenerationContext context)
            {
                return context.ObjectType == typeof(int);
            }
        }

        [Test]
        public void ProvidersCollection()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());
            generator.GenerationProviders.Add(new IntegerGenerationProvider());

            JSchema schema = generator.Generate(typeof(EnumTestClass));

            JSchema enumPropertySchema = schema.Properties["EnumProperty"];

            Assert.AreEqual(JSchemaType.String, enumPropertySchema.Type);
            Assert.AreEqual(6, enumPropertySchema.Enum.Count);
            Assert.AreEqual("CurrentCulture", (string)enumPropertySchema.Enum[0]);
            Assert.AreEqual("CurrentCultureIgnoreCase", (string)enumPropertySchema.Enum[1]);
            Assert.AreEqual("InvariantCulture", (string)enumPropertySchema.Enum[2]);
            Assert.AreEqual("InvariantCultureIgnoreCase", (string)enumPropertySchema.Enum[3]);
            Assert.AreEqual("Ordinal", (string)enumPropertySchema.Enum[4]);
            Assert.AreEqual("OrdinalIgnoreCase", (string)enumPropertySchema.Enum[5]);

            JSchema integerPropertySchema = schema.Properties["IntProperty"];

            Assert.AreEqual(JSchemaType.Integer, integerPropertySchema.Type);
            Assert.AreEqual(int.MaxValue, Convert.ToInt32(integerPropertySchema.Maximum));
            Assert.AreEqual(int.MinValue, Convert.ToInt32(integerPropertySchema.Minimum));
        }

#if !(NET40 || NET35)
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
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaReferenceHandling = SchemaReferenceHandling.All
            };

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
        public void ListWithLength_ReferenceArray()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaReferenceHandling = SchemaReferenceHandling.All
            };

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

        [Test]
        public void ListWithLength()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(ListWithMinAndMaxLength));

            JSchema list1 = schema.Properties["List1"];
            JSchema list2 = schema.Properties["List2"];
            JSchema list3 = schema.Properties["List3"];

            Assert.AreNotEqual(list1, list2);
            Assert.AreNotEqual(list1, list3);

            Assert.AreEqual(JSchemaType.Array | JSchemaType.Null, list1.Type);
            Assert.AreEqual(5, list1.MinimumItems);
            Assert.AreEqual(10, list1.MaximumItems);

            Assert.AreEqual(JSchemaType.Array | JSchemaType.Null, list2.Type);
            Assert.AreEqual(null, list2.MinimumItems);
            Assert.AreEqual(null, list2.MaximumItems);
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
        }
#endif

        public class NumberWithRange
        {
            [System.ComponentModel.DataAnnotations.RangeAttribute(5, 10)]
            public int IntegerProperty { get; set; }

            [System.ComponentModel.DataAnnotations.RangeAttribute(5.5, 10.5)]
            public decimal DecimalProperty { get; set; }

            public decimal DecimalPropertyNoAttribute { get; set; }

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
            JSchema decimalPropertyNoAttribute = schema.Properties["DecimalPropertyNoAttribute"];
            JSchema doubleProperty = schema.Properties["DoubleProperty"];

            Assert.AreEqual(JSchemaType.Integer, integerProperty.Type);
            Assert.AreEqual(5, integerProperty.Minimum);
            Assert.AreEqual(10, integerProperty.Maximum);

            Assert.AreEqual(JSchemaType.Number, decimalProperty.Type);
            Assert.AreEqual(5.5, decimalProperty.Minimum);
            Assert.AreEqual(10.5, decimalProperty.Maximum);

            Assert.AreEqual(JSchemaType.Number, decimalPropertyNoAttribute.Type);
            Assert.AreEqual(null, decimalPropertyNoAttribute.Minimum);
            Assert.AreEqual(null, decimalPropertyNoAttribute.Maximum);

            Assert.AreEqual(JSchemaType.Number, doubleProperty.Type);
            Assert.AreEqual(0.5, doubleProperty.Minimum);
            Assert.AreEqual(1.5, doubleProperty.Maximum);
        }

#if !NET35
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
        }
#endif

        [Test]
        public void Generate_GenericDictionary()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Dictionary<string, List<string>>));

            string json = schema.ToString();

            StringAssert.AreEqual(@"{
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

            string json = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
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
      ""type"": ""string"",
      ""format"": ""date-time""
    },
    ""LastModified"": {
      ""type"": ""string"",
      ""format"": ""date-time""
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

            StringAssert.AreEqual(@"{
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

            generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(new Uri(typeof(Store).Name, UriKind.RelativeOrAbsolute), schema.Id);

            generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.FullTypeName
            };
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(new Uri(typeof(Store).FullName, UriKind.RelativeOrAbsolute), schema.Id);

            generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.AssemblyQualifiedName
            };
            schema = generator.Generate(typeof(Store));
            Assert.AreEqual(new Uri(typeof(Store).AssemblyQualifiedName, UriKind.RelativeOrAbsolute), schema.Id);
        }

        [Test]
        public void CircularReferenceWithTypeNameId()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };

            JSchema schema = generator.Generate(typeof(CircularReferenceClass), true);

            Assert.AreEqual(JSchemaType.String, schema.Properties["Name"].Type);
            Assert.AreEqual(new Uri(typeof(CircularReferenceClass).Name, UriKind.RelativeOrAbsolute), schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreEqual(schema, schema.Properties["Child"]);
        }

        [Test]
        public void CircularReferenceWithExplicitId()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(CircularReferenceWithIdClass));

            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Name"].Type);
            Assert.AreEqual(new Uri("MyExplicitId", UriKind.RelativeOrAbsolute), schema.Id);
            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Child"].Type);
            Assert.AreNotEqual(schema, schema.Properties["Child"]); // child allow's null
        }

        [Test]
        public void GenerateSchemaForType()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };

            JSchema schema = generator.Generate(typeof(Type));

            Assert.AreEqual(JSchemaType.String, schema.Type);

            string json = JsonConvert.SerializeObject(typeof(Version), Formatting.Indented);

            JValue v = new JValue(json);
            Assert.IsTrue(v.IsValid(schema));
        }

#if !DNXCORE50
#if !PORTABLE || NETSTANDARD1_3 || NETSTANDARD2_0
        [Test]
        public void GenerateSchemaForISerializable()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };

            JSchema schema = generator.Generate(typeof(Exception));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(true, schema.AllowAdditionalProperties);
            Assert.AreEqual(0, schema.Properties.Count);
        }

        [Test]
        public void GenerateSchemaForDBNull()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };

            JSchema schema = generator.Generate(typeof(DBNull));

            Assert.AreEqual(JSchemaType.Null, schema.Type);
        }
#endif

        [Test]
        public void GenerateSchemaForObject()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(object));

            Assert.AreEqual(JSchemaType.String | JSchemaType.Number | JSchemaType.Integer | JSchemaType.Boolean | JSchemaType.Object | JSchemaType.Array, schema.Type);
        }

        [Test]
        public void GenerateSchemaForObject_RootNullable()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(object), true);

            Assert.AreEqual(null, schema.Type);
        }

        [Test]
        public void GenerateSchemaForObjectPropertyWithAttributes()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(ObjectPropertyWithAttributes));

            Console.WriteLine(schema.ToString());

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(1d, schema.Properties[nameof(ObjectPropertyWithAttributes.Value)].Minimum);
            Assert.AreEqual(50d, schema.Properties[nameof(ObjectPropertyWithAttributes.Value)].Maximum);
            Assert.AreEqual(20d, schema.Properties[nameof(ObjectPropertyWithAttributes.Value)].MaximumLength);
        }

        public class ObjectPropertyWithAttributes
        {
            [System.ComponentModel.DataAnnotations.Range(1, 50)]
            [StringLength(20)]
            public object Value { get; set; }
        }

        public class CustomDirectoryInfoMapper : DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                if (objectType == typeof(DirectoryInfo))
                {
                    return base.CreateObjectContract(objectType);
                }

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
#endif

        [Test]
        public void GenerateSchemaCamelCase()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
#if !(NETFX_CORE || PORTABLE || DNXCORE50 || PORTABLE40)
                    IgnoreSerializableAttribute = true
#endif
                }
            };

            JSchema schema = generator.Generate(typeof(TestObject), true);

            string json = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""TestObject"",
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

        private sealed class TestObject
        {
            public int Major { get; set; }
            public int Minor { get; set; }
            public int Build { get; set; }
            public int Revision { get; set; }
            public int MajorRevision { get; set; }
            public int MinorRevision { get; set; }
        }

#if !(PORTABLE40 || DNXCORE50)
        [Test]
        public void GenerateSchemaSerializable()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = false
                },
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.FullTypeName
            };

            JSchema schema = generator.Generate(typeof(Version), true);

            string json = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
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
            JsonSerializer serializer = new JsonSerializer
            {
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = false
                }
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

            StringAssert.AreEqual(@"{
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
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };
            JSchema schema = generator.Generate(type);

            // should succeed
            Assert.IsNotNull(schema);
        }

        [Test]
        public void CircularReferenceWithMixedRequires()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                DefaultRequired = Required.AllowNull
            };

            JSchema schema = generator.Generate(typeof(CircularReferenceClass), false);
            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""definitions"": {
    ""CircularReferenceClass"": {
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Name"": {
          ""type"": ""string""
        },
        ""Child"": {
          ""$ref"": ""#/definitions/CircularReferenceClass""
        }
      },
      ""required"": [
        ""Name""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""type"": ""string""
    },
    ""Child"": {
      ""$ref"": ""#/definitions/CircularReferenceClass""
    }
  },
  ""required"": [
    ""Name""
  ]
}", json);
        }

        [Test]
        public void CircularReferenceWithMixedRequires_WithId()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };

            JSchema schema = generator.Generate(typeof(CircularReferenceClass), false);
            string json = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""CircularReferenceClass"",
  ""definitions"": {
    ""CircularReferenceClass-1"": {
      ""id"": ""CircularReferenceClass-1"",
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Name"": {
          ""type"": ""string""
        },
        ""Child"": {
          ""$ref"": ""CircularReferenceClass-1""
        }
      },
      ""required"": [
        ""Name""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""type"": ""string""
    },
    ""Child"": {
      ""$ref"": ""CircularReferenceClass-1""
    }
  },
  ""required"": [
    ""Name""
  ]
}", json);
        }

        public class ParentCircularReferenceClass
        {
            public string Name { get; set; }
            public CircularReferenceClass Nested { get; set; }
        }

        [Test]
        public void NestedCircularReference()
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(ParentCircularReferenceClass));
            string json = schema.ToString();

            JSchema loadedSchema = JSchema.Parse(json);
            JSchema circularReferenceClassSchema = (JSchema)loadedSchema.ExtensionData["definitions"]["CircularReferenceClass"];

            Assert.IsNotNull(circularReferenceClassSchema);
            Assert.AreEqual(circularReferenceClassSchema, circularReferenceClassSchema.Properties["Child"]);

            StringAssert.AreEqual(@"{
  ""definitions"": {
    ""CircularReferenceClass"": {
      ""type"": [
        ""object"",
        ""null""
      ],
      ""properties"": {
        ""Name"": {
          ""type"": ""string""
        },
        ""Child"": {
          ""$ref"": ""#/definitions/CircularReferenceClass""
        }
      },
      ""required"": [
        ""Name""
      ]
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Nested"": {
      ""$ref"": ""#/definitions/CircularReferenceClass""
    }
  },
  ""required"": [
    ""Name"",
    ""Nested""
  ]
}", json);
        }

        [Test]
        public void JsonPropertyWithHandlingValues()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };
            JSchema schema = generator.Generate(typeof(JsonPropertyWithHandlingValues), true);
            string json = schema.ToString(SchemaVersion.Draft4);

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""id"": ""JsonPropertyWithHandlingValues"",
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
      ""$ref"": ""JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingIgnoreProperty"": {
      ""$ref"": ""JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingSerializeProperty"": {
      ""$ref"": ""JsonPropertyWithHandlingValues""
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

            StringAssert.AreEqual(@"{
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

#if !DNXCORE50
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

        [Test]
        public void DefinitionsOrder()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(BigItem));

            List<JProperty> t = schema.ExtensionData["definitions"].Cast<JProperty>().ToList();
            int index = 0;
            Assert.AreEqual("A", t[index++].Name);
            Assert.AreEqual("B", t[index++].Name);
            Assert.AreEqual("Item", t[index].Name);

            string json = schema.ToString();

            Console.WriteLine(json);
        }

        [Test]
        public void NoWriteJsonConverter()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ClassWithNoWriteConverter));

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Prop1"].Type);
        }

        [Test]
        public void GenerationProviderSettingPlusConverterAttribute()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new ClassWithConverterJSchemaGenerationProvider());
            JSchema schema = generator.Generate(typeof(ClassWithConverter));

            Assert.IsNotNull(schema.Properties["Provider"]);
        }

        [Test]
        public void GenerationProviderSettingPlusDerivedConverterAttribute()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new ClassWithConverterJSchemaGenerationProvider());
            JSchema schema = generator.Generate(typeof(DerivedFromAbstractClassWithConverter));

            Assert.IsNotNull(schema.Properties["Provider"]);
        }

        [Test]
        public void GenerationProviderAttributePlusConverterAttribute()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ClassWithConverterAndProvider));

            Assert.IsNotNull(schema.Properties["Provider"]);
        }

        [Test]
        public void GenerationProviderAttributeDerivedPlusConverterAttributeDerived()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(DerivedFromAbstractClassWithConverterAndProvider));

            Assert.IsNotNull(schema.Properties["Provider"]);
        }

#if !(NET35 || NET40)
        [Test]
        public void ReadOnlyDictionaryProperty()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(ClassWithReadOnlyDictionary));

            Assert.AreEqual(JSchemaType.Object | JSchemaType.Null, schema.Properties["Prop1"].Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["Prop1"].AdditionalProperties.Type);
        }
#endif

        internal class MyRootJsonClass
        {
            public Dictionary<string, BlockBase> Blocks { get; set; }
        }

        internal abstract class BlockBase
        {
            // Removed from sample: a hundred of other properties

            public Dictionary<string, BlockBase> NestedBlocks { get; set; }
        }

        internal class Block1 : BlockBase
        {

        }

        internal class Block2 : BlockBase
        {

        }

        // Removed from sample: a hundred descendants of BlockBase

        internal class MyJSchemaGenerationProvider : JSchemaGenerationProvider
        {
            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                JSchema schema = new JSchema();
                Type[] descendants = { typeof(Block1), typeof(Block2) };
                foreach (Type descendant in descendants)
                {
                    // The line below never exits, because it's calling MySchemaGenerator.GetSchema again with the same parameter
                    JSchema descendantSchema = context.Generator.Generate(descendant);
                    schema.PatternProperties.Add(descendant.Name + ".*", descendantSchema);
                }
                schema.AllowAdditionalProperties = false;
                schema.Type = JSchemaType.Object;
                return schema;
            }

            public override bool CanGenerateSchema(JSchemaTypeGenerationContext context)
            {
                return context.ObjectType == typeof(Dictionary<string, BlockBase>);
            }
        }

        [Test]
        public void JSchemaGenerationProvider_SelfReferencingAbstractType()
        {
            JSchemaGenerator generator = new JSchemaGenerator
            {
                SchemaReferenceHandling = SchemaReferenceHandling.All
            };
            generator.GenerationProviders.Add(new MyJSchemaGenerationProvider());

            JSchema schema = generator.Generate(typeof(MyRootJsonClass));
            string json = schema.ToString();

            StringAssert.AreEqual(@"{
  ""definitions"": {
    ""Block1"": {
      ""type"": ""object"",
      ""properties"": {
        ""NestedBlocks"": {
          ""$ref"": ""#/definitions/Dictionary<String, BlockBase>""
        }
      },
      ""required"": [
        ""NestedBlocks""
      ]
    },
    ""Block2"": {
      ""type"": ""object"",
      ""properties"": {
        ""NestedBlocks"": {
          ""$ref"": ""#/definitions/Dictionary<String, BlockBase>""
        }
      },
      ""required"": [
        ""NestedBlocks""
      ]
    },
    ""Dictionary<String, BlockBase>"": {
      ""type"": ""object"",
      ""additionalProperties"": false,
      ""patternProperties"": {
        ""Block1.*"": {
          ""$ref"": ""#/definitions/Block1""
        },
        ""Block2.*"": {
          ""$ref"": ""#/definitions/Block2""
        }
      }
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""Blocks"": {
      ""$ref"": ""#/definitions/Dictionary<String, BlockBase>""
    }
  },
  ""required"": [
    ""Blocks""
  ]
}", json);

            // check that generic definitions can be parsed
            JSchema loadedSchema = JSchema.Parse(json);
        }
    }

    public class NullableInt32TestClass
    {
        public int? Value { get; set; }
    }

    public class DMDSLBase
    {
        public string Comment;
    }

    public class Workspace : DMDSLBase
    {
        public ControlFlowItemCollection Jobs = new ControlFlowItemCollection();
    }

    public class ControlFlowItemBase : DMDSLBase
    {
        public string Name;
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

    public class A
    {
        public string Value { get; set; }
    }

    public class B
    {
        public string ValueB { get; set; }
    }

    public class Item
    {
        public A ItemA { get; set; }
        public B ItemB { get; set; }

        public List<A> ItemAs { get; set; }
        public List<B> ItemBs { get; set; }
    }

    public class BigItem
    {
        public Item SmallItem { get; set; }
    }

    public class TestJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public class TestNoWriteJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }

    public class ClassWithConverterJSchemaGenerationProvider : JSchemaGenerationProvider
    {
        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            return new JSchema
            {
                Properties =
                {
                    ["Provider"] = new JSchema()
                }
            };
        }
    }

    [JsonConverter(typeof(TestJsonConverter))]
    public class ClassWithConverter
    {
        public string Prop1 { get; set; }
    }

    [JsonConverter(typeof(TestNoWriteJsonConverter))]
    public class ClassWithNoWriteConverter
    {
        public string Prop1 { get; set; }
    }

    [JsonConverter(typeof(TestJsonConverter))]
    [JSchemaGenerationProvider(typeof(ClassWithConverterJSchemaGenerationProvider))]
    public class ClassWithConverterAndProvider
    {
        public string Prop1 { get; set; }
    }

    [JsonConverter(typeof(TestJsonConverter))]
    public abstract class AbstractClassWithConverter
    {
        public string Prop1 { get; set; }
    }

    public class DerivedFromAbstractClassWithConverter : AbstractClassWithConverter
    {
    }

    [JsonConverter(typeof(TestJsonConverter))]
    [JSchemaGenerationProvider(typeof(ClassWithConverterJSchemaGenerationProvider))]
    public abstract class AbstractClassWithConverterAndProvider
    {
        public string Prop1 { get; set; }
    }

    public class DerivedFromAbstractClassWithConverterAndProvider : AbstractClassWithConverterAndProvider
    {
    }

#if !(NET35 || NET40)
    public class ClassWithReadOnlyDictionary
    {
        public IReadOnlyDictionary<string, string> Prop1 { get; set; }
    }
#endif
}