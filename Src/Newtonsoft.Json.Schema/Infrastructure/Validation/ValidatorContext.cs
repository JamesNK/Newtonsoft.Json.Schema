#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
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

        public override void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            _hasErrors = true;
            Validator.RaiseError(message, errorType, schema, value, childErrors);
        }

        public override bool HasErrors => _hasErrors;
    }
}