using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalDFAState :
        DFAState<GrammarVocabulary, SyntacticalDFAState, IProductionRuleSource>
    {
        private bool? canBeEmpty;
        internal ParserBuilder builder;
        public SyntacticalDFAState()
        {

        }

        public SyntacticalDFAState(ParserBuilder builder)
        {
            this.builder = builder;
        }

        protected override bool SourceSetPredicate(IProductionRuleSource source)
        {
            if (!(source is IProductionRuleItem))
                return false;
            var tSource = (IProductionRuleItem)source;
            return !string.IsNullOrEmpty(tSource.Name);
        }

        public bool ContainsRule(IProductionRuleEntry rule)
        {
            return this.OutTransitions.FullCheck.Contains(rule);
        }

        public SyntacticalDFARootState this[IProductionRuleEntry rule]
        {
            get
            {
                return this.builder.RuleDFAStates[rule];
            }
        }

        protected override IFiniteAutomataTransitionTable<GrammarVocabulary, SyntacticalDFAState, SyntacticalDFAState> InitializeOutTransitionTable()
        {
            lock (this.builder)
                return new SyntacticalDFAStateTransitionTable(this);
        }

        public new SyntacticalDFAStateTransitionTable OutTransitions
        {
            get
            {
                return (SyntacticalDFAStateTransitionTable)base.OutTransitions;
            }
        }
        
        /// <summary>
        /// Returns whether a given state can be empty due to the rules within it yielding
        /// </summary>
        public bool CanBeEmpty
        {
            get
            {
                if (this.canBeEmpty == null)
                    /* *
                     * Potentially expensive operation, cache.
                     * */
                    if (this.IsEdge)
                        this.canBeEmpty = true;
                    else
                        this.canBeEmpty = SyntacticalDFAState.CanBeEmptyInternal(this, new List<SyntacticalDFAState>());
                return this.canBeEmpty.Value;
            }
        }

        private static bool CanBeEmptyInternal(SyntacticalDFAState target, List<SyntacticalDFAState> list)
        {
            if (list.Contains(target))
                return target.IsEdge;
            if (target.IsEdge)
                return true;
            list.Add(target);
            foreach (var transition in target.OutTransitions.Checks)
            {
                foreach (var rule in transition.Breakdown.Rules)
                {
                    var ruleState = target.builder.RuleDFAStates[rule.Source];
                    if (CanBeEmptyInternal(ruleState, list) &&
                        CanBeEmptyInternal(target.OutTransitions[transition], list))
                        return true;
                }
                foreach (var token in transition.Breakdown.Tokens.Cast<InlinedTokenEntry>())
                {
                    if (token.DFAState.IsEdge &&
                        CanBeEmptyInternal(target.OutTransitions[transition], list))
                        return true;
                }
            }
            return false;
        }

    }
}
