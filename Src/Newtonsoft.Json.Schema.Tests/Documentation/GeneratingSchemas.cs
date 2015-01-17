#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json.Schema.Generation;
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
    public class GeneratingSchemas : TestFixtureBase
    {
        #region PersonClass
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        #endregion

        [Test]
        public void BasicGeneration()
        {
            #region BasicGeneration
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(Person));
            //{
            //  "type": "object",
            //  "properties": {
            //    "Name": {
            //      "type": [ "string", "null" ]
            //    },
            //    "Age": { "type": "integer" }
            //  },
            //  "required": [ "Name", "Age" ]
            //}
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
        }

        [Test]
        public void IContractResolver()
        {
            #region IContractResolver
            JSchemaGenerator generator = new JSchemaGenerator();

            // change contract resolver so property names are camel case
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            JSchema schema = generator.Generate(typeof(Person));
            //{
            //  "type": "object",
            //  "properties": {
            //    "name": {
            //      "type": [ "string", "null" ]
            //    },
            //    "age": { "type": "integer" }
            //  },
            //  "required": [ "name", "age" ]
            //}
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(true, schema.Properties.ContainsKey("name"));
        }

        [Test]
        public void JSchemaUndefinedIdHandling()
        {
            #region JSchemaUndefinedIdHandling
            JSchemaGenerator generator = new JSchemaGenerator();

            // types with no defined ID have their type name as the ID
            generator.UndefinedSchemaIdHandling = Schema.JSchemaUndefinedIdHandling.UseTypeName;

            JSchema schema = generator.Generate(typeof(Person));
            //{
            //  "id": "Person",
            //  "type": "object",
            //  "properties": {
            //    "name": {
            //      "type": [ "string", "null" ]
            //    },
            //    "age": { "type": "integer" }
            //  },
            //  "required": [ "name", "age" ]
            //}
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual("Newtonsoft.Json.Schema.Tests.Documentation.GeneratingSchemas+Person", schema.Id.OriginalString);
        }

#if !NET40
        #region BuildingClass
        public class Building
        {
            [Required]
            [MaxLength(100)]
            public string Name { get; set; }
            [Required]
            [Phone]
            public string PhoneNumber { get; set; }
            [Required]
            [EnumDataType(typeof(Zone))]
            public string Zone { get; set; }
        }

        public enum Zone
        {
            Residential,
            Commercial,
            Industrial
        }
        #endregion

        [Test]
        public void DataAnnotations()
        {
            #region DataAnnotations
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(Building));
            //{
            //  "type": "object",
            //  "properties": {
            //    "Name": {
            //      "type": "string",
            //      "maxLength": 100
            //    },
            //    "PhoneNumber": {
            //      "type": "string",
            //      "format": "phone"
            //    },
            //    "Zone": {
            //      "type": "string",
            //      "enum": [
            //        "Residential",
            //        "Commercial",
            //        "Industrial"
            //      ]
            //    }
            //  },
            //  "required": [
            //    "Name",
            //    "PhoneNumber",
            //    "Zone"
            //  ]
            //}
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
        }

        public class BuildingReport
        {
            public DateTime Date { get; set; }
            public Zone Zone { get; set; }
        }

        [Test]
        public void JSchemaGenerationProvider()
        {
            #region JSchemaGenerationProvider
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(BuildingReport));
            //{
            //  "type": "object",
            //  "properties": {
            //    "Date": {
            //      "type": "string"
            //    },
            //    "Zone": {
            //      "type": "integer",
            //      "enum": [ 0, 1, 2 ]
            //    }
            //  },
            //  "required": [ "Date", "Zone" ]
            //}

            JSchemaGenerator stringEnumGenerator = new JSchemaGenerator();

            // change Zone enum to generate a string
            stringEnumGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema stringEnumSchema = stringEnumGenerator.Generate(typeof(BuildingReport));
            //{
            //  "type": "object",
            //  "properties": {
            //    "Date": {
            //      "type": "string"
            //    },
            //    "Zone": {
            //      "type": "string",
            //      "enum": [ "Residential", "Commercial", "Industrial" ]
            //    }
            //  },
            //  "required": [ "Date", "Zone" ]
            //}
            #endregion

            Console.WriteLine(stringEnumSchema.ToString());

            Assert.AreEqual(JSchemaType.Object, schema.Type);
        }
#endif
    }
}

#endif