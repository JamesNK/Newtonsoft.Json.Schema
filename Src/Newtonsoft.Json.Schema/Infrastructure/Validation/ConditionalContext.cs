#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ConditionalContext : ContextBase
    {
        public List<ValidationError> Errors;

        public ConditionalContext(Validator validator)
            : base(validator)
        {
        }

        public override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            if (Errors == null)
            {
                Errors = new List<ValidationError>();
            }

            Errors.Add(Validator.CreateError(message, errorType, schema, value, childErrors));
        }

        public static ConditionalContext Create(ContextBase context)
        {
            return new ConditionalContext(context.Validator);
        }

        public override bool HasErrors => !Errors.IsNullOrEmpty();
    }
}