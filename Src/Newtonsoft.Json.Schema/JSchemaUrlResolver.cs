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
    /// Resolves <see cref="JSchema"/> from an id.
    /// </summary>
    public class JSchemaUrlResolver : JSchemaResolver
    {
        private IDownloader _downloader;
        private ICredentials _credentials;

#if DEBUG
        internal void SetDownloader(IDownloader downloader)
        {
            _downloader = downloader;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaResolver"/> class.
        /// </summary>
        public JSchemaUrlResolver()
        {
            _downloader = new WebRequestDownloader();
        }

        public override JSchema GetSchema(Uri uri)
        {
            using (Stream s = _downloader.GetStream(uri, _credentials))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(sr))
            {
                return JSchema.Read(jsonReader);
            }
        }

        public override ICredentials Credentials
        {
            set { _credentials = value; }
        }
    }
}