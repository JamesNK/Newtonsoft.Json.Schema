#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaNode
    {
        public string Id { get; private set; }
        public ReadOnlyCollection<JSchema> Schemas { get; private set; }
        public Dictionary<string, JSchemaNode> Properties { get; private set; }
        public Dictionary<string, JSchemaNode> PatternProperties { get; private set; }
        public List<JSchemaNode> Items { get; private set; }
        public JSchemaNode AdditionalProperties { get; set; }
        public JSchemaNode AdditionalItems { get; set; }

        public JSchemaNode(JSchema schema)
        {
            Schemas = new ReadOnlyCollection<JSchema>(new[] { schema });
            Properties = new Dictionary<string, JSchemaNode>();
            PatternProperties = new Dictionary<string, JSchemaNode>();
            Items = new List<JSchemaNode>();

            Id = GetId(Schemas);
        }

        private JSchemaNode(JSchemaNode source, JSchema schema)
        {
            Schemas = new ReadOnlyCollection<JSchema>(source.Schemas.Union(new[] { schema }).ToList());
            Properties = new Dictionary<string, JSchemaNode>(source.Properties);
            PatternProperties = new Dictionary<string, JSchemaNode>(source.PatternProperties);
            Items = new List<JSchemaNode>(source.Items);
            AdditionalProperties = source.AdditionalProperties;
            AdditionalItems = source.AdditionalItems;

            Id = GetId(Schemas);
        }

        public JSchemaNode Combine(JSchema schema)
        {
            return new JSchemaNode(this, schema);
        }

        public static string GetId(IEnumerable<JSchema> schemata)
        {
            return string.Join("-", schemata.Select(s => s.InternalId).OrderBy(id => id, StringComparer.Ordinal).ToArray());
        }
    }
}