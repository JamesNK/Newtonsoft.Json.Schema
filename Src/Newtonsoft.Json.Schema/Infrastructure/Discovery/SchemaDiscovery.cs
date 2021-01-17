﻿#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal static class SchemaDiscovery
    {
        private static string UnescapeReference(string reference)
        {
            return Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");
        }

        private static Uri? GetTokenId(JToken o, JSchemaReader schemaReader)
        {
            Uri? id = null;
            if (schemaReader.EnsureVersion(SchemaVersion.Draft6)
                && o[Constants.PropertyNames.Id] is JValue idToken
                && (idToken.Type == JTokenType.String || idToken.Type == JTokenType.Uri))
            {
                id = GetTokenUri(idToken);
            }
            else if (o[Constants.PropertyNames.IdDraft4] is JValue idDraf4Token
                && (idDraf4Token.Type == JTokenType.String || idDraf4Token.Type == JTokenType.Uri))
            {
                id = GetTokenUri(idDraf4Token);
            }

            string? anchor = null;
            if (schemaReader.EnsureVersion(SchemaVersion.Draft2019_09)
                && o[Constants.PropertyNames.Anchor] is JValue anchorToken
                && anchorToken.Type == JTokenType.String)
            {
                anchor = (string)anchorToken!;
            }

            return CombineIdAndAnchor(id, anchor);
        }

        private static string? GetTokenDynamicAnchor(JToken o, JSchemaReader schemaReader)
        {
            if (schemaReader.EnsureVersion(SchemaVersion.Draft2019_09)
                && o[Constants.PropertyNames.RecursiveAnchor] is JValue recursiveAnchor
                && (recursiveAnchor.Type == JTokenType.Boolean))
            {
                return (bool)recursiveAnchor ? bool.TrueString: null;
            }

            return null;
        }

        public static Uri? CombineIdAndAnchor(Uri? id, string? anchor)
        {
            if (id != null)
            {
                if (!string.IsNullOrEmpty(anchor))
                {
                    return new Uri(id, "#" + anchor);
                }
                else
                {
                    return id;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(anchor))
                {
                    return new Uri("#" + anchor, UriKind.Relative);
                }
            }

            return null;
        }

        private static Uri? GetTokenUri(JValue t)
        {
            Uri? id;
            if (t.Type == JTokenType.Uri)
            {
                id = (Uri)t!;
            }
            else if (!Uri.TryCreate((string)t!, UriKind.RelativeOrAbsolute, out id))
            {
                id = null;
            }

            return id;
        }

        public static bool FindSchema(
            Action<JSchema> setSchema,
            JSchema schema,
            Uri? rootSchemaId,
            Uri reference,
            Uri originalReference,
            Uri? dynamicScope,
            JSchemaReader schemaReader,
            ref JSchemaDiscovery discovery)
        {
            // todo, better way to get parts from Uri
            string[] parts = reference.ToString().Split('/');

            bool resolvedSchema;

            if (parts.Length > 0 && IsInternalSchemaReference(parts[0], rootSchemaId))
            {
                int scopeInitialCount = schemaReader._identiferScopeStack.Count;

                schemaReader._identiferScopeStack.Add(schema);

                JSchema parent = schema;
                object? current = schema;
                for (int i = 1; i != parts.Length; ++i)
                {
                    string unescapedPart = UnescapeReference(parts[i]);

                    switch (current)
                    {
                        case JSchema s:
                            if (s != schema)
                            {
                                schemaReader._identiferScopeStack.Add(s);
                            }

                            parent = s;
                            current = GetCurrentFromSchema(s, unescapedPart);
                            break;
                        case JToken t:
                            IIdentiferScope? scope = null;
                            if (t is JObject)
                            {
                                Uri? id = GetTokenId(t, schemaReader);
                                if (id != null)
                                {
                                    scope = new JsonIdentiferScope(id, false, GetTokenDynamicAnchor(t, schemaReader));
                                }
                            }

                            schemaReader._identiferScopeStack.Add(scope ?? JsonIdentiferScope.Empty);

                            current = GetCurrentFromToken(t, unescapedPart, dynamicScope);
                            break;
                        case IDictionary<string, JSchema> d:
                            d.TryGetValue(unescapedPart, out JSchema temp);
                            current = temp;
                            break;
                        case IList<JSchema> l:
                            if (TryGetImplicitItemsSchema(parent, l, out JSchema? itemsSchema))
                            {
                                current = GetCurrentFromSchema(itemsSchema, unescapedPart);
                            }
                            else if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out int index))
                            {
                                if (index >= l.Count || index < 0)
                                {
                                    current = null;
                                }
                                else
                                {
                                    current = l[index];
                                }
                            }
                            else
                            {
                                current = null;
                            }
                            break;
                    }

                    if (current == null)
                    {
                        break;
                    }
                }

                if (current is IList<JSchema> itemsSchemas)
                {
                    if (TryGetImplicitItemsSchema(parent, itemsSchemas, out JSchema? itemsSchema))
                    {
                        current = itemsSchema;
                    }
                }

                switch (current)
                {
                    case JToken t:
                        JSchemaAnnotation? annotation = t.Annotation<JSchemaAnnotation>();
                        JSchema? previousSchema = annotation?.GetSchema(dynamicScope);
                        if (previousSchema != null)
                        {
                            setSchema(previousSchema);
                            resolvedSchema = true;
                        }
                        else
                        {
                            JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema, t, dynamicScope);
                            inlineSchema._referencedAs = originalReference;

                            discovery.Discover(inlineSchema, rootSchemaId);

                            resolvedSchema = true;
                        }
                        break;
                    case JSchema s:
                        setSchema(s);
                        resolvedSchema = true;

                        // schema is a reference schema and needs to be resolved
                        if (s.HasReference)
                        {
                            schemaReader.AddDeferedSchema(null, setSchema, s);
                        }
                        break;
                    default:
                        resolvedSchema = false;
                        break;
                }

                while (schemaReader._identiferScopeStack.Count > scopeInitialCount)
                {
                    schemaReader._identiferScopeStack.RemoveAt(schemaReader._identiferScopeStack.Count - 1);
                }
            }
            else
            {
                discovery.Discover(schema, null);

                Uri resolvedReference = ResolveSchemaId(rootSchemaId, reference);

                // use firstordefault to handle duplicates
                KnownSchema knownSchema = discovery.KnownSchemas.FirstOrDefault(s => UriComparer.Instance.Equals(s.Id, resolvedReference));

                if (knownSchema != null)
                {
                    resolvedSchema = true;
                    setSchema(knownSchema.Schema);
                }
                else
                {
                    if (SplitReference(resolvedReference, out Uri path, out Uri? fragment))
                    {
                        // there could be duplicated ids. use FirstOrDefault to get first schema with an id
                        knownSchema = discovery.KnownSchemas.FirstOrDefault(s => UriComparer.Instance.Equals(s.Id, path));

                        if (knownSchema != null)
                        {
                            // don't attempt to find a schema in the same schema again
                            // avoids stackoverflow
                            if (knownSchema.Schema != schema
                                || !UriComparer.Instance.Equals(rootSchemaId, path)
                                || !UriComparer.Instance.Equals(reference, fragment))
                            {
                                resolvedSchema = FindSchema(setSchema, knownSchema.Schema, path, fragment, originalReference, dynamicScope, schemaReader, ref discovery);
                            }
                            else
                            {
                                resolvedSchema = false;
                            }
                        }
                        else
                        {
                            resolvedSchema = false;
                        }
                    }
                    else
                    {
                        resolvedSchema = false;
                    }

                    if (!resolvedSchema)
                    {
                        resolvedSchema = TryFindSchemaInDefinitions(Constants.PropertyNames.Definitions, setSchema, schema, rootSchemaId, dynamicScope, schemaReader, discovery, resolvedReference);
                        if (!resolvedSchema)
                        {
                            resolvedSchema = TryFindSchemaInDefinitions(Constants.PropertyNames.Defs, setSchema, schema, rootSchemaId, dynamicScope, schemaReader, discovery, resolvedReference);
                        }
                    }
                }
            }

            return resolvedSchema;
        }

        private static bool SplitReference(Uri reference, out Uri path, [NotNullWhen(true)] out Uri? fragment)
        {
            int hashIndex = reference.OriginalString.IndexOf('#');
            if (hashIndex != -1)
            {
                path = new Uri(reference.OriginalString.Substring(0, hashIndex), UriKind.RelativeOrAbsolute);
                fragment = new Uri(reference.OriginalString.Substring(hashIndex), UriKind.RelativeOrAbsolute);
                return true;
            }
            else
            {
                path = reference;
                fragment = null;
                return false;
            }
        }

        private static bool IsInternalSchemaReference(string firstPart, Uri? rootSchemaId)
        {
            if (firstPart == "#")
            {
                return true;
            }

            if (rootSchemaId != null)
            {
                string id = rootSchemaId.ToString();
                if (!id.EndsWith("#", StringComparison.Ordinal))
                {
                    id += "#";
                }

                return firstPart == id;
            }

            return false;
        }

        private static bool TryFindSchemaInDefinitions(string definitionsName, Action<JSchema> setSchema, JSchema schema, Uri? rootSchemaId, Uri? dynamicScope,
            JSchemaReader schemaReader, JSchemaDiscovery discovery, Uri resolvedReference)
        {
            // special case
            // look in the root schema's definitions for a definition with the same property name and id as reference
            if (schema.ExtensionData.TryGetValue(definitionsName, out JToken definitions))
            {
                if (definitions is JObject definitionsObject)
                {
                    JProperty matchingProperty = definitionsObject
                        .Properties()
                        .FirstOrDefault(p => TryCompare(p.Name, resolvedReference));

                    if (matchingProperty?.Value is JObject o)
                    {
                        if (IsIdMatch(schemaReader, resolvedReference, o, rootSchemaId))
                        {
                            JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema, o, dynamicScope);

                            discovery.Discover(inlineSchema, rootSchemaId, definitionsName + "/" + matchingProperty.Name);

                            return true;
                        }
                    }
                    else
                    {
                        SplitReference(resolvedReference, out Uri path, out Uri? fragment);

                        return CheckDefinitionSchemaIds(definitionsName, setSchema, rootSchemaId, dynamicScope, schemaReader, discovery, resolvedReference, path, fragment, definitionsObject);
                    }
                }
            }

            return false;
        }

        private static bool CheckDefinitionSchemaIds(string definitionsName, Action<JSchema> setSchema, Uri? rootSchemaId, Uri? dynamicScope, JSchemaReader schemaReader, JSchemaDiscovery discovery, Uri resolvedReference, Uri matchingId, Uri? matchingFragment, JObject definitionsObject)
        {
            foreach (KeyValuePair<string, JToken?> property in definitionsObject)
            {
                if (property.Value is JObject obj)
                {
                    if (IsIdMatch(schemaReader, matchingId, obj, rootSchemaId))
                    {
                        // Pass in no setSchema here because we want to find a schema using the path first,
                        // without setting the schema, and then resolve the fragment using that schema.
                        JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema: null, obj, dynamicScope);
                        discovery.Discover(inlineSchema, rootSchemaId, definitionsName + "/" + property.Key);

                        if (matchingFragment != null)
                        {
                            return FindSchema(setSchema, inlineSchema, rootSchemaId: null, matchingFragment, matchingFragment, dynamicScope, schemaReader, ref discovery);
                        }
                        else
                        {
                            setSchema(inlineSchema);
                        }

                        return true;
                    }
                    else if (IsIdMatch(schemaReader, matchingFragment, obj, rootSchemaId) ||
                        IsIdMatch(schemaReader, resolvedReference, obj, rootSchemaId))
                    {
                        JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema, obj, dynamicScope);
                        discovery.Discover(inlineSchema, rootSchemaId, definitionsName + "/" + property.Key);

                        return true;
                    }
                    else
                    {
                        // Definition object doesn't match but one of its nested definitions might
                        // Recurse into nested definitions using the current definition scope
                        if (IsNestedDefinitionMatch(Constants.PropertyNames.Definitions, setSchema, rootSchemaId, dynamicScope, schemaReader, discovery, resolvedReference, matchingId, matchingFragment, obj))
                        {
                            return true;
                        }
                        if (IsNestedDefinitionMatch(Constants.PropertyNames.Defs, setSchema, rootSchemaId, dynamicScope, schemaReader, discovery, resolvedReference, matchingId, matchingFragment, obj))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsNestedDefinitionMatch(string definitionsName, Action<JSchema> setSchema, Uri? rootSchemaId, Uri? dynamicScope, JSchemaReader schemaReader, JSchemaDiscovery discovery,
            Uri resolvedReference, Uri matchingId, Uri? matchingFragment, JObject obj)
        {
            JObject? nestedDefinitions = obj[definitionsName] as JObject;
            if (nestedDefinitions != null)
            {
                Uri? id = GetTokenId(obj, schemaReader);

                Uri? resolvedId = id != null
                    ? ResolveSchemaId(rootSchemaId, id)
                    : rootSchemaId;

                return CheckDefinitionSchemaIds(definitionsName, setSchema, resolvedId, dynamicScope, schemaReader, discovery, resolvedReference, matchingId, matchingFragment, nestedDefinitions);
            }

            return false;
        }

        private static bool IsIdMatch(JSchemaReader schemaReader, Uri? resolvedReference, JObject o, Uri? rootSchemaId)
        {
            Uri? id = GetTokenId(o, schemaReader);

            if (id == null)
            {
                return false;
            }

            if (UriComparer.Instance.Equals(id, resolvedReference))
            {
                return true;
            }

            Uri resolvedId = ResolveSchemaId(rootSchemaId, id);

            if (UriComparer.Instance.Equals(resolvedId, resolvedReference))
            {
                return true;
            }

            return false;
        }

        private static bool TryGetImplicitItemsSchema(JSchema parent, IList<JSchema> items, [NotNullWhen(true)] out JSchema? schema)
        {
            // if the schema collection is items then implicitly get first item if there is no position validation
            if (ReferenceEquals(parent._items, items) && !parent.ItemsPositionValidation)
            {
                if (items.Count > 0)
                {
                    schema = items[0];
                    return true;
                }
            }

            schema = null;
            return false;
        }

        private static object? GetCurrentFromSchema(JSchema s, string unescapedPart)
        {
            switch (unescapedPart)
            {
                case Constants.PropertyNames.Properties:
                    return s._properties;
                case Constants.PropertyNames.Items:
                    return s._items;
                case Constants.PropertyNames.AdditionalProperties:
                    return s.AdditionalProperties;
                case Constants.PropertyNames.AdditionalItems:
                    return s.AdditionalItems;
                case Constants.PropertyNames.Not:
                    return s.Not;
                case Constants.PropertyNames.OneOf:
                    return s._oneOf;
                case Constants.PropertyNames.AllOf:
                    return s._allOf;
                case Constants.PropertyNames.AnyOf:
                    return s._anyOf;
                case Constants.PropertyNames.Enum:
                    return s._enum;
                case Constants.PropertyNames.PatternProperties:
                    return s._patternProperties;
                case Constants.PropertyNames.Dependencies:
                    return s._dependencies;
                default:
                    JToken temp;
                    s.ExtensionData.TryGetValue(unescapedPart, out temp);
                    return temp;
            }
        }

        private static object? GetCurrentFromToken(JToken t, string unescapedPart, Uri? dynamicScope)
        {
            JToken? resolvedToken;

            if (t is JObject)
            {
                resolvedToken = t[unescapedPart];
            }
            else if (t is JArray || t is JConstructor)
            {
                if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out int index))
                {
                    if (index >= t.Count() || index < 0)
                    {
                        resolvedToken = null;
                    }
                    else
                    {
                        resolvedToken = t[index];
                    }
                }
                else
                {
                    resolvedToken = null;
                }
            }
            else
            {
                resolvedToken = null;
            }

            if (resolvedToken == null)
            {
                return null;
            }

            JSchemaAnnotation? annotation = resolvedToken.Annotation<JSchemaAnnotation>();
            JSchema? previousSchema = annotation?.GetSchema(dynamicScope);
            if (previousSchema != null)
            {
                return previousSchema;
            }
            else
            {
                return resolvedToken;
            }
        }

        private static bool TryCompare(string value, Uri resolvedReference)
        {
            if (value == null)
            {
                return false;
            }

            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri definitionUri))
            {
                return UriComparer.Instance.Equals(definitionUri, resolvedReference);
            }

            return false;
        }

        public static Uri ResolveSchemaId(Uri? idScope, Uri schemaId)
        {
            if (idScope == null || schemaId.IsAbsoluteUri || schemaId.OriginalString.StartsWith("//", StringComparison.Ordinal))
            {
                idScope = schemaId;
            }
            else
            {
                Uri newId;
                Uri? tempRoot;

                try
                {
                    Uri resolvedId;
                    if (!idScope.IsAbsoluteUri)
                    {
                        tempRoot = new Uri("http://localhost/");
                        resolvedId = new Uri(tempRoot, idScope);
                    }
                    else
                    {
                        tempRoot = null;
                        resolvedId = idScope;
                    }

                    newId = new Uri(resolvedId, schemaId);
                }
                catch (Exception ex)
                {
                    string errorMessage = "Invalid URI error while resolving schema ID. Scope URI: '{0}', schema URI: '{1}'".FormatWith(CultureInfo.InvariantCulture, idScope.OriginalString, schemaId.OriginalString);

                    throw new JSchemaException(errorMessage, ex);
                }

                if (tempRoot != null)
                {
                    string relativeId = newId.OriginalString.Substring(tempRoot.OriginalString.Length);
                    newId = new Uri(relativeId, UriKind.RelativeOrAbsolute);
                }

                idScope = newId;
            }

            return idScope;
        }
    }
}