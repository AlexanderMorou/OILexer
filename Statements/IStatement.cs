using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for a general statement.
    /// </summary>
    public interface IStatement :
        ITypeReferenceable
    {
        /// <summary>
        /// Generates the CodeDom objects for the <see cref="IStatement"/>.
        /// </summary>
        /// <param name="options">The generator options that direct the generation process 
        /// specifics.</param>
        /// <returns>A <see cref="CodeStatement"/> that represents the <see cref="IStatement"/>.</returns>
        CodeStatement GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Returns whether the <see cref="StatementBlock"/> generating CodeDOM objects
        /// should skip this statement.
        /// </summary>
        bool Skip { get;set; }
        IStatementBlock SourceBlock { get; set; }
    }
}
