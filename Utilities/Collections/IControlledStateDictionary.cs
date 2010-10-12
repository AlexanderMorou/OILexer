using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2010 Allen Copeland Jr.                                  |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// Defines properties and methods for working with a dictionary whose keys and values
    /// are tightly controlled.
    /// </summary>
    public interface IControlledStateDictionary :
        IControlledStateCollection
    {

        /// <summary>
        /// Gets a <see cref="IControlledStateCollection"/> containing the 
        /// <see cref="IControlledStateDictionary"/>'s keys.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection"/> with the keys of the 
        /// <see cref="IControlledStateDictionary"/>.
        /// </returns>
        IControlledStateCollection Keys { get; }

        /// <summary>
        /// Gets a <see cref="IControlledStateCollection"/> containing the 
        /// <see cref="IControlledStateDictionary"/>'s values.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection"/> with the values of the 
        /// <see cref="IControlledStateDictionary"/>.
        /// </returns>
        IControlledStateCollection Values { get; }

        /// <summary>
        /// Returns the element of the <see cref="IControlledStateDictionary"/> with the 
        /// given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element with the specified <paramref name="key"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// There was no element in the <see cref="IControlledStateDictionary"/> 
        /// containing the <paramref name="key"/> provided.
        /// </exception>
        object this[object key] { get; }

        /// <summary>
        /// Determines whether the <see cref="IControlledStateDictionary"/> contains 
        /// an element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to search for in the 
        /// <see cref="IControlledStateDictionary"/>.
        /// </param>
        /// <returns>
        /// true, if the <see cref="IControlledStateDictionary"/> contains an element 
        /// with the <paramref name="key"/>; false, otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        bool ContainsKey(object key);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="IControlledStateDictionary"/>.
        /// </summary>
        /// <returns>A <see cref="IDictionaryEnumerator"/> that can be used to iterate through
        /// the <see cref="IControlledStateDictionary"/>.</returns>
        new IDictionaryEnumerator GetEnumerator();

    }
}
