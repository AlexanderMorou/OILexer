using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Oilexer.Utilities.Common;

namespace Oilexer.Utilities.Arrays
{
    /// <summary>
    /// Provides methods that give small case changes to arrays.
    /// </summary>
    public static partial class Tweaks
    {
        /// <summary>
        /// Casts an array from the <typeparamref name="originatingType"/> to the <typeparamref name="desiredType"/>
        /// </summary>
        /// <typeparam name="desiredType">The desired type of members in the array.</typeparam>
        /// <typeparam name="originatingType">The originating type of the members in the array.</typeparam>
        /// <param name="array">The array of members to cast.</param>
        /// <returns>An array of the desired type of members.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="System.InvalidCastException">A member from <paramref name="array"/> could not be cast to the <typeparamref name="desiredType"/>.</exception>
        public static desiredType[] CastArray<desiredType, originatingType>(originatingType[] array)
        {
            /* *
             * Written prior to .NET 3.0
             * */
            return TranslateArray<originatingType, desiredType>
                (
                    array,
                    delegate(originatingType item)
                    {
                        if (item is desiredType)
                            return (desiredType)(object)item;
                        else
                            throw new InvalidCastException("A member of the source array cannot be cast to the desiredType");
                    }
                );
        }

        /// <summary>
        /// Logically filters an array using the iteration-logic delegate provided.
        /// </summary>
        /// <typeparam name="TItem">The type of item to filter.</typeparam>
        /// <param name="items">The array of items to filter.</param>
        /// <param name="logicDelegate">The delegate which will provide the iteration
        /// logic to perform the filtering.</param>
        /// <returns>A list of the items filtered by the means provided by <paramref name="logicDelegate"/>.</returns>
        public static TItem[] FilterArray<TItem>(TItem[] items, Predicate<TItem> logicDelegate)
        {
            List<TItem> resultItems = new List<TItem>(items.Length);
            foreach (TItem item in items)
                if (logicDelegate(item))
                    resultItems.Add(item);
            resultItems.TrimExcess();
            return resultItems.ToArray();
        }

        /// <summary>
        /// Alters the contents of the array by the return values indicated by the <paramref name="alterDelegate"/>.
        /// </summary>
        /// <typeparam name="TSourceItem">The type of items to alter in an array.</typeparam>
        /// <typeparam name="TDestinationItem">The type of items to return in the result array.</typeparam>
        /// <param name="items">The array to alter</param>
        /// <param name="alterDelegate">The delegate to perform the change on the elements
        /// of <paramref name="items"/>.</param>
        /// <returns></returns>
        public static TDestinationItem[] TranslateArray<TSourceItem, TDestinationItem>(TSourceItem[] items, Common.TranslateArgument<TDestinationItem, TSourceItem> alterDelegate)
        {
            List<TDestinationItem> resultItems = new List<TDestinationItem>(items.Length);
            foreach (TSourceItem item in items)
                resultItems.Add(alterDelegate(item));
            return resultItems.ToArray();
        }
        /// <summary>
        /// Alters the contents of the array and ignores others based upon the two 
        /// <see cref="Common.TranslateArgument{TResult, TArgument}"/> delegates.
        /// </summary>
        /// <typeparam name="TSourceItem">The type of the parameter accepted as input.</typeparam>
        /// <typeparam name="TDestinationItem">The type of result that is expected.</typeparam>
        /// <param name="items">The array to be logically and data-wise altered.</param>
        /// <param name="filter">The delegate that determines which elements of <paramref name="items"/> are
        /// kept in the result array.</param>
        /// <param name="translator">The delegate that alters the value of the elements in <paramref name="items"/>.</param>
        /// <returns>A new array that contains the specific items transformed as desired by the two iteration logic delegates.</returns>
        public static TDestinationItem[] TranslateFilteredArray<TSourceItem, TDestinationItem>
            (TSourceItem[] items, 
            Predicate<TSourceItem> filter, 
            Common.TranslateArgument<TDestinationItem, TSourceItem> translator)
        {
            //Setup space.
            List<TDestinationItem> resultItems = new List<TDestinationItem>(items.Length);
            //Iterate, filter, and translate.
            foreach (TSourceItem item in items)
                if (filter(item))
                    resultItems.Add(translator(item));
            //Remove blanks.
            resultItems.TrimExcess();

            return resultItems.ToArray();
        }
        public static T[] ProcessArray<T>(T[] array, ProcessArgument<T> d)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            T[] result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = d(array[i]);
            return result;
        }
        public static T[] MergeArrays<T>(params T[][] series)
        {
            if (series == null)
                throw new ArgumentNullException("series");
            int fullLength = 0;
            for (int i = 0; i < series.Length; i++)
                if (series[i] == null)
                    throw new ArgumentException("A member of series was null", "series");
                else
                    fullLength += series[i].Length;
            T[] result = new T[fullLength];
            for (int i = 0, offset = 0; i < series.Length && offset < fullLength; offset += series[i++].Length)
                series[i].CopyTo(result, offset);
            return result;
        }

        public static int[] SelectTo(this int start, int end)
        {
            int[] result = new int[(Math.Max(start, end) - Math.Min(start, end)) + 1];
            if (start > end)
            {
                for (int i = start, j = 0; i >= end;)
                    result[j++] = i--;
            }
            else
            {
                for (int i = start; i <= end; i++)
                    result[i] = i;
            }
            return result;
        }
    }
}