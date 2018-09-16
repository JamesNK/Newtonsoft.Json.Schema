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
    public class Issue0154Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = JSchema.Parse(@"{
    ""properties"": {
        ""EmailCcList"": {
            ""description"": ""The email addresses "",
            ""type"": ""string""
        }
    }
}");

            JObject entityJObject = new JObject();
            entityJObject.Add("EmailCcList", new JValue((string)null));

            Assert.IsFalse(entityJObject.IsValid(s, out IList<string> errors));
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Invalid type. Expected String but got Null. Path 'EmailCcList'.", errors[0]);
        }
    }
}