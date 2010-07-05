using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Provides an implementation of a grammar variable symbol
    /// which represents a symbol derived from a token which does
    /// not consist of a literal value.
    /// </summary>
    public class GrammarVariableSymbol :
        GrammarTokenSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarVariableSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/>
        /// from which the <see cref="GrammarVariableSymbol"/> is
        /// derived.</param>
        public GrammarVariableSymbol(ITokenEntry source)
            : base(source)
        {
        }
    }
}
