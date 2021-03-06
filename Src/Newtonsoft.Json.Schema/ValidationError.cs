#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents a JSON Schema validation error.
    /// </summary>
    [DebuggerDisplay("{GetExtendedMessage(),nq}")]
    public class ValidationError : IJsonLineInfo
    {
        internal IList<ValidationError>? _childErrors;
        private IFormattable? _formattable;
        private string? _message;
        private string? _extendedMessage;

        /// <summary>
        /// Gets the message describing the error that occurred.
        /// </summary>
        /// <value>The message describing the error that occurred.</value>
        public string Message
        {
            get
            {
                EnsureMessages();
                return _message;
            }
        }

        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        public string Path { get; private set; } = default!;

        /// <summary>
        /// Gets the JSON value when the error occurred.
        /// </summary>
        /// <value>The JSON value when the error occurred.</value>
        public object? Value { get; private set; }

        /// <summary>
        /// Gets the <see cref="JSchema"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="JSchema"/> that generated the error.</value>
        [JsonIgnore]
        public JSchema Schema { get; private set; } = default!;

        /// <summary>
        /// Gets the ID of the <see cref="JSchema"/> that generated the error, relative to the root schema.
        /// </summary>
        /// <value>The path of the <see cref="JSchema"/> that generated the error, relative to the root schema.</value>
        public Uri? SchemaId { get; internal set; }

        /// <summary>
        /// Gets the base URI of the <see cref="JSchema"/> that generated the error.
        /// </summary>
        /// <value>The base URI of the <see cref="JSchema"/> that generated the error.</value>
        public Uri? SchemaBaseUri { get; private set; }

        /// <summary>
        /// Gets the <see cref="ErrorType"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="ErrorType"/> that generated the error.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType ErrorType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ValidationError"/>'s child errors.
        /// </summary>
        /// <value>The <see cref="ValidationError"/>'s child errors.</value>
        public IList<ValidationError> ChildErrors
        {
            get
            {
                if (_childErrors == null)
                {
                    _childErrors = new List<ValidationError>();
                }

                return _childErrors;
            }
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return (LineNumber > 0 && LinePosition > 0);
        }

        [MemberNotNull(nameof(_message))]
        [MemberNotNull(nameof(_extendedMessage))]
        private void EnsureMessages()
        {
            if (_formattable != null)
            {
                BuildMessages();
                _formattable = null;
            }

            ValidationUtils.Assert(_message != null);
            ValidationUtils.Assert(_extendedMessage != null);
        }

        [MemberNotNull(nameof(_message))]
        [MemberNotNull(nameof(_extendedMessage))]
        private void BuildMessages()
        {
            ValidationUtils.Assert(_formattable != null);

            if (_formattable is FormattableString formattableString)
            {
                string format = formattableString.Format;
                object[] args = formattableString.GetArguments();

                StringBuilder sb = new(format.Length + (args.Length * 8));
                if (args.Length > 0)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, format, args);
                }
                else
                {
                    sb.Append(format);
                }

                _message = sb.ToString();
                _extendedMessage = JSchemaException.FormatMessage(this, Path, sb);
            }
            else
            {
                string message = _formattable.ToString(null, CultureInfo.InvariantCulture);

                _message = message;
                _extendedMessage = JSchemaException.FormatMessage(this, Path, message);
            }

            _formattable = null;
        }

        internal string GetExtendedMessage()
        {
            EnsureMessages();
            return _extendedMessage;
        }

        internal static ValidationError CreateValidationError(IFormattable message, ErrorType errorType, JSchema schema, Uri? schemaId, object? value, IList<ValidationError>? childErrors, IJsonLineInfo? lineInfo, string path)
        {
            ValidationError error = new()
            {
                _formattable = message,
                _childErrors = childErrors,

                ErrorType = errorType,
                Path = path
            };
            if (lineInfo != null)
            {
                error.LineNumber = lineInfo.LineNumber;
                error.LinePosition = lineInfo.LinePosition;
            }
            error.Schema = schema;
            error.SchemaId = schemaId;
            error.SchemaBaseUri = schema.BaseUri;
            error.Value = value;
            return error;
        }
    }
}