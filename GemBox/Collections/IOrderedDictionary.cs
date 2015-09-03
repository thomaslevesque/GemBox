using System.Collections.Generic;

namespace GemBox.Collections
{
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        KeyValuePair<TKey, TValue> GetAt(int index);
        void Insert(int index, TKey key, TValue value);
        void RemoveAt(int index);
    }
}