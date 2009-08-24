using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;

namespace Oilexer.Statements
{
    public interface IBreakTargetStatement :
        IBlockedStatement
    {
        /// <summary>
        /// Returns/sets whether the <see cref="IBreakTargetStatement"/> includes 
        /// </summary>
        bool UtilizeBreakMeasures { get; set; }
        /// <summary>
        /// Returns the exit label defined for break points.
        /// </summary>
        IBreakTargetExitPoint ExitLabel { get; }
        IStatementBlockLocalMember BreakLocal { get; }

    }
}
