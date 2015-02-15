#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Load
{
    [TestFixture]
    public class JSchemaUrlResolverHttp : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            // resolver will fetch 'http://schema.org/address.json' with a web request as the parent schema is loaded
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

            Assert.IsTrue(isValid);
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