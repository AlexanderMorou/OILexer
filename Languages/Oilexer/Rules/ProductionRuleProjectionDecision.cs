using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with the 
    /// deciding factor of a given projection.
    /// </summary>
    /// <remarks>Denotes the deciding transitional element as well
    /// as the node that is to be transitioned into.</remarks>
    public class ProductionRuleProjectionDecision :
        IProductionRuleProjectionDecision
    {
        /// <summary>
        /// Returns/sets the <see cref="ProductionRuleProjectionNode"/>
        /// which is the target of the decision.
        /// </summary>
        public ProductionRuleProjectionNode Target { get; set; }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.Target.RootNode.Value.Rule;  }
        }

        public GrammarVocabulary DecidingFactor { get; internal set; }
    }
}
