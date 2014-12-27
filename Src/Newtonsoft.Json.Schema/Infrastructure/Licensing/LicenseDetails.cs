#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Globalization;
using System.Text;

namespace Newtonsoft.Json.Schema.Infrastructure.Licensing
{
    internal class LicenseDetails
    {
        public int Id { get; set; }
        public DateTime ExpiryDate { get; set; }
        public LicenseType Type { get; set; }

        internal byte[] GetSignificateData()
        {
            string s = string.Join(":", new[]
            {
                Id.ToString(CultureInfo.InvariantCulture),
                ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Type.ToString()
            });

            return Encoding.UTF8.GetBytes(s);
        }
    }
}