using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a statement series.
    /// </summary>
    /// <remarks>Useful for code generators.</remarks>
    public interface IStatementSeries : 
        IStatement
    {
        /// <summary>
        /// Generates the <see cref="CodeStatement"/> objects for the statement series.
        /// </summary>
        /// <param name="options">The generator options that direct the generation process 
        /// specifics.</param>
        /// <returns>A series of <see cref="CodeStatement"/> based upon the specific
        /// behaviours of the <see cref="IStatementSeries"/> implementation.</returns>
        new CodeStatement[] GenerateCodeDom(ICodeDOMTranslationOptions options);

    }
}
