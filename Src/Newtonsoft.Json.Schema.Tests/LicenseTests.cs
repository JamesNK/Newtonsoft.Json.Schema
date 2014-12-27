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
            string licenseText = "1002-eyJJZCI6MTAwMiwiRXhwaXJ5RGF0ZSI6IjIxMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTfP13NkV6WHXZxxB/PNbQSsm+IrS+Fosod1S9xzED96YVRJaaS9xjXkqmA0EHMUs9+JVzh31k6yhHQMNqDXMqITJF0yo5a+U+5AEnyx2hz1d5lKxSTxZVocHxMyf/hYntB09HIXFyZ8RbudISU/viH1WLXGMHPoDR870HNhNpkb";

            ExceptionAssert.Throws<JsonException>(() => License.RegisterLicense(licenseText), "Specified license is for testing only.");
        }

        [Test]
        public void RegisterLicense_Failure()
        {
            string licenseText = "1003-eyJJZCI6MTAwMiwiRXhwaXJ5RGF0ZSI6IjIxMTYtMTItMjdUMDA6MDA6MDBaIiwiVHlwZSI6IlRlc3QifTfP13NkV6WHXZxxB/PNbQSsm+IrS+Fosod1S9xzED96YVRJaaS9xjXkqmA0EHMUs9+JVzh31k6yhHQMNqDXMqITJF0yo5a+U+5AEnyx2hz1d5lKxSTxZVocHxMyf/hYntB09HIXFyZ8RbudISU/viH1WLXGMHPoDR870HNhNpkb";

            ExceptionAssert.Throws<JsonException>(() => License.RegisterLicense(licenseText), "License ID does not match signature license ID.");
        }
    }
}