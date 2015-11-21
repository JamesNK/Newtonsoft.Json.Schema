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
        private List<JToken> _uniqueArrayItems;

        public void Initialize(ContextBase context, Scope parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Array);
            InitializeSchema(schema);

            _index = -1;

            if (schema.UniqueItems)
            {
                _uniqueArrayItems = new List<JToken>();
            }
            else
            {
                _uniqueArrayItems = null;
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
                        TestType(Schema, JSchemaType.Array);
                        return false;
                    case JsonToken.StartConstructor:
                        RaiseError($"Invalid type. Expected {Schema.Type.Value.GetDisplayText()} but got Constructor.", ErrorType.Type, Schema, value, null);
                        return false;
                    case JsonToken.EndArray:
                    case JsonToken.EndConstructor:
                        int itemCount = _index + 1;

                        if (Schema.MaximumItems != null && itemCount > Schema.MaximumItems)
                        {
                            RaiseError($"Array item count {itemCount} exceeds maximum count of {Schema.MaximumItems}.", ErrorType.MaximumItems, Schema, itemCount, null);
                        }

                        if (Schema.MinimumItems != null && itemCount < Schema.MinimumItems)
                        {
                            RaiseError($"Array item count {itemCount} is less than minimum count of {Schema.MinimumItems}.", ErrorType.MinimumItems, Schema, itemCount, null);
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
                        JSchema itemSchema = (Schema._items != null)
                            ? Schema._items.ElementAtOrDefault(_index)
                            : null;

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
                }

                if (JsonTokenHelpers.IsPrimitiveOrEndToken(token))
                {
                    if (Schema.UniqueItems)
                    {
                        JToken currentToken = Context.TokenWriter.CurrentToken;
                        bool isDuplicate = JsonTokenHelpers.Contains(_uniqueArrayItems, currentToken);
                        if (isDuplicate)
                        {
                            object v = (currentToken is JValue) ? ((JValue)currentToken).Value : currentToken;

                            RaiseError($"Non-unique array item at index {_index}.", ErrorType.UniqueItems, Schema, v, null);
                        }
                        else
                        {
                            _uniqueArrayItems.Add(Context.TokenWriter.CurrentToken);
                        }
                    }
                }
            }

            return false;
        }
    }
}