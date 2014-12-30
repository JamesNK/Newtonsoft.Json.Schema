#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.IO;
using NUnit.Framework;

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

            // read JSchema directly from a file
            using (StreamReader file = File.OpenText(@"c:\schema.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JSchema schema2 = JSchema.Read(reader);
            }
            #endregion
        }
    }
}