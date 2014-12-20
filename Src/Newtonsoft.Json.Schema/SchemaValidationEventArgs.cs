#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Returns detailed information related to the <see cref="ValidationEventHandler"/>.
    /// </summary>
    public class SchemaValidationEventArgs : EventArgs
    {
        private readonly JSchemaException _ex;

        internal SchemaValidationEventArgs(JSchemaException ex)
        {
            ValidationUtils.ArgumentNotNull(ex, "ex");
            _ex = ex;
        }

        /// <summary>
        /// Gets the <see cref="JSchemaException"/> associated with the validation error.
        /// </summary>
        /// <value>The <see cref="JSchemaException"/> associated with the validation error.</value>
        public JSchemaException Exception
        {
            get { return _ex; }
        }

        /// <summary>
        /// Gets the path of the JSON location where the validation error occurred.
        /// </summary>
        /// <value>The path of the JSON location where the validation error occurred.</value>
        public string Path
        {
            get { return _ex.Path; }
        }

        /// <summary>
        /// Gets the text description corresponding to the validation error.
        /// </summary>
        /// <value>The text description.</value>
        public string Message
        {
            get { return _ex.Message; }
        }
    }
}