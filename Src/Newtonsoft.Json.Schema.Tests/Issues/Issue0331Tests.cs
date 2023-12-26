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
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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
    public class Issue0331Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(MyDto));

            Assert.IsFalse(schema.AllowAdditionalPropertiesSpecified);
        }

        public sealed class MyDto
        {
            [JsonProperty("foo")]
            public string Foo { get; set; }

            [JsonExtensionData]
            public IDictionary<string, JToken> ExtensionData { get; set; }
        }
    }
}
