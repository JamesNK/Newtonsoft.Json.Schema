#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
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
    }
}
