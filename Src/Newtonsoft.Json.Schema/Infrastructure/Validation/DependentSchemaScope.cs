#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class DependentSchemaScope : ConditionalScope
    {
        public string? PropertyName;

        public override void Initialize(ContextBase context, SchemaScope? parent, int initialDepth, ScopeType type)
        {
            base.Initialize(context, parent, initialDepth, type);

            PropertyName = null;
        }

        protected override bool EvaluateTokenCore(JsonToken token, object? value, int depth)
        {
            if (Parent is ObjectScope objectScope)
            {
                var readProperties = objectScope.ReadProperties;
                ValidationUtils.Assert(readProperties != null);
                ValidationUtils.Assert(PropertyName != null);

                if (readProperties.Contains(PropertyName))
                {
                    if (TryGetChildrenAnyValid(token, value, depth, out bool anyValid))
                    {
                        if (!anyValid)
                        {
                            RaiseError($"Dependencies for property '{PropertyName}' failed.", ErrorType.Dependencies, ParentSchemaScope.Schema, null, ConditionalContext.Errors);
                        }
                        else
                        {
                            foreach (SchemaScope childScope in ChildScopes)
                            {
                                if (childScope.IsValid)
                                {
                                    ConditionalContext.TrackEvaluatedSchema(childScope.Schema);
                                }
                            }
                        }
                    }
                    else
                    {
                        RaiseCircularDependencyError(ErrorType.Dependencies);
                    }
                }
            }

            return true;
        }
    }
}