#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0102Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema schema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    ["field"] = new JSchema { Type = JSchemaType.String },
                    ["op"] = new JSchema { Type = JSchemaType.String },
                    ["value"] = new JSchema { Type = JSchemaType.None }
                },
                Required = { "field", "op" }
            };

            StringAssert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""field"": {
      ""type"": ""string""
    },
    ""op"": {
      ""type"": ""string""
    },
    ""value"": {}
  },
  ""required"": [
    ""field"",
    ""op""
  ]
}", schema.ToString());
        }
    }
}