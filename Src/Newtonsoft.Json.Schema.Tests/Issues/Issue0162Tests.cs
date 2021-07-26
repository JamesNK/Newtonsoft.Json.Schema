#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
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
    public class Issue0162Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            var settings = new JSchemaReaderSettings();
            settings.ValidationEventHandler += OnValidate;

            ExceptionAssert.Throws<JSchemaException>(
                () => JSchema.Parse(Json, settings),
                "Error resolving schema ID 'file:/action_scenario.json#' in the current scope. The resolved ID must be a valid URI. Path 'properties.prop1', line 4, position 14.");
        }

        private const string Json = @"{
  ""$id"": ""test"",
  ""properties"": {
    ""prop1"": {
      ""$id"": ""file:/action_scenario.json#""
    }
  }
}";

        private void OnValidate(object sender, SchemaValidationEventArgs e)
        {
        }
    }
}