#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Schema.Generation;
#if DNXCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Schema.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif

namespace Newtonsoft.Json.Schema.Tests.Documentation.Samples.Generation
{
    [TestFixture]
    public class CreateCustomProvider : TestFixtureBase
    {
        #region Types
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class FormatSchemaProvider : JSchemaGenerationProvider
        {
            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                // customize the generated schema for these types to have a format
                if (context.ObjectType == typeof(int))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "int32");
                }
                if (context.ObjectType == typeof(long))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "int64");
                }
                if (context.ObjectType == typeof(float))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "float");
                }
                if (context.ObjectType == typeof(double))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "double");
                }
                if (context.ObjectType == typeof(byte))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "byte");
                }
                if (context.ObjectType == typeof(DateTime) || context.ObjectType == typeof(DateTimeOffset))
                {
                    return CreateSchemaWithFormat(context.ObjectType, context.Required, "date-time");
                }

                // use default schema generation for all other types
                return null;
            }

            private JSchema CreateSchemaWithFormat(Type type, Required required, string format)
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                JSchema schema = generator.Generate(type, required != Required.Always);
                schema.Format = format;

                return schema;
            }
        }
        #endregion

        [Test]
        public void Example()
        {
            #region Usage
            JSchemaGenerator generator = new JSchemaGenerator();
            generator.GenerationProviders.Add(new FormatSchemaProvider());

            JSchema schema = generator.Generate(typeof(User));
            // {
            //   "type": "object",
            //   "properties": {
            //     "Id": {
            //       "type": "integer",
            //       "format": "int32"
            //     },
            //     "Name": {
            //       "type": [
            //         "string",
            //         "null"
            //       ]
            //     },
            //     "CreatedDate": {
            //       "type": "string",
            //       "format": "date-time"
            //     }
            //   },
            //   "required": [
            //     "Id",
            //     "Name",
            //     "CreatedDate"
            //   ]
            // }
            #endregion

            Console.WriteLine(schema.ToString());

            Assert.AreEqual(JSchemaType.Integer, schema.Properties["Id"].Type);
            Assert.AreEqual("int32", schema.Properties["Id"].Format);
        }
    }
}