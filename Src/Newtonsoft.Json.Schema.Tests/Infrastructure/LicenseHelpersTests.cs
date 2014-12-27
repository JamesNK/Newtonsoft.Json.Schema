#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using NUnit.Framework;

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

            string licenseText = "1001-eyJJZCI6MTAwMSwiRXhwaXJ5RGF0ZSI6IjIwMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTe9KjOZYrE4JlyDUF64TO432y8TLNEZKvJxeVddMn+DjI0mUMiPyEfDs/YXyMhLOYUsyPHl/HRW9SnCrDMkk7kP8B1OhCl+ks7AR82zksR4Y0eV7gRVM5xjPrtRciu4RjsbMkWYAQ0tqwpXA//W35+0oIrUFOJ3dXxl550CtbOY";

            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified license is for testing only.");
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "Specified license is for testing only.");
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 28, 0, 0, 0, DateTimeKind.Utc)), "License is not valid for this version of Json.NET Schema. License expired on 2016-12-27. This version of Json.NET Schema was released on 2016-12-28.");

            licenseText = "1001-eyJJZCI6MTAwMSwiRXhwaXJ5RGF0ZSI6IjIwMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTe9KjOZYrE4JlyDUF64TO432y8TLNEZKvJxeVddMn+DjI0mUMiPyEfDs/YXyMhLOYUsyPHl/HRW9SnCrDMkk7kP8B1OhCl+ks7AR82zksR4Y0eV7gRVM5xjPrtRciu4RjsbMkWYAQ0tqwpXA//W35+0oIrUFOJ3dXxl550CtBOY";
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "License text does not match signature.");
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
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "License text is empty.");

            licenseText = "1002";
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1002-eyJJZCI6MTAwMSwiRXhwaXJ5RGF0ZSI6IjIwMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTe9KjOZYrE4JlyDUF64TO432y8TLNEZKvJxeVddMn+DjI0mUMiPyEfDs/YXyMhLOYUsyPHl/HRW9SnCrDMkk7kP8B1OhCl+ks7AR82zksR4Y0eV7gRVM5xjPrtRciu4RjsbMkWYAQ0tqwpXA//W35+0oIrUFOJ3dXxl550CtbOY";
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "License ID does not match signature license ID.");

            licenseText = "PIE-eyJJZCI6MTAwMSwiRXhwaXJ5RGF0ZSI6IjIwMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTe9KjOZYrE4JlyDUF64TO432y8TLNEZKvJxeVddMn+DjI0mUMiPyEfDs/YXyMhLOYUsyPHl/HRW9SnCrDMkk7kP8B1OhCl+ks7AR82zksR4Y0eV7gRVM5xjPrtRciu4RjsbMkWYAQ0tqwpXA//W35+0oIrUFOJ3dXxl550CtbOY";
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1001-ZZeyJJZCI6MTAwMSwiRXhwaXJ5RGF0ZSI6IjIwMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTe9KjOZYrE4JlyDUF64TO432y8TLNEZKvJxeVddMn+DjI0mUMiPyEfDs/YXyMhLOYUsyPHl/HRW9SnCrDMkk7kP8B1OhCl+ks7AR82zksR4Y0eV7gRVM5xjPrtRciu4RjsbMkWYAQ0tqwpXA//W35+0oIrUFOJ3dXxl550CtbOY";
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 27, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");

            licenseText = "1001-" + Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello world"));
            ExceptionAssert.Throws<JsonException>(() => LicenseHelpers.RegisterLicense(licenseText, new DateTime(2016, 12, 28, 0, 0, 0, DateTimeKind.Utc)), "Specified license text is invalid.");
        }
    }
}