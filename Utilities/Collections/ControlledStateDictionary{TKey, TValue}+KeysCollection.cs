using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    partial class ControlledStateDictionary<TKey, TValue>
    {
        public class KeysCollection :
            IControlledStateCollection<TKey>,
            IControlledStateCollection
        {
            private SharedLocals locals;
            public KeysCollection(SharedLocals locals)
            {
                this.locals = locals;
            }

            protected KeysCollection(ControlledStateDictionary<TKey, TValue> localOwner)
            {
                this.locals = localOwner.locals;
            }
            #region IControlledStateCollection<TKey> Members

            /// <summary>:
            /// Gets the number of elements contained in the <see cref="KeysCollection"/>.</summary>
            /// <returns>
            /// The number of elements contained in the <see cref="KeysCollection"/>.</returns>
            public virtual int Count
            {
                get { return this.locals.Count; }
            }

            /// <summary>
            /// Determines whether the <see cref="KeysCollection"/> contains a specific 
            /// value.</summary>
            /// <param name="item">
            /// The object to locate in the <see cref="KeysCollection"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="KeysCollection"/>;
            /// otherwise, false.
            /// </returns>
            public virtual bool Contains(TKey item)
            {
                return this.locals.orderings.ContainsKey(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="KeysCollection"/> to an
            /// <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> 
            /// index.
            /// </summary>
            /// <param name="array">
            /// The one-dimensional <see cref="System.Array"/> that is the destination of the 
            /// elements copied from <see cref="KeysCollection"/>. The 
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
            /// number of elements in the source <see cref="KeysCollection"/> is greater 
            /// than the available space from <paramref name="arrayIndex"/> to the 
            /// end of the destination <paramref name="array"/>.-or-Type <typeparamref name="T"/> 
            /// cannot be cast automatically to the type of the destination
            /// <paramref name="array"/>.</exception>
            public virtual void CopyTo(TKey[] array, int arrayIndex = 0)
            {
                if (this.Count == 0)
                    return;
                if (arrayIndex < 0 || arrayIndex >= array.Length)
                    throw new ArgumentOutOfRangeException("arrayIndex");
                if (this.Count + arrayIndex > array.Length)
                    throw new ArgumentException("array");
                lock (this.locals.syncObject)
                    this.locals.orderings.Keys.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Returns the element at the index provided
            /// </summary>
            /// <param name="index">The index of the element to get.</param>
            /// <returns>The instance of <typeparamref name="T"/> at the index provided.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// <paramref name="index"/> is  beyond the range of the 
            /// <see cref="KeysCollection"/>.
            /// </exception>
            public virtual TKey this[int index]
            {
                get
                {
                    return OnGetKey(index);
                }
                internal protected set
                {
                    OnSetKey(index, value);
                }
            }

            protected virtual TKey OnGetKey(int index)
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException("index");
                return this.locals.entries[index].Key;
            }

            protected virtual void OnSetKey(int index, TKey value)
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException("index");
                var currentElement = this.locals.entries[index];
                if (this.locals.orderings.ContainsKey(value))
                    throw new ArgumentException("element with key already exists", "value");
                this.locals.orderings.Remove(currentElement.Key);
                this.locals.orderings.Add(value, index);
                this.locals.entries[index] = new KeyValuePair<TKey, TValue>(value, currentElement.Value);
            }

            /// <summary>
            /// Translates the <see cref="KeysCollection"/> into a flat <see cref="System.Array"/>
            /// of <typeparamref name="T"/> elements.
            /// </summary>
            /// <returns>A new <see cref="System.Array"/> of <typeparamref name="T"/> instances.</returns>
            public virtual TKey[] ToArray()
            {
                TKey[] result;
                lock (this.locals.syncObject)
                {
                    result = new TKey[this.Count];
                    this.locals.orderings.Keys.CopyTo(result, 0);
                }
                return result;
            }

            /// <summary>
            /// Returns the <see cref="Int32"/> ordinal index of the 
            /// <paramref name="element"/> provided.
            /// </summary>
            /// <param name="element">The <typeparamref name="T"/>
            /// instance to find within the <see cref="KeysCollection"/>.</param>
            /// <returns>-1 if the <paramref name="element"/> was not found within
            /// the <see cref="KeysCollection"/>; a positive <see cref="Int32"/>
            /// value indicating the ordinal index of <paramref name="element"/>
            /// otherwise.</returns>
            public virtual int IndexOf(TKey key)
            {
                int index;
                if (this.locals.orderings.TryGetValue(key, out index))
                    return index;
                return -1;
            }
            #endregion

            #region IEnumerable<TKey> Members

            public virtual IEnumerator<TKey> GetEnumerator()
            {
                for (int i = 0; i < this.Count; i++)
                    yield return this.locals.entries[i].Key;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region IControlledStateCollection Members


            bool IControlledStateCollection.Contains(object item)
            {
                if (!(item is TKey))
                    throw new ArgumentException("item");
                return this.Contains((TKey)item);
            }

            void IControlledStateCollection.CopyTo(Array array, int arrayIndex)
            {
                ICollection_CopyTo(array, arrayIndex);
            }

            protected virtual void ICollection_CopyTo(Array array, int arrayIndex)
            {
                if (arrayIndex < 0 || arrayIndex >= array.Length)
                    throw new ArgumentException("arrayIndex");
                if (this.Count + arrayIndex > array.Length)
                    throw new ArgumentException("array");
                lock (this.locals.syncObject)
                    for (int i = 0; i < this.Count; i++)
                        array.SetValue(this.locals.entries[i].Key, i + arrayIndex);
            }

            object IControlledStateCollection.this[int index]
            {
                get { return this[index]; }
            }

            int IControlledStateCollection.IndexOf(object element)
            {
                if (element is TKey)
                    return this.IndexOf((TKey)element);
                return -1;
            }
            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                ICollection_CopyTo(array, arrayIndex);
            }


            public bool IsSynchronized
            {
                get { return true; }
            }

            public object SyncRoot
            {
                get { return this.locals.syncObject; }
            }

            #endregion

            internal void Rekey(IEnumerable<Tuple<TKey, TKey>> oldNewSeries)
            {
                var cachedElements = oldNewSeries.ToArray();
                for (int i = 0; i < cachedElements.Length; i++)
                {
                    int currentIndex;
                    var current = cachedElements[i];
                    var oldKey = current.Item1;
                    if (this.locals.orderings.TryGetValue(oldKey, out currentIndex))
                    {
                        var newKey = current.Item2;
                        this.locals.orderings.Remove(oldKey);
                        this.locals.orderings.Add(newKey, currentIndex);
                        var currentElement = this.locals.entries[currentIndex].Value;
                        this.locals.entries[currentIndex] = new KeyValuePair<TKey, TValue>(newKey, currentElement);
                    }
                    else
                        throw new KeyNotFoundException(string.Format("The key in element {0} was not found.", i));
                }
            }

        }

    }
}
