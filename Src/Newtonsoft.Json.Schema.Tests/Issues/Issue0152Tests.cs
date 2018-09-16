#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue0152Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            SchemaReferenceHandling v = SchemaReferenceHandling.Arrays | SchemaReferenceHandling.Dictionaries;
            Assert.AreNotEqual(SchemaReferenceHandling.All, v);
        }

        [Test]
        public void Test_All()
        {
            SchemaReferenceHandling v = SchemaReferenceHandling.Objects | SchemaReferenceHandling.Arrays | SchemaReferenceHandling.Dictionaries;
            Assert.AreEqual(SchemaReferenceHandling.All, v);
        }
    }
}