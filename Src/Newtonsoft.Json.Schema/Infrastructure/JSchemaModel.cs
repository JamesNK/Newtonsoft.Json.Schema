#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class JSchemaModel
    {
        public bool Required { get; set; }
        public JSchemaType Type { get; set; }
        public int? MinimumLength { get; set; }
        public int? MaximumLength { get; set; }
        public double? DivisibleBy { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public bool ExclusiveMinimum { get; set; }
        public bool ExclusiveMaximum { get; set; }
        public int? MinimumItems { get; set; }
        public int? MaximumItems { get; set; }
        public IList<string> Patterns { get; set; }
        public IList<JSchemaModel> Items { get; set; }
        public IDictionary<string, JSchemaModel> Properties { get; set; }
        public IDictionary<string, JSchemaModel> PatternProperties { get; set; }
        public JSchemaModel AdditionalProperties { get; set; }
        public JSchemaModel AdditionalItems { get; set; }
        public bool PositionalItemsValidation { get; set; }
        public bool AllowAdditionalProperties { get; set; }
        public bool AllowAdditionalItems { get; set; }
        public bool UniqueItems { get; set; }
        public IList<JToken> Enum { get; set; }
        public JSchemaType Disallow { get; set; }

        public JSchemaModel()
        {
            Type = JSchemaType.Any;
            AllowAdditionalProperties = true;
            AllowAdditionalItems = true;
            Required = false;
        }

        public static JSchemaModel Create(IList<JSchema> schemata)
        {
            JSchemaModel model = new JSchemaModel();

            foreach (JSchema schema in schemata)
            {
                Combine(model, schema);
            }

            return model;
        }

        private static void Combine(JSchemaModel model, JSchema schema)
        {
            // Version 3 of the Draft JSON Schema has the default value of Not Required
            model.Required = model.Required || (schema.Required ?? false);
            model.Type = model.Type & (schema.Type ?? JSchemaType.Any);

            model.MinimumLength = MathUtils.Max(model.MinimumLength, schema.MinimumLength);
            model.MaximumLength = MathUtils.Min(model.MaximumLength, schema.MaximumLength);

            // not sure what is the best way to combine divisibleBy
            model.DivisibleBy = MathUtils.Max(model.DivisibleBy, schema.DivisibleBy);

            model.Minimum = MathUtils.Max(model.Minimum, schema.Minimum);
            model.Maximum = MathUtils.Max(model.Maximum, schema.Maximum);
            model.ExclusiveMinimum = model.ExclusiveMinimum || (schema.ExclusiveMinimum ?? false);
            model.ExclusiveMaximum = model.ExclusiveMaximum || (schema.ExclusiveMaximum ?? false);

            model.MinimumItems = MathUtils.Max(model.MinimumItems, schema.MinimumItems);
            model.MaximumItems = MathUtils.Min(model.MaximumItems, schema.MaximumItems);
            model.PositionalItemsValidation = model.PositionalItemsValidation || schema.PositionalItemsValidation;
            model.AllowAdditionalProperties = model.AllowAdditionalProperties && schema.AllowAdditionalProperties;
            model.AllowAdditionalItems = model.AllowAdditionalItems && schema.AllowAdditionalItems;
            model.UniqueItems = model.UniqueItems || schema.UniqueItems;
            if (schema.Enum != null)
            {
                if (model.Enum == null)
                    model.Enum = new List<JToken>();

                model.Enum.AddRangeDistinct(schema.Enum, JToken.EqualityComparer);
            }
            model.Disallow = model.Disallow | (schema.Disallow ?? JSchemaType.None);

            if (schema.Pattern != null)
            {
                if (model.Patterns == null)
                    model.Patterns = new List<string>();

                model.Patterns.AddDistinct(schema.Pattern);
            }
        }
    }
}