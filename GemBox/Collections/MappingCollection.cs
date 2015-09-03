using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GemBox.Collections
{
    internal abstract class MappingCollection<TSource, TResult> : ICollection<TResult>
    {
        private readonly ICollection<TSource> _collection;
        private readonly Func<TSource, TResult> _projection;

        protected MappingCollection(ICollection<TSource> collection, Func<TSource, TResult> projection)
        {
            _collection = collection;
            _projection = projection;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return _collection.Select(_projection).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TResult item)
        {
            throw ReadOnlyException();
        }

        public void Clear()
        {
            throw ReadOnlyException();
        }

        public virtual bool Contains(TResult item)
        {
            return _collection.Select(_projection).Contains(item);
        }

        public void CopyTo(TResult[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var item in _collection)
            {
                array[i++] = _projection(item);
            }
        }

        public bool Remove(TResult item)
        {
            throw ReadOnlyException();
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        private NotSupportedException ReadOnlyException()
        {
            return new NotSupportedException("The collection is read-only");
        }
    }
}