using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal interface IDownloader
    {
        Stream GetStream(Uri uri, ICredentials credentials);
    }
}