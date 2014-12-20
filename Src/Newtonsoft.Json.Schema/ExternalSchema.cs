#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema
{
    public class ExternalSchema
    {
        private readonly Uri _uri;
        private readonly JSchema _schema;

        public Uri Uri
        {
            get { return _uri; }
        }

        public JSchema Schema
        {
            get { return _schema; }
        }

        public ExternalSchema(Uri uri, JSchema schema)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = uri;
            _schema = schema;
        }

        public ExternalSchema(JSchema schema)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);
            _schema = schema;
        }
    }
}