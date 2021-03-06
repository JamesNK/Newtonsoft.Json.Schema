#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class Issue0096Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string schemaJson = @"{
    ""$id"": ""http://example.net/root.json"",
    ""items"": {
        ""type"": ""array"",
        ""items"": { ""$ref"": ""#item"" }
    },
    ""definitions"": {
        ""single"": {
            ""$id"": ""#item"",
            ""type"": ""integer""
        }
    }
}";

            JSchema s = JSchema.Parse(schemaJson);
            Assert.AreEqual(new Uri("http://example.net/root.json"), s.Id);
            Assert.AreEqual(1, s.Items.Count);
            Assert.AreEqual(JSchemaType.Array, s.Items[0].Type);
            Assert.IsTrue(UriComparer.Instance.Equals(new Uri("#item", UriKind.RelativeOrAbsolute), s.Items[0].Items[0].Id));
            Assert.AreEqual("definitions.single", s.Items[0].Items[0].Path);
        }
    }
}