#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class NotScope : ConditionalScope
    {
        public NotScope(SchemaScope parent, ContextBase context, int depth)
            : base(context, parent, depth)
        {
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (depth == InitialDepth && JsonTokenHelpers.IsPrimitiveOrEndToken(token))
            {
                if (GetChildren().Any(IsValidPredicate))
                {
                    RaiseError($"JSON is valid against schema from 'not'.", ErrorType.Not, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                }

                return true;
            }

            return false;
        }
    }
}