#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Instructs the <see cref="JSchemaGenerator"/> to use the specified <see cref="JSchemaGenerationProvider"/> when generating the member or class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter, AllowMultiple = false)]
    public class JSchemaGenerationProviderAttribute : Attribute
    {
        private readonly Type _providerType;
        private readonly object[] _providerParameters;

        /// <summary>
        /// Gets the <see cref="Type"/> of the provider.
        /// </summary>
        /// <value>The <see cref="Type"/> of the provider.</value>
        public Type ProviderType => _providerType;

        /// <summary>
        /// The parameter list to use when constructing the <see cref="JSchemaGenerationProvider"/> described by <see cref="ProviderType"/>.
        /// If null, the default constructor is used.
        /// </summary>
        public object[] ProviderParameters => _providerParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaGenerationProviderAttribute"/> class.
        /// </summary>
        /// <param name="providerType">Type of the provider.</param>
        public JSchemaGenerationProviderAttribute(Type providerType)
        {
            _providerType = providerType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaGenerationProviderAttribute"/> class.
        /// </summary>
        /// <param name="providerType">Type of the provider.</param>
        /// <param name="providerParameters">Parameter list to use when constructing the JSchemaGenerationProvider. Can be null.</param>
        public JSchemaGenerationProviderAttribute(Type providerType, object[] providerParameters)
            : this(providerType)
        {
            _providerParameters = providerParameters;
        }
    }
}