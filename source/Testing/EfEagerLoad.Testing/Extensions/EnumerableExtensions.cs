using System;
using System.Collections.Generic;

namespace EfEagerLoad.Testing.Extensions
{
    public static class EnumerableExtensions
    {

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) { return; }

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

    }
}
