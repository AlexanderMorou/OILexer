using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Expression
{
    public interface ICommentExpression :
        IExpression
    {
        /// <summary>
        /// Returns/sets the comment associated with the <see cref="ICommentExpression"/>
        /// </summary>
        string Comment { get; set; }
    }
}
