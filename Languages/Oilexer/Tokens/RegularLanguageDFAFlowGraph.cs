using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class SyntacticalDFAFlowGraph :
        FiniteAutomataDFAFlowGraph<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, SyntacticalDFAFlowGraphNode, SyntacticalDFAFlowGraph>
    {
    }
    public class SyntacticalDFAFlowGraphNode :
        FiniteAutomataFlowGraphNode<GrammarVocabulary, SyntacticalDFAState, SyntacticalDFAState, IProductionRuleSource, SyntacticalDFAFlowGraphNode, SyntacticalDFAFlowGraph>
    {

    }
}
