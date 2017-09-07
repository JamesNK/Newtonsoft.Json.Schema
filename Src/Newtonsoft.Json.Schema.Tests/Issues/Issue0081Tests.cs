#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

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
    public class Issue0081Tests : TestFixtureBase
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

        private string originalSchemaWithArrayJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""arrayProp"": {
      ""type"": ""array"",
      ""items"": [
        {
          ""type"": ""object"",
          ""properties"": {
            ""referencedProp"": {
              ""$ref"": ""http://localhost/remoteReference.json#/definitions/aProperty""
            }
          }
        }
      ]
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

        private string dereferencedSchemaWithArrayJson = @"{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""type"": ""object"",
  ""properties"": {
    ""arrayProp"": {
      ""type"": ""array"",
      ""items"": [
        {
          ""type"": ""object"",
          ""properties"": {
            ""referencedProp"": {
              ""type"": ""string""
            }
          }
        }
      ]
    },
    ""otherProp"": {
      ""type"": ""object"",
      ""properties"": {
        ""referencingProp"": {
          ""$ref"": ""#/properties/arrayProp/items/0/properties/referencedProp""
        }
      }
    }
  }
}";

        [Test]
        public void TestWithSingleItem()
        {
            JSchema schema = JSchema.Parse(originalSchemaJson, TestResolver);
            string json = schema.ToString();

            StringAssert.AreEqual(dereferencedSchemaJson, json);

            JSchema schema2 = JSchema.Parse(json);

            Assert.AreEqual(schema.ToString(), schema2.ToString());
        }

        [Test]
        public void TestWithArrayItems()
        {
            JSchema schema = JSchema.Parse(originalSchemaWithArrayJson, TestResolver);
            string json = schema.ToString();

            StringAssert.AreEqual(dereferencedSchemaWithArrayJson, json);

            JSchema schema2 = JSchema.Parse(json);

            Assert.AreEqual(schema.ToString(), schema2.ToString());
        }

        private JSchemaPreloadedResolver TestResolver
        {
            get
            {
                JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
                Stream remoteReferenceStream = new MemoryStream(Encoding.UTF8.GetBytes(remoteReferenceJson));
                resolver.Add(new Uri("http://localhost/remoteReference.json"), remoteReferenceStream);

                return resolver;
            }
        }
    }
}