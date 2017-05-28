#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
using System.Reflection;
using Newtonsoft.Json.Schema.Infrastructure;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
using System.Text.RegularExpressions;
#endif

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class RegexHelpersTests : TestFixtureBase
    {
#if !(NET35 || NET40)
        [Test]
        public void TryGetPatternRegex_MatchTimeout_Null()
        {
            Regex regex1 = null;
            string errorMessage1 = null;
            RegexHelpers.TryGetPatternRegex("[abc]", null, ref regex1, ref errorMessage1);

            Assert.IsNotNull(regex1);

            Regex regex2 = regex1;
            string errorMessage2 = errorMessage1;
            RegexHelpers.TryGetPatternRegex("[abc]", null, ref regex2, ref errorMessage2);

            Assert.AreEqual(regex1, regex2);
        }

        [Test]
        public void TryGetPatternRegex_MatchTimeout_Defined()
        {
            TimeSpan matchTimeout = TimeSpan.FromSeconds(1);

            Regex regex1 = null;
            string errorMessage1 = null;
            RegexHelpers.TryGetPatternRegex("[abc]", matchTimeout, ref regex1, ref errorMessage1);

            Assert.IsNotNull(regex1);

            Regex regex2 = regex1;
            string errorMessage2 = errorMessage1;
            RegexHelpers.TryGetPatternRegex("[abc]", matchTimeout, ref regex2, ref errorMessage2);

            Assert.AreEqual(regex1, regex2);
        }
#endif
    }
}