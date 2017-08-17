#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Generation
{
    [TestFixture]
    public class GenerateWithDescriptions : TestFixtureBase
    {
        #region Types
        [DisplayName("Postal Address")]
        [Description("The mailing address.")]
        public class PostalAddress
        {
            [DisplayName("Street Address")]
            [Description("The street address. For example, 1600 Amphitheatre Pkwy.")]
            public string StreetAddress { get; set; }

            [DisplayName("Locality")]
            [Description("The locality. For example, Mountain View.")]
            public string AddressLocality { get; set; }

            [DisplayName("Region")]
            [Description("The region. For example, CA.")]
            public string AddressRegion { get; set; }

            [DisplayName("Country")]
            [Description("The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code.")]
            public string AddressCountry { get; set; }

            [DisplayName("Postal Code")]
            [Description("The postal code. For example, 94043.")]
            public string PostalCode { get; set; }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.DefaultRequired = Required.DisallowNull;

            JSchema schema = generator.Generate(typeof(PostalAddress));
            // {
            //   "title": "Postal Address",
            //   "description": "The mailing address.",
            //   "type": "object",
            //   "properties": {
            //     "StreetAddress": {
            //       "title": "Street Address",
            //       "description": "The street address. For example, 1600 Amphitheatre Pkwy.",
            //       "type": "string"
            //     },
            //     "AddressLocality": {
            //       "title": "Locality",
            //       "description": "The locality. For example, Mountain View.",
            //       "type": "string"
            //     },
            //     "AddressRegion": {
            //       "title": "Region",
            //       "description": "The region. For example, CA.",
            //       "type": "string"
            //     },
            //     "AddressCountry": {
            //       "title": "Country",
            //       "description": "The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code.",
            //       "type": "string"
            //     },
            //     "PostalCode": {
            //       "title": "Postal Code",
            //       "description": "The postal code. For example, 94043.",
            //       "type": "string"
            //     }
            //   }
            // }
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual("Postal Address", schema.Title);
            Assert.AreEqual("The mailing address.", schema.Description);

            Assert.AreEqual("Postal Code", schema.Properties["PostalCode"].Title);
            Assert.AreEqual("The postal code. For example, 94043.", schema.Properties["PostalCode"].Description);
        }
    }
}