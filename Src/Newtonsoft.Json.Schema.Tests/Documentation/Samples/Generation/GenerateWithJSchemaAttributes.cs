#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class GenerateWithJSchemaAttributes : TestFixtureBase
    {
        #region Types
        public class Computer
        {
            // always require a string value
            [UniqueItems]
            public IEnumerable<string> DiskIds { get; set; }

            public HashSet<string> ScreenIds { get; set; }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(Computer));
            //{
            //    "type": "object",
            //    "properties": {
            //        "DiskIds": {
            //            "type": "array",
            //            "items": {
            //                "type": "string",
            //            },
            //            "uniqueItems": true
            //        },
            //        "ScreenIds": {
            //            "type": "array",
            //            "items": {
            //                "type": "string",
            //            },
            //            "uniqueItems": true
            //        }
            //    }
            //}
            #endregion

            Assert.IsTrue(schema.Properties["DiskIds"].UniqueItems);
            Assert.IsTrue(schema.Properties["ScreenIds"].UniqueItems);
        }
    }
}