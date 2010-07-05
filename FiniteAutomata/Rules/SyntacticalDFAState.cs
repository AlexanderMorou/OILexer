using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;
using System.Diagnostics;

namespace Oilexer.FiniteAutomata.Rules
{
    [DebuggerDisplay("{StringForm}")]
    public class SyntacticalDFAState :
        DFAState<GrammarVocabulary, SyntacticalDFAState, IProductionRuleSource>
    {
        private IProductionRuleSource source;
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
        internal virtual string StringForm { get { return this.ToString(); } }

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
    }
}
