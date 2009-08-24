using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer._Internal
{
    internal class Tuple<T1, T2, T3> :
        Tuple<T1, T2>
    {
        private T3 item3;

        /// <summary>
        /// Creates a new <see cref="Tuple{T1, T2, T3}"/> with the 
        /// <paramref name="item"/>, <paramref name="item2"/> and 
        /// <paramref name="item3"/>.
        /// </summary>
        /// <param name="item">The singleton instance.</param>
        /// <param name="item2">The singleton's pair.</param>
        /// <param name="item3">The pair's triplet.</param>
        public Tuple(T1 item, T2 item2, T3 item3)
            : base(item, item2)
        {
            this.item3 = item3;
        }
        /// <summary>
        /// Creates a new <see cref="Tuple{T1, T2, T3}"/> with the 
        /// <paramref name="pair"/>, and <paramref name="item3"/>
        /// provided.
        /// </summary>
        /// <param name="pair">The <see cref="Pair{T1, T2}"/> which make up
        /// <see cref="Tuple{T}.Item"/> and <see cref="Tuple{T1, T2}.Item2"/>.</param>
        /// <param name="item3">The <paramref name="pair"/>'s triplet.</param>
        public Tuple(Tuple<T1, T2> pair, T3 item3)
            : this(pair.Item, pair.Item2, item3)
        {

        }

        /// <summary>
        /// Returns the pair's triplet.
        /// </summary>
        public virtual T3 Item3
        {
            get
            {
                return this.item3;
            }
        }
    }
}
