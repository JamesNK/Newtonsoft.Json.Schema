#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Newtonsoft.Json.Schema.Infrastructure.Discovery
{
    internal class KnownSchemaCollection : KeyedCollection<KnownSchemaKey, KnownSchema>
    {
        private Dictionary<KnownSchemaUriKey, KnownSchema>? _uriKnownSchemaLookup;

        private Dictionary<JSchema, KnownSchema>? _jSchemaKnownSchemaLookup;

        private Dictionary<KnownSchemaUriKey, KnownSchema> UriKnownSchemaLookup
        {
            get
            {
                if (_uriKnownSchemaLookup == null)
                {
                    _uriKnownSchemaLookup = new Dictionary<KnownSchemaUriKey, KnownSchema>();
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
            KnownSchemaUriKey key = KnownSchemaUriKey.Create(item);
            if (!UriKnownSchemaLookup.ContainsKey(key))
            {
                UriKnownSchemaLookup[key] = item;
            }
            JSchemaKnownSchemaLookup[item.Schema] = item;

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, KnownSchema item)
        {
            KnownSchema currentKnownSchema = Items[index];
            KnownSchemaUriKey currentKey = KnownSchemaUriKey.Create(currentKnownSchema);
            KnownSchemaUriKey argKey = KnownSchemaUriKey.Create(item);

            if (!currentKey.Equals(argKey))
            {
                if (UriKnownSchemaLookup.TryGetValue(currentKey, out var existingKnownSchema) && currentKnownSchema == existingKnownSchema)
                {
                    UriKnownSchemaLookup.Remove(currentKey);
                }
                // First Uri ID wins.
                if (!UriKnownSchemaLookup.ContainsKey(argKey))
                {
                    UriKnownSchemaLookup[argKey] = item;
                }

                JSchemaKnownSchemaLookup.Remove(currentKnownSchema.Schema);
                JSchemaKnownSchemaLookup[item.Schema] = item;
            }

            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            KnownSchema knownSchema = Items[index];
            KnownSchemaUriKey key = KnownSchemaUriKey.Create(knownSchema);

            _uriKnownSchemaLookup?.Remove(key);
            _jSchemaKnownSchemaLookup?.Remove(knownSchema.Schema);

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            _uriKnownSchemaLookup?.Clear();
            _jSchemaKnownSchemaLookup?.Clear();

            base.ClearItems();
        }

        protected override KnownSchemaKey GetKeyForItem(KnownSchema item)
        {
            return new KnownSchemaKey(item.Schema, item.DynamicScope);
        }

        public KnownSchema? GetById(KnownSchemaUriKey key)
        {
            if (_uriKnownSchemaLookup == null)
            {
                return null;
            }

            _uriKnownSchemaLookup.TryGetValue(key, out KnownSchema knownSchema);
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
