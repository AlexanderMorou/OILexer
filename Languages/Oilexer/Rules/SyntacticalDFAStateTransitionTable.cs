using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalDFAStateTransitionTable :
        FiniteAutomataSingleTargetTransitionTable<GrammarVocabulary, SyntacticalDFAState>
    {
        private SyntacticalDFAState state;

        public SyntacticalDFAStateTransitionTable(SyntacticalDFAState state)
        {
            this.state = state;
        }

        public Tuple<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState, SyntacticalDFAState> GetNode(IOilexerGrammarProductionRuleEntry rule)
        {
            lock (this.state.lookup)
                foreach (var key in this.Keys)
                    if (key.Contains(rule))
                        return new Tuple<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState, SyntacticalDFAState>(rule, this.state.lookup[rule], this[key]);
            throw new ArgumentException("rule");
        }

        public SyntacticalDFAState this[IOilexerGrammarProductionRuleEntry entry]
        {
            get
            {
                lock (this.state.lookup)
                    foreach (GrammarVocabulary transition in this.Keys)
                        if (transition.Contains(entry))
                            return this[transition];
                throw new ArgumentException("entry");
            }
        }

        protected override GrammarVocabulary GetTCheck()
        {
            return new GrammarVocabulary(this.state.symbols);
        }
    }
}
