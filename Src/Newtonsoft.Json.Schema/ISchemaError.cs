using System.Collections.Generic;
using Newtonsoft.Json.Schema.V4;

namespace Newtonsoft.Json.Schema
{
    public interface ISchemaError
    {
        string Message { get; }
        int LineNumber { get; }
        int LinePosition { get; }
        string Path { get; }
        JSchema4 Schema { get; }
        ErrorType ErrorType { get; }
        IList<ISchemaError> ChildErrors { get; }
    }
}