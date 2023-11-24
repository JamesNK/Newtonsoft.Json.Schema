#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class AnyOfScope : ConditionalScope
    {
        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            if (TryGetChildrenAnyValid(token, value, depth, out bool anyValid))
            {
                if (!anyValid)
                {
                    RaiseError($"JSON does not match any schemas from 'anyOf'.", ErrorType.AnyOf, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                }

                // TODO: A little inefficient to find the valid children again
                foreach (SchemaScope childScope in ChildScopes)
                {
                    if (childScope.IsValid)
                    {
                        ConditionalContext.TrackEvaluatedSchemaScope(childScope);
                    }
                }
            }
            else
            {
                RaiseCircularDependencyError(ErrorType.AnyOf);
            }

            return true;
        }
    }
}