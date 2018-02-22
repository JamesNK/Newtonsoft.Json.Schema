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

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class JSchemaDiscovery
    {
        private readonly JSchema _rootSchema;
        private readonly KnownSchemaState _state;
        private readonly List<SchemaPath> _pathStack;
        private readonly KnownSchemaCollection _knownSchemas;

        public List<ValidationError> ValidationErrors { get; set; }

        public KnownSchemaCollection KnownSchemas => _knownSchemas;

        public JSchemaDiscovery(JSchema rootSchema)
            : this(rootSchema, new KnownSchemaCollection(), KnownSchemaState.External)
        {
        }

        public JSchemaDiscovery(JSchema rootSchema, KnownSchemaCollection knownSchemas, KnownSchemaState state)
        {
            _rootSchema = rootSchema;
            _state = state;
            _pathStack = new List<SchemaPath>();
            _knownSchemas = knownSchemas ?? new KnownSchemaCollection();
        }

        public void Discover(JSchema schema, Uri uri, string path = "#")
        {
            Uri resolvedUri = uri ?? schema.Id ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            _pathStack.Add(new SchemaPath(resolvedUri, string.Empty));

            DiscoverInternal(schema, path);

            _pathStack.RemoveAt(_pathStack.Count - 1);
        }

        private void DiscoverInternal(JSchema schema, string latestPath, bool isDefinitionSchema = false)
        {
            if (schema.Reference != null)
            {
                return;
            }

            // give schemas that are dependencies a special state so they are written as a dependency and not inline
            KnownSchemaState resolvedSchemaState = (_state == KnownSchemaState.InlinePending && isDefinitionSchema)
                ? KnownSchemaState.DefinitionPending
                : _state;

            string scopePath = latestPath;
            Uri schemaKnownId = GetSchemaIdAndNewScopeId(schema, ref scopePath, out Uri newScopeId);

            if (_knownSchemas.Contains(schema))
            {
                KnownSchema alreadyDiscoveredSchema = _knownSchemas[schema];

                // schema was previously discovered but exists in definitions
                if (alreadyDiscoveredSchema.State == KnownSchemaState.InlinePending &&
                    resolvedSchemaState == KnownSchemaState.DefinitionPending &&
                    _rootSchema != schema)
                {
                    int existingKnownSchemaIndex = _knownSchemas.IndexOf(alreadyDiscoveredSchema);

                    _knownSchemas[existingKnownSchemaIndex] = new KnownSchema(schemaKnownId, schema, resolvedSchemaState);
                }

                return;
            }

            // check whether a schema with the resolved id is already known
            // this will be hit when a schema contains duplicate ids or references a schema with a duplicate id
            bool existingSchema = _knownSchemas.GetById(schemaKnownId) != null;

            // add schema to known schemas whether duplicate or not to avoid multiple errors
            // the first schema with a duplicate id will be used
            _knownSchemas.Add(new KnownSchema(schemaKnownId, schema, resolvedSchemaState));

            if (existingSchema)
            {
                if (ValidationErrors != null)
                {
                    ValidationError error = ValidationError.CreateValidationError($"Duplicate schema id '{schemaKnownId.OriginalString}' encountered.", ErrorType.Id, schema, null, schemaKnownId, null, schema, schema.Path);
                    ValidationErrors.Add(error);
                }
            }

            _pathStack.Add(new SchemaPath(newScopeId, scopePath));

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
            DiscoverDictionarySchemas(Constants.PropertyNames.Properties, schema._properties);
            DiscoverDictionarySchemas(Constants.PropertyNames.PatternProperties, schema._patternProperties);
            DiscoverDictionarySchemas(Constants.PropertyNames.Dependencies, schema._dependencies);
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
            Uri currentScopeId = _pathStack[_pathStack.Count - 1].Id;

            string currentPath;
            if (schema.Id == null)
            {
                currentPath = StringHelpers.Join("/", _pathStack.Where(p => p.Id == currentScopeId && !string.IsNullOrEmpty(p.Path)).Select(p => p.Path));

                if (!string.IsNullOrEmpty(currentScopeId.OriginalString)
                    && !currentPath.StartsWith("#", StringComparison.Ordinal))
                {
                    currentPath = "#/" + currentPath;
                }

                if (!string.IsNullOrEmpty(currentPath)
                    && !currentPath.EndsWith('/'))
                {
                    currentPath += "/";
                }

                currentPath += latestPath;
            }
            else
            {
                bool parentHash = _pathStack.Any(p => p.Id == currentScopeId && p.Path != null && p.Path.IndexOf('#') != -1);
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

            Uri schemaKnownId = ResolveSchemaIdAndScopeId(currentScopeId, schema.Id, currentPath, out newScopeId);
            return schemaKnownId;
        }

        public static Uri ResolveSchemaIdAndScopeId(Uri idScope, Uri schemaId, string path, out Uri newScope)
        {
            Uri knownSchemaId;
            if (schemaId != null)
            {
                newScope = SchemaDiscovery.ResolveSchemaId(idScope, schemaId);

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
            if (token is JObject)
            {
                JObject o = (JObject)token;

                JSchemaAnnotation annotation = token.Annotation<JSchemaAnnotation>();
                if (annotation != null)
                {
                    DiscoverInternal(annotation.Schema, name, isDefinitionSchema);
                }
                else
                {
                    foreach (KeyValuePair<string, JToken> valuePair in o)
                    {
                        bool isDefinitionsSchema = string.Equals(name, Constants.PropertyNames.Definitions, StringComparison.Ordinal) && schema == _rootSchema;

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