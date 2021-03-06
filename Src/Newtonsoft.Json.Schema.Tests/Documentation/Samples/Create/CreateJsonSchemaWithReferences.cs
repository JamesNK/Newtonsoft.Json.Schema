#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Create
{
    [TestFixture]
    public class CreateJsonSchemaWithReferences : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            JSchema addressSchema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    { "street", new JSchema { Type = JSchemaType.String } },
                    { "city", new JSchema { Type = JSchemaType.String } },
                    { "country", new JSchema { Type = JSchemaType.String } },
                    { "postCode", new JSchema { Type = JSchemaType.Integer } }
                }
            };

            JSchema deliverySchema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    { "date", new JSchema { Type = JSchemaType.String, Format = "date-time" } },
                    { "fromAddress", addressSchema },
                    { "toAddress", addressSchema }
                },
                ExtensionData =
                {
                    ["references"] = new JObject
                    {
                        ["address"] = addressSchema
                    }
                }
            };

            string schemaJson = deliverySchema.ToString();

            Console.WriteLine(schemaJson);
            // {
            //   "references": {
            //     "address": {
            //       "type": "object",
            //       "properties": {
            //         "street": {
            //           "type": "string"
            //         },
            //         "city": {
            //           "type": "string"
            //         },
            //         "country": {
            //           "type": "string"
            //         },
            //         "postCode": {
            //           "type": "integer"
            //         }
            //       }
            //     }
            //   },
            //   "type": "object",
            //   "properties": {
            //     "date": {
            //       "type": "string",
            //       "format": "date-time"
            //     },
            //     "fromAddress": {
            //       "$ref": "#/references/address"
            //     },
            //     "toAddress": {
            //       "$ref": "#/references/address"
            //     }
            //   }
            // }

            JObject delivery = JObject.Parse(@"{
              'date': '2017-01-14T20:34:22Z',
              'fromAddress': {'street':'33 Smith Street','City':'Wellington','Country':'NZ','postCode':6011},
              'toAddress': {'street':'2 Hurman Street','City':'Wellington','Country':'NZ','postCode':6005}
            }");

            bool valid = delivery.IsValid(deliverySchema);

            Console.WriteLine(valid);
            // true
            #endregion

            Assert.IsTrue(valid);
        }
    }
}