using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// A normal deterministic adapter for a given <see cref="SyntacticalDFAState"/>
    /// which aims to augment the state machine by providing context
    /// relative to states which require a prediction.
    /// </summary>
    public class ProductionRuleNormalAdapter :
        DFAAdapter<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, ProductionRuleNormalAdapter, ProductionRuleNormalContext>
    {
        public override void ConnectContext(object connectContext)
        {
            if (!(connectContext is ParserCompiler))
                throw new ArgumentOutOfRangeException("connectContext");
            this.AssociatedContext.ConnectAdapter(this, (ParserCompiler)connectContext);
        }
    }
}
