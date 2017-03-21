#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static IContractResolver _defaultInstance;

        private IContractResolver _contractResolver;
        internal List<JSchemaGenerationProvider> _generationProviders;
        private SchemaReferenceHandling _schemaReferenceHandling;
        private Required _defaultRequired;

        private static IContractResolver DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    FieldInfo field =
#if !PORTABLE
                        typeof(DefaultContractResolver).GetField(nameof(DefaultContractResolver.Instance), BindingFlags.Static);
#else
                        typeof(DefaultContractResolver).GetTypeInfo().DeclaredFields.SingleOrDefault(f => f.IsStatic && f.Name == nameof(DefaultContractResolver.Instance));
#endif
                    if (field != null)
                    {
                        _defaultInstance = (IContractResolver)field.GetValue(null);
                    }
                    else
                    {
                        PropertyInfo property =
#if !PORTABLE
                            typeof(DefaultContractResolver).GetProperty(nameof(DefaultContractResolver.Instance), BindingFlags.Static);
#else
                            typeof(DefaultContractResolver).GetTypeInfo().DeclaredProperties.SingleOrDefault(f => (f.GetMethod?.IsStatic ?? false) && f.Name == nameof(DefaultContractResolver.Instance));
#endif
                        if (property != null)
                        {
                            _defaultInstance = (IContractResolver)property.GetValue(null, null);
                        }
                        else
                        {
                            _defaultInstance = new DefaultContractResolver();
                        }
                    }
                }

                return _defaultInstance;
            }
        }

        /// <summary>
        /// Gets or sets how IDs are generated for schemas with no ID.
        /// </summary>
        public virtual SchemaIdGenerationHandling SchemaIdGenerationHandling { get; set; }

        /// <summary>
        /// Gets or sets the schema property order.
        /// </summary>
        public virtual SchemaPropertyOrderHandling SchemaPropertyOrderHandling { get; set; }

        /// <summary>
        /// Gets or sets the location of referenced schemas.
        /// </summary>
        public virtual SchemaLocationHandling SchemaLocationHandling { get; set; }

        /// <summary>
        /// Gets or sets whether generated schemas can be referenced.
        /// </summary>
        public virtual SchemaReferenceHandling SchemaReferenceHandling
        {
            get { return _schemaReferenceHandling; }
            set { _schemaReferenceHandling = value; }
        }

        /// <summary>
        /// Gets or sets the default required state of schemas.
        /// </summary>
        public virtual Required DefaultRequired
        {
            get { return _defaultRequired; }
            set { _defaultRequired = value; }
        }

        /// <summary>
        /// Gets a collection of <see cref="JSchemaGenerationProvider"/> instances that are used to customize <see cref="JSchema"/> generation.
        /// </summary>
        public virtual IList<JSchemaGenerationProvider> GenerationProviders
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
        public virtual IContractResolver ContractResolver
        {
            get
            {
                if (_contractResolver == null)
                {
                    return DefaultInstance;
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
            _schemaReferenceHandling = SchemaReferenceHandling.Objects;
            _defaultRequired = Required.AllowNull;
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public virtual JSchema Generate(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));

            return Generate(type, false);
        }

        /// <summary>
        /// Generate a <see cref="JSchema"/> from the specified type.
        /// </summary>
        /// <param name="type">The type to generate a <see cref="JSchema"/> from.</param>
        /// <param name="rootSchemaNullable">Specify whether the generated root <see cref="JSchema"/> will be nullable.</param>
        /// <returns>A <see cref="JSchema"/> generated from the specified type.</returns>
        public virtual JSchema Generate(Type type, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));

            LicenseHelpers.IncrementAndCheckGenerationCount();

            Required required = rootSchemaNullable ? Required.AllowNull : Required.Always;

            JSchemaGeneratorInternal generator = new JSchemaGeneratorInternal(this);
            return generator.Generate(type, required);
        }
    }
}