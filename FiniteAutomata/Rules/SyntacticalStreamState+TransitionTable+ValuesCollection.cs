using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Oilexer.Utilities.Collections;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        #region SyntacticalStreamState nested types
        partial class TransitionTable
        {
            #region TransitionTable nested types
            private class ValuesCollection :
                IControlledStateCollection<SyntacticalStreamState>
            {
                #region ValuesCollection data members
                private TransitionTable owner;
                #endregion // ValuesCollection data members
                #region ValuesCollection properties
                public int Count
                {
                    get
                    {
                        lock (this.owner.dataSource)
                            return this.owner.dataSource.Count;
                    }
                }

                public SyntacticalStreamState this[int index]
                {
                    get
                    {
                        return this.owner[index].Value;
                    }
                }
                #endregion // ValuesCollection properties
                #region ValuesCollection methods
                public bool Contains(SyntacticalStreamState value)
                {
                    lock (this.owner.dataCopy)
                        if (Enumerable.Contains<SyntacticalStreamState>(this.owner.dataCopy.Values, value))
                        {
                            // A quick check to the cache.
                            return true;
                        }
                        else
                        {
                            lock (this.owner.dataSource)
                                if (this.owner.dataSource.Count == this.owner.dataCopy.Count)
                                {
                                    /* ------------------------------------------------\
                                    |  If the cache for the current table is to size,  |
                                    |  then it doesn't exist in the current table.     |
                                    \------------------------------------------------ */
                                    return false;
                                }
                                else
                                {
                                    foreach (KeyValuePair<GrammarVocabulary, SyntacticalStreamState> currentPair in this.owner)
                                    {
                                        if (value == currentPair.Value)
                                            return true;
                                    }
                                    return false;
                                }
                        }
                }

                public void CopyTo(SyntacticalStreamState[] array, int arrayIndex)
                {
                    lock (this.owner.dataSource)
                        for (int i = 0; (i < this.owner.dataSource.Count); i++)
                        {
                            array[(arrayIndex + i)] = this[i];
                        }
                }

                public SyntacticalStreamState[] ToArray()
                {
                    lock (this.owner.dataSource)
                    {
                        SyntacticalStreamState[] result = new SyntacticalStreamState[this.Count];
                        this.CopyTo(result, 0);
                        return result;
                    }
                }

                public IEnumerator<SyntacticalStreamState> GetEnumerator()
                {
                    lock (this.owner.dataSource)
                        foreach (GrammarVocabulary transition in this.owner.dataSource.Keys)
                        {
                            yield return this.owner[transition];
                        }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
                #endregion // ValuesCollection methods
                #region ValuesCollection .ctors
                public ValuesCollection(TransitionTable owner)
                {
                    this.owner = owner;
                }
                #endregion // ValuesCollection .ctors

                #region IControlledStateCollection<SyntacticalStreamState> Members


                public int IndexOf(SyntacticalStreamState element)
                {
                    int index = 0;
                    foreach (var currentElement in this)
                        if (currentElement == element)
                            return index;
                        else
                            index++;
                    return -1;
                }

                #endregion
            }
            #endregion // TransitionTable nested types
        }
        #endregion // SyntacticalStreamState nested types
    }
}
