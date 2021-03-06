#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
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
using System.Globalization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using File = Newtonsoft.Json.Schema.Tests.Documentation.Samples.File;

namespace Newtonsoft.Json.Schema.Tests.Documentation
{
    [TestFixture]
    public class ValidatingJson : TestFixtureBase
    {
        [Test]
        public void IsValidBasic()
        {
            #region IsValidBasic
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties': {
                'name': {'type': 'string'},
                'hobbies': {
                  'type': 'array',
                  'items': {'type': 'string'}
                }
              }
            }";

            JSchema schema = JSchema.Parse(schemaJson);

            JObject person = JObject.Parse(@"{
              'name': 'James',
              'hobbies': ['.NET', 'Blogging', 'Reading', 'Xbox', 'LOLCATS']
            }");

            bool valid = person.IsValid(schema);
            // true
            #endregion

            Assert.IsTrue(valid);
        }

        [Test]
        public void IsValidMessages()
        {
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties': {
                'name': {'type': 'string'},
                'hobbies': {
                  'type': 'array',
                  'items': {'type': 'string'}
                }
              }
            }";

            #region IsValidMessages
            JSchema schema = JSchema.Parse(schemaJson);

            JObject person = JObject.Parse(@"{
              'name': null,
              'hobbies': ['Invalid content', 0.123456789]
            }");

            bool valid = person.IsValid(schema, out IList<string> messages);
            // Invalid type. Expected String but got Null. Line 2, position 21.
            // Invalid type. Expected String but got Number. Line 3, position 51.
            #endregion

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidValidationError()
        {
            #region IsValidValidationError
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

            bool valid = colors.IsValid(schema, out IList<ValidationError> errors);
            // Message - JSON is valid against schema from 'not'. Path '[2]', line 4, position 24.
            // SchemaId - #/items/0

            // Message - JSON does not match all schemas from 'allOf'. Invalid schema indexes: 0. Path '[3]', line 5, position 22.
            // SchemaId - #/items/0
            //   Message - String 'Black' does not match regex pattern '^#[A-Fa-f0-9]{6}$'. Path '[3]', line 5, position 22.
            //   SchemaId - #/definitions/hexColor
            #endregion

            Assert.IsFalse(valid);
        }

        [Test]
        public void JSchemaValidatingReader()
        {
            string schemaJson = "{}";

            #region JSchemaValidatingReader
            string json = @"{
              'name': 'James',
              'hobbies': ['.NET', 'Blogging', 'Reading', 'Xbox', 'LOLCATS']
            }";

            JsonTextReader reader = new JsonTextReader(new StringReader(json));

            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader)
            {
                Schema = JSchema.Parse(schemaJson)
            };

            IList<string> messages = new List<string>();
            validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);

            JsonSerializer serializer = new JsonSerializer();
            Person p = serializer.Deserialize<Person>(validatingReader);
            #endregion

            Assert.AreEqual(0, messages.Count);
        }

        public class Person
        {
            public string Name { get; set; }
            public List<string> Hobbies { get; set; }
        }

        [Test]
        public void JSchemaValidatingWriter()
        {
            string schemaJson = "{}";

            #region JSchemaValidatingWriter
            Person person = new Person
            {
                Name = "James",
                Hobbies = new List<string>
                {
                    ".NET", "Blogging", "Reading", "Xbox", "LOLCATS"
                }
            };

            StringWriter stringWriter = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(stringWriter);

            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer)
            {
                Schema = JSchema.Parse(schemaJson)
            };

            IList<string> messages = new List<string>();
            validatingWriter.ValidationEventHandler += (o, a) => messages.Add(a.Message);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(validatingWriter, person);
            #endregion

            Assert.AreEqual(0, messages.Count);
        }
    }
}

#endif