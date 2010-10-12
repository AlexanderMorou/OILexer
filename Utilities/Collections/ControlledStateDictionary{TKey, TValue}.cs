using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    /// <summary>
    /// A generic dictionary whose keys and values are tightly controlled.
    /// </summary>
    /// <typeparam name="TKey">The type of element used as a key.</typeparam>
    /// <typeparam name="TValue">The type of element used as the values associated to the keys.</typeparam>
    [Serializable]
    public partial class ControlledStateDictionary<TKey, TValue> :
        IControlledStateDictionary<TKey, TValue>,
        IControlledStateDictionary
    {
        private SharedLocals locals;


        public ControlledStateDictionary()
        {
            this.locals = new SharedLocals(InitializeKeysCollection, InitializeValuesCollection);
        }

        public ControlledStateDictionary(ControlledStateDictionary<TKey, TValue> sibling)
        {
            if (sibling == null)
                throw new ArgumentNullException("sibling");
            this.locals = sibling.locals;
        }

        public ControlledStateDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            if (entries == null)
                throw new ArgumentNullException("entries");
            this.locals = new SharedLocals(InitializeKeysCollection, InitializeValuesCollection);
            this.locals._AddRange(entries);
        }

        public KeysCollection Keys
        {
            get
            {
                return this.locals.Keys;
            }
        }

        protected virtual KeysCollection InitializeKeysCollection()
        {
            return new KeysCollection(this.locals);
        }

        public ValuesCollection Values
        {
            get
            {
                return this.locals.Values;
            }
        }

        protected virtual ValuesCollection InitializeValuesCollection()
        {
            return new ValuesCollection(this.locals);
        }

        #region IControlledStateDictionary<TKey,TValue> Members

        IControlledStateCollection<TKey> IControlledStateDictionary<TKey,TValue>.Keys
        {
            get {
                return this.Keys;
            }
        }

        IControlledStateCollection<TValue> IControlledStateDictionary<TKey,TValue>.Values
        {
            get { return this.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return OnGetThis(key);
            }
            protected set
            {
                OnSetThis(key, value);
            }
        }

        protected virtual void OnSetThis(TKey key, TValue value)
        {
            int index;
            if (this.locals.orderings.TryGetValue(key, out index))
                this.locals.entries[index] = new KeyValuePair<TKey, TValue>(key, value);
            else
                this.locals._Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        protected virtual TValue OnGetThis(TKey key)
        {
            return this.locals.entries[locals.orderings[key]].Value;
        }

        public virtual bool ContainsKey(TKey key)
        {
            return this.locals.orderings.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index;
            if (locals.orderings.TryGetValue(key, out index))
            {
                value = locals.entries[index].Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        #endregion

        #region IControlledStateCollection<KeyValuePair<TKey,TValue>> Members

        public int IndexOf(KeyValuePair<TKey, TValue> element)
        {
            int index;
            if (this.locals.orderings.TryGetValue(element.Key, out index))
            {
                if (this.locals.entries[index].Equals(element.Value))
                    return index;
            }
            return -1;
        }

        public virtual int Count
        {
            get { return this.locals.orderings.Count; }
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue checkedValue;
            if (TryGetValue(item.Key, out checkedValue))
            {
                if (checkedValue.Equals(item.Value))
                    return true;
            }
            return false;
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Array.ConstrainedCopy(this.locals.entries, 0, array, arrayIndex, this.Count);
        }

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                return OnGetThis(index);
            }
            protected set
            {
                value = OnSetThis(index, value);
            }
        }

        protected virtual KeyValuePair<TKey, TValue> OnSetThis(int index, KeyValuePair<TKey, TValue> value)
        {
            if (index < 0 ||
                index >= this.Count)
                throw new ArgumentOutOfRangeException("index");
            this.locals.entries[index] = value;
            return value;
        }

        protected virtual KeyValuePair<TKey, TValue> OnGetThis(int index)
        {
            if (index < 0 ||
                index >= this.Count)
                throw new ArgumentOutOfRangeException("index");
            return this.locals.entries[index];
        }

        public virtual KeyValuePair<TKey, TValue>[] ToArray()
        {
            KeyValuePair<TKey, TValue>[] result = new KeyValuePair<TKey, TValue>[this.Count];
            this.CopyTo(result, 0);
            return result;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
                yield return this.locals.entries[i];
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
            get { return this.Keys; }
        }

        IControlledStateCollection IControlledStateDictionary.Values
        {
            get { return this.Values; }
        }

        object IControlledStateDictionary.this[object key]
        {
            get {
                if (!(key is TKey))
                    throw new ArgumentException("key");
                return this[(TKey)key];
            }
        }

        public bool ContainsKey(object key)
        {
            if (!(key is TKey))
                throw new ArgumentException("key");
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IControlledStateDictionary.GetEnumerator()
        {
            return new SimpleDictionaryEnumerator<TKey, TValue>(this.GetEnumerator());
        }

        #endregion

        #region IControlledStateCollection Members

        bool IControlledStateCollection.Contains(object item)
        {
            if (!(item is KeyValuePair<TKey, TValue>))
                throw new ArgumentException("item");
            return this.Contains((KeyValuePair<TKey, TValue>)item);
        }

        void IControlledStateCollection.CopyTo(Array array, int arrayIndex)
        {
            ICollection_CopyTo(array, arrayIndex);
        }

        protected virtual void ICollection_CopyTo(Array array, int arrayIndex)
        {
            Array.ConstrainedCopy(this.locals.entries, 0, array, arrayIndex, this.Count);
        }

        object IControlledStateCollection.this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException("index");
                return this.locals.entries[index];
            }
        }

        int IControlledStateCollection.IndexOf(object element)
        {
            if (element is KeyValuePair<TKey, TValue>)
                return this.IndexOf((KeyValuePair<TKey, TValue>)element);
            return -1;
        }

        #endregion

        #region ICollection Members


        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            ((IControlledStateCollection)this).CopyTo(array, arrayIndex);
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

        protected internal virtual void _Add(TKey key, TValue value)
        {
            this.locals._Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        protected internal void _Add(KeyValuePair<TKey, TValue> item)
        {
            this._Add(item.Key, item.Value);
        }

        protected virtual void _AddRange(IEnumerable<KeyValuePair<TKey, TValue>> elements)
        {
            this.locals._AddRange(elements);
        }

        protected internal bool _Remove(TKey key)
        {
            int index;
            if (this.locals.orderings.TryGetValue(key, out index))
                this._Remove(index);
            return false;
        }

        protected internal virtual bool _Remove(int index)
        {
            return this.locals._Remove(index);
        }

        protected internal virtual void _Clear()
        {
            this.locals.Clear();
        }

        protected KeysCollection keysInstance
        {
            get
            {
                return this.locals.keysInstance;
            }
            set
            {
                this.locals.keysInstance = value;
            }
        }

        protected ValuesCollection valuesInstance
        {
            get
            {
                return this.locals.valuesInstance;
            }
            set
            {
                this.locals.valuesInstance = value;
            }
        }

    }
}
