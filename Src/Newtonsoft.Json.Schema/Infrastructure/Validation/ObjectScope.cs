#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal class ObjectScope : SchemaScope
    {
        private int _propertyCount;
        private string _currentPropertyName;
        private readonly List<string> _requiredProperties;
        private readonly List<string> _readProperties;
        private readonly Dictionary<string, SchemaScope> _dependencyScopes;

        public ObjectScope(ContextBase context, Scope parent, int initialDepth, JSchema schema)
            : base(context, parent, initialDepth, schema)
        {
            if (schema._required != null)
                _requiredProperties = schema._required.ToList();

            if (schema._dependencies != null && schema._dependencies.Count > 0)
            {
                _readProperties = new List<string>();

                if (schema._dependencies.Values.OfType<JSchema>().Any())
                    _dependencyScopes = new Dictionary<string, SchemaScope>(StringComparer.Ordinal);
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
                        TestType(Schema, JSchemaType.Object, null);
                        return false;
                    case JsonToken.EndObject:
                        if (_requiredProperties != null && _requiredProperties.Count > 0)
                            RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, StringHelpers.Join(", ", _requiredProperties)), ErrorType.Required, Schema, _requiredProperties, null);

                        if (Schema.MaximumProperties != null && _propertyCount > Schema.MaximumProperties)
                            RaiseError("Object property count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, _propertyCount, Schema.MaximumProperties), ErrorType.MaximumProperties, Schema, _propertyCount, null);

                        if (Schema.MinimumProperties != null && _propertyCount < Schema.MinimumProperties)
                            RaiseError("Object property count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, _propertyCount, Schema.MinimumProperties), ErrorType.MinimumProperties, Schema, _propertyCount, null);

                        if (_readProperties != null)
                        {
                            foreach (string readProperty in _readProperties)
                            {
                                object dependency;
                                if (Schema._dependencies.TryGetValue(readProperty, out dependency))
                                {
                                    List<string> requiredProperties = dependency as List<string>;
                                    if (requiredProperties != null)
                                    {
                                        if (!requiredProperties.All(r => _readProperties.Contains(r)))
                                        {
                                            List<string> missingRequiredProperties = requiredProperties.Where(r => !_readProperties.Contains(r)).ToList();
                                            string message = "Dependencies for property '{0}' failed. Missing required keys: {1}.".FormatWith(CultureInfo.InvariantCulture, readProperty, StringHelpers.Join(", ", missingRequiredProperties));

                                            RaiseError(message, ErrorType.Dependencies, Schema, readProperty, null);
                                        }
                                    }
                                    else
                                    {
                                        SchemaScope dependencyScope = _dependencyScopes[readProperty];
                                        if (dependencyScope.Context.HasErrors)
                                        {
                                            string message = "Dependencies for property '{0}' failed.".FormatWith(CultureInfo.InvariantCulture, readProperty);
                                            RaiseError(message, ErrorType.Dependencies, Schema, readProperty, ((ConditionalContext)dependencyScope.Context).Errors);
                                        }
                                    }
                                }
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (PatternSchema patternSchema in Schema._patternProperties.GetPatternSchemas())
                            {
                                Regex regex;
                                string errorMessage;
                                if (!patternSchema.TryGetPatternRegex(out regex, out errorMessage))
                                {
                                    RaiseError("Could not test property names with regex pattern '{0}'. There was an error parsing the regex: {1}".FormatWith(CultureInfo.InvariantCulture, patternSchema.Pattern, errorMessage),
                                        ErrorType.Pattern,
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

                    if (_requiredProperties != null)
                        _requiredProperties.Remove(_currentPropertyName);
                    if (_readProperties != null)
                        _readProperties.Add(_currentPropertyName);

                    if (!Schema.AllowAdditionalProperties)
                    {
                        if (!IsPropertyDefinied(Schema, _currentPropertyName))
                        {
                            string message = "Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, _currentPropertyName);
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
                            JSchema propertySchema;
                            if (Schema._properties.TryGetValue(_currentPropertyName, out propertySchema))
                            {
                                CreateScopesAndEvaluateToken(token, value, depth, propertySchema);
                                matched = true;
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (PatternSchema patternProperty in Schema._patternProperties.GetPatternSchemas())
                            {
                                Regex regex;
                                string errorMessage;
                                if (patternProperty.TryGetPatternRegex(out regex, out errorMessage))
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
                return true;

            if (schema._patternProperties != null)
            {
                foreach (PatternSchema patternSchema in schema._patternProperties.GetPatternSchemas())
                {
                    Regex regex;
                    string errorMessage;
                    if (patternSchema.TryGetPatternRegex(out regex, out errorMessage))
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
            if (_dependencyScopes != null)
            {
                foreach (KeyValuePair<string, object> dependency in Schema._dependencies)
                {
                    JSchema dependencySchema = dependency.Value as JSchema;
                    if (dependencySchema != null)
                    {
                        SchemaScope scope = CreateTokenScope(token, dependencySchema, ConditionalContext.Create(Context), null, InitialDepth);
                        _dependencyScopes.Add(dependency.Key, scope);
                    }
                }
            }
        }
    }
}