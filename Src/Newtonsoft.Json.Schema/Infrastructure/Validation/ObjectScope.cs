#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema.Infrastructure.Collections;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal sealed class ObjectScope : SchemaScope
    {
        private int _propertyCount;
        private string _currentPropertyName;
        private List<string> _requiredProperties;
        private List<string> _readProperties;
        private Dictionary<string, SchemaScope> _dependencyScopes;
        private Dictionary<string, UnevaluatedContext> _unevaluatedScopes;

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

            // Need to track the properties read when:
            //  - Schema has dependencies or,
            //  - Need to validate unevaluated properties
            if (HasDependencies())
            {
                if (_readProperties != null)
                {
                    _readProperties.Clear();
                }
                else
                {
                    _readProperties = new List<string>();
                }

                if ((schema._dependencies != null && schema._dependencies.HasSchemas)
                    || !schema._dependentSchemas.IsNullOrEmpty())
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

            if (ShouldValidateUnevaluated())
            {
                if (_unevaluatedScopes != null)
                {
                    _unevaluatedScopes.Clear();
                }
                else
                {
                    _unevaluatedScopes = new Dictionary<string, UnevaluatedContext>(StringComparer.Ordinal);
                }
            }
        }

        public override bool ShouldValidateUnevaluated()
        {
            // If additional items are validated then there are no unevaluated properties
            if (Schema.HasAdditionalProperties)
            {
                return false;
            }

            return !(Schema.AllowUnevaluatedProperties ?? true) || Schema.UnevaluatedProperties != null;
        }

        protected override void OnConditionalScopeValidated(ConditionalScope conditionalScope)
        {
            if (ShouldValidateUnevaluated())
            {
                if (!conditionalScope.EvaluatedSchemas.IsNullOrEmpty())
                {
                    foreach (KeyValuePair<string, UnevaluatedContext> unevaluatedScope in _unevaluatedScopes)
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
                    case JsonToken.StartObject:
                        EnsureValid(value);
                        TestType(Schema, JSchemaType.Object);
                        return false;
                    case JsonToken.EndObject:
                        ValidateConditionalChildren(token, value, depth);

                        if (!Schema._required.IsNullOrEmpty() && _requiredProperties.Count > 0)
                        {
                            //capture required properties at current depth as we may clear _requiredProperties later on
                            List<string> capturedRequiredProperties = _requiredProperties.ToList();
                            RaiseError($"Required properties are missing from object: {StringHelpers.Join(", ", capturedRequiredProperties)}.", ErrorType.Required, Schema, capturedRequiredProperties, null);
                        }

                        if (Schema.MaximumProperties != null && _propertyCount > Schema.MaximumProperties)
                        {
                            RaiseError($"Object property count {_propertyCount} exceeds maximum count of {Schema.MaximumProperties}.", ErrorType.MaximumProperties, Schema, _propertyCount, null);
                        }

                        if (Schema.MinimumProperties != null && _propertyCount < Schema.MinimumProperties)
                        {
                            RaiseError($"Object property count {_propertyCount} is less than minimum count of {Schema.MinimumProperties}.", ErrorType.MinimumProperties, Schema, _propertyCount, null);
                        }

                        if (HasDependencies())
                        {
                            foreach (string readProperty in _readProperties)
                            {
                                object dependency = null;
                                IList<string> requiredProperties = null;
                                if (Schema._dependencies?.TryGetValue(readProperty, out dependency) ?? false)
                                {
                                    requiredProperties = dependency as IList<string>;
                                    if (requiredProperties != null)
                                    {
                                        ValidateDependantProperties(readProperty, requiredProperties);
                                    }
                                    else
                                    {
                                        ValidateDependantSchema(readProperty);
                                    }
                                }

                                if (Schema._dependentRequired?.TryGetValue(readProperty, out requiredProperties) ?? false)
                                {
                                    ValidateDependantProperties(readProperty, requiredProperties);
                                }

                                if (Schema._dependentSchemas?.TryGetValue(readProperty, out _) ?? false)
                                {
                                    ValidateDependantSchema(readProperty);
                                }
                            }
                        }

                        // Evaluate after dependency schemas have been validated
                        if (!_unevaluatedScopes.IsNullOrEmpty())
                        {
                            foreach (KeyValuePair<string, UnevaluatedContext> item in _unevaluatedScopes)
                            {
                                if (!item.Value.Evaluated)
                                {
                                    IFormattable message = $"Property '{item.Key}' has not been successfully evaluated and the schema does not allow unevaluated properties.";
                                    RaiseError(message, ErrorType.UnevaluatedProperties, Schema, item.Key, item.Value.SchemaScope.GetValidationErrors());
                                }
                            }
                        }

                        if (Schema._patternProperties != null)
                        {
                            foreach (PatternSchema patternSchema in Schema.GetPatternSchemas())
                            {
                                if (!patternSchema.TryGetPatternRegex(
#if !(NET35 || NET40)
                                    Context.Validator.RegexMatchTimeout,
#endif
                                    out Regex _,
                                    out string errorMessage))
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
                    if (HasDependencies())
                    {
                        _readProperties.Add(_currentPropertyName);
                    }

                    if (Schema._propertyNames != null)
                    {
                        CreateScopesAndEvaluateToken(token, value, depth, Schema._propertyNames);
                    }

                    if (!Schema.AllowAdditionalProperties)
                    {
                        if (!IsPropertyDefined(Schema, _currentPropertyName))
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
                                if (patternProperty.TryGetPatternRegex(
#if !(NET35 || NET40)
                                    Context.Validator.RegexMatchTimeout,
#endif
                                    out Regex regex,
                                    out string _))
                                {
                                    if (RegexHelpers.IsMatch(regex, patternProperty.Pattern, _currentPropertyName))
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

                            if (ShouldValidateUnevaluated())
                            {
                                _unevaluatedScopes[_currentPropertyName] = Schema.UnevaluatedProperties != null
                                    ? new UnevaluatedContext(CreateScopesAndEvaluateToken(token, value, depth, Schema.UnevaluatedProperties, this, CreateConditionalContext()))
                                    : new UnevaluatedContext(AlwaysFalseScope.Instance);
                            }
                        }
                    }

                    if (JsonTokenHelpers.IsPrimitiveOrEndToken(token))
                    {
                        if (ShouldValidateUnevaluated() &&
                            _unevaluatedScopes.TryGetValue(_currentPropertyName, out UnevaluatedContext unevaluatedContext))
                        {
                            // Property is valid against unevaluatedProperties schema so no need to search further
                            bool isValid = unevaluatedContext.SchemaScope.IsValid;
                            unevaluatedContext.Evaluated = isValid;

                            if (!isValid)
                            {
                                for (int i = Context.Scopes.Count - 1; i >= 0; i--)
                                {
                                    Scope scope = Context.Scopes[i];
                                    if (scope.InitialDepth == InitialDepth + 1)
                                    {
                                        // Schema for a property
                                        if (scope.Parent != null && scope is SchemaScope schemaScope && schemaScope.IsValid)
                                        {
                                            unevaluatedContext.AddValidScope(schemaScope.Parent.Schema);
                                        }
                                    }
                                    else if (scope.InitialDepth == InitialDepth)
                                    {
                                        // Schema for the current object.
                                        // Need to check these for oneOf, allOf, etc.
                                        if (scope is SchemaScope schemaScope)
                                        {
                                            if (schemaScope.Schema._allowAdditionalProperties.GetValueOrDefault() ||
                                                schemaScope.Schema.AdditionalProperties != null ||
                                                schemaScope.Schema.AllowUnevaluatedProperties.GetValueOrDefault())
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
            }

            return false;
        }

        private bool HasDependencies()
        {
            return !Schema._dependencies.IsNullOrEmpty()
                || !Schema._dependentRequired.IsNullOrEmpty()
                || !Schema._dependentSchemas.IsNullOrEmpty();
        }

        private void ValidateDependantSchema(string readProperty)
        {
            SchemaScope dependencyScope = _dependencyScopes[readProperty];
            if (dependencyScope.Context.HasErrors)
            {
                IFormattable message = $"Dependencies for property '{readProperty}' failed.";
                RaiseError(message, ErrorType.Dependencies, Schema, readProperty, dependencyScope.GetValidationErrors());
            }
            else
            {
                if (!_unevaluatedScopes.IsNullOrEmpty())
                {
                    foreach (KeyValuePair<string, UnevaluatedContext> item in _unevaluatedScopes)
                    {
                        if (!item.Value.Evaluated && IsPropertyDefined(dependencyScope.Schema, item.Key))
                        {
                            item.Value.Evaluated = true;
                        }
                    }
                }
            }
            
        }

        private void ValidateDependantProperties(string readProperty, IList<string> requiredProperties)
        {
            if (!requiredProperties.All(r => _readProperties.Contains(r)))
            {
                IEnumerable<string> missingRequiredProperties = requiredProperties.Where(r => !_readProperties.Contains(r));
                IFormattable message = $"Dependencies for property '{readProperty}' failed. Missing required keys: {StringHelpers.Join(", ", missingRequiredProperties)}.";

                RaiseError(message, ErrorType.Dependencies, Schema, readProperty, null);
            }
        }

        private bool IsPropertyDefined(JSchema schema, string propertyName)
        {
            if (schema._properties != null && schema._properties.ContainsKey(propertyName))
            {
                return true;
            }

            if (schema._patternProperties != null)
            {
                foreach (PatternSchema patternSchema in schema.GetPatternSchemas())
                {
                    if (patternSchema.TryGetPatternRegex(
#if !(NET35 || NET40)
                        Context.Validator.RegexMatchTimeout,
#endif
                        out Regex regex,
                        out string _))
                    {
                        if (RegexHelpers.IsMatch(regex, patternSchema.Pattern, propertyName))
                        {
                            return true;
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
                        SchemaScope scope = CreateTokenScope(token, dependencySchema, CreateConditionalContext(), null, InitialDepth);
                        _dependencyScopes.Add(dependency.Key, scope);
                    }
                }
            }
            if (!Schema._dependentSchemas.IsNullOrEmpty())
            {
                foreach (KeyValuePair<string, JSchema> dependency in Schema._dependentSchemas)
                {
                    if (dependency.Value is JSchema dependencySchema)
                    {
                        SchemaScope scope = CreateTokenScope(token, dependencySchema, CreateConditionalContext(), null, InitialDepth);
                        _dependencyScopes.Add(dependency.Key, scope);
                    }
                }
            }
        }
    }
}