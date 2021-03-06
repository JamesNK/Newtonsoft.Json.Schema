#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
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
            JSchemaReaderSettings settings = new JSchemaReaderSettings();
            settings.ValidationEventHandler += OnValidate;

            ExceptionAssert.Throws<JSchemaException>(
                () => JSchema.Parse(Json, settings),
                "Error resolving schema ID 'file:/action_scenario.json#' in the current scope. The resolved ID must be a valid URI. Path 'properties.prop1', line 3, position 14.");
        }

        private const string Json = @"{
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