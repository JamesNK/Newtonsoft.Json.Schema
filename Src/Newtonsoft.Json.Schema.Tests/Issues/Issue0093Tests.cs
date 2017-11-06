#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(NET35)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0093Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaGenerator schemaGenerator = new JSchemaGenerator();
            JSchema schema = schemaGenerator.Generate(typeof(PostalAddress));

            string json = @"{
  ""title"": ""Postal Address"",
  ""description"": ""The mailing address."",
  ""type"": ""object"",
  ""properties"": {
    ""StreetAddress"": {
      ""title"": ""Street Address"",
      ""description"": ""The street address. For example, 1600 Amphitheatre Pkwy."",
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""AddressLocality"": {
      ""title"": ""Locality"",
      ""description"": ""The locality. For example, Mountain View."",
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""AddressRegion"": {
      ""title"": ""Region"",
      ""description"": ""The region. For example, CA."",
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""AddressCountry"": {
      ""title"": ""Country"",
      ""description"": ""The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code."",
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""PostalCode"": {
      ""title"": ""Postal Code"",
      ""description"": ""The postal code. For example, 94043."",
      ""type"": [
        ""string"",
        ""null""
      ]
    }
  },
  ""required"": [
    ""StreetAddress"",
    ""AddressLocality"",
    ""AddressRegion"",
    ""AddressCountry"",
    ""PostalCode""
  ]
}";

            StringAssert.AreEqual(json, schema.ToString());
        }

        [DisplayName("Postal Address")]
        [Description("The mailing address.")]
        public class PostalAddress
        {
            [Display(Name = nameof(PostalAddressResource.PortalAddressName), Description = nameof(PostalAddressResource.PortalAddressDescription), ResourceType = typeof(PostalAddressResource))]
            public string StreetAddress { get; set; }

            [Display(Name = "Locality", Description = "The locality. For example, Mountain View.")]
            public string AddressLocality { get; set; }

            [Display(Name = "Region", Description = "The region. For example, CA.")]
            public string AddressRegion { get; set; }

            [Display(Name = "Country")]
            [Description("The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code.")]
            public string AddressCountry { get; set; }

            [Display(Description = "The postal code. For example, 94043.")]
            [DisplayName("Postal Code")]
            public string PostalCode { get; set; }
        }

        public static class PostalAddressResource
        {
            public static string PortalAddressName { get; } = "Street Address";
            public static string PortalAddressDescription { get; } = "The street address. For example, 1600 Amphitheatre Pkwy.";
        }
    }
}
#endif