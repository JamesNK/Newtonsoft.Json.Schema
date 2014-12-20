using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    public interface ISchemaError
    {
        string Message { get; }
        int LineNumber { get; }
        int LinePosition { get; }
        string Path { get; }
        JSchema Schema { get; }
        ErrorType ErrorType { get; }
        IList<ISchemaError> ChildErrors { get; }
    }
}