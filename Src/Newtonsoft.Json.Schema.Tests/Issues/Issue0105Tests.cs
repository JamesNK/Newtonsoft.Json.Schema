#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Linq;
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
    public class Issue0105Test : TestFixtureBase
    {
        [TestCase("1977-02-01T00:00:00Z")]
        [TestCase("1977-02-01T00:00:00.0Z")]
        [TestCase("1977-02-01T00:00:00.00Z")]
        [TestCase("1977-02-01T00:00:00.000Z")]
        [TestCase("1977-02-01T00:00:00.0000Z")]
        [TestCase("1977-02-01T00:00:00.00000Z")]
        [TestCase("1977-02-01T00:00:00+01:00")]
        [TestCase("1977-02-01T00:00:00.0+01:00")]
        [TestCase("1977-02-01T00:00:00.00+01:00")]
        [TestCase("1977-02-01T00:00:00.000+01:00")]
        [TestCase("1977-02-01T00:00:00.0000+01:00")]
        [TestCase("1977-02-01T00:00:00.00000+01:00")]
        public void DateTimeValidPatterns(string validDateTimePattern)
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            var o = new JObject
            {
                ["someDate"] = validDateTimePattern
            };

            Assert.That(o.IsValid(schema), Is.True, "Should allow valid date-time pattern");
        }

        [TestCase("1977-02-01T00:00:00")]
        [TestCase("1977-02-01T00:00:00.000")]
        [TestCase("1977-02-01T00:00:00+01")]
        [TestCase("1977-02-01T00:00:00.000+01")]
        [TestCase("1977-02-01T00:00:00A")]
        public void DateTimeInvalidPatterns(string invalidDateTimePattern)
        {
            JSchema schema = JSchema.Parse(SchemaJson);

            var o = new JObject
            {
                ["someDate"] = invalidDateTimePattern
            };
            
            Assert.That(o.IsValid(schema), Is.False, "Should not allow invalid date-time patterns");
        }
        
        private const string SchemaJson = @"{
    ""$schema"": ""http://json-schema.org/draft-04/schema#"",
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
        ""someDate"": {
			""type"": ""string"",
			""description"": ""Some date"",
			""format"": ""date-time""
		}
    }
}";
    }
}
