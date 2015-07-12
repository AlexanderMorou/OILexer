using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IProductionRuleProjectionNode<TPath, TNode> :
        IKeyedTreeNode<IOilexerGrammarProductionRuleEntry, IProductionRuleProjectionNodeInfo<TPath, TNode>, TNode>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
        /// <summary>
        /// Returns the <see cref="IControlledDictionary{TKey, TValue}"/>
        /// which denotes the look-ahead for the current
        /// machine, if necessary.
        /// </summary>
        IControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<TPath, TNode>> LookAhead { get; }
        /// <summary>
        /// Constructs the initial look-ahead for the
        /// transitions.
        /// </summary>
        void ConstructInitialLookahead(GrammarSymbolSet grammarSymbols, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary);
    }
}
