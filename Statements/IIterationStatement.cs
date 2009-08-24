using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with an iteration statement.
    /// </summary>
    public interface IIterationStatement :
        IBlockedStatement<CodeIterationStatement>,
        IBreakTargetStatement
    {
        /// <summary>
        /// Returns/sets the initialization statement that is called before the
        /// iteration sequence begins.
        /// </summary>
        IStatement InitializationStatement { get; set; }
        /// <summary>
        /// Returns/sets the increment statement that is called after each full
        /// iteration.
        /// </summary>
        IStatement IncrementStatement { get; set; }
        /// <summary>
        /// Returns/sets the test expression that determines when to stop the 
        /// iteration sequence.
        /// </summary>
        IExpression TestExpression { get; set; }

    }
}
