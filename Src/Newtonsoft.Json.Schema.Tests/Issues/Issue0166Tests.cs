#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue0166Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema schema = generator.Generate(typeof(SwitchPosition));

            StringAssert.AreEqual(@"{
  ""type"": ""string"",
  ""enum"": [
    ""UNKNOWN"",
    ""ON"",
    ""A"",
    ""OFF"",
    ""B""
  ]
}", schema.ToString());
        }

        [Flags]
        public enum SwitchPosition
        {
            UNKNOWN = 0,
            ON = 1,
            OFF = 2,
            A = 1,
            B = 2
        }
    }
}