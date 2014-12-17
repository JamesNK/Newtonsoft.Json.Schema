using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class ArrayScope : SchemaScope
    {
        private int _index = -1;
        private readonly List<JToken> _uniqueArrayItems;

        public ArrayScope(Context context, Scope scope, int initialDepth, JSchema4 schema, bool raiseErrors)
            : base(context, scope, initialDepth, schema, raiseErrors)
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
                    case JsonToken.EndArray:
                        int itemCount = _index + 1;

                        if (Schema.MaximumItems != null && itemCount > Schema.MaximumItems)
                            RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, itemCount, Schema.MaximumItems), Schema);

                        if (Schema.MinimumItems != null && itemCount < Schema.MinimumItems)
                            RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, itemCount, Schema.MinimumItems), Schema);

                        return true;
                    default:
                        throw new Exception("Unexpected token.");
                }
            }

            if (relativeDepth == 1)
            {
                if (JsonReader.IsPrimitiveToken(token) || JsonReader.IsStartToken(token))
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
                        JSchema4 itemSchema = (Schema._items != null)
                            ? Schema._items.ElementAtOrDefault(_index)
                            : null;

                        if (itemSchema != null)
                        {
                            CreateScopesAndEvaluateToken(token, value, depth, itemSchema);
                        }
                        else
                        {
                            if (!Schema.AllowAdditionalItems)
                                RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, _index + 1), Schema);
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

                if (JsonReader.IsPrimitiveToken(token) || JsonWriter.IsEndToken(token))
                {
                    if (Schema.UniqueItems)
                    {
                        if (_uniqueArrayItems.Contains(Context.TokenWriter.CurrentToken, JToken.EqualityComparer))
                            RaiseError("Non-unique array item at index {0}.".FormatWith(CultureInfo.InvariantCulture, _index), Schema);
                        else
                            _uniqueArrayItems.Add(Context.TokenWriter.CurrentToken);
                    }
                }
            }

            return false;
        }
    }
}