using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Generation
{
    [TestFixture]
    public class GenerateWithProvider : TestFixtureBase
    {
        #region Types
        public class BuildingReport
        {
            public DateTime Date { get; set; }
            public BuildingZone Zone { get; set; }
        }

        public enum BuildingZone
        {
            Residential,
            Commercial,
            Industrial
        }
        #endregion

        [Test]
        public void Example()
        { 
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(BuildingReport));
            // {
            //   "type": "object",
            //   "properties": {
            //     "Date": {
            //       "type": "string"
            //     },
            //     "Zone": {
            //       "type": "integer",
            //       "enum": [ 0, 1, 2 ]
            //     }
            //   },
            //   "required": [ "Date", "Zone" ]
            // }

            JSchemaGenerator stringEnumGenerator = new JSchemaGenerator();

            // change Zone enum to generate a string
            stringEnumGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema stringEnumSchema = stringEnumGenerator.Generate(typeof(BuildingReport));
            // {
            //   "type": "object",
            //   "properties": {
            //     "Date": {
            //       "type": "string"
            //     },
            //     "Zone": {
            //       "type": "string",
            //       "enum": [ "Residential", "Commercial", "Industrial" ]
            //     }
            //   },
            //   "required": [ "Date", "Zone" ]
            // }
            #endregion

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["Zone"].Type);
            Assert.AreEqual(JSchemaType.String, stringEnumSchema.Properties["Zone"].Type);
        }
    }
}