#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class LicenseTests : TestFixtureBase
    {
        [Test]
        public void RegisterLicense_Success_ish()
        {
            string licenseText = "1002-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), "Specified test license expiried on 2116-12-27.");
        }

        [Test]
        public void RegisterLicense_Failure()
        {
            string licenseText = "1003-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), "License ID does not match signature license ID.");
        }

        [Test]
        public void RegisterLicense_Sample()
        {
            try
            {
                string licenseKey = "your-license-key";

                License.RegisterLicense(licenseKey);
            }
            catch (JSchemaException)
            {
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void HasRegisteredLicense_NoLicense_False()
        {
            Assert.IsFalse(License.HasRegisteredLicense());
        }
    }
}