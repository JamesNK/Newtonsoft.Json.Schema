#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Validate
{
    [TestFixture]
    public class JTokenIsValidWithValidationErrors : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            string schemaJson = @"{
              'description': 'Collection of non-primary colors',
              'type': 'array',
              'items': {
                'allOf': [ { '$ref': '#/definitions/hexColor' } ],
                'not': {
                  'enum': ['#FF0000','#00FF00','#0000FF']
                }
              },
              'definitions': {
                'hexColor': {
                  'type': 'string',
                  'pattern': '^#[A-Fa-f0-9]{6}$'
                }
              }
            }";

            JSchema schema = JSchema.Parse(schemaJson);

            JArray colors = JArray.Parse(@"[
              '#DAA520', // goldenrod
              '#FF69B4', // hot pink
              '#0000FF', // blue
              'Black'
            ]");

            IList<ValidationError> errors;
            bool valid = colors.IsValid(schema, out errors);
            // Message - JSON is valid against schema from 'not'. Path '[2]', line 4, position 24.
            // SchemaId - #/items/0

            // Message - JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path '[3]', line 5, position 22.
            // SchemaId - #/items/0
            //   Message - String 'Black' does not match regex pattern '^#[A-Fa-f0-9]{6}$'. Path '[3]', line 5, position 22.
            //   SchemaId - #/definitions/hexColor
            #endregion

            Assert.IsFalse(valid);
        }
    }
}