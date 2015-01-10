using System;
using System.Threading;

namespace GemBox.Threading
{
    static class Atomic
    {
        public static T LazyInit<T>(ref T location)
            where T : class, new()
        {
            if (location == null)
            {
                Interlocked.CompareExchange(ref location, new T(), null);
            }
            return location;
        }

        public static T LazyInit<T>(ref T location, Func<T> valueFactory)
            where T : class
        {
            if (location == null)
            {
                Interlocked.CompareExchange(ref location, valueFactory(), null);
            }
            return location;
        }
    }
}
