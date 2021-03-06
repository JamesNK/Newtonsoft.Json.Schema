#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
using Newtonsoft.Json.Schema.Infrastructure;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class FormatHelpersTests : TestFixtureBase
    {
        [Test]
        public void ValidateDuration()
        {
            Assert.IsTrue(FormatHelpers.ValidateDuration("P3Y6M4DT12H30M5S"));
        }
    }
}