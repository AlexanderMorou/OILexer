using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a series of conditions
    /// that flow from one to the next.
    /// </summary>
    public interface IConditionSeriesStatement :
        IStatement<CodeConditionStatement>
    {
        /// <summary>
        /// Returns the condition and statementblock pairs that make up the 
        /// <see cref="IConditionSeriesStatement"/>.
        /// </summary>
        IConditionSeriesParts Conditions { get; }
        /// <summary>
        /// Returns the statements of what to execute when all <see cref="Conditions"/> evaluate
        /// to false.
        /// </summary>
        IStatementBlock FalseStatements { get; }
    }
}
