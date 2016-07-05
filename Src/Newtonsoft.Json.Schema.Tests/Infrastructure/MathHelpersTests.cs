#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Reflection;
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
    public class MathHelpersTests
    {
        [Test]
        public void IsMultipleTests()
        {
            Assert.AreEqual(true, MathHelpers.IsMultiple(0.0075, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.00001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.000001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.0000001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.00000001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.000000001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.0000000001));
            Assert.AreEqual(true, MathHelpers.IsMultiple(500.4, 0.00000000001));

            Assert.AreEqual(false, MathHelpers.IsMultiple(500.4, 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsMultiple(500.4, 0.0021));
            Assert.AreEqual(false, MathHelpers.IsMultiple(0.00751, 0.0001));
        }
    }
}