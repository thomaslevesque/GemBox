using System.Collections.Generic;
using System.Linq;

namespace GemBox
{
    public class Interval<T>
    {
        private readonly T _start;
        private readonly T _end;

        public Interval(T start, T end)
        {
            _start = start;
            _end = end;
        }

        public T Start
        {
            get { return _start; }
        }

        public T End
        {
            get { return _end; }
        }

        public bool OverlapsWith(Interval<T> other, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return this.Contains(other.Start, comparer)
                || this.Contains(other.End, comparer)
                || other.Contains(this.Start, comparer)
                || other.Contains(this.End, comparer);
        }

        public Interval<T> Union(Interval<T> other, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            T start = comparer.Min(this.Start, other.Start);
            T end = comparer.Max(this.End, other.End);
            return Interval.Create(start, end);
        }

        public Interval<T> Intersection(Interval<T> other, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            T start = comparer.Max(this.Start, other.Start);
            T end = comparer.Min(this.End, other.End);
            return Interval.Create(start, end);
        }

        public bool Contains(T value, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            return comparer.Compare(this.Start, value) <= 0
                && comparer.Compare(value, this.End) <= 0;
        }
    }

    public static class Interval
    {
        public static Interval<T> Create<T>(T start, T end)
        {
            return new Interval<T>(start, end);
        }
    }

    public static class IntervalExtensions
    {
        public static IEnumerable<Interval<T>> Consolidate<T>(this IEnumerable<Interval<T>> intervals)
        {
            return intervals.Consolidate(null);
        }

        public static IEnumerable<Interval<T>> Consolidate<T>(this IEnumerable<Interval<T>> intervals, IComparer<T> comparer)
        {
            comparer = comparer ?? Comparer<T>.Default;
            bool first = true;
            Interval<T> prev = null;
            intervals = intervals.OrderBy(i => i.Start, comparer)
                                 .ThenBy(i => i.End, comparer);
            foreach (var item in intervals)
            {
                if (first)
                {
                    prev = item;
                    first = false;
                    continue;
                }

                if (item.OverlapsWith(prev))
                {
                    prev = item.Union(prev);
                }
                else
                {
                    yield return prev;
                    prev = item;
                }
            }
            if (!first)
                yield return prev;
        }
    }
}
