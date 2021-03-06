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

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Create
{
    [TestFixture]
    public class SaveJSchemaToFile : TestFixtureBase
    {
        [Test]
        public void Example()
        {
            #region Usage
            JSchema schema = new JSchema
            {
                Type = JSchemaType.Object
            };

            // serialize JSchema to a string and then write string to a file
            File.WriteAllText(@"c:\schema.json", schema.ToString());

            // serialize JSchema directly to a file
            using (StreamWriter file = File.CreateText(@"c:\schema.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                schema.WriteTo(writer);
            }
            #endregion
        }
    }
}