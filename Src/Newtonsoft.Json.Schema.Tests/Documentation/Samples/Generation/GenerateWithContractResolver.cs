#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Generation
{
    [TestFixture]
    public class GenerateWithContractResolver : TestFixtureBase
    {
        #region Types
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();

            // change contract resolver so property names are camel case
            generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

            JSchema schema = generator.Generate(typeof(Person));
            // {
            //   "type": "object",
            //   "properties": {
            //     "name": {
            //       "type": [ "string", "null" ]
            //     },
            //     "age": { "type": "integer" }
            //   },
            //   "required": [ "name", "age" ]
            // }
            #endregion

            Assert.AreEqual(JSchemaType.Object, schema.Type);
            Assert.AreEqual(true, schema.Properties.ContainsKey("name"));
        }
    }
}