#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue37Tests : TestFixtureBase
    {
        public class User
        {
            public int Id01 { get; set; }
            public int Id02 { get; set; }
            public int Id03 { get; set; }
            public int Id04 { get; set; }
            public int Id05 { get; set; }
            public int Id06 { get; set; }
            public int Id07 { get; set; }
            public int Id08 { get; set; }
            public int Id09 { get; set; }
            public int Id10 { get; set; }
            public int Id11 { get; set; }
            public int Id12 { get; set; }
            public int Id13 { get; set; }
            public int Id14 { get; set; }
            public int Id15 { get; set; }
            public int Id16 { get; set; }
            public int Id17 { get; set; }
            public int Id18 { get; set; }
            public int Id19 { get; set; }
            public int Id20 { get; set; }
            public int Id21 { get; set; }
            public int Id22 { get; set; }
            public int Id23 { get; set; }
            public int Id24 { get; set; }
            public int Id25 { get; set; }
            public int Id26 { get; set; }
        }

        public class TitleProvider : JSchemaGenerationProvider
        {
            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                JSchema schema = context.Generator.Generate(context.ObjectType);
                schema.Title = "foo";
                return schema;
            }
        }

        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new TitleProvider());
            JSchema schema = generator.Generate(typeof(User));

            Assert.AreEqual("foo", schema.Title);
            foreach (KeyValuePair<string, JSchema> property in schema.Properties)
            {
                Assert.AreEqual("foo", property.Value.Title);
            }
        }
    }
}