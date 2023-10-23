#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal class JSchemaDictionary : DictionaryBase<string, JSchema>, ICaseInsensitiveLookup<JSchema>
    {
        private readonly JSchema _parentSchema;

        public JSchemaDictionary(JSchema parentSchema, IDictionary<string, JSchema> dictionary)
            : base(dictionary)
        {
            _parentSchema = parentSchema;
        }

        public JSchemaDictionary(JSchema parentSchema)
            : this(parentSchema, new Dictionary<string, JSchema>(StringComparer.Ordinal))
        {
        }

        private void OnChanged()
        {
            _parentSchema.OnSelfChanged();
        }

        private void OnChildChanged(JSchema changedSchema)
        {
            _parentSchema.OnChildChanged(changedSchema);
        }

        protected override void AddItem(string key, JSchema value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            base.AddItem(key, value);
            value.Changed += OnChildChanged;
            OnChanged();
        }

        protected override void SetItem(string key, JSchema value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (TryGetValue(key, out JSchema s))
            {
                if (s == value)
                {
                    // new value is the same as the old value
                    return;
                }

                s.Changed -= OnChildChanged;
            }

            base.SetItem(key, value);
            value.Changed += OnChildChanged;
            OnChanged();
        }

        protected override bool RemoveItem(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!TryGetValue(key, out JSchema s))
            {
                return false;
            }

            s.Changed -= OnChildChanged;
            base.RemoveItem(key);
            OnChanged();
            return true;
        }

        protected override void ClearItems()
        {
            bool changed = false;
            foreach (KeyValuePair<string, JSchema> keyValuePair in this)
            {
                keyValuePair.Value.Changed -= OnChildChanged;
                changed = true;
            }

            base.ClearItems();
            if (changed)
            {
                OnChanged();
            }
        }

        public bool ContainsKey(string key, bool ignoreCase)
        {
            if (Dictionary is Dictionary<string, JSchema> dictionary)
            {
                return CollectionHelpers.ContainsKey(dictionary, key, ignoreCase);
            }
            else if (Dictionary is ICaseInsensitiveLookup<JSchema> caseInsensitiveLookup)
            {
                return caseInsensitiveLookup.ContainsKey(key, ignoreCase);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out JSchema? value, bool ignoreCase)
        {
            if (Dictionary is Dictionary<string, JSchema> dictionary)
            {
                return CollectionHelpers.TryGetValue(dictionary, key, out value, ignoreCase);
            }
            else if (Dictionary is ICaseInsensitiveLookup<JSchema> caseInsensitiveLookup)
            {
                return caseInsensitiveLookup.TryGetValue(key, out value, ignoreCase);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}