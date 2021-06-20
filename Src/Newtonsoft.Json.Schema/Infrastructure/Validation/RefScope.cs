#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class RefScope : ConditionalScope
    {
        private bool _isReentrant;

        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            // If there is a recursive chain of $ref schemas then this will stack overflow.
            // If the ref scope is already evaluating then exit without checking children.
            if (_isReentrant)
            {
                return true;
            }

            _isReentrant = true;
            try
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

                        IFormattable message = $"JSON does not match schema from '$ref'.";
                        RaiseError(message, ErrorType.Ref, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                    }
                    else
                    {
                        ConditionalContext.TrackEvaluatedSchema(ChildScopes[0].Schema);
                    }
                }
                else
                {
                    RaiseCircularDependencyError(ErrorType.Ref);
                }

                return true;
            }
            finally
            {
                _isReentrant = false;
            }
        }
    }
}