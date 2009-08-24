using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    partial class ControlledStateDictionary<TKey, TValue>
    {
        /// <summary>
        /// Represents the collection of keys in a <see cref="ControlledStateDictionary{TKey,TKey}"/>.
        /// </summary>
        public class KeysCollection :
            ControlledStateCollection<TKey>
        {
            internal KeysCollection(ICollection<TKey> baseCollection)
                : base(baseCollection)
            {
            }
        }
    }
}
