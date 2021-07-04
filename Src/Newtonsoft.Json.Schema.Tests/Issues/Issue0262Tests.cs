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
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
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
    public class Issue0262Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            ExceptionAssert.Throws<JSchemaReaderException>(
                () => JSchema.Parse(Schema, new JSchemaReaderSettings() { ValidateVersion = true }),
                "Validation error raised by version schema 'https://json-schema.org/draft/2019-09/schema': JSON does not match all schemas from 'allOf'. Invalid schema indexes: 2. Path '', line 4, position 1.");
        }

        private const string Schema = @"{
  ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
  ""maxLength"": -1
}";
    }
}
