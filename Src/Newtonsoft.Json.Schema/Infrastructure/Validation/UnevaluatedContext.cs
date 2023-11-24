#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Validation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class UnevaluatedContext
    {
        public UnevaluatedContext(SchemaScope schemaScope)
        {
            SchemaScope = schemaScope;
        }

        public SchemaScope SchemaScope { get; }
        public List<JSchema>? ValidScopes { get; set; }
        public bool Evaluated { get; set; }

        public void AddValidScope(JSchema schema)
        {
            if (ValidScopes == null)
            {
                ValidScopes = new List<JSchema>();
            }

            ValidScopes.Add(schema);
        }

        private string DebuggerDisplay => $"Evaluated = {Evaluated}, ValidScopes = {ValidScopes?.Count ?? 0}";
    }
}