#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Describes a <see cref="Type"/> and its context when generating a <see cref="JSchema"/>.
    /// </summary>
    public class JSchemaTypeGenerationContext
    {
        private readonly JSchemaGeneratorInternal _generatorInternal;

        internal JSchemaGenerationProvider? GenerationProvider;
        private JSchemaGeneratorProxy? _generatorProxy;

        /// <summary>
        /// The schema title.
        /// </summary>
        public string? SchemaTitle { get; }

        /// <summary>
        /// The schema description.
        /// </summary>
        public string? SchemaDescription { get; }

        /// <summary>
        /// The object type.
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// The required state.
        /// </summary>
        public Required Required { get; }

        /// <summary>
        /// The member property.
        /// </summary>
        public JsonProperty? MemberProperty { get; }

        /// <summary>
        /// The parent contract.
        /// </summary>
        public JsonContainerContract? ParentContract { get; }

        /// <summary>
        /// The current <see cref="JSchemaGenerator"/>.
        /// </summary>
        public JSchemaGenerator Generator
        {
            get
            {
                if (_generatorProxy == null)
                {
                    _generatorProxy = new JSchemaGeneratorProxy(_generatorInternal, GenerationProvider);
                }

                return _generatorProxy;
            }
        }

        internal JSchemaTypeGenerationContext(
            Type objectType,
            Required required,
            JsonProperty? memberProperty,
            JsonContainerContract? parentContract,
            JSchemaGeneratorInternal generatorInternal,
            string? schemaTitle,
            string? schemaDescription)
        {
            SchemaTitle = schemaTitle;
            SchemaDescription = schemaDescription;
            ObjectType = objectType;
            Required = required;
            MemberProperty = memberProperty;
            ParentContract = parentContract;
            _generatorInternal = generatorInternal;
        }
    }
}