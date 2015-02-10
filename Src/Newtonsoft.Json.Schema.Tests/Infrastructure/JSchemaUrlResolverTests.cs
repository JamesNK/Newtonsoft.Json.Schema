using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Infrastructure;
using NUnit.Framework;

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

            ExceptionAssert.Throws<JSchemaReaderException>(() =>
            {
                JSchema.Parse(@"{
    'allOf': [
        {'$ref':'http://google.com#'}
    ]
}", resolver);
            },
                "Error when resolving schema reference 'http://google.com/#'. Path 'allOf[0]', line 3, position 38.");
        }
#endif
    }
}