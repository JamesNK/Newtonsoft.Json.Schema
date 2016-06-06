#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Validate
{
    [TestFixture]
    public class CustomJsonValidator : TestFixtureBase
    {
        [Test]
        public void Usage()
        {
            #region Usage
            string json = @"[
              'en-US',
              'en-GB',
              'fr-FR',
              'purple monkey dishwasher',
              1234
            ]";

            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                Validators = new List<JsonValidator> { new CultureFormatValidator() }
            };

            // the culture validator will be used to validate the array items
            JSchema schema = JSchema.Parse(@"{
              'type': 'array',
              'items': {
                'type': 'string',
                'format': 'culture'
              }
            }", settings);

            JArray cultures = JArray.Parse(json);

            IList<ValidationError> errors;
            bool isValid = cultures.IsValid(schema, out errors);

            // false
            Console.WriteLine(isValid);

            // Text 'purple monkey dishwasher' is not a valid culture name.
            Console.WriteLine(errors[0].Message);

            // Invalid type. Expected String but got Integer.
            Console.WriteLine(errors[1].Message);
            #endregion

            Assert.AreEqual(2, errors.Count);

            Assert.AreEqual(@"Text 'purple monkey dishwasher' is not a valid culture name.", errors[0].Message);
            Assert.AreEqual(ErrorType.Validator, errors[0].ErrorType);
            Assert.AreEqual("#/items/0", errors[0].SchemaId.OriginalString);

            Assert.AreEqual(@"Invalid type. Expected String but got Integer.", errors[1].Message);
            Assert.AreEqual(ErrorType.Type, errors[1].ErrorType);
            Assert.AreEqual("#/items/0", errors[1].SchemaId.OriginalString);
        }

        #region Types
        public class CultureFormatValidator : JsonValidator
        {
            public override void Validate(JToken value, JsonValidatorContext context)
            {
                if (value.Type == JTokenType.String)
                {
                    string s = value.ToString();

                    try
                    {
                        // test whether the string is a known culture, e.g. en-US, fr-FR
                        new CultureInfo(s);
                    }
                    catch (CultureNotFoundException)
                    {
                        context.RaiseError($"Text '{s}' is not a valid culture name.");
                    }
                }
            }

            public override bool CanValidate(JSchema schema)
            {
                // validator will run when a schema has a format of culture
                return (schema.Format == "culture");
            }
        }
        #endregion
    }
}
