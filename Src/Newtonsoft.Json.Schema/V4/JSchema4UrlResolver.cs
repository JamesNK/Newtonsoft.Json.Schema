#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Schema.V4.Infrastructure;

namespace Newtonsoft.Json.Schema.V4
{
    /// <summary>
    /// Resolves <see cref="JSchema"/> from an id.
    /// </summary>
    public class JSchema4UrlResolver : JSchema4Resolver
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
        /// Gets or sets the loaded schemas.
        /// </summary>
        /// <value>The loaded schemas.</value>
        //internal IList<JSchema4.InlineSchema> LoadedSchemas { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaResolver"/> class.
        /// </summary>
        public JSchema4UrlResolver()
        {
            //LoadedSchemas = new List<JSchema4.InlineSchema>();
            _downloader = new WebRequestDownloader();
        }

        //public virtual void RegisterSchema(JSchema4 schema)
        //{
        //    LoadedSchemas.Add(new JSchema4.InlineSchema
        //    {
        //        Path = schema.Id,
        //        Schema = schema
        //    });
        //}

        //public virtual void RegisterSchema(JSchema4 schema, string path)
        //{
        //    LoadedSchemas.Add(new JSchema4.InlineSchema
        //    {
        //        Path = path,
        //        Schema = schema
        //    });
        //}

        ///// <summary>
        ///// Gets a <see cref="JSchema"/> for the specified reference.
        ///// </summary>
        ///// <param name="reference">The id.</param>
        ///// <returns>A <see cref="JSchema"/> for the specified reference.</returns>
        //public virtual JSchema4 GetSchema(string reference)
        //{
        //    JSchema4.InlineSchema schema = LoadedSchemas.SingleOrDefault(s => s.Path == reference);

        //    if (schema == null)
        //    {
        //        Uri uri;
        //        if (!Uri.TryCreate(reference, UriKind.Absolute, out uri))
        //            return null;

        //        using (Stream s = _downloader.GetStream(uri, null))
        //        using (StreamReader sr = new StreamReader(s))
        //        using (JsonTextReader jsonReader = new JsonTextReader(sr))
        //        {
        //            return JSchema4.Read(jsonReader);
        //        }
        //    }

        //    return schema.Schema;
        //}

        public override JSchema4 GetSchema(Uri uri)
        {
            using (Stream s = _downloader.GetStream(uri, _credentials))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(sr))
            {
                return JSchema4.Read(jsonReader);
            }
        }

        public override ICredentials Credentials
        {
            set { _credentials = value; }
        }
    }
}