#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System.Collections.ObjectModel;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal class JSchemaCollection : Collection<JSchema>
    {
        private readonly JSchema _parentSchema;

        public JSchemaCollection(JSchema parentSchema)
        {
            _parentSchema = parentSchema;
        }

        private void OnChanged()
        {
            _parentSchema.OnSelfChanged();
        }

        private void OnChildChanged(JSchema changedSchema)
        {
            _parentSchema.OnChildChanged(changedSchema);
        }

        protected override void InsertItem(int index, JSchema item)
        {
            base.InsertItem(index, item);

            item.Changed += OnChildChanged;
            OnChanged();
        }

        protected override void SetItem(int index, JSchema item)
        {
            JSchema s = this[index];
            s.Changed -= OnChildChanged;

            base.SetItem(index, item);

            item.Changed += OnChildChanged;
            OnChanged();
        }

        protected override void RemoveItem(int index)
        {
            JSchema s = this[index];
            s.Changed -= OnChildChanged;

            base.RemoveItem(index);
            OnChanged();
        }

        protected override void ClearItems()
        {
            foreach (JSchema schema in this)
            {
                schema.Changed -= OnChildChanged;
            }

            base.ClearItems();
            OnChanged();
        }
    }
}
