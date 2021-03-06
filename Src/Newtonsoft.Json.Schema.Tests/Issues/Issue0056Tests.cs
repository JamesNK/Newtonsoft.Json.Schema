﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
    public class Issue0056Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string json = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""properties"": {
    ""data"": {
      ""type"": ""array"",
      ""items"": {
        ""oneOf"": [
          {
            ""$ref"": ""#/definitions/sub1""
          },
          {
            ""$ref"": ""#/definitions/sub2""
          }
        ]
      }
    }
  },
  ""required"": [
    ""data""
  ],
  ""definitions"": {
    ""base"": {
      ""type"": ""object""
    },
    ""sub1"": {
      ""allOf"": [
        {
          ""$ref"": ""#/definitions/base""
        }
      ]
    },
    ""sub2"": {
      ""allOf"": [
        {
          ""$ref"": ""#/definitions/base""
        }
      ]
    }
  }
}";

            JSchema s = JSchema.Parse(json);

            JObject o = JObject.Parse(@"{
  ""data"": [
    null
  ]
}");


            Assert.IsFalse(o.IsValid(s, out IList<ValidationError> errors));
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("JSON is valid against no schemas from 'oneOf'.", errors[0].Message);
        }
    }
}