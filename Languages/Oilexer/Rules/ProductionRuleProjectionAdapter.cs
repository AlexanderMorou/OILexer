using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleProjectionAdapter :
        DFAAdapter<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, ProductionRuleProjectionAdapter, ProductionRuleProjectionContext>
    {
        public override void ConnectContext(object connectionContext)
        {
            this.AssociatedContext.Connect(this, connectionContext as ParserCompiler);
        }

        public IEnumerable<ProductionRuleProjectionAdapter> All
        {
            get
            {
                Stack<ProductionRuleProjectionAdapter> toObserve = new Stack<ProductionRuleProjectionAdapter>();
                List<ProductionRuleProjectionAdapter> seen = new List<ProductionRuleProjectionAdapter>();
                toObserve.Push(this);
                while (toObserve.Count > 0)
                {
                    var current = toObserve.Pop();
                    if (seen.Contains(current))
                        continue;
                    yield return current;
                    seen.Add(current);
                    foreach (var transition in current.OutgoingTransitions.Keys)
                        toObserve.Push(current.OutgoingTransitions[transition]);
                }
            }
        }
    }
}
