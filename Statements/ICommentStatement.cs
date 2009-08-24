using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a comment statement.
    /// </summary>
    public interface ICommentStatement :
        IStatement<CodeCommentStatement>
    {
        /// <summary>
        /// Returns/sets the comment relative to the <see cref="ICommentStatement"/>.
        /// </summary>
        string Comment { get; set; }
    }
}
