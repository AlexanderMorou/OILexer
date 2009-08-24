using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;

namespace Oilexer.Comments
{
    /// <summary>
    /// Defines properties and methods for working with a reference to a <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of reference to refer to during generation of the comment
    /// particle.</typeparam>
    public interface IMemberReferenceComment<T> :
        IMemberReferenceComment
        where T :
            IMemberReferenceExpression
    {
        /// <summary>
        /// Returns/sets the reference point.
        /// </summary>
        new T Reference { get; set; }
    }
}
