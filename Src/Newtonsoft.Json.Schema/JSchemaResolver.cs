#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves external <see cref="JSchema"/>s by schema IDs.
    /// </summary>
    public abstract class JSchemaResolver
    {
        /// <summary>
        /// Gets the schema resource for a given schema reference.
        /// </summary>
        /// <param name="context">The schema ID context.</param>
        /// <param name="reference">The schema reference.</param>
        /// <returns>The schema resource for a given schema reference.</returns>
        public abstract Stream? GetSchemaResource(ResolveSchemaContext context, SchemaReference reference);

        /// <summary>
        /// Resolves the schema reference from the specified schema ID context.
        /// </summary>
        /// <param name="context">The schema ID context to resolve.</param>
        /// <returns>The resolved schema reference.</returns>
        public virtual SchemaReference ResolveSchemaReference(ResolveSchemaContext context)
        {
            Uri? baseUri = ResolveBaseUri(context, out string? fragment);

            SchemaReference schemaReference = new SchemaReference();
            schemaReference.BaseUri = baseUri;
            if (fragment != null)
            {
                schemaReference.SubschemaId = new Uri(fragment, UriKind.RelativeOrAbsolute);
            }

            return schemaReference;
        }

        private Uri? ResolveBaseUri(ResolveSchemaContext context, out string? fragment)
        {
            Uri? baseUri = context.ResolverBaseUri;

            Uri uri = context.ResolvedSchemaId;
            if (!uri.IsAbsoluteUri && uri.OriginalString.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                fragment = uri.OriginalString;
                return baseUri;
            }

            if (baseUri == null || (!baseUri.IsAbsoluteUri && baseUri.OriginalString.Length == 0))
            {
                return RemoveFragment(uri, out fragment);
            }

            uri = RemoveFragment(uri, out fragment);

            uri = SchemaDiscovery.ResolveSchemaId(baseUri, uri);
            return uri;
        }

        private Uri RemoveFragment(Uri uri, out string? fragment)
        {
            int index = StringHelpers.IndexOf(uri.OriginalString, '#');

            if (uri.IsAbsoluteUri && index == -1)
            {
                fragment = null;
                return uri;
            }

            string uriText;

            if (index != -1)
            {
                uriText = uri.OriginalString.Substring(0, index);
                fragment = uri.OriginalString.Substring(index);
            }
            else
            {
                uriText = uri.OriginalString;
                fragment = null;
            }

            Uri resolvedUri = new Uri(uriText, UriKind.RelativeOrAbsolute);
            return resolvedUri;
        }

        /// <summary>
        /// Finds a subschema using the given schema reference.
        /// </summary>
        /// <param name="reference">The schema reference used to get the subschema.</param>
        /// <param name="rootSchema">The root schema to resolve the subschema from.</param>
        /// <returns>The matching subschema.</returns>
        public virtual JSchema? GetSubschema(SchemaReference reference, JSchema rootSchema)
        {
            if (reference.SubschemaId == null)
            {
                return rootSchema;
            }

            Uri? rootSchemaId = reference.BaseUri;
            Uri subschemaId = reference.SubschemaId;

            JSchemaReader? resolverSchemaReader = rootSchema.InternalReader;
            if (resolverSchemaReader == null)
            {
                resolverSchemaReader = new JSchemaReader(new JSchemaReaderSettings
                {
                    Resolver = this,
                    BaseUri = rootSchema.BaseUri
                });
                resolverSchemaReader.RootSchema = rootSchema;
            }

            JSchema? subSchema = null;

            Uri? dynamicScope = null; // Might be a problem. Should dynamic scope be passed in?
            ReferenceType referenceType = ReferenceType.Ref; // ref lookup always used here. Might need to support passing in reference type?

            FindSchemaContext context = new FindSchemaContext(s => subSchema = s, rootSchema, rootSchemaId, subschemaId, subschemaId, dynamicScope, referenceType, resolverSchemaReader, resolverSchemaReader._schemaDiscovery);
            SchemaDiscovery.FindSchema(context);

            if (subSchema != null)
            {
                resolverSchemaReader.ResolveDeferredSchemas();
                resolverSchemaReader.RaiseValidationErrors();

                return subSchema;
            }

            return null;
        }

        /// <summary>
        /// Sets the credentials used to authenticate web requests.
        /// </summary>
        public virtual ICredentials Credentials
        {
            set { }
        }
    }
}