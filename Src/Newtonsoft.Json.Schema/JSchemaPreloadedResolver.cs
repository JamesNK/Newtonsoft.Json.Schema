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
    public class JSchemaPreloadedResolver : JSchemaResolver
    {
        private readonly List<KnownSchema> _knownSchemas;
        private readonly JSchemaResolver _resolver;

        public JSchemaPreloadedResolver(JSchemaResolver resolver)
            : this()
        {
            _resolver = resolver;
        }

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

        public void Add(Uri uri, JSchema schema)
        {
            if (_knownSchemas.Any(s => Uri.Compare(s.Id, uri, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal) == 0))
                throw new JsonException("Resolver already has a JSON Schema for URI '{0}'.".FormatWith(CultureInfo.InvariantCulture, uri));

            _knownSchemas.Add(new KnownSchema(uri, schema, KnownSchemaState.External));
        }

        public void Add(JSchema schema)
        {
            Uri resolvedId = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            Add(resolvedId, schema);
        }
    }
}