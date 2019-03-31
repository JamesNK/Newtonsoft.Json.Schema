#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// The exception thrown when an error occurs in Json.NET Schema.
    /// </summary>
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
    [Serializable]
#endif
    public class JSchemaException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class.
        /// </summary>
        public JSchemaException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JSchemaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public JSchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public JSchemaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            return FormatMessage(lineInfo, path, new StringBuilder(message));
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, StringBuilder message)
        {
            message.TrimEnd();

            if (message[message.Length - 1] != '.')
            {
                message.Append('.');
            }

            if (path == null && (lineInfo == null || !lineInfo.HasLineInfo()))
            {
                return message.ToString();
            }

            message.Append(' ');

            if (path != null)
            {
                message.Append("Path '");
                message.Append(path);
                message.Append('\'');

                if (lineInfo != null && lineInfo.HasLineInfo())
                {
                    message.Append(", line ");
                    message.Append(lineInfo.LineNumber);
                    message.Append(", position ");
                    message.Append(lineInfo.LinePosition);
                }
            }
            else
            {
                message.Append("Line ");
                message.Append(lineInfo.LineNumber);
                message.Append(", position ");
                message.Append(lineInfo.LinePosition);
            }

            message.Append('.');
            return message.ToString();
        }
    }
}