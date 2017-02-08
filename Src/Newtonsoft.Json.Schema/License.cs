#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Infrastructure.Licensing;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Manages the license used with Json.NET Schema. A license can be purchased at <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see>.
    /// </summary>
    public static class License
    {
        /// <summary>
        /// Register the specified license with Json.NET Schema. A license can be purchased at <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see>.
        /// </summary>
        /// <param name="license">The license text to register.</param>
        /// <remarks> 
        /// The recommended way to register the license key is to call <see cref="RegisterLicense"/> once during application start up.
        /// In ASP.NET web applications it can be placed in the <c>Startup.cs</c> or <c>Global.asax.cs</c>,
        /// in WPF applications it can be placed in the <c>Application.Startup</c> event,
        /// and in Console applications it can be placed in the <c>static void Main(string[] args)</c> event.
        /// </remarks>
        /// <example> 
        /// This sample shows how to register a Json.NET Schema license with the <see cref="RegisterLicense"/> method.
        /// <code>
        /// // replace with your license key
        /// string licenseKey = "json-schema-license-key";
        /// License.RegisterLicense(licenseKey);
        /// </code>
        /// </example>
        public static void RegisterLicense(string license)
        {
            LicenseHelpers.RegisterLicense(license);
        }
    }
}