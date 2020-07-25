#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class ArrayScope : SchemaScope
    {
        private int _index;
        private int _matchCount;
        private List<JToken> _uniqueArrayItems;
        private List<ConditionalContext> _containsContexts;
        private Dictionary<int, UnevaluatedContext> _unevaluatedScopes;

        public void Initialize(ContextBase context, SchemaScope parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Array);
            InitializeSchema(schema);

            _index = -1;
            _matchCount = 0;

            if (schema.Contains != null)
            {
                if (_containsContexts != null)
                {
                    _containsContexts.Clear();
                }
                else
                {
                    _containsContexts = new List<ConditionalContext>();
                }
            }

            if (schema.UniqueItems)
            {
                if (_uniqueArrayItems != null)
                {
                    _uniqueArrayItems.Clear();
                }
                else
                {
                    _uniqueArrayItems = new List<JToken>();
                }
            }

            if (ShouldValidateUnevaluated())
            {
                if (_unevaluatedScopes != null)
                {
                    _unevaluatedScopes.Clear();
                }
                else
                {
                    _unevaluatedScopes = new Dictionary<int, UnevaluatedContext>();
                }
            }
        }

        public override bool ShouldValidateUnevaluated()
        {
            // If additional items are validated then there are no unevaluated items
            if (Schema.HasAdditionalItems)
            {
                return false;
            }

            return !(Schema.AllowUnevaluatedItems ?? true) || Schema.UnevaluatedItems != null;
        }

        protected override void OnConditionalScopeValidated(ConditionalScope conditionalScope)
        {
            if (ShouldValidateUnevaluated())
            {
                if (!conditionalScope.EvaluatedSchemas.IsNullOrEmpty())
                {
                    foreach (KeyValuePair<int, UnevaluatedContext> unevaluatedScope in _unevaluatedScopes)
                    {
                        UnevaluatedContext context = unevaluatedScope.Value;

                        if (!context.Evaluated && !context.ValidScopes.IsNullOrEmpty())
                        {
                            foreach (JSchema validScopes in context.ValidScopes)
                            {
                                if (conditionalScope.EvaluatedSchemas.Contains(validScopes))
                                {
                                    context.Evaluated = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            int relativeDepth = depth - InitialDepth;

            if (relativeDepth == 0)
            {
                EnsureEnum(token, value);

                switch (token)
                {
                    case JsonToken.StartArray:
                        EnsureValid(value);
                        TestType(Schema, JSchemaType.Array);
                        return false;
                    case JsonToken.StartConstructor:
                        JSchemaType schemaType = Schema.Type.GetValueOrDefault(JSchemaType.None);

                        if (schemaType != JSchemaType.None)
                        {
                            RaiseError($"Invalid type. Expected {schemaType.GetDisplayText()} but got Constructor.", ErrorType.Type, Schema, value, null);
                        }
                        return false;
                    case JsonToken.EndArray:
                    case JsonToken.EndConstructor:
                        ValidateConditionalChildren(token, value, depth);

                        int itemCount = _index + 1;

                        if (Schema.MaximumItems != null && itemCount > Schema.MaximumItems)
                        {
                            RaiseError($"Array item count {itemCount} exceeds maximum count of {Schema.MaximumItems}.", ErrorType.MaximumItems, Schema, itemCount, null);
                        }

                        if (Schema.MinimumItems != null && itemCount < Schema.MinimumItems)
                        {
                            RaiseError($"Array item count {itemCount} is less than minimum count of {Schema.MinimumItems}.", ErrorType.MinimumItems, Schema, itemCount, null);
                        }

                        if (Schema.Contains != null)
                        {
                            // MinimumContains overrides default contains behavior
                            if (Schema.MinimumContains != null)
                            {
                                if (_matchCount < Schema.MinimumContains)
                                {
                                    RaiseError($"Contains match count {_matchCount} is less than minimum contains count of {Schema.MinimumContains}.", ErrorType.MinimumContains, Schema, null, GetValidationErrors(_containsContexts));
                                }
                            }
                            else
                            {
                                if (_matchCount == 0)
                                {
                                    RaiseError($"No items match contains.", ErrorType.Contains, Schema, null, GetValidationErrors(_containsContexts));
                                }
                            }

                            if (_matchCount > Schema.MaximumContains)
                            {
                                RaiseError($"Contains match count {_matchCount} exceeds maximum contains count of {Schema.MaximumContains}.", ErrorType.MaximumContains, Schema, null, GetValidationErrors(_containsContexts));
                            }
                        }

                        if (!_unevaluatedScopes.IsNullOrEmpty())
                        {
                            foreach (KeyValuePair<int, UnevaluatedContext> item in _unevaluatedScopes)
                            {
                                if (!item.Value.Evaluated)
                                {
                                    IFormattable message = $"Item at index {item.Key} has not been successfully evaluated and the schema does not allow unevaluated items.";
                                    RaiseError(message, ErrorType.UnevaluatedItems, Schema, item.Key, item.Value.SchemaScope.GetValidationErrors());
                                }
                            }
                        }

                        return true;
                    default:
                        throw new InvalidOperationException("Unexpected token when evaluating array: " + token);
                }
            }

            if (relativeDepth == 1)
            {
                if (JsonTokenHelpers.IsPrimitiveOrStartToken(token))
                {
                    _index++;

                    if (Schema.UniqueItems || !Schema._validators.IsNullOrEmpty())
                    {
                        if (Context.TokenWriter == null)
                        {
                            Context.TokenWriter = new JTokenWriter();
                            Context.TokenWriter.WriteToken(token, value);
                        }
                    }

                    bool matched = false;

                    if (Schema.ItemsPositionValidation)
                    {
                        // TODO: Remove LINQ
                        JSchema itemSchema = Schema._items?.ElementAtOrDefault(_index);

                        if (itemSchema != null)
                        {
                            CreateScopesAndEvaluateToken(token, value, depth, itemSchema);
                            matched = true;
                        }
                        else
                        {
                            if (!Schema.AllowAdditionalItems)
                            {
                                RaiseError($"Index {_index + 1} has not been defined and the schema does not allow additional items.", ErrorType.AdditionalItems, Schema, value, null);
                            }
                            else if (Schema.AdditionalItems != null)
                            {
                                CreateScopesAndEvaluateToken(token, value, depth, Schema.AdditionalItems);
                                matched = true;
                            }
                        }
                    }
                    else
                    {
                        if (!Schema._items.IsNullOrEmpty())
                        {
                            CreateScopesAndEvaluateToken(token, value, depth, Schema._items[0]);
                            matched = true;
                        }
                    }

                    if (ShouldEvaluateContains())
                    {
                        ConditionalContext containsContext = CreateConditionalContext();
                        _containsContexts.Add(containsContext);

                        // contains scope should not have the current scope the parent
                        // do not want contain failures setting the current scope's IsValid
                        CreateScopesAndEvaluateToken(token, value, depth, Schema.Contains, null, containsContext);
                    }

                    if (!matched)
                    {
                        if (ShouldValidateUnevaluated())
                        {
                            _unevaluatedScopes[_index] = Schema.UnevaluatedItems != null
                                ? new UnevaluatedContext(CreateScopesAndEvaluateToken(token, value, depth, Schema.UnevaluatedItems, this, CreateConditionalContext()))
                                : new UnevaluatedContext(AlwaysFalseScope.Instance);
                        }
                    }
                }

                if (JsonTokenHelpers.IsPrimitiveOrEndToken(token))
                {
                    if (Schema.UniqueItems)
                    {
                        JToken currentToken = Context.TokenWriter.CurrentToken;
                        bool isDuplicate = JsonTokenHelpers.Contains(_uniqueArrayItems, currentToken);
                        if (isDuplicate)
                        {
                            object v = (currentToken is JValue valueToken) ? valueToken.Value : currentToken;

                            RaiseError($"Non-unique array item at index {_index}.", ErrorType.UniqueItems, Schema, v, null);
                        }
                        else
                        {
                            _uniqueArrayItems.Add(Context.TokenWriter.CurrentToken);
                        }
                    }

                    if (ShouldEvaluateContains())
                    {
                        ConditionalContext currentContainsContext = _containsContexts[_containsContexts.Count - 1];
                        if (!currentContainsContext.HasErrors)
                        {
                            _matchCount++;
                        }
                    }

                    if (ShouldValidateUnevaluated() &&
                        _unevaluatedScopes.TryGetValue(_index, out UnevaluatedContext unevaluatedContext))
                    {
                        // Property is valid against unevaluatedItems schema so no need to search further
                        bool isValid = unevaluatedContext.SchemaScope.IsValid;
                        unevaluatedContext.Evaluated = isValid;

                        if (!isValid)
                        {
                            for (int i = Context.Scopes.Count - 1; i >= 0; i--)
                            {
                                Scope scope = Context.Scopes[i];
                                if (scope.InitialDepth == InitialDepth + 1)
                                {
                                    // Schema for a item
                                    if (scope.Parent != null && scope is SchemaScope schemaScope && schemaScope.IsValid)
                                    {
                                        unevaluatedContext.AddValidScope(schemaScope.Parent.Schema);
                                    }
                                }
                                else if (scope.InitialDepth == InitialDepth)
                                {
                                    // Schema for the current array.
                                    // Need to check these for oneOf, allOf, etc.
                                    if (scope is SchemaScope schemaScope)
                                    {
                                        if (schemaScope.Schema._allowAdditionalItems.GetValueOrDefault() ||
                                            schemaScope.Schema.AdditionalItems != null ||
                                            schemaScope.Schema.AllowUnevaluatedItems.GetValueOrDefault())
                                        {
                                            unevaluatedContext.AddValidScope(schemaScope.Schema);
                                        }
                                    }
                                }
                                else if (scope.InitialDepth < InitialDepth)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
            }

            return false;
        }

        private List<ValidationError> GetValidationErrors(IList<ConditionalContext> contexts)
        {
            List<ValidationError> containsErrors = new List<ValidationError>();
            foreach (ConditionalContext containsContext in contexts)
            {
                if (containsContext.Errors != null)
                {
                    containsErrors.AddRange(containsContext.Errors);
                }
            }

            return containsErrors;
        }

        private bool ShouldEvaluateContains()
        {
            if (Schema.Contains != null)
            {
                // Match count is less than minimum
                if (_matchCount < (Schema.MinimumContains ?? 1))
                {
                    return true;
                }

                // Always evaluate contains when there is a max so
                // the number of matches is accurate
                if (Schema.MaximumContains != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}