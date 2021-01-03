#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves external JSON Schemas named by a Uniform Resource Identifier (URI) using cached schemas.
    /// </summary>
    public class JSchemaPreloadedResolver : JSchemaResolver
    {
        private readonly Dictionary<Uri, byte[]> _preloadedData;
        private readonly JSchemaResolver? _resolver;

        /// <summary>
        /// Gets a collection of preloaded URIs.
        /// </summary>
        public IEnumerable<Uri> PreloadedUris => _preloadedData.Keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaPreloadedResolver"/> class with the specified fallback resolver.
        /// </summary>
        /// <param name="resolver">The fallback resolver used when no cached schema was found.</param>
        public JSchemaPreloadedResolver(JSchemaResolver resolver)
            : this()
        {
            _resolver = resolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaPreloadedResolver"/> class.
        /// </summary>
        public JSchemaPreloadedResolver()
        {
            _preloadedData = new Dictionary<Uri, byte[]>(UriComparer.Instance);
        }

        /// <summary>
        /// Gets the schema resource for a given schema reference.
        /// </summary>
        /// <param name="context">The schema ID context.</param>
        /// <param name="reference">The schema reference.</param>
        /// <returns>The schema resource for a given schema reference.</returns>
        public override Stream? GetSchemaResource(ResolveSchemaContext context, SchemaReference reference)
        {
            if (_preloadedData.TryGetValue(reference.BaseUri, out byte[] data))
            {
                return new MemoryStream(data);
            }

            return _resolver?.GetSchemaResource(context, reference);
        }

        /// <summary>
        /// Adds a byte array for a schema to the <see cref="JSchemaPreloadedResolver"/> store and maps it to a URI.
        /// </summary>
        /// <param name="uri">The URI of the schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        /// <param name="value">The byte array for a schema that corresponds to the provided URI.</param>
        public void Add(Uri uri, byte[] value)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _preloadedData[uri] = value;
        }

        /// <summary>
        /// Adds a <see cref="Stream"/> for a schema to the <see cref="JSchemaPreloadedResolver"/> store and maps it to a URI.
        /// </summary>
        /// <param name="uri">The URI of the schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        /// <param name="value">The <see cref="Stream"/> for a schema that corresponds to the provided URI.</param>
        public void Add(Uri uri, Stream value)
        {
            MemoryStream ms = new MemoryStream();
            value.CopyTo(ms);

            Add(uri, ms.ToArray());
        }

        /// <summary>
        /// Adds a <see cref="String"/> for a schema to the <see cref="JSchemaPreloadedResolver"/> store and maps it to a URI.
        /// </summary>
        /// <param name="uri">The URI of the schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        /// <param name="value">The <see cref="String"/> for a schema that corresponds to the provided URI.</param>
        public void Add(Uri uri, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);

            Add(uri, data);
        }
    }
}