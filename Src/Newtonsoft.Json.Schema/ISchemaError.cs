using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Represents an error created when JSON Schema validation errors are encountered in a JSON document being validated.
    /// </summary>
    public interface ISchemaError
    {
        /// <summary>
        /// Gets the message describing the error that occurred.
        /// </summary>
        /// <value>The message describing the error that occurred.</value>
        string Message { get; }
        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        int LineNumber { get; }
        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        int LinePosition { get; }
        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        string Path { get; }
        /// <summary>
        /// Gets the <see cref="JSchema"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="JSchema"/> that generated the error.</value>
        JSchema Schema { get; }
        /// <summary>
        /// Gets the <see cref="ErrorType"/> that generated the error.
        /// </summary>
        /// <value>The <see cref="ErrorType"/> that generated the error.</value>
        ErrorType ErrorType { get; }
        /// <summary>
        /// Gets a collection of a <see cref="ISchemaError"/>'s child errors.
        /// </summary>
        /// <value>A collection of a <see cref="ISchemaError"/>'s child errors.</value>
        IList<ISchemaError> ChildErrors { get; }
    }
}