using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// Defines properties and methods for working with a generic collection which is tightly controlled.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="IControlledStateCollection{T}"/></typeparam>
    public interface IControlledStateCollection<T> :
        IEnumerable<T>, 
        IEnumerable
    {
        /// <summary>:
        /// Gets the number of elements contained in the <see cref="IControlledStateCollection{T}"/>.</summary>
        ///
        /// <returns>
        /// The number of elements contained in the <see cref="IControlledStateCollection{T}"/>.</returns>
        int Count { get; }


        /// <summary>
        /// Determines whether the <see cref="IControlledStateCollection{T}"/> contains a specific 
        /// value.</summary>
        /// <param name="item">
        /// The object to locate in the <see cref="IControlledStateCollection{T}"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="IControlledStateCollection{T}"/>;
        /// otherwise, false.
        /// </returns>
        bool Contains(T item);
        /// <summary>
        /// Copies the elements of the <see cref="IControlledStateCollection{T}"/> to an
        /// <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> 
        /// index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the 
        /// elements copied from <see cref="IControlledStateCollection{T}"/>. The 
        /// <see cref="System.Array"/> must
        /// have zero-based indexing.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> 
        /// is equal to or greater than the length of <paramref name="array"/>.-or-The 
        /// number of elements in the source <see cref="IControlledStateCollection{T}"/> is greater 
        /// than the available space from <paramref name="arrayIndex"/> to the 
        /// end of the destination <paramref name="array"/>.-or-Type <typeparamref name="T"/> 
        /// cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Returns the element at the index provided
        /// </summary>
        /// <param name="index">The index of the member to find.</param>
        /// <returns>The instance of <typeparamref name="T"/> at the index provided.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is  beyond the range of the 
        /// <see cref="IControlledStateCollection{T}"/>.
        /// </exception>
        T this[int index] { get; }

        /// <summary>
        /// Translates the <see cref="IControlledStateCollection{T}"/> into a flat <see cref="System.Array"/>
        /// of <typeparamref name="T"/> elements.
        /// </summary>
        /// <returns>A new <see cref="System.Array"/> of <typeparamref name="T"/> instances.</returns>
        T[] ToArray();
    }
}
