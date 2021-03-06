#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Validation;
using Newtonsoft.Json.Schema.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents a reader that provides <see cref="JSchema"/> validation.
    /// </summary>
    public class JSchemaValidatingReader : JsonReader, IJsonLineInfo
    {
        internal class ReaderValidator : Validator
        {
            private readonly JSchemaValidatingReader _reader;

            public ReaderValidator(JSchemaValidatingReader reader)
                : base(reader)
            {
                _reader = reader;
            }

            public override ValidationError CreateError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
            {
                return CreateError(message, errorType, schema, value, childErrors, _reader, _reader.Path);
            }
        }

        private readonly JsonReader _reader;
        private readonly ReaderValidator _validator;

        private object? _readAsValue;
        private JsonToken? _readAsToken;

        internal ReaderValidator Validator => _validator;

        /// <summary>
        /// Sets an event handler for receiving schema validation errors.
        /// </summary>
        public event SchemaValidationEventHandler ValidationEventHandler
        {
            add => _validator.ValidationEventHandler += value;
            remove => _validator.ValidationEventHandler -= value;
        }

#if !(NET35 || NET40)
        /// <summary>
        /// Gets or sets a timeout that will be used when executing regular expressions.
        /// </summary>
        public TimeSpan? RegexMatchTimeout
        {
            get => _validator.RegexMatchTimeout;
            set => _validator.RegexMatchTimeout = value;
        }
#endif

        /// <summary>
        /// Gets the text value of the current JSON token.
        /// </summary>
        /// <value></value>
        public override object? Value => _readAsValue ?? _reader.Value;

        /// <summary>
        /// Gets the depth of the current token in the JSON document.
        /// </summary>
        /// <value>The depth of the current token in the JSON document.</value>
        public override int Depth => _reader.Depth;

        /// <summary>
        /// Gets the path of the current JSON token. 
        /// </summary>
        public override string Path => _reader.Path;

        /// <summary>
        /// Gets the quotation mark character used to enclose the value of a string.
        /// </summary>
        /// <value></value>
        public override char QuoteChar => _reader.QuoteChar;

        /// <summary>
        /// Gets the type of the current JSON token.
        /// </summary>
        /// <value></value>
        public override JsonToken TokenType => _readAsToken ?? _reader.TokenType;

        /// <summary>
        /// Gets the Common Language Runtime (CLR) type for the current JSON token.
        /// </summary>
        /// <value></value>
        public override Type? ValueType => Value?.GetType();

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public JSchema? Schema
        {
            get => _validator.Schema;
            set => _validator.Schema = value;
        }

        /// <summary>
        /// Gets the <see cref="JsonReader"/> used to construct this <see cref="JSchemaValidatingReader"/>.
        /// </summary>
        /// <value>The <see cref="JsonReader"/> specified in the constructor.</value>
        public JsonReader Reader => _reader;

        bool IJsonLineInfo.HasLineInfo()
        {
            return _reader is IJsonLineInfo lineInfo && lineInfo.HasLineInfo();
        }

        int IJsonLineInfo.LineNumber => (_reader is IJsonLineInfo lineInfo) ? lineInfo.LineNumber : 0;

        int IJsonLineInfo.LinePosition => (_reader is IJsonLineInfo lineInfo) ? lineInfo.LinePosition : 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaValidatingReader"/> class that
        /// validates the content returned from the given <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from while validating.</param>
        public JSchemaValidatingReader(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, nameof(reader));
            _reader = reader;
            _validator = new ReaderValidator(this);
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Int32}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Int32}"/>.</returns>
        public override int? ReadAsInt32()
        {
            int? i = base.ReadAsInt32();

            if (i != null)
            {
                _readAsValue = i;
                _readAsToken = JsonToken.Integer;
            }

            return i;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Boolean}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Boolean}"/>.</returns>
        public override bool? ReadAsBoolean()
        {
            bool? b = base.ReadAsBoolean();

            if (b != null)
            {
                _readAsValue = b;
                _readAsToken = JsonToken.Boolean;
            }

            return b;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="byte"/>[].
        /// </summary>
        /// <returns>
        /// A <see cref="byte"/>[] or a null reference if the next JSON token is null.
        /// </returns>
        public override byte[]? ReadAsBytes()
        {
            byte[]? data = base.ReadAsBytes();

            if (data != null)
            {
                _readAsValue = data;
                _readAsToken = JsonToken.Bytes;
            }

            return data;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Decimal}"/>.</returns>
        public override decimal? ReadAsDecimal()
        {
            decimal? d = base.ReadAsDecimal();

            if (d != null)
            {
                _readAsValue = d;
                _readAsToken = JsonToken.Float;
            }

            return d;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Double}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Double}"/>.</returns>
        public override double? ReadAsDouble()
        {
            double? d = base.ReadAsDouble();

            if (d != null)
            {
                _readAsValue = d;
                _readAsToken = JsonToken.Float;
            }

            return d;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override string? ReadAsString()
        {
            string? s;
            DateParseHandling initialDateParseHandling = _reader.DateParseHandling;

            try
            {
                _reader.DateParseHandling = DateParseHandling.None;

                s = base.ReadAsString();
            }
            finally
            {
                _reader.DateParseHandling = initialDateParseHandling;
            }

            if (s != null)
            {
                _readAsValue = s;
                _readAsToken = JsonToken.String;
            }

            return s;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTime}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override DateTime? ReadAsDateTime()
        {
            DateTime? dateTime = base.ReadAsDateTime();

            if (dateTime != null)
            {
                _readAsValue = dateTime;
                _readAsToken = JsonToken.Date;
            }

            return dateTime;
        }

#if !NET20
        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTimeOffset}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{DateTimeOffset}"/>.</returns>
        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            DateTimeOffset? dateTimeOffset = _reader.ReadAsDateTimeOffset();

            if (dateTimeOffset != null)
            {
                _readAsValue = dateTimeOffset;
                _readAsToken = JsonToken.Date;
            }

            return dateTimeOffset;
        }
#endif

        /// <summary>
        /// Reads the next JSON token from the stream.
        /// </summary>
        /// <returns>
        /// true if the next token was read successfully; false if there are no more tokens to read.
        /// </returns>
        public override bool Read()
        {
            // clear out exlicit value and token so inner reader values are used
            _readAsValue = null;
            _readAsToken = null;

            bool success = _reader.Read();

            if (success)
            {
                ValidateCurrentToken();
            }

            return success;
        }

        /// <summary>
        /// Changes the reader's state to <see cref="JsonReader.State.Closed"/>.
        /// If <see cref="JsonReader.CloseInput"/> is set to <c>true</c>, the underlying <see cref="JsonReader"/> is also closed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (CloseInput)
            {
                _reader?.Close();
            }
        }

        private void ValidateCurrentToken()
        {
            JsonToken token = _reader.TokenType;

            if (token == JsonToken.Comment)
            {
                return;
            }

            object? value = _reader.Value;
            if (token == JsonToken.Date)
            {
                if (value is DateTimeOffset offset)
                {
                    StringWriter sw = new(CultureInfo.InvariantCulture);
                    DateTimeUtils.WriteDateTimeOffsetString(sw, offset, DateFormatHandling.IsoDateFormat, _reader.DateFormatString, _reader.Culture);

                    value = sw.ToString();
                }
                else
                {
                    DateTime resolvedDate = DateTimeUtils.EnsureDateTime((DateTime)value!, _reader.DateTimeZoneHandling);

                    StringWriter sw = new(CultureInfo.InvariantCulture);
                    DateTimeUtils.WriteDateTimeString(sw, resolvedDate, DateFormatHandling.IsoDateFormat, _reader.DateFormatString, _reader.Culture);

                    value = sw.ToString();
                }

                token = JsonToken.String;
            }

            _validator.ValidateCurrentToken(token, value, _reader.Depth);
        }
    }
}