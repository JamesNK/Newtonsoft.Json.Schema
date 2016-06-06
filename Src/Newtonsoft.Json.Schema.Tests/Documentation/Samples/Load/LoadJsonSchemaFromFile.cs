#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Load
{
    [TestFixture]
    public class LoadJSchemaFromFile : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            // read file into a string and parse JSchema from the string
            JSchema schema1 = JSchema.Parse(File.ReadAllText(@"c:\schema.json"));

            // load JSchema directly from a file
            using (StreamReader file = File.OpenText(@"c:\schema.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JSchema schema2 = JSchema.Load(reader);
            }
            #endregion
        }
    }
}