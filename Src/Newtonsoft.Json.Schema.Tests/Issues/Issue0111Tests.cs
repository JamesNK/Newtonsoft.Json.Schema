#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class Issue0111Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaGenerator stringEnumGenerator = new JSchemaGenerator();
            
            stringEnumGenerator.GenerationProviders.Add(new StringEnumGenerationProvider
            {
                CamelCaseText = true
            });
            
            JSchema stringEnumSchema = stringEnumGenerator.Generate(typeof(BuildingReport));

            string json = @"{
  ""type"": ""object"",
  ""properties"": {
    ""PropWithValidDefault"": {
      ""type"": ""string"",
      ""default"": ""residential"",
      ""enum"": [
        ""residential"",
        ""commercial"",
        ""industrial""
      ]
    },
    ""PropWithFlagsDefault"": {
      ""type"": ""string"",
      ""default"": ""commercial, industrial"",
      ""enum"": [
        ""residential"",
        ""commercial"",
        ""industrial""
      ]
    },
    ""PropWithUnspecifiedDefault"": {
      ""type"": ""string"",
      ""default"": ""99"",
      ""enum"": [
        ""residential"",
        ""commercial"",
        ""industrial""
      ]
    }
  },
  ""required"": [
    ""PropWithValidDefault"",
    ""PropWithFlagsDefault"",
    ""PropWithUnspecifiedDefault""
  ]
}";

            StringAssert.AreEqual(json, stringEnumSchema.ToString());
        }

        public class BuildingReport
        {
            [DefaultValue(BuildingZone.Residential)]
            public BuildingZone PropWithValidDefault { get; set; }
            [DefaultValue(BuildingZone.Commercial | BuildingZone.Industrial)]
            public BuildingZone PropWithFlagsDefault { get; set; }
            [DefaultValue(99)]
            public BuildingZone PropWithUnspecifiedDefault { get; set; }
        }

        [Flags]
        public enum BuildingZone
        {
            Residential = 0,
            Commercial = 1,
            Industrial = 2
        }
    }
}