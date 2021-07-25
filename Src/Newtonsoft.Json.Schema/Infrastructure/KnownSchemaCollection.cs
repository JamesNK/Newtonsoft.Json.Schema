#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class KnownSchemaCollection : KeyedCollection<JSchema, KnownSchema>
    {
        private Dictionary<Uri, KnownSchema>? _uriKnownSchemaLookup;

        private Dictionary<JSchema, KnownSchema>? _jSchemaKnownSchemaLookup;

        private Dictionary<Uri, KnownSchema> UriKnownSchemaLookup
        {
            get
            {
                if (_uriKnownSchemaLookup == null)
                {
                    _uriKnownSchemaLookup = new Dictionary<Uri, KnownSchema>(UriComparer.Instance);
                }

                return _uriKnownSchemaLookup;
            }
        }

        private Dictionary<JSchema, KnownSchema> JSchemaKnownSchemaLookup
        {
            get
            {
                if (_jSchemaKnownSchemaLookup == null)
                {
                    _jSchemaKnownSchemaLookup = new Dictionary<JSchema, KnownSchema>();
                }

                return _jSchemaKnownSchemaLookup;
            }
        }

        protected override void InsertItem(int index, KnownSchema item)
        {
            // First Uri ID wins.
            if (!UriKnownSchemaLookup.ContainsKey(item.Id))
            {
                UriKnownSchemaLookup[item.Id] = item;
            }
            JSchemaKnownSchemaLookup[item.Schema] = item;

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, KnownSchema item)
        {
            KnownSchema knownSchema = Items[index];

            if (!UriComparer.Instance.Equals(item.Id, knownSchema.Id))
            {
                if (UriKnownSchemaLookup.TryGetValue(knownSchema.Id, out var existingKnownSchema) && knownSchema == existingKnownSchema)
                {
                    UriKnownSchemaLookup.Remove(knownSchema.Id);
                }
                // First Uri ID wins.
                if (!UriKnownSchemaLookup.ContainsKey(item.Id))
                {
                    UriKnownSchemaLookup[item.Id] = item;
                }

                JSchemaKnownSchemaLookup.Remove(knownSchema.Schema);
                JSchemaKnownSchemaLookup[item.Schema] = item;
            }

            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            KnownSchema knownSchema = Items[index];
            _uriKnownSchemaLookup?.Remove(knownSchema.Id);
            _jSchemaKnownSchemaLookup?.Remove(knownSchema.Schema);

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            _uriKnownSchemaLookup?.Clear();
            _jSchemaKnownSchemaLookup?.Clear();

            base.ClearItems();
        }

        protected override JSchema GetKeyForItem(KnownSchema item)
        {
            return item.Schema;
        }

        public KnownSchema? GetById(Uri id)
        {
            if (_uriKnownSchemaLookup == null)
            {
                return null;
            }

            _uriKnownSchemaLookup.TryGetValue(id, out KnownSchema knownSchema);

            return knownSchema;
        }

        public KnownSchema? GetByJSchema(JSchema jSchema)
        {
            if (_jSchemaKnownSchemaLookup == null)
            {
                return null;
            }

            _jSchemaKnownSchemaLookup.TryGetValue(jSchema, out KnownSchema knownSchema);

            return knownSchema;
        }
    }
}
