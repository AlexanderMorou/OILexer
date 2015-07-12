using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleProjectionFollowFailure :
        IProductionRuleProjectionDecision
    {
        public ProductionRuleProjectionFollowFailure(ProductionRuleProjectionNode followNode)
        {
            this.Node = followNode;
        }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get
            {
                return this.Node.Rule;
            }
        }

        public ProductionRuleProjectionNode Node { get; private set; }

        public GrammarVocabulary DecidingFactor { get; internal set; }
    }
}
