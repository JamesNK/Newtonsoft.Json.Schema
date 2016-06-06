#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
using Newtonsoft.Json.Schema.Tests;
#if !(NET35 || NET20 || PORTABLE || DNXCORE50)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema.Tests.TestObjects;
using Newtonsoft.Json.Utilities;
using System.Globalization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using File = Newtonsoft.Json.Schema.Tests.Documentation.Samples.File;

namespace Newtonsoft.Json.Schema.Tests.Documentation
{
    [TestFixture]
    public class LoadingSchemas : TestFixtureBase
    {
        [Test]
        public void ParseString()
        {
#region ParseString
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties':
              {
                'name': {'type':['string','null']},
                'hobbies': {
                  'type': 'array',
                  'items': {'type':'string'}
                }
              }
            }";

            JSchema schema = JSchema.Parse(schemaJson);

            // validate JSON
#endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(JSchemaType.String | JSchemaType.Null, schema.Properties["name"].Type);
        }

        [Test]
        public void LoadFile()
        {
#region LoadFile
            using (StreamReader file = File.OpenText(@"c:\person.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JSchema schema = JSchema.Load(reader);

                // validate JSON
            }
#endregion
        }

        public class JSchemaUrlResolver : JSchemaPreloadedResolver
        {
            public JSchemaUrlResolver()
            {
                JSchema subSchema = JSchema.Parse(@"{
                    ""type"": ""integer""
                }");

                Add(new Uri("http://schema.org/address.json"), subSchema.ToString());
            }
        }

        [Test]
        public void JSchemaUrlResolverTest()
        {
#region JSchemaUrlResolver
            // resolver will fetch 'http://schema.org/address.json' as the parent schema is loaded
            JSchemaUrlResolver resolver = new JSchemaUrlResolver();

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'addresses': {
                  'type': 'array',
                  'items': {'$ref': 'http://schema.org/address.json'}
                }
              }
            }", resolver);

            JToken json = JToken.Parse(@"{
              'name': 'James',
              'addresses': [
                {
                  'line1': '99 Smithington Street',
                  'line2': 'Real Town',
                  'country': 'USA'
                }
              ]
            }");

            IList<string> errorMessages;
            bool isValid = json.IsValid(schema, out errorMessages);
#endregion
        }

        [Test]
        public void JSchemaUrlResolverRelativeTest()
        {
#region JSchemaUrlResolver_Relative
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

        [Test]
        public void JSchemaPreloadedResolver()
        {
#region JSchemaPreloadedResolver
            string addressSchemaJson = @"{
              'type': 'object',
              'properties': {
                'line1': {'type': 'string'},
                'line2': {'type': 'string'},
                'country': {'type': 'string'}
              }
            }";

            // preload schema with ID 'http://schema.org/address.json'
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://schema.org/address.json"), addressSchemaJson);

            // the external ref will use the preloaded schema
            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'addresses': {
                  'type': 'array',
                  'items': {'$ref': 'http://schema.org/address.json'}
                }
              }
            }", resolver);

            JToken json = JToken.Parse(@"{
              'name': 'James',
              'addresses': [
                {
                  'line1': '99 Smithington Street',
                  'line2': 'Real Town',
                  'Country': 'USA'
                }
              ]
            }");

            IList<string> errorMessages;
            bool isValid = json.IsValid(schema, out errorMessages);
#endregion
        }
    }
}

#endif