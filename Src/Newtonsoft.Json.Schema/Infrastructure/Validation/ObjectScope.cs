#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema.Infrastructure.Collections;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ObjectScope : SchemaScope
    {
        private int _propertyCount;
        private string _currentPropertyName;
        private List<string> _requiredProperties;
        private List<string> _readProperties;
        private Dictionary<string, SchemaScope> _dependencyScopes;

        public void Initialize(ContextBase context, SchemaScope parent, int initialDepth, JSchema schema)
        {
            Initialize(context, parent, initialDepth, ScopeType.Object);
            InitializeSchema(schema);

            _propertyCount = 0;
            _currentPropertyName = null;

            if (!schema._required.IsNullOrEmpty())
            {
                if (_requiredProperties != null)
                {
                    _requiredProperties.Clear();
                }
                else
                {
                    _requiredProperties = new List<string>(schema._required.Count);
                }

                foreach (string required in schema._required)
                {
                    _requiredProperties.Add(required);
                }
            }

            if (!schema._dependencies.IsNullOrEmpty())
            {
                if (_readProperties != null)
                {
                    _readProperties.Clear();
                }
                else
                {
                    _readProperties = new List<string>();
                }

                if (schema._dependencies.HasSchemas)
                {
                    if (_dependencyScopes != null)
                    {
                        _dependencyScopes.Clear();
                    }
                    else
                    {
                        _dependencyScopes = new Dictionary<string, SchemaScope>(StringComparer.Ordinal);
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
                    case JsonToken.StartObject:
                        TestType(Schema, JSchemaType.Object);
                        return false;
                    case JsonToken.EndObject:
                        if (!Schema._required.IsNullOrEmpty() && _requiredProperties.Count > 0)
                        {
                            RaiseError($"Required properties are missing from object: {StringHelpers.Join(", ", _requiredProperties)}.", ErrorType.Required, Schema, _requiredProperties, null);
                        }

                        if (Schema.MaximumProperties != null && _propertyCount > Schema.MaximumProperties)
                        {
                            RaiseError($"Object property count {_propertyCount} exceeds maximum count of {Schema.MaximumProperties}.", ErrorType.MaximumProperties, Schema, _propertyCount, null);
                        }

                        if (Schema.MinimumProperties != null && _propertyCount < Schema.MinimumProperties)
                        {
                            RaiseError($"Object property count {_propertyCount} is less than minimum count of {Schema.MinimumProperties}.", ErrorType.MinimumProperties, Schema, _propertyCount, null);
                        }

                        if (!Schema._dependencies.IsNullOrEmpty())
                        {
                            foreach (string readProperty in _readProperties)
                            {
                                if (Schema._dependencies.TryGetValue(readProperty, out object dependency))
                                {
                                    if (dependency is List<string> requiredProperties)
                                    {
                                        if (!requiredProperties.All(r => _readProperties.Contains(r)))
                                        {
                                            IEnumerable<string> missingRequiredProperties = requiredProperties.Where(r => !_readProperties.Contains(r));
                                            IFormattable message = $"Dependencies for property '{readProperty}' failed. Missing required keys: {StringHelpers.Join(", ", missingRequiredProperties)}.";

                                            RaiseError(message, ErrorType.Dependencies, Schema, readProperty, null);
                                        }
                                    }
                                    else
                                    {
                                        SchemaScope dependencyScope = _dependencyScopes[readProperty];
                                        if (dependencyScope.Context.HasErrors)
                                        {
                                            IFormattable message = $"Dependencies for property '{readProperty}' failed.";
                                            RaiseError(message, ErrorType.Dependencies, Schema, readProperty, ((ConditionalContext)dependencyScope.Context).Errors);
                                        }
                                    }
                                }
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (PatternSchema patternSchema in Schema.GetPatternSchemas())
                            {
                                if (!patternSchema.TryGetPatternRegex(out Regex regex, out string errorMessage))
                                {
                                    RaiseError($"Could not test property names with regex pattern '{patternSchema.Pattern}'. There was an error parsing the regex: {errorMessage}",
                                        ErrorType.PatternProperties,
                                        Schema,
                                        patternSchema.Pattern,
                                        null);
                                }
                            }
                        }
                        return true;
                    default:
                        throw new InvalidOperationException("Unexpected token when evaluating object: " + token);
                }
            }

            if (relativeDepth == 1)
            {
                if (token == JsonToken.PropertyName)
                {
                    _propertyCount++;
                    _currentPropertyName = (string)value;

                    if (!Schema._required.IsNullOrEmpty())
                    {
                        _requiredProperties.Remove(_currentPropertyName);
                    }
                    if (!Schema._dependencies.IsNullOrEmpty())
                    {
                        _readProperties.Add(_currentPropertyName);
                    }

                    if (!Schema.AllowAdditionalProperties)
                    {
                        if (!IsPropertyDefinied(Schema, _currentPropertyName))
                        {
                            IFormattable message = $"Property '{_currentPropertyName}' has not been defined and the schema does not allow additional properties.";
                            RaiseError(message, ErrorType.AdditionalProperties, Schema, _currentPropertyName, null);
                        }
                    }
                }
                else
                {
                    if (JsonTokenHelpers.IsPrimitiveOrStartToken(token))
                    {
                        bool matched = false;
                        if (Schema._properties != null)
                        {
                            if (Schema._properties.TryGetValue(_currentPropertyName, out JSchema propertySchema))
                            {
                                CreateScopesAndEvaluateToken(token, value, depth, propertySchema);
                                matched = true;
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (PatternSchema patternProperty in Schema.GetPatternSchemas())
                            {
                                if (patternProperty.TryGetPatternRegex(out Regex regex, out string errorMessage))
                                {
                                    if (regex.IsMatch(_currentPropertyName))
                                    {
                                        CreateScopesAndEvaluateToken(token, value, depth, patternProperty.Schema);
                                        matched = true;
                                    }
                                }
                            }
                        }

                        if (!matched)
                        {
                            if (Schema.AllowAdditionalProperties && Schema.AdditionalProperties != null)
                            {
                                CreateScopesAndEvaluateToken(token, value, depth, Schema.AdditionalProperties);
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool IsPropertyDefinied(JSchema schema, string propertyName)
        {
            if (schema._properties != null && schema._properties.ContainsKey(propertyName))
            {
                return true;
            }

            if (schema._patternProperties != null)
            {
                foreach (PatternSchema patternSchema in schema.GetPatternSchemas())
                {
                    if (patternSchema.TryGetPatternRegex(out Regex regex, out string errorMessage))
                    {
                        if (regex.IsMatch(_currentPropertyName))
                        {
                            if (Regex.IsMatch(propertyName, patternSchema.Pattern))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void InitializeScopes(JsonToken token)
        {
            if (!Schema._dependencies.IsNullOrEmpty())
            {
                foreach (KeyValuePair<string, object> dependency in Schema._dependencies.GetInnerDictionary())
                {
                    if (dependency.Value is JSchema dependencySchema)
                    {
                        SchemaScope scope = CreateTokenScope(token, dependencySchema, ConditionalContext.Create(Context), null, InitialDepth);
                        _dependencyScopes.Add(dependency.Key, scope);
                    }
                }
            }
        }
    }
}