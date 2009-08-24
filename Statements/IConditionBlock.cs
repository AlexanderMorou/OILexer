using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a condition block.
    /// </summary>
    public interface IConditionBlock :
        IBlockedStatement<CodeConditionStatement>
    {
        /// <summary>
        /// Returns/sets the condition for the <see cref="IConditionBlock"/>.
        /// </summary>
        IExpression Condition { get; set; }
    }
}
