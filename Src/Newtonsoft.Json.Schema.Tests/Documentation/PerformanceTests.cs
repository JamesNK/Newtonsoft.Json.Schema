#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
using Newtonsoft.Json.Schema.Tests;
#if !(NET35 || NET20 || PORTABLE || DNXCORE50)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema.Tests.TestObjects;
using Newtonsoft.Json.Utilities;
using System.Globalization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using File = Newtonsoft.Json.Schema.Tests.Documentation.Samples.File;

namespace Newtonsoft.Json.Schema.Tests.Documentation
{
    [TestFixture]
    public class PerformanceTests : TestFixtureBase
    {
        [Test]
        public void ValidateStream()
        {
#region ValidateStream
            string schemaJson = @"{
              'description': 'A person',
              'type': 'object',
              'properties': {
                'name': {'type': 'string'},
                'hobbies': {
                  'type': 'array',
                  'items': {'type': 'string'}
                }
              }
            }";

            JSchema schema = JSchema.Parse(schemaJson);

            using (StreamReader s = File.OpenText(@"c:\bigdata.json"))
            using (JSchemaValidatingReader reader = new JSchemaValidatingReader(new JsonTextReader(s)))
            {
                // assign schema and setup event handler
                reader.Schema = schema;
                reader.ValidationEventHandler += (sender, args) => { Console.WriteLine(args.Message); };

                // bigdata.json will be validated without loading the entire document into memory
                while (reader.Read())
                {
                }
            }
#endregion
        }
    }
}

#endif