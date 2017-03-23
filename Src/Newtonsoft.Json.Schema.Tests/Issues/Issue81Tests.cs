#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if !(NET20 || NET35 || PORTABLE)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
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
    public class Issue81Tests : TestFixtureBase
    {
        private string originalSchemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""arrayProp"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""referencedProp"": {
            ""$ref"": ""http://localhost/remoteReference.json#/definitions/aProperty""
          }
        }
      }
    },
    ""otherProp"": {
      ""type"": ""object"",
      ""properties"": {
        ""referencingProp"": {
          ""$ref"": ""http://localhost/remoteReference.json#/definitions/aProperty""
        }
      }
    }
  }
}";

        private string remoteReferenceJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""definitions"": {
    ""aProperty"": {
      ""type"": ""string""
    }
  }
}";

        private string dereferencedSchemaJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""arrayProp"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""properties"": {
          ""referencedProp"": {
            ""type"": ""string""
          }
        }
      }
    },
    ""otherProp"": {
      ""type"": ""object"",
      ""properties"": {
        ""referencingProp"": {
          ""$ref"": ""#/properties/arrayProp/items/properties/referencedProp""
        }
      }
    }
  }
}";

        [Test]
        public void Test()
        {
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            Stream remoteReferenceStream = new MemoryStream(Encoding.UTF8.GetBytes(remoteReferenceJson));
            resolver.Add(new Uri("http://localhost/remoteReference.json"), remoteReferenceStream);

            JSchema schema = JSchema.Parse(originalSchemaJson, resolver);
            string json = schema.ToString();

            StringAssert.AreEqual(dereferencedSchemaJson, json);

            JSchema schema2 = JSchema.Parse(json);

            Assert.AreEqual(schema.ToString(), schema2.ToString());
        }
    }
}
#endif