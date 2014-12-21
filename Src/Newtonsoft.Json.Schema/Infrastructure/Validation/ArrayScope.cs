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
        private int _index = -1;
        private readonly List<JToken> _uniqueArrayItems;

        public ArrayScope(ContextBase context, Scope scope, int initialDepth, JSchema schema)
            : base(context, scope, initialDepth, schema)
        {
            if (schema.UniqueItems)
                _uniqueArrayItems = new List<JToken>();
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
                        RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, Schema.Type, "Constructor"), ErrorType.Type, Schema, null);
                        return false;
                    case JsonToken.EndArray:
                    case JsonToken.EndConstructor:
                        int itemCount = _index + 1;

                        if (Schema.MaximumItems != null && itemCount > Schema.MaximumItems)
                            RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, itemCount, Schema.MaximumItems), ErrorType.MaximumItems, Schema, null);

                        if (Schema.MinimumItems != null && itemCount < Schema.MinimumItems)
                            RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, itemCount, Schema.MinimumItems), ErrorType.MinimumItems, Schema, null);

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

                    if (Schema.UniqueItems)
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
                                RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, _index + 1), ErrorType.AdditionalItems, Schema, null);
                            else if (Schema.AdditionalItems != null)
                                CreateScopesAndEvaluateToken(token, value, depth, Schema.AdditionalItems);
                        }
                    }
                    else
                    {
                        if (Schema._items != null && Schema._items.Count > 0)
                            CreateScopesAndEvaluateToken(token, value, depth, Schema._items[0]);
                    }
                }

                if (JsonTokenHelpers.IsPrimitiveOrEndToken(token))
                {
                    if (Schema.UniqueItems)
                    {
                        if (_uniqueArrayItems.Contains(Context.TokenWriter.CurrentToken, JToken.EqualityComparer))
                            RaiseError("Non-unique array item at index {0}.".FormatWith(CultureInfo.InvariantCulture, _index), ErrorType.UniqueItems, Schema, null);
                        else
                            _uniqueArrayItems.Add(Context.TokenWriter.CurrentToken);
                    }
                }
            }

            return false;
        }
    }
}