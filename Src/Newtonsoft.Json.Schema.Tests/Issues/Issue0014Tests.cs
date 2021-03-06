#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
    public class Issue0014Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string referencedSchemaJson = @"{
    ""definitions"": {},
    ""address"": {
        ""type"": ""object"",
        ""properties"": {
        ""street_address"": { ""type"": ""string"" },
        ""city"": { ""type"": ""string"" },
        ""state"": { ""type"": ""string"" }
    },
    ""required"": [""street_address"", ""city"", ""state""]
    }
}";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();

            resolver.Add(new Uri("schema.json", UriKind.RelativeOrAbsolute), referencedSchemaJson);
            JSchema sampleSchema = JSchema.Parse(@"{
    'type': 'object',
    'allOf': [
        { '$ref': 'schema.json#/address' }
    ]
}", resolver);
        }
    }
}