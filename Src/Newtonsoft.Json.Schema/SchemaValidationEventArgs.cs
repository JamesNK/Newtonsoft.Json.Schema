#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Returns detailed information related to the <see cref="SchemaValidationEventHandler"/>.
    /// </summary>
    public class SchemaValidationEventArgs : EventArgs
    {
        private readonly ValidationError _validationError;

        internal SchemaValidationEventArgs(ValidationError validationError)
        {
            ValidationUtils.ArgumentNotNull(validationError, nameof(validationError));
            _validationError = validationError;
        }

        /// <summary>
        /// Gets the <see cref="ValidationError"/> associated with the validation event.
        /// </summary>
        /// <value>The <see cref="ValidationError"/> associated with the validation event.</value>
        public ValidationError ValidationError => _validationError;

        /// <summary>
        /// Gets the path of the JSON location where the validation event occurred.
        /// </summary>
        /// <value>The path of the JSON location where the validation event occurred.</value>
        public string Path => _validationError.Path;

        /// <summary>
        /// Gets the text description corresponding to the validation event.
        /// </summary>
        /// <value>The text description.</value>
        public string Message => _validationError.GetExtendedMessage();
    }
}