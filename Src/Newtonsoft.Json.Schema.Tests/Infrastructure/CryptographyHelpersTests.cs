#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class CryptographyHelpersTests : TestFixtureBase
    {
        [Test]
        public void ValidateData()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world");

            byte[] signature = Convert.FromBase64String("sAx4uDJZFOKcbhj+a65RdCVUF0E/kvluG7FnwGYWRIWNay3r/A7xw5p46shtT0HJrYKApIel00pY7rCdI/OVBP2VvCNvMiTD9VtY6LzMwOXcNKgBDAiSp/cu/ZvqQO7dCALmlBffj+5uKMtThbAgLN+i74wpI7Zt+2kwvQKbbR4=");

            bool valid = CryptographyHelpers.ValidateData(testData, signature);

            Assert.IsTrue(valid);
        }

        [Test]
        public void ValidateData_Failure()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world TEH HACKZOR!");

            byte[] signature = Convert.FromBase64String("sAx4uDJZFOKcbhj+a65RdCVUF0E/kvluG7FnwGYWRIWNay3r/A7xw5p46shtT0HJrYKApIel00pY7rCdI/OVBP2VvCNvMiTD9VtY6LzMwOXcNKgBDAiSp/cu/ZvqQO7dCALmlBffj+5uKMtThbAgLN+i74wpI7Zt+2kwvQKbbR4=");

            bool valid = CryptographyHelpers.ValidateData(testData, signature);

            Assert.IsFalse(valid);
        }
    }
}