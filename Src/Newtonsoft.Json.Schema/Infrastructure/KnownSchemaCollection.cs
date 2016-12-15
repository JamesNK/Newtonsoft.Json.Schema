#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Schema.Infrastructure.Discovery;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class KnownSchemaCollection : KeyedCollection<JSchema, KnownSchema>
    {
        private Dictionary<Uri, KnownSchema> _uriKnownSchemaLookup;

        internal Dictionary<Uri, KnownSchema> UriKnownSchemaLookup
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

        protected override void InsertItem(int index, KnownSchema item)
        {
            UriKnownSchemaLookup[item.Id] = item;

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, KnownSchema item)
        {
            KnownSchema knownSchema = Items[index];

            if (!UriComparer.Instance.Equals(item.Id, knownSchema.Id))
            {
                UriKnownSchemaLookup.Remove(knownSchema.Id);
                UriKnownSchemaLookup[item.Id] = item;
            }

            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            KnownSchema knownSchema = Items[index];
            _uriKnownSchemaLookup?.Remove(knownSchema.Id);

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            _uriKnownSchemaLookup?.Clear();

            base.ClearItems();
        }

        protected override JSchema GetKeyForItem(KnownSchema item)
        {
            return item.Schema;
        }

        public KnownSchema GetById(Uri id)
        {
            if (_uriKnownSchemaLookup == null)
            {
                return null;
            }

            KnownSchema knownSchema;
            _uriKnownSchemaLookup.TryGetValue(id, out knownSchema);

            return knownSchema;
        }
    }
}
