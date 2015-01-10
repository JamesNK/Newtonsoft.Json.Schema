#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Schema.Infrastructure
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
#if !NET40
            result.ConfigureAwait(false);
#endif

            WebResponse response = result.Result;

            return response.GetResponseStream();
        }
    }
}