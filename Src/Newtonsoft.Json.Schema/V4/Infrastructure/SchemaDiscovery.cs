using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema.V4.Infrastructure
{
    internal static class SchemaDiscovery
    {
        private static string UnescapeReference(string reference)
        {
            return Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");
        }

        public static JSchema4 FindSchema(JSchema4 schema, Uri rootSchemaId, Uri reference, JSchema4Reader schemaReader)
        {
            // todo, better way to get parts from Uri
            string[] parts = reference.ToString().Split('/');

            JSchema4 resolvedSchema;

            if (parts.Length > 0 && parts[0] == "#")
            {
                parts = parts.Skip(1).ToArray();

                object current = schema;
                foreach (string part in parts)
                {
                    string unescapedPart = UnescapeReference(part);

                    if (current is JSchema4)
                    {
                        JSchema4 s = current as JSchema4;

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
                        resolvedSchema = annotation.Schema;
                    }
                    else
                    {
                        JsonReader reader = t.CreateReader();
                        reader.Read();

                        resolvedSchema = new JSchema4();
                        t.AddAnnotation(new JSchemaAnnotation(resolvedSchema));

                        schemaReader.ReadInlineSchema(reader, resolvedSchema);   
                    }
                }
                else
                {
                    resolvedSchema = current as JSchema4;
                }
            }
            else
            {
                JSchema4Discovery discovery = new JSchema4Discovery();
                discovery.Discover(schema, null);

                Uri resolvedReference = ResolveSchemaId(rootSchemaId, reference);

                KnownSchema knownSchema = discovery.KnownSchemas.SingleOrDefault(s => s.Id == resolvedReference);

                if (knownSchema != null)
                    resolvedSchema = knownSchema.Schema;
                else
                    resolvedSchema = null;
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
