#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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
        /// Gets the schema for a given schema ID context.
        /// </summary>
        /// <param name="context">The schema ID context to resolve.</param>
        /// <returns>The resolved schema.</returns>
        public abstract JSchema GetSchema(ResolveSchemaContext context);

        /// <summary>
        /// Sets the credentials used to authenticate web requests.
        /// </summary>
        public virtual ICredentials Credentials
        {
            set { }
        }

        /// <summary>
        /// Finds a child schema using the given ID.
        /// </summary>
        /// <param name="parentSchema">The parent schema to resolve the child schema from.</param>
        /// <param name="parentSchemaId">The parent schema ID.</param>
        /// <param name="childSchemaId">The child schema ID.</param>
        /// <returns>The child schema.</returns>
        protected JSchema ResolveChildSchema(JSchema parentSchema, Uri parentSchemaId, Uri childSchemaId)
        {
            JSchemaReader resolverSchemaReader = new JSchemaReader(this)
            {
                RootSchema = parentSchema
            };

            JSchema subSchema = null;

            SchemaDiscovery.FindSchema(s => subSchema = s, parentSchema, parentSchemaId, childSchemaId, resolverSchemaReader);

            if (subSchema != null)
            {
                resolverSchemaReader.ResolveDeferedSchemas();

                return subSchema;
            }

            return null;
        }
    }
}