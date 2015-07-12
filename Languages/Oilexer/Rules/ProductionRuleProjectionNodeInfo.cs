using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of a projection node's information.
    /// </summary>
    internal class ProductionRuleProjectionNodeInfo :
        ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>>,
        IProductionRuleProjectionNodeInfo<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>
    {
        private Dictionary<int, List<ProductionRuleProjectionDPath>> cyclePaths = new Dictionary<int, List<ProductionRuleProjectionDPath>>();
        private ProductionRuleLeftRecursionType leftRecursionType;

        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionNode"/> which contains
        /// the <see cref="ProductionRuleProjectionNodeInfo"/>.
        /// </summary>
        public ProductionRuleProjectionNode Parent { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> from which the
        /// <see cref="ProductionRuleProjectionNodeInfo"/> is derived.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry Rule { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ProductionRuleProjectionNodeInfo"/> instance
        /// with the <paramref name="parent"/> provided.
        /// </summary>
        /// <param name="parent">The <see cref="ProductionRuleProjectionNode"/>
        /// from which the <see cref="ProductionRuleProjectionNodeInfo"/> is derived.</param>
        public ProductionRuleProjectionNodeInfo(ProductionRuleProjectionNode parent, SyntacticalDFAState originalState, IOilexerGrammarProductionRuleEntry rule)
        {
            this.Parent = parent;
            this.OriginalState = originalState;
            this.Rule = rule;
        }

        /// <summary>
        /// Returns whether the <see cref="Parent"/> represents
        /// a rule's node.
        /// </summary>
        /// <remarks>Returns true when <see cref="OriginalState"/> is a <see cref="SyntacticalDFARootState"/>; false, otherwise.</remarks>
        public bool IsRuleNode { get { return this.OriginalState is SyntacticalDFARootState; } }

        /// <summary>
        /// Returns the <see cref="SyntacticalDFAState"/> from which
        /// the <see cref="ProductionRuleProjectionNodeInfo"/> originates.
        /// </summary>
        public SyntacticalDFAState OriginalState { get; internal set; }

        internal void CheckDependencies()
        {
        }


        public void ConstructInitialLookAheadProjection(int cycleIndex, int maxCycles)
        {
            if (!this.cyclePaths.ContainsKey(cycleIndex))
                this.cyclePaths.Add(cycleIndex, new List<ProductionRuleProjectionDPath>());
            int previousCycle = cycleIndex - 1;
            int previousCycle2 = previousCycle - 1;
            if (previousCycle2 >= 0)
                this.cyclePaths[previousCycle2].Clear();
            this.cyclePaths[cycleIndex] = this.ObtainAllPaths(cycleIndex, maxCycles);
        }

        public bool ConstructEpsilonLookAheadProjection(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, int cycleDepth)
        {
            if (cycleDepth > 1)
                this.cyclePaths[cycleDepth - 2].Clear();
            var currentCycle = this.cyclePaths[cycleDepth - 1];
            bool result = false;
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath> resultTable = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath>();
            var previousLeftRecursive = leftRecursionType;
            if (leftRecursionType == ProductionRuleLeftRecursionType.None)
            {
                var temp = 
                (from p in currentCycle
                 where p.Depth == 0
                 select p.GetRecursionType()).Aggregate(ProductionRuleProjectionDPath.LeftRecursionAggregateDelegate);
                if (leftRecursionType != (leftRecursionType | temp))
                    leftRecursionType = temp;
            }
            if (previousLeftRecursive != leftRecursionType)
                result = true;
            var transitionalPaths = (from p in currentCycle
                                     from ePath in p.ObtainEpsilonTransitions(fullSeries)
                                     select ePath).ToArray();
            foreach (var transitionalPath in transitionalPaths)
                if (!currentCycle.Contains(transitionalPath))
                {
                    currentCycle.Add(transitionalPath);
                    var leftRecursionChange =
                        (from path in currentCycle
                         select path.GetRecursionType()).Aggregate(ProductionRuleProjectionDPath.LeftRecursionAggregateDelegate);
                    
                }
            foreach (var path in currentCycle)
                /* *
                 * Use a transition table to sort out ambiguities.
                 * */
                foreach (var transition in path.CurrentNode.Value.OriginalState.OutTransitions.Keys)
                {
                    if (leftRecursionType == ProductionRuleLeftRecursionType.None)
                    {
                        //var tokVariant = transition.GetTokenVariant();
                        var tokenVar = transition.GetTokenVariant();
                        var reducedRules = transition.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).Select(k => k.Symbol);
                        var ruleVar = new GrammarVocabulary(transition.symbols, reducedRules.ToArray());

                        if (!tokenVar.IsEmpty)
                            resultTable.Add(tokenVar, new List<ProductionRuleProjectionDPath>() { path });
                        if (!ruleVar.IsEmpty)
                            resultTable.Add(ruleVar, new List<ProductionRuleProjectionDPath>() { path });
                    }
                    else
                        resultTable.Add(transition, new List<ProductionRuleProjectionDPath>() { path });
                }
            int currentCount = this.Count;
            if (currentCount != resultTable.Count)
            {
                result = true;
                if (this.Count > 0)
                    this._Clear();

                foreach (var transition in resultTable.Keys)
                {
                    var pathSet = ProductionRuleProjectionDPathSet.GetPathSet(transition, resultTable[transition], this.Parent, ProductionRuleProjectionType.LookAhead, PredictionDerivedFrom.Lookahead_Epsilon);
                    this._Add(transition, pathSet);
                    pathSet.CheckReductionState(fullSeries, ruleVocabulary);
                    pathSet.ProcessCommonSymbol();
                }
            }

            return result;
        }

        private List<ProductionRuleProjectionDPath> ObtainAllPaths(int cycleIndex, int maxCycles)
        {
            if (cycleIndex == 0 && maxCycles == 1)
                return (from path in YieldAll().ToArray()
                        let rootPath = new ProductionRuleProjectionDPath(path.ToArray())
                        from subPath in rootPath.GetAllDepths()
                        select subPath).ToList();
            else if (cycleIndex == 0)
                return (from path in YieldAll().ToArray()
                        let rootPath = new ProductionRuleProjectionDPath(path.ToArray())
                        select rootPath).ToList();
            else if (maxCycles - 1 == cycleIndex)
                return (from path in YieldAll(cycleIndex).ToArray()
                        let rootPath = new ProductionRuleProjectionDPath(path.ToArray())
                        from subPath in rootPath.GetAllDepths()
                        select subPath).ToList();
            else
                return (from path in YieldAll(cycleIndex).ToArray()
                        let rootPath = new ProductionRuleProjectionDPath(path.ToArray())
                        select rootPath).ToList();
        }

        private IEnumerable<IEnumerable<ProductionRuleProjectionNode>> YieldAll(Stack<ProductionRuleProjectionNode> currentLayout = null)
        {
            if (currentLayout == null)
                currentLayout = new Stack<ProductionRuleProjectionNode>();
            if (currentLayout.Contains(this.Parent))
            {
                yield return new[] { this.Parent };
                yield break;
            }
            currentLayout.Push(this.Parent);
            try
            {
                foreach (var rule in this.Parent.Keys)
                {
                    var targetInstance = this.Parent[rule].Value as ProductionRuleProjectionNodeInfo;
                    if (targetInstance == null)
                        continue;
                    /* *
                     * Build it from the deepest up to the current, in reverse order.
                     * */
                    foreach (var element in targetInstance.YieldAll(currentLayout).ToArray())
                        yield return new[] { this.Parent }.Concat(element).ToArray();
                }
                if (this.Parent.Keys.Count == 0)
                    yield return new[] { this.Parent };
            }
            finally
            {
                currentLayout.Pop();
            }
        }

        private IEnumerable<IEnumerable<ProductionRuleProjectionNode>> YieldAll(int cycleIndex)
        {
            foreach (var rule in this.Parent.Keys)
            {
                var targetInstance = this.Parent[rule].Value as ProductionRuleProjectionNodeInfo;
                if (targetInstance == null)
                    continue;
                /* *
                 * Build it from the deepest up to the 
                 * current, in reverse order.
                 * */
                foreach (var element in targetInstance.cyclePaths[cycleIndex - 1].ToArray())
                    yield return new[] { this.Parent }.Concat(element).ToArray();
            }
            if (this.Parent.Keys.Count == 0)
                yield return new[] { this.Parent };
        }

        public ProductionRuleLeftRecursionType LeftRecursionType
        {
            get
            {
                return this.leftRecursionType;
            }
        }


        public void ClearCache()
        {
            this.cyclePaths.Clear();
            this._Clear();
        }
    }
}
