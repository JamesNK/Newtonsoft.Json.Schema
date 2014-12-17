using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.V4.Infrastructure.Validation
{
    internal class ObjectScope : SchemaScope
    {
        private int _propertyCount;
        private string _currentPropertyName;
        private readonly List<string> _requiredProperties;
        
        public ObjectScope(Context context, Scope scope, int initialDepth, JSchema4 schema, bool raiseErrors)
            : base(context, scope, initialDepth, schema, raiseErrors)
        {
            if (schema._required != null)
                _requiredProperties = schema._required.ToList();
        }

        protected override bool EvaluateTokenCore(JsonToken token, object value, int depth)
        {
            int relativeDepth = depth - InitialDepth;

            if (relativeDepth == 0)
            {
                EnsureEnum(token, value);

                switch (token)
                {
                    case JsonToken.StartObject:
                        TestType(Schema, JSchemaType.Object);
                        return false;
                    case JsonToken.EndObject:
                        if (_requiredProperties != null && _requiredProperties.Count > 0)
                            RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", _requiredProperties)), Schema);

                        if (Schema.MaximumProperties != null && _propertyCount > Schema.MaximumProperties)
                            RaiseError("Object property count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, _propertyCount, Schema.MaximumProperties), Schema);

                        if (Schema.MinimumProperties != null && _propertyCount < Schema.MinimumProperties)
                            RaiseError("Object property count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, _propertyCount, Schema.MinimumProperties), Schema);

                        return true;
                    default:
                        throw new Exception("Unexpected token.");
                }
            }

            if (relativeDepth == 1)
            {
                if (token == JsonToken.PropertyName)
                {
                    _propertyCount++;
                    _currentPropertyName = (string)value;

                    if (_requiredProperties != null)
                        _requiredProperties.Remove(_currentPropertyName);

                    if (!Schema.AllowAdditionalProperties)
                    {
                        if (!IsPropertyDefinied(Schema, _currentPropertyName))
                            RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, _currentPropertyName), Schema);
                    }
                }
                else
                {
                    if (JsonReader.IsPrimitiveToken(token) || JsonReader.IsStartToken(token))
                    {
                        bool matched = false;
                        if (Schema._properties != null)
                        {
                            JSchema4 propertySchema;
                            if (Schema._properties.TryGetValue(_currentPropertyName, out propertySchema))
                            {
                                CreateScopesAndEvaluateToken(token, value, depth, propertySchema);
                                matched = true;
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (KeyValuePair<string, JSchema4> patternProperty in Schema._patternProperties)
                            {
                                if (Regex.IsMatch(_currentPropertyName, patternProperty.Key))
                                {
                                    CreateScopesAndEvaluateToken(token, value, depth, patternProperty.Value);
                                    matched = true;
                                }
                            }
                        }

                        if (!matched)
                        {
                            if (Schema.AllowAdditionalProperties && Schema.AdditionalProperties != null)
                                CreateScopesAndEvaluateToken(token, value, depth, Schema.AdditionalProperties);
                        }
                    }
                }
            }

            return false;
        }

        private bool IsPropertyDefinied(JSchema4 schema, string propertyName)
        {
            if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
                return true;

            if (schema.PatternProperties != null)
            {
                foreach (string pattern in schema.PatternProperties.Keys)
                {
                    if (Regex.IsMatch(propertyName, pattern))
                        return true;
                }
            }

            return false;
        }
    }
}