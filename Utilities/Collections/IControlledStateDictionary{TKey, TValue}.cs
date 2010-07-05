using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// Defines properties and methods for working with a generic dictionary whose keys and values
    /// are tightly controlled.
    /// </summary>
    /// <typeparam name="TKey">The type of element used as a key.</typeparam>
    /// <typeparam name="TValue">The type of element used as the values associated to the keys.</typeparam>
    public interface IControlledStateDictionary<TKey, TValue> :
        IControlledStateCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets a <see cref="IControlledStateCollection{T}"/> containing the 
        /// <see cref="IControlledStateDictionary{TKey, TValue}"/>'s keys.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection{T}"/> with the keys of the 
        /// <see cref="IControlledStateDictionary{TKey, TValue}"/>.
        /// </returns>
        IControlledStateCollection<TKey> Keys { get; }
        /// <summary>
        /// Gets a <see cref="IControlledStateCollection{T}"/> containing the 
        /// <see cref="IControlledStateDictionary{TKey, TValue}"/>'s values.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection{T}"/> with the values of the 
        /// <see cref="IControlledStateDictionary{TKey, TValue}"/>.
        /// </returns>
        IControlledStateCollection<TValue> Values { get; }

        /// <summary>
        /// Returns the element of the <see cref="IControlledStateDictionary{TKey, TValue}"/> with the 
        /// given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to get.</param>
        /// <returns>The element with the specified <paramref name="key"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// There was no element in the <see cref="IControlledStateDictionary{TKey, TValue}"/> 
        /// containing the <paramref name="key"/> provided.
        /// </exception>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Determines whether the <see cref="IControlledStateDictionary{TKey, TValue}"/> contains 
        /// an element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The <typeparamref name="TKey"/> to search for in the 
        /// <see cref="IControlledStateDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// true, if the <see cref="IControlledStateDictionary{TKey, TValue}"/> contains an element 
        /// with the <paramref name="key"/>; false, otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        bool ContainsKey(TKey key);
        /// <summary>
        /// Tries to obtain a value from the <see cref="IControlledStateDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IControlledStateDictionary{TKey, TValue}"/></param>
        /// <param name="value">The value to return, if successful.</param>
        /// <returns>True, if the the element at <paramref name="key"/> is found; false otherwise.</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}
