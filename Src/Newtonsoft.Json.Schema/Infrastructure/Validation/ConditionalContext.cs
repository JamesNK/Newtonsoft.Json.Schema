using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ConditionalContext : ContextBase
    {
        public List<ISchemaError> Errors;

        public ConditionalContext(Validator validator)
            : base(validator)
        {
        }

        public override void RaiseError(string message, ErrorType errorType, JSchema schema, IList<ISchemaError> childErrors)
        {
            if (Errors == null)
                Errors = new List<ISchemaError>();

            Errors.Add(Validator.CreateError(message, errorType, schema, childErrors));
        }

        public static ConditionalContext Create(ContextBase context)
        {
            return new ConditionalContext(context.Validator);
        }

        public override bool HasErrors
        {
            get { return (Errors != null && Errors.Count > 0); }
        }
    }
}