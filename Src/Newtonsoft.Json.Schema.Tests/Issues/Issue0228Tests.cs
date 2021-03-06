#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Linq;
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
    public class Issue0228Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            Template template = new Template();

            JObject o = JObject.FromObject(template);

            StringAssert.AreEqual(@"{
  ""Schema"": null
}", o.ToString());


            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableInterface = true,
                },
            };
            Template parsed = JsonConvert.DeserializeObject<Template>(o.ToString(), settings);

            Assert.AreEqual(null, parsed.Schema);
        }

        [Test]
        public void Test_Root()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableInterface = true,
                },
            };
            JSchema parsed = JsonConvert.DeserializeObject<JSchema>("null", settings);

            Assert.AreEqual(null, parsed);
        }

        public class Template
        {
            public JSchema Schema { get; set; }
        }
    }
}
