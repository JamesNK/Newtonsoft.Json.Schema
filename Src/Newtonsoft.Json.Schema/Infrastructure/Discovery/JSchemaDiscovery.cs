#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class JSchemaDiscovery
    {
        private readonly JSchema? _rootSchema;
        private readonly KnownSchemaState _state;
        private readonly List<SchemaPath> _pathStack;

        public List<ValidationError>? ValidationErrors { get; set; }
        public SchemaVersion SchemaVersion { get; set; }

        public KnownSchemaCollection KnownSchemas { get; }

        public JSchemaDiscovery(JSchema? rootSchema)
            : this(rootSchema, SchemaVersion.Unset, new KnownSchemaCollection(), KnownSchemaState.External)
        {
        }

        public JSchemaDiscovery(JSchema? rootSchema, SchemaVersion schemaVersion, KnownSchemaCollection knownSchemas, KnownSchemaState state)
        {
            _rootSchema = rootSchema;
            SchemaVersion = schemaVersion;
            _state = state;
            _pathStack = new List<SchemaPath>();
            KnownSchemas = knownSchemas ?? new KnownSchemaCollection();
        }

        public void Discover(JSchema schema, Uri? scopedUri, string path = "#", Uri? dynamicScope = null)
        {
            Uri resolvedScopeUri = scopedUri ?? schema.ResolvedId ?? new Uri(string.Empty, UriKind.RelativeOrAbsolute);

            _pathStack.Add(new SchemaPath(resolvedScopeUri, schema._referencedAs, string.Empty, dynamicScope));

            DiscoverInternal(schema, path);

            _pathStack.RemoveAt(_pathStack.Count - 1);
        }

        private void DiscoverInternal(JSchema schema, string latestPath, bool isDefinitionSchema = false, Uri? dynamicScope = null)
        {
            // Resolving the current scope from the path stack is a bit of a hack to avoid passing it to each method.
            // Maybe there should be a discover context that gets passed around and dynamicScope is a value on it?
            dynamicScope ??= schema.DynamicLoadScope ?? GetDynamicScope();

            // give schemas that are dependencies a special state so they are written as a dependency and not inline
            KnownSchemaState resolvedSchemaState = (_state == KnownSchemaState.InlinePending && isDefinitionSchema)
                ? KnownSchemaState.DefinitionPending
                : _state;

            string scopePath = latestPath;
            Uri schemaKnownId = GetSchemaIdAndNewScopeId(schema, ref scopePath, out Uri? newScopeId);

            KnownSchemaKey knownSchemaKey = new KnownSchemaKey(schema, dynamicScope);
            if (KnownSchemas.Contains(knownSchemaKey))
            {
                KnownSchema alreadyDiscoveredSchema = KnownSchemas[knownSchemaKey];

                // schema was previously discovered but exists in definitions
                if (alreadyDiscoveredSchema.State == KnownSchemaState.InlinePending &&
                    resolvedSchemaState == KnownSchemaState.DefinitionPending &&
                    _rootSchema != schema)
                {
                    int existingKnownSchemaIndex = KnownSchemas.IndexOf(alreadyDiscoveredSchema);

                    KnownSchemas[existingKnownSchemaIndex] = new KnownSchema(schemaKnownId, dynamicScope, schema, schema.Root, resolvedSchemaState);
                }

                return;
            }

            if (ValidationErrors != null)
            {
                // check whether a schema with the resolved id is already known
                // this will be hit when a schema contains duplicate ids or references a schema with a duplicate id
                bool existingSchema = KnownSchemas.GetById(new KnownSchemaUriKey(schemaKnownId, dynamicScope, schema.Root)) != null;
                if (existingSchema)
                {
                    ValidationError error = ValidationError.CreateValidationError($"Duplicate schema id '{schemaKnownId.OriginalString}' encountered.", ErrorType.Id, schema, null, schemaKnownId, null, schema, schema.Path!);
                    ValidationErrors.Add(error);
                }
            }

            // If a schema was loaded with a dynamic scope, then don't attempt to registered it as a known schema with a different scope.
            // The schema should be loaded again to handle dynamic changes.
            if (schema.DynamicLoadScope != null &&
                dynamicScope != null &&
                !UriComparer.Instance.Equals(schema.DynamicLoadScope, dynamicScope))
            {
                return;
            }

            // add schema to known schemas whether duplicate or not to avoid multiple errors
            // the first schema with a duplicate id will be used
            KnownSchemas.Add(new KnownSchema(schemaKnownId, dynamicScope, schema, schema.Root, resolvedSchemaState));

            ValidationUtils.Assert(newScopeId != null);
            _pathStack.Add(new SchemaPath(newScopeId, schema._referencedAs, scopePath, dynamicScope));

            // discover should happen in the same order as writer except extension data (e.g. definitions)
            if (schema._extensionData != null)
            {
                var pathScopes = new List<string>();
                foreach (KeyValuePair<string, JToken> valuePair in schema._extensionData)
                {
                    pathScopes.Add(EscapePath(valuePair.Key));
                    DiscoverTokenSchemas(schema, pathScopes, valuePair.Value);
                    pathScopes.RemoveAt(pathScopes.Count - 1);
                }
            }

            DiscoverSchema(Constants.PropertyNames.AdditionalProperties, schema.AdditionalProperties);
            if (SchemaVersionHelpers.EnsureVersion(SchemaVersion, SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
            {
                DiscoverSchema(Constants.PropertyNames.AdditionalItems, schema.AdditionalItems);
            }
            else
            {
                DiscoverSchema(Constants.PropertyNames.Items, schema.AdditionalItems);
            }
            DiscoverSchema(Constants.PropertyNames.UnevaluatedProperties, schema.UnevaluatedProperties);
            DiscoverSchema(Constants.PropertyNames.UnevaluatedItems, schema.UnevaluatedItems);
            DiscoverDictionarySchemas(Constants.PropertyNames.Properties, schema._properties);
            DiscoverDictionarySchemas(Constants.PropertyNames.PatternProperties, schema._patternProperties);
            DiscoverDictionarySchemas(Constants.PropertyNames.Dependencies, schema._dependencies);
            DiscoverDictionarySchemas(Constants.PropertyNames.DependentSchemas, schema._dependentSchemas);
            if (SchemaVersionHelpers.EnsureVersion(SchemaVersion, SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
            {
                if (schema.ItemsPositionValidation)
                {
                    DiscoverArraySchemas(Constants.PropertyNames.Items, schema._items);
                }
                else if (schema._items != null && schema._items.Count > 0)
                {
                    DiscoverSchema(Constants.PropertyNames.Items, schema._items[0]);
                }
            }
            else
            {
                DiscoverArraySchemas(Constants.PropertyNames.PrefixItems, schema._items);
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

        private Uri? GetDynamicScope()
        {
            for (int i = _pathStack.Count - 1; i >= 0; i--)
            {
                if (_pathStack[i].DynamicScope is { } stackScope)
                {
                    return stackScope;
                }
            }

            return null;
        }

        private Uri GetSchemaIdAndNewScopeId(JSchema schema, ref string latestPath, out Uri? newScopeId)
        {
            Uri currentScopeId = _pathStack[_pathStack.Count - 1].ScopeId;

            string currentPath;
            if (schema.ResolvedId != null)
            {
                bool parentHash = false;
                for (int i = 0; i < _pathStack.Count; i++)
                {
                    SchemaPath p = _pathStack[i];
                    if (p.ScopeId == currentScopeId && p.Path != null && StringHelpers.IndexOf(p.Path, '#') != -1)
                    {
                        parentHash = true;
                        break;
                    }
                }

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

                if (!string.IsNullOrEmpty(currentScopeId.OriginalString)
                    && (currentParts.Count == 0 || !currentParts[0].StartsWith("#", StringComparison.Ordinal)))
                {
                    currentParts.Insert(0, "#");
                }

                currentParts.Add(latestPath);

                currentPath = StringHelpers.Join("/", currentParts);
            }

            Uri schemaKnownId = ResolveSchemaIdAndScopeId(currentScopeId, schema, currentPath, out newScopeId);
            return schemaKnownId;
        }

        internal static string GetFragment(Uri referencedAs)
        {
            string reference = referencedAs.OriginalString;
            int fragmentIndex = StringHelpers.IndexOf(reference, '#');
            if (fragmentIndex > 0)
            {
                reference = reference.Substring(fragmentIndex);
            }

            return reference;
        }

        public static Uri ResolveSchemaIdAndScopeId(Uri? idScope, JSchema schema, string path, out Uri? newScope)
        {
            Uri? schemaId = schema.ResolvedId;
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

        private void DiscoverTokenSchemas(JSchema schema, List<string> pathScopes, JToken? token, bool isDefinitionSchema = false)
        {
            if (token is JObject o)
            {
                JSchemaAnnotation? annotation = token.Annotation<JSchemaAnnotation>();
                Uri? dynamicScope = GetDynamicScope();

                JSchema? registeredSchema = annotation?.GetSchema(dynamicScope);
                if (registeredSchema != null)
                {
                    string name;
                    if (pathScopes.Count == 1)
                    {
                        name = pathScopes[0];
                    }
                    else
                    {
                        name = StringHelpers.Join("/", pathScopes);
                    }
                    DiscoverInternal(registeredSchema, name, isDefinitionSchema, dynamicScope);
                }
                else
                {
                    foreach (KeyValuePair<string, JToken?> valuePair in o)
                    {
                        bool isDefinitionsSchema = pathScopes.Count == 1 && Constants.PropertyNames.IsDefinition(pathScopes[0]) && schema == _rootSchema;

                        pathScopes.Add(EscapePath(valuePair.Key));
                        DiscoverTokenSchemas(schema, pathScopes, valuePair.Value, isDefinitionsSchema);
                        pathScopes.RemoveAt(pathScopes.Count - 1);
                    }
                }
            }
            else if (token is JArray || token is JConstructor)
            {
                IList<JToken> l = (IList<JToken>)token;

                for (int i = 0; i < l.Count; i++)
                {
                    pathScopes.Add(i.ToString(CultureInfo.InvariantCulture));
                    DiscoverTokenSchemas(schema, pathScopes, l[i]);
                    pathScopes.RemoveAt(pathScopes.Count - 1);
                }
            }
        }

        private void DiscoverDictionarySchemas(string name, IDictionary<string, object>? schemas)
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

        private void DiscoverDictionarySchemas(string name, IDictionary<string, JSchema>? schemas)
        {
            if (schemas != null)
            {
                foreach (KeyValuePair<string, JSchema> valuePair in schemas)
                {
                    DiscoverInternal(valuePair.Value, name + "/" + EscapePath(valuePair.Key));
                }
            }
        }

        private void DiscoverArraySchemas(string name, IList<JSchema>? schemas)
        {
            if (schemas != null)
            {
                for (int i = 0; i < schemas.Count; i++)
                {
                    DiscoverInternal(schemas[i], name + "/" + i.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private void DiscoverSchema(string name, JSchema? schema)
        {
            if (schema != null)
            {
                DiscoverInternal(schema, name);
            }
        }

        private string EscapePath(string path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '~' || path[i] == '/')
                {
                    // Could be faster but most paths don't need to be escaped.
                    string escapedPath = StringHelpers.Replace(path, "~", "~0");
                    escapedPath = StringHelpers.Replace(escapedPath, "/", "~1");
                    return escapedPath;
                }
            }

            return path;
        }
    }
}