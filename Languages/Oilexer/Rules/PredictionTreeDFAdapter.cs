using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeDFAdapter :
        DFAAdapter<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, PredictionTreeDFAdapter, PredictionTreeDFAContext>
    {
        public override void ConnectContext(object connectionContext)
        {
            this.AssociatedContext.Connect(this, connectionContext as ParserCompiler);
        }

        public IEnumerable<PredictionTreeDFAdapter> All
        {
            get
            {
                Stack<PredictionTreeDFAdapter> toObserve = new Stack<PredictionTreeDFAdapter>();
                List<PredictionTreeDFAdapter> seen = new List<PredictionTreeDFAdapter>();
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
