#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaTests : TestFixtureBase
    {
        [Test]
        public void SchemaChanged()
        {
            int i = 0;

            JSchema s = new JSchema();
            s.Changed += schema => i++;

            s.Id = new Uri("test", UriKind.RelativeOrAbsolute);

            Assert.AreEqual(1, i);

            JSchema child = new JSchema();
            s.Items.Add(child);

            Assert.AreEqual(2, i);

            child.Id = new Uri("#hi", UriKind.RelativeOrAbsolute);

            Assert.AreEqual(3, i);

            child.Id = new Uri("#hi", UriKind.RelativeOrAbsolute);

            Assert.AreEqual(3, i);

            child.Not = new JSchema();

            Assert.AreEqual(4, i);

            child.Not.AdditionalProperties = s;

            Assert.AreEqual(5, i);

            child.Not.AdditionalProperties.AdditionalItems = s;

            Assert.AreEqual(6, i);

            s.Properties["prop1"] = new JSchema();

            Assert.AreEqual(7, i);

            s.Properties["prop1"].Id = new Uri("#hi", UriKind.RelativeOrAbsolute);

            Assert.AreEqual(8, i);

            s.Items.Add(new JSchema());

            Assert.AreEqual(9, i);

            s.Items[0] = new JSchema();

            Assert.AreEqual(10, i);

            s.Items.RemoveAt(0);

            Assert.AreEqual(11, i);

            s.Items.Clear();

            Assert.AreEqual(12, i);

            s.PatternProperties.Add("[abc]", new JSchema());

            Assert.AreEqual(13, i);

            s.PatternProperties["[abc]"] = new JSchema();

            Assert.AreEqual(14, i);

            s.PatternProperties.Remove("[abc]");

            Assert.AreEqual(15, i);

            s.PatternProperties.Remove("[abc]");

            Assert.AreEqual(15, i);

            s.PatternProperties.Clear();

            Assert.AreEqual(15, i);

            s.Dependencies.Add("dep", new JSchema());

            Assert.AreEqual(16, i);

            s.Dependencies["dep"] = new JSchema();

            Assert.AreEqual(17, i);

            s.Dependencies.Remove("dep");

            Assert.AreEqual(18, i);

            s.Dependencies.Clear();

            Assert.AreEqual(18, i);

            s.Dependencies["dep"] = "string!";

            Assert.AreEqual(18, i);

            s.If = new JSchema();

            Assert.AreEqual(19, i);

            s.Then = new JSchema();

            Assert.AreEqual(20, i);

            s.Else = new JSchema();

            Assert.AreEqual(21, i);
        }

        [Test]
        public void DiscoveryCache()
        {
            JSchema s = new JSchema
            {
                Properties =
                {
                    ["prop1"] = new JSchema
                    {
                        Type = JSchemaType.String
                    }
                }
            };

            string json = @"{'prop1':0, 'prop2':1}";

            JObject o = JObject.Parse(json);

            IList<ValidationError> errors;
            o.IsValid(s, out errors);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("#/properties/prop1", errors[0].SchemaId.OriginalString);

            Assert.AreEqual(2, s.KnownSchemas.Count);

            s.Properties["prop2"] = new JSchema { Type = JSchemaType.String };
            Assert.AreEqual(0, s.KnownSchemas.Count);

            o.IsValid(s, out errors);

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("#/properties/prop1", errors[0].SchemaId.OriginalString);
            Assert.AreEqual("#/properties/prop2", errors[1].SchemaId.OriginalString);

            Assert.AreEqual(3, s.KnownSchemas.Count);
        }

        [Test]
        public void PatternPropertiesValues()
        {
            JSchema s = new JSchema();

            ICollection<JSchema> patternPropertiesSchemas = s.PatternProperties.Values;

            Assert.AreEqual(0, patternPropertiesSchemas.Count);

            JSchema patternSchema = new JSchema
            {
                Id = new Uri("test", UriKind.RelativeOrAbsolute)
            };

            s.PatternProperties["\n+"] = patternSchema;

            Assert.AreEqual(1, patternPropertiesSchemas.Count);

            Assert.AreEqual(patternSchema, patternPropertiesSchemas.ElementAt(0));

            s.PatternProperties.Clear();

            Assert.AreEqual(0, patternPropertiesSchemas.Count);
        }

        [Test]
        public void Properties_Set_NullValue()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.Properties["prop1"] = null;
            }, @"Value cannot be null.
Parameter name: value");
        }

        [Test]
        public void Properties_Set_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.Properties[null] = new JSchema();
            }, @"Value cannot be null.
Parameter name: key");
        }

        [Test]
        public void Properties_Add_NullValue()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.Properties.Add("prop1", null);
            }, @"Value cannot be null.
Parameter name: value");
        }

        [Test]
        public void Properties_Add_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.Properties.Add(null, new JSchema());
            }, @"Value cannot be null.
Parameter name: key");
        }

        [Test]
        public void Properties_Remove_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.Properties.Remove(null);
            }, @"Value cannot be null.
Parameter name: key");
        }

        [Test]
        public void PatternProperties_Set_NullValue()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.PatternProperties["prop1"] = null;
            }, @"Value cannot be null.
Parameter name: value");
        }

        [Test]
        public void PatternProperties_Set_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.PatternProperties[null] = new JSchema();
            }, @"Value cannot be null.
Parameter name: key");
        }

        [Test]
        public void PatternProperties_Add_NullValue()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.PatternProperties.Add("prop1", null);
            }, @"Value cannot be null.
Parameter name: value");
        }

        [Test]
        public void PatternProperties_Add_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.PatternProperties.Add(null, new JSchema());
            }, @"Value cannot be null.
Parameter name: key");
        }

        [Test]
        public void PatternProperties_Remove_NullKey()
        {
            JSchema s = new JSchema();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                s.PatternProperties.Remove(null);
            }, @"Value cannot be null.
Parameter name: key");
        }
    }
}