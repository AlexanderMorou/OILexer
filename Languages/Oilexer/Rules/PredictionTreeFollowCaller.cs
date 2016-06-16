using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeFollowCaller :
        IPredictionTreeDestination
    {
        public PredictionTreeFollowCaller(PredictionTreeLeaf followNode)
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

        public PredictionTreeLeaf Node { get; private set; }

        public GrammarVocabulary DecidingFactor { get; internal set; }
    }
}
