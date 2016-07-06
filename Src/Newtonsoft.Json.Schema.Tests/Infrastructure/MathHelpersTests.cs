#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
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
        public void IsDoubleMultipleTests()
        {
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(0.0075, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.00001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.000001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.0000001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.00000001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.000000001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.0000000001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.4, 0.00000000001));

            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(500.4, 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(500.4, 0.0021));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(0.00751, 0.0001));
        }

        [Test]
        public void IsIntegerMultipleTests()
        {
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.0001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.00001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.0000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.00000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.000000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.0000000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, 0.00000000001));

            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, 0.0021));
        }

#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
        [Test]
        public void IsIntegerMultipleTests_BigInteger()
        {
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.00001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.00000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.000000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0000000001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.00000000001));

            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1.0E100), 1));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(BigInteger.Parse("99999999999999999999999999999999999999999999999999"), 1));

            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0021));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(BigInteger.Parse("99999999999999999999999999999999999999999999999999"), 2));
        }
#endif
    }
}