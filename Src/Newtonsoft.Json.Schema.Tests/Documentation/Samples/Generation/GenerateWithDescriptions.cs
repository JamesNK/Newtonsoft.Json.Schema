#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(PORTABLE)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
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
        [Description("The mailing address.")]
        public class PostalAddress
        {
            [Description("The street address. For example, 1600 Amphitheatre Pkwy.")]
            public string StreetAddress { get; set; }

            [Description("The locality. For example, Mountain View.")]
            public string AddressLocality { get; set; }

            [Description("The region. For example, CA.")]
            public string AddressRegion { get; set; }

            [Description("The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code.")]
            public string AddressCountry { get; set; }

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
            //   "description": "The mailing address.",
            //   "type": "object",
            //   "properties": {
            //     "StreetAddress": {
            //       "description": "The street address. For example, 1600 Amphitheatre Pkwy.",
            //       "type": "string"
            //     },
            //     "AddressLocality": {
            //       "description": "The locality. For example, Mountain View.",
            //       "type": "string"
            //     },
            //     "AddressRegion": {
            //       "description": "The region. For example, CA.",
            //       "type": "string"
            //     },
            //     "AddressCountry": {
            //       "description": "The country. For example, USA. You can also provide the two-letter ISO 3166-1 alpha-2 country code.",
            //       "type": "string"
            //     },
            //     "PostalCode": {
            //       "description": "The postal code. For example, 94043.",
            //       "type": "string"
            //     }
            //   }
            // }
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual("The mailing address.", schema.Description);
        }
    }
}

#endif