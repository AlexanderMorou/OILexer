using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
/*---------------------------------------------------------------------\
| Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalNFAState :
        NFAState<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>
    {
        private ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup;
        private GrammarSymbolSet symbols;
        public SyntacticalNFAState(ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            this.lookup = lookup;
            this.symbols = symbols;
        }

        protected override SyntacticalDFAState GetDFAState()
        {
            return new SyntacticalDFAState(lookup, symbols);
        }
    }
}
