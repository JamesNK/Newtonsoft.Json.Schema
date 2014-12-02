#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves <see cref="JSchema"/> from an id.
    /// </summary>
    public class JSchemaResolver
    {
        /// <summary>
        /// Gets or sets the loaded schemas.
        /// </summary>
        /// <value>The loaded schemas.</value>
        public IList<JSchema> LoadedSchemas { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaResolver"/> class.
        /// </summary>
        public JSchemaResolver()
        {
            LoadedSchemas = new List<JSchema>();
        }

        /// <summary>
        /// Gets a <see cref="JSchema"/> for the specified reference.
        /// </summary>
        /// <param name="reference">The id.</param>
        /// <returns>A <see cref="JSchema"/> for the specified reference.</returns>
        public virtual JSchema GetSchema(string reference)
        {
            JSchema schema = LoadedSchemas.SingleOrDefault(s => string.Equals(s.Id, reference, StringComparison.Ordinal));

            if (schema == null)
                schema = LoadedSchemas.SingleOrDefault(s => string.Equals(s.Location, reference, StringComparison.Ordinal));

            return schema;
        }
    }
}