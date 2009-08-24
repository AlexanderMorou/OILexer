using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    partial class ControlledStateDictionary<TKey, TValue>
    {
        private struct Enumerator :
            IEnumerator<KeyValuePair<TKey, TValue>>, 
            IDisposable, 
            IDictionaryEnumerator
        {
            private Dictionary<TKey, TValue>.Enumerator baseEnumerator;

            internal Enumerator(Dictionary<TKey, TValue>.Enumerator baseEnumerator)
            {
                this.baseEnumerator = baseEnumerator;
            }

            #region IDictionaryEnumerator Members

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get { return ((IDictionaryEnumerator)this.baseEnumerator).Entry; }
            }

            object IDictionaryEnumerator.Key
            {
                get { return ((IDictionaryEnumerator)this.baseEnumerator).Key; }
            }

            object IDictionaryEnumerator.Value
            {
                get { return ((IDictionaryEnumerator)this.baseEnumerator).Value; }
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            bool IEnumerator.MoveNext()
            {
                return this.baseEnumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                ((IEnumerator)this.baseEnumerator).Reset();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.baseEnumerator.Dispose();
            }

            #endregion

            #region IEnumerator<KeyValuePair<TKey,TValue>> Members
            /// <summary>
            /// Returns the current element in the <see cref="Enumerator"/>.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get {
                    return this.baseEnumerator.Current;
                }
            }

            #endregion
        }
    }
}
