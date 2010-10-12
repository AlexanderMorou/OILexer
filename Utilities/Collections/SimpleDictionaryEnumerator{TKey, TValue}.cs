using System;
using System.Collections;
using System.Collections.Generic;
 /*---------------------------------------------------------------------\
 | Copyright © 2010 Allen Copeland Jr.                                  |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */


namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// Provides a generic simplified version of <see cref="IEnumerator{T}"/> for a <see cref="KeyValuePair{TKey, TValue}"/>
    /// implemented as an <see cref="IDictionaryEnumerator"/>.
    /// </summary>
    /// <typeparam name="TKey">The type to use as the key of the 
    /// <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/>.</typeparam>
    /// <typeparam name="TValue">The type to use as the element values of the 
    /// <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/>.</typeparam>
    public class SimpleDictionaryEnumerator<TKey, TValue> :
        IDictionaryEnumerator,
        IDisposable
    {
        /// <summary>
        /// Data member containing the original <see cref="IEnumerator{T}"/> that the
        /// <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/> is based upon.
        /// </summary>
        private IEnumerator<KeyValuePair<TKey, TValue>> original;

        /// <summary>
        /// Creates a new <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/> with the <paramref name="original"/>
        /// provided.
        /// </summary>
        /// <param name="original">The original <see cref="IEnumerator{T}"/> which enumerates a series of
        /// <see cref="KeyValuePair{TKey, TValue}"/> instances.</param>
        public SimpleDictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> original)
        {
            this.original = original;
        }
        #region IDictionaryEnumerator Members

        /// <summary>
        /// Gets both the key and the value of the current dictionary entry in the enumeration.
        /// </summary>
        /// <returns>A <see cref="DictionaryEntry"/> containing both the key and the value
        /// of the current dictionary entry.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/> is positioned before the first
        /// entry of the dictionary or after the last entry.</exception>
        public DictionaryEntry Entry
        {
            get { return new DictionaryEntry(this._Current.Key, this._Current.Value); }
        }

        /// <summary>
        /// Gets the key of the current dictionary entry.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/> is positioned before the first
        /// entry of the dictionary or after the last entry.</exception>
        public object Key
        {
            get { return this._Current.Key; }
        }

        /// <summary>Gets the value of the current dictionary entry.</summary>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/> is positioned before the first
        /// entry of the dictionary or after the last entry.</exception>
        public object Value
        {
            get { return this._Current.Value; }
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return this.Entry; }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false
        /// if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext()
        {
            return this.original.MoveNext();
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element
        /// in the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public void Reset()
        {
            this.original.Reset();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the current <see cref="SimpleDictionaryEnumerator{TKey, TValue}"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.original == null)
                return;
            this.original.Dispose();
            this.original = null;
        }

        #endregion

        private KeyValuePair<TKey, TValue> _Current
        {
            get
            {
                //Law of demeter?
                return this.original.Current;
            }
        }
    }
}
