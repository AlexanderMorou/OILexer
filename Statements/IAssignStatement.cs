using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a
    /// statement that assigns a value to a referenced target.
    /// </summary>
    public interface IAssignStatement :
        IStatement<CodeAssignStatement>
    {
        /// <summary>
        /// The member to assign to.
        /// </summary>
        IAssignStatementTarget Reference { get; }
        /// <summary>
        /// The value to assign.
        /// </summary>
        IExpression Value { get; }
    }
}
