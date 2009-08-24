using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer._Internal
{
    /// <summary>
    /// A singleton tuple
    /// </summary>
    /// <typeparam name="T">The type of the singleton value.</typeparam>
    internal class Tuple<T>
    {
        private T item;
        /// <summary>
        /// Creates a new <see cref="Tuple{T}"/> with the 
        /// <paramref name="item"/> provided.
        /// </summary>
        /// <param name="item">The singleton value.</param>
        public Tuple(T item)
        {
            this.item = item;
        }

        /// <summary>
        /// Returns the singleton value.
        /// </summary>
        public virtual T Item
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }
        public static implicit operator Tuple<T>(T expr)
        {
            return new Tuple<T>(expr);
        }

    }
}
