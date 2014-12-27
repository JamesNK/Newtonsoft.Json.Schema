#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Generation
{
    public class JSchemaGenerationProviderAttribute : Attribute
    {
        private readonly Type _providerType;
        private readonly object[] _providerParameters;

        public Type ProviderType
        {
            get { return _providerType; }
        }

        /// <summary>
        /// The parameter list to use when constructing the <see cref="JSchemaGenerationProvider"/> described by <see cref="ProviderType"/>.
        /// If null, the default constructor is used.
        /// </summary>
        public object[] ProviderParameters
        {
            get { return _providerParameters; }
        }

        public JSchemaGenerationProviderAttribute(Type providerType)
        {
            _providerType = providerType;
        }

        public JSchemaGenerationProviderAttribute(Type providerType, object[] providerParameters)
            : this(providerType)
        {
            _providerParameters = providerParameters;
        }
    }
}