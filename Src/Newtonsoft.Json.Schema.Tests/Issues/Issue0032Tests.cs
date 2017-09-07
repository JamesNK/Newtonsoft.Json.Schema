#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Issue0032Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string swaggerJson = TestHelpers.OpenFileText("Resources/Schemas/swagger-2.0.json");

            JSchemaUrlResolver resolver = new JSchemaUrlResolver();

            JSchema swagger = JSchema.Parse(swaggerJson, resolver);

            // resolve the nested schema
            JSchema infoSchema = resolver.GetSubschema(new SchemaReference
            {
                BaseUri = new Uri("#", UriKind.RelativeOrAbsolute),
                SubschemaId = new Uri("#/definitions/info", UriKind.RelativeOrAbsolute)
            }, swagger);

            Assert.AreEqual("General information about the API.", infoSchema.Description);

            Console.WriteLine(infoSchema.ToString());
        }
    }
}