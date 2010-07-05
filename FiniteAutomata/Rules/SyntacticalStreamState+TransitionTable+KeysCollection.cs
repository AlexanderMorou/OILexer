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
            private class KeysCollection :
                IControlledStateCollection<GrammarVocabulary>
            {
                #region KeysCollection data members
                private TransitionTable owner;
                #endregion // KeysCollection data members
                #region KeysCollection properties
                public int Count
                {
                    get
                    {
                        lock (this.owner.dataSource)
                            return this.owner.dataSource.Count;
                    }
                }

                public GrammarVocabulary this[int index]
                {
                    get
                    {
                        int i = 0;
                        lock (this.owner.dataSource)
                            foreach (GrammarVocabulary currentTransition in this.owner.dataSource.Keys)
                            {
                                if (i == index)
                                    return currentTransition;
                                else
                                    i++;
                            }
                        return new GrammarVocabulary();
                    }
                }
                #endregion // KeysCollection properties
                #region KeysCollection methods
                public bool Contains(GrammarVocabulary key)
                {
                    return this.owner.ContainsKey(key);
                }

                public void CopyTo(GrammarVocabulary[] array, int arrayIndex)
                {
                    int i = 0;
                    lock (this.owner.dataSource)
                        foreach (GrammarVocabulary currentTransition in this.owner.dataSource.Keys)
                        {
                            array[(i + arrayIndex)] = currentTransition;
                            i++;
                        }
                }

                public GrammarVocabulary[] ToArray()
                {
                    lock (this.owner.dataSource)
                    {
                        GrammarVocabulary[] result = new GrammarVocabulary[this.owner.dataSource.Count];
                        this.CopyTo(result, 0);
                        return result;
                    }
                }

                public IEnumerator<GrammarVocabulary> GetEnumerator()
                {
                    lock (this.owner.dataSource)
                        return this.owner.dataSource.Keys.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
                #endregion // KeysCollection methods
                #region KeysCollection .ctors
                public KeysCollection(TransitionTable owner)
                {
                    this.owner = owner;
                }
                #endregion // KeysCollection .ctors


                #region IControlledStateCollection<GrammarVocabulary> Members

                public int IndexOf(GrammarVocabulary element)
                {
                    lock (this.owner.dataSource)
                    {
                        int index = 0;
                        foreach (var key in this.owner.dataSource.Keys)
                            if (key == element)
                                return index;
                            else
                                index++;
                        return -1;
                    }
                }

                #endregion
            }
            #endregion // TransitionTable nested types
        }
        #endregion // SyntacticalStreamState nested types
    }
}
