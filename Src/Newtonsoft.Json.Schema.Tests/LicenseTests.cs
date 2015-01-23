#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class LicenseTests : TestFixtureBase
    {
        [Test]
        public void RegisterLicense_Success_ish()
        {
            string licenseText = "1002-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JsonException>(() => License.RegisterLicense(licenseText), "Specified license is for testing only.");
        }

        [Test]
        public void RegisterLicense_Failure()
        {
            string licenseText = "1003-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JsonException>(() => License.RegisterLicense(licenseText), "License ID does not match signature license ID.");
        }

        [Test]
        public void RegisterLicense_Sample()
        {
            try
            {
                string licenseKey = "your-license-key";

                License.RegisterLicense(licenseKey);
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail();
        }
    }
}