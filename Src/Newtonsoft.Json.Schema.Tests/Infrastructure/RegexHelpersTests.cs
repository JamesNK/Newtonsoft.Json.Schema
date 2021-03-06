#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
#if !(NET20 || NET35 || PORTABLE) || DNXCORE50
using System.Numerics;
#endif
using Newtonsoft.Json.Schema.Infrastructure;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using System.Text.RegularExpressions;

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

        [Test]
        public void TryGetPatternRegex_ECMARegexPattern_Success()
        {
            string pattern = @"^-?(?:[1-9]\d{3}(-?)(?:(?:0[1-9]|1[0-2])\1(?:0[1-9]|1\d|2[0-8])|(?:0[13-9]|1[0-2])\1(?:29|30)|(?:0[13578]|1[02])(?:\1)31|00[1-9]|0[1-9]\d|[12]\d{2}|3(?:[0-5]\d|6[0-5]))|(?:[1-9]\d(?:0[48]|[2468][048]|[13579][26])|(?:[2468][048]|[13579][26])00)(?:(-?)02(?:\2)29|-?366))(?:Z|[+-][01]\d(?:\3[0-5]\d)?)$";

            Regex regex1 = null;
            string errorMessage1 = null;
            bool result1 = RegexHelpers.TryGetPatternRegex(pattern, null, ref regex1, ref errorMessage1);

            Assert.AreEqual(true, result1);
            Assert.AreEqual(RegexOptions.ECMAScript, regex1.Options);
        }
#endif
    }
}