using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    partial class ControlledStateDictionary<TKey, TValue>
    {
        /// <summary>
        /// Represents the collection of values in a <see cref="ControlledStateDictionary{TKey,TKey}"/>.
        /// </summary>
        public class ValuesCollection :
            ControlledStateCollection<TValue>
        {
            internal ValuesCollection(ICollection<TValue> baseCollection)
                : base(baseCollection)
            {
            }
        }
    }
}
