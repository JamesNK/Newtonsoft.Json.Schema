#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class ContextBase
    {
        public Validator Validator;

        public List<Scope> Scopes
        {
            [DebuggerStepThrough]
            get { return Validator.Scopes; }
        }

        public JTokenWriter TokenWriter
        {
            get => Validator.TokenWriter;
            set => Validator.TokenWriter = value;
        }

        protected ContextBase(Validator validator)
        {
            Validator = validator;
        }

        public abstract void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors);
        public abstract bool HasErrors { get; }
    }
}