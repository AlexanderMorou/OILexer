using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a grammar
    /// constant which is defined as an element of a token category.
    /// </summary>
    public interface IGrammarConstantItemSymbol :
        IGrammarConstantSymbol
    {
        /// <summary>
        /// Returns the <see cref="ILiteralTokenItem"/> on which the
        /// <see cref="IGrammarConstantItemSymbol"/> is based.
        /// </summary>
        ILiteralTokenItem SourceItem { get; }
    }
}
