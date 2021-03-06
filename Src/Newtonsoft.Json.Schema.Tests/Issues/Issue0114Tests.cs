#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
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
        private static readonly string _personSchemaJson = @"{
              'type': 'object',
              'properties': {
                'name': { 'type': 'string' },
                'age': { 'type': 'integer' }
              }
            }";

        // the external 'person.json' schema will be found using the resolver
        // the internal 'salary' schema will be found using the default resolution logic
        private static readonly string _employeeSchema = @"{
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

        [Test]
        public void ReadRoot_ResolveSchemaReferencesTrue_Dereferences()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("person.json", UriKind.RelativeOrAbsolute), _personSchemaJson);

            // Arrange
            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings { Resolver = resolver });

            // Act
            JSchema schema = schemaReader.ReadRoot(new JsonTextReader(new StringReader(_employeeSchema)));

            // Assert
            Assert.AreEqual(null, schema.AllOf[0].Reference);
            Assert.AreEqual(null, schema.Properties["salary"].Reference);
        }

        [Test]
        public void ReadRoot_ResolveSchemaReferencesFalse_DoesNotDereference()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("person.json", UriKind.RelativeOrAbsolute), _personSchemaJson);

            // Arrange
            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings { ResolveSchemaReferences = false, Resolver = resolver });

            // Act
            JSchema schema = schemaReader.ReadRoot(new JsonTextReader(new StringReader(_employeeSchema)));

            // Assert
            Assert.AreEqual(new Uri("person.json", UriKind.Relative), schema.AllOf[0].Reference);
            Assert.AreEqual(new Uri("#/definitions/salary", UriKind.Relative), schema.Properties["salary"].Reference);
        }

        [Test]
        public void Validation_SchemaWithReferences_Errors()
        {
            JSchema s = new JSchema();
            s.Properties["property"] = new JSchema
            {
                Reference = new Uri("person.json#hi", UriKind.RelativeOrAbsolute)
            };

            JObject o = new JObject
            {
                ["property"] = 1
            };

            ExceptionAssert.Throws<JSchemaException>(() =>
            {
                o.IsValid(s);
            }, "Schema has unresolved reference 'person.json#hi'. All references must be resolved before a schema can be validated.");
        }
    }
}