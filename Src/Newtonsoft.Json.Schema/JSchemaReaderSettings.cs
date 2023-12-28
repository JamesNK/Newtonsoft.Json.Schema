#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies the settings used when reading a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaReaderSettings
    {
        // IMPORTANT: Any settings added here need to be copied inside JSchemaReader

        private SchemaValidationEventHandler? _validationEventHandler;

        /// <summary>
        /// The base URI for the schema being read. The base URI is used to resolve relative URI schema references.
        /// </summary>
        public Uri? BaseUri { get; set; }

        /// <summary>
        /// The <see cref="JSchemaResolver"/> to use when resolving schema references.
        /// </summary>
        public JSchemaResolver? Resolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether schema references should be resolved.
        /// </summary>
        /// <remarks>Default is <c>true</c></remarks>
        public bool ResolveSchemaReferences { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the schema JSON should be validated using the JSON Schema version identifier defined by '$schema'.
        /// </summary>
        public bool ValidateVersion { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="JsonValidator"/> collection that will be used during validation.
        /// </summary>
        /// <value>The converters.</value>
        public IList<JsonValidator>? Validators { get; set; }

        /// <summary>
        /// Sets an event handler for receiving information about receives information about JSON Schema syntax errors. 
        /// </summary>
        public event SchemaValidationEventHandler ValidationEventHandler
        {
            add => _validationEventHandler += value;
            remove => _validationEventHandler -= value;
        }

        internal SchemaValidationEventHandler? GetValidationEventHandler()
        {
            return _validationEventHandler;
        }

        internal void SetValidationEventHandler(SchemaValidationEventHandler? handler)
        {
            _validationEventHandler = handler;
        }
    }
}