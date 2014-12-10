#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.V4
{
    public class ExternalSchema
    {
        private readonly Uri _uri;
        private readonly JSchema4 _schema;

        public Uri Uri
        {
            get { return _uri; }
        }

        public JSchema4 Schema
        {
            get { return _schema; }
        }

        public ExternalSchema(Uri uri, JSchema4 schema)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = uri;
            _schema = schema;
        }

        public ExternalSchema(JSchema4 schema)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);
            _schema = schema;
        }
    }
}