using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal abstract class ContextBase
    {
        public IValidator Validator;
        public List<Scope> Scopes;
        public JTokenWriter TokenWriter;
        public abstract void RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors);
        public abstract bool HasErrors { get; }
    }
}