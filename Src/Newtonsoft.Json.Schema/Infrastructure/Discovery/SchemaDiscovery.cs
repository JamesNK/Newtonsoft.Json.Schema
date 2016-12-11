#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
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

        public static bool FindSchema(Action<JSchema> setSchema, JSchema schema, Uri rootSchemaId, Uri reference, JSchemaReader schemaReader, ref JSchemaDiscovery discovery)
        {
            // todo, better way to get parts from Uri
            string[] parts = reference.ToString().Split('/');

            bool resolvedSchema;

            if (parts.Length > 0 && (parts[0] == "#" || parts[0] == rootSchemaId + "#"))
            {
                schemaReader._schemaStack.Push(schema);

                parts = parts.Skip(1).ToArray();

                object current = schema;
                foreach (string part in parts)
                {
                    string unescapedPart = UnescapeReference(part);

                    if (current is JSchema)
                    {
                        JSchema s = current as JSchema;

                        schemaReader._schemaStack.Push(s);

                        switch (unescapedPart)
                        {
                            case Constants.PropertyNames.Properties:
                                current = s._properties;
                                break;
                            case Constants.PropertyNames.Items:
                                current = s._items;
                                break;
                            case Constants.PropertyNames.AdditionalProperties:
                                current = s.AdditionalProperties;
                                break;
                            case Constants.PropertyNames.AdditionalItems:
                                current = s.AdditionalItems;
                                break;
                            case Constants.PropertyNames.Not:
                                current = s.Not;
                                break;
                            case Constants.PropertyNames.OneOf:
                                current = s._oneOf;
                                break;
                            case Constants.PropertyNames.AllOf:
                                current = s._allOf;
                                break;
                            case Constants.PropertyNames.AnyOf:
                                current = s._anyOf;
                                break;
                            case Constants.PropertyNames.Enum:
                                current = s._enum;
                                break;
                            case Constants.PropertyNames.PatternProperties:
                                current = s._patternProperties;
                                break;
                            case Constants.PropertyNames.Dependencies:
                                current = s._dependencies;
                                break;
                            default:
                                JToken t;
                                s.ExtensionData.TryGetValue(unescapedPart, out t);
                                current = t;
                                break;
                        }
                    }
                    else if (current is JToken)
                    {
                        JToken resolvedToken;

                        JToken t = (JToken)current;
                        if (t is JObject)
                        {
                            resolvedToken = t[unescapedPart];
                        }
                        else if (t is JArray || t is JConstructor)
                        {
                            int index;
                            if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out index))
                            {
                                if (index > t.Count() || index < 0)
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

                        if (resolvedToken != null)
                        {
                            JSchemaAnnotation annotation = resolvedToken.Annotation<JSchemaAnnotation>();
                            if (annotation != null)
                            {
                                current = annotation.Schema;
                            }
                            else
                            {
                                current = resolvedToken;
                            }
                        }
                        else
                        {
                            current = null;
                        }
                    }
                    else if (current is IDictionary)
                    {
                        IDictionary d = (IDictionary)current;

                        current = d[unescapedPart];
                    }
                    else if (current is IList)
                    {
                        IList l = (IList)current;

                        int index;
                        if (int.TryParse(unescapedPart, NumberStyles.None, CultureInfo.InvariantCulture, out index))
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

                if (current is JToken)
                {
                    JToken t = (JToken)current;

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
                }
                else
                {
                    JSchema s = current as JSchema;
                    if (s != null)
                    {
                        setSchema(s);
                        resolvedSchema = true;

                        // schema is a reference schema and needs to be resolved
                        if (s.Reference != null)
                        {
                            schemaReader.AddDeferedSchema(null, setSchema, s);
                        }
                    }
                    else
                    {
                        resolvedSchema = false;
                    }
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
                                if (o != null && TryCompare((string)o["id"], resolvedReference))
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

                Uri newId = new Uri(resolvedId, schemaId);

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