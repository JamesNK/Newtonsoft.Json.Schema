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
    public class Issue0155Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            var schema = JSchema.Parse("{type:'object', properties: { a: {type: 'integer'}, b: {type:'integer'}}}");
            var content = @"{a: 1, b: 1}  {'a': true, b: 1}";

            using (var reader = new JsonTextReader(new StringReader(content)) { SupportMultipleContent = true })
            using (var vreader = new JSchemaValidatingReader(reader) { Schema = schema, SupportMultipleContent = true })
            {
                var count = 0;
                vreader.ValidationEventHandler += (o, a) => { count++; System.Console.WriteLine(a.Message); };

                while (vreader.Read())
                {
                    System.Console.WriteLine(vreader.TokenType);
                }
                System.Console.WriteLine("Done, {0} error(s)", count);
            }
        }

        [Test]
        public void Test_Complex()
        {
            var schema = JSchema.Parse(@"{
    allOf: [
        {type:'object',properties: { a: {type: 'integer'}, b: {type:'integer'}}},
        {type:'object',properties: { a: {type: 'integer'}, b: {type:'integer'}}}
    ]
}");
            var content = @"{a: 1, b: 1}  {'a': true, b: 1}";

            using (var reader = new JsonTextReader(new StringReader(content)) { SupportMultipleContent = true })
            using (var vreader = new JSchemaValidatingReader(reader) { Schema = schema, SupportMultipleContent = true })
            {
                var count = 0;
                vreader.ValidationEventHandler += (o, a) => { count++; System.Console.WriteLine(a.Message); };

                while (vreader.Read())
                {
                    System.Console.WriteLine(vreader.TokenType);
                }
                System.Console.WriteLine("Done, {0} error(s)", count);
            }
        }
    }
}