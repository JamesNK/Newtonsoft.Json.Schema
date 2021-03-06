#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections;
using System.Collections.Generic;
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
    public class Issue0097Tests : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            JSchema s = new JSchema();
            s.PatternProperties["test"] = new JSchema();

            IDictionary dictionary = (IDictionary)s.PatternProperties;
            Assert.AreEqual(s.PatternProperties["test"], dictionary["test"]);
            Assert.AreEqual(null, dictionary["BAD"]);
            Assert.AreEqual(true, dictionary.Contains("test"));

            IList<DictionaryEntry> entires = new List<DictionaryEntry>();

            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                entires.Add(enumerator.Entry);
            }

            Assert.AreEqual(1, entires.Count);
            Assert.AreEqual("test", entires[0].Key);
        }
    }
}