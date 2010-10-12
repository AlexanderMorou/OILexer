using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oilexer.Utilities.Collections
{
    partial class ControlledStateDictionary<TKey, TValue>
    {
        public class SharedLocals
        {
            internal Dictionary<TKey, int> orderings = new Dictionary<TKey, int>();
            internal KeyValuePair<TKey, TValue>[] entries;
            internal object syncObject = new object();
            private KeysCollection keys;
            private ValuesCollection values;
            Func<KeysCollection> keysInitializer;
            Func<ValuesCollection> valuesInitializer;
            public SharedLocals(Func<KeysCollection> keysInitializer, Func<ValuesCollection> valuesInitializer)
            {
                this.keysInitializer = keysInitializer;
                this.valuesInitializer = valuesInitializer;
            }

            internal void EnsureSpaceExists(int newCount)
            {
                if (this.entries == null)
                {
                    this.entries = new KeyValuePair<TKey, TValue>[newCount];
                    return;
                }
                if (this.entries.Length < newCount)
                {
                    lock (this.entries)
                    {
                        int doubleCount = Math.Max(this.orderings.Count * 2, 4);
                        KeyValuePair<TKey, TValue>[] newEntries = new KeyValuePair<TKey, TValue>[Math.Max(doubleCount, newCount)];
                        Array.ConstrainedCopy(entries, 0, newEntries, 0, this.orderings.Count);
                        this.entries = newEntries;
                    }
                }
            }

            public int Count
            {
                get
                {
                    return this.orderings.Count;
                }
            }

            public KeysCollection Keys
            {
                get
                {
                    if (this.keys == null)
                        this.keys = this.keysInitializer();
                    return this.keys;
                }
            }

            public ValuesCollection Values
            {
                get
                {
                    if (this.values == null)
                        this.values = this.valuesInitializer();
                    return this.values;
                }
            }

            internal void _Add(KeyValuePair<TKey, TValue> item)
            {
                lock (this.syncObject)
                {
                    this.EnsureSpaceExists(this.Count + 1);
                    this.entries[this.Count] = item;
                    this.orderings.Add(item.Key, this.Count);
                }
            }

            internal void _AddRange(IEnumerable<KeyValuePair<TKey, TValue>> elements)
            {
                if (elements == null)
                    throw new ArgumentNullException("elements");
                KeyValuePair<TKey, TValue>[] newSet = elements.ToArray();
                if (newSet.Length == 0)
                    return;
                lock (this.syncObject)
                {
                    EnsureSpaceExists(this.Count + newSet.Length);
                    int startingCount = this.Count;
                    Parallel.For(0, newSet.Length, i =>
                    {
                        var newitem = newSet[i];
                        this.entries[startingCount + i] = newitem;
                        lock (orderings)
                            this.orderings.Add(newitem.Key, startingCount + i);
                    });
                }
            }

            internal bool _Remove(int index)
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException("index");
                var removed = this.entries[index];
                lock (this.syncObject)
                {
                    for (int i = index; i < this.Count - 1; i++)
                    {
                        var next = this.entries[i + 1];
                        this.orderings[next.Key] = i;
                        this.entries[i] = next;
                    }
                    this.orderings.Remove(removed.Key);
                    this.entries[this.Count] = default(KeyValuePair<TKey, TValue>);
                }
                return true;
            }

            internal void Clear()
            {
                lock (this.syncObject)
                {
                    this.orderings.Clear();
                    this.entries = null;
                }
            }

            internal KeysCollection keysInstance
            {
                get
                {
                    return this.keys;
                }
                set
                {
                    this.keys = value;
                }
            }

            internal ValuesCollection valuesInstance
            {
                get
                {
                    return this.values;
                }
                set
                {
                    this.values = value;
                }
            }


        }
    }
}
