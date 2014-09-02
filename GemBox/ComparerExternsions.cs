using System;
using System.Collections.Generic;

namespace GemBox
{
    public static class CompareExtensions
    {
        public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            return new ComparisonComparer<T>(comparison);
        }

        public static Comparison<T> ToComparison<T>(this IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            return comparer.Compare;
        }

        public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            return new ReverseComparer<T>(comparer);
        }

        public static Comparison<T> Reverse<T>(this Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            return (x, y) => comparison(y, x);
        }

        public static T Max<T>(this IComparer<T> comparer, T x, T y)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (comparer.Compare(x, y) >= 0)
                return x;
            return y;
        }

        public static T Min<T>(this IComparer<T> comparer, T x, T y)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (comparer.Compare(x, y) <= 0)
                return x;
            return y;
        }

        class ComparisonComparer<T> : Comparer<T>
        {
            private readonly Comparison<T> _comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public override int Compare(T x, T y)
            {
                return _comparison(x, y);
            }
        }

        class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> _baseComparer;

            public ReverseComparer(IComparer<T> baseComparer)
            {
                _baseComparer = baseComparer;
            }
            
            #region Implementation of IComparer<T>

            public int Compare(T x, T y)
            {
                return _baseComparer.Compare(y, x);
            }

            #endregion
        }
    }
}
