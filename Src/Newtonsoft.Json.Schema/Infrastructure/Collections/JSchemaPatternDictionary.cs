#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json.Schema.Infrastructure.Collections
{
    internal class PatternSchema
    {
        private readonly JSchema _schema;
        private readonly string _pattern;
        private Regex _patternRegex;
        private string _patternError;

        public string Pattern
        {
            get { return _pattern; }
        }

        public JSchema Schema
        {
            get { return _schema; }
        }

        public PatternSchema(string pattern, JSchema schema)
        {
            _pattern = pattern;
            _schema = schema;
        }

        internal bool TryGetPatternRegex(out Regex regex, out string errorMessage)
        {
            bool result = RegexHelpers.TryGetPatternRegex(_pattern, ref _patternRegex, ref _patternError);
            regex = _patternRegex;
            errorMessage = _patternError;

            return result;
        }
    }

    internal class JSchemaPatternDictionary : IDictionary<string, JSchema>
    {
        private readonly Dictionary<string, PatternSchema> _inner;
        private ValuesCollection _values;

        public JSchemaPatternDictionary()
        {
            _inner = new Dictionary<string, PatternSchema>(StringComparer.Ordinal);
        }

        public IEnumerable<PatternSchema> GetPatternSchemas()
        {
            return _inner.Values;
        }

        public IEnumerator<KeyValuePair<string, JSchema>> GetEnumerator()
        {
            foreach (KeyValuePair<string, PatternSchema> patternSchema in _inner)
            {
                yield return new KeyValuePair<string, JSchema>(patternSchema.Key, patternSchema.Value.Schema);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, JSchema> item)
        {
            _inner.Add(item.Key, new PatternSchema(item.Key, item.Value));
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(KeyValuePair<string, JSchema> item)
        {
            if (!_inner.TryGetValue(item.Key, out PatternSchema patternSchema))
            {
                return false;
            }

            return (patternSchema.Schema == item.Value);
        }

        public void CopyTo(KeyValuePair<string, JSchema>[] array, int arrayIndex)
        {
            KeyValuePair<string, PatternSchema>[] tempArray = new KeyValuePair<string, PatternSchema>[array.Length];
            ((IDictionary<string, PatternSchema>)_inner).CopyTo(tempArray, arrayIndex);

            for (int i = 0; i < tempArray.Length; i++)
            {
                KeyValuePair<string, PatternSchema> item = tempArray[i];
                array[i] = new KeyValuePair<string, JSchema>(item.Key, item.Value.Schema);
            }
        }

        public bool Remove(KeyValuePair<string, JSchema> item)
        {
            if (!_inner.TryGetValue(item.Key, out PatternSchema patternSchema))
            {
                return false;
            }

            if (patternSchema.Schema != item.Value)
            {
                return false;
            }

            return _inner.Remove(item.Key);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<string, PatternSchema>)_inner).IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _inner.ContainsKey(key);
        }

        public void Add(string key, JSchema value)
        {
            _inner.Add(key, new PatternSchema(key, value));
        }

        public bool Remove(string key)
        {
            return _inner.Remove(key);
        }

        public bool TryGetValue(string key, out JSchema value)
        {
            if (_inner.TryGetValue(key, out PatternSchema patternSchema))
            {
                value = patternSchema.Schema;
                return true;
            }

            value = null;
            return false;
        }

        public JSchema this[string key]
        {
            get
            {
                PatternSchema patternSchema = _inner[key];
                return patternSchema.Schema;
            }
            set { _inner[key] = new PatternSchema(key, value); }
        }

        public ICollection<string> Keys
        {
            get { return _inner.Keys; }
        }

        public ICollection<JSchema> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new ValuesCollection(_inner.Values);
                }

                return _values;
            }
        }
    }

    internal class ValuesCollection : ICollection<JSchema>
    {
        private readonly ICollection<PatternSchema> _inner;

        public ValuesCollection(ICollection<PatternSchema> inner)
        {
            _inner = inner;
        }

        public IEnumerator<JSchema> GetEnumerator()
        {
            foreach (PatternSchema patternSchema in _inner)
            {
                yield return patternSchema.Schema;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(JSchema item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(JSchema item)
        {
            return _inner.Any(p => p.Schema == item);
        }

        public void CopyTo(JSchema[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(JSchema item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }
    }
}