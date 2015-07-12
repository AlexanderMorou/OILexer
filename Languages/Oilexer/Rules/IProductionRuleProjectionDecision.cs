using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IProductionRuleProjectionDecision :
        IProductionRuleSource
    {
        /// <summary>
        /// Returns the <see cref="GrammarVocabulary"/> which
        /// denotes the symbol(s) necessary to advance.
        /// </summary>
        GrammarVocabulary DecidingFactor { get; }
    }
}
