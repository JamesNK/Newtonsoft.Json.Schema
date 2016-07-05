#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
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
    public class Issue40Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            string json = @"{
  ""id"": ""TestComplexClass1"",
  ""definitions"": {
    ""TestClass1"": {
      ""id"": ""TestClass1""
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""TestProperty"": {
      ""$ref"": ""TestClass1""
    }
  },
  ""required"": [
    ""TestProperty""
  ]
}";

            JSchemaReader schemaReader = new JSchemaReader(new JSchemaReaderSettings());
            JSchema schema = schemaReader.ReadRoot(new JsonTextReader(new StringReader(json)));

            Assert.AreEqual("TestClass1", schema.Properties["TestProperty"].Id.OriginalString);
        }
    }
}