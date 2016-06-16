using System;
using System.Collections.Generic;
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
    /// Defines properties and methods for working with a grammar
    /// symbol which references a rule.
    /// </summary>
    public interface IGrammarRuleSymbol :
        IGrammarSymbol
    {
        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/>
        /// on which the <see cref="IGrammarRuleSymbol"/> is based.
        /// </summary>
        IOilexerGrammarProductionRuleEntry Source { get; }
    }
}
