#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves external JSON Schemas named by a Uniform Resource Identifier (URI).
    /// </summary>
    public class JSchemaUrlResolver : JSchemaResolver
    {
        private ICredentials _credentials;

#if DEBUG
        private IDownloader _downloader;

        internal void SetDownloader(IDownloader downloader)
        {
            _downloader = downloader;
        }
#else
        private readonly IDownloader _downloader;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaResolver"/> class.
        /// </summary>
        public JSchemaUrlResolver()
        {
            _downloader = new WebRequestDownloader();
        }

        /// <summary>
        /// Gets the schema for a given URI.
        /// </summary>
        /// <param name="uri">The schema URI to resolve.</param>
        /// <returns>The resolved schema.</returns>
        public override JSchema GetSchema(Uri uri)
        {
            using (Stream s = _downloader.GetStream(uri, _credentials))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(sr))
            {
                return JSchema.Load(jsonReader);
            }
        }

        /// <summary>
        /// Sets the credentials used to authenticate web requests.
        /// </summary>
        public override ICredentials Credentials
        {
            set { _credentials = value; }
        }
    }
}