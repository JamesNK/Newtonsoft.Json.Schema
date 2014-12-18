using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class ValidatorContext : ContextBase
    {
        private bool _hasErrors;

        public override void RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors)
        {
            _hasErrors = true;
            Validator.RaiseError(message, schema, childErrors);
        }

        public override bool HasErrors
        {
            get { return _hasErrors; }
        }
    }
}