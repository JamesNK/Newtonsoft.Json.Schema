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

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Validate
{
    [TestFixture]
    public class JTokenIsValidWithMessages : TestFixtureBase
    {
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

            JSchema schema = JSchema.Parse(schemaJson);

            JObject person = JObject.Parse(@"{
              'name': null,
              'hobbies': ['Invalid content', 0.123456789]
            }");

            IList<string> messages;
            bool valid = person.IsValid(schema, out messages);

            Console.WriteLine(valid);
            // false

            foreach (string message in messages)
            {
                Console.WriteLine(message);
            }
            // Invalid type. Expected String but got Null. Line 2, position 21.
            // Invalid type. Expected String but got Number. Line 3, position 51.
            #endregion

            Assert.AreEqual(2, messages.Count);
        }
    }
}