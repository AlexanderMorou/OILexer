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
using AllenCopeland.Abstraction.Utilities.Collections;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalDFAState :
        DFAState<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>,
        IProductionRuleSource
    {
        private bool? canBeEmpty;
        private bool? containsEmptyTarget;
        internal ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup;
        internal GrammarSymbolSet symbols;
#if DEBUG
        private string debugString;
#endif
        public SyntacticalDFAState()
        {

        }

        internal virtual void ReduceDFA(bool recognizer, Func<SyntacticalDFAState,SyntacticalDFAState, bool> additionalReducer = null)
        {
            Reduce(this, recognizer, additionalReducer);
        }

        public SyntacticalDFAState(ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (lookup == null)
                throw new ArgumentNullException("lookup");
            this.lookup = lookup;
            this.symbols = symbols;
        }

        protected override bool SourceSetPredicate(IProductionRuleSource source)
        {
            if (!(source is IProductionRuleItem))
                return false;
            var tSource = (IProductionRuleItem)source;
            return !string.IsNullOrEmpty(tSource.Name);
        }

        public bool ContainsRule(IOilexerGrammarProductionRuleEntry rule)
        {
            return this.OutTransitions.FullCheck.Contains(rule);
        }

        public SyntacticalDFARootState this[IOilexerGrammarProductionRuleEntry rule]
        {
            get
            {
                return this.lookup[rule];
            }
        }

        protected override IFiniteAutomataTransitionTable<GrammarVocabulary, SyntacticalDFAState, SyntacticalDFAState> InitializeOutTransitionTable()
        {
            lock (this.lookup)
                return new SyntacticalDFAStateTransitionTable(this);
        }

        public new SyntacticalDFAStateTransitionTable OutTransitions
        {
            get
            {
                return (SyntacticalDFAStateTransitionTable)base.OutTransitions;
            }
        }

        public override string ToString()
        {
            return string.Format("StateValue: {0}", StateValue);
        }

#if DEBUG
        protected string DebugString
        {
            get
            {
                if (this.debugString == null)
                    this.debugString = base.ToString();
                return this.debugString;
            }
        }
#endif

        /// <summary>
        /// Returns whether a given state can be empty due to the rules within it yielding
        /// on their initial state.
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

        /// <summary>
        /// Returns whether a given state can be empty due to the rules within it yielding
        /// on their initial state.
        /// </summary>
        public bool ContainsEmptyTarget
        {
            get
            {
                if (this.containsEmptyTarget == null)
                    /* *
                     * Potentially expensive operation, cache.
                     * */
                    if (this.IsEdge)
                        this.containsEmptyTarget = true;
                    else
                        this.containsEmptyTarget = SyntacticalDFAState.ContainsEmptyTargetInternal(this, new List<SyntacticalDFAState>());
                return this.containsEmptyTarget.Value;
            }
        }

        public static bool ContainsEmptyTargetInternal(SyntacticalDFAState target, List<SyntacticalDFAState> list)
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
                    var ruleState = target.lookup[rule.Source];
                    if (ContainsEmptyTargetInternal(ruleState, list))
                        return true;
                }
                foreach (var token in transition.Breakdown.Tokens.Cast<InlinedTokenEntry>())
                    if (token.DFAState.IsEdge)
                        return true;
            }
            return false;
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
                    var ruleState = target.lookup[rule.Source];
                    if (CanBeEmptyInternal(ruleState, list) &&
                        CanBeEmptyInternal(target.OutTransitions[transition], list))
                        return true;
                }
                foreach (var token in transition.Breakdown.Tokens.Cast<InlinedTokenEntry>())
                {
                    if ((!(token is IOilexerGrammarTokenEofEntry)) && token.DFAState.IsEdge &&
                        CanBeEmptyInternal(target.OutTransitions[transition], list))
                        return true;
                }
            }
            return false;
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
