#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class JSchemaDiscovery
    {
        private readonly JSchema _rootSchema;
        private readonly KnownSchemaState _state;
        private readonly List<SchemaPath> _pathStack;

        public List<ValidationError> ValidationErrors { get; set; }

        public KnownSchemaCollection KnownSchemas { get; }

        public JSchemaDiscovery(JSchema rootSchema)
            : this(rootSchema, new KnownSchemaCollection(), KnownSchemaState.External)
        {
        }

        public JSchemaDiscovery(JSchema rootSchema, KnownSchemaCollection knownSchemas, KnownSchemaState state)
        {
            _rootSchema = rootSchema;
            _state = state;
            _pathStack = new List<SchemaPath>();
            KnownSchemas = knownSchemas ?? new KnownSchemaCollection();
        }

        public void Discover(JSchema schema, Uri scopedUri, string path = "#")
        {
            Uri resolvedScopeUri = scopedUri ?? schema.ResolvedId ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            _pathStack.Add(new SchemaPath(resolvedScopeUri, schema._referencedAs, string.Empty));

            DiscoverInternal(schema, path);

            _pathStack.RemoveAt(_pathStack.Count - 1);
        }

        private void DiscoverInternal(JSchema schema, string latestPath, bool isDefinitionSchema = false)
        {
            if (schema.HasReference)
            {
                return;
            }

            // give schemas that are dependencies a special state so they are written as a dependency and not inline
            KnownSchemaState resolvedSchemaState = (_state == KnownSchemaState.InlinePending && isDefinitionSchema)
                ? KnownSchemaState.DefinitionPending
                : _state;

            string scopePath = latestPath;
            Uri schemaKnownId = GetSchemaIdAndNewScopeId(schema, ref scopePath, out Uri newScopeId);

            if (KnownSchemas.Contains(schema))
            {
                KnownSchema alreadyDiscoveredSchema = KnownSchemas[schema];

                // schema was previously discovered but exists in definitions
                if (alreadyDiscoveredSchema.State == KnownSchemaState.InlinePending &&
                    resolvedSchemaState == KnownSchemaState.DefinitionPending &&
                    _rootSchema != schema)
                {
                    int existingKnownSchemaIndex = KnownSchemas.IndexOf(alreadyDiscoveredSchema);

                    KnownSchemas[existingKnownSchemaIndex] = new KnownSchema(schemaKnownId, schema, resolvedSchemaState);
                }

                return;
            }

            // check whether a schema with the resolved id is already known
            // this will be hit when a schema contains duplicate ids or references a schema with a duplicate id
            bool existingSchema = KnownSchemas.GetById(schemaKnownId) != null;

            // add schema to known schemas whether duplicate or not to avoid multiple errors
            // the first schema with a duplicate id will be used
            KnownSchemas.Add(new KnownSchema(schemaKnownId, schema, resolvedSchemaState));

            if (existingSchema)
            {
                if (ValidationErrors != null)
                {
                    ValidationError error = ValidationError.CreateValidationError($"Duplicate schema id '{schemaKnownId.OriginalString}' encountered.", ErrorType.Id, schema, null, schemaKnownId, null, schema, schema.Path);
                    ValidationErrors.Add(error);
                }
            }

            _pathStack.Add(new SchemaPath(newScopeId, schema._referencedAs, scopePath));

            // discover should happen in the same order as writer except extension data (e.g. definitions)
            if (schema._extensionData != null)
            {
                foreach (KeyValuePair<string, JToken> valuePair in schema._extensionData)
                {
                    DiscoverTokenSchemas(schema, EscapePath(valuePair.Key), valuePair.Value);
                }
            }

            DiscoverSchema(Constants.PropertyNames.AdditionalProperties, schema.AdditionalProperties);
            DiscoverSchema(Constants.PropertyNames.AdditionalItems, schema.AdditionalItems);
            DiscoverSchema(Constants.PropertyNames.UnevaluatedProperties, schema.UnevaluatedProperties);
            DiscoverSchema(Constants.PropertyNames.UnevaluatedItems, schema.UnevaluatedItems);
            DiscoverDictionarySchemas(Constants.PropertyNames.Properties, schema._properties);
            DiscoverDictionarySchemas(Constants.PropertyNames.PatternProperties, schema._patternProperties);
            DiscoverDictionarySchemas(Constants.PropertyNames.Dependencies, schema._dependencies);
            DiscoverDictionarySchemas(Constants.PropertyNames.DependentSchemas, schema._dependentSchemas);
            if (schema.ItemsPositionValidation)
            {
                DiscoverArraySchemas(Constants.PropertyNames.Items, schema._items);
            }
            else if (schema._items != null && schema._items.Count > 0)
            {
                DiscoverSchema(Constants.PropertyNames.Items, schema._items[0]);
            }
            DiscoverArraySchemas(Constants.PropertyNames.AllOf, schema._allOf);
            DiscoverArraySchemas(Constants.PropertyNames.AnyOf, schema._anyOf);
            DiscoverArraySchemas(Constants.PropertyNames.OneOf, schema._oneOf);
            DiscoverSchema(Constants.PropertyNames.Ref, schema.Ref);
            DiscoverSchema(Constants.PropertyNames.Not, schema.Not);
            DiscoverSchema(Constants.PropertyNames.PropertyNamesSchema, schema.PropertyNames);
            DiscoverSchema(Constants.PropertyNames.Contains, schema.Contains);
            DiscoverSchema(Constants.PropertyNames.If, schema.If);
            DiscoverSchema(Constants.PropertyNames.Then, schema.Then);
            DiscoverSchema(Constants.PropertyNames.Else, schema.Else);

            _pathStack.RemoveAt(_pathStack.Count - 1);
        }

        private Uri GetSchemaIdAndNewScopeId(JSchema schema, ref string latestPath, out Uri newScopeId)
        {
            Uri currentScopeId = _pathStack[_pathStack.Count - 1].ScopeId;

            string currentPath;
            if (schema.ResolvedId != null)
            {
                bool parentHash = _pathStack.Any(p => p.ScopeId == currentScopeId && p.Path != null && p.Path.IndexOf('#') != -1);
                if (parentHash)
                {
                    latestPath = string.Empty;
                    currentPath = string.Empty;
                }
                else
                {
                    latestPath = "#";
                    currentPath = "#";
                }
            }
            else if (schema._referencedAs != null && _state != KnownSchemaState.InlinePending)
            {
                currentPath = GetFragment(schema._referencedAs);
            }
            else
            {
                bool firstInScope = true;
                List<string> currentParts = new List<string>();
                for (int i = 0; i < _pathStack.Count; i++)
                {
                    SchemaPath p = _pathStack[i];

                    if (p.ScopeId == currentScopeId)
                    {
                        if (p.ReferencedAs != null && !firstInScope && _state != KnownSchemaState.InlinePending)
                        {
                            currentParts.Clear();

                            string reference = GetFragment(p.ReferencedAs);

                            currentParts.Add(reference);
                        }
                        else if (!string.IsNullOrEmpty(p.Path))
                        {
                            currentParts.Add(p.Path);
                        }

                        firstInScope = false;
                    }
                }

                currentPath = StringHelpers.Join("/", currentParts);

                if (!string.IsNullOrEmpty(currentScopeId.OriginalString)
                    && !currentPath.StartsWith("#", StringComparison.Ordinal))
                {
                    currentPath = "#/" + currentPath;
                }

                if (!string.IsNullOrEmpty(currentPath)
                    && !currentPath.EndsWith("/", StringComparison.Ordinal))
                {
                    currentPath += "/";
                }

                currentPath += latestPath;
            }

            Uri schemaKnownId = ResolveSchemaIdAndScopeId(currentScopeId, schema, currentPath, out newScopeId);
            return schemaKnownId;
        }

        private static string GetFragment(Uri referencedAs)
        {
            string reference = referencedAs.OriginalString;
            int fragmentIndex = reference.IndexOf('#');
            if (fragmentIndex > 0)
            {
                reference = reference.Substring(fragmentIndex);
            }

            return reference;
        }

        public static Uri ResolveSchemaIdAndScopeId(Uri idScope, JSchema schema, string path, out Uri newScope)
        {
            Uri schemaId = schema.ResolvedId;
            Uri knownSchemaId;
            if (schemaId != null)
            {
                try
                {
                    newScope = SchemaDiscovery.ResolveSchemaId(idScope, schemaId);
                }
                catch (Exception ex)
                {
                    string message = "Error resolving schema ID '{0}' in the current scope. The resolved ID must be a valid URI.".FormatWith(CultureInfo.InvariantCulture, schemaId);

                    throw new JSchemaException(JSchemaException.FormatMessage(schema, schema.Path, message), ex);
                }

                knownSchemaId = newScope;
            }
            else
            {
                if (idScope == null)
                {
                    knownSchemaId = new Uri(path, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    knownSchemaId = SchemaDiscovery.ResolveSchemaId(idScope, new Uri(path, UriKind.RelativeOrAbsolute));
                }

                newScope = idScope;
            }

            return knownSchemaId;
        }

        private void DiscoverTokenSchemas(JSchema schema, string name, JToken token, bool isDefinitionSchema = false)
        {
            if (token is JObject o)
            {
                JSchemaAnnotation annotation = token.Annotation<JSchemaAnnotation>();
                if (annotation != null)
                {
                    DiscoverInternal(annotation.Schema, name, isDefinitionSchema);
                }
                else
                {
                    foreach (KeyValuePair<string, JToken> valuePair in o)
                    {
                        bool isDefinitionsSchema = Constants.PropertyNames.IsDefinition(name) && schema == _rootSchema;

                        DiscoverTokenSchemas(schema, name + "/" + EscapePath(valuePair.Key), valuePair.Value, isDefinitionsSchema);
                    }
                }
            }
            else if (token is JArray || token is JConstructor)
            {
                IList<JToken> l = (IList<JToken>)token;

                for (int i = 0; i < l.Count; i++)
                {
                    DiscoverTokenSchemas(schema, name + "/" + i.ToString(CultureInfo.InvariantCulture), l[i]);
                }
            }
        }

        private void DiscoverDictionarySchemas(string name, IDictionary<string, object> schemas)
        {
            if (schemas != null)
            {
                foreach (KeyValuePair<string, object> valuePair in schemas)
                {
                    if (valuePair.Value is JSchema schema)
                    {
                        DiscoverInternal(schema, name + "/" + EscapePath(valuePair.Key));
                    }
                }
            }
        }

        private void DiscoverDictionarySchemas(string name, IDictionary<string, JSchema> schemas)
        {
            if (schemas != null)
            {
                foreach (KeyValuePair<string, JSchema> valuePair in schemas)
                {
                    DiscoverInternal(valuePair.Value, name + "/" + EscapePath(valuePair.Key));
                }
            }
        }

        private void DiscoverArraySchemas(string name, IList<JSchema> schemas)
        {
            if (schemas != null)
            {
                for (int i = 0; i < schemas.Count; i++)
                {
                    DiscoverInternal(schemas[i], name + "/" + i.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private void DiscoverSchema(string name, JSchema schema)
        {
            if (schema != null)
            {
                DiscoverInternal(schema, name);
            }
        }

        private string EscapePath(string path)
        {
            return path.Replace("~", "~0").Replace("/", "~1");
        }
    }
}