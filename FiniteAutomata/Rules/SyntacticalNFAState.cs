using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.Builder;

namespace Oilexer.FiniteAutomata.Rules
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
