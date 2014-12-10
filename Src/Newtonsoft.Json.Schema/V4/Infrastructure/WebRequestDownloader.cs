using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal class WebRequestDownloader : IDownloader
    {
        public Stream GetStream(Uri uri, ICredentials credentials)
        {
            WebRequest request = WebRequest.Create(uri);

            if (credentials != null)
                request.Credentials = credentials;

            Task<WebResponse> result = Task.Factory.FromAsync(
                request.BeginGetResponse,
                new Func<IAsyncResult, WebResponse>(request.EndGetResponse), null);
            result.ConfigureAwait(false);

            WebResponse response = result.Result;

            return response.GetResponseStream();
        }
    }
}