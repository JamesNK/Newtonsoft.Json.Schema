using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.V4.Infrastructure.Validation;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.V4
{
    /// <summary>
    /// Represents a reader that provides <see cref="JSchema"/> validation.
    /// </summary>
    public class JSchema4ValidatingReader : JsonReader, IJsonLineInfo, IValidator
    {
        private readonly JsonReader _reader;
        private readonly List<Scope> _scopes;
        private JSchema4 _schema;
        private ValidatorContext _context;

        /// <summary>
        /// Sets an event handler for receiving schema validation errors.
        /// </summary>
        public event SchemaValidationEventHandler ValidationEventHandler;

        /// <summary>
        /// Gets the text value of the current JSON token.
        /// </summary>
        /// <value></value>
        public override object Value
        {
            get { return _reader.Value; }
        }

        /// <summary>
        /// Gets the depth of the current token in the JSON document.
        /// </summary>
        /// <value>The depth of the current token in the JSON document.</value>
        public override int Depth
        {
            get { return _reader.Depth; }
        }

        /// <summary>
        /// Gets the path of the current JSON token. 
        /// </summary>
        public override string Path
        {
            get { return _reader.Path; }
        }

        /// <summary>
        /// Gets the quotation mark character used to enclose the value of a string.
        /// </summary>
        /// <value></value>
        public override char QuoteChar
        {
            get { return _reader.QuoteChar; }
        }

        /// <summary>
        /// Gets the type of the current JSON token.
        /// </summary>
        /// <value></value>
        public override JsonToken TokenType
        {
            get { return _reader.TokenType; }
        }

        /// <summary>
        /// Gets the Common Language Runtime (CLR) type for the current JSON token.
        /// </summary>
        /// <value></value>
        public override Type ValueType
        {
            get { return _reader.ValueType; }
        }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public JSchema4 Schema
        {
            get { return _schema; }
            set
            {
                if (TokenType != JsonToken.None)
                    throw new InvalidOperationException("Cannot change schema while validating JSON.");

                _schema = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="JsonReader"/> used to construct this <see cref="JSchemaValidatingReader"/>.
        /// </summary>
        /// <value>The <see cref="JsonReader"/> specified in the constructor.</value>
        public JsonReader Reader
        {
            get { return _reader; }
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
            return lineInfo != null && lineInfo.HasLineInfo();
        }

        int IJsonLineInfo.LineNumber
        {
            get
            {
                IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
                return (lineInfo != null) ? lineInfo.LineNumber : 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                IJsonLineInfo lineInfo = _reader as IJsonLineInfo;
                return (lineInfo != null) ? lineInfo.LinePosition : 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaValidatingReader"/> class that
        /// validates the content returned from the given <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from while validating.</param>
        public JSchema4ValidatingReader(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            _reader = reader;
            _scopes = new List<Scope>();
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Int32}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Int32}"/>.</returns>
        public override int? ReadAsInt32()
        {
            int? i = _reader.ReadAsInt32();

            ValidateCurrentToken();
            return i;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Byte"/>[].
        /// </summary>
        /// <returns>
        /// A <see cref="Byte"/>[] or a null reference if the next JSON token is null.
        /// </returns>
        public override byte[] ReadAsBytes()
        {
            byte[] data = _reader.ReadAsBytes();

            ValidateCurrentToken();
            return data;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Decimal}"/>.</returns>
        public override decimal? ReadAsDecimal()
        {
            decimal? d = _reader.ReadAsDecimal();

            ValidateCurrentToken();
            return d;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="String"/>.
        /// </summary>
        /// <returns>A <see cref="String"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override string ReadAsString()
        {
            string s = _reader.ReadAsString();

            ValidateCurrentToken();
            return s;
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTime}"/>.
        /// </summary>
        /// <returns>A <see cref="String"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override DateTime? ReadAsDateTime()
        {
            DateTime? dateTime = _reader.ReadAsDateTime();

            ValidateCurrentToken();
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

            ValidateCurrentToken();
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
            if (!_reader.Read())
                return false;

            if (_reader.TokenType == JsonToken.Comment)
                return true;

            ValidateCurrentToken();
            return true;
        }

        private void ValidateCurrentToken()
        {
            if (_scopes.Count == 0)
            {
                _context = new ValidatorContext();
                _context.Validator = this;
                _context.Scopes = _scopes;

                SchemaScope.CreateTokenScope(_reader.TokenType, _schema, _context, null, _reader.Depth);
            }

            if (_context.TokenWriter != null)
                _context.TokenWriter.WriteToken(_reader, false);

            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = _scopes[i];

                scope.EvaluateToken(_reader.TokenType, _reader.Value, _reader.Depth);
            }

            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = _scopes[i];

                if (scope.Complete)
                    _scopes.RemoveAt(i);
            }

            if (_context.TokenWriter != null && _context.TokenWriter.Top == 0)
                _context.TokenWriter = null;
        }

        private void OnValidationEvent(JSchemaException exception)
        {
            SchemaValidationEventHandler handler = ValidationEventHandler;
            if (handler != null)
                handler(this, new SchemaValidationEventArgs(exception));
            else
                throw exception;
        }

        internal ISchemaError CreateError(string message, JSchema4 schema, IList<ISchemaError> childErrors)
        {
            IJsonLineInfo lineInfo = this;

            string exceptionMessage = (lineInfo.HasLineInfo())
                ? message + " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition)
                : message;

            JSchemaException exception = new JSchemaException(exceptionMessage, null);
            exception.Path = Path;
            exception.LineNumber = lineInfo.LineNumber;
            exception.LinePosition = lineInfo.LinePosition;
            exception.Schema = schema;
            exception.ChildErrors = childErrors ?? new List<ISchemaError>();

            return exception;

        }

        void IValidator.RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors)
        {
            JSchemaException exception = (JSchemaException)CreateError(message, schema, childErrors);
            OnValidationEvent(exception);
        }

        ISchemaError IValidator.CreateError(string message, JSchema4 schema, IList<ISchemaError> childErrors)
        {
            return CreateError(message, schema, childErrors);
        }
    }
}