#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class OneOfScope : ConditionalScope
    {
        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            int validCount = GetChildrenValidCount(token, value, depth);

            if (validCount != 1)
            {
                List<int> validIndexes = new List<int>();
                int index = 0;
                foreach (SchemaScope schemaScope in ChildScopes)
                {
                    if (schemaScope.IsValid)
                    {
                        validIndexes.Add(index);
                    }

                    index++;
                }

                IFormattable message;
                if (validIndexes.Count > 0)
                {
                    message = $"JSON is valid against more than one schema from 'oneOf'. Valid schema indexes: {StringHelpers.Join(", ", validIndexes)}.";
                }
                else
                {
                    message = $"JSON is valid against no schemas from 'oneOf'.";
                }

                RaiseError(message, ErrorType.OneOf, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
            }
            else
            {
                // TODO: A little inefficent to find the valid child again
                foreach (SchemaScope childScope in ChildScopes)
                {
                    if (childScope.IsValid)
                    {
                        ConditionalContext.TrackEvaluatedSchema(childScope.Schema);
                    }
                }
            }

            return true;
        }
    }
}