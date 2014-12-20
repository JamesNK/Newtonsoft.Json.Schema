#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
    public class JSchemaWriteSettings
    {
        public JSchemaResolver SchemaResolver { get; set; }
        public IList<ExternalSchema> ExternalSchemas { get; set; }
    }
}
