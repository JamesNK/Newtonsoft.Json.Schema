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
        /// <summary>
        /// Gets the schema URI.
        /// </summary>
        /// <value>The schema URI.</value>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public JSchema Schema { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSchema"/> class with the specified URI and <see cref="JSchema"/>.
        /// </summary>
        /// <param name="uri">The schema URI.</param>
        /// <param name="schema">The schema.</param>
        public ExternalSchema(Uri uri, JSchema schema)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSchema"/> class with the specified <see cref="JSchema"/>.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public ExternalSchema(JSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            Uri = schema.ResolvedId ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);
            Schema = schema;
        }
    }
}