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
using System.IO;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Load
{
    [TestFixture]
    public class JSchemaUrlResolverRelative : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            // person.json, has a relative external schema reference 'address.json'
            // --------
            // {
            //   'type': 'object',
            //   'properties': {
            //     'name': {'type':'string'},
            //     'addresses': {
            //       'type': 'array',
            //       'items': {'$ref': 'address.json'}
            //     }
            //   }
            // }
            // --------

            using (StreamReader file = File.OpenText(@"c:\person.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JSchemaUrlResolver resolver = new JSchemaUrlResolver();

                JSchema schema = JSchema.Load(reader, new JSchemaReaderSettings
                {
                    Resolver = resolver,
                    // where the schema is being loaded from
                    // referenced 'address.json' schema will be loaded from disk at 'c:\address.json'
                    BaseUri = new Uri(@"c:\person.json")
                });

                // validate JSON
            }
            #endregion
        }

        public class JSchemaUrlResolver : JSchemaPreloadedResolver
        {
            public JSchemaUrlResolver()
            {
                JSchema subSchema = JSchema.Parse(@"{
                    ""type"": ""object""
                }");

                Add(new Uri("http://schema.org/address.json"), subSchema.ToString());
            }
        }
    }
}