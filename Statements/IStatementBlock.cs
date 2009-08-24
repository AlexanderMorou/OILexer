using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Types.Members;
using Oilexer.Expression;
using Oilexer.Types;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    public interface IStatementBlock :
        IStatementBlockInsertBase,
        IList<IStatement>,
        IDeclarationTarget,
        ITypeReferenceable
    {
        /// <summary>
        /// Generates the appropriate code-dom for the statement block.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A series of <see cref="CodeStatement"/>s pertinent to the <see cref="IStatementBlock"/>
        /// entries.</returns>
        CodeStatementCollection GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Returns whether the <see cref="IStatementBlock"/> has a name defined in its scope.
        /// </summary>
        /// <param name="name">The name to search for in the local area and higher scopes.</param>
        /// <returns>true if the name appears in the scope; false otherwise.</returns>
        bool ScopeContains(string name);
        /// <summary>
        /// Returns the blocked statement which contains the <see cref="IStatementBlock"/>.
        /// </summary>
        IBlockParent Parent { get; }

    }
}
