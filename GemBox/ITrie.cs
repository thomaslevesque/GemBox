using System;
using System.Collections.Generic;

namespace GemBox
{
    public interface ITrie<TValue> : IDictionary<string, TValue>
    {
        /// <summary>
        /// Determines whether this trie contains the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>true if the trie contains the specified prefix; otherwise, false.</returns>
        bool ContainsPrefix(string prefix);

        /// <summary>
        /// Removes all key/value pairs whose key starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to remove.</param>
        /// <returns>The number of key/value pairs that were removed.</returns>
        int RemovePrefix(string prefix);

        /// <summary>
        /// Returns all key/value pairs whose key starts with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>A sequence of all key/value pairs whose key starts with <c>prefix</c>.</returns>
        IEnumerable<KeyValuePair<string, TValue>> FindPrefix(string prefix);

        /// <summary>
        /// Adds a key/value pair to the <see cref="Trie{TValue}"/> if the key does not already exist, or updates a key/value pair
        /// in the <see cref="Trie{TValue}"/> by using the specified function if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <returns>The new value for the key. This will be either be addValue (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        TValue AddOrUpdate(string key, TValue addValue, Func<string, TValue, TValue> updateValueFactory);

        /// <summary>
        /// Uses the specified functions to add a key/value pair to the <see cref="Trie{TValue}"/> if the key does not already exist,
        /// or to update a key/value pair in the <see cref="Trie{TValue}"/> if the key already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <returns>The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or the result of updateValueFactory (if the key was present).</returns>
        TValue AddOrUpdate(string key, Func<string, TValue> addValueFactory, Func<string, TValue, TValue> updateValueFactory);
    }
}