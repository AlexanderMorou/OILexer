using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer._Internal
{
    internal class Tuple<T1, T2, T3, T4> :
        Tuple<T1, T2, T3>
    {
        private T4 item4;
        /// <summary>
        /// Creates a new <see cref="Tuple{T1, T2, T3, T4}"/> with the
        /// <paramref name="item"/>, <paramref name="item2"/>, <paramref name="item3"/>
        /// <paramref name="item4"/> provided.
        /// </summary>
        /// <param name="item">The singleton instance.</param>
        /// <param name="item2">The singleton's pair.</param>
        /// <param name="item3">The pair's triplet.</param>
        /// <param name="item4">The triplet's quadruplet.</param>
        public Tuple(T1 item, T2 item2, T3 item3, T4 item4)
            : base(item, item2, item3)
        {
            this.item4 = item4;
        }
        /// <summary>
        /// Creates a new <see cref="Tuple{T1, T2, T3, T4}"/> with the 
        /// <paramref name="pairA"/> and <paramref name="pairB"/>
        /// provided.
        /// </summary>
        /// <param name="pairA">The transitionFirst pair.</param>
        /// <param name="pairB">The second pair.</param>
        public Tuple(Tuple<T1, T2> pairA, Tuple<T3, T4> pairB)
            : this(pairA.Item, pairA.Item2, pairB.Item, pairB.Item2)
        {
        }
        /// <summary>
        /// Creates a new <see cref="Tuple{T1, T2, T3, T4}"/> with the 
        /// <paramref name="pairA"/> and <paramref name="pairB"/>
        /// provided.
        /// </summary>
        /// <param name="triplet">The (<typeparamref name="T1"/>, <typeparamref name="T2"/> and
        /// <typeparamref name="T3"/>) <see cref="Tuple{T1, T2, T3}"/> triplet.</param>
        /// <param name="singleton">The (<typeparamref name="T4"/>) singleton.</param>
        public Tuple(Tuple<T1, T2, T3> triplet, T4 singleton)
            : this(triplet.Item, triplet.Item2, triplet.Item3, singleton)
        {
        }
        /// <summary>
        /// Returns the triplet's quadruplet.
        /// </summary>
        public virtual T4 Item4
        {
            get
            {
                return this.item4;
            }
            set
            {
                this.item4 = value;
            }
        }
    }
}
