using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Oilexer.Utilities.Arrays;
using System.Collections.ObjectModel;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// Utility collection functions which have specialized functionality.
    /// </summary>
    public static class Special
    {
        public static T GetThisAt<T>(ICollection<T> col, int index)
        {
            int i = 0;
            foreach (T t in col)
                if (i++ == index)
                    return t;
            return default(T);
        }
        public static T GetThisAt<T>(Stack<T> col, int index)
        {
            int i = 0;
            foreach (T t in col)
                if (i++ == index)
                    return t;
            return default(T);
        }
        public static object[] GetCollectionItems(ICollection list)
        {
            object[] result = new object[list.Count];
            int index = 0;
            foreach (object o in list)
                result[index++] = o;
            return result;
        }
        public static T[] GetCollectionItems<T>(ICollection<T> list)
        {
            T[] result = new T[list.Count];
            list.CopyTo(result, 0);
            return result;
        }
        public static T[] GetCollectionItems<T>(IControlledStateCollection<T> list)
        {
            T[] result = new T[list.Count];
            list.CopyTo(result, 0);
            return result;
        }
        public static ICollection<T> InlineInsert<T>(ICollection<T> col, params T[] insert)
        {
            ICollection<T> result = new Collection<T>();
            foreach (T tC in col)
                result.Add(tC);
            foreach (T tI in insert)
                result.Add(tI);
            return result;
        }
        public static T[] InlineInsert<T>(T[] a, params T[] b)
        {
            T[] result = new T[a.Length + b.Length];
            a.CopyTo(result, 0);
            b.CopyTo(result, a.Length);
            return result;
        }
        public static ICollection<T> CollectionMerge<T>(ICollection<T> a, ICollection<T> b)
        {
            ICollection<T> result = new Collection<T>();
            foreach (T tA in a)
                result.Add(tA);
            foreach (T tB in b)
                result.Add(tB);
            return result;
        }
        #region AttributeCollection Methods

        /// <summary>
        /// Filters an <see cref="AttributeCollection"/> to a <typeparamref name="desiredType"/>.
        /// </summary>
        /// <typeparam name="desiredType">The desired type of <see cref="Attribute"/>.</typeparam>
        /// <param name="collection">The base <see cref="AttributeCollection"/> to be filtered.</param>
        /// <returns>A new <see cref="AttributeCollection"/>.</returns>
        /// <remarks><typeparamref name="desiredType"/> is not restricted to <see cref="Attribute"/> to allow for interface-based filtering.</remarks>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public static AttributeCollection FilterAttributeCollection<desiredType>(AttributeCollection collection)
        {
            Attribute[] items = new Attribute[collection.Count];
            collection.CopyTo(items, 0);
            return new AttributeCollection(
                Tweaks.FilterArray
                (
                    items, 
                    delegate(Attribute item)
                    {
                        return item is desiredType;
                    }
                )
            );
        }

        /// <summary>
        /// Filters an array of <typeparamref name="originatingType"/> to a list of
        /// <typeparamref name="desiredType"/> which is stored in an <see cref="AttributeCollection"/>
        /// </summary>
        /// <typeparam name="desiredType">The desired <see cref="System.Type"/> that the filter processes to.</typeparam>
        /// <typeparam name="originatingType">The <see cref="System.Type"/> that the items in the array consist of.</typeparam>
        /// <param name="collection">The array of <typeparamref name="originatingType"/> that is to 
        /// be filtered.</param>
        /// <returns>An <see cref="AttributeCollection"/> containing the instances of <paramref name="collection"/> that
        /// are also instances of <typeparamref name="desiredType"/>.</returns>
        public static AttributeCollection FilterToAttributeCollection<desiredType, originatingType>(originatingType[] collection)
        {
            int validCount = 0;
            for (int i = 0; i < collection.Length; i++)
                if (collection[i] is desiredType)
                    validCount++;
            int j = 0;
            Attribute[] result = new Attribute[validCount];
            foreach (originatingType attr in collection)
                if (attr is desiredType)
                {
                    result[j] = (Attribute)(object)attr;
                    j++;
                }
            return new AttributeCollection(result);
        }

        /// <summary>
        /// Filters an <see cref="AttributeCollection"/> to a <paramref name="desiredType"/>.
        /// </summary>
        /// <param name="desiredType">The desired type of <see cref="Attribute"/>.</param>
        /// <param name="collection">The base <see cref="AttributeCollection"/> to be filtered.</param>
        /// <returns>A new <see cref="AttributeCollection"/>.</returns>
        /// <remarks><paramref name="desiredType"/> is not restricted to <see cref="Attribute"/> to allow for interface-based filtering.</remarks>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public static AttributeCollection FilterAttributeCollection(AttributeCollection collection, System.Type desiredType)
        {
            Attribute[] items = new Attribute[collection.Count];
            collection.CopyTo (items, 0);
            return new AttributeCollection(
                    Tweaks.FilterArray(items, delegate(Attribute item)
                    {
                        return item.GetType() == desiredType;
                    })
                );
        }
        #endregion
        public static TKey ReverseLookup<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
                if (kvp.Value.Equals(value))
                    return kvp.Key;
            return default(TKey);
        }
        public static void RekeyDictionaryItem<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TValue item, TKey newKey)
        {
            int itemIndex = -1;
            int index = 0;
            foreach (KeyValuePair<TKey, TValue> kvpItem in dictionary)
            {
                if (kvpItem.Value.Equals(item))
                {
                    if (kvpItem.Key.Equals(newKey))
                        return;
                    itemIndex = index;
                }
                index++;
            }
            if (itemIndex == -1)
                return;
            KeyValuePair<TKey, TValue>[] newList = new KeyValuePair<TKey, TValue>[dictionary.Count];
            dictionary.CopyTo(newList, 0);
            newList[itemIndex] = new KeyValuePair<TKey, TValue>(newKey, item);
            dictionary.Clear();
            AddSeries(dictionary, newList);
        }

        public static void AddSeries<T>(ICollection<T> collection, T[] series)
        {
            for (int i = 0; i < series.Length; i++)
                collection.Add(series[i]);
        }
    }
}
