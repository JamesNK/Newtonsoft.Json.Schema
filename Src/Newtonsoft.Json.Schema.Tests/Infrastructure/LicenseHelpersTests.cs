#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
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
    public class LicenseHelpersTests : TestFixtureBase
    {
        [Test]
        public void LicenseValidation()
        {
            LicenseDetails license = new LicenseDetails
            {
                Id = 1001,
                ExpiryDate = new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc),
                Type = LicenseType.Test
            };

            string licenseText = "1001-N70qM5lisTgmXINQXrhM7jfbLxMs0Rkq8nF5V10yf4OMjSZQyI/IR8Oz9hfIyEs5hSzI8eX8dFb1KcKsMySTuQ/wHU6EKX6SzsBHzbOSxHhjR5XuBFUznGM+u1FyK7hGOxsyRZgBDS2rClcD/9bfn7SgitQU4nd1fGXnnQK1s5h7IklkIjoxMDAxLCJFeHBpcnlEYXRlIjoiMjAxNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";

            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified test license expiried on 2016-12-27.");
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "Specified test license expiried on 2016-12-27.");
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 28, 0, 0, 0, DateTimeKind.Utc)), "License is not valid for this version of Json.NET Schema. License free upgrade date expired on 2016-12-27. This version of Json.NET Schema was released on 2016-12-28.");

            licenseText = "1001-BADqM5lisTgmXINQXrhM7jfbLxMs0Rkq8nF5V10yf4OMjSZQyI/IR8Oz9hfIyEs5hSzI8eX8dFb1KcKsMySTuQ/wHU6EKX6SzsBHzbOSxHhjR5XuBFUznGM+u1FyK7hGOxsyRZgBDS2rClcD/9bfn7SgitQU4nd1fGXnnQK1s5h7IklkIjoxMDAxLCJFeHBpcnlEYXRlIjoiMjAxNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "License text does not match signature.");
        }

        [Test]
        public void FreeQuotaLimitValidation()
        {
            for (var i = 0; i < 10; i++) LicenseHelpers.IncrementAndCheckGenerationCount();

            ExceptionAssert.Throws<JSchemaException>(
                LicenseHelpers.IncrementAndCheckGenerationCount,
                "The free-quota limit of 10 schema generations per hour has been reached. Please visit http://www.newtonsoft.com/jsonschema to upgrade to a commercial license.");
        }

        [Test]
        public void InvalidLicenseText()
        {
            LicenseDetails license = new LicenseDetails
            {
                Id = 1001,
                ExpiryDate = new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc),
                Type = LicenseType.Test
            };

            string licenseText = "";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "License text is empty.");

            licenseText = "1002";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1003-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "License ID does not match signature license ID.");

            licenseText = "PIES-N8/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1002-!!/Xc2RXpYddnHEH881tBKyb4itL4Wiyh3VL3HMQP3phVElppL3GNeSqYDQQcxSz34lXOHfWTrKEdAw2oNcyohMkXTKjlr5T7kASfLHaHPV3mUrFJPFlWhwfEzJ/+Fie0HT0chcXJnxFu50hJT++IfVYtcYwc+gNHzvQc2E2mRt7IklkIjoxMDAyLCJFeHBpcnlEYXRlIjoiMjExNi0xMi0yN1QwMDowMDowMFoiLCJUeXBlIjoiVGVzdCJ9";
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1001-" + Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello world"));
            ExceptionAssert.Throws<JSchemaException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 28, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");
        }
    }
}