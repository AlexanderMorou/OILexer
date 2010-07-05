using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.Collections;
using Oilexer._Internal;
using System.Diagnostics;

namespace Oilexer.FiniteAutomata
{
    [DebuggerDisplay("Count = {Count}")]
    public abstract partial class FiniteAutomataTransitionTable<TCheck, TState, TNodeTarget> :
        IControlledStateDictionary<TCheck, TNodeTarget>,
        IFiniteAutomataTransitionTable<TCheck, TState, TNodeTarget>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        private KeysCollection keys;
        private ValuesCollection values;
        private Dictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TNodeTarget>> backup = new Dictionary<TCheck,IFiniteAutomataTransitionNode<TCheck,TNodeTarget>>();
        #region IFiniteAutomataTransitionTable<TCheck,TState,TNodeTarget> Members

        public abstract void Add(TCheck check, TNodeTarget target);

        #endregion

        #region IFiniteAutomataTransitionTable<TCheck,TState> Members

        public void AddState(TCheck check, TState target)
        {
            this.Add(check, GetStateTarget(target));
        }

        #endregion

        protected abstract TNodeTarget GetStateTarget(TState state);

        #region IControlledStateDictionary<TCheck,TNodeTarget> Members

        public IControlledStateCollection<TCheck> Keys
        {
            get
            {
                if (this.keys == null)
                    this.keys = this.InitializeKeys();
                return this.keys;
            }
        }

        public IControlledStateCollection<TNodeTarget> Values
        {
            get
            {
                if (this.values == null)
                    this.values = this.InitializeValues();
                return this.values;
            }
        }

        public TNodeTarget this[TCheck key]
        {
            get {
                if (!this.ContainsKey(key))
                    throw new KeyNotFoundException();
                return this.backup[key].Target;
            }
        }

        public bool ContainsKey(TCheck key)
        {
            return this.backup.ContainsKey(key);
        }

        public bool TryGetValue(TCheck key, out TNodeTarget value)
        {
            IFiniteAutomataTransitionNode<TCheck, TNodeTarget> node;
            if (this.backup.TryGetValue(key, out node))
            {
                value = node.Target;
                return true;
            }
            value = default(TNodeTarget);
            return false;
        }

        #endregion

        #region IControlledStateCollection<KeyValuePair<TCheck,TNodeTarget>> Members

        public int Count
        {
            get { return this.backup.Count; }
        }

        public bool Contains(KeyValuePair<TCheck, TNodeTarget> item)
        {
            IFiniteAutomataTransitionNode<TCheck, TNodeTarget> node;
            if (this.backup.TryGetValue(item.Key, out node))
                return object.ReferenceEquals(node.Target, item.Value);
            return false;
        }

        public void CopyTo(KeyValuePair<TCheck, TNodeTarget>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex + this.Count > array.Length)
                throw new ArgumentException("array");
            int i = 0;
            foreach (var item in this)
                array[arrayIndex + i++] = item;
        }

        public KeyValuePair<TCheck, TNodeTarget> this[int index]
        {
            get
            {
                if (index < 0 ||
                    index >= this.Count)
                    throw new ArgumentOutOfRangeException("index");
                int i = 0;
                foreach (var node in this.backup.Values)
                    if (i++ == index)
                        return new KeyValuePair<TCheck, TNodeTarget>(node.Check, node.Target);
                return default(KeyValuePair<TCheck, TNodeTarget>);
            }
        }

        public KeyValuePair<TCheck, TNodeTarget>[] ToArray()
        {
            KeyValuePair<TCheck, TNodeTarget>[] result = new KeyValuePair<TCheck, TNodeTarget>[this.Count];
            this.CopyTo(result, 0);
            return result;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TCheck,TNodeTarget>> Members

        public IEnumerator<KeyValuePair<TCheck, TNodeTarget>> GetEnumerator()
        {
            foreach (var kvpNode in this.backup)
                yield return new KeyValuePair<TCheck, TNodeTarget>(kvpNode.Key, kvpNode.Value.Target);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        protected virtual KeysCollection InitializeKeys()
        {
            return new KeysCollection(this);
        }

        protected virtual ValuesCollection InitializeValues()
        {
            return new ValuesCollection(this);
        }


        #region IFiniteAutomataTransitionTable<TCheck,TState> Members

        public void Remove(TCheck check)
        {
            if (!this.ContainsKey(check))
                return;
            this.backup.Remove(check);
        }

        public abstract IEnumerable<TState> Targets { get; }
        #endregion

        public TCheck GetColliders(TCheck condition, out  IDictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TNodeTarget>> colliders)
        {
            Dictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TNodeTarget>> result = new Dictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TNodeTarget>>();
            TCheck current = condition;
            foreach (TCheck key in this.backup.Keys)
            {
                var intersection = key.Intersect(current);
                if (intersection.IsEmpty)
                    continue;
                result.Add(intersection, this.backup[key]);
                current = current.SymmetricDifference(intersection);
                if (current.IsEmpty)
                    break;
            }
            colliders = result;
            return current;
        }

        public IFiniteAutomataTransitionNode<TCheck, TNodeTarget> GetNode(TCheck condition)
        {
            if (!this.ContainsKey(condition))
                throw new KeyNotFoundException();
            return this.backup[condition];
        }

        internal void AddInternal(TCheck check, TNodeTarget target)
        {
            lock(this.backup)
                this.backup.Add(check, new FiniteAutomataTransitionNode<TCheck, TNodeTarget>() { Check=check, Target = target });
        }

        public IEnumerable<TCheck> Checks
        {
            get
            {
                return this.backup.Keys;   
            }
        }

        public TCheck FullCheck
        {
            get
            {
                if (this.Count == 0)
                    return new TCheck();
                var fullCheck = default(TCheck);
                foreach (var check in this.Checks)
                {
                    fullCheck = check.Union(fullCheck);
                }
                return fullCheck;
            }
        }

        #region IControlledStateCollection<KeyValuePair<TCheck,TNodeTarget>> Members


        public int IndexOf(KeyValuePair<TCheck, TNodeTarget> element)
        {
            var tC = element.Key;
            var tT = element.Value;
            var index = this.Keys.IndexOf(tC);
            if (index != -1 && this.values[index].Equals(tT))
                return index;
            return -1;
        }

        #endregion
    }
}
