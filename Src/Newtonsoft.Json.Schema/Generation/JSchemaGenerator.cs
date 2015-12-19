#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Infrastructure;
using Newtonsoft.Json.Schema.Infrastructure.Licensing;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Generates a <see cref="JSchema"/> from a specified <see cref="Type"/>.
    /// </summary>
    public class JSchemaGenerator
    {
        private IContractResolver _contractResolver;
        internal List<JSchemaGenerationProvider> _generationProviders;

        /// <summary>
        /// Gets or sets how IDs are generated for schemas with no ID.
        /// </summary>
        public SchemaIdGenerationHandling SchemaIdGenerationHandling { get; set; }

        /// <summary>
        /// Gets or sets the schema property order.
        /// </summary>
        public SchemaPropertyOrderHandling SchemaPropertyOrderHandling { get; set; }

        /// <summary>
        /// Gets or sets the location of referenced schemas.
        /// </summary>
        public SchemaLocationHandling SchemaLocationHandling { get; set; }

        /// <summary>
        /// Gets or sets whether generated schemas can be referenced.
        /// </summary>
        public SchemaReferenceHandling SchemaReferenceHandling { get; set; }

        /// <summary>
        /// Gets or sets the default required state of schemas.
        /// </summary>
        public Required DefaultRequired { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="JSchemaGenerationProvider"/> instances that are used to customize <see cref="JSchema"/> generation.
        /// </summary>
        public IList<JSchemaGenerationProvider> GenerationProviders
        {
            get
            {
                if (_generationProviders == null)
                {
                    _generationProviders = new List<JSchemaGenerationProvider>();
                }

                return _generationProviders;
            }
        }

        /// <summary>
        /// Gets or sets the contract resolver.
        /// </summary>
        /// <value>The contract resolver.</value>
        public IContractResolver ContractResolver
        {
            get
            {
                if (_contractResolver == null)
                {
                    return DefaultContractResolver.Instance;
                }

                return _contractResolver;
            }
            set { _contractResolver = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaGenerator"/> class.
        /// </summary>
        public JSchemaGenerator()
        {
            SchemaReferenceHandling = SchemaReferenceHandling.Objects;
            DefaultRequired = Required.AllowNull;
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            return Generate(type, false);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="JSchema"/> will be nullable.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public JSchema Generate(Type type, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            LicenseHelpers.IncrementAndCheckGenerationCount();

            Required required = rootSchemaNullable ? Required.AllowNull : Required.Always;

            JSchemaGeneratorInternal generator = new JSchemaGeneratorInternal(this);
            return generator.Generate(type, required);
        }
    }
}