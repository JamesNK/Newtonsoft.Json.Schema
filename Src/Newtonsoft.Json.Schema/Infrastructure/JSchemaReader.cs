#region License

// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#if HAVE_BIG_INTEGER
using System.Numerics;
#endif
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Collections;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal sealed class JSchemaReader
    {
        private static readonly List<JsonToken> ValidLongTokens = new List<JsonToken> { JsonToken.Integer, JsonToken.Float };

        internal JSchemaDiscovery _schemaDiscovery;
        internal readonly IdentifierScopeStack _identifierScopeStack;
        private readonly DeferredSchemaCollection _deferredSchemas;
        private readonly DeferredSchemaCollection _resolvedDeferredSchemas;
        private readonly JSchemaResolver _resolver;
        private readonly Uri? _baseUri;
        private readonly bool _validateSchema;
        private readonly SchemaValidationEventHandler? _validationEventHandler;
        private readonly List<ValidationError>? _validationErrors;
        private readonly IList<JsonValidator>? _validators;
        private readonly bool _resolveSchemaReferences;

        private Uri? _versionUri;
        private SchemaVersion _version;
        private JSchema? _validatingSchema;
        private bool _isReentrant;

        public JSchema RootSchema { get; set; } = default!;
        public Dictionary<Uri, JSchema> Cache { get; set; }
        public JSchemaReader? Parent { get; private set; }

        public JSchemaReader(JSchemaResolver resolver)
            : this(new JSchemaReaderSettings { Resolver = resolver })
        {
        }

        public JSchemaReader(JSchemaReaderSettings settings)
        {
            Cache = new Dictionary<Uri, JSchema>(UriComparer.Instance);
            _deferredSchemas = new DeferredSchemaCollection();
            _resolvedDeferredSchemas = new DeferredSchemaCollection();
            _identifierScopeStack = new IdentifierScopeStack();

            _resolver = settings.Resolver ?? JSchemaDummyResolver.Instance;
            _baseUri = settings.BaseUri;
            _validateSchema = settings.ValidateVersion;
            _schemaDiscovery = new JSchemaDiscovery(null);
            _validationEventHandler = settings.GetValidationEventHandler();
            _validators = settings.Validators;
            _resolveSchemaReferences = settings.ResolveSchemaReferences;

            if (_validationEventHandler != null)
            {
                _validationErrors = new List<ValidationError>();
                _schemaDiscovery.ValidationErrors = _validationErrors;
            }
        }

        internal List<IIdentifierScope> GetAndClearScopes()
        {
            var scopes = _identifierScopeStack.ToList();
            _identifierScopeStack.Clear();
            return scopes;
        }

        internal void PushIdentifierScope(IIdentifierScope identifierScope)
        {
            _identifierScopeStack.Add(identifierScope);
        }

        internal void PopIdentifierScope()
        {
            _identifierScopeStack.RemoveAt(_identifierScopeStack.Count - 1);
        }

        internal JSchema ReadRoot(JsonReader reader)
        {
            return ReadRoot(reader, _resolveSchemaReferences);
        }

        internal JSchema ReadRoot(JsonReader reader, bool resolveDeferredSchemas)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error reading schema from JsonReader.");
                }
            }

            ValidateSchemaStart(reader, null);

            RootSchema = new JSchema();

            LoadAndSetSchema(reader, null, s =>
            {
                RootSchema = s;

                if (s.BaseUri != null)
                {
                    s.InternalReader = this;
                    Cache[s.BaseUri] = s;
                }
            }, true);

            if (resolveDeferredSchemas)
            {
                ResolveDeferredSchemas();
            }

            RaiseValidationErrors();

            return RootSchema;
        }

        public void RaiseValidationErrors()
        {
            if (_validationErrors != null)
            {
                _schemaDiscovery.Discover(RootSchema, null);

                foreach (ValidationError error in _validationErrors)
                {
                    // Get first match because the schema could have been added multiple times with duplicate IDs.
                    KnownSchema knownSchema = _schemaDiscovery.KnownSchemas.FirstOrDefault(s => s.Schema == error.Schema);
                    if (knownSchema != null)
                    {
                        error.SchemaId = knownSchema.Id;
                    }

                    ValidationUtils.Assert(_validationEventHandler != null);
                    _validationEventHandler(this, new SchemaValidationEventArgs(error));
                }

                _validationErrors.Clear();
            }
        }

        internal bool EnsureVersion(SchemaVersion minimum, SchemaVersion? maximum = null)
        {
            return SchemaVersionHelpers.EnsureVersion(_version, minimum, maximum);
        }

        internal JSchema ReadInlineSchema(Action<JSchema>? setSchema, JToken inlineToken, Uri? dynamicScope)
        {
            JTokenPathAnnotation? pathAnnotation = inlineToken.Root.Annotation<JTokenPathAnnotation>();
            string path;
            if (!string.IsNullOrEmpty(pathAnnotation?.BasePath))
            {
                path = pathAnnotation!.BasePath;

                if (!string.IsNullOrEmpty(inlineToken.Path))
                {
                    if (!inlineToken.Path.StartsWith("[", StringComparison.Ordinal))
                    {
                        path += ".";
                    }

                    path += inlineToken.Path;
                }
            }
            else
            {
                path = inlineToken.Path;
            }

            JsonReader reader = new JTokenReader(inlineToken, path);

            if (_validatingSchema != null)
            {
                JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
                validatingReader.Schema = _validatingSchema;
                validatingReader.ValidationEventHandler += RaiseSchemaValidationError;

                reader = validatingReader;
            }

            reader.Read();

            LoadAndSetSchema(reader, null, s =>
            {
                s.DynamicLoadScope = dynamicScope;

                setSchema?.Invoke(s);
                JSchemaAnnotation? annotation = inlineToken.Annotation<JSchemaAnnotation>();
                if (annotation == null)
                {
                    annotation = new JSchemaAnnotation();
                    inlineToken.AddAnnotation(annotation);
                }
                annotation.RegisterSchema(dynamicScope, s);
            });

            return inlineToken.Annotation<JSchemaAnnotation>()!.GetSchema(dynamicScope)!;
        }

        internal bool AddDeferredSchema(JSchema? target, Action<JSchema> setSchema, JSchema referenceSchema)
        {
            Uri? scopeId = ResolveScopeId(out string? scopeDynamicAnchor, out bool couldBeDynamic, out Uri? dynamicScope);
            Uri resolvedReference = ResolveSchemaReference(scopeId, referenceSchema);

            ResolveReferenceFromSchema(referenceSchema, scopeDynamicAnchor, out ReferenceType referenceType, out Uri originalReference);
            return AddDeferredSchema(resolvedReference, originalReference, scopeId, scopeDynamicAnchor, couldBeDynamic, referenceType, dynamicScope, referenceSchema, target, setSchema);
        }

        private static void ResolveReferenceFromSchema(JSchema referenceSchema, string? scopeDynamicAnchor, out ReferenceType referenceType, out Uri originalReference)
        {
            if (referenceSchema.Reference != null)
            {
                referenceType = ReferenceType.Ref;
                originalReference = referenceSchema.Reference;
            }
            else if (referenceSchema.DynamicReference != null)
            {
                referenceType = ReferenceType.DynamicRef;
                originalReference = referenceSchema.DynamicReference;
            }
            else if (referenceSchema.RecursiveReference != null)
            {
                // If there is no recursive/dynamic anchor then the reference is treated as a standard $ref
                referenceType = scopeDynamicAnchor != null ? ReferenceType.RecursiveRef : ReferenceType.Ref;
                originalReference = referenceSchema.RecursiveReference;
            }
            else
            {
                throw new InvalidOperationException("Couldn't find a reference on schema.");
            }
        }

        private void ReadSchemaProperties(JsonReader reader, JSchema target, bool isRoot)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        ProcessSchemaName(ref reader, target, isRoot, (string)reader.Value!);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading schema: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
        }

        public void ResolveDeferredSchemas()
        {
            if (_isReentrant)
            {
                return;
            }

            _isReentrant = true;

            if (_deferredSchemas.Count > 0)
            {
                // note that deferred schemas could be added while resolving
                while (_deferredSchemas.Count > 0)
                {
                    DeferredSchema deferredSchema = _deferredSchemas[0];

                    if (!TryResolveDeferredSchema(this, deferredSchema))
                    {
                        throw JSchemaReaderException.Create(deferredSchema.ReferenceSchema, _baseUri, deferredSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferredSchema.ResolvedReference));
                    }

                    DeferredSchemaKey deferredSchemaKey = DeferredSchema.CreateKey(deferredSchema);

                    if (_resolvedDeferredSchemas.TryGetValue(deferredSchemaKey, out DeferredSchema? previouslyResolvedSchema))
                    {
                        if (!deferredSchema.Success)
                        {
                            throw JSchemaReaderException.Create(previouslyResolvedSchema.ReferenceSchema, _baseUri, previouslyResolvedSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferredSchema.ResolvedReference));
                        }
                        else
                        {
                            _resolvedDeferredSchemas.Remove(previouslyResolvedSchema);
                        }
                    }

                    _resolvedDeferredSchemas.Add(deferredSchema);
                    _deferredSchemas.Remove(deferredSchema);
                }

                foreach (DeferredSchema resolvedDeferredSchema in _resolvedDeferredSchemas)
                {
                    if (!resolvedDeferredSchema.Success)
                    {
                        throw JSchemaReaderException.Create(resolvedDeferredSchema.ReferenceSchema, _baseUri, resolvedDeferredSchema.ReferenceSchema.Path, "Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, resolvedDeferredSchema.ResolvedReference));
                    }
                }
            }

            _isReentrant = false;
        }

        private bool TryResolveDeferredSchema(JSchemaReader resolvingReader, DeferredSchema deferredSchema)
        {
            switch (deferredSchema.ReferenceType)
            {
                case ReferenceType.Ref:
                    return TryResolveDeferredSchemaWithScope(resolvingReader, deferredSchema);
                case ReferenceType.RecursiveRef:
                    if (deferredSchema.ReferenceType == ReferenceType.RecursiveRef)
                    {
                        while (((IIdentifierScope)resolvingReader.RootSchema).DynamicAnchor == deferredSchema.DynamicAnchor
                            && resolvingReader.Parent != null
                            && ((IIdentifierScope)resolvingReader.Parent.RootSchema).DynamicAnchor == deferredSchema.DynamicAnchor)
                        {
                            resolvingReader = resolvingReader.Parent;
                        }
                    }

                    return TryResolveDeferredSchemaWithScope(resolvingReader, deferredSchema);
                case ReferenceType.DynamicRef:
                    // Initial resolve is standard $ref lookup.
                    deferredSchema.Reset(deferredSchema.ResolvedReference, ReferenceType.Ref);
                    if (!TryResolveDeferredSchemaWithScope(resolvingReader, deferredSchema))
                    {
                        return false;
                    }

                    // If the resolved schema has the same dynamic anchor then do dynamic lookup.
                    var resolvedSchema = deferredSchema.ResolvedSchema!;
                    var originalAnchor = new Uri(JSchemaDiscovery.GetFragment(deferredSchema.OriginalReference), UriKind.Relative);
                    if (resolvedSchema.DynamicAnchor == originalAnchor.OriginalString.TrimStart('#'))
                    {
                        // TODO: There might be a situation where the schema isn't available in the initial reader.
                        // If that happens then we'll need to add logic to find the right reader to resolve in.
                        var current = resolvingReader;
                        while (current.Parent != null)
                        {
                            current = current.Parent;
                        }

                        var currentScopes = new List<IIdentifierScope>();
                        foreach (var item in deferredSchema.Scopes)
                        {
                            currentScopes.Add(item);

                            Uri? scopeId = ResolveScopeId(currentScopes, out string? scopeDynamicAnchor, out bool couldBeDynamic, out Uri? dynamicScope);
                            Uri resolvedReference = SchemaDiscovery.ResolveSchemaId(scopeId, originalAnchor);

                            deferredSchema.Reset(resolvedReference, ReferenceType.DynamicRef);

                            if (current.TryResolveDeferredSchemaCore(deferredSchema))
                            {
                                return true;
                            }
                        }
                    }
                    return true;
                default:
                    throw new InvalidOperationException("Unexpeceted reference type: " + deferredSchema.ReferenceType);
            }

            static bool TryResolveDeferredSchemaWithScope(JSchemaReader resolvingReader, DeferredSchema deferredSchema)
            {
                int scopesAdded = 0;
                if (deferredSchema.DynamicAnchor != null)
                {
                    // If the deferred schema has a dynamic anchor then all scopes with that dynamic anchor must be available
                    // Resolving the dynamic anchor starts at the outer most scope.
                    foreach (var scope in deferredSchema.Scopes)
                    {
                        if (scope.DynamicAnchor == deferredSchema.DynamicAnchor)
                        {
                            resolvingReader.PushIdentifierScope(scope);
                            scopesAdded++;
                        }
                    }
                }
                else
                {
                    resolvingReader.PushIdentifierScope(deferredSchema);
                    scopesAdded++;
                }

                try
                {
                    return resolvingReader.TryResolveDeferredSchemaCore(deferredSchema);
                }
                finally
                {
                    for (int i = 0; i < scopesAdded; i++)
                    {
                        resolvingReader.PopIdentifierScope();
                    }
                }
            }
        }

        private bool TryResolveDeferredSchemaCore(DeferredSchema deferredSchema)
        {
            // We don't want to use the dynamic scope if we're resolving the actual recursive reference.
            // It resolves back to the original scopeless object
            Uri? dynamicScope = deferredSchema.DynamicScope;

            FindSchemaContext context = new FindSchemaContext(
                s =>
                {
                    // 2019-09 and later support the $ref property with content.
                    // In earlier versions copy any additional JSON to the referenced schema.
                    // Kind of hacky.
                    if (!EnsureVersion(SchemaVersion.Draft2019_09))
                    {
                        if (s != deferredSchema.ReferenceSchema && deferredSchema.ReferenceSchema._extensionData != null)
                        {
                            foreach (KeyValuePair<string, JToken> keyValuePair in deferredSchema.ReferenceSchema._extensionData)
                            {
                                s.ExtensionData[keyValuePair.Key] = keyValuePair.Value;
                            }
                        }
                    }

                    deferredSchema.SetResolvedSchema(s);
                },
                RootSchema,
                RootSchema.ResolvedId,
                deferredSchema.ResolvedReference,
                deferredSchema.OriginalReference,
                dynamicScope,
                deferredSchema.ReferenceType,
                this,
                _schemaDiscovery);

            bool found = SchemaDiscovery.FindSchema(context);

            if (found)
            {
                return true;
            }

            JSchema? resolvedSchema;
            try
            {
                Uri? reference = GetReference(deferredSchema.ReferenceSchema);
                if (reference == null)
                {
                    throw new InvalidOperationException("The deferred schema's reference schema doesn't have a reference.");
                }
                resolvedSchema = ResolvedSchema(reference, deferredSchema.ResolvedReference);
            }
            catch (Exception ex)
            {
                throw JSchemaReaderException.Create(deferredSchema.ReferenceSchema, _baseUri, deferredSchema.ReferenceSchema.Path, "Error when resolving schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, deferredSchema.ReferenceSchema.Reference), ex);
            }

            if (resolvedSchema != null)
            {
                deferredSchema.SetResolvedSchema(resolvedSchema);
                return true;
            }

            return false;
        }

        private void ReadSchema(JsonReader reader, JSchema target, string name, Action<JSchema> setSchema)
        {
            ValidateSchemaStart(reader, name);

            LoadAndSetSchema(reader, target, setSchema);
        }

        private void ValidateSchemaStart(JsonReader reader, string? name)
        {
            string? errorMessage = (name == null)
                ? "Unexpected token encountered when reading schema. Expected {1}, got {2}."
                : null;

            if (EnsureVersion(SchemaVersion.Draft6))
            {
                if (name != null)
                {
                    EnsureRead(reader, name);
                }

                if (reader.TokenType != JsonToken.Boolean)
                {
                    EnsureToken(reader, name, Constants.SchemaTokens, errorMessage);
                }
            }
            else
            {
                EnsureReadAndToken(reader, name, JsonToken.StartObject, errorMessage);
            }
        }

        private void EnsureRead(JsonReader reader, string? name)
        {
            // read to the next non-comment
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.Comment)
                {
                    return;
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading value for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void EnsureReadAndToken(JsonReader reader, string? name, JsonToken tokenType, string? errorMessage = null)
        {
            EnsureRead(reader, name);

            EnsureToken(reader, name, tokenType, errorMessage);
        }

        private void EnsureToken(JsonReader reader, string? name, JsonToken tokenType, string? errorMessage)
        {
            if (reader.TokenType != tokenType)
            {
                throw JSchemaReaderException.Create(reader, _baseUri, (errorMessage ?? "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.").FormatWith(CultureInfo.InvariantCulture, name, tokenType, reader.TokenType));
            }
        }

        private void EnsureToken(JsonReader reader, string? name, List<JsonToken> tokenTypes, string? errorMessage)
        {
            if (!tokenTypes.Contains(reader.TokenType))
            {
                throw JSchemaReaderException.Create(reader, _baseUri, (errorMessage ?? "Unexpected token encountered when reading value for '{0}'. Expected {1}, got {2}.").FormatWith(CultureInfo.InvariantCulture, name, StringHelpers.Join(", ", tokenTypes), reader.TokenType));
            }
        }

        private void EnsureReadAndToken(JsonReader reader, string name, List<JsonToken> tokenTypes, string? errorMessage = null)
        {
            EnsureRead(reader, name);

            EnsureToken(reader, name, tokenTypes, errorMessage);
        }

        private Uri ReadUri(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.String);
            string id = (string) reader.Value!;
            return ParseUri(reader, id);
        }

        private Uri ParseUri(JsonReader reader, string id)
        {
            try
            {
                return new Uri(id, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing id '{0}'. Id must be a valid URI.".FormatWith(CultureInfo.InvariantCulture, id), ex);
            }
        }

        private string ReadString(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.String);
            return (string) reader.Value!;
        }

        private bool ReadBoolean(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, JsonToken.Boolean);
            return (bool) reader.Value!;
        }

        private long ReadLong(JsonReader reader, string name)
        {
            // A long can be an integer or a float with a zero fraction.
            // https://github.com/json-schema-org/JSON-Schema-Test-Suite/pull/429
            //
            // Accept integer and float values here and then validate there is no fraction below.
            EnsureReadAndToken(reader, name, ValidLongTokens);

#if HAVE_BIG_INTEGER
            if (reader.Value is BigInteger i)
            {
                if (i > long.MaxValue || i < long.MaxValue)
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing integer for '{0}'. {1} cannot fit in an Int64.".FormatWith(CultureInfo.InvariantCulture, name, i));
                }

                return (long) i;
            }
#endif
            if (reader.Value is decimal dec)
            {
                if (dec % 1 != 0)
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing integer for '{0}'. {1} has a non-zero fraction".FormatWith(CultureInfo.InvariantCulture, name, dec));
                }
            }
            else if (reader.Value is double d)
            {
                if (!CompareUtils.ApproxEquals(d % 1, 0))
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Error parsing integer for '{0}'. {1} has a non-zero fraction".FormatWith(CultureInfo.InvariantCulture, name, d));
                }
            }

            return Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture);
        }

        private double ReadDouble(JsonReader reader, string name)
        {
            EnsureReadAndToken(reader, name, Constants.NumberTokens);

            return GetDouble(reader);
        }

        private static double GetDouble(JsonReader reader)
        {
#if HAVE_BIG_INTEGER
            if (reader.Value is BigInteger i)
            {
                return (double) i;
            }
#endif

            return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
        }

        private void ReadProperties(JsonReader reader, JSchema target, IDictionary<string, JSchema> properties)
        {
            EnsureReadAndToken(reader, Constants.PropertyNames.Properties, JsonToken.StartObject);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string) reader.Value!;

                        if (EnsureVersion(SchemaVersion.Draft6))
                        {
                            EnsureReadAndToken(reader, name, Constants.SchemaTokens);
                        }
                        else
                        {
                            EnsureReadAndToken(reader, name, JsonToken.StartObject);
                        }

                        // use last schema for duplicates
                        // will this cause issues with a previously deferred schemas?
                        LoadAndSetSchema(reader, target, s => properties[name] = s);
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading properties: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema properties.");
        }

        private static void SetAtIndex<T>(IList<T> list, int index, T value)
        {
            if (index == list.Count)
            {
                list.Add(value);
            }
            else if (index < list.Count)
            {
                list[index] = value;
            }
            else
            {
                throw new InvalidOperationException("Could not add value to list. Index is greater than the list length.");
            }
        }

        private void ReadItems(JsonReader reader, string name, JSchema target)
        {
            if (EnsureVersion(SchemaVersion.Draft6, SchemaVersion.Draft2019_09))
            {
                EnsureReadAndToken(reader, name, Constants.ItemsDraft6Tokens);
            }
            else if (EnsureVersion(SchemaVersion.Draft2020_12))
            {
                EnsureReadAndToken(reader, name, Constants.ItemsDraft2020_12Tokens);
            }
            else
            {
                EnsureReadAndToken(reader, name, Constants.ItemsDraft4Tokens);
            }

            if (target._items == null)
            {
                target._items = new JSchemaCollection(target);
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                case JsonToken.Boolean:
                    LoadAndSetSchema(reader, target, s => SetAtIndex(target._items, 0, s));
                    target.ItemsPositionValidation = false;
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, target, name, target._items);
                    target.ItemsPositionValidation = true;
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected array or JSON schema object for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Items, reader.TokenType));
            }
        }

        private void ReadSchemaArray(JsonReader reader, JSchema target, string name, out JSchemaCollection schemas)
        {
            EnsureReadAndToken(reader, name, JsonToken.StartArray);

            schemas = new JSchemaCollection(target);

            PopulateSchemaArray(reader, target, name, schemas);
        }

        private void ReadAdditionalItems(JsonReader reader, string name, JSchema target)
        {
            EnsureRead(reader, name);

            if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09) &&
                reader.TokenType == JsonToken.Boolean)
            {
                target.AllowAdditionalItems = (bool) reader.Value!;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.AdditionalItems = s);
            }
        }

        private void ReadUnevaluatedItems(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.UnevaluatedItems);

            if (reader.TokenType == JsonToken.Boolean)
            {
                target.AllowUnevaluatedItems = (bool)reader.Value!;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.UnevaluatedItems = s);
            }
        }

        private void ReadAdditionalProperties(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.AdditionalProperties);

            if (reader.TokenType == JsonToken.Boolean)
            {
                target.AllowAdditionalProperties = (bool) reader.Value!;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.AdditionalProperties = s);
            }
        }

        private void ReadUnevaluatedProperties(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.UnevaluatedProperties);

            if (reader.TokenType == JsonToken.Boolean)
            {
                target.AllowUnevaluatedProperties = (bool)reader.Value!;
            }
            else
            {
                LoadAndSetSchema(reader, target, s => target.UnevaluatedProperties = s);
            }
        }

        private void ReadRequired(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.Required);

            if (reader.TokenType == JsonToken.Boolean)
            {
                if (!EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected StartArray, got Boolean.");
                }

                target.DeprecatedRequired = (bool) reader.Value!;
            }
            else
            {
                if (!EnsureVersion(SchemaVersion.Draft4))
                {
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for 'required'. Expected Boolean, got StartArray.");
                }

                ReadStringArray(reader, Constants.PropertyNames.Required, out target._required);
            }
        }

        private void ReadExtends(JsonReader reader, JSchema target)
        {
            EnsureRead(reader, Constants.PropertyNames.Extends);
            if (target._allOf == null)
            {
                target._allOf = new JSchemaCollection(target);
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    int index = target._allOf.Count;
                    LoadAndSetSchema(reader, target, s => SetAtIndex(target._allOf, index, s));
                    break;
                case JsonToken.StartArray:
                    PopulateSchemaArray(reader, target, Constants.PropertyNames.Extends, target._allOf);
                    break;
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Extends, reader.TokenType));
            }
        }

        private void ReadTokenArray(JsonReader reader, string name, [NotNull] ref List<JToken>? values)
        {
            EnsureReadAndToken(reader, name, JsonToken.StartArray);

            if (values == null)
            {
                values = new List<JToken>();
            }

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        JToken t = JToken.ReadFrom(reader);

                        values.Add(t);
                        break;
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadStringArray(JsonReader reader, string name, out List<string> values)
        {
            values = new List<string>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                        values.Add((string) reader.Value!);
                        break;
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, name, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadObjectOfStringArrays(JsonReader reader, string name, IDictionary<string, IList<string>> result)
        {
            EnsureReadAndToken(reader, name, JsonToken.StartObject);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = (string)reader.Value!;

                        EnsureReadAndToken(reader, propertyName, JsonToken.StartArray);

                        // use last property when duplicates are defined
                        ReadStringArray(reader, propertyName, out List<string> l);
                        result[propertyName] = l;
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, name, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        private void ReadDependencies(JsonReader reader, JSchema target)
        {
            EnsureReadAndToken(reader, Constants.PropertyNames.Dependencies, JsonToken.StartObject);

            JSchemaDependencyDictionary dependencies = new JSchemaDependencyDictionary(target);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string name = (string) reader.Value!;

                        List<JsonToken> validDependencyTokens = EnsureVersion(SchemaVersion.Draft6)
                            ? Constants.DependencyTokens
                            : Constants.DependencyDraft4Tokens;
                        EnsureReadAndToken(reader, name, validDependencyTokens);

                        // use last dependency when duplicates are defined
                        if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Boolean)
                        {
                            LoadAndSetSchema(reader, target, s => dependencies[name] = s);
                        }
                        else if (reader.TokenType == JsonToken.StartArray)
                        {
                            ReadStringArray(reader, name, out List<string> l);
                            dependencies[name] = l;
                        }
                        else
                        {
                            EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3);

                            dependencies[name] = new List<string>
                            {
                                (string) reader.Value!
                            };
                        }
                        break;
                    case JsonToken.Comment:
                        // nom, nom
                        break;
                    case JsonToken.EndObject:
                        target._dependencies = dependencies;
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading dependencies: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading dependencies.");
        }

        private void PopulateSchemaArray(JsonReader reader, JSchema target, string name, IList<JSchema> schemas)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.Boolean:
                        int itemCount = schemas.Count;
                        LoadAndSetSchema(reader, target, s => SetAtIndex(schemas, itemCount, s));
                        break;
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        return;
                    default:
                        throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token when reading schemas: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schemas for '{0}'.".FormatWith(CultureInfo.InvariantCulture, name));
        }

        internal JSchemaType? MapType(JsonReader reader)
        {
            string typeName = (string) reader.Value!;

            if (!Constants.JSchemaTypeMapping.TryGetValue(typeName, out JSchemaType mappedType))
            {
                if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                {
                    if (typeName == Constants.Types.Any)
                    {
                        return null;
                    }
                }

                throw JSchemaReaderException.Create(reader, _baseUri, "Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, typeName));
            }

            return mappedType;
        }

        private object? ReadType(JsonReader reader, JSchema target, string name)
        {
            EnsureRead(reader, name);

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return MapType(reader);
                case JsonToken.StartArray:
                    return ReadTypeArray(reader, target);
                default:
                    throw JSchemaReaderException.Create(reader, _baseUri, "Expected array or string for '{0}', got {1}.".FormatWith(CultureInfo.InvariantCulture, Constants.PropertyNames.Type, reader.TokenType));
            }
        }

        private object? ReadTypeArray(JsonReader reader, JSchema target)
        {
            List<JSchemaType>? types = new List<JSchemaType>();
            JSchemaCollection? typeSchemas = null;
            bool isAny = false;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                        JSchemaType? t = MapType(reader);

                        if (t == null)
                        {
                            isAny = true;
                        }
                        else
                        {
                            if (typeSchemas != null)
                            {
                                typeSchemas.Add(new JSchema { Type = t });
                            }
                            else
                            {
                                types!.Add(t.Value);
                            }
                        }
                        break;
                    case JsonToken.Comment:
                        // nom nom nom
                        break;
                    case JsonToken.EndArray:
                        // type of "any" removes all other type constraints
                        if (isAny)
                        {
                            return null;
                        }

                        if (typeSchemas != null)
                        {
                            return typeSchemas;
                        }

                        JSchemaType finalType = JSchemaType.None;
                        foreach (JSchemaType type in types!)
                        {
                            finalType = finalType | type;
                        }

                        return finalType;
                    default:
                        if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                        {
                            if (typeSchemas == null)
                            {
                                typeSchemas = new JSchemaCollection(target);
                                foreach (JSchemaType type in types!)
                                {
                                    typeSchemas.Add(new JSchema
                                    {
                                        Type = type
                                    });
                                }
                                types = null;
                            }
                            int count = typeSchemas.Count;
                            JSchemaCollection l = typeSchemas;
                            LoadAndSetSchema(reader, target, s => SetAtIndex(l, count, s));
                        }
                        else
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "Expected string token for type, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }
                        break;
                }
            }

            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema type.");
        }

        private void LoadAndSetSchema(JsonReader reader, JSchema? target, Action<JSchema> setSchema, bool isRoot = false)
        {
            // check whether a schema for this token has already been loaded
            if (reader is JTokenReader tokenReader)
            {
                JSchemaAnnotation? schemaAnnotation = tokenReader.CurrentToken!.Annotation<JSchemaAnnotation>();
                JSchema? previousSchema = schemaAnnotation?.GetSchema(null);
                if (previousSchema != null)
                {
                    setSchema(previousSchema);
                    tokenReader.Skip();
                    return;
                }
            }

            JSchema loadingSchema = new JSchema();
            loadingSchema.State = JSchemaState.Loading;
            loadingSchema.BaseUri = _baseUri;
            loadingSchema.Root = isRoot;

            if (reader is IJsonLineInfo lineInfo)
            {
                loadingSchema.SetLineInfo(lineInfo);
            }
            loadingSchema.Path = reader.Path;

            PushIdentifierScope(loadingSchema);

            try
            {
                if (reader.TokenType == JsonToken.Boolean)
                {
                    loadingSchema.Valid = (bool)reader.Value!;
                }
                else
                {
                    ReadSchemaProperties(reader, loadingSchema, isRoot);

                    if (!EnsureVersion(SchemaVersion.Draft2019_09))
                    {
                        // In older schema versions a $ref value means sibling id has no effect.
                        if (loadingSchema.HasReference && loadingSchema.Id != null)
                        {
                            loadingSchema.Id = null;
                        }
                    }
                }

                if (loadingSchema.HasReference)
                {
                    Uri? scopeId = ResolveScopeId(out string? scopeDynamicAnchor, out bool couldBeDynamic, out Uri? dynamicScope);
                    ResolveReferenceFromSchema(loadingSchema, scopeDynamicAnchor, out ReferenceType referenceType, out Uri originalReference);

                    if (referenceType == ReferenceType.RecursiveRef)
                    {
                        // Outer most matching dynamic anchor in scope
                        for (int i = 0; i < _identifierScopeStack.Count; i++)
                        {
                            IIdentifierScope current = _identifierScopeStack[i];
                            if (current.ScopeId != null && current.DynamicAnchor == scopeDynamicAnchor)
                            {
                                scopeId = current.ScopeId;
                                break;
                            }
                        }
                    }

                    Uri resolvedReference = ResolveSchemaReference(scopeId, loadingSchema);

                    if (AddDeferredSchema(resolvedReference, originalReference, scopeId, scopeDynamicAnchor, couldBeDynamic, referenceType, dynamicScope, loadingSchema, target, setSchema))
                    {
                        return;
                    }
                }
                else
                {
                    if (_validators != null)
                    {
                        foreach (JsonValidator validator in _validators)
                        {
                            if (validator.CanValidate(loadingSchema))
                            {
                                loadingSchema.Validators.Add(validator);
                            }
                        }
                    }
                }

                loadingSchema.State = JSchemaState.Default;

                setSchema(loadingSchema);
            }
            finally
            {
                PopIdentifierScope();
            }
        }

        private bool AddDeferredSchema(Uri resolvedReference, Uri originalReference, Uri? scopeId, string? dynamicAnchor, bool couldBeDynamic, ReferenceType referenceType, Uri? dynamicScope, JSchema referenceSchema, JSchema? target, Action<JSchema> setSchema)
        {
            DeferredSchemaKey deferredSchemaKey = new DeferredSchemaKey(resolvedReference, dynamicScope);

            if (_resolvedDeferredSchemas.TryGetValue(deferredSchemaKey, out DeferredSchema? deferredSchema))
            {
                // schema has already been successfully resolved
                if (deferredSchema.Success)
                {
                    JSchema resolvedSchema = deferredSchema.ResolvedSchema;
                    if (referenceSchema.HasNonRefContent && EnsureVersion(SchemaVersion.Draft2019_09))
                    {
                        referenceSchema.Ref = resolvedSchema;
                        referenceSchema.Reference = null;
                        referenceSchema.RecursiveReference = null;
                        referenceSchema.DynamicReference = null;

                        resolvedSchema = referenceSchema;
                    }

                    setSchema(resolvedSchema);
                    return true;
                }
                else
                {
                    deferredSchema.AddSchemaSet(setSchema, target);
                    return false;
                }
            }
            else
            {
                if (!_deferredSchemas.TryGetValue(deferredSchemaKey, out deferredSchema))
                {
                    bool supportsRef = EnsureVersion(SchemaVersion.Draft2019_09);

                    deferredSchema = new DeferredSchema(resolvedReference, originalReference, scopeId, dynamicAnchor, couldBeDynamic, dynamicScope, referenceType, referenceSchema, supportsRef, _identifierScopeStack.Clone());
                    _deferredSchemas.Add(deferredSchema);
                }

                Action<JSchema> updatedSetSchema = s =>
                {
                    SetResolvedSchema(referenceSchema, setSchema, s, deferredSchema);
                };

                deferredSchema.AddSchemaSet(updatedSetSchema, target);
                return false;
            }
        }

        internal void SetResolvedSchema(JSchema referenceSchema, Action<JSchema> setSchema, JSchema resolvedSchema, DeferredSchema deferredSchema)
        {
            bool supportsRef = EnsureVersion(SchemaVersion.Draft2019_09);
            if (supportsRef && referenceSchema.HasReference && referenceSchema.HasNonRefContent)
            {
                referenceSchema.Ref = resolvedSchema;
                referenceSchema.Reference = null;
                referenceSchema.RecursiveReference = null;
                referenceSchema.DynamicReference = null;
            }
            else if (deferredSchema.ReferenceType == ReferenceType.DynamicRef && referenceSchema.Ref != null)
            {
                // Setting a dynamicRef could happen multiple times.
                // There is an initial match in the current scope which will clear the reference.
                // Then an optional match could happen if a dynamicAnchor is found in an outer scope.
                // Because the reference is cleared in the initial match, extra logic is required to correctly set the extra match.
                referenceSchema.Ref = resolvedSchema;
            }
            else
            {
                setSchema(resolvedSchema);
            }
        }

        private JSchema? ResolvedSchema(Uri schemaId, Uri resolvedSchemaId)
        {
            ResolveSchemaContext context = new ResolveSchemaContext
            {
                ResolverBaseUri = _baseUri,
                SchemaId = schemaId,
                ResolvedSchemaId = resolvedSchemaId
            };

            SchemaReference schemaReference = _resolver.ResolveSchemaReference(context);

            if (schemaReference.BaseUri == _baseUri && schemaReference.SubschemaId != null)
            {
                // reference is to inside the current schema
                // use normal schema resolution
                return null;
            }

            ValidationUtils.Assert(schemaReference.BaseUri != null);

            if (Cache.ContainsKey(schemaReference.BaseUri))
            {
                // base URI has already been resolved
                // use previously retrieved schema
                JSchema cachedSchema = Cache[schemaReference.BaseUri];
                return _resolver.GetSubschema(schemaReference, cachedSchema);
            }

            Stream? schemaData = _resolver.GetSchemaResource(context, schemaReference);

            if (schemaData == null)
            {
                // resolver returned no data
                return null;
            }

            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                BaseUri = schemaReference.BaseUri,
                Resolver = _resolver,
                ValidateVersion = _validateSchema,
                Validators = _validators,
                ResolveSchemaReferences = _resolveSchemaReferences
            };
            settings.SetValidationEventHandler(_validationEventHandler);
            JSchemaReader schemaReader = new JSchemaReader(settings);

            schemaReader.Cache = Cache;
            schemaReader.Parent = this;
            for (int i = 0; i < _identifierScopeStack.Count; i++)
            {
                schemaReader.PushIdentifierScope(_identifierScopeStack[i]);
            }

            JSchema rootSchema;
            using (StreamReader streamReader = new StreamReader(schemaData))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                rootSchema = schemaReader.ReadRoot(jsonReader, false);
                rootSchema.InternalReader = schemaReader;
            }

            Cache[schemaReference.BaseUri] = rootSchema;

            // resolve deferred schemas after it has been cached to avoid
            // stackoverflow on circular references
            schemaReader.ResolveDeferredSchemas();

            // root schema might have been a reference so update cache again
            Cache[schemaReference.BaseUri] = schemaReader.RootSchema;

            return _resolver.GetSubschema(schemaReference, schemaReader.RootSchema);
        }

        private Uri? ResolveScopeId(out string? dynamicAnchor, out bool couldBeDynamic, out Uri? dynamicScope)
        {
            return ResolveScopeId(_identifierScopeStack, out dynamicAnchor, out couldBeDynamic, out dynamicScope);
        }

        private static Uri? ResolveScopeId(List<IIdentifierScope> stack, out string? dynamicAnchor, out bool couldBeDynamic, out Uri? dynamicScope)
        {
            dynamicAnchor = null;
            dynamicScope = null;
            couldBeDynamic = false;
            Uri? scopeId = null;

            for (int i = 0; i < stack.Count; i++)
            {
                IIdentifierScope current = stack[i];
                if (current.Root)
                {
                    // Clear the scope when moving into a new schema file
                    scopeId = null;
                }

                Uri? part = current.ScopeId;

                if (part != null)
                {
                    scopeId = scopeId == null
                        ? part
                        : SchemaDiscovery.ResolveSchemaId(scopeId, part);
                    dynamicAnchor = current.DynamicAnchor;
                    couldBeDynamic = current.CouldBeDynamic;

                    if (!couldBeDynamic)
                    {
                        // If there is no dynamic anchor on the root of a document then the scope is reset.
                        if (current.Root)
                        {
                            dynamicScope = null;
                        }
                    }
                    else
                    {
                        // If there is a dynamic anchor, and there currently isn't a dynamic scope,
                        // then the current scope becomes the dynamic scope.
                        if (dynamicScope == null)
                        {
                            dynamicScope = scopeId;
                        }
                    }
                }
            }

            return scopeId;
        }

        private Uri ResolveSchemaReference(Uri? scopeId, JSchema schema)
        {
            try
            {
                return SchemaDiscovery.ResolveSchemaId(scopeId, GetReference(schema)!);
            }
            catch (Exception ex)
            {
                string message = "Error resolving schema reference '{0}' in the scope '{1}'. The resolved reference must be a valid URI.".FormatWith(CultureInfo.InvariantCulture, schema.Reference, scopeId);
                throw JSchemaReaderException.Create(schema, _baseUri, schema.Path, message, ex);
            }
        }

        private static Uri? GetReference(JSchema schema)
        {
            return schema.Reference ?? schema.DynamicReference ?? schema.RecursiveReference;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private void ProcessSchemaName(ref JsonReader reader, JSchema target, bool isRoot, string name)
        {
            switch (name)
            {
                case Constants.PropertyNames.Id:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        target.Id = ReadUri(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.IdDraft3:
                    if (EnsureVersion(SchemaVersion.Draft3))
                    {
                        target.Id = ReadUri(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Ref:
                    target.Reference = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.RecursiveRef:
                    target.RecursiveReference = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.RecursiveAnchor:
                    target.RecursiveAnchor = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.DynamicRef:
                    target.DynamicReference = ReadUri(reader, name);
                    break;
                case Constants.PropertyNames.DynamicAnchor:
                    target.DynamicAnchor = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Anchor:
                    target.Anchor = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Properties:
                    target._properties = new JSchemaDictionary(target);
                    ReadProperties(reader, target, target._properties);

                    // add schemas with deprecated required flag to new required array
                    foreach (KeyValuePair<string, JSchema> schemaProperty in target._properties)
                    {
                        if (schemaProperty.Value.DeprecatedRequired)
                        {
                            if (!target.Required.Contains(schemaProperty.Key))
                            {
                                target.Required.Add(schemaProperty.Key);
                            }
                        }
                    }
                    break;
                case Constants.PropertyNames.Items:
                    // The meaning of "items" changed in 2020-12 to be like "additionalItems".
                    // Continue to use the old behavior if the schema version is unset.
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
                    {
                        ReadItems(reader, name, target);
                    }
                    else
                    {
                        ReadAdditionalItems(reader, name, target);
                    }
                    break;
                case Constants.PropertyNames.PrefixItems:
                    if (EnsureVersion(SchemaVersion.Draft2020_12))
                    {
                        ReadItems(reader, name, target);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Type:
                {
                    object typeResult = ReadType(reader, target, name)!;
                    if (typeResult is JSchemaType type)
                    {
                        target.Type = type;
                    }
                    else
                    {
                        target._anyOf = (JSchemaCollection) typeResult;
                    }
                    break;
                }
                case Constants.PropertyNames.AnyOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._anyOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.AllOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._allOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.OneOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchemaArray(reader, target, name, out target._oneOf);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.If:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        ReadSchema(reader, target, name, s => target.If = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Then:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        ReadSchema(reader, target, name, s => target.Then = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Else:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        ReadSchema(reader, target, name, s => target.Else = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Not:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        ReadSchema(reader, target, name, s => target.Not = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.PropertyNamesSchema:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        ReadSchema(reader, target, name, s => target.PropertyNames = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Contains:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        ReadSchema(reader, target, name, s => target.Contains = s);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Title:
                    target.Title = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Description:
                    target.Description = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.Format:
                    target.Format = ReadString(reader, name);
                    break;
                case Constants.PropertyNames.AdditionalProperties:
                    ReadAdditionalProperties(reader, target);
                    break;
                case Constants.PropertyNames.UnevaluatedProperties:
                    ReadUnevaluatedProperties(reader, target);
                    break;
                case Constants.PropertyNames.AdditionalItems:
                    // The meaning of "items" changed in 2020-12 to be like "additionalItems".
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft2019_09))
                    {
                        ReadAdditionalItems(reader, name, target);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.UnevaluatedItems:
                    ReadUnevaluatedItems(reader, target);
                    break;
                case Constants.PropertyNames.PatternProperties:
                    ReadProperties(reader, target, target.PatternProperties);

                    if (_validationErrors != null)
                    {
                        foreach (PatternSchema patternProperty in target.GetPatternSchemas())
                        {
                            if (!patternProperty.TryGetPatternRegex(
#if !(NET35 || NET40)
                                null,
#endif
                                out Regex? _,
                                out string? errorMessage))
                            {
                                ValidationError error = ValidationError.CreateValidationError($"Could not parse regex pattern '{patternProperty.Pattern}'. Regex parser error: {errorMessage}", ErrorType.PatternProperties, target, null, patternProperty.Pattern, null, target, target.Path!);
                                _validationErrors.Add(error);
                            }
                        }
                    }
                    break;
                case Constants.PropertyNames.Required:
                    ReadRequired(reader, target);
                    break;
                case Constants.PropertyNames.Dependencies:
                    ReadDependencies(reader, target);
                    break;
                case Constants.PropertyNames.DependentSchemas:
                    ReadProperties(reader, target, target.DependentSchemas);
                    break;
                case Constants.PropertyNames.DependentRequired:
                    if (EnsureVersion(SchemaVersion.Draft2019_09))
                    {
                        ReadObjectOfStringArrays(reader, Constants.PropertyNames.DependentRequired, target.DependentRequired);
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case Constants.PropertyNames.Minimum:
                    target.Minimum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.Maximum:
                    target.Maximum = ReadDouble(reader, name);
                    break;
                case Constants.PropertyNames.ExclusiveMinimum:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMinimum, Constants.MaximumMinimumTokens);
                    }
                    else
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMinimum, JsonToken.Boolean);
                    }

                    if (reader.TokenType == JsonToken.Boolean)
                    {
                        target.ExclusiveMinimum = (bool)reader.Value!;
                    }
                    else
                    {
                        target.Minimum = GetDouble(reader);
                        target.ExclusiveMinimum = true;
                    }
                    break;
                case Constants.PropertyNames.ExclusiveMaximum:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMaximum, Constants.MaximumMinimumTokens);
                    }
                    else
                    {
                        EnsureReadAndToken(reader, Constants.PropertyNames.ExclusiveMaximum, JsonToken.Boolean);
                    }

                    if (reader.TokenType == JsonToken.Boolean)
                    {
                        target.ExclusiveMaximum = (bool)reader.Value!;
                    }
                    else
                    {
                        target.Maximum = GetDouble(reader);
                        target.ExclusiveMaximum = true;
                    }
                    break;
                case Constants.PropertyNames.MaximumLength:
                    target.MaximumLength = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MinimumLength:
                    target.MinimumLength = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MaximumItems:
                    target.MaximumItems = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MinimumItems:
                    target.MinimumItems = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MaximumContains:
                    target.MaximumContains = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MinimumContains:
                    target.MinimumContains = ReadLong(reader, name);
                    break;
                case Constants.PropertyNames.MaximumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        target.MaximumProperties = ReadLong(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.MinimumProperties:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        target.MinimumProperties = ReadLong(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.DivisibleBy:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        target.MultipleOf = ReadDouble(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.MultipleOf:
                    if (EnsureVersion(SchemaVersion.Draft4))
                    {
                        try
                        {
                            target.MultipleOf = ReadDouble(reader, name);
                        }
                        catch (Exception ex)
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "multipleOf must be greater than zero.", ex);
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Disallow:
                {
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        if (target.Not == null)
                        {
                            target.Not = new JSchema();
                        }

                        object disallowResult = ReadType(reader, target, name)!;
                        if (disallowResult is JSchemaType schemaType)
                        {
                            JSchemaType type = target.Not.Type ?? JSchemaType.None;
                            target.Not.Type = type | schemaType;
                        }
                        else
                        {
                            target.Not._anyOf = (JSchemaCollection)disallowResult;
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                }
                case Constants.PropertyNames.Pattern:
                    target.Pattern = ReadString(reader, name);

                    if (_validationErrors != null)
                    {
                        if (!target.TryGetPatternRegex(
#if !(NET35 || NET40)
                            null,
#endif
                            out _,
                            out string? errorMessage))
                        {
                            ValidationError error = ValidationError.CreateValidationError($"Could not parse regex pattern '{target.Pattern}'. Regex parser error: {errorMessage}", ErrorType.Pattern, target, null, target.Pattern, null, target, target.Path!);
                            _validationErrors.Add(error);
                        }
                    }
                    break;
                case Constants.PropertyNames.Enum:
                    ReadTokenArray(reader, name, ref target._enum);
                    if (target._enum.Count == 0)
                    {
                        throw JSchemaReaderException.Create(reader, _baseUri, "Enum array must have at least one value.");
                    }
                    break;
                case Constants.PropertyNames.Const:
                    if (EnsureVersion(SchemaVersion.Draft6))
                    {
                        EnsureRead(reader, Constants.PropertyNames.Const);
                        JToken t = JToken.ReadFrom(reader);

                        target.Const = t;
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.Extends:
                    if (EnsureVersion(SchemaVersion.Draft3, SchemaVersion.Draft3))
                    {
                        ReadExtends(reader, target);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.UniqueItems:
                    target.UniqueItems = ReadBoolean(reader, name);
                    break;
                case Constants.PropertyNames.Default:
                    EnsureRead(reader, Constants.PropertyNames.Default);
                    target.Default = JToken.ReadFrom(reader);
                    break;
                case Constants.PropertyNames.ContentEncoding:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        target.ContentEncoding = ReadString(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.ContentMediaType:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        target.ContentMediaType = ReadString(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.ReadOnly:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        target.ReadOnly = ReadBoolean(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                case Constants.PropertyNames.WriteOnly:
                    if (EnsureVersion(SchemaVersion.Draft7))
                    {
                        target.WriteOnly = ReadBoolean(reader, name);
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
                default:
                    if (isRoot && name == Constants.PropertyNames.Schema)
                    {
                        if (!reader.Read())
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
                        }

                        if (reader.TokenType != JsonToken.String)
                        {
                            throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected token encountered when reading value for '$schema'. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }

                        target.SchemaVersion = ParseUri(reader, (string)reader.Value!);

                        _versionUri = target.SchemaVersion;

                        _version = SchemaVersionHelpers.MapSchemaUri(_versionUri);
                        _schemaDiscovery.SchemaVersion = _version;

                        if (_validateSchema)
                        {
                            _validatingSchema = SchemaVersionHelpers.GetSchema(_version);

                            if (_validatingSchema == null)
                            {
                                if (!_versionUri.IsAbsoluteUri)
                                {
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Schema version identifier '{0}' is not an absolute URI.".FormatWith(CultureInfo.InvariantCulture, _versionUri.OriginalString));
                                }

                                _validatingSchema = ResolvedSchema(_versionUri, _versionUri);

                                if (_validatingSchema == null)
                                {
                                    throw JSchemaReaderException.Create(reader, _baseUri, "Could not resolve schema version identifier '{0}'.".FormatWith(CultureInfo.InvariantCulture, _versionUri.OriginalString));
                                }
                            }

                            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
                            validatingReader.Schema = _validatingSchema;
                            // push state that we're already inside an object
                            validatingReader.Validator.ValidateCurrentToken(JsonToken.StartObject, null, 0);
                            validatingReader.ValidationEventHandler += RaiseSchemaValidationError;

                            reader = validatingReader;
                        }
                    }
                    else
                    {
                        ReadExtensionData(reader, target, name);
                    }
                    break;
            }

            if (name != Constants.PropertyNames.Ref && name != Constants.PropertyNames.RecursiveRef && name != Constants.PropertyNames.DynamicRef)
            {
                target.HasNonRefContent = true;
            }
        }

        private void ReadExtensionData(JsonReader reader, JSchema target, string name)
        {
            if (!reader.Read())
            {
                throw JSchemaReaderException.Create(reader, _baseUri, "Unexpected end when reading schema.");
            }

            JToken t;
            if (reader is JTokenReader tokenReader)
            {
                t = tokenReader.CurrentToken!;
                tokenReader.Skip();
            }
            else
            {
                string basePath = reader.Path;
                t = JToken.ReadFrom(reader);
                t.AddAnnotation(new JTokenPathAnnotation(basePath));
            }

            target.ExtensionData[name] = t;
        }

        private void RaiseSchemaValidationError(object sender, SchemaValidationEventArgs e)
        {
            throw JSchemaReaderException.Create(
                (JsonReader)sender,
                _baseUri,
                "Validation error raised by version schema '{0}': {1}".FormatWith(CultureInfo.InvariantCulture, _versionUri, e.ValidationError.Message),
                JSchemaValidationException.Create(e.ValidationError));
        }
    }
}