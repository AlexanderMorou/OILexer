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
    public class PredictionTreeLeafVeins :
        ControlledDictionary<GrammarVocabulary, PredictionTree>
    {
        private Dictionary<int, List<PredictionTreeBranch>> cyclePaths = new Dictionary<int, List<PredictionTreeBranch>>();
        private ProductionRuleLeftRecursionType leftRecursionType;
        private bool? _alwaysLeftRecursive;

        /// <summary>
        /// Returns the <see cref="PredictionTreeLeaf"/> which contains
        /// the <see cref="PredictionTreeLeafVeins"/>.
        /// </summary>
        public PredictionTreeLeaf Leaf { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> from which the <see cref="PredictionTreeLeafVeins"/> is derived.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry Rule { get; private set; }

        /// <summary>Creates a new <see cref="PredictionTreeLeafVeins"/> instance with the <paramref name="parent"/> provided.</summary>
        /// <param name="parent">The <see cref="PredictionTreeLeaf"/> from which the <see cref="PredictionTreeLeafVeins"/> is derived.</param>
        public PredictionTreeLeafVeins(PredictionTreeLeaf leaf, SyntacticalDFAState originalState, IOilexerGrammarProductionRuleEntry rule)
        {
            this.Leaf = leaf;
            this.DFAOriginState = originalState;
            this.Rule = rule;
        }

        /// <summary>Returns whether the <see cref="Leaf"/> represents a rule's node.</summary><remarks>Returns true when <see cref="DFAOriginState"/> is a <see cref="SyntacticalDFARootState"/>; false, otherwise.</remarks>
        public bool IsRuleEntryPoint { get { return this.DFAOriginState is SyntacticalDFARootState; } }

        /// <summary>Returns the <see cref="SyntacticalDFAState"/> from which the <see cref="PredictionTreeLeafVeins"/> originates.</summary>
        public SyntacticalDFAState DFAOriginState { get; internal set; }

        internal void CheckDependencies()
        {
        }

        public void ConstructInitialLookAheadProjection(int cycleIndex, int maxCycles)
        {
            if (!this.cyclePaths.ContainsKey(cycleIndex))
                 this.cyclePaths.Add(cycleIndex, this.ObtainAllPaths(cycleIndex, maxCycles));
        }

        public bool ConstructEpsilonLookAheadProjection(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, int cycleDepth)
        {
            if (cycleDepth > 1)
                this.cyclePaths[cycleDepth - 2].Clear();
            var currentCycle = this.cyclePaths[cycleDepth - 1];
            bool result = false;
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, PredictionTreeBranch> resultTable = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, PredictionTreeBranch>();
            var previousLeftRecursive = leftRecursionType;
            if (leftRecursionType == ProductionRuleLeftRecursionType.None)
            {
                var temp = 
                (from p in currentCycle
                 where p.Depth == 0
                 select p.GetRecursionType()).Aggregate(PredictionTreeBranch.LeftRecursionAggregateDelegate);
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
                         select path.GetRecursionType()).Aggregate(PredictionTreeBranch.LeftRecursionAggregateDelegate);
                    
                }
            var leftRecursivePaths =
                (from p in currentCycle
                 where p.Depth == 0
                 let recurType = p.GetRecursionType()
                 where recurType != ProductionRuleLeftRecursionType.None
                 select p.GetFirstRecursion()).ToArray();
            foreach (var path in currentCycle)
            {
                /* *
                 * Use a transition table to sort out ambiguities.
                 * */
                foreach (var transition in path.CurrentNode.Veins.DFAOriginState.OutTransitions.Keys)
                {
                    if (leftRecursionType == ProductionRuleLeftRecursionType.None)
                    {
                        //var tokVariant = transition.GetTokenVariant();
                        var tokenVar = transition.GetTokenVariant();
                        var reducedRules = transition.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).Select(k => k.Symbol);
                        var ruleVar = new GrammarVocabulary(transition.symbols, reducedRules.ToArray());

                        if (!tokenVar.IsEmpty)
                            resultTable.Add(tokenVar, new List<PredictionTreeBranch>() { path });
                        if (!ruleVar.IsEmpty)
                            resultTable.Add(ruleVar, new List<PredictionTreeBranch>() { path });
                    }
                    else
                    {
                        var currentSet = new List<PredictionTreeBranch>() { path };
                        //if (this.Rule.Name == "RelationalExp")
                        //{
                        //    if (transition.ToString() == "Identifier")
                        //    {
                                foreach (var lrPath in leftRecursivePaths)
                                    currentSet.Add(lrPath.GetTransitionPath(lrPath, lrPath.Take(lrPath.Depth).ToArray(), path, ruleVocabulary, false, false, false));
                        //    }
                        //}
                        resultTable.Add(transition, currentSet);
                    }
                }
            }
            int currentCount = this.Count;
            if (currentCount != resultTable.Count)
            {
                result = true;
                if (this.Count > 0)
                    this._Clear();

                foreach (var transition in resultTable.Keys)
                {
                    var pathSet = PredictionTree.GetPathSet(transition, resultTable[transition], this.Leaf, ProductionRuleProjectionType.LookAhead, PredictionDerivedFrom.Lookahead_Epsilon);
                    this._Add(transition, pathSet);
                    pathSet.CheckReductionState(fullSeries, ruleVocabulary);
                    //pathSet.ProcessCommonSymbol(fullSeries, ruleVocabulary);
                }
            }

            return result;
        }

        private List<PredictionTreeBranch> ObtainAllPaths(int cycleIndex, int maxCycles)
        {
            if (cycleIndex == 0 && maxCycles == 1)
                return (from path in YieldAll().ToArray()
                        let rootPath = new PredictionTreeBranch(path.ToArray())
                        from subPath in rootPath.GetAllDepths()
                        select subPath).ToList();
            else if (cycleIndex == 0)
                return (from path in YieldAll().ToArray()
                        let rootPath = new PredictionTreeBranch(path.ToArray())
                        select rootPath).ToList();
            else if (maxCycles - 1 == cycleIndex)
                return (from path in YieldAll(cycleIndex).ToArray()
                        let rootPath = new PredictionTreeBranch(path.ToArray())
                        from subPath in rootPath.GetAllDepths()
                        select subPath).ToList();
            else
                return (from path in YieldAll(cycleIndex).ToArray()
                        let rootPath = new PredictionTreeBranch(path.ToArray())
                        select rootPath).ToList();
        }

        private IEnumerable<IEnumerable<PredictionTreeLeaf>> YieldAll(Stack<PredictionTreeLeaf> currentLayout = null)
        {
            if (currentLayout == null)
                currentLayout = new Stack<PredictionTreeLeaf>();
            if (currentLayout.Contains(this.Leaf))
            {
                yield return new[] { this.Leaf };
                yield break;
            }
            currentLayout.Push(this.Leaf);
            try
            {
                foreach (var rule in this.Leaf.Keys)
                {
                    var targetVeins = this.Leaf[rule].Veins as PredictionTreeLeafVeins;
                    if (targetVeins == null)
                        continue;
                    /* *
                     * Build it from the deepest up to the current, in reverse order.
                     * */
                    foreach (var element in targetVeins.YieldAll(currentLayout).ToArray())
                        yield return new[] { this.Leaf }.Concat(element).ToArray();
                }
                if (this.Leaf.Keys.Count == 0)
                    yield return new[] { this.Leaf };
            }
            finally
            {
                currentLayout.Pop();
            }
        }

        private IEnumerable<IEnumerable<PredictionTreeLeaf>> YieldAll(int cycleIndex)
        {
            foreach (var rule in this.Leaf.Keys)
            {
                var targetInstance = this.Leaf[rule].Veins as PredictionTreeLeafVeins;
                if (targetInstance == null)
                    continue;
                /* *
                 * Build it from the deepest up to the 
                 * current, in reverse order.
                 * */
                foreach (var element in targetInstance.cyclePaths[cycleIndex - 1].ToArray())
                    yield return new[] { this.Leaf }.Concat(element).ToArray();
            }
            if (this.Leaf.Keys.Count == 0)
                yield return new[] { this.Leaf };
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

        public bool IsAlwaysLeftRecursive
        {
            get 
            {
                return (this._alwaysLeftRecursive ?? (this._alwaysLeftRecursive = this.InitializeIsAlwaysLeftRecursive()).Value);
            }
        }

        private bool InitializeIsAlwaysLeftRecursive()
        {
            if (this.LeftRecursionType == ProductionRuleLeftRecursionType.None)
                return false;
            if (this.DFAOriginState.OutTransitions.FullCheck.GetTokenVariant().TrueCount == 0)
                return this.Leaf.Values.All(k => k.LookAhead.Values.Any(l => l.Any(j => j.CurrentNode == this.Leaf.RootLeaf)));
            return false;
        }
    }
}
