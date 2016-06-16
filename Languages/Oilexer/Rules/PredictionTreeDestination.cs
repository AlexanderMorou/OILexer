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
    public class PredictionTreeDestination :
        IPredictionTreeKnownDestination
    {
        /// <summary>Returns/sets the <see cref="PredictionTreeLeaf"/> which is the target of the decision.</summary>
        public PredictionTreeLeaf Target { get; set; }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.Target.RootLeaf.Veins.Rule;  }
        }

        public GrammarVocabulary DecidingFactor { get; internal set; }
        public bool PossibleLeftRecursiveDecision { get; set; }
    }
    public class PredictionTreeDestinationPredictFailure :
        IPredictionTreeDestinationPredictFailure
    {
        public GrammarVocabulary DecidingFactor { get; internal set; }

        public IOilexerGrammarProductionRuleEntry Rule { get; set; }
    }
}
