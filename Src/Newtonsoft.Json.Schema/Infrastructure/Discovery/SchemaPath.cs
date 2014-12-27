#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class SchemaPath
    {
        public readonly Uri Id;
        public readonly string Path;

        public SchemaPath(Uri id, string path)
        {
            Id = id;
            Path = path;
        }
    }
}