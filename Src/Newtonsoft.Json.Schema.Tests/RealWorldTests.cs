using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class RealWorldTests : TestFixtureBase
    {
        [Test]
        public void CoffeeLint()
        {
            string coffeeLintJson = @"{
    ""coffeescript_error"": {
        ""level"": ""error""
    },
    ""arrow_spacing"": {
        ""name"": ""arrow_spacing"",
        ""level"": ""warn""
    },
    ""no_tabs"": {
        ""name"": ""no_tabs"",
        ""level"": ""error""
    },
    ""no_trailing_whitespace"": {
        ""name"": ""no_trailing_whitespace"",
        ""level"": ""warn"",
        ""allowed_in_comments"": false,
        ""allowed_in_empty_lines"": true
    },
    ""max_line_length"": {
        ""name"": ""max_line_length"",
        ""value"": 80,
        ""level"": ""warn"",
        ""limitComments"": true
    },
    ""line_endings"": {
        ""name"": ""line_endings"",
        ""level"": ""ignore"",
        ""value"": ""unix""
    },
    ""no_trailing_semicolons"": {
        ""name"": ""no_trailing_semicolons"",
        ""level"": ""error""
    },
    ""indentation"": {
        ""name"": ""indentation"",
        ""value"": 2,
        ""level"": ""error""
    },
    ""camel_case_classes"": {
        ""name"": ""camel_case_classes"",
        ""level"": ""error""
    },
    ""colon_assignment_spacing"": {
        ""name"": ""colon_assignment_spacing"",
        ""level"": ""warn"",
        ""spacing"": {
            ""left"": 0,
            ""right"": 1
        }
    },
    ""no_implicit_braces"": {
        ""name"": ""no_implicit_braces"",
        ""level"": ""ignore"",
        ""strict"": true
    },
    ""no_plusplus"": {
        ""name"": ""no_plusplus"",
        ""level"": ""ignore""
    },
    ""no_throwing_strings"": {
        ""name"": ""no_throwing_strings"",
        ""level"": ""error""
    },
    ""no_backticks"": {
        ""name"": ""no_backticks"",
        ""level"": ""error""
    },
    ""no_implicit_parens"": {
        ""name"": ""no_implicit_parens"",
        ""level"": ""ignore""
    },
    ""no_empty_param_list"": {
        ""name"": ""no_empty_param_list"",
        ""level"": ""warn""
    },
    ""no_stand_alone_at"": {
        ""name"": ""no_stand_alone_at"",
        ""level"": ""ignore""
    },
    ""space_operators"": {
        ""name"": ""space_operators"",
        ""level"": ""warn""
    },
    ""duplicate_key"": {
        ""name"": ""duplicate_key"",
        ""level"": ""error""
    },
    ""empty_constructor_needs_parens"": {
        ""name"": ""empty_constructor_needs_parens"",
        ""level"": ""ignore""
    },
    ""cyclomatic_complexity"": {
        ""name"": ""cyclomatic_complexity"",
        ""value"": 10,
        ""level"": ""ignore""
    },
    ""newlines_after_classes"": {
        ""name"": ""newlines_after_classes"",
        ""value"": 3,
        ""level"": ""ignore""
    },
    ""no_unnecessary_fat_arrows"": {
        ""name"": ""no_unnecessary_fat_arrows"",
        ""level"": ""warn""
    },
    ""missing_fat_arrows"": {
        ""name"": ""missing_fat_arrows"",
        ""level"": ""ignore""
    },
    ""non_empty_constructor_needs_parens"": {
        ""name"": ""non_empty_constructor_needs_parens"",
        ""level"": ""ignore""
    }
}";

            JSchema schema = TestHelpers.OpenSchemaFile("Resources/Schemas/coffeelint.json");

            JObject o = JObject.Parse(coffeeLintJson);

            Assert.IsTrue(o.IsValid(schema));

        }

        [Test]
        public void JsonLD()
        {
            string jsonLd = @"{
    ""@id"": ""http://store.example.com/"",
    ""@type"": ""Store"",
    ""name"": ""Links Bike Shop"",
    ""description"": ""The most \""linked\"" bike store on earth!"",
    ""product"": [
        {
            ""@id"": ""p:links-swift-chain"",
            ""@type"": ""Product"",
            ""name"": ""Links Swift Chain"",
            ""description"": ""A fine chain with many links."",
            ""category"": [""cat:parts"", ""cat:chains""],
            ""price"": ""10.00"",
            ""stock"": 10
        },
        {
            ""@id"": ""p:links-speedy-lube"",
            ""@type"": ""Product"",
            ""name"": ""Links Speedy Lube"",
            ""description"": ""Lubricant for your chain links."",
            ""category"": [""cat:lube"", ""cat:chains""],
            ""price"": ""5.00"",
            ""stock"": 20
        }
    ],
    ""@context"": {
        ""Store"": ""http://ns.example.com/store#Store"",
        ""Product"": ""http://ns.example.com/store#Product"",
        ""product"": ""http://ns.example.com/store#product"",
        ""category"":
        {
          ""@id"": ""http://ns.example.com/store#category"",
          ""@type"": ""@id""
        },
        ""price"": ""http://ns.example.com/store#price"",
        ""stock"": ""http://ns.example.com/store#stock"",
        ""name"": ""http://purl.org/dc/terms/title"",
        ""description"": ""http://purl.org/dc/terms/description"",
        ""p"": ""http://store.example.com/products/"",
        ""cat"": ""http://store.example.com/category/""
    }
}";

            JSchema schema = TestHelpers.OpenSchemaFile("Resources/Schemas/jsonld.json");

            JObject o = JObject.Parse(jsonLd);

            Assert.IsTrue(o.IsValid(schema));
        }

        [Test]
        public void SchemaDraft4()
        {
            string schemaJson = TestHelpers.OpenFileText("Resources/Schemas/schema-draft-v4.json");
            JSchema schema = JSchema.Parse(schemaJson);

            JObject o = JObject.Parse(schemaJson);

            Assert.IsTrue(o.IsValid(schema));
        }
    }
}
