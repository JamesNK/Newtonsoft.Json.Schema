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
            string schemaJson = @"{
              'id': 'person',
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'age': {'type':'integer'}
              }
            }";

            JSchema personSchema = JSchema.Parse(schemaJson);

            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(personSchema);

            schemaJson = @"{
              'id': 'employee',
              'type': 'object',
              'extends': {'$ref':'person'},
              'properties': {
                'salary': {'type':'number'},
                'jobTitle': {'type':'string'}
              }
            }";

            JSchema employeeSchema = JSchema.Parse(schemaJson, resolver);

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