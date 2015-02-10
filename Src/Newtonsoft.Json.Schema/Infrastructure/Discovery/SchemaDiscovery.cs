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

        public static bool FindSchema(Action<JSchema> setSchema, JSchema schema, Uri rootSchemaId, Uri reference, JSchemaReader schemaReader)
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
                                    resolvedToken = null;
                                else
                                    resolvedToken = t[index];
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
                                current = annotation.Schema;
                            else
                                current = resolvedToken;
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
                                current = null;
                            else
                                current = l[index];
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
                        schemaReader.ReadInlineSchema(setSchema, t);
                        resolvedSchema = true;
                    }
                }
                else
                {
                    var s = current as JSchema;
                    if (s != null)
                    {
                        setSchema(s);
                        resolvedSchema = true;

                        // schema is a reference schema and needs to be resolved
                        if (s.Reference != null)
                            schemaReader.AddDeferedSchema(setSchema, s);
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
                JSchemaDiscovery discovery = new JSchemaDiscovery();
                discovery.Discover(schema, null);

                Uri resolvedReference = ResolveSchemaId(rootSchemaId, reference);

                // default Uri comparison ignores fragments
                KnownSchema knownSchema = discovery.KnownSchemas.SingleOrDefault(s => s.Id.OriginalString.TrimEnd('#') == resolvedReference.OriginalString.TrimEnd('#'));

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

                        // default Uri comparison ignores fragments
                        knownSchema = discovery.KnownSchemas.SingleOrDefault(s => s.Id.OriginalString.TrimEnd('#') == path.OriginalString);

                        if (knownSchema != null)
                            resolvedSchema = FindSchema(setSchema, knownSchema.Schema, path, fragment, schemaReader);
                        else
                            resolvedSchema = false;
                    }
                    else
                    {
                        resolvedSchema = false;
                    }
                }
            }
            return resolvedSchema;
        }

        public static Uri ResolveSchemaIdAndScopeId(Uri idScope, Uri schemaId, string path, out Uri newScope)
        {
            Uri knownSchemaId;
            if (schemaId != null)
            {
                newScope = ResolveSchemaId(idScope, schemaId);

                knownSchemaId = newScope;
            }
            else
            {
                if (idScope == null)
                    knownSchemaId = new Uri(path, UriKind.RelativeOrAbsolute);
                else
                    knownSchemaId = ResolveSchemaId(idScope, new Uri(path, UriKind.RelativeOrAbsolute));

                newScope = idScope;
            }

            return knownSchemaId;
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
