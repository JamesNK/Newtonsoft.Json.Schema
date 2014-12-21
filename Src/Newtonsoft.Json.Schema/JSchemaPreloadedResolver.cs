#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves external JSON Schemas named by a Uniform Resource Identifier (URI) using cached schemas.
    /// </summary>
    public class JSchemaPreloadedResolver : JSchemaResolver
    {
        private readonly List<KnownSchema> _knownSchemas;
        private readonly JSchemaResolver _resolver;

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
            _knownSchemas = new List<KnownSchema>();
        }

        /// <summary>
        /// Gets the schema for a given URI.
        /// </summary>
        /// <param name="uri">The schema URI to resolve.</param>
        /// <returns>The resolved schema.</returns>
        public override JSchema GetSchema(Uri uri)
        {
            foreach (KnownSchema knownSchema in _knownSchemas)
            {
                string uriText = uri.ToString();
                string knownText = (knownSchema.Id != null) ? knownSchema.Id.ToString() : string.Empty;

                if (uriText == knownText)
                    return knownSchema.Schema;

                if (uriText.StartsWith(knownText, StringComparison.Ordinal))
                {
                    string relative = uriText.Substring(knownText.Length);
                    Uri relativeUri = new Uri(relative, UriKind.RelativeOrAbsolute);

                    JSchemaReader resolverSchemaReader = new JSchemaReader(this)
                    {
                        RootSchema = knownSchema.Schema
                    };

                    JSchema subSchema = null;
                    
                    SchemaDiscovery.FindSchema(s => subSchema = s, knownSchema.Schema, knownSchema.Id, relativeUri, resolverSchemaReader);

                    if (subSchema != null)
                    {
                        resolverSchemaReader.ResolveDeferedSchemas();

                        return subSchema;
                    }
                }
            }

            if (_resolver != null)
                return _resolver.GetSchema(uri);

            return null;
        }

        /// <summary>
        /// Adds a <see cref="JSchema"/> to the <see cref="JSchemaPreloadedResolver"/> store and maps it to a URI.
        /// If the store already contains a mapping for the same URI, the existing mapping is overridden.
        /// </summary>
        /// <param name="schema">The schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        /// <param name="uri">The URI of the schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        public void Add(JSchema schema, Uri uri)
        {
            var existing = _knownSchemas.SingleOrDefault(s => Uri.Compare(s.Id, uri, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal) == 0);
            if (existing != null)
                _knownSchemas.Remove(existing);

            _knownSchemas.Add(new KnownSchema(uri, schema, KnownSchemaState.External));
        }

        /// <summary>
        /// Adds a <see cref="JSchema"/> to the <see cref="JSchemaPreloadedResolver"/> store and maps it to a URI taken from the root schema's ID.
        /// If the store already contains a mapping for the same URI, the existing mapping is overridden.
        /// </summary>
        /// <param name="schema">The schema that is being added to the <see cref="JSchemaPreloadedResolver"/> store.</param>
        public void Add(JSchema schema)
        {
            Uri resolvedId = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            Add(schema, resolvedId);
        }
    }
}