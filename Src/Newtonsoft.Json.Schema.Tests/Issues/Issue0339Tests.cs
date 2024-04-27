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
    public class Issue0339Tests : TestFixtureBase
    {
        [Test]
        public void RecursiveRef_DynamicSelection_NoLoadErrors()
        {
            string schemaJson = @"{
    ""$id"": ""recursiveRef8_main.json"",
    ""$defs"": {
        ""inner"": {
            ""$id"": ""recursiveRef8_inner.json"",
            ""$recursiveAnchor"": true,
            ""title"": ""inner"",
            ""additionalProperties"": {
                ""$recursiveRef"": ""#""
            }
        }
    },
    ""if"": {
        ""propertyNames"": {
            ""pattern"": ""^[a-m]""
        }
    },
    ""then"": {
        ""title"": ""any type of node"",
        ""$id"": ""recursiveRef8_anyLeafNode.json"",
        ""$recursiveAnchor"": true,
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner""
    },
    ""else"": {
        ""title"": ""integer node"",
        ""$id"": ""recursiveRef8_integerNode.json"",
        ""$recursiveAnchor"": true,
        ""type"": [ ""object"", ""integer"" ],
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner""
    }
}";

            List<ValidationError> schemaErrors = new List<ValidationError>();
            JSchemaReaderSettings settings = new JSchemaReaderSettings();
            settings.ValidationEventHandler += (sender, args) => schemaErrors.Add(args.ValidationError);

            JSchema schema = JSchema.Parse(schemaJson, settings);

            Assert.AreEqual(0, schemaErrors.Count);
        }

        [Test]
        public void RecursiveRef_DynamicSelection_Duplicates_HasLoadErrors()
        {
            string schemaJson = @"{
    ""$id"": ""recursiveRef8_main.json"",
    ""$defs"": {
        ""inner"": {
            ""$id"": ""recursiveRef8_inner.json"",
            ""$recursiveAnchor"": true,
            ""title"": ""inner"",
            ""additionalProperties"": {
                ""$recursiveRef"": ""#""
            }
        },
        ""inner2"": {
            ""$id"": ""recursiveRef8_inner.json"",
            ""$recursiveAnchor"": true,
            ""title"": ""inner"",
            ""additionalProperties"": {
                ""$recursiveRef"": ""#""
            }
        }
    },
    ""if"": {
        ""propertyNames"": {
            ""pattern"": ""^[a-m]""
        }
    },
    ""then"": {
        ""title"": ""any type of node"",
        ""$id"": ""recursiveRef8_anyLeafNode.json"",
        ""$recursiveAnchor"": true,
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner""
    },
    ""else"": {
        ""title"": ""integer node"",
        ""$id"": ""recursiveRef8_anyLeafNode.json"",
        ""$recursiveAnchor"": true,
        ""type"": [ ""object"", ""integer"" ],
        ""$ref"": ""recursiveRef8_main.json#/$defs/inner2""
    }
}";

            List<ValidationError> schemaErrors = new List<ValidationError>();
            JSchemaReaderSettings settings = new JSchemaReaderSettings();
            settings.ValidationEventHandler += (sender, args) => schemaErrors.Add(args.ValidationError);

            JSchema schema = JSchema.Parse(schemaJson, settings);

            Assert.AreEqual(4, schemaErrors.Count);
        }
    }
}
