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
        public void ValidateData_Sha1()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world");

            byte[] signature = Convert.FromBase64String("sAx4uDJZFOKcbhj+a65RdCVUF0E/kvluG7FnwGYWRIWNay3r/A7xw5p46shtT0HJrYKApIel00pY7rCdI/OVBP2VvCNvMiTD9VtY6LzMwOXcNKgBDAiSp/cu/ZvqQO7dCALmlBffj+5uKMtThbAgLN+i74wpI7Zt+2kwvQKbbR4=");

            bool valid = CryptographyHelpers.ValidateData(testData, signature, HashAlgorithm.SHA1);

            Assert.IsTrue(valid);
        }

        [Test]
        public void ValidateData_Sha1_Failure()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world TEH HACKZOR!");

            byte[] signature = Convert.FromBase64String("sAx4uDJZFOKcbhj+a65RdCVUF0E/kvluG7FnwGYWRIWNay3r/A7xw5p46shtT0HJrYKApIel00pY7rCdI/OVBP2VvCNvMiTD9VtY6LzMwOXcNKgBDAiSp/cu/ZvqQO7dCALmlBffj+5uKMtThbAgLN+i74wpI7Zt+2kwvQKbbR4=");

            bool valid = CryptographyHelpers.ValidateData(testData, signature, HashAlgorithm.SHA1);

            Assert.IsFalse(valid);
        }

#if !(NETSTANDARD2_0 || NET35)
        [Test]
        public void ValidateData_Sha256()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world");

            byte[] signature = Convert.FromBase64String("LoHhY2xZZnImlTv2mAqEeLjyNxRr+LFJUCTE70eqP8qeKqPCsG7lSEy6sVBv3UMxTdoYQlZrvC8DSID4+m/p8I4oh5NwuTeP924ekbjmJzajjD3wG/crIZQ1LX0YqrBmaIlm0i7E/6vWArwSA+z6szy2AtkgL+/q7EAHJdGcxU/BfURp9g09bllOWf2gugpcr1cbPljSK2apBt67Rus8xftiyVjFJVj5r4grp8wi6dmrMZ23i2yVj1Xc7O7uvEUczARo7IkyswkxE4e952JevP+teg9kJ/IRpKTz9qDuuVWsl9TnlP6M9BBHo9AeQL2E1f5Ffi1PrQ6AZEK1WNrWdA==");

            bool valid = CryptographyHelpers.ValidateData(testData, signature, HashAlgorithm.SHA256);

            Assert.IsTrue(valid);
        }

        [Test]
        public void ValidateData_Sha256_Failure()
        {
            byte[] testData = Encoding.UTF8.GetBytes("Hello world TEH HACKZOR!");

            byte[] signature = Convert.FromBase64String("LoHhY2xZZnImlTv2mAqEeLjyNxRr+LFJUCTE70eqP8qeKqPCsG7lSEy6sVBv3UMxTdoYQlZrvC8DSID4+m/p8I4oh5NwuTeP924ekbjmJzajjD3wG/crIZQ1LX0YqrBmaIlm0i7E/6vWArwSA+z6szy2AtkgL+/q7EAHJdGcxU/BfURp9g09bllOWf2gugpcr1cbPljSK2apBt67Rus8xftiyVjFJVj5r4grp8wi6dmrMZ23i2yVj1Xc7O7uvEUczARo7IkyswkxE4e952JevP+teg9kJ/IRpKTz9qDuuVWsl9TnlP6M9BBHo9AeQL2E1f5Ffi1PrQ6AZEK1WNrWdA==");

            bool valid = CryptographyHelpers.ValidateData(testData, signature, HashAlgorithm.SHA256);

            Assert.IsFalse(valid);
        }
#endif
    }
}