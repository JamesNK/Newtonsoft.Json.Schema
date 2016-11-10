#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
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
    public class CreateJSchemaManually : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            JSchema schema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    { "name", new JSchema { Type = JSchemaType.String } },
                    {
                        "hobbies", new JSchema
                        {
                            Type = JSchemaType.Array,
                            Items = { new JSchema { Type = JSchemaType.String } }
                        }
                    }
                }
            };

            string schemaJson = schema.ToString();

            Console.WriteLine(schemaJson);
            // {
            //   "type": "object",
            //   "properties": {
            //     "name": {
            //       "type": "string"
            //     },
            //     "hobbies": {
            //       "type": "array",
            //       "items": {
            //         "type": "string"
            //       }
            //     }
            //   }
            // }

            JObject person = JObject.Parse(@"{
              'name': 'James',
              'hobbies': ['.NET', 'Blogging', 'Reading', 'Xbox', 'LOLCATS']
            }");

            bool valid = person.IsValid(schema);

            Console.WriteLine(valid);
            // true
            #endregion

            Assert.IsTrue(valid);
        }
    }
}