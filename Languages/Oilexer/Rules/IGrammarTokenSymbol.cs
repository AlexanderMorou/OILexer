using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a symbol 
    /// which is derived from a token or an element of a token.
    /// </summary>
    public interface IGrammarTokenSymbol :
        IGrammarSymbol
    {
        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> on which the
        /// <see cref="IGrammarTokenSymbol"/> is based.
        /// </summary>
        IOilexerGrammarTokenEntry Source { get; }
    }
}
