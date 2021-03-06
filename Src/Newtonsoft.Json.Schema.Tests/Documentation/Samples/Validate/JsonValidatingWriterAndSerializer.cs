#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Validate
{
    [TestFixture]
    public class JsonValidatingWriterAndSerializer : TestFixtureBase
    {
        #region Types
        public class Person
        {
            public string Name { get; set; }
            public IList<string> Hobbies { get; set; }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'hobbies': {
                  'type': 'array',
                  'items': {'type':'string'}
                }
              }
            }";

            Person p = new Person
            {
                Name = "James",
                Hobbies = new List<string>
                {
                    ".NET", "Blogging", "Reading", "Xbox", "LOLCATS"
                }
            };

            StringWriter stringWriter = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented
            };

            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer)
            {
                Schema = JSchema.Parse(schemaJson)
            };

            IList<string> messages = new List<string>();
            validatingWriter.ValidationEventHandler += (o, a) => messages.Add(a.Message);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(validatingWriter, p);

            Console.WriteLine(stringWriter);
            // {
            //   "Name": "James",
            //   "Hobbies": [
            //     ".NET",
            //     "Blogging",
            //     "Reading",
            //     "Xbox",
            //     "LOLCATS"
            //   ]
            // }

            bool isValid = (messages.Count == 0);

            Console.WriteLine(isValid);
            // true
            #endregion

            Assert.IsTrue(isValid);
        }
    }
}