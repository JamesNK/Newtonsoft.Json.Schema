#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    internal abstract class Validator
    {
        private readonly List<Scope> _scopes;
        private readonly object _publicValidator;
        private readonly ValidatorContext _context;
        private JSchemaDiscovery _schemaDiscovery;
        
        public JTokenWriter TokenWriter;
        public JSchema Schema;
        public event SchemaValidationEventHandler ValidationEventHandler;

        public List<Scope> Scopes
        {
            [DebuggerStepThrough]
            get { return _scopes; }
        }

        public abstract ValidationError CreateError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors);

        protected Validator(object publicValidator)
        {
            _publicValidator = publicValidator;
            _scopes = new List<Scope>();
            _context = new ValidatorContext(this);
        }

        public void RaiseError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors)
        {
            ValidationError error = CreateError(message, errorType, schema, value, childErrors);

            SchemaValidationEventHandler handler = ValidationEventHandler;
            if (handler != null)
                handler(_publicValidator, new SchemaValidationEventArgs(error));
            else
                throw JSchemaValidationException.Create(message, error);
        }

        protected ValidationError CreateError(string message, ErrorType errorType, JSchema schema, object value, IList<ValidationError> childErrors, IJsonLineInfo lineInfo, string path)
        {
            if (_schemaDiscovery == null)
            {
                _schemaDiscovery = new JSchemaDiscovery();
                _schemaDiscovery.Discover(Schema, null);
            }

            ValidationError error = new ValidationError();
            error.Message = message;
            error.ErrorType = errorType;
            error.Path = path;
            if (lineInfo != null)
            {
                error.LineNumber = lineInfo.LineNumber;
                error.LinePosition = lineInfo.LinePosition;
            }
            error.Schema = schema;
            error.SchemaId = _schemaDiscovery.KnownSchemas.Single(s => s.Schema == schema).Id;
            error.Value = value;
            error.ChildErrors = childErrors;

            return error;
        }

        public void ValidateCurrentToken(JsonToken token, object value, int depth)
        {
            if (_scopes.Count == 0)
            {
                if (Schema == null)
                    throw new JSchemaException("No schema has been set for the validator.");

                LicenseHelpers.IncrementAndCheckValidationCount();
                SchemaScope.CreateTokenScope(token, Schema, _context, null, depth);
            }

            if (TokenWriter != null)
                TokenWriter.WriteToken(token, value);

            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                Scope scope = _scopes[i];

                if (!scope.Complete)
                    scope.EvaluateToken(token, value, depth);
                else
                    _scopes.RemoveAt(i);
            }

            if (TokenWriter != null && TokenWriter.Top == 0)
                TokenWriter = null;
        }
    }
}