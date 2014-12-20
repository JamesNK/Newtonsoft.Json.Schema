using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ValidatorContext : ContextBase
    {
        private bool _hasErrors;

        public ValidatorContext(Validator validator)
            : base(validator)
        {
        }

        public override void RaiseError(string message, JSchema schema, IList<ISchemaError> childErrors)
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