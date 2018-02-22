#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// The exception thrown when an error occurs in Json.NET Schema.
    /// </summary>
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
    [Serializable]
#endif
    public class JSchemaReaderException : JSchemaException
    {
        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber { get; }

        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition { get; }

        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        public string Path { get; }

        /// <summary>
        /// Gets the base URI of the schema document, or <c>null</c> if not available.
        /// </summary>
        /// <value>The base URI of the schema document, or <c>null</c> if not available.</value>
        public Uri BaseUri { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaReaderException"/> class.
        /// </summary>
        public JSchemaReaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaReaderException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JSchemaReaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaReaderException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public JSchemaReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaReaderException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public JSchemaReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        internal JSchemaReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition, Uri baseUri)
            : base(message, innerException)
        {
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
            BaseUri = baseUri;
        }

        internal static JSchemaReaderException Create(JsonReader reader, Uri baseUri, string message)
        {
            return Create(reader, baseUri, message, null);
        }

        internal static JSchemaReaderException Create(JsonReader reader, Uri baseUri, string message, Exception ex)
        {
            return Create(reader as IJsonLineInfo, baseUri, reader.Path, message, ex);
        }

        internal static JSchemaReaderException Create(IJsonLineInfo lineInfo, Uri baseUri, string message)
        {
            return Create(lineInfo, null, message, null);
        }

        internal static JSchemaReaderException Create(IJsonLineInfo lineInfo, Uri baseUri, string path, string message)
        {
            return Create(lineInfo, baseUri, path, message, null);
        }

        internal static JSchemaReaderException Create(IJsonLineInfo lineInfo, Uri baseUri, string path, string message, Exception ex)
        {
            message = FormatMessage(lineInfo, path, message);

            int lineNumber;
            int linePosition;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
            }
            else
            {
                lineNumber = 0;
                linePosition = 0;
            }

            return new JSchemaReaderException(message, ex, path, lineNumber, linePosition, baseUri);
        }
    }
}