#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Load
{
    [TestFixture]
    public class RefJSchemaResolver : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            string personSchemaJson = @"{
              'type': 'object',
              'properties': {
                'name': { 'type': 'string' },
                'age': { 'type': 'integer' }
              }
            }";

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("person.json", UriKind.RelativeOrAbsolute), personSchemaJson);

            // the external 'person' schema will be found using the resolver
            // the internal 'salary' schema will be found using the default resolution logic
            JSchema employeeSchema = JSchema.Parse(@"{
              'type': 'object',
              'allOf': [
                { '$ref': 'person.json' }
              ],
              'properties': {
                'salary': { '$ref': '#/definitions/salary' },
                'jobTitle': { 'type': 'string' }
              },
              'definitions': {
                'salary': { 'type': 'number' }
              }
            }", resolver);

            string json = @"{
              'name': 'James',
              'age': 29,
              'salary': 9000.01,
              'jobTitle': 'Junior Vice President'
            }";

            JObject employee = JObject.Parse(json);

            bool valid = employee.IsValid(employeeSchema);

            Console.WriteLine(valid);
            // true
            #endregion

            Assert.IsTrue(valid);
        }
    }
}