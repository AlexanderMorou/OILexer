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
        public partial class TransitionTable :
            IControlledStateDictionary<GrammarVocabulary, SyntacticalStreamState>
        {
            #region TransitionTable data members
            private bool fullSetObtained;

            private GrammarVocabulary fullSet;

            private Dictionary<GrammarVocabulary, GrammarVocabulary> partialKeys = new Dictionary<GrammarVocabulary, GrammarVocabulary>();

            private GrammarVocabulary[] dataKeys;

            private Dictionary<GrammarVocabulary, StatePath[]> dataSource;

            private Dictionary<GrammarVocabulary, SyntacticalStreamState> dataCopy = new Dictionary<GrammarVocabulary, SyntacticalStreamState>();

            private SyntacticalStreamState owner;

            private Dictionary<int, KeyValuePair<GrammarVocabulary, SyntacticalStreamState>> indexCache = new Dictionary<int, KeyValuePair<GrammarVocabulary, SyntacticalStreamState>>();

            private KeysCollection keys;

            private ValuesCollection values;
            #endregion // TransitionTable data members
            #region TransitionTable properties
            public IControlledStateCollection<GrammarVocabulary> Keys
            {
                get
                {
                    if (this.keys == null)
                        this.keys = new KeysCollection(this);
                    return this.keys;
                }
            }

            public IControlledStateCollection<SyntacticalStreamState> Values
            {
                get
                {
                    if (this.values == null)
                        this.values = new ValuesCollection(this);
                    return this.values;
                }
            }

            public int Count
            {
                get
                {
                    return this.dataSource.Count;
                }
            }

            public SyntacticalStreamState this[GrammarVocabulary key]
            {
                get
                {
                    SyntacticalStreamState result;
                    this.TryGetValue(key, out result);
                    return result;
                }
            }

            public KeyValuePair<GrammarVocabulary, SyntacticalStreamState> this[int index]
            {
                get
                {
                    this.CheckItemAt(index);
                    lock (indexCache)
                    {
                        if (this.indexCache.ContainsKey(index))
                            return this.indexCache[index];
                    }
                    return new KeyValuePair<GrammarVocabulary, SyntacticalStreamState>();
                }
            }
            #endregion // TransitionTable properties
            #region TransitionTable methods
            public bool ContainsKey(GrammarVocabulary key)
            {
                lock (this.dataSource)
                {
                    if (!(this.dataSource.ContainsKey(key)))
                    {
                        lock (this.partialKeys)
                        {
                            if (this.partialKeys.ContainsKey(key))
                                return true;
                            foreach (GrammarVocabulary fullTransition in this.dataSource.Keys)
                            {
                                GrammarVocabulary intersection = (fullTransition & key);
                                if (intersection.Equals(key))
                                {
                                    this.partialKeys.Add(key, fullTransition);
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                    else
                        return true;
                }
            }

            private void CheckItemAt(GrammarVocabulary key)
            {
                lock (this.dataCopy)
                    lock (this.partialKeys)
                        if ((!(this.ContainsKey(key)) || (this.partialKeys.ContainsKey(key) && this.dataCopy.ContainsKey(this.partialKeys[key]))) || this.dataCopy.ContainsKey(key))
                            return;

                SyntacticalStreamState result = null;
                lock (this.dataSource)
                    if (this.dataSource.ContainsKey(key))
                    {
                        result = ObtainState(this.dataSource[key], key);
                        lock (this.dataCopy)
                            this.dataCopy.Add(key, result);
                    }
                    else
                    {
                        lock (this.partialKeys)
                        {
                            GrammarVocabulary fullKey = this.partialKeys[key];

                            result = ObtainState(this.dataSource[fullKey], fullKey);
                            this.dataCopy.Add(fullKey, result);
                        }
                    }
            }

            private void CheckItemAt(int index)
            {
                lock (this.dataSource)
                {
                    if ((index >= this.dataSource.Count) || (index < 0))
                        return;
                    lock (this.indexCache)
                    {
                        if (!(this.indexCache.ContainsKey(index)))
                        {
                            GrammarVocabulary indexedTransition = this.Keys[index];
                            this.CheckItemAt(indexedTransition);
                            this.indexCache.Add(index, new KeyValuePair<GrammarVocabulary, SyntacticalStreamState>(indexedTransition, this.dataCopy[indexedTransition]));
                        }
                    }
                }
            }

            public bool TryGetValue(GrammarVocabulary key, out SyntacticalStreamState value)
            {
                lock (this.dataCopy)
                {
                    lock (this.partialKeys)
                    {
                        if (!((this.partialKeys.ContainsKey(key) || this.dataCopy.ContainsKey(key))))
                            this.CheckItemAt(key);
                    }
                    if (this.dataCopy.ContainsKey(key))
                    {
                        value = this.dataCopy[key];
                        return true;
                    }
                    else
                    {
                        lock (this.partialKeys)
                        {
                            if (this.partialKeys.ContainsKey(key))
                            {
                                var fullKey = this.partialKeys[key];
                                if (!this.dataCopy.ContainsKey(fullKey))
                                    this.CheckItemAt(fullKey);
                                value = this.dataCopy[this.partialKeys[key]];
                                return true;
                            }
                        }
                    }
                    value = null;
                    return false;
                }
            }

            public bool Contains(KeyValuePair<GrammarVocabulary, SyntacticalStreamState> item)
            {
                if (!(this.ContainsKey(item.Key)))
                    return false;
                return (this[item.Key] == item.Value);
            }

            public void CopyTo(KeyValuePair<GrammarVocabulary, SyntacticalStreamState>[] array, int arrayIndex)
            {
                for (int i = 0; (i < this.Count); i++)
                {
                    array[(arrayIndex + i)] = this[i];
                }
            }

            public KeyValuePair<GrammarVocabulary, SyntacticalStreamState>[] ToArray()
            {
                lock (this.dataCopy)
                {
                    KeyValuePair<GrammarVocabulary, SyntacticalStreamState>[] result = new KeyValuePair<GrammarVocabulary, SyntacticalStreamState>[this.Count];
                    this.CopyTo(result, 0);
                    return result;
                }
            }

            public IEnumerator<KeyValuePair<GrammarVocabulary, SyntacticalStreamState>> GetEnumerator()
            {
                lock (this.dataSource)
                {
                    foreach (GrammarVocabulary key in this.dataSource.Keys)
                    {
                        yield return new KeyValuePair<GrammarVocabulary, SyntacticalStreamState>(key, this[key]);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion // TransitionTable methods
            #region TransitionTable .ctors
            internal TransitionTable(SyntacticalStreamState owner)
            {
                this.owner = owner;
                lock (this)
                {
                    this.dataSource = GetLookAhead(owner.sources);
                    this.dataKeys = new GrammarVocabulary[this.dataSource.Count];
                    this.dataSource.Keys.CopyTo(this.dataKeys, 0);
                }
            }
            #endregion // TransitionTable .ctors

            #region IControlledStateCollection<KeyValuePair<GrammarVocabulary,SyntacticalStreamState>> Members


            public int IndexOf(KeyValuePair<GrammarVocabulary, SyntacticalStreamState> element)
            {
                lock (this.dataCopy)
                {
                    int index = this.Keys.IndexOf(element.Key);
                    if (index == -1)
                        return index;
                    else if (this.Values[index] == element.Value)
                        return index;
                    return -1;
                }
            }

            #endregion

            public GrammarVocabulary FullSet
            {
                get
                {
                    if (!this.fullSetObtained)
                    {
                        for (int i = 0; i < this.dataKeys.Length; i++)
                            fullSet |= this.dataKeys[i];
                        fullSetObtained = true;
                    }
                    return fullSet;
                }
            }

        }
        #endregion // SyntacticalStreamState nested types
    }
}
