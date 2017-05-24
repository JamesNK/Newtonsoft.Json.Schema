#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal static class SchemaDiscovery
    {
        private static string UnescapeReference(string reference)
        {
            return Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");
        }

        public static bool FindSchema(Action<JSchema> setSchema, JSchema schema, Uri rootSchemaId, Uri reference, JSchemaReader schemaReader, ref JSchemaDiscovery discovery)
        {
            // todo, better way to get parts from Uri
            string[] parts = reference.ToString().Split('/');

            bool resolvedSchema;

            if (parts.Length > 0 && (parts[0] == "#" || parts[0] == rootSchemaId + "#"))
            {
                schemaReader._schemaStack.Add(schema);

                JSchema parent = schema;
                object current = schema;
                for (int i = 1; i != parts.Length; ++i)
                {
                    string unescapedPart = UnescapeReference(parts[i]);

                    JSchema s = current as JSchema;
                    if (s != null)
                    {
                        schemaReader._schemaStack.Add(s);

                        parent = s;
                        current = GetCurrentFromSchema(s, unescapedPart);
                    }
                    else if (current is JToken)
                    {
                        current = GetCurrentFromToken((JToken) current, unescapedPart);
                    }
                    else if (current is IDictionary<string, JSchema>)
                    {
                        IDictionary<string, JSchema> d = (IDictionary<string, JSchema>) current;

                        JSchema temp;
                        d.TryGetValue(unescapedPart, out temp);
                        current = temp;
                    }
                    else if (current is IList<JSchema>)
                    {
                        IList<JSchema> l = (IList<JSchema>) current;
                        int index;

                        JSchema itemsSchema;
                        if (TryGetImplicitItemsSchema(parent, l, out itemsSchema))
                        {
                            current = GetCurrentFromSchema(itemsSchema, unescapedPart);
                        }
                        else if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out index))
                        {
                            if (index > l.Count || index < 0)
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
                    }
                    else
                    {
                        break;
                    }
                }

                IList<JSchema> itemsSchemas = current as IList<JSchema>;
                if (itemsSchemas != null)
                {
                    JSchema itemsSchema;
                    if (TryGetImplicitItemsSchema(parent, itemsSchemas, out itemsSchema))
                    {
                        current = itemsSchema;
                    }
                }

                switch (current)
                {
                    case JToken t:
                        JSchemaAnnotation annotation = t.Annotation<JSchemaAnnotation>();
                        if (annotation != null)
                        {
                            setSchema(annotation.Schema);
                            resolvedSchema = true;
                        }
                        else
                        {
                            JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema, t);

                            string path = reference.OriginalString;
                            if (path.StartsWith("#/", StringComparison.Ordinal))
                            {
                                path = path.Substring(2, path.Length - 2);
                            }

                            discovery.Discover(inlineSchema, rootSchemaId, path);

                            resolvedSchema = true;
                        }
                        break;
                    case JSchema s:
                        setSchema(s);
                        resolvedSchema = true;

                        // schema is a reference schema and needs to be resolved
                        if (s.Reference != null)
                        {
                            schemaReader.AddDeferedSchema(null, setSchema, s);
                        }
                        break;
                    default:
                        resolvedSchema = false;
                        break;
                }

                schemaReader._schemaStack.Clear();
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
                    int hashIndex = resolvedReference.OriginalString.IndexOf('#');
                    if (hashIndex != -1)
                    {
                        Uri path = new Uri(resolvedReference.OriginalString.Substring(0, hashIndex), UriKind.RelativeOrAbsolute);
                        Uri fragment = new Uri(resolvedReference.OriginalString.Substring(hashIndex), UriKind.RelativeOrAbsolute);

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
                                resolvedSchema = FindSchema(setSchema, knownSchema.Schema, path, fragment, schemaReader, ref discovery);
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
                        // special case
                        // look in the root schema's definitions for a definition with the same property name and id as reference
                        JToken definitions;
                        if (schema.ExtensionData.TryGetValue(Constants.PropertyNames.Definitions, out definitions))
                        {
                            JObject definitionsObject = definitions as JObject;
                            if (definitionsObject != null)
                            {
                                JProperty matchingProperty = definitionsObject.Properties().FirstOrDefault(p => TryCompare(p.Name, resolvedReference));

                                JObject o = matchingProperty?.Value as JObject;
                                if (o != null && TryCompare((string) o["id"], resolvedReference))
                                {
                                    JSchema inlineSchema = schemaReader.ReadInlineSchema(setSchema, o);

                                    discovery.Discover(inlineSchema, rootSchemaId, Constants.PropertyNames.Definitions + "/" + resolvedReference.OriginalString);

                                    resolvedSchema = true;
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
                    }
                }
            }

            return resolvedSchema;
        }

        private static bool TryGetImplicitItemsSchema(JSchema parent, IList<JSchema> items, out JSchema schema)
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

        private static object GetCurrentFromSchema(JSchema s, string unescapedPart)
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

        private static object GetCurrentFromToken(JToken t, string unescapedPart)
        {
            JToken resolvedToken;

            if (t is JObject)
            {
                resolvedToken = t[unescapedPart];
            }
            else if (t is JArray || t is JConstructor)
            {
                int index;
                if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out index))
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

            JSchemaAnnotation annotation = resolvedToken.Annotation<JSchemaAnnotation>();
            if (annotation != null)
            {
                return annotation.Schema;
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

            Uri definitionUri;
            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out definitionUri))
            {
                return UriComparer.Instance.Equals(definitionUri, resolvedReference);
            }

            return false;
        }

        public static Uri ResolveSchemaId(Uri idScope, Uri schemaId)
        {
            if (idScope == null || schemaId.IsAbsoluteUri)
            {
                idScope = schemaId;
            }
            else
            {
                Uri tempRoot;
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

                Uri newId;

                try
                {
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