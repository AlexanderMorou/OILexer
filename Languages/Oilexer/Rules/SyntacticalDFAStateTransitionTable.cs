using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
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

        public Tuple<IProductionRuleEntry, SyntacticalDFARootState, SyntacticalDFAState> GetNode(IProductionRuleEntry rule)
        {
            lock (this.state.builder)
                foreach (var key in this.Keys)
                    if (key.Contains(rule))
                        return new Tuple<IProductionRuleEntry, SyntacticalDFARootState, SyntacticalDFAState>(rule, this.state.builder.RuleDFAStates[rule], this[key]);
            throw new ArgumentException("rule");
        }

        public SyntacticalDFAState this[IProductionRuleEntry entry]
        {
            get
            {
                lock (this.state.builder)
                    foreach (GrammarVocabulary transition in this.Keys)
                        if (transition.Contains(entry))
                            return this[transition];
                throw new ArgumentException("entry");
            }
        }

        protected override GrammarVocabulary GetTCheck()
        {
            return new GrammarVocabulary(this.state.builder.GrammarSymbols);
        }
    }
}
