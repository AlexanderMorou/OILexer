using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types.Members;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for a break statement
    /// which exits an <see cref="IBreakTargetStatement"/>.
    /// </summary>
    public interface IBreakStatement :
        IStatementSeries
    {
        /// <summary>
        /// Returns/sets the condition that the break executes.
        /// </summary>
        /// <remarks>
        /// If Condition is 'null', a CodePrimitiveExpression of 'true' is used.
        /// </remarks>
        IExpression Condition { get; set; }
        IStatementBlockLocalMember TerminalVariable { get; }
        IBreakTargetExitPoint ExitSymbol { get; }
    }
}
