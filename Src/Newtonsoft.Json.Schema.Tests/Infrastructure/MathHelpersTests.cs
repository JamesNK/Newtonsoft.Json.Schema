#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
    public class MathHelpersTests : TestFixtureBase
    {
        [Test]
        public void IsDoubleMultipleTests()
        {
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(5555555555555555555555555555.01d, 0.01));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(5555555555555555555555555555.01d, 0.013453));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(555555555555555555555555555.01m, 0.01));

            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(3199.981, 0.001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(3199.980, 0.001));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(540.1, 0.001));
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
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(double.MaxValue, double.Epsilon));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(double.MinValue, double.Epsilon));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(double.MaxValue, double.MaxValue));
            Assert.AreEqual(true, MathHelpers.IsDoubleMultiple(500.001f, 0.001));

            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(540.0001, 0.001));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(500.4, 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(500.4, 0.0021));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(0.00751, 0.0001));
            Assert.AreEqual(false, MathHelpers.IsDoubleMultiple(double.MaxValue, 0.001));
        }

        [Test]
        public void IsIntegerMultipleTests()
        {
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(3200, 0.001));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(3199, 0.001));
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
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(1, double.Epsilon));

            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, 0.0021));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, double.MaxValue));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(1, double.MinValue));
        }

#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
        [Test]
        public void IsIntegerMultipleTests_BigInteger()
        {
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(BigInteger.Parse("79228162514264337593543950330"), 1));
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
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1), double.Epsilon));

            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(new BigInteger(1.0E100), 1d));
            Assert.AreEqual(true, MathHelpers.IsIntegerMultiple(BigInteger.Parse("99999999999999999999999999999999999999999999999999"), 1d));

            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(BigInteger.Parse("2000000000000000000000000000"), 0.01));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(BigInteger.Parse("79228162514264337593543950330"), Convert.ToDouble(decimal.MaxValue) + Math.PI));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(BigInteger.Parse("999999999999999999999999999999999999999999999999999999999"), 1.1d));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0000007));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(new BigInteger(1), 0.0021));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(BigInteger.Parse("99999999999999999999999999999999999999999999999999"), 2d));
            Assert.AreEqual(false, MathHelpers.IsIntegerMultiple(new BigInteger(1), double.MaxValue));
        }
#endif
    }
}