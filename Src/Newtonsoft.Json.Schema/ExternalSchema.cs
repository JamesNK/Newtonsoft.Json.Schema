#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents an external <see cref="JSchema"/>.
    /// </summary>
    public class ExternalSchema
    {
        private readonly Uri _uri;
        private readonly JSchema _schema;

        /// <summary>
        /// Gets the schema URI.
        /// </summary>
        /// <value>The schema URI.</value>
        public Uri Uri
        {
            get { return _uri; }
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public JSchema Schema
        {
            get { return _schema; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSchema"/> class with the specified URI and <see cref="JSchema"/>.
        /// </summary>
        /// <param name="uri">The schema URI.</param>
        /// <param name="schema">The schema.</param>
        public ExternalSchema(Uri uri, JSchema schema)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = uri;
            _schema = schema;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSchema"/> class with the specified <see cref="JSchema"/>.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public ExternalSchema(JSchema schema)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            _uri = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);
            _schema = schema;
        }
    }
}