using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for the value aspect of a given
    /// <typeparamref name="TNode"/> instance.
    /// </summary>
    /// <remarks>Provides further context within the tree nodes to 
    /// simplify the object model.</remarks>
    /// <typeparam name="TPath">The type of <see cref="IProductionRuleProjectionPath{TPath, TNode}"/>
    /// from which sequences of <typeparamref name="TNode"/>
    /// are constructed.</typeparam>
    /// <typeparam name="TNode">The creatable type of
    /// <see cref="IProductionRuleProjectionNode{TPath, TNode}"/>
    /// on which the <see cref="IProductionRuleProjectionNodeInfo{TPath, TNode}"/>
    /// are contained.</typeparam>
    public interface IProductionRuleProjectionNodeInfo<TPath, TNode> :
        IControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<TPath, TNode>>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
        /// <summary>
        /// Returns the <typeparamref name="TNode"/> from which the
        /// <see cref="IProductionRuleProjectionNodeInfo{TPath, TNode}"/> 
        /// is derived.
        /// </summary>
        TNode Parent { get; }
        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> associated to the
        /// <see cref="IProductionRuleProjectionNodeInfo{TPath, TNode}"/>.
        /// </summary>
        IOilexerGrammarProductionRuleEntry Rule { get; }
        /// <summary>
        /// Returns the <see cref="SyntacticalDFAState"/> which is represented by
        /// the <see cref="Parent"/>.
        /// </summary>
        SyntacticalDFAState OriginalState { get; }
        
        /// <summary>
        /// Returns whether the <see cref="OriginalState"/> is derived from 
        /// <see cref="SyntacticalDFARootState"/>.
        /// </summary>
        bool IsRuleNode { get; }

        void ConstructInitialLookAheadProjection(int cycleIndex, int maxCycles);

        bool ConstructEpsilonLookAheadProjection(Dictionary<SyntacticalDFAState, TNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, int cycleDepth);

        ProductionRuleLeftRecursionType LeftRecursionType { get; }

        void ClearCache();
    }
}
