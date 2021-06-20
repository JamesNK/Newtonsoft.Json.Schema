#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class AllOfScope : ConditionalScope
    {
        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            if (TryGetChildrenAllValid(token, value, depth, out bool allValid))
            {
                if (!allValid)
                {
                    List<int> invalidIndexes = new List<int>();
                    int index = 0;
                    foreach (SchemaScope schemaScope in ChildScopes)
                    {
                        if (!schemaScope.IsValid)
                        {
                            invalidIndexes.Add(index);
                        }
                        else
                        {
                            ConditionalContext.TrackEvaluatedSchema(schemaScope.Schema);
                        }

                        index++;
                    }

                    IFormattable message = $"JSON does not match all schemas from 'allOf'. Invalid schema indexes: {StringHelpers.Join(", ", invalidIndexes)}.";
                    RaiseError(message, ErrorType.AllOf, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                }
                else
                {
                    for (int i = 0; i < ChildScopes.Count; i++)
                    {
                        ConditionalContext.TrackEvaluatedSchema(ChildScopes[i].Schema);
                    }
                }
            }
            else
            {
                RaiseCircularDependencyError(ErrorType.AllOf);
            }
            
            return true;
        }
    }
}