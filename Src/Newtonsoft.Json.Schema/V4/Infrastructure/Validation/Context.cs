using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class Context
    {
        public Action<string, JSchema4> RaiseErrorEvent;
        public List<Scope> Scopes;
        public JTokenWriter TokenWriter;

        public void RaiseError(string message, JSchema4 schema)
        {
            //_isValid = false;
            RaiseErrorEvent(message, schema);
        }
    }
}