using System.Collections;
using System.Collections.Generic;

namespace GemBox.Collections
{
    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly IDictionary<TKey, TValue> _dictionary;
        private readonly IList<KeyValuePair<TKey, TValue>> _entries;

        public OrderedDictionary() : this(0, null)
        {
        }

        public OrderedDictionary(int initialCapacity) : this(initialCapacity, null)
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        {
        }

        public OrderedDictionary(int initialCapacity, IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
            _dictionary = new Dictionary<TKey, TValue>(initialCapacity, comparer);
            _entries = new List<KeyValuePair<TKey, TValue>>(initialCapacity);
        } 

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item);
            _entries.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
            _entries.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _entries.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = IndexOfKey(item.Key);
            if (index < 0)
                return false;
            if (!EqualityComparer<TValue>.Default.Equals(item.Value, _entries[index].Value))
                return false;
            _entries.RemoveAt(index);
            _dictionary.Remove(item);
            return true;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _entries.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key)
        {
            int index = IndexOfKey(key);
            if (index < 0)
                return false;

            _entries.RemoveAt(index);
            _dictionary.Remove(key);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                if (_dictionary.ContainsKey(key))
                    _dictionary[key] = value;
                else
                    Add(key, value);
            }
        }

        public ICollection<TKey> Keys => new KeyCollection(_entries, this);

        public ICollection<TValue> Values => new ValueCollection(_entries);

        private int IndexOfKey(TKey key)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_comparer.Equals(key, _entries[i].Key))
                    return i;
            }
            return -1;
        }

        private class KeyCollection : MappingCollection<KeyValuePair<TKey, TValue>, TKey>
        {
            private readonly IDictionary<TKey, TValue> _dictionary;

            public KeyCollection(ICollection<KeyValuePair<TKey, TValue>> collection, IDictionary<TKey, TValue> dictionary)
                : base(collection, kvp => kvp.Key)
            {
                _dictionary = dictionary;
            }

            public override bool Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }
        }

        private class ValueCollection : MappingCollection<KeyValuePair<TKey, TValue>, TValue>
        {
            public ValueCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
                : base(collection, kvp => kvp.Value)
            {
            }
        }

        public KeyValuePair<TKey, TValue> GetAt(int index) => _entries[index];

        public void Insert(int index, TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _entries.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        public void RemoveAt(int index)
        {
            var key = _entries[index].Key;
            _entries.RemoveAt(index);
            _dictionary.Remove(key);
        }
    }
}
