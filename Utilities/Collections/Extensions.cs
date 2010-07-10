using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public static class Extensions
    {

        /// <summary>
        /// Performs <paramref name="f"/> on all <typeparamref name="T"/> intances in <paramref name="e"/>.
        /// </summary>
        /// <typeparam name="T">The type of members in <see cref="e"/>.</typeparam>
        /// <param name="e">The <see cref="IEnumerable{T}"/> which needs to have <paramref name="f"/> carried out on
        /// its entries.</param>
        /// <param name="f">A method with no return value that accepts values of <typeparamref name="T"/>.</param>
        public static void OnAll<T>(this IEnumerable<T> e, Action<T> f)
        {
            foreach (T t in e)
                f(t);
        }
        public static void OnAll<T1, T2>(this IEnumerable<T1> e, Action<T1, T2> f, T2 arg1)
        {
            foreach (T1 t in e)
                f(t, arg1);
        }
        public static void OnAll<T1, T2, T3>(this IEnumerable<T1> e, Action<T1, T2, T3> f, T2 arg1, T3 arg2)
        {
            foreach (T1 t in e)
                f(t, arg1, arg2);
        }
        public static void OnAll<T1, T2, T3, T4>(this IEnumerable<T1> e, Action<T1, T2, T3, T4> f, T2 arg1, T3 arg2, T4 arg3)
        {
            foreach (T1 t in e)
                f(t, arg1, arg2, arg3);
        }

        public static IList<TCallResult> OnAll<TItem, TCallResult>(this IEnumerable<TItem> e, Func<TItem, TCallResult> f)
        {
            List<TCallResult> result = new List<TCallResult>();
            foreach (TItem t in e)
                result.Add(f(t));
            return result;
        }

        public static IList<TCallResult> OnAll<TItem, TCallResult, T1>(this IEnumerable<TItem> e, Func<TItem, T1, TCallResult> f, T1 arg1)
        {
            List<TCallResult> result = new List<TCallResult>();
            foreach (TItem t in e)
                result.Add(f(t, arg1));
            return result;
        }

        public static IList<TCallResult> OnAll<TItem, TCallResult, T1, T2>(this IEnumerable<TItem> e, Func<TItem, T1, T2, TCallResult> f, T1 arg1, T2 arg2)
        {
            List<TCallResult> result = new List<TCallResult>();
            foreach (TItem t in e)
                result.Add(f(t, arg1, arg2));
            return result;
        }

        public static IList<TCallResult> OnAll<TItem, TCallResult, T1, T2, T3>(this IEnumerable<TItem> e, Func<TItem, T1, T2, T3, TCallResult> f, T1 arg1, T2 arg2, T3 arg3)
        {
            List<TCallResult> result = new List<TCallResult>();
            foreach (TItem t in e)
                result.Add(f(t, arg1, arg2, arg3));
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="IDictionary{TKey, TValue}"/> with the keys of <paramref name="source"/>
        /// and values as yielded by <paramref name="f"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the resulted <see cref="IDictionary{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of values in the resulted <see cref="IDictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="source">The data source for the <typeparamref name="TKey"/> instances for the resulted <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <param name="f">The delegate to manage the translation of the keys to the values.</param>
        /// <returns>A new <see cref="IDictionary{TKey, TValue}"/> with the elements of <paramref name="source"/> the results of the 
        /// </returns>
        public static IDictionary<TKey, TValue> ToDictionaryAlt<TKey, TValue>(this IEnumerable<TKey> source, Func<TKey, TValue> f)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            else if (f == null)
                throw new ArgumentNullException("f");
            Dictionary<TKey, TValue> r = new Dictionary<TKey, TValue>();
            source.OnAll((tk, target, method) => target.Add(tk, method(tk)), r, f);
            return r;
        }

        public static IEnumerable<T> Merge<T>(params IEnumerable<T>[] elements)
        {
            if (elements == null)
                throw new ArgumentException();
            return MergeInternal(elements);
        }

        private static IEnumerable<T> MergeInternal<T>(IEnumerable<T>[] elements)
        {
            foreach (var set in elements)
                if (set != null)
                    foreach (var t in set)
                        yield return t;
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> elements)
        {
            if (elements == null)
                throw new ArgumentException();
            var p = elements.Distinct();
            return MergeInternal(elements);
        }


        private static IEnumerable<T> MergeInternal<T>(IEnumerable<IEnumerable<T>> elements)
        {
            foreach (var set in elements)
                if (set != null)
                    foreach (var t in set)
                        yield return t;
        }

        public static IEnumerable<T> MergeDistinct<T>(this IEnumerable<IEnumerable<T>> elements)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");
            return elements.MergeDistinctInternal();
        }

        private static IEnumerable<T> MergeDistinctInternal<T>(this IEnumerable<IEnumerable<T>> elements)
        {
            HashSet<T> observed = new HashSet<T>();
            foreach (var set in elements)
                if (set != null)
                    foreach (var t in set)
                        if (observed.Add(t))
                            yield return t;
        }

    }
}
