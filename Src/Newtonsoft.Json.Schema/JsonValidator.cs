#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Validation;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// The context for a <see cref="JsonValidator"/>. Provides methods for raising validation errors.
    /// </summary>
    public class JsonValidatorContext
    {
        private readonly Scope _scope;
        private readonly JSchema _schema;

        internal JsonValidatorContext(Scope scope, JSchema schema)
        {
            _scope = scope;
            _schema = schema;
        }

        /// <summary>
        /// The current schema.
        /// </summary>
        public JSchema Schema => _schema;

        /// <summary>
        /// Raises a validation error.
        /// </summary>
        /// <param name="message">A message describing the error that occurred.</param>
        public void RaiseError(string message)
        {
            RaiseError(message, null);
        }

        /// <summary>
        /// Raises a validation error.
        /// </summary>
        /// <param name="message">The message describing the error that occurred.</param>
        /// <param name="value">The JSON value when the error occurred.</param>
        public void RaiseError(string message, object value)
        {
            _scope.RaiseError(FormattableStringFactory.Create(message, CollectionHelpers.EmptyArray), ErrorType.Validator, _schema, value, null);
        }
    }

    /// <summary>
    /// Provides a base class for implementing a custom JSON validator.
    /// </summary>
    public abstract class JsonValidator
    {
        /// <summary>
        /// Validates the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="value">The <see cref="JToken"/> to validate.</param>
        /// <param name="context">The validation context.</param>
        public abstract void Validate(JToken value, JsonValidatorContext context);

        /// <summary>
        /// Determines whether this instance should validate with the specified <see cref="JSchema"/>.
        /// </summary>
        /// <param name="schema">The <see cref="JSchema"/>.</param>
        /// <returns>
        /// 	<c>true</c> if this instance should validate with the specified <see cref="JSchema"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanValidate(JSchema schema);
    }
}