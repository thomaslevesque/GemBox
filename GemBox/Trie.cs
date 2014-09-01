using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GemBox
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
        private int _count;
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

            foreach (var kvp in root.Children)
            {
                foreach (var entry in Enumerate(kvp.Value, key))
                {
                    yield return entry;
                }
            }
        }

        private static TrieNode FindNode(TrieNode root, char[] key, int depth, bool create, out bool created)
        {
            created = false;

            if (depth == key.Length)
                return root;

            TrieNode node;
            if (root.Children.TryGetValue(key[depth], out node))
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
                if (current.Children.Any() || current.HasValue)
                    break;
                if (current.Parent != null)
                {
                    current.Parent.Children.Remove(current.PartialKey);
                }
                current = current.Parent;
            }
        }

        class TrieNode
        {
            private readonly char _partialKey;
            private TValue _value;
            private bool _hasValue;
            private readonly TrieNode _parent;
            private readonly SortedDictionary<char, TrieNode> _children;

            public TrieNode(char partialKey, TrieNode parent)
            {
                _partialKey = partialKey;
                _parent = parent;
                _children = new SortedDictionary<char, TrieNode>();
            }

            public TrieNode Parent
            {
                get { return _parent; }
            }

            public char PartialKey
            {
                get { return _partialKey; }
            }

            public bool HasValue
            {
                get { return _hasValue; }
            }

            public TValue Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    _hasValue = true;
                }
            }

            public void ClearValue()
            {
                _hasValue = false;
            }

            public IDictionary<char, TrieNode> Children
            {
                get { return _children; }
            }
        }

        #endregion

        #region IDictionary<string, TValue> implementation

        /// <summary>
        /// Retourne un énumérateur qui parcourt le dictionnaire
        /// </summary>
        /// <returns>Un énumerateur pour parcourir le dictionnaire</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return Enumerate(_rootNode, null).GetEnumerator();
        }

        /// <summary>
        /// Retourne un énumérateur qui parcourt le dictionnaire
        /// </summary>
        /// <returns>Un énumerateur pour parcourir le dictionnaire</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Ajoute la paire clé/valeur spécifiée au dictionnaire 
        /// </summary>
        /// <param name="item">Paire clé/valeur à ajouter</param>
        void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Supprime toutes les clés et les valeurs du dictionnaire.
        /// </summary>
        public void Clear()
        {
            _rootNode.ClearValue();
            _rootNode.Children.Clear();
            _count = 0;
        }

        /// <summary>
        /// Détermine si le dictionnaire contient la paire clé/valeur spécifiée.
        /// </summary>
        /// <param name="item">Paire clé/valeur recherchée</param>
        /// <returns>true si le dictionnaire contient la paire clé/valeur, false sinon</returns>
        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item)
        {
            TValue value;
            if (TryGetValue(item.Key, out value))
                return Equals(value, item.Value);
            return false;
        }

        /// <summary>
        /// Copie les éléments du dictionnaire dans un tableau, à partir de la position spécifiée
        /// </summary>
        /// <param name="array">Tableau de destination</param>
        /// <param name="arrayIndex">Index du tableau où commencer la copie</param>
        void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            if (_count + arrayIndex > array.Length)
                throw new ArgumentException("The destination array is not large enough");
            int index = arrayIndex;
            foreach (var item in Enumerate(_rootNode, null))
            {
                array[index] = item;
                index++;
            }
        }

        /// <summary>
        /// Supprime du dictionnaire la paire clé/valeur spécifiée.
        /// </summary>
        /// <param name="item">Paire clé/valeur</param>
        /// <returns>true si la recherche et la suppression de l'élément réussissent ; sinon, false.</returns>
        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
        {
            var node = FindNode(item.Key, false);
            if (node == null)
                return false;
            if (node.HasValue && Equals(node.Value, item.Value))
            {
                node.ClearValue();
                _count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient le nombre de paires clé/valeur contenues dans le dictionnaire.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le dictionnaire est en lecture seule.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Détermine si le dictionnaire contient la clé spécifiée. 
        /// </summary>
        /// <param name="key">Clé à rechercher</param>
        /// <returns>true si le dictionnaire contient un élément correspondant à la clé spécifiée ; sinon, false.</returns>
        public bool ContainsKey(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            var node = FindNode(key, false);
            if (node != null)
                return node.HasValue;
            return false;
        }

        /// <summary>
        /// Ajoute la clé et la valeur spécifiées au dictionnaire. 
        /// </summary>
        /// <param name="key">Clé de l'élément à ajouter. </param>
        /// <param name="value">Valeur de l'élément à ajouter. La valeur peut être null pour les types référence.</param>
        public void Add(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException("key");
            var node = FindNode(key, true);
            if (node.HasValue)
                throw new ArgumentException("An element with the same key already exists in the trie.");
            node.Value = value;
            _count++;
        }

        /// <summary>
        /// Supprime du dictionnaire la valeur ayant la clé spécifiée. 
        /// </summary>
        /// <param name="key">Clé de l'élément à supprimer.</param>
        /// <returns>true si la recherche et la suppression de l'élément réussissent ; sinon, false.</returns>
        public bool Remove(string key)
        {
            var node = FindNode(key, false);
            if (node == null)
                return false;

            if (node.HasValue)
            {
                node.ClearValue();
                Prune(node);
                _count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient la valeur associée à la clé spécifiée. 
        /// </summary>
        /// <param name="key">Clé de la valeur à obtenir.</param>
        /// <param name="value">Paramètre de sortie auquel est affecté la valeur trouvée</param>
        /// <returns>true si le dictionnaire contient un élément correspondant à la clé spécifiée ; sinon, false.</returns>
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
        /// Obtient ou définit la valeur associée à la clé spécifiée.
        /// </summary>
        /// <param name="key">Clé de l'élément à obtenir ou à définir</param>
        /// <value>Valeur associée à la clé spécifiée. Si la clé spécifiée est introuvable, une opération Get retourne KeyNotFoundException et une opération Set crée un nouvel élément avec la clé spécifiée.</value>
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
                    _count++;
            }
        }

        /// <summary>
        /// Obtient une collection contenant les clés du dictionnaire.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return Enumerate(_rootNode, null).Select(kvp => kvp.Key).ToList(); }
        }

        /// <summary>
        /// Obtient une collection contenant les valeurs du dictionnaire.
        /// </summary>
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
            return (node != null) && (node.HasValue || node.Children.Any());
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

            // Shouldn't happen if the tree is properly pruned...
            if (count == 0)
                return 0;

            node.Children.Clear();
            node.ClearValue();
            Prune(node);
            _count -= count;

            return count;
        }

        /// <summary>
        /// Returns all key/value pairs whose key starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>A sequence of all key/value pairs whose key starts with <c>prefix</c>.</returns>
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
            node.Value = node.HasValue
                ? addValueFactory(key)
                : updateValueFactory(key, node.Value);
            return node.Value;
        }

        #endregion
    }
}
