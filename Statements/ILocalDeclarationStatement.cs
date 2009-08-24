using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a local variable declaration.
    /// </summary>
    public interface ILocalDeclarationStatement :
        IStatement<CodeVariableDeclarationStatement>
    {
        /// <summary>
        /// Returns/sets the referenced member the <see cref="ILocalDeclarationStatement"/>
        /// declares.
        /// </summary>
        IStatementBlockLocalMember ReferencedMember { get; }
    }
}
