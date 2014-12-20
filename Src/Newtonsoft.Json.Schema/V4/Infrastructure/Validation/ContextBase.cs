using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal abstract class ContextBase
    {
        public Validator Validator;

        public List<Scope> Scopes
        {
            get { return Validator.Scopes; }
        }

        public JTokenWriter TokenWriter
        {
            get { return Validator.TokenWriter; }
            set { Validator.TokenWriter = value; }
        }

        protected ContextBase(Validator validator)
        {
            Validator = validator;
        }
        
        public abstract void RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors);
        public abstract bool HasErrors { get; }
    }
}