using System;
using System.Collections.Generic;

namespace CocosInjector.InjectorHost.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> kvps,
            Action<TKey, TValue> action)
        {
            foreach (var kvp in kvps)
                action(kvp.Key, kvp.Value);
        }

        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
        {
            foreach (var obj in objs)
                action(obj);
        }
    }
}