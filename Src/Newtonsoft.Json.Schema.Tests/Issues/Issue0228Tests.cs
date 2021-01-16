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
    public class Issue0228Tests : TestFixtureBase
    {
		[Test]
		public void Test()
		{
			var template = new Template();

			var o = JObject.FromObject(template);

			StringAssert.AreEqual(@"{
  ""Schema"": null
}", o.ToString());


			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				ContractResolver = new DefaultContractResolver
				{
					IgnoreSerializableInterface = true,
				},
			};
			var parsed = JsonConvert.DeserializeObject<Template>(o.ToString(), settings);

			Assert.AreEqual(null, parsed.Schema);
		}

		[Test]
		public void Test_Root()
		{
			var settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				ContractResolver = new DefaultContractResolver
				{
					IgnoreSerializableInterface = true,
				},
			};
			var parsed = JsonConvert.DeserializeObject<JSchema>("null", settings);

			Assert.AreEqual(null, parsed);
		}

		public class Template
        {
            public JSchema Schema { get; set; }
        }
    }
}
