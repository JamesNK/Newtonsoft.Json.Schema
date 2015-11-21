#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
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

        public bool HasSchemas
        {
            get { return _schemasCount > 0; }
        }

        public Dictionary<string, object> GetInnerDictionary()
        {
            return (Dictionary<string, object>) Dictionary;
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
            JSchema s = value as JSchema;
            if (s != null)
            {
                _schemasCount++;
                s.Changed += OnChildChanged;
                OnChanged();
            }
        }

        protected override void SetItem(string key, object value)
        {
            bool changed = false;

            object o;
            if (TryGetValue(key, out o))
            {
                JSchema s1 = o as JSchema;
                if (s1 != null)
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
            JSchema s2 = value as JSchema;
            if (s2 != null)
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
            object o;
            if (!TryGetValue(key, out o))
            {
                return false;
            }

            JSchema s = o as JSchema;

            base.RemoveItem(key);
            if (s != null)
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
                JSchema s = keyValuePair.Value as JSchema;
                if (s != null)
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