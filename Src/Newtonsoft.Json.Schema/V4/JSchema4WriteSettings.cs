#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.V4.Infrastructure;

namespace Newtonsoft.Json.Schema.V4
{
    public class JSchema4WriteSettings
    {
        public JSchema4Resolver SchemaResolver { get; set; }
        public IList<ExternalSchema> ExternalSchemas { get; set; }
    }
}
