using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleProjectionReduction :
        IProductionRuleSource
    {
        public int LookAheadDepth { get; internal set; }
        /// <summary>
        /// Returns/sets the <see cref="GrammarVocabulary"/> which
        /// denotes the symbol(s) necessary to advance.
        /// </summary>
        public GrammarVocabulary ReducedRule { get; set; }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }

        public PredictionTree BranchPoint { get; set; }

        public PredictionTree ReducePoint { get; set; }
    }
}
