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
        public void RegisterLicense_Sha1_Success_ish()
        {
            string licenseText = "1002-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), "Specified test license expiried on 2116-12-27.");
        }

        [Test]
        public void RegisterLicense_Sha1_Failure()
        {
            string licenseText = "1003-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), "License ID does not match signature license ID.");
        }

        [Test]
        public void RegisterLicense_Sha256_Success_ish()
        {
            string licenseText = "1002-s/d/CeY2p5IJs1YycKRckMxyAW5Ol6fRqrWf2f8hjgcz8Vwv7qBnmCw4pF66Dj/xJkTEIyHyvbZNgcTp/pb6mASzwuwFj/CGG9HuGETNtlLYWyp+ZbG3tPqtQPAzGv4RnhgmyAclWlV//6XbF5LlUSZSJnf5Hs0FpKMaSZmRw5o/h/FoVitCOkONMlgI+6u3gDWRRz00vIdxOayFGucLMN6qAK8f46z3GbOBBgNYIMy2doJn+uo8nhrDueN4W9zTRx3YtgprDi9r9UnHn8/rAVGAPDYJPwS7GyfZu4zWOFODSZ8a8g2Tc/yWXhbrzs8ApPh0LrdIsc0loay2gjcpunsiSWQiOjEwMDIsIkV4cGlyeURhdGUiOiIyMTE2LTEyLTI3VDAwOjAwOjAwWiIsIlR5cGUiOiJUZXN0In0=";
#if NETCOREAPP3_1
            string errorMessage = "License hash algorithm is not supported on this platform: SHA256";
#else
            string errorMessage = "Specified test license expiried on 2116-12-27.";
#endif

            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), errorMessage);
        }

        [Test]
        public void RegisterLicense_Sha256_Failure()
        {
            string licenseText = "1003-s/d/CeY2p5IJs1YycKRckMxyAW5Ol6fRqrWf2f8hjgcz8Vwv7qBnmCw4pF66Dj/xJkTEIyHyvbZNgcTp/pb6mASzwuwFj/CGG9HuGETNtlLYWyp+ZbG3tPqtQPAzGv4RnhgmyAclWlV//6XbF5LlUSZSJnf5Hs0FpKMaSZmRw5o/h/FoVitCOkONMlgI+6u3gDWRRz00vIdxOayFGucLMN6qAK8f46z3GbOBBgNYIMy2doJn+uo8nhrDueN4W9zTRx3YtgprDi9r9UnHn8/rAVGAPDYJPwS7GyfZu4zWOFODSZ8a8g2Tc/yWXhbrzs8ApPh0LrdIsc0loay2gjcpunsiSWQiOjEwMDIsIkV4cGlyeURhdGUiOiIyMTE2LTEyLTI3VDAwOjAwOjAwWiIsIlR5cGUiOiJUZXN0In0=";
#if NETCOREAPP3_1
            string errorMessage = "License hash algorithm is not supported on this platform: SHA256";
#else
            string errorMessage = "License ID does not match signature license ID.";
#endif


            ExceptionAssert.Throws<JSchemaException>(() => License.RegisterLicense(licenseText), errorMessage);
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