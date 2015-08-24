using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.Infrastructure;

namespace Newtonsoft.Json.Schema
{
    internal class PatternSchema
    {
        private readonly JSchema _schema;
        private readonly string _pattern;
        private Regex _patternRegex;
        private string _patternError;

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

    internal class PatternPropertyDictionary : IDictionary<string, JSchema>
    {
        private readonly Dictionary<string, PatternSchema> _inner;

        public PatternPropertyDictionary()
        {
            _inner = new Dictionary<string, PatternSchema>(StringComparer.Ordinal);
        }

        public IEnumerable<KeyValuePair<string, PatternSchema>> GetPatternSchemas()
        {
            return _inner;
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
            PatternSchema patternSchema;
            if (!_inner.TryGetValue(item.Key, out patternSchema))
            {
                return false;
            }

            return (patternSchema.Schema == item.Value);
        }

        public void CopyTo(KeyValuePair<string, JSchema>[] array, int arrayIndex)
        {
            KeyValuePair<string, PatternSchema>[] tempArray = new KeyValuePair<string, PatternSchema>[array.Length];
            ((IDictionary<string, PatternSchema>) _inner).CopyTo(tempArray, arrayIndex);

            for (int i = 0; i < tempArray.Length; i++)
            {
                KeyValuePair<string, PatternSchema> item = tempArray[i];
                array[i] = new KeyValuePair<string, JSchema>(item.Key, item.Value.Schema); 
            }
        }

        public bool Remove(KeyValuePair<string, JSchema> item)
        {
            PatternSchema patternSchema;
            if (!_inner.TryGetValue(item.Key, out patternSchema))
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
            get { return ((IDictionary<string, PatternSchema>) _inner).IsReadOnly; }
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
            PatternSchema patternSchema;
            if (_inner.TryGetValue(key, out patternSchema))
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
            get { throw new NotSupportedException(); }
        }

        public ICollection<JSchema> Values
        {
            get { throw new NotSupportedException(); }
        }
    }
}
