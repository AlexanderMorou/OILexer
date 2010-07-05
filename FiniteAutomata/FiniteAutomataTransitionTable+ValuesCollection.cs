using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using System.Collections;

namespace Oilexer.FiniteAutomata
{
    partial class FiniteAutomataTransitionTable<TCheck, TState, TNodeTarget>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        protected class ValuesCollection :
            IControlledStateCollection<TNodeTarget>
        {
            private FiniteAutomataTransitionTable<TCheck, TState, TNodeTarget> owner;
            public ValuesCollection(FiniteAutomataTransitionTable<TCheck, TState, TNodeTarget> owner)
            {
                this.owner = owner;
            }

            #region IControlledStateCollection<TNodeTarget> Members

            public int Count
            {
                get { return this.owner.backup.Count; }
            }

            public virtual bool Contains(TNodeTarget item)
            {
                foreach (var node in this.owner.backup.Values)
                    if (object.ReferenceEquals(node.Target, item))
                        return true;
                return false;
            }

            public void CopyTo(TNodeTarget[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (arrayIndex + this.Count > array.Length)
                    throw new ArgumentException("array");
                int index = 0;
                foreach (var node in this.owner.backup.Values)
                    array[arrayIndex + index++] = node.Target;
            }

            public TNodeTarget this[int index]
            {
                get {
                    if (index < 0 ||
                        index >= this.Count)
                        throw new ArgumentOutOfRangeException("index");
                    int i = 0;
                    foreach (var node in this.owner.backup.Values)
                        if (i++ == index)
                            return node.Target;
                    return default(TNodeTarget);
                }
            }

            public TNodeTarget[] ToArray()
            {
                TNodeTarget[] result = new TNodeTarget[this.Count];
                this.CopyTo(result, 0);
                return result;
            }

            #endregion

            #region IEnumerable<TNodeTarget> Members

            public IEnumerator<TNodeTarget> GetEnumerator()
            {
                ICollection<IFiniteAutomataTransitionNode<TCheck, TNodeTarget>> nodes = this.owner.backup.Values;
                foreach (var node in nodes)
                    yield return node.Target;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region IControlledStateCollection<TNodeTarget> Members


            public int IndexOf(TNodeTarget element)
            {
                int index = -1;
                int i = 0;
                foreach (var currentElement in this.owner.Values)
                    if (currentElement.Equals(element))
                    {
                        index = i;
                        break;
                    }
                    else
                        i++;
                return index;
            }

            #endregion
        }
    }
}
