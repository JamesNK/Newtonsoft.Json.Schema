#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Serialization;
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
    public class Issue0114Test : TestFixtureBase
    {
        private string _personSchemaJson;
        private string _employeeSchema;
        private JSchemaPreloadedResolver _resolver;

        [SetUp]
        public void Setup()
        {
            _personSchemaJson = @"{
              'type': 'object',
              'properties': {
                'name': { 'type': 'string' },
                'age': { 'type': 'integer' }
              }
            }";

            // the external 'person.json' schema will be found using the resolver
            // the internal 'salary' schema will be found using the default resolution logic
            _employeeSchema = @"{
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
            }";

            _resolver = new JSchemaPreloadedResolver();
            _resolver.Add(new Uri("person.json", UriKind.RelativeOrAbsolute), _personSchemaJson);
        }

        [Test]
        public void JSchemaReader_ReadRoot_Dereferences()
        {
            // Arrange
            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings { Resolver = _resolver });

            // Act
            JSchema schema = schemaReader.ReadRoot(new JsonTextReader(new StringReader(_employeeSchema)));

            // Assert
            Assert.AreEqual(new Uri("person.json", UriKind.Relative), schema.AllOf[0].Reference);
            Assert.AreEqual(true, schema.AllOf[0].IsReferenceResolved);
            Assert.AreEqual(new Uri("#/definitions/salary", UriKind.Relative), schema.Properties["salary"].Reference);
            Assert.AreEqual(true, schema.Properties["salary"].IsReferenceResolved);
        }

        [Test]
        public void JSchemaReader_ReadRoot_DoesNotDereference()
        {
            // Arrange
            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings { ResolveDeferedSchemas = false, Resolver = _resolver });

            // Act
            JSchema schema = schemaReader.ReadRoot(new JsonTextReader(new StringReader(_employeeSchema)));

            // Assert
            Assert.AreEqual(new Uri("person.json", UriKind.Relative), schema.AllOf[0].Reference);
            Assert.AreEqual(false, schema.AllOf[0].IsReferenceResolved);
            Assert.AreEqual(new Uri("#/definitions/salary", UriKind.Relative), schema.Properties["salary"].Reference);
            Assert.AreEqual(false, schema.Properties["salary"].IsReferenceResolved);
        }
    }
}