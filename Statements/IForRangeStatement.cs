using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface IForRangeStatement :
        IBlockedStatement<CodeIterationStatement>,
        IBreakTargetStatement
    {
        /// <summary>
        /// The variable that contains the iteration value.
        /// </summary>
        IStatementBlockLocalMember IterationIndex { get; }
        /// <summary>
        /// Returns/sets the start point of the iteration.
        /// </summary>
        IExpression Start { get; set; }
        /// <summary>
        /// Returns/sets the maximum point.
        /// </summary>
        IExpression Max { get; set; }
        /// <summary>
        /// Returns/sets the size each increment is.
        /// </summary>
        IExpression Step { get;  set; }
    }
}
