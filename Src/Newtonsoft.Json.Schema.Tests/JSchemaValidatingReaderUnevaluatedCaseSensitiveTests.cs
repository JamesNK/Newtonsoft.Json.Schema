#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Schema.Infrastructure;
#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
using System.Text;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Theory = Xunit.TheoryAttribute;
using TestCase = Xunit.InlineDataAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
using Theory = NUnit.Framework.TestAttribute;
#endif

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaValidatingReaderUnevaluatedCaseSensitiveTests : JSchemaValidatingReaderUnevaluatedTestsBase
    {
        protected override JSchemaValidatingReader CreateValidatingReader(JsonReader reader)
        {
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            return validatingReader;
        }
    }
}