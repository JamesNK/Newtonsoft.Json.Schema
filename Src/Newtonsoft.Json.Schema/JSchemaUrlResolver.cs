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

#if !PORTABLE
        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the download will attempt to download before timing out.
        /// </summary>
        public int? Timeout { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value, in bytes, that determines the maximum download size allowed before failing.
        /// </summary>
        public int? ByteLimit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaResolver"/> class.
        /// </summary>
        public JSchemaUrlResolver()
        {
            _downloader = new WebRequestDownloader();
        }

        /// <summary>
        /// Sets the credentials used to authenticate web requests.
        /// </summary>
        public override ICredentials Credentials
        {
            set { _credentials = value; }
        }

        /// <summary>
        /// Gets the schema resource for a given schema reference.
        /// </summary>
        /// <param name="context">The schema ID context.</param>
        /// <param name="reference">The schema reference.</param>
        /// <returns>The schema resource for a given schema reference.</returns>
        public override Stream GetSchemaResource(ResolveSchemaContext context, SchemaReference reference)
        {
            if (!reference.BaseUri.IsAbsoluteUri)
            {
                return null;
            }

#if !PORTABLE
            int? timeout = Timeout;
#else
            int? timeout = null;
#endif

            return _downloader.GetStream(reference.BaseUri, _credentials, timeout, ByteLimit);
        }
    }
}