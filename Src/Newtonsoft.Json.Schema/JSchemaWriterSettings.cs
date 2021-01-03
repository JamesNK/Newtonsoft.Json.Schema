#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies the settings used when writing a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaWriterSettings
    {
        /// <summary>
        /// Gets or set the <see cref="SchemaVersion"/> of the written JSON. Explicitly setting the version will include the <c>$schema</c> property
        /// and limit the written schema to only features available in that version.
        /// </summary>
        /// <value>The <see cref="SchemaVersion"/> of the written JSON.</value>
        public SchemaVersion? Version { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExternalSchema"/>s used when resolving the schema reference to write.
        /// </summary>
        /// <value>The <see cref="ExternalSchema"/>s used when resolving the schema reference to write.</value>
        public IList<ExternalSchema>? ExternalSchemas { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="JSchemaWriterReferenceHandling"/> setting used when writing schema references.
        /// This setting doesn't effect <see cref="ExternalSchemas"/> references.
        /// </summary>
        /// <value>The <see cref="JSchemaWriterReferenceHandling"/> setting used when writing schema references.</value>
        public JSchemaWriterReferenceHandling ReferenceHandling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaWriterSettings"/> class.
        /// </summary>
        public JSchemaWriterSettings()
        {
            ExternalSchemas = new List<ExternalSchema>();
        }
    }
}