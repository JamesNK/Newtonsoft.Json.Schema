#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
using Newtonsoft.Json.Schema.Tests;
#if !(NET35 || NET20 || PORTABLE || ASPNETCORE50)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
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
    public class ValidatingJson : TestFixtureBase
    {
        [Test]
        public void IsValidBasic()
        {
            #region IsValidBasic
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties':
              {
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
               'properties':
               {
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

            IList<string> messages;
            bool valid = person.IsValid(schema, out messages);
            // false
            // Invalid type. Expected String but got Null. Line 2, position 21.
            // Invalid type. Expected String but got Float. Line 3, position 51.
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

            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = JSchema.Parse(schemaJson);

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

            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = JSchema.Parse(schemaJson);

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