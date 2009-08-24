using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Common
{
    /// <summary>
    /// Defines a simple data-based event handler that contains a single event argument. <seealso cref="EventArgs{T}.Data"/>
    /// </summary>
    /// <typeparam name="T">The type of data to use in the argument.</typeparam>
    /// <param name="s">The sender of the event.</param>
    /// <param name="e">The <see cref="EventArgs{T}"/> which contains the data.</param>
    public delegate void DataEventHandler<T>(object s, EventArgs<T> e);
    /// <summary>
    /// Defines a simple data-based event handler that contains a single event argument. <seealso cref="EventArgs{T}.Data"/>
    /// </summary>
    /// <typeparam name="T">The type of data to use in the first argument.</typeparam>
    /// <typeparam name="U">The type of data to use in the second argument.</typeparam>
    /// <param name="s">The sender of the event.</param>
    /// <param name="e">The <see cref="EventArgs{T, U}"/> which contains the data.</param>
    public delegate void DataEventHandler<T, U>(object s, EventArgs<T, U> e);
}
