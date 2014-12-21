using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Schema.Infrastructure.Validation;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents a writer that provides <see cref="JSchema"/> validation.
    /// </summary>
    public class JSchemaValidatingWriter : JsonWriter
    {
        internal class WriterValidator : Validator
        {
            private readonly JSchemaValidatingWriter _writer;

            public WriterValidator(JSchemaValidatingWriter writer)
                : base(writer)
            {
                _writer = writer;
            }

            public override ISchemaError CreateError(string message, ErrorType errorType, JSchema schema, IList<ISchemaError> childErrors)
            {
                return CreateError(message, errorType, schema, childErrors, null, _writer.Path);
            }
        }

        private readonly JsonWriter _writer;
        private readonly WriterValidator _validator;

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public JSchema Schema
        {
            get { return _validator.Schema; }
            set
            {
                if (_writer.WriteState != WriteState.Start)
                    throw new InvalidOperationException("Cannot change schema while validating JSON.");

                _validator.Schema = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaValidatingWriter"/> class that
        /// validates the content that will be written to the given <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to while validating.</param>
        public JSchemaValidatingWriter(JsonWriter writer)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            _writer = writer;
            _validator = new WriterValidator(this);
        }

        /// <summary>
        /// Sets an event handler for receiving schema validation errors.
        /// </summary>
        public event SchemaValidationEventHandler ValidationEventHandler
        {
            add { _validator.ValidationEventHandler += value; }
            remove { _validator.ValidationEventHandler -= value; }
        }

        /// <summary>
        /// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Writes out a comment <code>/*...*/</code> containing the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment.</param>
        public override void WriteComment(string text)
        {
            _writer.WriteComment(text);

            base.WriteComment(text);
            ValidateCurrentToken(JsonToken.Comment, null, _writer.Top);
        }

        /// <summary>
        /// Writes the start of a constructor with the given name.
        /// </summary>
        /// <param name="name">The name of the constructor.</param>
        public override void WriteStartConstructor(string name)
        {
            _writer.WriteStartConstructor(name);

            base.WriteStartConstructor(name);
            ValidateCurrentToken(JsonToken.StartConstructor, null, _writer.Top - 1);
        }

        /// <summary>
        /// Writes raw JSON.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public override void WriteRaw(string json)
        {
            _writer.WriteRaw(json);

            base.WriteRaw(json);
            ValidateCurrentToken(JsonToken.Raw, null, _writer.Top);
        }

        /// <summary>
        /// Writes raw JSON where a value is expected and updates the writer's state.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public override void WriteRawValue(string json)
        {
            _writer.WriteRawValue(json);

            base.WriteRawValue(json);
            ValidateCurrentToken(JsonToken.Raw, null, _writer.Top);
        }

        /// <summary>
        /// Writes the beginning of a Json array.
        /// </summary>
        public override void WriteStartArray()
        {
            _writer.WriteStartArray();

            base.WriteStartArray();
            ValidateCurrentToken(JsonToken.StartArray, null, _writer.Top - 1);
        }

        /// <summary>
        /// Writes the beginning of a Json object.
        /// </summary>
        public override void WriteStartObject()
        {
            _writer.WriteStartObject();

            base.WriteStartObject();
            ValidateCurrentToken(JsonToken.StartObject, null, _writer.Top - 1);
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a Json object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public override void WritePropertyName(string name)
        {
            _writer.WritePropertyName(name);

            base.WritePropertyName(name);
            ValidateCurrentToken(JsonToken.PropertyName, name, _writer.Top);
        }

        /// <summary>
        /// Closes this stream and the underlying stream.
        /// </summary>
        public override void Close()
        {
            _writer.Close();
        }

        /// <summary>
        /// Writes the specified end token.
        /// </summary>
        /// <param name="token">The end token to write.</param>
        protected override void WriteEnd(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    _writer.WriteEndObject();

                    ValidateCurrentToken(JsonToken.EndObject, null, _writer.Top);
                    break;
                case JsonToken.EndArray:
                    _writer.WriteEndArray();

                    ValidateCurrentToken(JsonToken.EndArray, null, _writer.Top);
                    break;
                case JsonToken.EndConstructor:
                    _writer.WriteEndConstructor();

                    ValidateCurrentToken(JsonToken.EndConstructor, null, _writer.Top);
                    break;
                default:
                    throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
            }
        }

        #region WriteValue methods
        /// <summary>
        /// Writes a null value.
        /// </summary>
        public override void WriteNull()
        {
            _writer.WriteNull();

            base.WriteNull();
            ValidateCurrentToken(JsonToken.Null, null, _writer.Top);
        }

        /// <summary>
        /// Writes an undefined value.
        /// </summary>
        public override void WriteUndefined()
        {
            _writer.WriteUndefined();

            base.WriteUndefined();
            ValidateCurrentToken(JsonToken.Undefined, null, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="String"/> value.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to write.</param>
        public override void WriteValue(string value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);

            if (value != null)
                ValidateCurrentToken(JsonToken.String, value, _writer.Top);
            else
                ValidateCurrentToken(JsonToken.Null, null, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Int32"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value to write.</param>
        public override void WriteValue(int value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="UInt32"/> value.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> value to write.</param>
        [CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Int64"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value to write.</param>
        public override void WriteValue(long value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> value to write.</param>
        [CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Single"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Single"/> value to write.</param>
        public override void WriteValue(float value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Float, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Double"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value to write.</param>
        public override void WriteValue(double value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Float, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Boolean"/> value to write.</param>
        public override void WriteValue(bool value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Boolean, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Int16"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Int16"/> value to write.</param>
        public override void WriteValue(short value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="UInt16"/> value.
        /// </summary>
        /// <param name="value">The <see cref="UInt16"/> value to write.</param>
        [CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Char"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Char"/> value to write.</param>
        public override void WriteValue(char value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.String, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Byte"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/> value to write.</param>
        public override void WriteValue(byte value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="SByte"/> value.
        /// </summary>
        /// <param name="value">The <see cref="SByte"/> value to write.</param>
        [CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Integer, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Decimal"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Decimal"/> value to write.</param>
        public override void WriteValue(decimal value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Float, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value to write.</param>
        public override void WriteValue(DateTime value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);

            DateTime resolvedDate = DateTimeUtils.EnsureDateTime(value, _writer.DateTimeZoneHandling);

            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            DateTimeUtils.WriteDateTimeString(sw, resolvedDate, _writer.DateFormatHandling, _writer.DateFormatString, _writer.Culture);

            string dateText = sw.ToString();
            ValidateCurrentToken(JsonToken.String, dateText, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="DateTimeOffset"/> value.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> value to write.</param>
        public override void WriteValue(DateTimeOffset value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);

            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            DateTimeUtils.WriteDateTimeOffsetString(sw, value, DateFormatHandling, DateFormatString, Culture);

            string dateText = sw.ToString();
            ValidateCurrentToken(JsonToken.String, dateText, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="T:Byte[]"/> value.
        /// </summary>
        /// <param name="value">The <see cref="T:Byte[]"/> value to write.</param>
        public override void WriteValue(byte[] value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.Bytes, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to write.</param>
        public override void WriteValue(Guid value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.String, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> value to write.</param>
        public override void WriteValue(TimeSpan value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.String, value, _writer.Top);
        }

        /// <summary>
        /// Writes a <see cref="Uri"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Uri"/> value to write.</param>
        public override void WriteValue(Uri value)
        {
            _writer.WriteValue(value);

            base.WriteValue(value);
            ValidateCurrentToken(JsonToken.String, value, _writer.Top);
        }
        #endregion

        private void ValidateCurrentToken(JsonToken token, object value, int depth)
        {
            _validator.ValidateCurrentToken(token, value, depth);
        }
    }
}