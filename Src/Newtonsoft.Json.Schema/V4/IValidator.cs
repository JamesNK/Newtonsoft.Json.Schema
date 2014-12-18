using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.V4
{
    internal interface IValidator
    {
        void RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors);
        ISchemaError CreateError(string message, JSchema4 schema, IList<ISchemaError> childErrors);
    }
}