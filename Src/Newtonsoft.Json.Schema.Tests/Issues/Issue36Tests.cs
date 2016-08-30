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
    public class Issue36Tests : TestFixtureBase
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedDate { get; set; }
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
            Assert.AreEqual("foo", schema.Properties["Id"].Title);
            Assert.AreEqual("foo", schema.Properties["Name"].Title);
            Assert.AreEqual("foo", schema.Properties["CreatedDate"].Title);
        }
    }
}