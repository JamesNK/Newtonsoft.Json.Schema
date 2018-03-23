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
        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            if (GetChildrenAnyValid(token, value, depth))
            {
                RaiseError($"JSON is valid against schema from 'not'.", ErrorType.Not, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
            }

            return true;
        }
    }
}