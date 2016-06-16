using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IPredictionTreeDestination :
        IProductionRuleSource
    {
        /// <summary>
        /// Returns the <see cref="GrammarVocabulary"/> which denotes the symbol(s) necessary to advance.
        /// </summary>
        GrammarVocabulary DecidingFactor { get; }
    }
    public interface IPredictionTreeKnownDestination :
        IPredictionTreeDestination
    {
        /// <summary>Returns the <see cref="PredictionTreeLeaf"/> which is targeted by the <see cref="IPredictionTreeDestination"/></summary>
        PredictionTreeLeaf Target { get; }
        bool PossibleLeftRecursiveDecision { get; set; }
    }
    public interface IPredictionTreeDestinationPredictFailure : 
        IPredictionTreeDestination
    {
    }
}
