﻿#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal static class SchemaDiscovery
    {
        private static string UnescapeReference(string reference)
        {
            string newReference = Uri.UnescapeDataString(reference);
            newReference = StringHelpers.Replace(newReference, "~1", "/");
            newReference = StringHelpers.Replace(newReference, "~0", "~");

            return newReference;
        }

        private static Uri? GetTokenId(JToken o, JSchemaReader schemaReader, ReferenceType referenceType)
        {
            Uri? id = null;
            if (schemaReader.EnsureVersion(SchemaVersion.Draft6)
                && o[Constants.PropertyNames.Id] is JValue idToken
                && (idToken.Type == JTokenType.String || idToken.Type == JTokenType.Uri))
            {
                id = GetTokenUri(idToken);
            }
            else if (o[Constants.PropertyNames.IdDraft3] is JValue idDraft3Token
                && (idDraft3Token.Type == JTokenType.String || idDraft3Token.Type == JTokenType.Uri))
            {
                id = GetTokenUri(idDraft3Token);
            }

            string? anchor = null;

            // Only use dynamicAnchor when resolving dynamic reference.
            if (schemaReader.EnsureVersion(SchemaVersion.Draft2019_09)
                && o[Constants.PropertyNames.Anchor] is JValue anchorToken
                && anchorToken.Type == JTokenType.String
                && referenceType != ReferenceType.DynamicRef)
            {
                anchor = (string)anchorToken!;
            }
            else if (schemaReader.EnsureVersion(SchemaVersion.Draft2020_12)
                && o[Constants.PropertyNames.DynamicAnchor] is JValue dynamicAnchorToken
                && dynamicAnchorToken.Type == JTokenType.String)
            {
                anchor = (string)dynamicAnchorToken!;
            }

            return CombineIdAndAnchor(id, anchor);
        }

        private static string? GetTokenDynamicAnchor(JToken o, JSchemaReader schemaReader)
        {
            if (schemaReader.EnsureVersion(SchemaVersion.Draft2020_12)
                && o[Constants.PropertyNames.DynamicAnchor] is JValue dynamicAnchor
                && (dynamicAnchor.Type == JTokenType.String))
            {
                return (string)dynamicAnchor!;
            }
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
                    return ResolveSchemaId(id, new Uri("#" + anchor, UriKind.RelativeOrAbsolute));
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

        private struct SplitEnumerator
        {
            private int _currentIndex;
            public string OriginalString { get; }

            public SplitEnumerator(string s, int currentIndex)
            {
                OriginalString = s;
                _currentIndex = currentIndex;
                Current = null!;
            }

            // Needed to be compatible with the foreach operator
            public SplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                if (OriginalString.Length == _currentIndex) // Reach the end of the string
                {
                    return false;
                }

                var index = OriginalString.IndexOf('/', _currentIndex + 1);
                if (index == -1) // The string is composed of only one line
                {
                    Current = OriginalString.Substring(_currentIndex + 1);
                    _currentIndex = OriginalString.Length;
                    return true;
                }

                Current = OriginalString.Substring(_currentIndex + 1, index - _currentIndex - 1);
                _currentIndex = index;
                return true;
            }

            public string Current { get; private set; }
        }

        public static bool FindSchema(FindSchemaContext context)
        {
            bool resolvedSchema;

            string referenceText = context.Reference.ToString();

            if (IsInternalSchemaReference(referenceText, context.RootSchemaId))
            {
                int scopeInitialCount = context.SchemaReader._identifierScopeStack.Count;

                context.SchemaReader.PushIdentifierScope(context.Schema);

                JSchema parent = context.Schema;
                object? current = context.Schema;

                int separatorIndex = StringHelpers.IndexOf(referenceText, '/');
                if (separatorIndex != -1)
                {
                    SplitEnumerator enumerator = new SplitEnumerator(referenceText, separatorIndex);
                    while (enumerator.MoveNext())
                    {
                        string unescapedPart = UnescapeReference(enumerator.Current);

                        switch (current)
                        {
                            case JSchema s:
                                if (s != context.Schema)
                                {
                                    context.SchemaReader.PushIdentifierScope(s);
                                }

                                parent = s;
                                current = GetCurrentFromSchema(context.SchemaReader, s, unescapedPart);
                                break;
                            case JToken t:
                                IIdentifierScope? scope = null;
                                if (t is JObject)
                                {
                                    Uri? id = GetTokenId(t, context.SchemaReader, context.ReferenceType);
                                    if (id != null)
                                    {
                                        string? dynamicAnchor = GetTokenDynamicAnchor(t, context.SchemaReader);
                                        scope = new JsonIdentifierScope(id, false, dynamicAnchor, dynamicAnchor != null);
                                    }
                                }

                                context.SchemaReader.PushIdentifierScope(scope ?? JsonIdentifierScope.Empty);

                                current = GetCurrentFromToken(t, unescapedPart, context.DynamicScope);
                                break;
                            case IDictionary<string, JSchema> d:
                                d.TryGetValue(unescapedPart, out JSchema temp);
                                current = temp;
                                break;
                            case IList<JSchema> l:
                                if (TryGetImplicitItemsSchema(parent, l, out JSchema? itemsSchema))
                                {
                                    current = GetCurrentFromSchema(context.SchemaReader, itemsSchema, unescapedPart);
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
                        JSchema? previousSchema = annotation?.GetSchema(context.DynamicScope);
                        if (previousSchema != null)
                        {
                            context.SetSchema(previousSchema);
                            resolvedSchema = true;
                        }
                        else
                        {
                            JSchema inlineSchema = context.SchemaReader.ReadInlineSchema(context.SetSchema, t, context.DynamicScope);
                            inlineSchema._referencedAs = context.OriginalReference;

                            context.Discovery.Discover(inlineSchema, context.RootSchemaId, dynamicScope: context.DynamicScope);

                            resolvedSchema = true;
                        }
                        break;
                    case JSchema s:
                        context.SetSchema(s);
                        resolvedSchema = true;

                        // schema is a reference schema and needs to be resolved
                        if (s.HasReference)
                        {
                            context.SchemaReader.AddDeferredSchema(null, context.SetSchema, s);
                        }
                        break;
                    default:
                        resolvedSchema = false;
                        break;
                }

                while (context.SchemaReader._identifierScopeStack.Count > scopeInitialCount)
                {
                    context.SchemaReader.PopIdentifierScope();
                }
            }
            else
            {
                context.Discovery.Discover(context.Schema, null, dynamicScope: context.DynamicScope);

                Uri resolvedReference = ResolveSchemaId(context.RootSchemaId, context.Reference);

                KnownSchema? knownSchema = context.Discovery.KnownSchemas.GetById(new KnownSchemaUriKey(resolvedReference, context.DynamicScope, isRoot: false));

                if (knownSchema != null)
                {
                    resolvedSchema = true;
                    context.SetSchema(knownSchema.Schema);
                }
                else
                {
                    if (SplitReference(resolvedReference, out Uri path, out Uri? fragment))
                    {
                        knownSchema = context.Discovery.KnownSchemas.GetById(new KnownSchemaUriKey(path, context.DynamicScope, isRoot: true));

                        if (knownSchema != null)
                        {
                            // don't attempt to find a schema in the same schema again
                            // avoids stackoverflow
                            if (knownSchema.Schema != context.Schema
                                || !UriComparer.Instance.Equals(context.RootSchemaId, path)
                                || !UriComparer.Instance.Equals(context.Reference, fragment))
                            {
                                var knownSchemaContext = context with { Schema = knownSchema.Schema, RootSchemaId = path, Reference = fragment };

                                resolvedSchema = FindSchema(knownSchemaContext);
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
                        resolvedSchema = TryFindSchemaInDefinitions(context, resolvedReference);
                        if (!resolvedSchema)
                        {
                            // TODO: Hack to pass a schema suite test. Putting defs in nested schemas is valid but an odd thing to do.
                            // Should ideally look in every possible nested schema to do this properly.
                            if (context.Schema._allOf != null)
                            {
                                foreach (var item in context.Schema._allOf)
                                {
                                    if (item.ResolvedId == null)
                                    {
                                        FindSchemaContext allOfSchemaContext = context with { Schema = item };
                                        if (TryFindSchemaInDefinitions(allOfSchemaContext, resolvedReference))
                                        {
                                            resolvedSchema = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return resolvedSchema;

            static bool TryFindSchemaInDefinitions(FindSchemaContext context, Uri resolvedReference)
            {
                if (TryFindSchemaInDefinitionsCore(Constants.PropertyNames.Definitions, context, resolvedReference))
                {
                    return true;
                }

                return TryFindSchemaInDefinitionsCore(Constants.PropertyNames.Defs, context, resolvedReference);
            }
        }

        private static bool SplitReference(Uri reference, out Uri path, [NotNullWhen(true)] out Uri? fragment)
        {
            int hashIndex = StringHelpers.IndexOf(reference.OriginalString, '#');
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

        private static bool IsInternalSchemaReference(string reference, Uri? rootSchemaId)
        {
            if (reference.Length > 0)
            {
                if (reference[0] == '#')
                {
                    if (reference.Length == 1)
                    {
                        return true;
                    }
                    if (reference[1] == '/')
                    {
                        return true;
                    }
                }
            }

            if (rootSchemaId != null)
            {
                string id = rootSchemaId.ToString();

                var separatorIndex = StringHelpers.IndexOf(reference, '/');
                var length = separatorIndex == -1 ? reference.Length : separatorIndex;

                if (!id.EndsWith("#", StringComparison.Ordinal))
                {
                    id += "#";
                }

                if (length != id.Length)
                {
                    return false;
                }

                for (int i = 0; i < length; i++)
                {
                    if (reference[i] != id[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private static bool TryFindSchemaInDefinitionsCore(string definitionsName, FindSchemaContext context, Uri resolvedReference)
        {
            // special case
            // look in the root schema's definitions for a definition with the same property name and id as reference
            if (context.Schema.ExtensionData.TryGetValue(definitionsName, out JToken definitions))
            {
                // Add root schema ID to the scope stack. This is required because schemas in definitions may be loaded as
                // fragments when deferred schemas are resolver. If the root schema has an "$id" value, this is need to
                // correctly resolve IDs using it.
                context.SchemaReader.PushIdentifierScope(new JsonIdentifierScope(context.RootSchemaId, false, dynamicAnchor: null, couldBeDynamic: false));

                try
                {
                    if (definitions is JObject definitionsObject)
                    {
                        JProperty? matchingProperty = null;
                        foreach (JProperty property in definitionsObject.Properties())
                        {
                            if (TryCompare(property.Name, resolvedReference))
                            {
                                matchingProperty = property;
                                break;
                            }
                        }
                        if (matchingProperty == null)
                        {
                            foreach (JProperty property in definitionsObject.Properties())
                            {
                                if (TryCompare(property.Name, context.OriginalReference))
                                {
                                    matchingProperty = property;
                                    break;
                                }
                            }
                        }

                        if (matchingProperty?.Value is JObject o)
                        {
                            if (IsIdMatch(resolvedReference, o, context, allowFragmentlessMatch: true))
                            {
                                JSchema inlineSchema = context.SchemaReader.ReadInlineSchema(context.SetSchema, o, context.DynamicScope);

                                context.Discovery.Discover(inlineSchema, context.RootSchemaId, definitionsName + "/" + matchingProperty.Name, context.DynamicScope);

                                return true;
                            }
                        }
                        else
                        {
                            SplitReference(resolvedReference, out Uri path, out Uri? fragment);

                            return CheckDefinitionSchemaIds(definitionsName, context, resolvedReference, path, fragment, definitionsObject);
                        }
                    }
                }
                finally
                {
                    context.SchemaReader.PopIdentifierScope();
                }
            }

            return false;
        }

        private static bool CheckDefinitionSchemaIds(string definitionsName, FindSchemaContext context, Uri resolvedReference, Uri matchingId, Uri? matchingFragment, JObject definitionsObject)
        {
            foreach (KeyValuePair<string, JToken?> property in definitionsObject)
            {
                if (property.Value is JObject obj)
                {
                    if (CheckObject(obj, definitionsName + "/" + property.Key, context, resolvedReference, matchingId, matchingFragment, definitionsObject))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckObject(JObject obj, string key, FindSchemaContext context, Uri resolvedReference, Uri matchingId, Uri? matchingFragment, JObject definitionsObject)
        {
            if (IsIdMatch(matchingId, obj, context))
            {
                // Pass in no setSchema here because we want to find a schema using the path first,
                // without setting the schema, and then resolve the fragment using that schema.
                JSchema inlineSchema = context.SchemaReader.ReadInlineSchema(setSchema: null, obj, context.DynamicScope);
                context.Discovery.Discover(inlineSchema, context.RootSchemaId, key, context.DynamicScope);

                if (matchingFragment != null)
                {
                    FindSchemaContext inlineFindContext = context with { Schema = inlineSchema, RootSchemaId = matchingId, Reference = matchingFragment, OriginalReference = matchingFragment };
                    return FindSchema(inlineFindContext);
                }
                else
                {
                    context.SetSchema(inlineSchema);
                }

                return true;
            }
            else if (IsIdMatch(matchingFragment, obj, context) ||
                IsIdMatch(resolvedReference, obj, context))
            {
                JSchema inlineSchema = context.SchemaReader.ReadInlineSchema(context.SetSchema, obj, context.DynamicScope);
                context.Discovery.Discover(inlineSchema, context.RootSchemaId, key, context.DynamicScope);

                return true;
            }
            else
            {
                Uri? id = GetTokenId(obj, context.SchemaReader, context.ReferenceType);

                Uri? resolvedId = id != null
                    ? ResolveSchemaId(context.RootSchemaId, id)
                    : context.RootSchemaId;

                FindSchemaContext contextWithResolvedId = context with { RootSchemaId = resolvedId };

                // This is a hack so one spec test passes.
                // A complete fix would be to visit all properties that could contain a schema in JSON and check whether it matches the desired ID.
                if (obj[Constants.PropertyNames.Not] is JObject o)
                {
                    if (CheckObject(o, Constants.PropertyNames.Not, contextWithResolvedId, resolvedReference, matchingId, matchingFragment, definitionsObject))
                    {
                        return true;
                    }
                }

                // Definition object doesn't match but one of its nested definitions might
                // Recurse into nested definitions using the current definition scope
                if (IsNestedDefinitionMatch(Constants.PropertyNames.Definitions, contextWithResolvedId, resolvedReference, matchingId, matchingFragment, obj))
                {
                    return true;
                }
                if (IsNestedDefinitionMatch(Constants.PropertyNames.Defs, contextWithResolvedId, resolvedReference, matchingId, matchingFragment, obj))
                {
                    return true;
                }

                return false;
            }
        }

        private static bool IsNestedDefinitionMatch(string definitionsName, FindSchemaContext context, Uri resolvedReference, Uri matchingId, Uri? matchingFragment, JObject obj)
        {
            JObject? nestedDefinitions = obj[definitionsName] as JObject;
            if (nestedDefinitions != null)
            {
                return CheckDefinitionSchemaIds(definitionsName, context, resolvedReference, matchingId, matchingFragment, nestedDefinitions);
            }

            return false;
        }

        private static bool IsIdMatch(Uri? resolvedReference, JObject o, FindSchemaContext context, bool allowFragmentlessMatch = false)
        {
            Uri? id = GetTokenId(o, context.SchemaReader, context.ReferenceType);

            if (id == null)
            {
                return false;
            }

            if (MatchIdCore(id, resolvedReference, allowFragmentlessMatch))
            {
                return true;
            }

            Uri resolvedId = ResolveSchemaId(context.RootSchemaId, id);
            if (MatchIdCore(resolvedId, resolvedReference, allowFragmentlessMatch))
            {
                return true;
            }

            if (context.RootSchemaId == null)
            {
                // This could remove leading / and ./ before comparison.
                resolvedId = ResolveSchemaId(TempHash, id);
                if (MatchIdCore(resolvedId, resolvedReference, allowFragmentlessMatch))
                {
                    return true;
                }
            }

            return false;

            static bool MatchIdCore(Uri resolvedId, Uri? resolvedReference, bool allowFragmentlessMatch)
            {
                if (UriComparer.Instance.Equals(resolvedId, resolvedReference))
                {
                    return true;
                }

                // If the resolver reference doesn't have an anchor and the token ID does, then repeat check but ignore token ID's fragment.
                if (allowFragmentlessMatch &&
                    resolvedReference != null &&
                    !JSchemaDiscovery.HasFragment(resolvedReference) &&
                    JSchemaDiscovery.HasFragment(resolvedId) &&
                    resolvedId == resolvedReference)
                {
                    return true;
                }

                return false;
            }
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

        private static object? GetCurrentFromSchema(JSchemaReader reader, JSchema s, string unescapedPart)
        {
            switch (unescapedPart)
            {
                case Constants.PropertyNames.PrefixItems:
                    if (reader.EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
                    {
                        goto default;
                    }
                    else
                    {
                        return s._items;
                    }
                case Constants.PropertyNames.Properties:
                    return s._properties;
                case Constants.PropertyNames.Items:
                    if (reader.EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
                    {
                        return s._items;
                    }
                    else
                    {
                        return s.AdditionalItems;
                    }
                case Constants.PropertyNames.AdditionalProperties:
                    return s.AdditionalProperties;
                case Constants.PropertyNames.AdditionalItems:
                    if (reader.EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
                    {
                        return s.AdditionalItems;
                    }
                    else
                    {
                        goto default;
                    }
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
                    if (index >= ((JContainer) t).Count || index < 0)
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

        private static readonly Uri TempRoot = new Uri("http://localhost/");
        private static readonly Uri TempHash = new Uri("#", UriKind.RelativeOrAbsolute);

        public static Uri ResolveSchemaId(Uri? idScope, Uri schemaId)
        {
            if (idScope == null || schemaId.IsAbsoluteUri)
            {
                return schemaId;
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(idScope.OriginalString))
                    {
                        return schemaId;
                    }

                    if (idScope.IsAbsoluteUri)
                    {
                        return new Uri(idScope, schemaId);
                    }

                    // Unable to combine two Uris when one isn't absolute.
                    Uri tempId = new Uri(new Uri(TempRoot, idScope), schemaId);
                    string relativeId = tempId.OriginalString.Substring(TempRoot.OriginalString.Length);
                    return new Uri(relativeId, UriKind.RelativeOrAbsolute);
                }
                catch (Exception ex)
                {
                    string errorMessage = "Invalid URI error while resolving schema ID. Scope URI: '{0}', schema URI: '{1}'".FormatWith(CultureInfo.InvariantCulture, idScope.OriginalString, schemaId.OriginalString);

                    throw new JSchemaException(errorMessage, ex);
                }
            }
        }
    }

    internal readonly record struct FindSchemaContext(
        Action<JSchema> SetSchema,
        JSchema Schema,
        Uri? RootSchemaId,
        Uri Reference,
        Uri OriginalReference,
        Uri? DynamicScope,
        ReferenceType ReferenceType,
        JSchemaReader SchemaReader,
        JSchemaDiscovery Discovery);
}