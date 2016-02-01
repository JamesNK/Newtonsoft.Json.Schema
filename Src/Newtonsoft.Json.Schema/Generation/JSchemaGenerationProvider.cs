#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Generation
{
    /// <summary>
    /// Customizes <see cref="JSchema"/> generation for a <see cref="Type"/>.
    /// </summary>
    public abstract class JSchemaGenerationProvider
    {
        /// <summary>
        /// Gets a <see cref="JSchema"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="context">The <see cref="Type"/> and associated information used to generate a <see cref="JSchema"/>.</param>
        /// <returns>The generated <see cref="JSchema"/> or <c>null</c> if type should not have a customized schema.</returns>
        public abstract JSchema GetSchema(JSchemaTypeGenerationContext context);

        /// <summary>
        /// Determines whether this instance can generate a <see cref="JSchema"/> for the specified object type.
        /// </summary>
        /// <param name="context">The <see cref="Type"/> and associated information.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public virtual bool CanGenerateSchema(JSchemaTypeGenerationContext context)
        {
            return true;
        }
    }
}