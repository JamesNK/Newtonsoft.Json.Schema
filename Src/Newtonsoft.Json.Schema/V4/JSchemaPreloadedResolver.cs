#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema.V4.Infrastructure;

namespace Newtonsoft.Json.Schema.V4
{
    public class JSchemaPreloadedResolver : JSchema4Resolver
    {
        private readonly List<KnownSchema> _knownSchemas;
        private readonly JSchema4Resolver _resolver;

        public JSchemaPreloadedResolver(JSchema4Resolver resolver)
            : this()
        {
            _resolver = resolver;
        }

        public JSchemaPreloadedResolver()
        {
            _knownSchemas = new List<KnownSchema>();
        }

        public override JSchema4 GetSchema(Uri uri)
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

                    JSchema4Reader resolverSchemaReader = new JSchema4Reader(this)
                    {
                        RootSchema = knownSchema.Schema
                    };

                    JSchema4 subSchema = null;
                    
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

        public void Add(Uri uri, JSchema4 schema)
        {
            _knownSchemas.Add(new KnownSchema(uri, schema, KnownSchemaState.External));
        }

        public void Add(JSchema4 schema)
        {
            Uri resolvedId = schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            Add(resolvedId, schema);
        }
    }
}