#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class NotScope : ConditionalScope
    {
        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            // Special case schema references itself, e.g. { "not": { "ref": "#" } }, and all children are valid. 
            // Force this kind of schema to always be invalid.
            if (GetChildrenAnyValid(token, value, depth) ||
                (ChildScopes.Count == 1 && ChildScopes[0] == ParentSchemaScope))
            {
                RaiseError($"JSON is valid against schema from 'not'.", ErrorType.Not, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
            }

            // Note: Schema in not can never be used to indicate evaluated properties/items

            return true;
        }
    }
}