#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Manages the license used with Json.NET Schema. A license can be purchased at <see href="http://www.newtonsoft.com">http://www.newtonsoft.com</see>.
    /// </summary>
    public static class License
    {
        /// <summary>
        /// Register the specified license with Json.NET Schema. A license can be purchased at <see href="http://www.newtonsoft.com">http://www.newtonsoft.com</see>.
        /// </summary>
        /// <param name="license">The license text to register.</param>
        public static void RegisterLicense(string license)
        {
            LicenseHelpers.RegisterLicense(license);
        }
    }
}