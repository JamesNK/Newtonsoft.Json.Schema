#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Contains the JSON schema extension methods.
    /// </summary>
    public static class SchemaExtensions
    {
        /// <summary>
        /// Determines whether the <see cref="JToken"/> is valid.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="JToken"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this JToken source, JSchema schema)
        {
            bool valid = true;
            source.Validate(schema, (sender, args) => { valid = false; });
            return valid;
        }

        /// <summary>
        /// Determines whether the <see cref="JToken"/> is valid.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        /// <param name="errorMessages">When this method returns, contains any error messages generated while validating.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="JToken"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this JToken source, JSchema schema, out IList<string> errorMessages)
        {
            IList<string> errors = new List<string>();

            source.Validate(schema, (sender, args) => errors.Add(args.Message));

            errorMessages = errors;
            return (errorMessages.Count == 0);
        }

        /// <summary>
        /// Determines whether the <see cref="JToken"/> is valid.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        /// <param name="errors">When this method returns, contains any errors generated while validating.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="JToken"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this JToken source, JSchema schema, out IList<ValidationError> errors)
        {
            IList<ValidationError> schemaErrors = new List<ValidationError>();

            source.Validate(schema, (sender, args) => schemaErrors.Add(args.ValidationError));

            errors = schemaErrors;
            return (errors.Count == 0);
        }

        /// <summary>
        /// Validates the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        public static void Validate(this JToken source, JSchema schema)
        {
            source.Validate(schema, null);
        }

        /// <summary>
        /// Validates the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        public static void Validate(this JToken source, JSchema schema, SchemaValidationEventHandler? validationEventHandler)
        {
            ValidationUtils.ArgumentNotNull(source, nameof(source));
            ValidationUtils.ArgumentNotNull(schema, nameof(schema));

            using JSchemaValidatingReader reader = new(source.CreateReader());
            reader.Schema = schema;
            if (validationEventHandler != null)
            {
                reader.ValidationEventHandler += validationEventHandler;
            }

            while (reader.Read())
            {
            }
        }
    }
}