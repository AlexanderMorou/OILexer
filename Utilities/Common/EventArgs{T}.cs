using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Common
{
    public class EventArgs<T> :
        EventArgs
    {
        /// <summary>
        /// Data member for <see cref="Data"/>.
        /// </summary>
        private T data;
        /// <summary>
        /// Creates a new <see cref="EventArgs{T}"/> with the <paramref name="data"/> provided.
        /// </summary>
        /// <param name="data">The <typeparamref name="T"/> which the event args should pass.</param>
        public EventArgs(T data)
        {
            this.data = data;
        }

        /// <summary>
        /// Returns the <typeparamref name="T"/> which the <see cref="EventArgs{T}"/> represents.
        /// </summary>
        public T Data
        {
            get
            {
                return this.data;
            }
        }
    }
    public class EventArgs<T, U> :
        EventArgs
    {
        /// <summary>
        /// Data member for <see cref="Data1"/>.
        /// </summary>
        private T data1;
        /// <summary>
        /// Data member for <see cref="Data2"/>.
        /// </summary>
        private U data2;
        /// <summary>
        /// Creates a new <see cref="EventArgs{T,U}"/> with the <paramref name="data1"/> and
        /// <paramref name="data2"/> provided.
        /// </summary>
        /// <param name="data1">The <typeparamref name="T"/> which the event args should pass.</param>
        /// <param name="data2">The <typeparamref name="U"/> which the event args should pass.</param>
        public EventArgs(T data1, U data2)
        {
            this.data1 = data1;
            this.data2 = data2;
        }

        /// <summary>
        /// Returns the <typeparamref name="T"/> which the <see cref="EventArgs{T,U}"/> represents.
        /// </summary>
        public T Data1
        {
            get
            {
                return this.data1;
            }
        }
        /// <summary>
        /// Returns the <typeparamref name="U"/> which the <see cref="EventArgs{T,U}"/> represents.
        /// </summary>
        public U Data2
        {
            get
            {
                return this.data2;
            }
        }
    }
}
