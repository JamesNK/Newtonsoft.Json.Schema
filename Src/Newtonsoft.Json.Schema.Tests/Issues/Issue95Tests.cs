#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue95Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = new JSchema();
            s.Minimum = 1;
            s.Maximum = 1000;

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""minimum"": 1.0,
  ""maximum"": 1000.0
}", s.ToString(SchemaVersion.Draft4));

            StringAssert.AreEqual(@"{
  ""$schema"": ""http://json-schema.org/draft-06/schema#"",
  ""minimum"": 1.0,
  ""maximum"": 1000.0
}", s.ToString(SchemaVersion.Draft6));
        }

        public const string Schema = @"{
  ""type"": ""integer"",
  ""minimum"": 1.0,
  ""minimum"": 1000.0
}";
    }
}