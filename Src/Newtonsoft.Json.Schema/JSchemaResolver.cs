#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Net;

namespace Newtonsoft.Json.Schema
{
    /// <summary>
    /// Resolves external JSON Schemas named by a Uniform Resource Identifier (URI).
    /// </summary>
    public abstract class JSchemaResolver
    {
        /// <summary>
        /// Gets the schema for a given URI.
        /// </summary>
        /// <param name="uri">The schema URI to resolve.</param>
        /// <returns>The resolved schema.</returns>
        public abstract JSchema GetSchema(Uri uri);

        /// <summary>
        /// Sets the credentials used to authenticate web requests.
        /// </summary>
        public virtual ICredentials Credentials
        {
            set { }
        }
    }
}