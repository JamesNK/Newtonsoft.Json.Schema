#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Issues
{
    [TestFixture]
    public class Issue0176Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            // Arrange
            JSchema schema = JSchema.Parse(SchemaJson);
            JObject payload = JObject.Parse(Json);

            // Act
            bool result = payload.IsValid(schema, out IList<ValidationError> errors);

            // Assert
            Assert.AreEqual(new List<string> { "line1", "line2" }, errors[0].Value);
        }

        private const string SchemaJson = @"{
            'type': 'object',
            'properties': {
                    'address': {
                    'type': 'object',
                    'properties': {
                        'line1': {'type':'string'},
                        'line2': {'type':'string'}
                    },
                    'required': [ 'line1', 'line2' ]
                },
                'address2': {
                    'type': 'object',
                    'properties': {
                        'line1': {'type':'string'},
                        'line2': {'type':'string'}
                    },
                    'required': [ 'line1', 'line2' ]
                }
            },
            'required': [ 'address', 'address2' ]
        }";

        private const string Json = @"{
            'address': {},
            'address2': {
                'line1': 'Bar',
                'line2': 'Boo'
            }
        }";
    }
}
