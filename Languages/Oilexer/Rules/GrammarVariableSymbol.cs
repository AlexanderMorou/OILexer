using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides an implementation of a grammar variable symbol
    /// which represents a symbol derived from a token which does
    /// not consist of a literal value.
    /// </summary>
    public class GrammarVariableSymbol :
        GrammarTokenSymbol,
        IGrammarVariableSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarVariableSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/>
        /// from which the <see cref="GrammarVariableSymbol"/> is
        /// derived.</param>
        public GrammarVariableSymbol(IOilexerGrammarTokenEntry source)
            : base(source)
        {
        }
    }
}
