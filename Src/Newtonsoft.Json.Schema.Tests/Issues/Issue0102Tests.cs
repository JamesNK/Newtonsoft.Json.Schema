#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
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