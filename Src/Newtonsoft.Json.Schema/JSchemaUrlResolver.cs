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
        /// Gets the schema for a given schema ID context.
        /// </summary>
        /// <param name="context">The schema ID context to resolve.</param>
        /// <returns>The resolved schema.</returns>
        public override JSchema GetSchema(ResolveSchemaContext context)
        {
            if (!context.SchemaId.IsAbsoluteUri)
                return null;

            Uri uri = context.SchemaId;
            string fragment = uri.Fragment;

            UriBuilder b = new UriBuilder(uri);
            b.Fragment = string.Empty;

            Uri requestUri = b.Uri;

#if !PORTABLE
            int? timeout = Timeout;
#else
            int? timeout = null;
#endif

            using (Stream s = _downloader.GetStream(requestUri, _credentials, timeout, ByteLimit))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(sr))
            {
                JSchema schema = JSchema.Load(jsonReader, this);

                // no fragment, returned schema is final result
                if (string.IsNullOrEmpty(fragment))
                    return schema;

                // look inside schema with fragment
                JSchema relativeSchema = ResolveChildSchema(schema, requestUri, new Uri(fragment, UriKind.Relative));

                return relativeSchema;
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