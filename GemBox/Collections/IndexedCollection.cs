using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GemBox.Collections
{
    public class IndexedCollection<T> : IList<T>
    {
        private static readonly T[] _empty = new T[0];

        private readonly IList<T> _list = new List<T>();
        private readonly IList<Index> _indices = new List<Index>();

        public IIndex<TExpression> CreateIndex<TExpression>(Expression<Func<T, TExpression>> expression, IEqualityComparer<TExpression> comparer = null)
        {
            var index = new Index<TExpression>(expression, comparer);
            index.Build(_list);
            _indices.Add(index);
            return index;
        }

        public IIndex<TExpression> CreateUniqueIndex<TExpression>(Expression<Func<T, TExpression>> expression, IEqualityComparer<TExpression> comparer = null)
        {
            var index = new UniqueIndex<TExpression>(expression, comparer);
            index.Build(_list);
            _indices.Add(index);
            return index;
        }

        public IEnumerable<T> GetAllByIndex<TExpression>(IIndex<TExpression> index, TExpression key)
        {
            return ((IndexBase<TExpression>)index).GetAll(key);
        }

        public bool TryGetByIndex<TExpression>(IIndex<TExpression> index, TExpression key, out T item)
        {
            var uniqueIndex = index as UniqueIndex<TExpression>;
            if (uniqueIndex == null)
                throw new InvalidOperationException("index is not unique");
            return uniqueIndex.TryGet(key, out item);
        }

        public T GetByIndexOrDefault<TExpression>(IIndex<TExpression> index, TExpression key, T defaultValue = default(T))
        {
            T item;
            if (TryGetByIndex(index, key, out item))
                return item;
            return defaultValue;
        }

        public T GetByIndex<TExpression>(IIndex<TExpression> index, TExpression key)
        {
            T item;
            if (TryGetByIndex(index, key, out item))
                return item;
            throw new KeyNotFoundException("The given key was not present in the index.");
        }

        public bool ContainsByIndex<TExpression>(IIndex<TExpression> index, TExpression key)
        {
            return ((IndexBase<TExpression>)index).Contains(key);
        }

        public bool ContainsByIndex<TExpression>(IIndex<TExpression> index, TExpression key, T item)
        {
            return ((IndexBase<TExpression>)index).Contains(key, item);
        }

        public int RemoveByIndex<TExpression>(IIndex<TExpression> index, TExpression key)
        {
            var items = ((IndexBase<TExpression>)index).GetAll(key);
            int count = items.Count;
            foreach (var item in items)
            {
                Remove(item);
            }
            return count;
        }

        #region IList<T> implementation

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Add(T item)
        {
            foreach (var index in _indices)
            {
                index.EnsureCanAdd(item);
            }
            _list.Add(item);
            foreach (var index in _indices)
            {
                index.Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
            foreach (var index in _indices)
            {
                index.Clear();
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool removed = _list.Remove(item);
            if (removed)
            {
                foreach (var index in _indices)
                {
                    index.Remove(item);
                }
            }
            return removed;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            foreach (var indx in _indices)
            {
                indx.EnsureCanAdd(item);
            }
            _list.Insert(index, item);
            foreach (var indx in _indices)
            {
                indx.Add(item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            foreach (var indx in _indices)
            {
                indx.Remove(item);
            }
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                var oldItem = _list[index];
                foreach (var indx in _indices)
                {
                    indx.EnsureCanReplace(oldItem, value);
                }
                _list[index] = value;
                foreach (var indx in _indices)
                {
                    indx.Replace(oldItem, value);
                }
            }
        }

        #endregion

        private abstract class Index : IIndex
        {
            public abstract void EnsureCanAdd(T item);
            public abstract void Add(T item);
            public abstract void Clear();
            public abstract bool Remove(T item);
            public abstract void Replace(T oldItem, T newItem);
            public abstract void EnsureCanReplace(T oldItem, T newItem);
        }

        private abstract class IndexBase<TExpression> : Index, IIndex<TExpression>
        {
            private readonly Func<T, TExpression> _keyGetter;

            protected IndexBase(Expression<Func<T, TExpression>> expression)
            {
                _keyGetter = expression.Compile();
            }

            public virtual void Build(IList<T> items)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }

            public override void EnsureCanAdd(T item)
            {
                var key = _keyGetter(item);
                EnsureCanAddCore(key);
            }

            public override void Add(T item)
            {
                var key = _keyGetter(item);
                AddCore(key, item);
            }

            public override bool Remove(T item)
            {
                var key = _keyGetter(item);
                return RemoveCore(key, item);
            }

            public override void Replace(T oldItem, T newItem)
            {
                Remove(oldItem);
                Add(newItem);
            }

            public override void EnsureCanReplace(T oldItem, T newItem)
            {
                var oldKey = _keyGetter(oldItem);
                var newKey = _keyGetter(newItem);
                EnsureCanReplaceCore(oldKey, newKey);
            }

            public abstract IReadOnlyCollection<T> GetAll(TExpression key);

            protected abstract void EnsureCanAddCore(TExpression key);
            protected abstract void EnsureCanReplaceCore(TExpression oldKey, TExpression newKey);
            protected abstract void AddCore(TExpression key, T item);
            protected abstract bool RemoveCore(TExpression key);
            protected abstract bool RemoveCore(TExpression key, T item);
            public abstract bool Contains(TExpression key);
            public abstract bool Contains(TExpression key, T item);
        }

        private class Index<TExpression> : IndexBase<TExpression>
        {
            private readonly MultiValueDictionary<TExpression, T> _items;

            public Index(Expression<Func<T, TExpression>> expression, IEqualityComparer<TExpression> comparer)
                : base(expression)
            {
                _items = new MultiValueDictionary<TExpression, T>(comparer);
            }

            public override IReadOnlyCollection<T> GetAll(TExpression key)
            {
                return _items[key];
            }

            protected override void EnsureCanAddCore(TExpression key)
            {
            }

            protected override void EnsureCanReplaceCore(TExpression oldKey, TExpression newKey)
            {
            }

            protected override void AddCore(TExpression key, T item)
            {
                _items.Add(key, item);
            }

            protected override bool RemoveCore(TExpression key)
            {
                return _items.Remove(key);
            }

            protected override bool RemoveCore(TExpression key, T item)
            {
                return _items.Remove(key, item);
            }

            public override void Clear()
            {
                _items.Clear();
            }

            public override bool Contains(TExpression key)
            {
                return _items.ContainsKey(key);
            }

            public override bool Contains(TExpression key, T item)
            {
                return _items.Contains(key, item);
            }
        }

        private class UniqueIndex<TExpression> : IndexBase<TExpression>
        {
            private readonly IEqualityComparer<TExpression> _comparer;
            private readonly Dictionary<TExpression, T> _items;

            public UniqueIndex(Expression<Func<T, TExpression>> expression, IEqualityComparer<TExpression> comparer)
                : base(expression)
            {
                _comparer = comparer;
                _items = new Dictionary<TExpression, T>(comparer);
            }

            public override void Clear()
            {
                _items.Clear();
            }

            public override IReadOnlyCollection<T> GetAll(TExpression key)
            {
                T item;
                if (_items.TryGetValue(key, out item))
                    return new[] { item };
                return _empty;
            }

            protected override void EnsureCanAddCore(TExpression key)
            {
                if (_items.ContainsKey(key))
                    throw new ArgumentException("An item with the same key has already been added.");
            }

            protected override void EnsureCanReplaceCore(TExpression oldKey, TExpression newKey)
            {
                if (_comparer.Equals(oldKey, newKey))
                    return;
                EnsureCanAddCore(newKey);
            }

            protected override void AddCore(TExpression key, T item)
            {
                _items.Add(key, item);
            }

            protected override bool RemoveCore(TExpression key)
            {
                return _items.Remove(key);
            }

            protected override bool RemoveCore(TExpression key, T item)
            {
                T existingItem;
                if (_items.TryGetValue(key, out existingItem))
                {
                    if (Equals(item, existingItem))
                    {
                        _items.Remove(key);
                        return true;
                    }
                }
                return false;
            }

            public override bool Contains(TExpression key)
            {
                return _items.ContainsKey(key);
            }

            public override bool Contains(TExpression key, T item)
            {
                T existingItem;
                if (_items.TryGetValue(key, out existingItem))
                {
                    if (Equals(item, existingItem))
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool TryGet(TExpression key, out T item)
            {
                return _items.TryGetValue(key, out item);
            }
        }
    }

    public interface IIndex
    {
    }

    // ReSharper disable once UnusedTypeParameter (used for type inference)
    public interface IIndex<TExpression> : IIndex
    {
    }
}
