using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GemBox.Threading;

namespace GemBox.Collections
{
    /// <summary>
    /// A Trie (prefix-tree) data structure that associates keys with values and
    /// allows fast prefix-based search.
    /// </summary>
    /// <typeparam name="TValue">The type of the values</typeparam>
    /// <remarks>Prefix-based search operations in this trie operate in O(p), where
    /// <c>p</c> is the length of the searched prefix.</remarks>
    public class Trie<TValue> : ITrie<TValue>
    {
        private readonly TrieNode _rootNode;

        /// <summary>
        /// Creates a new instance of <see cref="Trie{TValue}"/>.
        /// </summary>
        public Trie()
        {
            _rootNode = new TrieNode('\0', null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="Trie{TValue}"/> containing the specified items.
        /// </summary>
        /// <param name="entries">The items to include in the trie</param>
        public Trie(IEnumerable<KeyValuePair<string, TValue>> entries)
            : this()
        {
            if (entries == null) throw new ArgumentNullException("entries");
            foreach (var entry in entries)
            {
                Add(entry.Key, entry.Value);
            }
        }

        #region Core implementation

        private IEnumerable<KeyValuePair<string, TValue>> Enumerate(TrieNode root, string prefix)
        {
            string key;
            if (root == _rootNode)
                key = prefix + string.Empty;
            else
                key = prefix + root.PartialKey;

            if (root.HasValue)
                yield return new KeyValuePair<string, TValue>(key, root.Value);

            if (root.HasChildren)
            {
                foreach (var kvp in root.Children)
                {
                    foreach (var entry in Enumerate(kvp.Value, key))
                    {
                        yield return entry;
                    }
                }
            }
        }

        private static TrieNode FindNode(TrieNode root, char[] key, int depth, bool create, out bool created)
        {
            created = false;

            if (depth == key.Length)
                return root;

            TrieNode node = null;
            if (root.HasChildren && root.Children.TryGetValue(key[depth], out node))
                return FindNode(node, key, depth + 1, create, out created);

            if (create)
            {
                var current = root;
                for (int i = depth; i < key.Length; i++)
                {
                    node = new TrieNode(key[i], current);
                    current.Children.Add(key[i], node);
                    current = node;
                }
                created = true;
                return node;
            }

            return null;
        }

        private TrieNode FindNode(string key, bool create, out bool created)
        {
            return FindNode(_rootNode, key.ToCharArray(), 0, create, out created);
        }

        private TrieNode FindNode(string key, bool create)
        {
            bool created;
            return FindNode(_rootNode, key.ToCharArray(), 0, create, out created);
        }

        private static void Prune(TrieNode removedNode)
        {
            var current = removedNode;
            while (current != null)
            {
                if (current.HasChildren || current.HasValue)
                    break;
                current.Parent?.Children.Remove(current.PartialKey);
                current = current.Parent;
            }
        }

        class TrieNode
        {
            private TValue _value;
            private SortedDictionary<char, TrieNode> _children;

            public TrieNode(char partialKey, TrieNode parent)
            {
                PartialKey = partialKey;
                Parent = parent;
                _children = new SortedDictionary<char, TrieNode>();
            }

            public TrieNode Parent { get; }

            public char PartialKey { get; }

            public bool HasValue { get; private set; }

            public TValue Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    HasValue = true;
                }
            }

            public void ClearValue()
            {
                HasValue = false;
            }

            public IDictionary<char, TrieNode> Children => Atomic.LazyInit(ref _children);

            public bool HasChildren => _children?.Any() ?? false;

            public void ClearChildren()
            {
                _children?.Clear();
            }
        }

        #endregion

        #region IDictionary<string, TValue> implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return Enumerate(_rootNode, null).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified value to the <see cref="ICollection{T}"/> with the specified key.
        /// </summary>
        /// <param name="item">The key/value pair to add to the <see cref="Trie{TValue}"/>.</param>
        void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="Trie{TValue}"/>.
        /// </summary>
        public void Clear()
        {
            _rootNode.ClearValue();
            _rootNode.ClearChildren();
            Count = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific key and value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item)
        {
            TValue value;
            if (TryGetValue(item.Key, out value))
                return Equals(value, item.Value);
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            if (Count + arrayIndex > array.Length)
                throw new ArgumentException("The destination array is not large enough");
            int index = arrayIndex;
            foreach (var item in Enumerate(_rootNode, null))
            {
                array[index] = item;
                index++;
            }
        }

        /// <summary>
        /// Removes a key and value from the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="Trie{TValue}"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="Trie{TValue}"/>.
        /// </returns>
        /// <param name="item">The key/value pair to remove from the <see cref="Trie{TValue}"/>.</param>
        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
        {
            var node = FindNode(item.Key, false);
            if (node == null)
                return false;
            if (node.HasValue && Equals(node.Value, item.Value))
            {
                node.ClearValue();
                Count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Trie{TValue}"/>.
        /// </returns>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<string, TValue>>.IsReadOnly => false;

        /// <summary>
        /// Determines whether the <see cref="Trie{TValue}"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Trie{TValue}"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="Trie{TValue}"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var node = FindNode(key, false);
            if (node != null)
                return node.HasValue;
            return false;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="Trie{TValue}"/>.</exception>
        public void Add(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            var node = FindNode(key, true);
            if (node.HasValue)
                throw new ArgumentException("An element with the same key already exists in the trie.");
            node.Value = value;
            Count++;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="Trie{TValue}"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var node = FindNode(key, false);
            if (node == null)
                return false;

            if (node.HasValue)
            {
                node.ClearValue();
                Prune(node);
                Count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Trie{TValue}"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(string key, out TValue value)
        {
            value = default(TValue);
            var node = FindNode(key, false);
            if (node == null)
                return false;
            if (node.HasValue)
            {
                value = node.Value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception>
        public TValue this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException("key");
                TValue value;
                if (TryGetValue(key, out value))
                    return value;
                throw new KeyNotFoundException();
            }
            set
            {
                if (key == null) throw new ArgumentNullException("key");
                bool created;
                var node = FindNode(key, true, out created);
                node.Value = value;
                if (created)
                    Count++;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="Trie{TValue}"/>.
        /// </returns>
        public ICollection<string> Keys
        {
            get { return Enumerate(_rootNode, null).Select(kvp => kvp.Key).ToList(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="Trie{TValue}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="Trie{TValue}"/>.
        /// </returns>
        public ICollection<TValue> Values
        {
            get { return Enumerate(_rootNode, null).Select(kvp => kvp.Value).ToList(); }
        }

        #endregion

        #region ITrie<TValue> implementation

        /// <summary>
        /// Determines whether this trie contains the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>true if the trie contains the specified prefix; otherwise, false.</returns>
        public bool ContainsPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            var node = FindNode(prefix, false);
            return (node != null) && (node.HasValue || node.HasChildren);
        }

        /// <summary>
        /// Removes all key/value pairs whose key starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to remove.</param>
        /// <returns>The number of key/value pairs that were removed.</returns>
        public int RemovePrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            var node = FindNode(prefix, false);
            if (node == null)
                return 0;

            int count = Enumerate(node, null).Count();

            // Shouldn't happen if the trie is properly pruned...
            if (count == 0)
                return 0;

            node.ClearChildren();
            node.ClearValue();
            Prune(node);
            Count -= count;

            return count;
        }

        /// <summary>
        /// Returns all key/value pairs whose key starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>A sequence of all key/value pairs whose key starts with <paramref name="prefix"/>.</returns>
        public IEnumerable<KeyValuePair<string, TValue>> FindPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            return FindPrefixIterator(prefix);
        }

        private IEnumerable<KeyValuePair<string, TValue>> FindPrefixIterator(string prefix)
        {
            var node = FindNode(prefix, false);
            if (node == null)
                yield break;

            string prefix2 = null;
            if (prefix.Length > 0)
                prefix2 = prefix.Substring(0, prefix.Length - 1);
            foreach (var kvp in Enumerate(node, prefix2))
            {
                yield return kvp;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="Trie{TValue}"/> if the key does not already exist, or updates a key/value pair
        /// in the <see cref="Trie{TValue}"/> by using the specified function if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <returns>The new value for the key. This will be either be addValue (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        public TValue AddOrUpdate(string key, TValue addValue, Func<string, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate(key, k => addValue, updateValueFactory);
        }

        /// <summary>
        /// Uses the specified functions to add a key/value pair to the <see cref="Trie{TValue}"/> if the key does not already exist,
        /// or to update a key/value pair in the <see cref="Trie{TValue}"/> if the key already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <returns>The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        public TValue AddOrUpdate(string key, Func<string, TValue> addValueFactory, Func<string, TValue, TValue> updateValueFactory)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            var node = FindNode(key, true);
            if (node.HasValue)
            {
                node.Value = updateValueFactory(key, node.Value);
            }
            else
            {
                node.Value = addValueFactory(key);
                Count++;
            }
            return node.Value;
        }

        #endregion
    }
}
