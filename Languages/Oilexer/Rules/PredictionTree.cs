using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// With a given <see cref="PredictionTree.Discriminator"/>, specifies the paths which tie for the given grammar vocabulary.
    /// </summary>
    /// <remarks>
    /// Represents the snapshot of the dependent paths before the transition.
    /// </remarks>
    [DebuggerTypeProxyAttribute(typeof(PredictionTree.DebuggerTypeProxy))]
    [DebuggerDisplay("{_instTracker} : {Discriminator}")]
    public class PredictionTree :
        ControlledCollection<PredictionTreeBranch>,
        IEquatable<PredictionTree>,
        IProductionRuleSource
    {
        private static int _InstanceTracker = 0;
        private static object instLocker = new object();
        private int _instTracker;
        private PredictionDerivedFrom _derivedFrom;
        private bool _hasBeenReducedAlready = false;
        private SyntacticalNFAState nfaState;
        private bool hasBuiltNFA = false;
        private LookAheadReductionType _reductionType = LookAheadReductionType.Uncalculated;
        private int reductionLookAheadDrop;
        private bool isRepetitionPoint;
        private ControlledDictionary<GrammarVocabulary, PredictionTree> lookAhead;
        private PredictionTreeLeaf commonSymbol;
        private List<PredictionTreeLeaf> commonSymbols;
        private List<PredictionTreeBranch> originalSet;
        private PredictionTree pointOfRedundancy;
        private ProductionRuleProjectionReduction reductionContext;
        private List<PredictionTree> replFixups = new List<PredictionTree>();
        private string expandedForm;
        internal ProductionRuleProjectionType projectionType = ProductionRuleProjectionType.Unspecified;
        private Dictionary<PredictionTreeBranch, int[]> followEpsilonLevels;
        private bool _isBadReductionCandidate = false;
        private Dictionary<PredictionTreeBranch, PredictionTreeBranch[]> origins = new Dictionary<PredictionTreeBranch,PredictionTreeBranch[]>();
        private PredictionTree(GrammarVocabulary discriminator, List<PredictionTreeBranch> series, PredictionDerivedFrom derivedFrom)
        {
            this._derivedFrom = derivedFrom;
            lock (instLocker)
                this._instTracker = _InstanceTracker++;
            Debug.Assert(series.Count > 0);
            /* *
             * Too many reducable sources push data through here.
             * *
             * Instead of relying on it being done, make sure.
             * */
            series = series.Distinct().ToList();
            /* *
             * The original set of paths that caused this
             * production to be.
             * */
            this.originalSet = series.ToArray().ToList();
            this.baseList = this.originalSet.ToArray().ToList();
            this.Discriminator = discriminator;

        }
        public GrammarVocabulary Discriminator { get; internal set; }

        //public override string ToString()
        //{
        //    return string.Format("{0} : {1}", this._instTracker, this.Discriminator);
        //}

        /// <summary>
        /// Returns the <see cref="PredictionTree"/>
        /// which denotes a point of redundancy in the
        /// result state determination machine.
        /// </summary>
        public PredictionTree ReplicationPoint { get { return this.pointOfRedundancy; } }

        public static PredictionTree GetPathSet(GrammarVocabulary discriminator, List<PredictionTreeBranch> series, PredictionTreeLeaf root, ProductionRuleProjectionType projectionType, PredictionDerivedFrom derivedFrom)
        {
            return new PredictionTree(discriminator, series, derivedFrom) { Root = root, projectionType = projectionType };
        }

        public LookAheadReductionType ReductionType
        {
            get
            {
                return this._reductionType;
            }
        }

        internal void CheckReductionState(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            if (this._reductionType == LookAheadReductionType.Uncalculated)
                this._reductionType = this.GetReductionType(fullSeries, ruleVocabulary);
        }

        public IEnumerable<PredictionTreeBranch> GetEpsilonLevel(int level = 0)
        {
            var result = new HashSet<PredictionTreeBranch>();
            foreach (var set in this.followEpsilonLevels.Keys)
            {
                var eps = this.followEpsilonLevels[set];
                if (eps.Contains(level))
                    result.Add(set);
            }
            return result;
        }

        private LookAheadReductionType GetReductionType(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            bool isLeftRecursive = false;
            if (this.isRepetitionPoint)
                return LookAheadReductionType.RepetitionPoint;
            if (this.IsLeftRecursiveRule(fullSeries, ruleVocabulary))
                isLeftRecursive = true;
            if (this.Rule.Name == "MultiplicitiveExpression")
            {
                if (this.CurrentLookAheadBranches == "MultiplicitiveExpression->OperatorOrPunctuator[*, /, %]")
                {

                }
            }
            if (!this._isBadReductionCandidate && this.GetHasCommonSymbol(this.originalSet, ref this.commonSymbol))
            {
                if (isLeftRecursive)
                {

                }
                return LookAheadReductionType.CommonForwardSymbol;
            }
            if (this.GetIsLocalTransition())
                return LookAheadReductionType.LocalTransition;
            if (isLeftRecursive)
                return LookAheadReductionType.LeftRecursive;
            else
                return LookAheadReductionType.None;
        }

        private bool IsLeftRecursiveRule(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            if (this.Count == 1)
                return false;
            if (this.GetCurrentPathSets().Any(k => k.ReductionType == LookAheadReductionType.LeftRecursive))
                return false;
            var projectedOrigin = this.ProjectOrigin(fullSeries, ruleVocabulary);
            var previousRootPaths = projectedOrigin[0].Select(k => k.PreviousPath).ToArray();
            var firstMin = this.Where(k => k.Depth == k.MinDepth).FirstOrDefault();
            /* Skip this check in the event that the request comes from the root. */
            if (!(previousRootPaths.Any(k => k.Depth == k.MinDepth)))
            {
                var distinctChildGroups = (from path in previousRootPaths
                                           group path by path[path.MinDepth + 1]).ToDictionary(k => k.Key, v => v.ToArray());
                if (distinctChildGroups.Keys.All(k => k.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None))
                {
                    var distinctChildRightSides =
                        (from child in distinctChildGroups.Keys
                         select new { Paths = distinctChildGroups[child].Select(path => (IEnumerable<PredictionTreeLeaf>)path.Skip(path.Depth).ToArray()).Distinct(ProductionRuleProjectionNodeSetComparer.Singleton).ToArray(), Child = child })
                         .ToDictionary(k => k.Child, v => v.Paths.Select(k => k.ToArray()).ToArray());
                    var initialSet = distinctChildRightSides.First();
                    /* If all children are left-recursive, contain the other relevant children within them, and all right hand side paths are equivalent, we have an indirectly left recursive prediction on our hands. */
                    if (distinctChildRightSides.Keys.All(child => distinctChildRightSides.Keys.Except(new[] { child }).All(j => child.Veins.ContainsKey(ruleVocabulary[j.Veins.Rule]))) && distinctChildRightSides.Skip(1).All(k => k.Value.Length == initialSet.Value.Length && k.Value.All(j => initialSet.Value.Any(h => h.SequenceEqual(j)))))
                    {
                        this.commonSymbols = new List<PredictionTreeLeaf>(distinctChildRightSides.Keys.ToArray());
                        /* This requires its own special 'disambiguator' method in the end due to the fact that the identity on the stack is not knowable until... it is. */
                        return true;
                    }
                }
            }
            else if (this.Count > 1 && firstMin != null && this.commonSymbol == null)
            {
                var selection = this.Except(new[] { firstMin }).ToArray();
                foreach (var element in selection)
                {
                    if (element.CurrentNode != firstMin.CurrentNode)
                        return false;
                    var prev = element.Take(element.Depth);
                    if (!prev.Any())
                        return false;
                    if (!prev.Any(k=>k == k.RootLeaf))
                        return false;

                }
                return true;
            }
            return false;
        }
        internal void ProcessCommonSymbol(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            if (this.ReductionType != LookAheadReductionType.CommonForwardSymbol)
                return;
            if (this._processedSymbol)
                return;
            this._processedSymbol = true;
            //this.CheckBack();
            PredictionTree backRef = null;
            var devLevels = (from path in this
                             let devLevel = Math.Max(0, path.GetCurrentDeviation() - (ReductionCount - 1))
                             select devLevel).Distinct().ToArray();
            //int devLevel = 0;
            foreach (var devLevel in devLevels)
            {
                if (devLevel > 0 && this.Previous != null)
                {
                    backRef = this.GetBack(devLevel);
                    this.commonSymbol.DenoteReductionPoint(this, backRef);
                    this._getBacker = backRef;
                }
                else
                    this.commonSymbol.DenoteReductionPoint(this);
                ProcessReductionContext(fullSeries, ruleVocabulary, backRef, devLevel);
            }
        }

        private void ProcessReductionContext(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, PredictionTree backRef, int devLevel)
        {
            if (this.reductionContext == null)
            {
                var transitionGrammar = ruleVocabulary[this.commonSymbol.Rule];
                this.reductionContext = this.Root.RootLeaf.SetReductionOn(transitionGrammar, 0, fullSeries);
            }
            if (this.reductionContext != null)
            {
                this.reductionContext.LookAheadDepth = devLevel;
                this.reductionContext.ReducePoint = this;
                this.reductionContext.BranchPoint = backRef ?? this;
            }
        }

        public ProductionRuleProjectionReduction ReductionDetail { get { return this.reductionContext; } }

        public short ReductionCount
        {
            get
            {
                int count = 0;
                PredictionTree current = this;
                while (current != null)
                {
                    if (current.ReductionType == LookAheadReductionType.CommonForwardSymbol)
                        count++;
                    current = (PredictionTree)current.Previous;
                }
                return (short)count;
                //return (short)(this.GetCurrentPathSets().Where(k => k.ReductionType == LookAheadReductionType.CommonForwardSymbol).Count());
            }
        }

        private PredictionTree GetBack(int steps)
        {
            PredictionTree current = this;

            for (int i = 0; i < steps; i++)
            {
                if (current != null)
                    current = (PredictionTree)current.Previous;
            }
            return current;
        }

        public int[] GetRightSideSimilarity(Tuple<int[], PredictionTree> other)
        {
            if (other.Item2.Count != this.Count)
                return new int[0];
            if (other.Item2.Equals(this))
                return (from p in this
                        select p.Count).ToArray();
            int[] result = new int[this.Count];
            for (int pathIndex = 0; pathIndex < this.Count; pathIndex++)
            {
                var leftPath = this[pathIndex];
                var rightPath = other.Item2[pathIndex];
                int minLength = Math.Min(leftPath.Count, rightPath.Count);
                int currentSimilarity = 0;
                for (int leftNodeIndex = leftPath.Count - 1, rightNodeIndex = rightPath.Count - 1; leftNodeIndex >= leftPath.Count - minLength; leftNodeIndex--, rightNodeIndex--)
                {
                    var leftNode = leftPath[leftNodeIndex];
                    var rightNode = rightPath[rightNodeIndex];
                    if (leftNode == rightNode)
                        currentSimilarity++;
                    else
                        break;
                }
                if (currentSimilarity == 0)
                    return new int[0];
                result[pathIndex] = currentSimilarity;
            }
            return result;
        }

        public int[] GetCompoundRightSideSimilarity(IEnumerable<Tuple<int[], PredictionTree>> others)
        {
            int[] result = new int[0];
            foreach (var other in others)
            {
                var currentResult = GetRightSideSimilarity(other);
                if (currentResult.Length == 0)
                    return currentResult;
                else if (result.Length == 0)
                {
                    result = currentResult;
                    continue;
                }
                else
                {
                    for (int index = 0; index < result.Length; index++)
                        if ((result[index] = Math.Min(result[index], currentResult[index])) == 0)
                            return new int[0];
                }
            }
            return result;
        }

        internal static Dictionary<int, List<Tuple<Tuple<int[], PredictionTree>[], int[]>>> GetCompoundRightSideSimilarities(IEnumerable<Tuple<int[], PredictionTree>> series)
        {
            var currentBreakdowns = (from s in series
                                     let pathSet = s.Item2
                                     group s by s.Item2.Count).ToDictionary(k => k.Key, v => v.ToArray());
            Dictionary<int, List<Tuple<Tuple<int[], PredictionTree>[], int[]>>> results = new Dictionary<int, List<Tuple<Tuple<int[], PredictionTree>[], int[]>>>();
            foreach (var count in currentBreakdowns.Keys)
            {
                List<Tuple<Tuple<int[], PredictionTree>[], int[]>> currentResults;
                if (!results.TryGetValue(count, out currentResults))
                    results.Add(count, currentResults = new List<Tuple<Tuple<int[], PredictionTree>[], int[]>>());
                Stack<Tuple<int[], PredictionTree>> toMatch = new Stack<Tuple<int[], PredictionTree>>(currentBreakdowns[count]);
                Stack<Tuple<int[], PredictionTree>> nextMatch = new Stack<Tuple<int[], PredictionTree>>();
                while (toMatch.Count > 0)
                {
                    var currentTarget = toMatch.Pop();
                    var currentMatches = new List<Tuple<int[], PredictionTree>>() { currentTarget };
                    while (toMatch.Count > 0)
                    {
                        var nextTarget = toMatch.Pop();
                        var currentComparison = currentTarget.Item2.GetRightSideSimilarity(nextTarget);
                        if (currentComparison.Length == 0)
                            nextMatch.Push(nextTarget);
                        else
                            currentMatches.Add(nextTarget);
                    }
                    currentResults.Add(Tuple.Create(currentMatches.ToArray(), currentTarget.Item2.GetCompoundRightSideSimilarity(currentMatches)));

                    if (nextMatch.Count > 0)
                    {
                        toMatch = nextMatch;
                        nextMatch = new Stack<Tuple<int[], PredictionTree>>();
                    }
                }
            }
            return results;
        }

        private bool HasInnerCommonSymbol(IList<PredictionTreeBranch> series, ref PredictionTreeLeaf commonSymbol)
        {
            var firstBranch = series[0];
            if (firstBranch.Depth == firstBranch.MinDepth)
                return false;
            bool performLeftRecursiveCheck = false;
            if (this.Count % 2 == 0)
                for (int lrCheckIndex = 0; lrCheckIndex < firstBranch.Count; lrCheckIndex++)
                    if (firstBranch[lrCheckIndex].RootLeaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None)
                    {
                        performLeftRecursiveCheck = true;
                        break;
                    }
            if (!performLeftRecursiveCheck)
                return ProcessStandardReductionCheck(series, ref commonSymbol, firstBranch);
            else
                return ProcessLeftRecursiveReductionCheck(series, ref commonSymbol, firstBranch) ||
                       ProcessStandardReductionCheck(series, ref commonSymbol, firstBranch);
        }

        private bool ProcessLeftRecursiveReductionCheck(IList<PredictionTreeBranch> series, ref PredictionTreeLeaf commonSymbol, PredictionTreeBranch firstPath)
        {
            List<PredictionTreeBranch> finalSet = null;
            PredictionTreeLeaf resultSymbol = null;

            /* This is a very specific case for handling left-recursive rules which might otherwise recurse infinitely.
             * The basic principle here is we search from the right-side back 
             * */
            for (int rootNodeIndex = Math.Max(firstPath.MinDepth, 1); rootNodeIndex <= firstPath.Depth; rootNodeIndex++)
            {
                var currentRootPaths = new PredictionTreeBranch[series.Count / 2];
                var currentRecursedBranches = new PredictionTreeBranch[series.Count / 2];
                var firstPathCurrentNode = firstPath[rootNodeIndex];
                var currentRootLeaf = firstPath[rootNodeIndex].RootLeaf;
                if (currentRootLeaf.Veins.LeftRecursionType == ProductionRuleLeftRecursionType.None)
                    continue;
                bool currentLeafHasMatch = true;
                int currentRootBranchIndex = 0;
                int currentRecursedPathIndex = 0;
                for (int pathIndex = 0; pathIndex < series.Count; pathIndex++)
                {
                    var currentBranch = series[pathIndex];
                    int lastRecursedBranchIndex = -1;
                    int lastRootBranchIndex = -1;
                    for (int nodeIndex = currentBranch.Depth; nodeIndex > 0; nodeIndex--)
                    {
                        var currentNode = currentBranch[nodeIndex];
                        if (currentNode.RootLeaf != currentRootLeaf)
                            continue;
                        for (int scanIndex = nodeIndex - 1; scanIndex >= 0; scanIndex--)
                        {
                            /* Peek backwards to see what's in the callstack, if all of the nodes just before the one we're at
                             * are in their initial state, and we also hit the root-node of the currentNode then we've got a
                             * left-recursive cycle on our hands. */
                            var scanNode = currentBranch[scanIndex];
                            if (scanNode.RootLeaf == scanNode && scanNode == currentRootLeaf)
                            {
                                lastRecursedBranchIndex = nodeIndex;
                                break;
                            }
                        }
                        if (lastRecursedBranchIndex == -1 && currentNode == firstPathCurrentNode)
                            lastRootBranchIndex = nodeIndex;
                    }
                    if (lastRootBranchIndex == -1 &&
                        lastRecursedBranchIndex == -1)
                    {
                        currentLeafHasMatch = false;
                        break;
                    }
                    /* The left recursion detection works by matching pairs of left-recursive path and the root path that brought about the left recursive cycle
                     * So in general we will have (ignoring deviation tracking):
                     * A::B::C(N)
                     * A::B::C::A::B::C(N)
                     * There's likely multiple sets of these in complex grammars, so we match up all sets and there should be equal parts on each
                     * side.  The only paths we end up being interested in are the A::B::C(N) portions, as the variants are just repetitions of
                     * the same, they add no semantic value to the prediction process.
                     */
                    if (lastRecursedBranchIndex != -1)
                    {
                        if (currentRecursedPathIndex > currentRecursedBranches.Length - 1)
                        {
                            currentLeafHasMatch = false;
                            break;
                        }
                        currentRecursedBranches[currentRecursedPathIndex++] = currentBranch.DepthAt(lastRecursedBranchIndex, false);
                    } 
                    else if (lastRootBranchIndex != -1)
                    {
                        if (currentRootBranchIndex > currentRootPaths.Length - 1)
                        {
                            currentLeafHasMatch = false;
                            break;
                        }
                        currentRootPaths[currentRootBranchIndex++] = currentBranch.DepthAt(lastRootBranchIndex, false);
                    }
                }
                if (!currentLeafHasMatch)
                    continue;
                if (currentRootBranchIndex == currentRecursedPathIndex && currentRecursedPathIndex == currentRootPaths.Length)
                {
                    var companions =
                        GetCompanionSets(currentRootPaths.Concat(currentRecursedBranches));
                    if (companions.Count >= currentRootPaths.Length)
                    {
                        if (companions.Count > currentRootPaths.Length)
                        {

                        }
                        finalSet = companions;
                        resultSymbol = firstPathCurrentNode;
                        break;
                    }
                    else
                    {

                    }
                }
            }
            if (finalSet != null)
            {
                /* Check if we can reduce _further_ due to condensing the final set into the non-left recursive variety.  This will simplify the resultant machines a lot. */
                if (ProcessStandardReductionCheck(finalSet, ref commonSymbol, finalSet[0]))
                    return true;
                ReduceSet(finalSet);
                commonSymbol = resultSymbol;
                this._isLeftRecursiveReduction = true;
                return true;
            }
            return false;
        }

        private bool ProcessStandardReductionCheck(IList<PredictionTreeBranch> series, ref PredictionTreeLeaf commonSymbol, PredictionTreeBranch firstPath)
        {
            int[] matchDepths = null;
            PredictionTreeLeaf matchSymbol = null;
            bool result = false;
            for (int nodeIndex = firstPath.MinDepth + 1; nodeIndex <= firstPath.Depth; nodeIndex++)
            {
                int[] newDepths = new int[series.Count];
                newDepths[0] = nodeIndex;
                bool allMatched = series.Count > 1;
                var currentFirstNode = firstPath[nodeIndex];
                /* *
                 * Project backwards first, always try to reduce the *most*.
                 * */
                for (int pathIndex = 1; pathIndex < series.Count; pathIndex++)
                {
                    var currentPath = series[pathIndex];
                    if (currentPath.Depth == currentPath.MinDepth)
                        return false;
                    bool match = false;
                    for (int subNodeIndex = matchDepths == null ? currentPath.Depth : (matchDepths[pathIndex] - 1); subNodeIndex > currentPath.MinDepth; subNodeIndex--)
                    {
                        var currentNode = currentPath[subNodeIndex];
                        if (currentNode == currentFirstNode)
                        {
                            newDepths[pathIndex] = subNodeIndex;
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        allMatched = false;
                        break;
                    }
                }
                if (allMatched)
                {
                    int pathIndex = 0;
                    int[] deviations = new int[series.Count];
                    for (pathIndex = 0; pathIndex < series.Count; pathIndex++)
                    {
                        var currentPath = series[pathIndex];
                        for (int index = currentPath.Depth; index > newDepths[pathIndex]; index--)
                        {
                            deviations[pathIndex] += currentPath.GetDeviationAt(index);
                        }
                    }
                    //deviation = deviations[0];
                    //if (!deviations.Skip(1).All(k => k == deviation))
                    //{
                    //    /* *
                    //     * If the look-ahead shift varies, we have a non-deterministic situation
                    //     * which makes it a non-candidate for this solution.
                    //     * */
                    //    continue;
                    //}
                    matchSymbol = currentFirstNode;
                    matchDepths = newDepths;
                    result = true;
                    break;
                }
            }
            if (matchSymbol != null)
            {
                //if (matchSymbol.RootLeaf.Count == 0 && this.Count < 4)
                //{
                //    /* If the rule has no direct other rules, inlining it might make the most sense. */
                //    this._isBadReductionCandidate = true;
                //    return false;
                //}
                int deviation = 0;
                int pathIndex = 0;
                int[] deviations = new int[series.Count];
                pathIndex = 0;
                deviation = deviations[0];
                if (deviation > 0)
                    reductionLookAheadDrop = deviation;

                var matchedPaths = (from path in series
                                    let queryPathIndex = pathIndex++
                                    select path.DepthAt(matchDepths[queryPathIndex], false)).ToList();

                ReduceSet(matchedPaths);
                commonSymbol = matchSymbol;

            }
            return result;
        }

        private void ReduceSet(List<PredictionTreeBranch> basePaths)
        {
            if (_hasBeenReducedAlready)
                return;
            _hasBeenReducedAlready = true;
            lock (this.baseList)
            {
                if (this.baseList.Count != this.Count)
                    return;
                this._hashCode = null;
                this.baseList.Clear();
                var basePathsCopy = basePaths.ToList();
                for (int i = 0; i < basePathsCopy.Count; i++)
                {
                    var basePath = basePathsCopy[i];
                    this.AddImpl(basePath);
                }
            }
        }

        private bool GetHasCommonSymbol(List<PredictionTreeBranch> series, ref PredictionTreeLeaf commonSymbol)
        {
            PredictionTreeLeaf commonSymbol2 = commonSymbol;
            if (series.Count <= 1 || !series.All(k => k.Depth > k.MinDepth))
                return false;
        //RecheckCommon:
            if (this.HasInnerCommonSymbol(series, ref commonSymbol2))
                if (commonSymbol2 != commonSymbol)
                {
                    commonSymbol = commonSymbol2;
                    //goto RecheckCommon;
                }
            return commonSymbol != null;
        }

        private bool GetIsLocalTransition()
        {
            if (this.Count == 1)
            {
                /* *
                 * Change March 10, 2015:
                 * *
                 * Previously just Depth = 0 was checked.
                 * *
                 * It's necessary to check that the current path is targeting the
                 * initial rule, if it isn't the actual action exists above the current
                 * rule and no consumption should occur.
                 * */
                if (this.First.projectionType == ProductionRuleProjectionType.FollowAmbiguity)
                    return this[0].CurrentNode.Rule == this.Root.Rule;
                else
                    return this[0].Depth == 0;
            }
            else if (this.Count == 0)
                return false;
            var first = this[0];
            if (first.Depth != 0)
                return false;
            return this.Skip(1).All(k => k.Depth == 0 && k.CurrentNode == first.CurrentNode);
        }

        public PredictionTreeLeaf Root { get; private set; }

        private PredictionTree _previous;
        public PredictionTree Previous
        {
            get
            {
                return this._previous;
            }
            internal set
            {
                this._previous = value;
            }
        }

        public IControlledDictionary<GrammarVocabulary, PredictionTree> LookAhead
        {
            get
            {
                if (this.lookAhead == null)
                    this.lookAhead = new ControlledDictionary<GrammarVocabulary, PredictionTree>();
                return this.lookAhead;
            }
        }

        public IEnumerable<GrammarVocabulary> GetCurrentLookAheadStream()
        {
            return this.GetCurrentLookAheadStreamInternal().Reverse();
        }

        private IEnumerable<GrammarVocabulary> GetCurrentLookAheadStreamInternal()
        {
            var current = this;
            while (current != null)
            {
                yield return current.Discriminator;
                current = (PredictionTree)current.Previous;
            }
            yield break;
        }

        private PredictionTree[] pathSetCache;

        internal PredictionTree[] GetCurrentPathSets()
        {
            if (pathSetCache == null)
            {
                PredictionTree[] result;
                PredictionTree current = this;
                int count = 0;
                while (current != null)
                {
                    count++;
                    current = (PredictionTree)current.Previous;
                }
                result = new PredictionTree[count];
                count = 0;
                current = this;
                while (current != null)
                {
                    result[result.Length - ++count] = current;
                    current = (PredictionTree)current.Previous;
                }
                pathSetCache = result;
            }
            return pathSetCache;
        }

        public int LookAheadDepth
        {
            get
            {
                int result = 0;
                var current = this;
                while (current != null)
                {
                    current = (PredictionTree)current.Previous;
                    result++;
                }
                return result;
            }
        }

        public bool Equals(PredictionTree other)
        {
            return other != null && 
                this.Count == other.Count && 
                PathEquivalence(other) && 
                other.Discriminator.Equals(this.Discriminator) &&
                this.ReductionType == other.ReductionType &&
                this.LookAheadDepth == other.LookAheadDepth &&
                (this.Previous == null && other.Previous == null || this.First.Discriminator.Equals(other.First.Discriminator));
        }

        private bool PathEquivalence(PredictionTree other)
        {
            if (this.Count != other.Count)
                return false;
            if (this.GetHashCode() != other.GetHashCode())
                return false;
            for (int thisIndex = 0; thisIndex < this.Count; thisIndex++)
            {
                var thisItem = this.baseList[thisIndex];
                bool found=false;
                for (int thatIndex = 0; thatIndex < this.Count; thatIndex++)
                    if (thisItem.Equals(other.baseList[thatIndex]))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return false;
            }
            return true;
        }

        private bool PathEquivalence(List<PredictionTreeBranch> companions)
        {
            if (this.Count != companions.Count)
                return false;
            if (this.GetHashCode() != companions.GetHashCode())
                return false;

            for (int thisIndex = 0; thisIndex < this.Count; thisIndex++)
            {
                var thisItem = this.baseList[thisIndex];
                bool found = false;
                for (int thatIndex = 0; thatIndex < this.Count; thatIndex++)
                    if (thisItem.Equals(companions[thatIndex]))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (this._hashCode == null)
            {
                int result = ~this.Count;
                foreach (var element in this)
                    result ^= element.GetHashCode();
                result ^= this.Discriminator.GetHashCode();
                this._hashCode = result;
            }
            return this._hashCode.Value;
        }

        protected override void AddImpl(PredictionTreeBranch expression)
        {
            if (!base.baseList.Contains(expression))
                base.AddImpl(expression);
        }

        public bool HasAdvanced { get { return this.advanceCalled; } }

        bool advanceCalled = false;
        private object locker = new object();
        private bool shouldAdvanceResult = false;
        private List<PredictionTree> advanceSet = new List<PredictionTree>();
        public List<PredictionTree> GetAndClearAdvanceSet()
        {
            if (advanceSet == null)
                return new List<PredictionTree>();
            var result = advanceSet;
            advanceSet = new List<PredictionTree>();
            return result;
        }
        internal bool Advance(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
            bool buildAdvanceSet = !this.advanceCalled;



            var result = AdvanceInternal(fullSeries, ruleVocabulary, compilationErrors, grammarSymbols);
            if (result && buildAdvanceSet)
            {
                this.advanceSet = this.lookAhead.Values.Cast<PredictionTree>().ToList();
                //if (this.advanceSet!=null && this.advanceSet.Count > 0)
                //    this.advanceSet = this.lookAhead.Values.Cast<PredictionTree>().Where(k => !k._isCopy).Concat(this.advanceSet).ToList();
                //else
                //    this.advanceSet = this.lookAhead.Values.Cast<PredictionTree>().Where(k=>!k._isCopy).ToList();
            }
            else if (this.advanceSet == null)
                this.advanceSet = new List<PredictionTree>();
            this._expandedFormHandler = new Lazy<string>(() => this.GenerateDebugString(fullSeries, ruleVocabulary, grammarSymbols));
            return result;
        }

        internal bool AdvanceInternal(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
            lock (locker)
            {
                if (this.lookAhead == null)
                    this.lookAhead = new ControlledDictionary<GrammarVocabulary, PredictionTree>();
                if (advanceCalled)
                    return false;
                advanceCalled = true;
            }
            if (this.lookAhead.Count > 0)
                return false;

            this.CheckReductionState(fullSeries, ruleVocabulary);
            switch (this.ReductionType)
            {
                case LookAheadReductionType.None:
                    {

                        if (this.Count > 1) //|| this.Discriminator.Breakdown.Rules.Count > 0)
                        {
                            if (this.AreAnyBeyondLookupLimit())
                            {

                            }
                            else
                            {
                                var replicationPoint = GetReplicationPoint();
                                bool allAreEquivalent;
                                PredictionTreeBranch firstPath;
                                AllCurrentNodesAreEquivalent(out firstPath, out allAreEquivalent);
                                if (replicationPoint == null)
                                {
#if ShowStateBreakdown
                            Console.WriteLine("{0} - {1} - Ambiguous state, affected paths: {5}, steps: {3}\r\n\t{4}", this.Discriminator, this.Root, string.Empty, currentSets.Length, this.CurrentLookAheadBranches, this.Count);
                            PrintPaths();
#endif
                                    if (allAreEquivalent && this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                    {
                                        PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
                                        if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                            Debug.Assert(this.ProjectedRootTarget != null);
                                        return shouldAdvanceResult = false;
                                    }
                                    else
                                        ProcessNormalTransition(fullSeries, ruleVocabulary, compilationErrors, grammarSymbols);
                                }
                                else
                                    SetIsReplicationPoint(fullSeries, ruleVocabulary, replicationPoint);
                            }
                        }
                        else
                        {
#if ShowStateBreakdown
                        Console.WriteLine("{0} - {1} - Final Look-Ahead encountered, took {4} steps: {2}\r\n\t{3}", this.Discriminator, this.Root, this[0], this.CurrentLookAheadBranches, currentSets.Length);
                        PrintPaths();
#endif

                            HandleTransition(fullSeries, ruleVocabulary, compilationErrors);
                            return shouldAdvanceResult = false;
                        }
                        break;
                    }
                case LookAheadReductionType.LeftRecursive:
                    {
                        /* This denotes a point of replication which requires its own disambiguation mechanism. */
                        //Be sure to add logic so directly left recursive rules have sufficient context to parse the non left recursive variant.
                        {
                            PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
                            if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                Debug.Assert(this.ProjectedRootTarget != null);
                            return shouldAdvanceResult = false;
                        }
                    }
                    //break;
                case LookAheadReductionType.CommonForwardSymbol:
                    {
                        PredictionTreeBranch firstPath;

                        bool allAreEquivalent;
                        AllCurrentNodesAreEquivalent(out firstPath, out allAreEquivalent);
                        /* The transition that was necessary to get to this point isn't relevant at this point,
                         * Due to the reduction we're now looking at what the paths transition into after
                         * that commonSymbol. */
                        //if (this.Count > 1 && this.Any(k => k.CurrentNode.RootLeaf.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None))
                        //{
                        //    if (!HandleLeftRecursionCheck(fullSeries, ruleVocabulary, compilationErrors, this))
                        //        return shouldAdvanceResult = false;
                        //}


                        if (allAreEquivalent && this.commonSymbol.RootLeaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None)
                        {

                            /* Left recursive Tokens+1 check. If the rule is left-recursive with the current rule,
                             * then we need to take one step further to ensure it is indeed the rule we suspect. */
                            if (this.commonSymbol.RootLeaf.Veins.IsAlwaysLeftRecursive && this.baseList.Any(k => k.Skip(k.Depth + 1).Contains(this.Root)))
                            {
                                var currentSet = this.baseList.ToArray();
                                this.baseList = new List<PredictionTreeBranch>(this.originalSet);

                                this._hasBeenReducedAlready = false;
                                this._reductionType = LookAheadReductionType.Uncalculated;
                                this._isBadReductionCandidate = true;
                                this.CheckReductionState(fullSeries, ruleVocabulary);
                                ProcessNormalTransition(fullSeries, ruleVocabulary, compilationErrors, grammarSymbols);
                                var trimmedLookAhead =
                                    (from laKey in this.LookAhead.Keys
                                     let laTarget = (PredictionTree)this.LookAhead[laKey]
                                     from laPath in laTarget
                                     let laOrigins = laTarget.origins[laPath]
                                     from laOrigin in laOrigins
                                     where laPath.CurrentNode.RootLeaf == commonSymbol.RootLeaf
                                     where currentSet.Any(current => laOrigin.SequenceEqual(current))
                                     select new { Key = laKey, Value = laTarget }).Distinct().ToDictionary(k => k.Key, v => v.Value);
                                if (trimmedLookAhead.Count > 0)
                                {
                                    this.commonSymbol = null;
                                    this.lookAhead._Clear();
                                    foreach (var key in trimmedLookAhead.Keys)
                                        this.lookAhead._Add(key, trimmedLookAhead[key]);
                                    return shouldAdvanceResult = true;
                                }
                                else
                                {
                                    this.baseList = currentSet.ToList();
                                    this._hasBeenReducedAlready = true;
                                    this.lookAhead = null;
                                    this._isBadReductionCandidate = false;
                                    this._reductionType = LookAheadReductionType.CommonForwardSymbol;
                                }
                            }
                        }
                        if (this.Count > 1 && !allAreEquivalent)
                        {
                            var transitionGrammar = ruleVocabulary[this.commonSymbol.Rule];

                            var replicationPoint = GetReplicationPoint();
                            if (replicationPoint == null)
                            {
                                BuildLookAheadWith(fullSeries, ruleVocabulary, transitionGrammar, true, grammarSymbols, compilationErrors, true);
                                /* After reaching this point, we need to evaluate the possibility that the rule
                                 * being reduced is a bad candidate for reduction. If any of the edge nodes for the
                                 * rule being reduced overlap with the prediction, we have a bad candidate, not because
                                 * it's impossible to generate a machine for, but because it has a great chance for yielding
                                 * infinite machines. */
                                if (!LeftRecursiveCheck(fullSeries, ruleVocabulary, compilationErrors))
                                  return shouldAdvanceResult = false;
                                this.ProcessCommonSymbol(fullSeries, ruleVocabulary);
                                if (this.reductionContext == null)
                                    this.reductionContext = this.Root.RootLeaf.SetReductionOn(transitionGrammar, 0, fullSeries);


#if ShowStateBreakdown
                            Console.WriteLine("{0} - {1} - Common Forward Symbol - All paths point to common symbol: `{2}`, Paths Affected: {5}, Steps Taken: {4}\r\n\t{3}", this.Discriminator, this.Root, this.commonSymbol.Name, this.CurrentLookAheadBranches, currentSets.Length, this.Count);
                            PrintPaths();
#endif
                            }
                            else
                                SetIsReplicationPoint(fullSeries, ruleVocabulary, replicationPoint);
                        }
                        else
                        {
#if ShowStateBreakdown
                        Console.WriteLine("{0} - {1} - Paths reduced to common `{2}`, Steps Taken: `{3}`\r\n\t{4}", this.Discriminator, this.Root, this.commonSymbol.Name, this.CurrentLookAheadBranches, currentSets.Length);
                        PrintPaths();
#endif
                            PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
                            if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                Debug.Assert(this.ProjectedRootTarget != null);
                            return shouldAdvanceResult = false;
                        }
                        break;
                    }
                case LookAheadReductionType.LocalTransition:
                    {

                        if (this.Discriminator.IsEmpty)
                            this.ForceEdgeState = true;
                        PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
                        if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity && !this.Discriminator.IsEmpty && this.ProjectedRootTarget == null)
                            Debug.Assert(this.ProjectedRootTarget != null);
#if ShowStateBreakdown
                    Console.WriteLine("{0} - {1} - Local Transition, Steps Taken: `{2}`\r\n\t{3}", this.Discriminator, this.Root, currentSets.Length, this.CurrentLookAheadBranches);
                    PrintPaths();
#endif
                        break;
                    }
            }
            return shouldAdvanceResult = true;
        }

        private bool AreAnyBeyondLookupLimit()
        {
            //if (this.Rule.LookaheadTokenLimit != null && this.LookAheadDepth >= this.Rule.LookaheadTokenLimit.Value)
            if (this.Rule.LookaheadTokenLimit != null && this.LookAheadDepth >= this.Rule.LookaheadTokenLimit.Value)
            {
                return true;
            }
            foreach (var item in this)
            {
                if (item.Count == item.MinDepth + 1)
                    continue;
                var deviationDepth = item.GetDeviationAt(item.MinDepth + 1);
                var nextElement = item.DepthAt(item.MinDepth + 1, false);
                var currentNode = item.CurrentNode;
                if (currentNode.Rule.LookaheadTokenLimit != null && deviationDepth >= currentNode.Rule.LookaheadTokenLimit)
                    return true;
            }
            return false;
        }

        private void ProcessNormalTransition(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
            BuildLookAheadWith(fullSeries, ruleVocabulary, this.Discriminator, false, grammarSymbols, compilationErrors, false);
            if (this.LookAhead.Count == 0 || this.LookAhead.Count == 1 && this.LookAhead.Keys[0].IsEmpty)
            {
                /* *
                 * An ambiguous construct is noted, regardless of the intent, 
                 * this will yield an ambiguity due to two rules requiring the same
                 * set of tokens.
                 * */
                if (!this.Discriminator.IsEmpty || this.All(k => k.CurrentNode.Veins.DFAOriginState.IsEdge))
                {
                    //AllCurrentNodesAreEquivalent(out firstPath, out allAreEquivalent);
                    //if (!allAreEquivalent)
                    //{
                    if (this.Root.RootLeaf.Veins.LeftRecursionType == ProductionRuleLeftRecursionType.None && this.First._derivedFrom != PredictionDerivedFrom.LookAhead_FollowPrediction)
                        DenoteAmbiguity(compilationErrors);
                    else
                    {
                        var projectedOrigin = this.ProjectOrigin(fullSeries, ruleVocabulary);
                        var firstOriginPreviousPath = projectedOrigin[0][0].PreviousPath;
                        var firstOriginPreviousPathMinNode = firstOriginPreviousPath[firstOriginPreviousPath.MinDepth];
                        if (firstOriginPreviousPathMinNode == this.Root)
                        {
                            var recursiveVariants =
                                projectedOrigin[0].Where(k => k.PreviousPath.Skip(1).Any(j => j == firstOriginPreviousPathMinNode)).ToArray();
                            var nonRecursiveVariants =
                                projectedOrigin[0].Except(recursiveVariants).ToArray();

                            if (nonRecursiveVariants.Length == 1)
                            {
                                var pathToCheck = nonRecursiveVariants[0].PreviousPath;
                                var nodesToMatch = pathToCheck.Skip(pathToCheck.MinDepth + 1).ToArray();
                                
                                if (recursiveVariants.All(k=> k.PreviousPath.Reverse().Take(nodesToMatch.Length).Reverse().SequenceEqual(nodesToMatch)))
                                {
                                    HandleTransition(fullSeries, ruleVocabulary, compilationErrors);
                                    return;
                                }
                            }
                        }
                        DenoteAmbiguity(compilationErrors);
                    }
                    //}
                }
            }
        }

        private void DenoteAmbiguity(ICompilerErrorCollection compilationErrors)
        {
            lock (compilationErrors)
                compilationErrors.ModelError<string>(OilexerGrammarCore.CompilerErrors.AmbiguousPathDetected, this.ExpandedForm.First());
            this.IsAmbiguousState = true;
        }

        private bool HandleLeftRecursionCheck(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, PredictionTree lrTarget)
        {
            var companionSets = GetCompanionSets(lrTarget);
            if ((double)companionSets.Count == ((double)lrTarget.Count) / 2)
            {
                if (companionSets.Count == 1)
                {
                    lrTarget.HandleTransition(fullSeries, ruleVocabulary, compilationErrors);
                    return lrTarget.shouldAdvanceResult = false;
                }
            }
            return true;
        }

        private static List<PredictionTreeBranch> GetCompanionSets(IEnumerable<PredictionTreeBranch> lrTarget)
        {
            var companionSets =
                (from p in lrTarget
                 where p != null
                 from p2 in lrTarget
                 where p2 != null
                 where p2.Depth < p.Depth
                 where p2 != p &&
                     p2.CurrentNode == p.CurrentNode &&
                     p.Take(p2.Depth).SequenceEqual(p2.Take(p2.Depth)) &&
                     p[p2.Depth] == p.CurrentNode.RootLeaf
                 where p.Skip(p2.Depth).Take(p.Depth - p2.Depth).All(k => k.RootLeaf == k) &&
                       p.Skip(p.Depth).SequenceEqual(p2.Skip(p2.Depth))
                 select p2).ToList();
            return companionSets;
        }

        private bool LeftRecursiveCheck(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors)
        {
            var firstItem = this.FirstOrDefault();

            if (this._reductionType == LookAheadReductionType.CommonForwardSymbol)
            {
                //Transition end check.
                if (this.LookAhead.ContainsKey(GrammarVocabulary.NullInst))
                {
                    var currentNode = firstItem.FirstOrDefault();
                    if (currentNode != null && this.Root.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None)
                    {
                        var epsTrans = this.LookAhead[GrammarVocabulary.NullInst];

                        if (epsTrans[0].Depth == 0 && epsTrans[0][0].Rule == this.Rule)
                        {
                            var firstRule = this.commonSymbol.Rule;
                            if (firstRule != null)
                            {
                                var ruleVocab = ruleVocabulary[firstRule];
                                if (this.Root.Veins.DFAOriginState.OutTransitions.ContainsKey(ruleVocab))
                                {
                                    int devLevel = this[0].GetCurrentDeviation() - (ReductionCount - 1);
                                    var backRef = this.GetBack(devLevel);
                                    /* If, in looking back on the reduction, we're at the beginning, we've
                                     * found our cause of concern. */
                                    if (backRef == this.First)
                                    {
                                        this.ProjectedRootTransition = ruleVocab;
                                        this.ProjectedRootTarget = fullSeries[this.Root.Veins.DFAOriginState.OutTransitions[ruleVocab]];
                                        this.lookAhead = null;
                                        this._isBadReductionCandidate = true;
                                        this._reductionType = LookAheadReductionType.Uncalculated;
                                        this.CheckReductionState(fullSeries, ruleVocabulary);
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //if (this.Rule.Name != "InvocationExpression")
                    //    return true;
                    /*Left recursive 'explosion' state*/
                    var pathsByDepth =
                        (from path in this
                         group path by path.Depth).ToDictionary(k=>k.Key, v=>v.ToList());
                    var maxMinDepthOfOriginalSet = this.originalSet.OrderByDescending(k => k.MinDepth).First().MinDepth;
                    if (pathsByDepth.All(k => k.Key > maxMinDepthOfOriginalSet))
                    {
                        var mindepth = pathsByDepth.Keys.Min();
                        int stepCount = 0;
                        while (stepCount < mindepth)
                        {
                            PredictionTreeLeaf currentNode = null;
                            foreach (var depth in pathsByDepth.Keys)
                            {
                                var currentSet = pathsByDepth[depth];
                                //if (depth - stepCount < maxMinDepthOfOriginalSet)
                                    //return true;
                                var newSet = (from path in currentSet
                                              let prevPath = path.DecreaseDepth(false)
                                              select prevPath).ToArray();
                                var firstOfNewSet = newSet.First();
                                if (!newSet.All(k => k.CurrentNode == firstOfNewSet.CurrentNode))
                                    return true;
                                if (currentNode == null)
                                    currentNode = firstOfNewSet.CurrentNode;
                                if (firstOfNewSet.CurrentNode != currentNode)
                                    return true;
                                currentSet.Clear();
                                currentSet.AddRange(newSet);
                            }
                            stepCount++;
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void HandleTransition(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors)
        {
            var first = this[0];
            PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
            if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
            {
                Debug.Assert(this.ProjectedRootTarget != null, "Cannot determine target!");
            }
        }

        private void AllCurrentNodesAreEquivalent(out PredictionTreeBranch firstPath, out bool allAreEquivalent)
        {

            //if (this._isLeftRecursiveReduction)
            //{
            //    firstPath = this.First();
            //    var initialDepth = firstPath.Depth;
            //    var initialNodes = firstPath.Take(firstPath.Depth + 1);
            //    /* *
            //     * Different paths lead here; however, if the roots of all 
            //     * current paths lead to the same forward symbol,
            //     * then this is a special case that doesn't require further
            //     * look-ahead.
            //     * */
            //    allAreEquivalent = this.All(k => k.Depth > 0 && k.Depth == initialDepth && k.Take(k.Depth + 1).SequenceEqual(initialNodes));
            //}
            //else
            //{
                firstPath = this.First();
                if (firstPath.Depth > firstPath.MinDepth)
                {
                    var node1 = firstPath[firstPath.MinDepth];
                    var node2 = firstPath[firstPath.MinDepth + 1];
                    bool firstTwoMatch = true;
                    for (int pathIndex = firstPath.MinDepth; pathIndex < this.Count; pathIndex++)
                    {
                        var currentPath = this[pathIndex];
                        if (currentPath.Count <= firstPath.MinDepth)
                        {
                            firstTwoMatch = false;
                            break;
                        }
                        if (currentPath[firstPath.MinDepth] != node1 || currentPath[firstPath.MinDepth + 1] != node2)
                        {
                            firstTwoMatch = false;
                            break;
                        }
                    }
                    if (firstTwoMatch)
                    {
                        allAreEquivalent = true;
                        return;
                    }
                }
            //}
            allAreEquivalent = false;
        }

        private PredictionTree GetReplicationPoint()
        {
            var current = this.Previous as PredictionTree;
            while (current != null)
            {
                if (this._reductionType != current._reductionType ||
                    this.Count != current.Count ||
                    !this.Discriminator.Equals(current.Discriminator) ||
                    current.Previous == null)
                {
                    goto nextStep;
                }
                if (this.PathEquivalence(current))
                    return current;
            nextStep:
                current = (PredictionTree)current.Previous;
            }
            return null;
            //var currentSets = this.GetCurrentPathSets();

            //var replicationPoint = currentSets.FirstOrDefault(k => !Object.ReferenceEquals(this, k) && k.GetHashCode() == this.GetHashCode() && k.Equivalent(this));
            //return replicationPoint;
        }

        private PredictionTree GetReplicationPoint(List<PredictionTreeBranch> paths)
        {
            var current = this.Previous as PredictionTree;
            while (current != null)
            {
                if (this._reductionType != current._reductionType ||
                    this.Count != current.Count ||
                    !this.Discriminator.Equals(current.Discriminator) ||
                    current.Previous == null)
                {
                    goto nextStep;
                }
                if (current.PathEquivalence(paths))
                    return current;
            nextStep:
                current = (PredictionTree)current.Previous;
            }
            return null;
        }

        private void SetIsReplicationPoint(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, PredictionTree replicationPoint)
        {
            this.isRepetitionPoint = true;
            this._reductionType = LookAheadReductionType.Uncalculated;
            this.pointOfRedundancy = replicationPoint;
            this.CheckReductionState(fullSeries, ruleVocabulary);
            this.ReplicationPoint.RegisterRepetition();
            //this.ProcessCommonSymbol(fullSeries, ruleVocabulary);
#if ShowStateBreakdown
            Console.WriteLine("Replication Point Encountered.");
#endif
        }

        internal void RegisterRepetition()
        {
            this.IsRepetitionTarget = true;
        }

#if ShowStateBreakdown
        private void PrintPaths()
        {
            Console.WriteLine("\tPaths:");
            foreach (var path in this)
                Console.WriteLine("\t\t{0}", path);
            Console.WriteLine();
        }
#endif
        private void PredictTransitionVocabulary(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, PredictionTree disambiguationContext = null)
        {
            SyntacticalDFAState originatingState;
            if (this.Previous == null && disambiguationContext == null)
            {
                switch (this.ReductionType)
                {
                    case LookAheadReductionType.None:
                        this.ProjectedRootTransition = ruleVocabulary[this[0][1].RootLeaf.Rule];
                        break;
                    case LookAheadReductionType.CommonForwardSymbol:
                        this.ProjectedRootTransition = ruleVocabulary[this[0][1].RootLeaf.Rule];
                        break;
                    case LookAheadReductionType.LeftRecursive:
                    case LookAheadReductionType.LocalTransition:
                        this.ProjectedRootTransition = this.Discriminator;
                        break;
                }
                if (this.ProjectedRootTransition != null)
                {
                    var firstPath = this[0];
                    var firstNode = firstPath[0];
                    /* *
                     * We do have a root node; however, the issue with 
                     * this lies in that the root node may differ from the
                     * initial path.
                     * *
                     * Why? Empty rules which yield no result.  These
                     * will allow a path set to contain a reference
                     * to an initial starting point other than the root.
                     * *
                     * i.e. 'Hidden' transition, or epsilon transition.
                     * */
                    originatingState = firstNode.Veins.DFAOriginState;
                    SetProjectedRootTarget(fullSeries, originatingState, ruleVocabulary);
                    goto DecisionPoint;
                }
                /* *
                 * ToDo: Note Possible Projection Failure.
                 * */
                goto DecisionPoint;
            }
            PredictionTreeDestinationAvenues origin;
            //PredictionTree[] series;

            //List<Tuple<PredictionTreeBranch, PredictionTreeBranch>>[] resultTransitionSet;
            ProjectOrigin(fullSeries, ruleVocabulary, out origin);
            if (disambiguationContext != null)
            {
                bool ruleReduction = origin[origin.Count - 1].Tree.ReductionType == LookAheadReductionType.CommonForwardSymbol;
                bool usedRuleContext = origin[origin.Count - 1].Tree.Discriminator.GetTokenVariant().IsEmpty;

                var shiftedFirstSet = (from path in origin[origin.Count - 1]
                                       where path.PreviousPath.Depth >= path.PreviousPath.MinDepth
                                       let transitions = path.PreviousPath.FollowTransition(fullSeries, ruleReduction ? ruleVocabulary[path.PreviousPath.CurrentNode.Rule] : origin[origin.Count - 1].Tree.Discriminator, ruleVocabulary, (usedRuleContext || ruleReduction) && path.PreviousPath.Depth > path.PreviousPath.MinDepth, false, true, false, true)
                                       from target in transitions.Values
                                       from targetPath in target
                                       where disambiguationContext == null ? targetPath.CurrentNode.Veins.DFAOriginState.IsEdge : disambiguationContext.Contains(targetPath)
                                       select path).Distinct().ToList();
                if (shiftedFirstSet.Count > 0)
                    origin.baseList[0] = Trace(shiftedFirstSet, origin, origin[origin.Count - 1].Tree);
            }

            var element = origin[0];

            for (int targetOffset = 0; targetOffset < element.Count; targetOffset++)
            {
                var firstPath = element[targetOffset];
                var firstNode = GetTargetNode(firstPath);
                var fromNode = firstPath.PreviousPath[firstPath.PreviousPath.MinDepth];
                if (firstPath.PreviousPath.Count == 1)
                {
                    if (element.Skip((targetOffset + 1)).All(k => k.PreviousPath[0] == firstNode))
                    {
                        this.ProjectedRootTransition = origin[0].Tree.Discriminator;
                        SetProjectedRootTarget(fullSeries, fromNode.Veins.DFAOriginState, ruleVocabulary, origin);
                        goto DecisionPoint;
                    }
                    else
                    {
                        //?? ToDo: Investtigate why this is here, and what impact it has to have nothing here!
                    }
                }
                else if ((firstPath.PreviousPath.Count == 1 ? 0 : firstPath.PreviousPath.Depth == firstPath.PreviousPath.MinDepth ? 0 : 1) > firstPath.PreviousPath.MinDepth && element.Skip((targetOffset + 1)).All(k => GetTargetNode(k) == firstNode))
                {
                    if (this.ReductionType == LookAheadReductionType.Uncalculated)
                        this.CheckReductionState(fullSeries, ruleVocabulary);
                    switch (this.ReductionType)
                    {
                        case LookAheadReductionType.None:
                            this.ProjectedRootTransition = ruleVocabulary[firstNode.Rule];
                            break;
                        case LookAheadReductionType.CommonForwardSymbol:

                            this.ProjectedRootTransition = ruleVocabulary[firstNode.Rule];
                            break;
                        case LookAheadReductionType.LocalTransition:
                        case LookAheadReductionType.LeftRecursive:
                            if (firstPath.PreviousPath.Depth == 0)
                                this.ProjectedRootTransition = origin[0].Tree.Discriminator;
                            else
                                this.ProjectedRootTransition = ruleVocabulary[firstNode.Rule];
                            break;
                    }
                    SetProjectedRootTarget(fullSeries, fromNode.Veins.DFAOriginState, ruleVocabulary, origin);
                    goto DecisionPoint;
                }
                else if (disambiguationContext != null && disambiguationContext.Previous == this)
                {
                    this.ProjectedRootTarget = disambiguationContext.ProjectedRootTarget;
                    this.ProjectedRootTransition = disambiguationContext.ProjectedRootTransition;
                    if (this.ProjectedRootTarget != null)
                        SetProjectedRootTarget(fullSeries, fromNode.Veins.DFAOriginState, ruleVocabulary, origin);
                    goto DecisionPoint;
                }
                else
                {
                    this.ProjectedRootTransition = origin[0].Tree.Discriminator;
                    SetProjectedRootTarget(fullSeries, fromNode.Veins.DFAOriginState, ruleVocabulary, origin);
                    if (this.ProjectedRootTarget == null)
                        continue;
                    goto DecisionPoint;
                }
            }
            /* *
             * ToDo: Note Possible Projection Failure.
             * */
            return;
        DecisionPoint:
            if (this.ProjectedRootTarget == null)
            {
                //Debug.WriteLine("Null Transition Target");
            }
        }

        private static PredictionTreeLeaf GetTargetNode(PredictionTreeDestinationPoint firstPath)
        {
            int firstNodeTarget = firstPath.PreviousPath.MinDepth + (firstPath.PreviousPath.Count == 1 ? 0 : firstPath.PreviousPath.Depth == firstPath.PreviousPath.MinDepth ? 0 : 1);
            var firstNode = firstPath.PreviousPath[firstNodeTarget];
            return firstNode;
        }

        private PredictionTreeDestinationAvenue Trace(
            List<PredictionTreeDestinationPoint> traceSource, PredictionTreeDestinationAvenues traceTrail, PredictionTree ownerSet)
        {
            var origin = new Stack<PredictionTreeDestinationAvenue>(traceTrail);
            if (origin.Count > 0)
                origin.Pop();
            while (origin.Count > 0)
            {
                var originTracker = origin.Pop();
                traceSource = (from t in traceSource
                               join source in originTracker on t.PreviousPath equals source.CurrentPath
                               select source).Distinct().ToList();
            }
            return new PredictionTreeDestinationAvenue(traceSource) { Tree = ownerSet };
        }

        internal PredictionTreeDestinationAvenues ProjectOrigin(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            //PredictionTree[] series;
            //List<Tuple<PredictionTreeBranch, PredictionTreeBranch>>[] resultTransitionSet;
            PredictionTreeDestinationAvenues origin;
            ProjectOrigin(fullSeries, ruleVocabulary, out origin);
            return origin;
        }

        private void ProjectOrigin(
            Dictionary<SyntacticalDFAState, PredictionTreeLeaf>                 fullSeries,
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary>   ruleVocabulary,
            out PredictionTreeDestinationAvenues                                origin)
        {
            var seriesCopy = this.GetCurrentPathSets().ToList();
            Stack<PredictionTree> series = new Stack<PredictionTree>(seriesCopy);
            //series = this.GetCurrentPathSets();
            Stack<PredictionTree> reverseProjections = new Stack<PredictionTree>(seriesCopy);
            Stack<PredictionTreeDestinationAvenue> reversedPathTransitionDiagram = new Stack<PredictionTreeDestinationAvenue>();

            reversedPathTransitionDiagram.Push(new PredictionTreeDestinationAvenue(
                                                from k in this.originalSet.ToList()
                                                select new PredictionTreeDestinationPoint { CurrentPath = null, PreviousPath = k }) { Tree = series.Pop() });
            var previousSet = this.originalSet.ToList();
            PredictionTree previous = null;
            while (reverseProjections.Count > 0)
            {
                var current = reverseProjections.Pop();
                if (previous != null)
                {
                    bool useRuleTarget = current.ReductionType == LookAheadReductionType.CommonForwardSymbol;
                    /* Trading computational power for simplicity, versus tracking this in detail as we go, we can rebuild it by reversing the process and rewinding the transition stack. */
                    var resultTransitions = new PredictionTreeDestinationAvenue(
                                                (from path in current
                                                 let subset = path.FollowTransition(fullSeries, useRuleTarget ? ruleVocabulary[current.PointOfCommonality.Rule] : current.Discriminator, ruleVocabulary, useRuleTarget, true, true, false)
                                                 from transitionKey in subset.Keys
                                                 let actualPossibleAmbiguousKey = previous.Discriminator.DisambiguateVocabulary()
                                                 let intersection = transitionKey.Intersect(actualPossibleAmbiguousKey)
                                                 where !intersection.IsEmpty || (transitionKey.IsEmpty && actualPossibleAmbiguousKey.IsEmpty)
                                                 let properSubset = subset[transitionKey]
                                                 from properPath in properSubset
                                                 from previousSetPath in previousSet
                                                 where PathsEquivalent(properPath, previousSetPath)
                                                 select new PredictionTreeDestinationPoint { CurrentPath = previousSetPath, PreviousPath = current.originalSet[current.IndexOf(path)] }).Distinct().ToArray())
                                                 {
                                                    Tree = series.Pop()
                                                 };
                    previousSet = (from k in resultTransitions
                                   select k.PreviousPath).ToList();
                    reversedPathTransitionDiagram.Push(resultTransitions);
                }
                previous = current;
            }
            origin = new PredictionTreeDestinationAvenues(reversedPathTransitionDiagram);
        }

        

        private static bool PathsEquivalent(PredictionTreeBranch properPath, PredictionTreeBranch previousPath)
        {
            if (previousPath.Equals(properPath))
                return true;
            if (previousPath.Depth < properPath.Depth && properPath.MinDepth <= previousPath.Depth)
                return properPath.DepthAt(previousPath.Depth, false).Equals(previousPath);
            else if (previousPath.Depth > properPath.Depth && previousPath.MinDepth <= properPath.Depth)
                return previousPath.DepthAt(properPath.Depth, false).Equals(properPath);
            return false;
        }

        private void SetProjectedRootTarget(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, SyntacticalDFAState originatingState, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, PredictionTreeDestinationAvenues incomingTransitions = null)
        {
            var actualTransitionKey = originatingState.OutTransitions.Keys.FirstOrDefault(k => !k.Intersect(this.ProjectedRootTransition.DisambiguateVocabulary()).IsEmpty);
            if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
            {
                if (actualTransitionKey != null)
                {
                    this.ProjectedRootTarget = fullSeries[originatingState.OutTransitions[actualTransitionKey]];
                    Debug.Assert(this.ProjectedRootTarget.RootLeaf == this.Root.RootLeaf);
                }
                else
                {

                }
            }
            else
            {
                var firstFirst = (from firstLevel in this.First.followEpsilonLevels
                                  from level in firstLevel.Value
                                  where level == 0
                                  select firstLevel.Key).ToArray();
                var origNode = fullSeries[originatingState];
                Debug.Assert(firstFirst.All(k => k.Depth > 0));
                /* *
                 * All states which lead into this as a follow should contain
                 * the proper context of what is being matched by reversing
                 * the prediction.  This is a memory-based issue.
                 * */
                var firstIncoming = incomingTransitions[0];
                PredictionTreeBranch targetSequenceMatch = null;
                foreach (var tailInitialPair in firstIncoming)
                {
                    var topLevel = tailInitialPair.PreviousPath;
                    var firstSequenceMatch = firstFirst.FirstOrDefault(firstSequence => firstSequence.SequenceEqual(topLevel));
                    if (firstSequenceMatch != null)
                    {
                        targetSequenceMatch = firstSequenceMatch;
                        break;
                    }
                }
                if (targetSequenceMatch != null)
                {
                    /* *
                     * The follow transition disambiguated through
                     * a look-ahead element from a nested rule called
                     * from the edge state.
                     * */
                    if (targetSequenceMatch.Depth > targetSequenceMatch.MinDepth)
                    {
                        var subSequence = new PredictionTreeBranch(targetSequenceMatch.Take(targetSequenceMatch.MinDepth + 2).ToList(), targetSequenceMatch.MinDepth + 1, false, false);
                        var subSequenceDecrease = subSequence.DecreaseDepth(false);
                        this.ProjectedRootTransition = ruleVocabulary[subSequence.CurrentNode.Rule];
                        actualTransitionKey = subSequenceDecrease.CurrentNode.Veins.DFAOriginState.OutTransitions.Keys.FirstOrDefault(k => !k.Intersect(this.ProjectedRootTransition).IsEmpty);
                        if (actualTransitionKey != null)
                        {
                            this.ProjectedRootTarget = fullSeries[subSequenceDecrease.CurrentNode.Veins.DFAOriginState.OutTransitions[actualTransitionKey]];
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        /* *
                         * The follow transition disambiguated through the
                         * state's local transitions.
                         * */
                        if (actualTransitionKey != null)
                            this.ProjectedRootTarget = fullSeries[originatingState.OutTransitions[actualTransitionKey]];
                        else
                        {

                        }
                    }
                }
                else
                {
                    /* *
                     * There wasn't enough look-ahead to allow the automation
                     * to consume the input on the stack.
                     * *
                     * This is a legal 'null transition target'.
                     * */
                    return;
                }
            }
        }

        public string CurrentLookAheadBranches
        {
            get
            {
                /* *
                 * When debugging, the more information the better.
                 * *
                 * Especially with such large datasets!
                 * */
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (var set in this.GetCurrentPathSets())
                {
                    var gv = set.Discriminator;
                    if (first)
                        first = false;
                    else
                        sb.Append("->");
                    if (gv.IsEmpty)
                        sb.Append("ε");
                    else
                        sb.Append(gv);
                    if (set.ReductionType == LookAheadReductionType.CommonForwardSymbol)
                    {
                        sb.Append("->«");
                        sb.Append(set.commonSymbol.Veins.Rule.Name);
                        sb.Append("»");
                    }
                }
                return sb.ToString();
            }
        }

        private void BuildLookAheadWith(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, GrammarVocabulary transitionGrammar, bool usePrevious, GrammarSymbolSet grammarSymbols, ICompilerErrorCollection compilationErrors, bool isReduction)
        {
            Dictionary<GrammarVocabulary, List<Tuple<PredictionTreeBranch, PredictionTreeBranch>>> advanceSet;
            advanceSet = (from path in this
                          from element in path.FollowTransition(fullSeries, transitionGrammar, ruleVocabulary, usePrevious, true, isReduction: isReduction)
                          from subElement in element.Value
                          group new { Origin = path, Target = subElement } by element.Key).ToDictionary(k => k.Key, v => v.Distinct().Select(k => Tuple.Create(k.Origin, k.Target)).ToList());
            var transitionTable = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<PredictionTreeBranch, PredictionTreeBranch>>();

            foreach (var grouping in advanceSet)
            {
                /* With Auto Segmentation, the transition table thinks it is done immediately upon hitting an empty 'remaining' set during breakdown.
                 * This fixes that shortcoming to handle epsilon transitions, used to mark an edge state even when more transitions are possible. */
                if (grouping.Key.IsEmpty)
                    transitionTable.SetAutoSegment(false);
                transitionTable.Add(grouping.Key, grouping.Value);
                if (grouping.Key.IsEmpty)
                    transitionTable.SetAutoSegment(true);
            }

            foreach (var key in transitionTable.Keys)
            {
                var transitionTargetsDetail = transitionTable[key];
                var transitionTargetsDistinct = transitionTargetsDetail.Select(k => k.Item2).Distinct().ToList();

                var currentValue = new PredictionTree(key, transitionTargetsDistinct, PredictionDerivedFrom.LookAhead_LLk) { Previous = this, Root = this.Root };
                currentValue.origins =
                    (from det in transitionTargetsDetail
                     group det.Item1 by det.Item2).ToDictionary(k => k.Key, v => v.ToArray());
                this.lookAhead._Add(key, currentValue);
            }
            /* *
             * Lexical ambiguity handling follows after the full transition table is known.
             * */
            SyntacticAnalysisCore.CreateLexicalAmbiguityTransitions(grammarSymbols, this.lookAhead, this, this.Root, fullSeries, ruleVocabulary);
        }

        private string _pathSetsString;

        private string PathSetsString
        {
            get
            {
                if (_pathSetsString == null)
                {
                    StringBuilder sb = new StringBuilder();
                    bool first=true;
                    foreach (var p in this)
                    {
                        if (first)
                            first = false;
                        else
                            sb.AppendLine();
                        sb.Append(p);
                    }
                    _pathSetsString = sb.ToString();
                }
                return _pathSetsString;
            }
        }

        //private List<PredictionTree> CopyLogic(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, bool usePrevious, GrammarSymbolSet grammarSymbols, ICompilerErrorCollection compilationErrors, PredictionTree currentValue)
        //{
        //    var replicationSets = new Stack<Tuple<PredictionTree, PredictionTree>>();
        //    var nextSet = new Stack<Tuple<PredictionTree, PredictionTree>>();
        //    replicationSets.Push(Tuple.Create(this, currentValue));
        //    bool trackFinalAdvanceSet = this.advanceCalled;
        //    var advanceSet = new List<PredictionTree>();
        //    repeatSet:
        //    Parallel.ForEach(replicationSets, currentSet=>
        //    {
        //        //var currentSet = replicationSets.Pop();
        //        var currentLeft = currentSet.Item2;
        //        var currentRight = currentSet.Item1;
        //        currentLeft.baseList                = currentRight.baseList;
        //        currentLeft.originalSet             = currentRight.originalSet;
        //        currentLeft.advanceCalled           = currentRight.advanceCalled;
        //        if (trackFinalAdvanceSet && !currentRight.advanceCalled)
        //            lock (advanceSet)
        //                advanceSet.Add(currentLeft);
        //        else if (currentRight.advanceCalled)
        //        {
        //            currentLeft.lookAhead = new ControlledDictionary<GrammarVocabulary, PredictionTree>();
        //            foreach (var transitionKvp in currentRight.LookAhead)
        //            {
        //                var transitionKey = transitionKvp.Key;
        //                var transitionTargetSet = (PredictionTree)transitionKvp.Value;
        //                var newLeft = new PredictionTree(transitionKey, transitionTargetSet.ToList(), transitionTargetSet._derivedFrom);
        //                newLeft.Previous = currentLeft;
        //                newLeft.originalSet = transitionTargetSet.originalSet;
        //                currentLeft.lookAhead._Add(transitionKey, newLeft);
        //                lock (nextSet)
        //                    nextSet.Push(Tuple.Create(transitionTargetSet, newLeft));
        //            }
        //            currentLeft.advanceCalled = true;
        //        }

        //        if (currentLeft != currentValue)
        //            currentLeft.Root = currentValue.Root;
        //        if (currentLeft.Count > 1 && currentLeft.Any(k => k.CurrentNode.RootLeaf.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None))
        //        {
        //            if (!HandleLeftRecursionCheck(fullSeries, ruleVocabulary, compilationErrors, currentLeft))
        //                return;
        //        }

        //        currentLeft.reductionType = currentRight.reductionType;
        //        currentLeft.ForceEdgeState = currentRight.ForceEdgeState;
        //        currentLeft.isRepetitionPoint = currentRight.isRepetitionPoint;
        //        if (currentRight.reductionType == LookAheadReductionType.CommonForwardSymbol)
        //        {
        //            currentLeft.commonSymbol = currentRight.commonSymbol;
        //            currentLeft.ProcessCommonSymbol(fullSeries, ruleVocabulary);
        //        }
        //        if (currentRight.ProjectedRootTarget != null || currentRight.ProjectedRootTransition != null)
        //            currentLeft.PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
        //        currentLeft._isCopy = true;
        //        if (currentLeft.isRepetitionPoint)
        //        {
        //            var replicationPoint = currentLeft.GetReplicationPoint();
        //            if (replicationPoint != null)
        //                currentLeft.SetIsReplicationPoint(fullSeries, ruleVocabulary, replicationPoint);
        //            else
        //            {
        //                currentLeft.isRepetitionPoint = false;
        //                if (currentLeft.lookAhead.Count == 0)
        //                    currentLeft.advanceCalled = false;
        //                currentLeft.reductionType = LookAheadReductionType.Uncalculated;
        //                currentLeft.CheckReductionState(fullSeries, ruleVocabulary);
        //                lock (advanceSet)
        //                    advanceSet.Add(currentLeft);
        //            }
        //        }
        //        //currentLeft.ProjectedRootTarget     = currentRight.ProjectedRootTarget;
        //        //currentLeft.ProjectedRootTransition = currentRight.ProjectedRootTransition;
        //    });
        //    if (nextSet != null && nextSet.Count > 0)
        //    {
        //        replicationSets = nextSet;
        //        nextSet = new Stack<Tuple<PredictionTree, PredictionTree>>();
        //        goto repeatSet;
        //    }
        //    return advanceSet;
        //}

        /* *
         * Hack to allow explicit control over when this is invoked
         * by the debugger.
         * */
        public IEnumerable<string> ExpandedForm
        {
            get
            {
                if (this._expandedFormHandler != null)
                {
                    yield return (this.expandedForm = this._expandedFormHandler.Value);
                    this._expandedFormHandler = null;
                }
                else
                    yield return GetDebugStringOld();
            }
        }

        public int FullCount
        {
            get
            {
                var previous = Previous as PredictionTree;
                if (previous == null)
                    return this.Count;
                else
                    return previous.FullCount + this.Count;
            }
        }

        private Lazy<string> _expandedFormHandler = null;
        private string GenerateDebugString(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, GrammarSymbolSet grammarSymbols)
        {
            var origin = this.ProjectOrigin(fullSeries, ruleVocabulary);
            int accumulator = 0;
            StringBuilder sb = new StringBuilder();
            string leftPadding = string.Empty;
            int depth = 0;
            foreach (var nodeSet in origin)
            {
                leftPadding = new string(' ', depth * 4);
                var pathSet = nodeSet.Tree;
                sb.AppendFormat("{0}Discriminator: {1}", leftPadding, pathSet.Discriminator);
                sb.AppendLine();
                sb.AppendFormat("{0}Reduction Type: {1}", leftPadding, pathSet.ReductionType);
                sb.AppendLine();
                if (pathSet.PointOfCommonality != null)
                {
                    sb.AppendFormat("{0}Point of Commonality: {1}", leftPadding, pathSet.PointOfCommonality.Rule.Name);
                    sb.AppendLine();
                }
                sb.AppendFormat("{0}Paths: ", leftPadding);
                sb.AppendLine();


                for (int index = 0; index < pathSet.Count; index++, accumulator++)
                {
                    sb.AppendFormat("{0}[{1}] - {2}", leftPadding, accumulator, pathSet[index]);
                    sb.AppendLine();
                }
                depth++;
            }
            return sb.ToString();
        }

        public string GetDebugStringOld(string previous = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Discriminator: {0}", this.Discriminator);
            sb.AppendLine();
            sb.AppendFormat("Reduction Type: {0}", this.ReductionType);
            sb.AppendLine();
            if (this.PointOfCommonality != null)
            {
                sb.AppendFormat("Point of Commonality: {0}", this.PointOfCommonality.Rule.Name);
                sb.AppendLine();
            }
            sb.AppendLine("Paths: ");
            int baseOffset = this.FullCount - this.Count;

            for (int index = 0; index < this.Count; index++)
            {
                sb.AppendFormat("[{0}] - {1}", index + baseOffset, this[index]);
                sb.AppendLine();
            }

            if (previous != null)
            {
                var lines = previous.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    sb.AppendFormat("\t{0}", line);
                    sb.AppendLine();
                }
            }

            if (this.Previous == null)
                return sb.ToString();
            else
                return ((PredictionTree)this.Previous).GetDebugStringOld(sb.ToString());
        }

        public IEnumerable<PredictionTreeBranch> UnalteredOriginals
        {
            get
            {
                if (this.originalSet != null)
                    lock (this.originalSet)
                        return this.originalSet.ToArray();
                else
                    lock (this.baseList)
                        return this.baseList.ToArray();
            }
        }

        public SyntacticalNFAState GetNFAState(ParserCompiler compiler)//ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (this._reductionType == LookAheadReductionType.Uncalculated)
            {
                this.Advance(compiler.AllProjectionNodes, compiler.RuleGrammarLookup, compiler.CompilationErrors, compiler._GrammarSymbols);
                Debug.Assert(this.LookAhead.Count == 0);
            }
            if (this.nfaState == null)
            {
                switch (this.ReductionType)
                {
                    case LookAheadReductionType.None:
                        this.nfaState = new SyntacticalNFAState(compiler.RuleDFAStates, compiler._GrammarSymbols);
                        HandleNoReductionType(this, this, this.nfaState, compiler.RuleDFAStates, compiler._GrammarSymbols);
                        break;
                    case LookAheadReductionType.CommonForwardSymbol:
                        this.nfaState = new SyntacticalNFAState(compiler.RuleDFAStates, compiler._GrammarSymbols);
                        HandleCommonForwardSymbol(this, this, nfaState);
                        break;
                    case LookAheadReductionType.LeftRecursive:
                    case LookAheadReductionType.LocalTransition:
                        this.nfaState = new SyntacticalNFAState(compiler.RuleDFAStates, compiler._GrammarSymbols);
                        HandleLocalTransition(this, this, this.nfaState);
                        break;
                    case LookAheadReductionType.RepetitionPoint:
                        this.nfaState = new SyntacticalNFAState(compiler.RuleDFAStates, compiler._GrammarSymbols);
                        break;
                    default:
                        break;
                }
                if (this.IsEdge)
                    this.nfaState.IsEdge = true;
            }
            return this.nfaState;
        }

        private static void SetupReplicationPointSources(ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols, PredictionTree set)
        {
            switch (set.ReplicationPoint.ReductionType)
            {
                case LookAheadReductionType.None:
                    HandleNoReductionType(set.ReplicationPoint, set, set.nfaState, lookup, symbols);
                    break;
                case LookAheadReductionType.CommonForwardSymbol:
                    HandleCommonForwardSymbol(set.ReplicationPoint, set, set.nfaState);
                    break;
                case LookAheadReductionType.LocalTransition:
                    HandleLocalTransition(set.ReplicationPoint, set, set.nfaState);
                    break;
                default:
                    break;
            }
        }

        private static void HandleLocalTransition(PredictionTree masterSet, PredictionTree replicationSet, SyntacticalNFAState nfaState)
        {
            if (masterSet.ProjectedRootTransition == null && masterSet.First.projectionType == ProductionRuleProjectionType.FollowAmbiguity)
                nfaState.SetFinal(masterSet.Root.RootLeaf.SetDecisionFor(masterSet.ProjectedRootTransition ?? replicationSet.Discriminator, masterSet.ProjectedRootTarget));
            else if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                nfaState.SetFinal(masterSet.Root.RootLeaf.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            if (masterSet.reductionContext != null)
                nfaState.SetInitial(masterSet.reductionContext);
            nfaState.SetInitial(replicationSet);
        }

        private static void HandleCommonForwardSymbol(PredictionTree masterSet, PredictionTree replicationSet, SyntacticalNFAState nfaState)
        {
            if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                nfaState.SetFinal(masterSet.Root.RootLeaf.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            if (masterSet.reductionContext != null)
                if (replicationSet != masterSet)
                    nfaState.SetInitial(replicationSet.reductionContext = new ProductionRuleProjectionReduction { LookAheadDepth = 0, ReducedRule = masterSet.reductionContext.ReducedRule, Rule = masterSet.reductionContext.Rule });
                else
                    nfaState.SetInitial(masterSet.reductionContext);
            nfaState.SetInitial(replicationSet);
        }

        private static void HandleNoReductionType(PredictionTree masterSet, PredictionTree replicationSet, SyntacticalNFAState state, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                if (masterSet.First.projectionType == ProductionRuleProjectionType.FollowAmbiguity)
                    state.SetFinal(masterSet.Root.RootLeaf.SetDecisionFor(masterSet.ProjectedRootTransition ?? replicationSet.Discriminator, masterSet.ProjectedRootTarget));
                else
                    state.SetFinal(masterSet.Root.RootLeaf.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            state.SetInitial(replicationSet);
        }

        internal void BuildNFA(ParserCompiler compiler)//Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (hasBuiltNFA)
                return;
            this.hasBuiltNFA = true;
            if (this.nfaState == null)
                this.GetNFAState(compiler);//compiler.RuleDFAStates, compiler._GrammarSymbols);
            if (this.ReductionType == LookAheadReductionType.RepetitionPoint)
            {
                lock (this.First.replFixups) 
                    this.First.replFixups.Add(this);
                return;
            }
            foreach (var transition in this.LookAhead.Keys)
            {
                if (transition.IsEmpty)
                    this.nfaState.IsEdge = true;
                var currentTarget = (PredictionTree)this.LookAhead[transition];
                currentTarget.BuildNFA(compiler);//fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            }
            foreach (var transition in this.LookAhead.Keys)
            {
                var currentTarget = (PredictionTree)this.LookAhead[transition];
                if (transition.IsEmpty)
                {
                    nfaState.IsEdge = true;

                    currentTarget.GetNFAState(compiler).IterateSources((source, type) =>
                    {
                        switch (type)
                        {
                            case FiniteAutomationSourceKind.Final:
                                this.PredictTransitionVocabulary(compiler.AllProjectionNodes, compiler.RuleGrammarLookup, compiler.CompilationErrors, currentTarget);
                                if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                {
                                    Debug.Assert(this.ProjectedRootTransition != null && this.ProjectedRootTarget != null);
                                    this.nfaState.SetFinal(this.Root.RootLeaf.SetDecisionFor(this.ProjectedRootTransition, this.ProjectedRootTarget));
                                }
                                else
                                    this.nfaState.SetFinal(this.Root.RootLeaf.SetDecisionFor(this.ProjectedRootTransition ?? this.Discriminator, this.ProjectedRootTarget));
                                break;
                            case FiniteAutomationSourceKind.Initial:
                                this.nfaState.SetInitial(source);
                                break;
                            case FiniteAutomationSourceKind.Intermediate:
                                this.nfaState.SetIntermediate(source);
                                break;
                            case FiniteAutomationSourceKind.RepeatPoint:
                                this.nfaState.SetRepeat(source);
                                break;
                        }
                    });
                }
                else
                    this.nfaState.MoveTo(transition, currentTarget.GetNFAState(compiler));//lookup, symbols));
            }
        }

        internal PredictionTree FindDuplicate(IEnumerable<PredictionTree> otherSet)
        {
            using (var otherEnum = otherSet.GetEnumerator())
            {
                while (otherEnum.MoveNext())
                //for (int otherIndex = 0; otherIndex < otherSet.Count; otherIndex++)
                {
                    var other = otherEnum.Current;
                    var thisCurrent = this;
                    while (thisCurrent != null && other != null)
                    {
                        if (thisCurrent.Count != other.Count)
                            goto continueOuter;
                        if (thisCurrent.ReductionType != other.ReductionType)
                            goto continueOuter;
                        if (thisCurrent.GetHashCode() != other.GetHashCode())
                            goto continueOuter;
                        if (!thisCurrent.Discriminator.Equals(other.Discriminator))
                            goto continueOuter;
                        if (thisCurrent.Previous == null ^ other.Previous == null)
                            goto continueOuter;
                        if (!thisCurrent.PathEquivalence(other))
                            goto continueOuter;
                        thisCurrent = (PredictionTree)thisCurrent.Previous;
                        other = (PredictionTree)other.Previous;
                    }
                    return otherEnum.Current;
                continueOuter:
                    ;
                }
            }
            return null;
        }

        internal void HandleReplFixups(ParserCompiler compiler)//Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            foreach (var repl in replFixups)
                HandleReplFixup(compiler, repl);//, fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            this.replFixups.Clear();
        }

        private static void HandleReplFixup(ParserCompiler compiler, PredictionTree set)//, Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            var replState = set.ReplicationPoint.GetNFAState(compiler);
            SetupReplicationPointSources(compiler.RuleDFAStates, compiler._GrammarSymbols, set);
            set.ReplicationPoint.BuildNFA(compiler);//fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            foreach (var transition in replState.OutTransitions.Keys)
            {
                var targetStateSet = replState.OutTransitions[transition];

                foreach (var targetState in targetStateSet)
                    set.nfaState.MoveTo(transition, targetState);
                if (transition.IsEmpty)
                    set.nfaState.IsEdge = true;
            }
        }

        public PredictionTreeLeaf PointOfCommonality
        {
            get
            {
                return this.commonSymbol;
            }
        }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.Root.Rule; }
        }

        private PredictionTree _first;
        private bool _processedSymbol;
        private int? _hashCode;
        private bool _isLeftRecursiveReduction;
        public PredictionTree First
        {
            get
            {
                return _first ?? (this._first = this.GetFirst());
            }
        }

        private PredictionTree GetFirst()
        {
            PredictionTree current = this;
            while (current.Previous != null)
                current = (PredictionTree)current.Previous;
            return current;
        }

        internal void SetFollowEpsilonData(Func<PredictionTreeBranch, int[]> contextProvider)
        {
            if (contextProvider == null)
                throw new ArgumentNullException("contextProvider");
            this.followEpsilonLevels = new Dictionary<PredictionTreeBranch, int[]>();
            foreach (var path in this)
                followEpsilonLevels.Add(path, contextProvider(path));
        }

        public bool IsEdge
        {
            get
            {
                if (this._reductionType == LookAheadReductionType.RepetitionPoint)
                    return this.ReplicationPoint.IsEdge;
                return this.LookAhead.Count == 0 || this.ForceEdgeState;
            }
        }

        internal void ReplaceLookahead(GrammarVocabulary element, PredictionTree targetExistingPath)
        {
            lock (this.locker)
            {
                if (this.lookAhead == null)
                    return;
                this.lookAhead._Remove(element);
                this.lookAhead._Add(element, targetExistingPath);
            }
        }

        internal GrammarVocabulary[] GetGrammarsArray()
        {
            lock (locker)
                if (this.lookAhead == null)
                    return new GrammarVocabulary[0];
                else
                    return this.lookAhead.Keys.ToArray();
        }

        public bool ForceEdgeState { get; set; }

        public bool IsAmbiguousState { get; set; }

        public GrammarVocabulary ProjectedRootTransition { get; private set; }
        public PredictionTreeLeaf ProjectedRootTarget { get; private set; }

        public bool IsRepetitionTarget { get; private set; }

        internal PredictionTree _getBacker { get; set; }

        public int Instance { get { return this._instTracker; } }

        internal class DebuggerTypeProxy
        {
            private NamedPropertyDetail[] npdSet;
            private PredictionTree _original;
            private LookAheadDetail[] _lookAheadDetail;
            private string[] _expandedForm;
            //private NamedPropertyDetail[] _npdExpandedForm;
            public DebuggerTypeProxy(PredictionTree original)
            {
                this._original = original;
            }

            public PredictionDerivedFrom DerivedFrom { get { return this._original._derivedFrom; } }
            public int Instance { get { return this._original._instTracker; } }
            public PredictionTree Previous { get { return (PredictionTree)this._original.Previous; } }
            public LookAheadReductionType ReductionType { get { return this._original.ReductionType; } }

            public PredictionTreeLeaf ReductionSymbol { get { return this._original.commonSymbol; } }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public LookAheadDetail[] LookAhead
            {
                get
                {
                    if (this._lookAheadDetail != null && (this._lookAheadDetail.Length != this._original.LookAhead.Count || !this._lookAheadDetail.Select(lad => lad.Discriminator).SequenceEqual(this._original.LookAhead.Keys)))
                        this._lookAheadDetail = null;
                    return this._lookAheadDetail ?? (this._lookAheadDetail = this.InitializeLookAheadDetail());
                }
            }

            public GrammarVocabulary Discriminator { get { return this._original.Discriminator; } }

            private LookAheadDetail[] InitializeLookAheadDetail()
            {
                var result = new LookAheadDetail[this._original.LookAhead.Count];
                int index = 0;
                foreach (var kvp in this._original.LookAhead)
                    result[index++] = new LookAheadDetail { Discriminator = kvp.Key, Target = (PredictionTree)kvp.Value };
                return result;
            }

            //public IControlledDictionary<GrammarVocabulary, PredictionTree> LookAhead { get { return this._original.LookAhead; } }
            public string LookAheadBranches { get { return this._original.CurrentLookAheadBranches; } }

            [DebuggerDisplay("{Target._instTracker} : {Target.Discriminator}", Name = "{Discriminator}")]
            public class LookAheadDetail
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public GrammarVocabulary Discriminator { get; set; }
                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public PredictionTree Target { get; set; }
            }

            [DebuggerDisplay("{Value, nq}", Name="{PrefixText,nq}{Index}")]
            public class NamedPropertyDetail
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public int Index { get; set; }
                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public object Value { get; set; }
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public string Prefix { get; set; }
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public string PrefixText { get { return string.IsNullOrEmpty(this.Prefix) ? string.Empty : string.Format("{0} - ", this.Prefix); } }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public NamedPropertyDetail[] ÿElements
            {
                get
                {
                    if (this.npdSet != null && this.npdSet.Length != this._original.Count)
                        this.npdSet = null;
                    return this.npdSet ?? (this.npdSet = this.InitializeNPDSet());
                }
            }

            public int Count { get { return this._original.Count; } }

            //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            //public IEnumerable<string> ExpandedForm 
            //{ 
            //    get 
            //    {
            //        return this._expandedForm ?? (this._expandedForm = this.InitializeExpandedFrom());
            //    }
            //}

            public int FullCount { get { return this._original.FullCount; } }

            public bool LookAheadAdvanced { get { return this._original.HasAdvanced; } }

            public bool IsAmbiguousState { get { return this._original.IsAmbiguousState; } }

            public bool IsEdge { get { return this._original.IsEdge; } }

            public bool IsRepetitionPoint { get { return this._original.IsRepetitionTarget; } }

            public GrammarVocabulary ProjectedRootTransition { get { return this._original.ProjectedRootTransition; } }

            public PredictionTreeLeaf ProjectedRootTarget { get { return this._original.ProjectedRootTarget; } }

            public ProductionRuleProjectionType ProjectionType { get { return this._original.projectionType; } }

            public PredictionTreeLeaf Root { get { return this._original.Root; } }

            public IOilexerGrammarProductionRuleEntry Rule { get { return this._original.Rule; } }



            private string[] InitializeExpandedFrom()
            {
                List<string> expandedForm = this._original.ExpandedForm.ToList();
                var expandedForm2 = new List<string>();
                foreach (var element in expandedForm)
                {
                    var split = element.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    expandedForm2.AddRange(split);
                }
                return expandedForm2.ToArray();
            }


            private NamedPropertyDetail[] InitializeNPDSet()
            {
                NamedPropertyDetail[] result = new NamedPropertyDetail[this._original.Count];
                int index = 0;
                foreach (var element in this._original)
                {
                    result[index] = new NamedPropertyDetail { Index = index, Value = element };
                    index++;
                }
                return result;
            }
        }
    }

    public enum PredictionDerivedFrom
    {
        Unknown,
        LookAhead_LL1,
        LookAhead_LLk,
        LookAhead_Prediction,
        LookAhead_FollowPrediction,
        Lookahead_Epsilon,
        LookAhead_AmbiguityCast,
    }
}
//ToDO: Investigate the bloat of the Parser results, I added something during my last visit to this project that related to Left-recursive situations which bloated the result code.