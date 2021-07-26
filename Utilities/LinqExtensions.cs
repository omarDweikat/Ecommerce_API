using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce_API.Utilities
{
    public static class LinqExtensions
    {
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this ICollection source, Func<TSource, TResult> selector)
        {
            var list = new List<TResult>(source.Count);
            foreach (TSource item in source)
                list.Add(selector(item));
            return list;
        }

        public static IEnumerable<TSource> Where<TSource>(this ICollection source, Func<TSource, bool> projector)
        {
            var list = new List<TSource>(source.Count);
            foreach (TSource item in source)
                if (projector(item))
                    list.Add(item);

            return list;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }

        public static bool ContainsAll<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return !b.Except(a).Any();
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> list, int n)
        {
            return list.Skip(Math.Max(0, list.Count() - n));
        }

        // public static HashSet<T> ToHashSet<T>(this IEnumerable<T> data)
        // {
        //     return new HashSet<T>(data);
        // }
    }
}