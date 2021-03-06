#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal class JSchemaDependencyDictionary : DictionaryBase<string, object>
    {
        private readonly JSchema _parentSchema;
        private int _schemasCount;

        public JSchemaDependencyDictionary(JSchema parentSchema)
            : base(StringComparer.Ordinal)
        {
            _parentSchema = parentSchema;
        }

        public bool HasSchemas => _schemasCount > 0;

        public Dictionary<string, object> GetInnerDictionary()
        {
            return (Dictionary<string, object>)Dictionary;
        }

        private void OnChanged()
        {
            _parentSchema.OnSelfChanged();
        }

        private void OnChildChanged(JSchema changedSchema)
        {
            _parentSchema.OnChildChanged(changedSchema);
        }

        protected override void AddItem(string key, object value)
        {
            base.AddItem(key, value);
            if (value is JSchema s)
            {
                _schemasCount++;
                s.Changed += OnChildChanged;
                OnChanged();
            }
        }

        protected override void SetItem(string key, object value)
        {
            bool changed = false;

            if (TryGetValue(key, out object o))
            {
                if (o is JSchema s1)
                {
                    if (s1 == value)
                    {
                        // new value is the same as the old value
                        return;
                    }

                    _schemasCount--;
                    s1.Changed -= OnChildChanged;
                    changed = true;
                }
            }

            base.SetItem(key, value);
            if (value is JSchema s2)
            {
                _schemasCount++;
                s2.Changed += OnChildChanged;
                changed = true;
            }

            if (changed)
            {
                OnChanged();
            }
        }

        protected override bool RemoveItem(string key)
        {
            if (!TryGetValue(key, out object o))
            {
                return false;
            }

            base.RemoveItem(key);
            if (o is JSchema s)
            {
                _schemasCount--;
                s.Changed -= OnChildChanged;
                OnChanged();
            }
            return true;
        }

        protected override void ClearItems()
        {
            bool changed = false;

            foreach (KeyValuePair<string, object> keyValuePair in this)
            {
                if (keyValuePair.Value is JSchema s)
                {
                    s.Changed -= OnChildChanged;
                    changed = true;
                }
            }

            base.ClearItems();
            _schemasCount = 0;

            if (changed)
            {
                OnChanged();
            }
        }
    }
}