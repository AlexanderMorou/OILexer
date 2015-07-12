using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
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
    /// Provides a base implementation of properties and methods 
    /// for working with a series of <see cref="IProductionRuleProjectionDPath{TPath, TNode}"/>
    /// elements which is structured towards dependency analysis.
    /// </summary>
    /// <remarks>
    /// Used to disambiguate the target path of a given
    /// production in look-ahead analysis.</remarks>
    public class ProductionRuleProjectionDPathSet :
        ControlledCollection<ProductionRuleProjectionDPath>,
        IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>,
        IEquatable<ProductionRuleProjectionDPathSet>,
        IProductionRuleSource
    {
        private static int instanceTracker = 0;
        private int _instTracker = instanceTracker++;
        private PredictionDerivedFrom _derivedFrom;
        private bool hasBeenReducedAlready = false;
        private SyntacticalNFAState nfaState;
        private bool hasBuiltNFA = false;
        private LookAheadReductionType reductionType = LookAheadReductionType.Uncalculated;
        private int reductionLookAheadDrop;
        private bool isRepetitionPoint;
        private ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>> lookAhead;
        private ProductionRuleProjectionNode commonSymbol;
        private List<ProductionRuleProjectionDPath> originalSet;
        private ProductionRuleProjectionDPathSet pointOfRedundancy;
        private ProductionRuleProjectionReduction reductionContext;
        private List<ProductionRuleProjectionDPathSet> replFixups = new List<ProductionRuleProjectionDPathSet>();
        private string expandedForm;
        internal ProductionRuleProjectionType projectionType = ProductionRuleProjectionType.Unspecified;
        private Dictionary<ProductionRuleProjectionDPath, int[]> followEpsilonLevels;
        private bool _isBadReductionCandidate = false;
        private ProductionRuleProjectionDPathSet(GrammarVocabulary discriminator, List<ProductionRuleProjectionDPath> series, PredictionDerivedFrom derivedFrom)
        {
            this._derivedFrom = derivedFrom;
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
            /* *
             * 
             * */
            this.baseList = this.originalSet.ToArray().ToList();
            this.Discriminator = discriminator;
        }
        public GrammarVocabulary Discriminator { get; internal set; }

        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionDPathSet"/>
        /// which denotes a point of redundancy in the
        /// result state determination machine.
        /// </summary>
        public ProductionRuleProjectionDPathSet ReplicationPoint { get { return this.pointOfRedundancy; } }

        public static ProductionRuleProjectionDPathSet GetPathSet(GrammarVocabulary discriminator, List<ProductionRuleProjectionDPath> series, ProductionRuleProjectionNode root, ProductionRuleProjectionType projectionType, PredictionDerivedFrom derivedFrom)
        {
            return new ProductionRuleProjectionDPathSet(discriminator, series, derivedFrom) { Root = root, projectionType = projectionType };
        }

        public LookAheadReductionType ReductionType
        {
            get
            {
                return this.reductionType;
            }
        }

        internal void CheckReductionState(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            if (this.reductionType == LookAheadReductionType.Uncalculated)
                this.reductionType = this.GetReductionType(fullSeries, ruleVocabulary);
        }

        public IEnumerable<ProductionRuleProjectionDPath> GetEpsilonLevel(int level = 0)
        {
            var result = new HashSet<ProductionRuleProjectionDPath>();
            foreach (var set in this.followEpsilonLevels.Keys)
            {
                var eps = this.followEpsilonLevels[set];
                if (eps.Contains(level))
                    result.Add(set);
            }
            return result;
        }

        private LookAheadReductionType GetReductionType(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            if (this.isRepetitionPoint)
                return LookAheadReductionType.RepetitionPoint;
            if (!this._isBadReductionCandidate && this.GetHasCommonSymbol(this.originalSet, ref this.commonSymbol))
                return LookAheadReductionType.CommonForwardSymbol;
            if (this.GetIsLocalTransition())
                return LookAheadReductionType.LocalTransition;
            return LookAheadReductionType.None;
        }
        internal void ProcessCommonSymbol()
        {
            if (this.ReductionType != LookAheadReductionType.CommonForwardSymbol)
                return;
            if (this._processedSymbol)
                return;
            this._processedSymbol = true;
            //if (this.Previous != null)
            //{
            this.CheckBack();
            int devLevel = this[0].GetCurrentDeviation() - ReductionCount;
            if (devLevel > 0 && this.Previous != null)
            {
                var backRef = this.GetBack(devLevel);
                this.commonSymbol.DenoteReductionPoint(this, backRef);
                this._getBacker = backRef;
            }
            else
                this.commonSymbol.DenoteReductionPoint(this);
            //}
        }

        public short ReductionCount
        {
            get
            {
                return (short)(this.GetCurrentPathSets().Where(k => k.ReductionType == LookAheadReductionType.CommonForwardSymbol).Count());
            }
        }

        private ProductionRuleProjectionDPathSet GetBack(int steps)
        {
            ProductionRuleProjectionDPathSet current = this;

            for (int i = 0; i < steps; i++)
            {
                if (current != null)
                    current = (ProductionRuleProjectionDPathSet)current.Previous;
            }
            return current;
        }

        //public void DetermineReductionState

        public int[] GetRightSideSimilarity(Tuple<int[], ProductionRuleProjectionDPathSet> other)
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

        public int[] GetCompoundRightSideSimilarity(IEnumerable<Tuple<int[], ProductionRuleProjectionDPathSet>> others)
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

        internal static Dictionary<int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>> GetCompoundRightSideSimilarities(IEnumerable<Tuple<int[], ProductionRuleProjectionDPathSet>> series)
        {
            var currentBreakdowns = (from s in series
                                     let pathSet = s.Item2
                                     group s by s.Item2.Count).ToDictionary(k => k.Key, v => v.ToArray());
            Dictionary<int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>> results = new Dictionary<int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>>();
            foreach (var count in currentBreakdowns.Keys)
            {
                List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>> currentResults;
                if (!results.TryGetValue(count, out currentResults))
                    results.Add(count, currentResults = new List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>());
                Stack<Tuple<int[], ProductionRuleProjectionDPathSet>> toMatch = new Stack<Tuple<int[], ProductionRuleProjectionDPathSet>>(currentBreakdowns[count]);
                Stack<Tuple<int[], ProductionRuleProjectionDPathSet>> nextMatch = new Stack<Tuple<int[], ProductionRuleProjectionDPathSet>>();
                while (toMatch.Count > 0)
                {
                    var currentTarget = toMatch.Pop();
                    var currentMatches = new List<Tuple<int[], ProductionRuleProjectionDPathSet>>() { currentTarget };
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
                        nextMatch = new Stack<Tuple<int[], ProductionRuleProjectionDPathSet>>();
                    }
                }
            }
            return results;
        }

        private bool HasInnerCommonSymbol(IList<ProductionRuleProjectionDPath> series, ref ProductionRuleProjectionNode commonSymbol)
        {
            var firstPath = series[0];
            bool result = false;
            if (firstPath.Depth == firstPath.MinDepth)
                return false;
            int[] matchDepths = null;
            ProductionRuleProjectionNode matchSymbol = null;
            for (int nodeIndex = firstPath.Depth; nodeIndex > firstPath.MinDepth; nodeIndex--)
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
                    int deviation = 0;
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
                    deviation = deviations[0];
                    if (!deviations.Skip(1).All(k => k == deviation))
                    {
                        /* *
                         * If the look-ahead shift varies, we have a non-deterministic situation
                         * which makes it a non-candidate for this solution.
                         * */
                        continue;
                    }
                    matchSymbol = currentFirstNode;
                    matchDepths = newDepths;
                    result = true;
                }
            }
            if (matchSymbol != null)
            {
                int deviation = 0;
                int pathIndex = 0;
                int[] deviations = new int[series.Count];
                for (pathIndex = 0; pathIndex < series.Count; pathIndex++)
                {
                    var currentPath = series[pathIndex];
                    for (int index = currentPath.Depth; index > matchDepths[pathIndex]; index--)
                    {
                        deviations[pathIndex] += currentPath.GetDeviationAt(index);
                    }
                }
                pathIndex = 0;
                deviation = deviations[0];
                if (deviation > 0)
                    reductionLookAheadDrop = deviation;

                /*Determine if the selected symbol is a good candidate for reduction*/

                var matchedPaths = (from path in series
                                    let queryPathIndex = pathIndex++
                                    select path.DepthAt(matchDepths[queryPathIndex])).ToList();



                ReduceSet(matchedPaths);
                commonSymbol = matchSymbol;

            }
            return result;
        }

        private void ReduceSet(List<ProductionRuleProjectionDPath> basePaths)
        {
            if (hasBeenReducedAlready)
                return;
            hasBeenReducedAlready = true;
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

        private bool GetHasCommonSymbol(List<ProductionRuleProjectionDPath> series, ref ProductionRuleProjectionNode commonSymbol)
        {
            ProductionRuleProjectionNode commonSymbol2 = commonSymbol;
            if (series.Count <= 1 || !series.All(k => k.Depth > k.MinDepth))
                return false;
        RecheckCommon:
            if (this.HasInnerCommonSymbol(series, ref commonSymbol2))
                if (commonSymbol2 != commonSymbol)
                {
                    commonSymbol = commonSymbol2;
                    goto RecheckCommon;
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

        private int MostInCommon(int minLength)
        {
            if (this.Count == 0)
                return 0;
            if (this.Count == 1)
                return this[0].Count;
            var compareSet = this[0];
            int mostInCommon = int.MaxValue;
            for (int seriesIndex = 1; seriesIndex < this.Count; seriesIndex++)
            {
                int currentResult = 0;
                var currentSet = this[seriesIndex];
                for (int leftSeriesIndex = currentSet.Count - 1, rightSeriesIndex = compareSet.Count - 1; leftSeriesIndex >= currentSet.Count - minLength; leftSeriesIndex--, rightSeriesIndex--)
                {
                    var left = currentSet[leftSeriesIndex];
                    var right = compareSet[rightSeriesIndex];
                    if (left != right)
                        break;
                    currentResult++;
                }
                if (currentResult < mostInCommon)
                    mostInCommon = currentResult;
            }
            return mostInCommon;
        }

        public ProductionRuleProjectionNode Root { get; private set; }


        private IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode> _previous;
        public IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode> Previous
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

        internal void CheckBack()
        {
            if (this.Previous != null && !this.Discriminator.IsEmpty)
                if (!this.Previous.LookAhead.Keys.Any(k => !k.Intersect(this.Discriminator).IsEmpty))
                    Debug.Assert(false, "Invalid association!");
        }
        public IControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>> LookAhead
        {
            get
            {
                if (this.lookAhead == null)
                    this.lookAhead = new ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>>();
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
                current = (ProductionRuleProjectionDPathSet)current.Previous;
            }
            yield break;
        }

        internal IEnumerable<ProductionRuleProjectionDPathSet> GetCurrentPathSets()
        {
            return GetCurrentPathSetsInternal().Reverse();
        }

        public int LookAheadDepth
        {
            get
            {
                int result = 0;
                var current = this;
                while (current != null)
                {
                    current = (ProductionRuleProjectionDPathSet)current.Previous;
                    result++;
                }
                return result;
            }
        }

        private IEnumerable<ProductionRuleProjectionDPathSet> GetCurrentPathSetsInternal()
        {
            var current = this;
            while (current != null)
            {
                yield return current;
                current = (ProductionRuleProjectionDPathSet)current.Previous;
            }
            yield break;
        }

        public bool Equals(IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode> other)
        {
            return this.Equals(other as ProductionRuleProjectionDPathSet);
        }

        public bool Equals(ProductionRuleProjectionDPathSet other)
        {
            return other != null && this.Count == other.Count && PathEquivalence(other) && other.Discriminator.Equals(this.Discriminator) && this.ReductionType == other.ReductionType && this.LookAheadDepth == other.LookAheadDepth && (this.Previous == null && other.Previous == null || this.First.Discriminator.Equals(other.First.Discriminator));
        }

        public bool Equivalent(ProductionRuleProjectionDPathSet other)
        {
            if (other == null || this.ReductionType != other.ReductionType || other.Count != this.Count || !other.Discriminator.Equals(this.Discriminator))
                return false;
            return PathEquivalence(other) && (this.Previous == null && other.Previous == null || this.First.Discriminator.Equals(other.First.Discriminator));
        }

        private bool PathEquivalence(ProductionRuleProjectionDPathSet other)
        {
            using (IEnumerator<ProductionRuleProjectionDPath> myEnum = this.GetEnumerator())
            {
                for (; myEnum.MoveNext(); )
                {
                    bool found = false;
                    using (IEnumerator<ProductionRuleProjectionDPath> otherEnum = other.GetEnumerator())
                        for (; otherEnum.MoveNext(); )
                            if (otherEnum.Current.Equals(myEnum.Current))
                            {
                                found = true;
                                break;
                            }
                    if (!found)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (this._hashCode == null)
            {
                int result = this.Count;
                foreach (var element in this)
                    result ^= element.GetHashCode();
                result ^= this.Discriminator.GetHashCode();
                //result ^= this.LookAheadDepth;
                this._hashCode = result;
            }
            return this._hashCode.Value;
        }

        protected override void AddImpl(ProductionRuleProjectionDPath expression)
        {
            if (!base.baseList.Contains(expression))
                base.AddImpl(expression);
        }

        public bool HasAdvanced { get { return this.advanceCalled; } }

        bool advanceCalled = false;
        private object locker = new object();
        internal bool Advance(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
            if (this.lookAhead == null)
                this.lookAhead = new ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>>();
            lock (locker)
            {
                if (advanceCalled)
                    return false;
                advanceCalled = true;
            }
            if (this.lookAhead.Count > 0)
                return false;
        RecheckReductionState:
            this.CheckReductionState(fullSeries, ruleVocabulary);
            try
            {
                switch (this.ReductionType)
                {
                    case LookAheadReductionType.None:
                        {
                            /* *
                             * allAreEquivalent is not relevant here.
                             * The purpose of LookAheadReductionType.CommonForwardSymbol
                             * is to catch cases where 'allAreEquivalent' might occur.
                             * */
                            if (this.Count > 1)
                            {
                                var replicationPoint = GetReplicationPoint();
                                if (replicationPoint == null)
                                {
#if ShowStateBreakdown
                                Console.WriteLine("{0} - {1} - Ambiguous state, affected paths: {5}, steps: {3}\r\n\t{4}", this.Discriminator, this.Root, string.Empty, currentSets.Length, this.CurrentLookAheadBranches, this.Count);
                                PrintPaths();
#endif
                                    BuildLookAheadWith(fullSeries, ruleVocabulary, this.Discriminator, false, grammarSymbols);
                                    if (this.LookAhead.Count == 0 || this.LookAhead.Count == 1 && this.LookAhead.Keys[0].IsEmpty)
                                    {
                                        /* *
                                         * An ambiguous construct is noted, regardless of the intent, 
                                         * this will yield an ambiguity due to two rules requiring the same
                                         * set of tokens.
                                         * */
                                        if (!this.Discriminator.IsEmpty || this.All(k => k.CurrentNode.Value.OriginalState.IsEdge))
                                        {
                                            ProductionRuleProjectionDPath firstPath;
                                            bool allAreEquivalent;
                                            AllCurrentNodesAreEquivalent(out firstPath, out allAreEquivalent);
                                            if (!allAreEquivalent)
                                            {
                                                lock (compilationErrors)
                                                    compilationErrors.ModelError<string>(OilexerGrammarCore.CompilerErrors.AmbiguousPathDetected, this.ExpandedForm.First());
                                                this.IsAmbiguousState = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SetIsReplicationPoint(fullSeries, ruleVocabulary, replicationPoint);
                                    //return true;
                                }
                            }
                            else
                            {
#if ShowStateBreakdown
                            Console.WriteLine("{0} - {1} - Final Look-Ahead encountered, took {4} steps: {2}\r\n\t{3}", this.Discriminator, this.Root, this[0], this.CurrentLookAheadBranches, currentSets.Length);
                            PrintPaths();
#endif
                                var first = this[0];
                                PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors);
                                if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                {
                                    Debug.Assert(this.ProjectedRootTarget != null, "Cannot determine target!");
                                    //if (this.ProjectedRootTarget != null)
                                    //    Debug.Print(this.ProjectedRootTarget.ToString());
                                }

                                return false;
                            }
                            break;
                        }
                    case LookAheadReductionType.CommonForwardSymbol:
                        {
                            ProductionRuleProjectionDPath firstPath;

                            bool allAreEquivalent;
                            AllCurrentNodesAreEquivalent(out firstPath, out allAreEquivalent);
                            /* The transition that was necessary to get to this point isn't relevant at this point,
                             * Due to the reduction we're now looking at what the paths transition into after
                             * that commonSymbol. */
                            if (this.Count > 1 && !allAreEquivalent)
                            {
                                var transitionGrammar = ruleVocabulary[this.commonSymbol.Rule];

                                var replicationPoint = GetReplicationPoint();
                                if (replicationPoint == null)
                                {
                                    var firstOriginalPath = this.originalSet.First();

                                    this.reductionContext = this.Root.RootNode.SetReductionOn(transitionGrammar, firstOriginalPath.GetCurrentDeviation() + reductionLookAheadDrop + 1, fullSeries);
                                    BuildLookAheadWith(fullSeries, ruleVocabulary, transitionGrammar, true, grammarSymbols);
                                    /* After reaching this point, we need to evaluate the possibility that the rule
                                     * being reduced is a bad candidate for reduction. If any of the edge nodes for the
                                     * rule being reduced overlap with the prediction, we have a bad candidate, not because
                                     * it's impossible to generate a machine for, but because it has a great chance for yielding
                                     * infinite machines. */
                                    if (ShouldBackOutOfCommonTransition(fullSeries, ruleVocabulary))
                                        goto RecheckReductionState;
#if ShowStateBreakdown
                                Console.WriteLine("{0} - {1} - Common Forward Symbol - All paths point to common symbol: `{2}`, Paths Affected: {5}, Steps Taken: {4}\r\n\t{3}", this.Discriminator, this.Root, this.commonSymbol.Name, this.CurrentLookAheadBranches, currentSets.Length, this.Count);
                                PrintPaths();
#endif
                                }
                                else
                                {
                                    SetIsReplicationPoint(fullSeries, ruleVocabulary, replicationPoint);
                                    //return true;
                                }
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
                                return false;
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
                return true;
            }
            finally
            {
                this.ProcessCommonSymbol();
            }
        }

        private bool ShouldBackOutOfCommonTransition(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            var edgesOfCommon = this.commonSymbol.RootNode.Value.OriginalState.ObtainEdges();
            var commonEdgeTransitionAggregate = edgesOfCommon.Select(k => k.OutTransitions.FullCheck).Aggregate(GrammarVocabulary.UnionAggregateDelegate);
            if (!this.lookAhead.Keys.Aggregate(GrammarVocabulary.UnionAggregateDelegate).Intersect(commonEdgeTransitionAggregate).IsEmpty)
            {
                this.commonSymbol = null;
                lock (this.baseList)
                {
                    this.baseList.Clear();
                    foreach (var element in this.originalSet)
                        this.baseList.Add(element);
                }
                this.reductionType = LookAheadReductionType.Uncalculated;
                this._isBadReductionCandidate = true;
                this.lookAhead._Clear();
                return true;
            }
            return false;
        }

        private void AllCurrentNodesAreEquivalent(out ProductionRuleProjectionDPath firstPath, out bool allAreEquivalent)
        {
            firstPath = this.First();
            var initialDepth = firstPath.Depth;
            var initialNodes = firstPath.Take(firstPath.Depth + 1);
            /* *
             * Different paths lead here; however, if the roots of all 
             * current paths lead to the same forward symbol,
             * then this is a special case that doesn't require further
             * look-ahead.
             * */
            allAreEquivalent = this.All(k => k.Depth > 0 && k.Depth == initialDepth && k.Take(k.Depth + 1).SequenceEqual(initialNodes));
        }

        private ProductionRuleProjectionDPathSet GetReplicationPoint()
        {
            var currentSets = this.GetCurrentPathSets().ToArray();
            var replicationPoint = currentSets.FirstOrDefault(k => k.Equivalent(this) && !Object.ReferenceEquals(this, k));
            return replicationPoint;
        }

        private void SetIsReplicationPoint(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ProductionRuleProjectionDPathSet replicationPoint)
        {
            this.isRepetitionPoint = true;
            this.reductionType = LookAheadReductionType.Uncalculated;
            this.pointOfRedundancy = replicationPoint;
            this.CheckReductionState(fullSeries, ruleVocabulary);
            this.ReplicationPoint.RegisterRepetition();
            this.ProcessCommonSymbol();
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
        private void PredictTransitionVocabulary(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ProductionRuleProjectionDPathSet disambiguationContext = null)
        {
            SyntacticalDFAState originatingState;
            if (this.Previous == null && disambiguationContext == null)
            {
                switch (this.ReductionType)
                {
                    case LookAheadReductionType.None:
                        this.ProjectedRootTransition = ruleVocabulary[this[0][1].RootNode.Rule];
                        break;
                    case LookAheadReductionType.CommonForwardSymbol:
                        this.ProjectedRootTransition = ruleVocabulary[this[0][1].RootNode.Rule];
                        break;
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
                    originatingState = firstNode.Value.OriginalState;
                    SetProjectedRootTarget(fullSeries, originatingState, ruleVocabulary);
                    goto DecisionPoint;
                }
                /* *
                 * ToDo: Note Possible Projection Failure.
                 * */
                goto DecisionPoint;
            }
            ProductionRuleProjectionDPathSet[] series;
            List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[] resultTransitionSet;

            ProjectOrigin(fullSeries, ruleVocabulary, out series, out resultTransitionSet);
            if (disambiguationContext != null)
            {
                bool ruleReduction = series[series.Length - 1].ReductionType == LookAheadReductionType.CommonForwardSymbol;
                bool usedRuleContext = series[series.Length - 1].Discriminator.GetTokenVariant().IsEmpty;

                var shiftedFirstSet = (from path in resultTransitionSet[resultTransitionSet.Length - 1]
                                       where path.Item2.Depth >= path.Item2.MinDepth
                                       let transitions = path.Item2.FollowTransition(fullSeries, ruleReduction ? ruleVocabulary[path.Item2.CurrentNode.Rule] : series[series.Length - 1].Discriminator, ruleVocabulary, (usedRuleContext || ruleReduction) && path.Item2.Depth > path.Item2.MinDepth, false, true, false, true)
                                       from target in transitions.Values
                                       from targetPath in target
                                       where disambiguationContext == null ? targetPath.CurrentNode.Value.OriginalState.IsEdge : disambiguationContext.Contains(targetPath)
                                       select path).Distinct().ToList();
                if (shiftedFirstSet.Count > 0)
                    resultTransitionSet[0] = Trace(shiftedFirstSet, resultTransitionSet);
            }
            var element = resultTransitionSet[0];

            for (int targetOffset = 0; targetOffset < element.Count; targetOffset++)
            {
                var firstPath = element[targetOffset];
                var firstNode = GetTargetNode(firstPath);
                var fromNode = firstPath.Item2[firstPath.Item2.MinDepth];
                if (firstPath.Item2.Count == 1)
                {
                    if (element.Skip((targetOffset + 1)).All(k => k.Item2[0] == firstNode))
                    {
                        this.ProjectedRootTransition = series[0].Discriminator;
                        SetProjectedRootTarget(fullSeries, fromNode.Value.OriginalState, ruleVocabulary, resultTransitionSet);
                        goto DecisionPoint;
                    }
                    else
                    {

                    }
                }
                else if ((firstPath.Item2.Count == 1 ? 0 : firstPath.Item2.Depth == firstPath.Item2.MinDepth ? 0 : 1) > firstPath.Item2.MinDepth && element.Skip((targetOffset+1)).All(k => GetTargetNode(k) == firstNode))
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
                            if (firstPath.Item2.Depth == 0)
                                this.ProjectedRootTransition = series[0].Discriminator;
                            else
                                this.ProjectedRootTransition = ruleVocabulary[firstNode.Rule];
                            break;
                    }
                    SetProjectedRootTarget(fullSeries, fromNode.Value.OriginalState, ruleVocabulary, resultTransitionSet);
                    goto DecisionPoint;
                }
                else if (disambiguationContext != null && disambiguationContext.Previous == this)
                {
                    this.ProjectedRootTarget = disambiguationContext.ProjectedRootTarget;
                    this.ProjectedRootTransition = disambiguationContext.ProjectedRootTransition;
                    if (this.ProjectedRootTarget != null)
                        SetProjectedRootTarget(fullSeries, fromNode.Value.OriginalState, ruleVocabulary, resultTransitionSet);
                    goto DecisionPoint;
                }
                else
                {
                    this.ProjectedRootTransition = series[0].Discriminator;
                    SetProjectedRootTarget(fullSeries, fromNode.Value.OriginalState, ruleVocabulary, resultTransitionSet);
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
                Debug.WriteLine("Null Transition Target");
            }
        }

        private static ProductionRuleProjectionNode GetTargetNode(Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath> firstPath)
        {
            int firstNodeTarget = firstPath.Item2.MinDepth + (firstPath.Item2.Count == 1 ? 0 : firstPath.Item2.Depth == firstPath.Item2.MinDepth ? 0 : 1);
            var firstNode = firstPath.Item2[firstNodeTarget];
            return firstNode;
        }

        private List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>> Trace(List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>> traceSource, List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[] traceTrail)
        {
            var origin = new Stack<List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>>(traceTrail);
            if (origin.Count > 0)
                origin.Pop();
            while (origin.Count > 0)
            {
                var originTracker = origin.Pop();
                traceSource = (from t in traceSource
                               join source in originTracker on t.Item2 equals source.Item1
                               select source).Distinct().ToList();
            }
            return traceSource;
        }

        internal Tuple<ProductionRuleProjectionDPathSet[], List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[]> ProjectOrigin(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            ProductionRuleProjectionDPathSet[] series;
            List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[] resultTransitionSet;
            ProjectOrigin(fullSeries, ruleVocabulary, out series, out resultTransitionSet);
            return Tuple.Create(series, resultTransitionSet);
        }

        private void ProjectOrigin(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, out ProductionRuleProjectionDPathSet[] series, out List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[] resultTransitionSet)
        {
            series = this.GetCurrentPathSets().ToArray();
            Stack<ProductionRuleProjectionDPathSet> reverseProjections = new Stack<ProductionRuleProjectionDPathSet>(series);
            Stack<List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>> reversedPathTransitionDiagram = new Stack<List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>>();

            reversedPathTransitionDiagram.Push((from k in this.originalSet.ToList()
                                                select new Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>(null, k)).ToList());
            var previousSet = this.originalSet.ToList();
            ProductionRuleProjectionDPathSet previous = null;
            while (reverseProjections.Count > 0)
            {
                var current = reverseProjections.Pop();
                if (previous != null)
                {
                    //var subsetPeek = reversedPathTransitionDiagram.Peek();
                    bool useRuleTarget = current.ReductionType == LookAheadReductionType.CommonForwardSymbol;
                    var resultTransitions = (from path in current
                                             let subset = path.FollowTransition(fullSeries, useRuleTarget ? ruleVocabulary[current.PointOfCommonality.Rule] : current.Discriminator, ruleVocabulary, useRuleTarget, true, true, false)
                                             from transitionKey in subset.Keys
                                             let actualPossibleAmbiguousKey = previous.Discriminator.DisambiguateVocabulary()
                                             let intersection = transitionKey.Intersect(actualPossibleAmbiguousKey)
                                             where !intersection.IsEmpty || (transitionKey.IsEmpty && actualPossibleAmbiguousKey.IsEmpty)
                                             let properSubset = subset[transitionKey]
                                             from properPath in properSubset
                                             from previousSetPath in previousSet
                                             where PathsEquivalent(properPath, previousSetPath)
                                             select Tuple.Create<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>(previousSetPath, current.UnalteredOriginals.ElementAt(current.IndexOf(path)))).Distinct().ToList();
                    previousSet = (from k in resultTransitions
                                   select k.Item2).ToList();
                    reversedPathTransitionDiagram.Push(resultTransitions);
                }
                previous = current;
            }
            resultTransitionSet = reversedPathTransitionDiagram.ToArray();
        }

        

        private static bool PathsEquivalent(ProductionRuleProjectionDPath properPath, ProductionRuleProjectionDPath previousPath)
        {
            if (previousPath.Equals(properPath))
                return true;
            if (previousPath.Depth < properPath.Depth && properPath.MinDepth <= previousPath.Depth)
                return properPath.DepthAt(previousPath.Depth).Equals(previousPath);
            else if (previousPath.Depth > properPath.Depth && previousPath.MinDepth <= properPath.Depth)
                return previousPath.DepthAt(properPath.Depth).Equals(properPath);
            return false;
        }

        private void SetProjectedRootTarget(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, SyntacticalDFAState originatingState, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, List<Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>[] incomingTransitions = null)
        {
            var actualTransitionKey = originatingState.OutTransitions.Keys.FirstOrDefault(k => !k.Intersect(this.ProjectedRootTransition.DisambiguateVocabulary()).IsEmpty);
            if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
            {
                if (actualTransitionKey != null)
                {
                    this.ProjectedRootTarget = fullSeries[originatingState.OutTransitions[actualTransitionKey]];
                    Debug.Assert(this.ProjectedRootTarget.RootNode == this.Root.RootNode);
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
                ProductionRuleProjectionDPath targetSequenceMatch = null;
                foreach (var tailInitialPair in firstIncoming)
                {
                    var topLevel = tailInitialPair.Item2;
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
                        var subSequence = new ProductionRuleProjectionDPath(targetSequenceMatch.Take(targetSequenceMatch.MinDepth + 2).ToList(), targetSequenceMatch.MinDepth + 1, false, false);
                        var subSequenceDecrease = subSequence.DecreaseDepth();
                        this.ProjectedRootTransition = ruleVocabulary[subSequence.CurrentNode.Rule];
                        actualTransitionKey = subSequenceDecrease.CurrentNode.Value.OriginalState.OutTransitions.Keys.FirstOrDefault(k => !k.Intersect(this.ProjectedRootTransition).IsEmpty);
                        if (actualTransitionKey != null)
                        {
                            this.ProjectedRootTarget = fullSeries[subSequenceDecrease.CurrentNode.Value.OriginalState.OutTransitions[actualTransitionKey]];
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
                        sb.Append(set.commonSymbol.Value.Rule.Name);
                        sb.Append("»");
                    }
                }
                return sb.ToString();
            }
        }

        private void BuildLookAheadWith(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, GrammarVocabulary transitionGrammar, bool usePrevious, GrammarSymbolSet grammarSymbols)
        {
            var advanceSet = (from path in this
                              from element in path.FollowTransition(fullSeries, transitionGrammar, ruleVocabulary, usePrevious, true)
                              from subElement in element.Value
                              group subElement by element.Key).ToArray();
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath> transitionTable = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath>();
            foreach (var grouping in advanceSet)
            {
                if (grouping.Key.IsEmpty)
                    transitionTable.SetAutoSegment(false);
                transitionTable.Add(grouping.Key, grouping.ToList());
                if (grouping.Key.IsEmpty)
                    transitionTable.SetAutoSegment(true);
            }
            foreach (var key in transitionTable.Keys)
            {
                var transitionTargets = transitionTable[key];
                var currentValue = new ProductionRuleProjectionDPathSet(key, transitionTargets, PredictionDerivedFrom.LookAhead_LLk) { Previous = this, Root = this.Root };
                currentValue.CheckReductionState(fullSeries, ruleVocabulary);
                ProductionRuleProjectionDPathSet currentMasterValue;
                lock (this.Root.PathSets)
                    currentMasterValue = this.Root.PathSets.FirstOrDefault(k => k.Equals(currentValue));
                if (this.lookAhead.ContainsKey(key))
                {
                    ProductionRuleProjectionDPathSet currentTarget = (ProductionRuleProjectionDPathSet)this.lookAhead[key];
                    if (!currentTarget.Equivalent(currentValue))
                    {
                        /* *
                         * ToDo: Figure out timing issue that causes
                         * this anomaly to occur and require this 'hack'.
                         * */
                        var repeatPoint = currentValue.GetReplicationPoint();
                        if (repeatPoint != null)
                            currentValue.SetIsReplicationPoint(fullSeries, ruleVocabulary, repeatPoint);
                        if (!currentTarget.Equivalent(currentValue))
                            throw new InvalidOperationException("An unknown state transition error occurred.");
                    }

                }
                else if (currentMasterValue != null)
                {
                    /* If the master has a path set which is, state-wise, identical to the current, we can copy what comes next
                     * because they'd evaluate the same; however, it's important that how we got here be tracked. */
                    currentMasterValue.CopyLogic(currentValue);
                    this.lookAhead._Add(key, currentValue);
                }
                else
                {
                    this.lookAhead._Add(key, currentValue);
                    lock (this.Root.PathSets)
                        this.Root.PathSets.Add(currentValue);
                    currentValue.ProcessCommonSymbol();
                }
                //(currentMasterValue ?? currentValue).CheckBack();
            }
            /* *
             * Lexical ambiguity handling follows after the full transition table is known.
             * */
            SyntacticAnalysisCore.CreateLexicalAmbiguityTransitions(grammarSymbols, this.lookAhead, this, this.Root, fullSeries, ruleVocabulary);
        }

        private void CopyLogic(ProductionRuleProjectionDPathSet currentValue)
        {
            currentValue.baseList = this.baseList;
            currentValue.originalSet = this.originalSet;
            currentValue.advanceCalled = this.advanceCalled;
            currentValue.lookAhead = this.lookAhead;
            currentValue.ForceEdgeState = this.ForceEdgeState;
            currentValue.isRepetitionPoint = this.isRepetitionPoint;
            currentValue.ProjectedRootTarget = this.ProjectedRootTarget;
            currentValue.ProjectedRootTransition = this.ProjectedRootTransition;
        }

        /* *
         * Hack to allow explicit control over when this is invoked
         * by the debugger.
         * */
        public IEnumerable<string> ExpandedForm
        {
            get
            {
                yield return expandedForm ?? (expandedForm = GetDebugString());
            }
        }

        public int FullCount
        {
            get
            {
                var previous = Previous as ProductionRuleProjectionDPathSet;
                if (previous == null)
                    return this.Count;
                else
                    return previous.FullCount + this.Count;
            }
        }

        public string GetDebugString(string previous = null)
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
                return ((ProductionRuleProjectionDPathSet)this.Previous).GetDebugString(sb.ToString());
        }

        public IEnumerable<ProductionRuleProjectionDPath> UnalteredOriginals
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

        public SyntacticalNFAState GetNFAState(ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            //if (this[0].instanceTracker == 1681589)
            //{
            //}
            if (this.nfaState == null)
            {
                switch (this.ReductionType)
                {
                    case LookAheadReductionType.None:
                        this.nfaState = new SyntacticalNFAState(lookup, symbols);
                        HandleNoReductionType(this, this, this.nfaState, lookup, symbols);
                        break;
                    case LookAheadReductionType.CommonForwardSymbol:
                        this.nfaState = new SyntacticalNFAState(lookup, symbols);
                        HandleCommonForwardSymbol(this, this, nfaState);
                        break;
                    case LookAheadReductionType.LocalTransition:
                        this.nfaState = new SyntacticalNFAState(lookup, symbols);
                        HandleLocalTransition(this, this, this.nfaState);
                        break;
                    case LookAheadReductionType.RepetitionPoint:
                        this.nfaState = new SyntacticalNFAState(lookup, symbols);
                        break;
                    default:
                        break;
                }
                if (this.IsEdge)
                    this.nfaState.IsEdge = true;
            }
            return this.nfaState;
        }

        private static void SetupReplicationPointSources(ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols, ProductionRuleProjectionDPathSet set)
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

        private static void HandleLocalTransition(ProductionRuleProjectionDPathSet masterSet, ProductionRuleProjectionDPathSet replicationSet, SyntacticalNFAState nfaState)
        {
            if (masterSet.ProjectedRootTransition == null && masterSet.First.projectionType == ProductionRuleProjectionType.FollowAmbiguity)
                nfaState.SetFinal(masterSet.Root.RootNode.SetDecisionFor(masterSet.ProjectedRootTransition ?? replicationSet.Discriminator, masterSet.ProjectedRootTarget));
            else if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                nfaState.SetFinal(masterSet.Root.RootNode.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            if (masterSet.reductionContext != null)
                nfaState.SetInitial(masterSet.reductionContext);
            nfaState.SetInitial(replicationSet);
        }

        private static void HandleCommonForwardSymbol(ProductionRuleProjectionDPathSet masterSet, ProductionRuleProjectionDPathSet replicationSet, SyntacticalNFAState nfaState)
        {
            if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                nfaState.SetFinal(masterSet.Root.RootNode.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            if (masterSet.reductionContext != null)
                if (replicationSet != masterSet)
                    nfaState.SetInitial(replicationSet.reductionContext = new ProductionRuleProjectionReduction { LookAheadDepth = 0, ReducedRule = masterSet.reductionContext.ReducedRule, Rule = masterSet.reductionContext.Rule });
                else
                    nfaState.SetInitial(masterSet.reductionContext);
            nfaState.SetInitial(replicationSet);
        }

        private static void HandleNoReductionType(ProductionRuleProjectionDPathSet masterSet, ProductionRuleProjectionDPathSet replicationSet, SyntacticalNFAState state, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (masterSet.IsEdge && !masterSet.IsAmbiguousState)
                if (masterSet.First.projectionType == ProductionRuleProjectionType.FollowAmbiguity)
                    state.SetFinal(masterSet.Root.RootNode.SetDecisionFor(masterSet.ProjectedRootTransition ?? replicationSet.Discriminator, masterSet.ProjectedRootTarget));
                else
                    state.SetFinal(masterSet.Root.RootNode.SetDecisionFor(masterSet.ProjectedRootTransition, masterSet.ProjectedRootTarget));
            state.SetInitial(replicationSet);
        }

        internal void BuildNFA(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            if (hasBuiltNFA)
                return;
            this.hasBuiltNFA = true;
            if (this.nfaState == null)
                this.GetNFAState(lookup, symbols);
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
                var currentTarget = (ProductionRuleProjectionDPathSet)this.LookAhead[transition];
                currentTarget.BuildNFA(fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            }
            foreach (var transition in this.LookAhead.Keys)
            {
                var currentTarget = (ProductionRuleProjectionDPathSet)this.LookAhead[transition];
                if (transition.IsEmpty)
                {
                    nfaState.IsEdge = true;

                    currentTarget.GetNFAState(lookup, symbols).IterateSources((source, type) =>
                    {
                        switch (type)
                        {
                            case FiniteAutomationSourceKind.Final:
                                this.PredictTransitionVocabulary(fullSeries, ruleVocabulary, compilationErrors, currentTarget);
                                if (this.First.projectionType != ProductionRuleProjectionType.FollowAmbiguity)
                                {
                                    Debug.Assert(this.ProjectedRootTransition != null && this.ProjectedRootTarget != null);
                                    this.nfaState.SetFinal(this.Root.RootNode.SetDecisionFor(this.ProjectedRootTransition, this.ProjectedRootTarget));
                                }
                                else
                                    this.nfaState.SetFinal(this.Root.RootNode.SetDecisionFor(this.ProjectedRootTransition ?? this.Discriminator, this.ProjectedRootTarget));
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
                    this.nfaState.MoveTo(transition, currentTarget.GetNFAState(lookup, symbols));
            }
        }

        internal void HandleReplFixups(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            foreach (var repl in replFixups)
                HandleReplFixup(repl, fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            this.replFixups.Clear();
        }

        private static void HandleReplFixup(ProductionRuleProjectionDPathSet set, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            var replState = set.ReplicationPoint.GetNFAState(lookup, symbols);
            SetupReplicationPointSources(lookup, symbols, set);
            set.ReplicationPoint.BuildNFA(fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            foreach (var transition in replState.OutTransitions.Keys)
            {
                var targetStateSet = replState.OutTransitions[transition];

                foreach (var targetState in targetStateSet)
                    set.nfaState.MoveTo(transition, targetState);
                if (transition.IsEmpty)
                    set.nfaState.IsEdge = true;
            }
        }

        public ProductionRuleProjectionNode PointOfCommonality
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

        private ProductionRuleProjectionDPathSet _first;
        private bool _processedSymbol;
        private int? _hashCode;
        public ProductionRuleProjectionDPathSet First
        {
            get
            {
                return _first ?? (this._first = this.GetFirst());
            }
        }

        private ProductionRuleProjectionDPathSet GetFirst()
        {
            ProductionRuleProjectionDPathSet current = this;
            while (current.Previous != null)
                current = (ProductionRuleProjectionDPathSet)current.Previous;
            return current;
        }

        internal void SetFollowEpsilonData(Func<ProductionRuleProjectionDPath, int[]> contextProvider)
        {
            if (contextProvider == null)
                throw new ArgumentNullException("contextProvider");
            this.followEpsilonLevels = new Dictionary<ProductionRuleProjectionDPath, int[]>();
            foreach (var path in this)
                followEpsilonLevels.Add(path, contextProvider(path));
        }

        public bool IsEdge
        {
            get
            {
                if (this.reductionType == LookAheadReductionType.RepetitionPoint)
                    return this.ReplicationPoint.IsEdge;
                return this.LookAhead.Count == 0 || this.ForceEdgeState;
            }
        }

        internal void ReplaceLookahead(GrammarVocabulary element, ProductionRuleProjectionDPathSet targetExistingPath)
        {
            if (this.lookAhead == null)
                return;
            this.lookAhead._Remove(element);
            this.lookAhead._Add(element, targetExistingPath);
        }

        public bool ForceEdgeState { get; set; }

        public bool IsAmbiguousState { get; set; }

        public GrammarVocabulary ProjectedRootTransition { get; private set; }
        public ProductionRuleProjectionNode ProjectedRootTarget { get; private set; }

        public bool IsRepetitionTarget { get; private set; }

        public ProductionRuleProjectionDPathSet _getBacker { get; set; }
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
