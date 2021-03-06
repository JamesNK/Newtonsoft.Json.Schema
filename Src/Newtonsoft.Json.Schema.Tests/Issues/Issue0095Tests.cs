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
    public class Issue0095Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = new JSchema
            {
                Minimum = 1,
                Maximum = 1000
            };

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
    }
}