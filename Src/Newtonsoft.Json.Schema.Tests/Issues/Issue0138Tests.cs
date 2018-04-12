#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
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
    public class Issue0138Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            DateTime date = new DateTime(2018, 3, 14, 15, 9, 26, DateTimeKind.Utc);

            string schema =
                @"{
                'properties': {
                    'Date': { 'enum': [" + JsonConvert.SerializeObject(date) + @"], 'required': true }
                }
            }";

            string json = JsonConvert.SerializeObject(new DateTimeContainer {Date = date});

            JObject jsonObject = JObject.Parse(json);
            JSchema jsonSchema = JSchema.Parse(schema);

            bool valid = jsonObject.IsValid(jsonSchema, out IList<string> messages);

            Assert.IsTrue(valid);
        }

        public class DateTimeContainer
        {
            public DateTime Date { get; set; }
        }
    }
}