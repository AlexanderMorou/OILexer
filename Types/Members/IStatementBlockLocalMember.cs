using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a local variable defined in code.
    /// </summary>
    public interface IStatementBlockLocalMember :
        IMember<IStatementBlock, CodeVariableDeclarationStatement>
    {
        /// <summary>
        /// Returns/sets whether the CodeDom generation process should auto-declare the member.
        /// </summary>
        /// <remarks>If <see cref="AutoDeclare"/> is false, use <see cref="GetDeclarationStatement()"/>
        /// to declare the member.</remarks>
        bool AutoDeclare { get; set; }
        /// <summary>
        /// Returns the declaration statement for the <see cref="IStatementBlockLocalMember"/>.
        /// Use this if <see cref="AutoDeclare"/> is false.
        /// </summary>
        ILocalDeclarationStatement GetDeclarationStatement();
        /// <summary>
        /// Returns/sets the type of the local.
        /// </summary>
        ITypeReference LocalType { get; set; }
        /// <summary>
        /// Returns/sets the initialization expression.
        /// </summary>
        IExpression InitializationExpression { get; set; }
        new ILocalReferenceExpression GetReference();
    }
}
