using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// The type of crement operation.
    /// </summary>
    public enum CrementType
    {
        /// <summary>
        /// The <see cref="ICrementStatement.Operation"/> appears before the <see cref="ICrementStatement.Target"/>.
        /// </summary>
        Prefix,
        /// <summary>
        /// The <see cref="ICrementStatement.Operation"/> appears after the <see cref="ICrementStatement.Target"/>.
        /// </summary>
        Postfix
    }
    public enum CrementOperation
    {
        /// <summary>
        /// The <see cref="ICrementStatement.Target"/> is incremented.
        /// </summary>
        Increment,
        /// <summary>
        /// The <see cref="ICrementStatement.Target"/> is decremented.
        /// </summary>
        Decrement
    }
    /// <summary>
    /// Defines properties and methods for a crement operation that increments or decrements 
    /// an <see cref="IAssignStatementTarget"/>.
    /// </summary>
    public interface ICrementStatement :
        IStatement<CodeSnippetStatement>
    {
        /// <summary>
        /// Returns/sets whether the crement statement is prefix based or postfix based.
        /// </summary>
        CrementType CrementType { get; set; }
        /// <summary>
        /// Returns/sets the crement operation that is applied to <see cref="Target"/>.
        /// </summary>
        CrementOperation Operation { get; set; }
        /// <summary>
        /// Returns the <see cref="IAssignStatementTarget"/> which has the <see cref="Operation"/> applied to it.
        /// </summary>
        IAssignStatementTarget Target { get; set; }
    }
}
