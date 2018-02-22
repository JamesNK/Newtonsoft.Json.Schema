#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ArrayScope : SchemaScope
    {
        private int _index;
        private bool _matchContains;
        private List<JToken> _uniqueArrayItems;
        private IList<ConditionalContext> _containsContexts;

        public void Initialize(ContextBase context, SchemaScope parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Array);
            InitializeSchema(schema);

            _index = -1;
            _matchContains = false;

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

                        if (Schema.Contains != null && !_matchContains)
                        {
                            List<ValidationError> containsErrors = new List<ValidationError>();
                            foreach (ConditionalContext containsContext in _containsContexts)
                            {
                                containsErrors.AddRange(containsContext.Errors);
                            }

                            RaiseError($"No items match contains.", ErrorType.Contains, Schema, null, containsErrors);
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

                    if (Schema.ItemsPositionValidation)
                    {
                        JSchema itemSchema = Schema._items?.ElementAtOrDefault(_index);

                        if (itemSchema != null)
                        {
                            CreateScopesAndEvaluateToken(token, value, depth, itemSchema);
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
                            }
                        }
                    }
                    else
                    {
                        if (!Schema._items.IsNullOrEmpty())
                        {
                            CreateScopesAndEvaluateToken(token, value, depth, Schema._items[0]);
                        }
                    }

                    // no longer need to check contains schema after match
                    if (Schema.Contains != null && !_matchContains)
                    {
                        ConditionalContext containsContext = ConditionalContext.Create(Context);
                        _containsContexts.Add(containsContext);

                        // contains scope should not have the current scope the parent
                        // do not want contain failures setting the current scope's IsValid
                        CreateScopesAndEvaluateToken(token, value, depth, Schema.Contains, null, containsContext);
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

                    if (Schema.Contains != null && !_matchContains)
                    {
                        ConditionalContext currentContainsContext = _containsContexts[_containsContexts.Count - 1];
                        if (!currentContainsContext.HasErrors)
                        {
                            _matchContains = true;

                            // no longer need previous errors after match
                            _containsContexts.Clear();
                        }
                    }
                }
            }

            return false;
        }
    }
}