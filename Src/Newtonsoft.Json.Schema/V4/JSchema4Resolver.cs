#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Net;

namespace Newtonsoft.Json.Schema.V4
{
    public abstract class JSchema4Resolver
    {
        public abstract JSchema4 GetSchema(Uri uri);

        public virtual Uri GetReference(Uri scopeId, JSchema4 schema)
        {
            return schema.Id;
        }

        public virtual Uri ResolveUri(Uri baseUri, Uri relativeUri)
        {
            if (baseUri == null || (!baseUri.IsAbsoluteUri && baseUri.OriginalString.Length == 0))
                return relativeUri;

            if (relativeUri == null)
                return baseUri;

            if (!baseUri.IsAbsoluteUri)
                return new Uri(baseUri.OriginalString + relativeUri.OriginalString, UriKind.RelativeOrAbsolute);

            return new Uri(baseUri, relativeUri);
        }

        public virtual ICredentials Credentials
        {
            set { }
        }
    }
}