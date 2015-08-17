#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Specifies the settings used when reading a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaReaderSettings
    {
        private SchemaValidationEventHandler _validationEventHandler;

        /// <summary>
        /// The base URI for the schema being read. The base URI is used to resolve relative URI schema references.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// The <see cref="JSchemaResolver"/> to use when resolving schema references.
        /// </summary>
        public JSchemaResolver Resolver { get; set; }

        /// <summary>
        /// Gets or sets a flag to indicate whether the schema JSON should be validated using the JSON Schema version identifer defined by '$schema'.
        /// </summary>
        public bool ValidateVersion { get; set; }

        /// <summary>
        /// Sets an event handler for receiving information about receives information about JSON Schema syntax errors. 
        /// </summary>
        public event SchemaValidationEventHandler ValidationEventHandler
        {
            add { _validationEventHandler += value; }
            remove { _validationEventHandler -= value; }
        }

        internal SchemaValidationEventHandler GetValidationEventHandler()
        {
            return _validationEventHandler;
        }

        internal void SetValidationEventHandler(SchemaValidationEventHandler handler)
        {
            _validationEventHandler = handler;
        }
    }
}
