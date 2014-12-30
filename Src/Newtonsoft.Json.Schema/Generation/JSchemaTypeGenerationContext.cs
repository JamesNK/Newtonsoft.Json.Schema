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
        private readonly Type _objectType;
        private readonly Required _required;
        private readonly JsonProperty _memberProperty;
        private readonly JsonContainerContract _parentContract;
        private readonly JSchemaGenerator _generator;

        /// <summary>
        /// The object type.
        /// </summary>
        public Type ObjectType
        {
            get { return _objectType; }
        }

        /// <summary>
        /// The required state.
        /// </summary>
        public Required Required
        {
            get { return _required; }
        }

        /// <summary>
        /// The member property.
        /// </summary>
        public JsonProperty MemberProperty
        {
            get { return _memberProperty; }
        }

        /// <summary>
        /// The parent contract.
        /// </summary>
        public JsonContainerContract ParentContract
        {
            get { return _parentContract; }
        }

        /// <summary>
        /// The current <see cref="JSchemaGenerator"/>.
        /// </summary>
        public JSchemaGenerator Generator
        {
            get { return _generator; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaTypeGenerationContext"/> class.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="required">The required state.</param>
        /// <param name="memberProperty">The member property.</param>
        /// <param name="parentContract">The parent contract.</param>
        /// <param name="generator">The current <see cref="JSchemaGenerator"/>.</param>
        public JSchemaTypeGenerationContext(Type objectType, Required required, JsonProperty memberProperty, JsonContainerContract parentContract, JSchemaGenerator generator)
        {
            _objectType = objectType;
            _required = required;
            _memberProperty = memberProperty;
            _parentContract = parentContract;
            _generator = generator;
        }
    }
}