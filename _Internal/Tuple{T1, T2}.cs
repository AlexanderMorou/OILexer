using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer._Internal
{
    internal class Tuple<T1, T2> :
        Tuple<T1>
    {
        private T2 item2;
        /// <summary>
        /// Creates a new <see cref="Pair{T1, T2}"/> instance
        /// with the <paramref name="item"/> and
        /// <paramref name="item2"/> provided.
        /// </summary>
        /// <param name="item">The singleton instance.</param>
        /// <param name="item2">The instance to make it a pair.</param>
        public Tuple(T1 item, T2 item2)
            : base(item)
        {
            this.item2 = item2;
        }
        /// <summary>
        /// Returns the singleton's pair.
        /// </summary>
        public virtual T2 Item2
        {
            get
            {
                return this.item2;
            }
            set
            {
                this.item2 = value;
            }
        }


    }
}
