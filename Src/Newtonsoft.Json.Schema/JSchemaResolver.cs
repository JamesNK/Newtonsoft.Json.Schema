#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Net;

namespace Newtonsoft.Json.Schema
{
    public abstract class JSchemaResolver
    {
        public abstract JSchema GetSchema(Uri uri);

        public virtual ICredentials Credentials
        {
            set { }
        }
    }
}