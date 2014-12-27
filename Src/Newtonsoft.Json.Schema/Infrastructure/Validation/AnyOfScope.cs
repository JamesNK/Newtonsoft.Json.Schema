#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class AnyOfScope : ConditionalScope
    {
        public AnyOfScope(SchemaScope parent, ContextBase context, int depth)
            : base(context, parent, depth)
        {
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && JsonTokenHelpers.IsPrimitiveOrEndToken(token))
            {
                if (!GetChildren().Any(IsValidPredicate))
                {
                    RaiseError("JSON does not match any schemas from 'anyOf'.", ErrorType.AnyOf, ParentSchemaScope.Schema, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}