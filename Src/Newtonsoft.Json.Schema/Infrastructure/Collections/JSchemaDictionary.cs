using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal class JSchemaDictionary : DictionaryBase<string, JSchema>
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
            base.AddItem(key, value);
            value.Changed += OnChildChanged;
            OnChanged();
        }

        protected override void SetItem(string key, JSchema value)
        {
            JSchema s;
            if (TryGetValue(key, out s))
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
            JSchema s;
            if (!TryGetValue(key, out s))
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
    }
}