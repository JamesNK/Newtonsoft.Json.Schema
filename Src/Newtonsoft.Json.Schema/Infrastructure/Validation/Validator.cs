#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class Validator
    {
        private readonly List<Scope> _scopes;
        private readonly List<Scope> _scopesCache;
        private readonly object _publicValidator;
        private readonly ValidatorContext _context;

        private bool _knownSchemasPopulated;
        private SchemaVersion? _schemaVersion;
        private bool _hasValidatedLicense;

        public JTokenWriter? TokenWriter;
        public JSchema? Schema;
        public event SchemaValidationEventHandler? ValidationEventHandler;
#if !(NET35 || NET40)
        public TimeSpan? RegexMatchTimeout;
#endif

        public List<Scope> Scopes
        {
            [DebuggerStepThrough]
            get { return _scopes; }
        }

        public abstract ValidationError CreateError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors);

        protected Validator(object publicValidator)
        {
            _publicValidator = publicValidator;
            _scopes = new List<Scope>();
            _scopesCache = new List<Scope>();
            _context = new ValidatorContext(this);
        }

        public void RaiseError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors)
        {
            ValidationError error = CreateError(message, errorType, schema, value, childErrors);
            RaiseError(error);
        }

        public void RaiseError(ValidationError error)
        {
            // shared cache information that could be read/populated from multiple threads
            // lock to ensure that only one thread writes known schemas
            if (!_knownSchemasPopulated)
            {
                ValidationUtils.Assert(Schema != null);
                lock (Schema.KnownSchemas)
                {
                    if (!_knownSchemasPopulated)
                    {
                        if (Schema.KnownSchemas.Count == 0)
                        {
                            JSchemaDiscovery discovery = new JSchemaDiscovery(Schema, Schema.KnownSchemas, KnownSchemaState.External);
                            discovery.Discover(Schema, null);
                        }

                        _knownSchemasPopulated = true;
                    }
                }
            }

            PopulateSchemaId(error);

#if DEBUG
            ValidateErrorNotRecursive(error);
#endif

            SchemaValidationEventHandler? handler = ValidationEventHandler;
            if (handler != null)
            {
                handler(_publicValidator, new SchemaValidationEventArgs(error));
            }
            else
            {
                throw JSchemaValidationException.Create(error);
            }
        }

        private static void ValidateErrorNotRecursive(ValidationError error)
        {
            List<ValidationError> errorStack = new List<ValidationError>();
            AddAndValidate(errorStack, error);

            static void AddAndValidate(List<ValidationError> errorStack, ValidationError error)
            {
                if (errorStack.Contains(error))
                {
                    throw new Exception("Recursive validation error.");
                }

                errorStack.Add(error);
                foreach (var item in error.ChildErrors)
                {
                    AddAndValidate(errorStack, item);
                }
                errorStack.RemoveAt(errorStack.Count - 1);
            }
        }

        public SchemaVersion SchemaVersion
        {
            get
            {
                if (_schemaVersion == null)
                {
                    ValidationUtils.Assert(Schema != null);
                    _schemaVersion = SchemaVersionHelpers.MapSchemaUri(Schema.SchemaVersion);
                }

                return _schemaVersion.Value;
            }
        }

        private void PopulateSchemaId(ValidationError error)
        {
            ValidationUtils.Assert(Schema != null);
            
            if (error.SchemaId != null)
            {
                return;
            }

            Uri? schemaId = null;
            for (int i = 0; i < Schema.KnownSchemas.Count; i++)
            {
                KnownSchema s = Schema.KnownSchemas[i];
                if (s.Schema == error.Schema)
                {
                    schemaId = s.Id;
                    break;
                }
            }

            error.SchemaId = schemaId;

            // property getter will lazy create a new list
            // don't create child error list unneeded
            if (error._childErrors != null)
            {
                for (int i = 0; i < error._childErrors.Count; i++)
                {
                    ValidationError validationError = error._childErrors[i];
                    PopulateSchemaId(validationError);
                }
            }
        }

        protected ValidationError CreateError(IFormattable message, ErrorType errorType, JSchema schema, object? value, IList<ValidationError>? childErrors, IJsonLineInfo? lineInfo, string path)
        {
            ValidationError error = ValidationError.CreateValidationError(message, errorType, schema, null, value, childErrors, lineInfo, path);

            return error;
        }

        public void ValidateCurrentToken(JsonToken token, object? value, int depth)
        {
            if (depth == 0)
            {
                // Handle validating multiple content
                RemoveCompletedScopes();
            }

            if (_scopes.Count == 0)
            {
                if (Schema == null)
                {
                    throw new JSchemaException("No schema has been set for the validator.");
                }

                if (!_hasValidatedLicense)
                {
                    LicenseHelpers.IncrementAndCheckValidationCount();
                    _hasValidatedLicense = true;
                }

                SchemaScope.CreateTokenScope(token, Schema, _context, null, depth);
            }

            if (TokenWriter != null)
            {
                // JTokenReader can return JsonToken.String with a null value which WriteToken doesn't like.
                // Hacky - change token to JsonToken.Null. Can be removed when fixed Newtonsoft.Json is public.
                JsonToken fixedToken = (token == JsonToken.String && value == null) ? JsonToken.Null : token;

                TokenWriter.WriteToken(fixedToken, value);
            }

            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = _scopes[i];

                if (scope.Complete != CompleteState.Completed)
                {
                    scope.EvaluateToken(token, value, depth);
                }
                else
                {
                    _scopes.RemoveAt(i);
                    _scopesCache.Add(scope);
                }
            }

            if (TokenWriter != null && (TokenWriter.WriteState == WriteState.Start || TokenWriter.WriteState == WriteState.Closed))
            {
                TokenWriter = null;
            }
        }

        private void RemoveCompletedScopes()
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = _scopes[i];

                if (scope.Complete == CompleteState.Completed)
                {
                    _scopes.RemoveAt(i);
                    _scopesCache.Add(scope);
                }
            }
        }

        public T? GetCachedScope<T>(ScopeType type) where T : Scope
        {
            for (int i = 0; i < _scopesCache.Count; i++)
            {
                Scope s = _scopesCache[i];
                if (s.Type == type)
                {
                    _scopesCache.RemoveAt(i);
                    return (T)s;
                }
            }
            return null;
        }

        public void ReturnScopeToCache(Scope scope)
        {
            _scopesCache.Add(scope);
        }
    }
}