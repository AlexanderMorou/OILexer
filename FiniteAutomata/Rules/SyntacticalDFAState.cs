using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;
using System.Diagnostics;
using System.Linq;
using Oilexer._Internal.Inlining;

namespace Oilexer.FiniteAutomata.Rules
{
    public class SyntacticalDFAState :
        DFAState<GrammarVocabulary, SyntacticalDFAState, IProductionRuleSource>
    {
        private IProductionRuleSource source;
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
            return source is IProductionRuleItem;
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
