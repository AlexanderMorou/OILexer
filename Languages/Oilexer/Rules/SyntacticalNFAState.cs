using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalNFAState :
        NFAState<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>
    {
        internal ParserBuilder builder;
        public SyntacticalNFAState(ParserBuilder builder)
        {
            this.builder = builder;
        }

        protected override SyntacticalDFAState GetDFAState()
        {
            lock (this.builder)
                return new SyntacticalDFAState(this.builder);
        }
    }
}
