using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a symbol 
    /// which is derived from a token or an element of a token.
    /// </summary>
    public interface IGrammarTokenSymbol :
        IGrammarSymbol
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> on which the
        /// <see cref="IGrammarTokenSymbol"/> is based.
        /// </summary>
        ITokenEntry Source { get; }
    }
}
