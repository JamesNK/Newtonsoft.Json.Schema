using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class ConditionalContext : ContextBase
    {
        public List<ISchemaError> Errors;

        public override void RaiseError(string message, JSchema4 schema, IList<ISchemaError> childErrors)
        {
            if (Errors == null)
                Errors = new List<ISchemaError>();

            Errors.Add(Validator.CreateError(message, schema, childErrors));
        }

        public static ConditionalContext Create(ContextBase context)
        {
            return new ConditionalContext
            {
                Validator = context.Validator,
                Scopes = context.Scopes,
                TokenWriter = context.TokenWriter
            };
        }

        public override bool HasErrors
        {
            get { return (Errors != null && Errors.Count > 0); }
        }
    }
}