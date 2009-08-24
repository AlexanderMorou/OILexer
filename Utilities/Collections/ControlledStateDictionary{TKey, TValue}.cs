using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// A generic dictionary whose keys and values are tightly controlled.
    /// </summary>
    /// <typeparam name="TKey">The type of element used as a key.</typeparam>
    /// <typeparam name="TValue">The type of element used as the values associated to the keys.</typeparam>
    [Serializable]
    public partial class ControlledStateDictionary<TKey, TValue> :
        ControlledStateCollection<KeyValuePair<TKey, TValue>>,
        IControlledStateDictionary<TKey, TValue>,
        IControlledStateDictionary,
        ISerializable,
        IDeserializationCallback
    {
        private KeyValuePair<TKey, TValue>[] serializationKVPs;
        #region ControlledStateDictionary Data members
        /// <summary>
        /// Data member which holds the <see cref="ControlledStateDictionary{TKey, TValue}"/> data.
        /// </summary>
        internal protected Dictionary<TKey, TValue> dictionaryCopy;

        /// <summary>
        /// Data member for <see cref="Keys"/>.
        /// </summary>
        private KeysCollection keysCollection;

        /// <summary>
        /// Data member for <see cref="Values"/>
        /// </summary>
        private ValuesCollection valuesCollection;


        #endregion
        #region ControlledStateDictionary<TKey, TValue> Constructors

        /// <summary>
        /// Creates a new <see cref="ControlledStateDictionary{TKey, TValue}"/> initialized to the
        /// default state.
        /// </summary>
        public ControlledStateDictionary()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Creates a new <see cref="ControlledStateDictionary{TKey, TValue}"/> with the <see cref="IDictionary{TKey, TValue}"/>
        /// to wrap.
        /// </summary>
        public ControlledStateDictionary(IDictionary<TKey, TValue> target)
            : base(target)
        {
            this.dictionaryCopy = (Dictionary<TKey, TValue>)base.baseCollection;
            this.keysCollection = new KeysCollection(this.dictionaryCopy.Keys);
            this.valuesCollection = new ValuesCollection(this.dictionaryCopy.Values);            
        }
        #endregion
        #region IControlledStateDictionary<TKey,TValue> Members

        /// <summary>
        /// Gets a <see cref="IControlledStateCollection{T}"/> containing the 
        /// <see cref="ControlledStateDictionary{TKey, TValue}"/>'s keys.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection{T}"/> with the keys of the 
        /// <see cref="ControlledStateDictionary{TKey, TValue}"/>.
        /// </returns>
        public virtual IControlledStateCollection<TKey> Keys
        {
            get {
                return this.keysCollection; }
        }

        /// <summary>
        /// Gets a <see cref="IControlledStateCollection{T}"/> containing the 
        /// <see cref="ControlledStateDictionary{TKey, TValue}"/>'s values.
        /// </summary>
        /// <returns>
        /// A <see cref="IControlledStateCollection{T}"/> with the values of the 
        /// <see cref="ControlledStateDictionary{TKey, TValue}"/>.
        /// </returns>
        public virtual IControlledStateCollection<TValue> Values
        {
            get { return this.valuesCollection; }
        }

        /// <summary>
        /// Returns the element of the <see cref="ControlledStateDictionary{TKey, TValue}"/> with the 
        /// given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to get.</param>
        /// <returns>The element with the specified <paramref name="key"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// There was no element in the <see cref="ControlledStateDictionary{TKey, TValue}"/> 
        /// containing the <paramref name="key"/> provided.
        /// </exception>
        public virtual TValue this[TKey key]
        {
            get {
                return this.dictionaryCopy[key];
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ControlledStateDictionary{TKey, TValue}"/> contains 
        /// an element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The <typeparamref name="TKey"/> to search for in the 
        /// <see cref="ControlledStateDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// true, if the <see cref="ControlledStateDictionary{TKey, TValue}"/> contains an element 
        /// with the <paramref name="key"/>; false, otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public virtual bool ContainsKey(TKey key)
        {
            return this.dictionaryCopy.ContainsKey(key);
        }

        /// <summary>
        /// Tries to obtain a value from the <see cref="ControlledStateDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="ControlledStateDictionary{TKey, TValue}"/></param>
        /// <param name="value">The value to return, if successful.</param>
        /// <returns>True, if the the element at <paramref name="key"/> is found; false otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dictionaryCopy.TryGetValue(key, out value);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IControlledStateDictionary Members

        IControlledStateCollection IControlledStateDictionary.Keys
        {
            get { return (IControlledStateCollection)this.Keys; }
        }

        IControlledStateCollection IControlledStateDictionary.Values
        {
            get { return (IControlledStateCollection)this.Values; }
        }

        object IControlledStateDictionary.this[object key]
        {
            get
            {
                if (!(key is TKey))
                    throw new ArgumentException("The key isn't properly typed", "key");
                return this[(TKey)key];
            }
        }

        bool IControlledStateDictionary.ContainsKey(object key)
        {
            if (!(key is TKey))
                throw new ArgumentException("The key isn't properly typed", "key");
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IControlledStateDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)this.GetEnumerator();
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((KeyValuePair<TKey,TValue>[])array, index);
        }

        bool ICollection.IsSynchronized
        {
            get 
            {
                return ((ICollection)this.baseCollection).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get {
                return ((ICollection)this.baseCollection).SyncRoot;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="ControlledStateDictionary{TKey, TValue}"/> with the <paramref name="info"/> and <paramref name="context"/>
        /// used for deserialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> used to deserialize the <see cref="ControlledStateDictionary{TKey, TValue}"/></param>
        /// <param name="context">The <see cref="StreamingContext"/> used to identify extra information about the deserialization process from the caller.</param>
        protected ControlledStateDictionary(SerializationInfo info, StreamingContext context)
            : this()
        {
            int count = info.GetInt32("Count");
            this.serializationKVPs = new KeyValuePair<TKey,TValue>[count];
            for (int i = 0; i < count; i++)
            {
                TKey keyCurrent = (TKey)info.GetValue(String.Format("KEY_{0:0000}",i), typeof(TKey));
                TValue valueCurrent = (TValue)info.GetValue(String.Format("VALUE_{0:0000}", i), typeof(TValue));
                serializationKVPs[i] = new KeyValuePair<TKey,TValue>(keyCurrent, valueCurrent);
            }

        }

        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data
        /// needed to serialize the target object.
        /// </summary>
        /// <param name="info">The destination (see <see cref="System.Runtime.Serialization.StreamingContext"/>) for this 
        /// serialization.</param>
        /// <param name="context">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Count", this.Count);
            for (int i = 0; i < this.Count; i++)
            {
                KeyValuePair<TKey, TValue> current = this[i];
                info.AddValue(String.Format("KEY_{0:0000}", i), current.Key);
                info.AddValue(String.Format("VALUE_{0:0000}", i), current.Value);
            }
        }

        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. 
        /// The functionality for this parameter is not currently implemented.</param>
        public virtual void OnDeserialization(object sender)
        {
            if (serializationKVPs == null)
                return;
            for (int i = 0; i < this.serializationKVPs.Length; i++)
                this.baseCollection.Add(serializationKVPs[i]);
        }

        #endregion

        /// <summary>
        /// Removes all instances from the <see cref="ControlledStateDictionary{TKey, TValue}"/>.
        /// </summary>
        protected void Clear()
        {
            base.baseCollection.Clear();
        }

        /// <summary>
        /// Removes the item at the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">A zero-based index which designates the <see cref="KeyValuePair{TKey, TItem}"/> to 
        /// remove from the <see cref="ControlledStateDictionary{TKey, TValue}"/></param>
        protected virtual void Remove(int index)
        {
            this.Remove(this.Keys[index]);
        }

        /// <summary>
        /// Removes the element under the provided <paramref name="key"/>
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to remove.</param>
        protected virtual void Remove(TKey key)
        {
            this.dictionaryCopy.Remove(key);
        }

        /// <summary>
        /// Adds an element with the given <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The lookup key from which the <paramref name="value"/> is referred to
        /// later.</param>
        /// <param name="value">The value of the element.</param>
        protected virtual void Add(TKey key, TValue value)
        {
            this.dictionaryCopy.Add(key, value);
        }
        /// <summary>
        /// Adds a series of <see cref="KeyValuePair{TKey, TValue}"/> instances.
        /// </summary>
        /// <param name="items">The <see cref="KeyValuePair{TKey, TValue}"/> array to add.</param>
        protected void AddRange(KeyValuePair<TKey, TValue>[] items)
        {
            foreach (KeyValuePair<TKey, TValue> item in items)
                this.Add(item.Key, item.Value);
        }
    }
}
