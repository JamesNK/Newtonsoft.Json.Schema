#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents a JSON Schema validation error.
    /// </summary>
    public class ValidationError : IJsonLineInfo
    {
        private IList<ValidationError> _childErrors;

        /// <summary>
        /// Gets the message describing the error that occurred.
        /// </summary>
        /// <value>The message describing the error that occurred.</value>
        public string Message { get; internal set; }

        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber { get; internal set; }

        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition { get; internal set; }

        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        public string Path { get; internal set; }

        /// <summary>
        /// Gets the JSON value when the error occurred.
        /// </summary>
        /// <value>The JSON value when the error occurred.</value>
        public object Value { get; internal set; }

        /// <summary>
        /// Gets the <see cref="JSchema"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="JSchema"/> that generated the error.</value>
        [JsonIgnore]
        public JSchema Schema { get; internal set; }

        /// <summary>
        /// Gets the ID of the <see cref="JSchema"/> that generated the error, relative to the root schema.
        /// </summary>
        /// <value>The path of the <see cref="JSchema"/> that generated the error, relative to the root schema.</value>
        public Uri SchemaId { get; set; }

        /// <summary>
        /// Gets the <see cref="ErrorType"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="ErrorType"/> that generated the error.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType ErrorType { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ValidationError"/>'s child errors.
        /// </summary>
        /// <value>The <see cref="ValidationError"/>'s child errors.</value>
        public IList<ValidationError> ChildErrors
        {
            get
            {
                if (_childErrors == null)
                    _childErrors = new List<ValidationError>();

                return _childErrors;
            }
            internal set { _childErrors = value; }
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return (LineNumber > 0 && LinePosition > 0);
        }

        internal string BuildExtendedMessage()
        {
            return JSchemaException.FormatMessage(this, Path, Message);
        }
    }
}