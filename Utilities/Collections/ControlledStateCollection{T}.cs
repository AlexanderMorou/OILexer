using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// A generic collection which is tightly controlled.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="ControlledStateCollection{T}"/></typeparam>
    public class ControlledStateCollection<T> :
        IControlledStateCollection<T>,
        IControlledStateCollection,
        IEnumerable<T>
    {
        /// <summary>
        /// The list to wrap.
        /// </summary>
        internal protected IList<T> baseList;

        #region ControlledStateCollection Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ControlledStateCollection{T}"/> with the 
        /// <paramref name="baseList"/> to wrap provided.
        /// </summary>
        /// <param name="baseList">The list to wrap.</param>
        public ControlledStateCollection(IList<T> baseList)
        {
            this.baseList = baseList;
        }

        /// <summary>
        /// Creates a new <see cref="ControlledStateCollection{T}"/> with a default state.
        /// </summary>
        public ControlledStateCollection()
            : this(new List<T>())
        {

        }

        #endregion

        #region IControlledStateCollection<T> Members
        /// <summary>:
        /// Gets the number of elements contained in the <see cref="ControlledStateCollection{T}"/>.
        /// </summary>
        ///
        /// <returns>
        /// The number of elements contained in the <see cref="ControlledStateCollection{T}"/>.
        /// </returns>
        public virtual int Count
        {
            get { return this.baseList.Count; }
        }

        /// <summary>
        /// Determines whether the <see cref="ControlledStateCollection{T}"/> contains a specific 
        /// value.</summary>
        /// <param name="item">
        /// The object to locate in the <see cref="ControlledStateCollection{T}"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ControlledStateCollection{T}"/>;
        /// otherwise, false.
        /// </returns>
        public virtual bool Contains(T item)
        {
            return baseList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ControlledStateCollection{T}"/> to an
        /// <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> 
        /// index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the 
        /// elements copied from <see cref="ControlledStateCollection{T}"/>. The 
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
        /// number of elements in the source <see cref="ControlledStateCollection{T}"/> is greater 
        /// than the available space from <paramref name="arrayIndex"/> to the 
        /// end of the destination <paramref name="array"/>.-or-Type <typeparamref name="T"/> 
        /// cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            this.baseList.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T element)
        {
            return baseList.IndexOf(element);
        }
        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ControlledStateCollection{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through
        /// the <see cref="ControlledStateCollection{T}"/>.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.baseList.GetEnumerator();
        }

        #endregion

        #region IControlledStateCollection Members
        bool IControlledStateCollection.Contains(object item)
        {
            if (!(item is T))
                return false;
            return this.Contains((T)item);
        }
        object IControlledStateCollection.this[int index]
        {
            get
            {
                return this[index];
            }
        }
        void IControlledStateCollection.CopyTo(Array array, int index)
        {
            this.CopyTo((T[])array, index);
        }


        int IControlledStateCollection.IndexOf(object element)
        {
            if (element is T)
                return this.IndexOf((T)element);
            return -1;
        }
        #endregion

        /// <summary>
        /// Translates the <see cref="ControlledStateCollection{T}"/> into a flat <see cref="System.Array"/>
        /// of <typeparamref name="T"/> elements.
        /// </summary>
        /// <returns>A new <see cref="System.Array"/> of <typeparamref name="T"/> instances.</returns>
        public virtual T[] ToArray()
        {
            T[] r = new T[this.Count];
            this.CopyTo(r, 0);
            return r;
        }
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the element at the index provided
        /// </summary>
        /// <param name="index">The index of the member to find.</param>
        /// <returns>The instance of <typeparamref name="T"/> at the index provided.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is  beyond the range of the 
        /// <see cref="ControlledStateCollection{T}"/>.
        /// </exception>
        public virtual T this[int index]
        {
            get
            {
                return this.baseList[index];
            }
        }
        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((T[])(array), index);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        #endregion

        protected virtual void AddImpl(T expression)
        {
            lock (baseList)
                this.baseList.Add(expression);
        }

        protected virtual void InsertItem(int index, T item)
        {
            lock (baseList)
            {
                this.baseList.Insert(index, item);
            }
        }

    }
}
