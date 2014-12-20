using System;
using System.IO;
using System.Net;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal interface IDownloader
    {
        Stream GetStream(Uri uri, ICredentials credentials);
    }
}