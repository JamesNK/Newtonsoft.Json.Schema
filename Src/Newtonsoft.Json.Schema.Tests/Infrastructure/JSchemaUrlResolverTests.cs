#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

#if DNXCORE50
using Xunit;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
using Test = Xunit.FactAttribute;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Infrastructure
{
    [TestFixture]
    public class JSchemaUrlResolverTests : TestFixtureBase
    {
#if DEBUG
        public class MockDownloader : IDownloader
        {
            public Stream GetStream(Uri uri, ICredentials credentials, int? timeout, int? byteLimit)
            {
                throw new Exception("Could not find");
            }
        }

        [Test]
        public void HandleError()
        {
            JSchemaUrlResolver resolver = new JSchemaUrlResolver();
            resolver.SetDownloader(new MockDownloader());

            ExceptionAssert.Throws<JSchemaReaderException>(() => { JSchema.Parse(@"{
    'allOf': [
        {'$ref':'http://google.com#'}
    ]
}", resolver); },
                "Error when resolving schema reference 'http://google.com/#'. Path 'allOf[0]', line 3, position 9.");
        }
#endif
    }
}