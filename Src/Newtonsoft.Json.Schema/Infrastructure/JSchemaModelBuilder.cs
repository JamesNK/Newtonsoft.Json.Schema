#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaModelBuilder
    {
        private JSchemaNodeCollection _nodes = new JSchemaNodeCollection();
        private Dictionary<JSchemaNode, JSchemaModel> _nodeModels = new Dictionary<JSchemaNode, JSchemaModel>();
        private JSchemaNode _node;

        public JSchemaModel Build(JSchema schema)
        {
            _nodes = new JSchemaNodeCollection();
            _node = AddSchema(null, schema);

            _nodeModels = new Dictionary<JSchemaNode, JSchemaModel>();
            JSchemaModel model = BuildNodeModel(_node);

            return model;
        }

        public JSchemaNode AddSchema(JSchemaNode existingNode, JSchema schema)
        {
            string newId;
            if (existingNode != null)
            {
                if (existingNode.Schemas.Contains(schema))
                    return existingNode;

                newId = JSchemaNode.GetId(existingNode.Schemas.Union(new[] { schema }));
            }
            else
            {
                newId = JSchemaNode.GetId(new[] { schema });
            }

            if (_nodes.Contains(newId))
                return _nodes[newId];

            JSchemaNode currentNode = (existingNode != null)
                ? existingNode.Combine(schema)
                : new JSchemaNode(schema);

            _nodes.Add(currentNode);

            AddProperties(schema.Properties, currentNode.Properties);

            AddProperties(schema.PatternProperties, currentNode.PatternProperties);

            if (schema.Items != null)
            {
                for (int i = 0; i < schema.Items.Count; i++)
                {
                    AddItem(currentNode, i, schema.Items[i]);
                }
            }

            if (schema.AdditionalItems != null)
                AddAdditionalItems(currentNode, schema.AdditionalItems);

            if (schema.AdditionalProperties != null)
                AddAdditionalProperties(currentNode, schema.AdditionalProperties);

            if (schema.Extends != null)
            {
                foreach (JSchema jsonSchema in schema.Extends)
                {
                    currentNode = AddSchema(currentNode, jsonSchema);
                }
            }

            return currentNode;
        }

        public void AddProperties(IDictionary<string, JSchema> source, IDictionary<string, JSchemaNode> target)
        {
            if (source != null)
            {
                foreach (KeyValuePair<string, JSchema> property in source)
                {
                    AddProperty(target, property.Key, property.Value);
                }
            }
        }

        public void AddProperty(IDictionary<string, JSchemaNode> target, string propertyName, JSchema schema)
        {
            JSchemaNode propertyNode;
            target.TryGetValue(propertyName, out propertyNode);

            target[propertyName] = AddSchema(propertyNode, schema);
        }

        public void AddItem(JSchemaNode parentNode, int index, JSchema schema)
        {
            JSchemaNode existingItemNode = (parentNode.Items.Count > index)
                ? parentNode.Items[index]
                : null;

            JSchemaNode newItemNode = AddSchema(existingItemNode, schema);

            if (!(parentNode.Items.Count > index))
            {
                parentNode.Items.Add(newItemNode);
            }
            else
            {
                parentNode.Items[index] = newItemNode;
            }
        }

        public void AddAdditionalProperties(JSchemaNode parentNode, JSchema schema)
        {
            parentNode.AdditionalProperties = AddSchema(parentNode.AdditionalProperties, schema);
        }

        public void AddAdditionalItems(JSchemaNode parentNode, JSchema schema)
        {
            parentNode.AdditionalItems = AddSchema(parentNode.AdditionalItems, schema);
        }

        private JSchemaModel BuildNodeModel(JSchemaNode node)
        {
            JSchemaModel model;
            if (_nodeModels.TryGetValue(node, out model))
                return model;

            model = JSchemaModel.Create(node.Schemas);
            _nodeModels[node] = model;

            foreach (KeyValuePair<string, JSchemaNode> property in node.Properties)
            {
                if (model.Properties == null)
                    model.Properties = new Dictionary<string, JSchemaModel>();

                model.Properties[property.Key] = BuildNodeModel(property.Value);
            }
            foreach (KeyValuePair<string, JSchemaNode> property in node.PatternProperties)
            {
                if (model.PatternProperties == null)
                    model.PatternProperties = new Dictionary<string, JSchemaModel>();

                model.PatternProperties[property.Key] = BuildNodeModel(property.Value);
            }
            foreach (JSchemaNode t in node.Items)
            {
                if (model.Items == null)
                    model.Items = new List<JSchemaModel>();

                model.Items.Add(BuildNodeModel(t));
            }
            if (node.AdditionalProperties != null)
                model.AdditionalProperties = BuildNodeModel(node.AdditionalProperties);
            if (node.AdditionalItems != null)
                model.AdditionalItems = BuildNodeModel(node.AdditionalItems);

            return model;
        }
    }
}