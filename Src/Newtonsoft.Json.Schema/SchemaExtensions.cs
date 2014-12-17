#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.V4;
using Newtonsoft.Json.Utilities;

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
        public static bool IsValid(this JToken source, JSchema4 schema)
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
        /// <param name="errorMessages">When this method returns, contains any error messages generated while validating. </param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="JToken"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(this JToken source, JSchema4 schema, out IList<string> errorMessages)
        {
            IList<string> errors = new List<string>();

            source.Validate(schema, (sender, args) => errors.Add(args.Message));

            errorMessages = errors;
            return (errorMessages.Count == 0);
        }

        /// <summary>
        /// Validates the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        public static void Validate(this JToken source, JSchema4 schema)
        {
            source.Validate(schema, null);
        }

        /// <summary>
        /// Validates the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="source">The source <see cref="JToken"/> to test.</param>
        /// <param name="schema">The schema to test with.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        public static void Validate(this JToken source, JSchema4 schema, SchemaValidationEventHandler validationEventHandler)
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            ValidationUtils.ArgumentNotNull(schema, "schema");

            using (JSchema4ValidatingReader reader = new JSchema4ValidatingReader(source.CreateReader()))
            {
                reader.Schema = schema;
                if (validationEventHandler != null)
                    reader.ValidationEventHandler += validationEventHandler;

                while (reader.Read())
                {
                }
            }
        }
    }
}