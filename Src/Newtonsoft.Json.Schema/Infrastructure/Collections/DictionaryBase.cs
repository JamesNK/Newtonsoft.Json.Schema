#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal abstract class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        protected DictionaryBase()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        protected DictionaryBase(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        protected DictionaryBase(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        protected DictionaryBase(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        protected DictionaryBase(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        protected DictionaryBase(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
        }

        protected virtual void AddItem(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        protected virtual void SetItem(TKey key, TValue value)
        {
            _dictionary[key] = value;
        }

        protected virtual bool RemoveItem(TKey key)
        {
            return _dictionary.Remove(key);
        }

        protected virtual void ClearItems()
        {
            _dictionary.Clear();
        }

        private static void VerifyKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!(key is TKey))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Keys is of type {0}.", typeof(TKey)), nameof(key));
            }
        }

        private static void VerifyValueType(object value)
        {
            if (!(value is TValue))
            {
                if (value != null ||
#if !PORTABLE
                    typeof(TValue).IsValueType
#else
                    typeof(TValue).GetTypeInfo().IsValueType
#endif
                    )
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Value is of type {0}.", typeof(TValue)), nameof(value));
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            AddItem(key, value);
        }

        public bool Remove(TKey key)
        {
            return RemoveItem(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set { SetItem(key, value); }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            AddItem(keyValuePair.Key, keyValuePair.Value);
        }

        public void Clear()
        {
            ClearItems();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(keyValuePair);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TValue value;
            if (TryGetValue(keyValuePair.Key, out value))
            {
                if (EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
                {
                    RemoveItem(keyValuePair.Key);
                    return true;
                }
            }
            return false;
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            VerifyKey(key);
            VerifyValueType(value);
            AddItem((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_dictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            VerifyKey(key);
            Remove((TKey)key);
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_dictionary)[key]; }
            set
            {
                VerifyKey(key);
                VerifyValueType(value);
                SetItem((TKey)key, (TValue)value);
            }
        }

        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_dictionary).Keys; }
        }

        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_dictionary).Values; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)_dictionary).IsReadOnly; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dictionary).CopyTo(array, index);
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_dictionary).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_dictionary).IsSynchronized; }
        }
    }
}