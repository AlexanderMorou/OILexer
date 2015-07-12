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
    public class ProductionRuleProjectionNode :
        KeyedTree<IOilexerGrammarProductionRuleEntry, IProductionRuleProjectionNodeInfo<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>, ProductionRuleProjectionNode>,
        IProductionRuleProjectionNode<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>,
        IProductionRuleSource
    {
        private HashSet<ProductionRuleProjectionDPath> incoming = new HashSet<ProductionRuleProjectionDPath>();
        private ControlledDictionary<GrammarVocabulary, List<IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>>> incomingPaths;
        private ProductionRuleProjectionNode rootNode;
        private MultikeyedDictionary<GrammarVocabulary, ProductionRuleProjectionNode, IProductionRuleProjectionDecision> decisionTree = new MultikeyedDictionary<GrammarVocabulary, ProductionRuleProjectionNode, IProductionRuleProjectionDecision>();
        private MultikeyedDictionary<GrammarVocabulary, int, ProductionRuleProjectionReduction> reductionTree = new MultikeyedDictionary<GrammarVocabulary, int, ProductionRuleProjectionReduction>();
        private MultikeyedDictionary<GrammarVocabulary, int, List<ProductionRuleProjectionFollow>> followAmbiguities = new MultikeyedDictionary<GrammarVocabulary, int, List<ProductionRuleProjectionFollow>>();
        private int bucketCount = 0;
        private object laBucketLock = new object();
        private Dictionary<GrammarVocabulary, IProductionRuleProjectionDecision> followFailures = new Dictionary<GrammarVocabulary, IProductionRuleProjectionDecision>();
        private HashSet<ProductionRuleProjectionDPathSet> ambiguityContexts = new HashSet<ProductionRuleProjectionDPathSet>();
        private HashSet<ProductionRuleProjectionReductionDetail> steppedAmbiguityContexts = new HashSet<ProductionRuleProjectionReductionDetail>();
        private HashSet<HashList<HashList<ProductionRuleProjectionNode>>> rootAmbiguityContexts = new HashSet<HashList<HashList<ProductionRuleProjectionNode>>>();
        private bool _hasBeenReduced;
        public bool HasBeenReduced
        {
            get { return this._hasBeenReduced; }
            set
            {
                if (value != _hasBeenReduced)
                {
                    _hasBeenReduced = value;
                    if (value && !ParserCompiler.ReductionSignalTriggered)
                    {
                        ParserCompiler.ReductionSignalTriggered = true;
                    }
                    //else if (value && ParserCompiler.ReductionSignalTriggered)
                    //{
                    //    Console.WriteLine("Signaled Reduce on {0}", this.Rule.Name);
                    //}
                }
            }
        }
        /// <summary>
        /// Returns/sets the <see cref="IProductionRuleProjectionNodeInfo{TPath, TNode}"/>
        /// instance relative to the current node position.
        /// </summary>
        public IProductionRuleProjectionNodeInfo<ProductionRuleProjectionDPath, ProductionRuleProjectionNode> Value { get; set; }

        public IProductionRuleSource TerminalSource
        {
            get
            {
                return this;
            }
        }

        public ProductionRuleProjectionNode() { this.PathSets = new HashSet<ProductionRuleProjectionDPathSet>(); }

        internal void LinkProjections(IDictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleProjectionNode> nodeLookup)
        {
            if (this.Value == null)
                return;
            var fullCheck = this.Value.OriginalState.OutTransitions.FullCheck;
            var ruleCheck = fullCheck.GetRuleVariant();
            foreach (var rule in from r in ruleCheck.GetSymbols()
                                 select (IGrammarRuleSymbol)r)
                this._Add(rule.Source, nodeLookup[rule.Source]);
        }

        public override string ToString()
        {
            if (this.Value == null ||
                this.Value.OriginalState == null ||
                this.Value.Rule == null)
                return string.Format("Invalid Object State");
            if (this.Value.IsRuleNode)
                return string.Format("Rule Node for {0}", this.Value.Rule.Name);
            else
                return string.Format("Rule Sub-Node for {0} at {1}", this.Value.Rule.Name, this.Value.OriginalState.StateValue);
        }


        public IControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>> LookAhead { get; private set; }

        public bool RequiresLookAheadAutomation
        {
            get
            {
#if ShortcutToFindBug13
                return false;
#else
                return this.Value.OriginalState.OutTransitions.Count > 1 && this.LookAhead.Count > 0 && this.LookAhead.Values.Any(k => k.LookAhead.Count > 0) && this.followAmbiguities != null && this.followAmbiguities.Count == 0;
#endif
            }
        }

        public SyntacticalDFAState ConstructAdvanceDFA(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, ICompilerErrorCollection compilationErrors, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
        {
            SyntacticalNFAState resultNFANode = null;
            /* *
             * Simple for now, but I suspect it'll be more complicated
             * later on to provide sufficient context.
             * */
            if (this.Value.OriginalState is SyntacticalDFARootState)
                resultNFANode = new SyntacticalNFARootState(this.Value.Rule, lookup, symbols);
            else
                resultNFANode = new SyntacticalNFAState(lookup, symbols);
            foreach (var transition in this.LookAhead.Keys)
            {
                ((ProductionRuleProjectionDPathSet)this.LookAhead[transition])
                    .BuildNFA(fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            }
            foreach (var transition in this.LookAhead.Keys)
            {
                ((ProductionRuleProjectionDPathSet)this.LookAhead[transition]).
                HandleReplFixups(fullSeries, ruleVocabulary, compilationErrors, lookup, symbols);
            }
            /* *
             * Transition from the Variable look-ahead table
             * into the NFA states.
             * */
            foreach (var transition in this.LookAhead.Keys)
            {
                var currentSet = (ProductionRuleProjectionDPathSet)this.LookAhead[transition];
                resultNFANode.MoveTo(transition, currentSet.GetNFAState(lookup, symbols));
            }
            /* *
             * Create a Deterministic automation from the results.
             * */
            resultNFANode.SetInitial(this);
            return resultNFANode.DeterminateAutomata();
        }

        /// <summary>
        /// Constructs the initial look-ahead for the transitions.
        /// </summary>
        /// <remarks></remarks>
        public void ConstructInitialLookahead(GrammarSymbolSet grammarSymbols, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            /* *
             * Left-recursive rules will use the relevant rule
             * paths within their projections within a loop to avoid 
             * stack overflow.
             * */
            bool includeRules = this.RootNode.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None;
            var result = new ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>>();
            this.LookAhead = result;
            if (includeRules)
            {
                /* *
                 * On left-recursive rules, the requirements change.
                 * *
                 * The rules must be considered at the start to 
                 * ensure that  the left recursive branches
                 * can be switched 'off' to allow for reductions
                 * to function properly.
                 * */
                foreach (var key in this.Value.Keys)
                {
                    var targets = this.Value[key];
                    if (targets.Any(k => k.GetRecursionType() != ProductionRuleLeftRecursionType.None && this.RootNode == this))
                    {
                        var intersection = key.Intersect(this.Value.OriginalState.OutTransitions.FullCheck);
                        var reducedRules = key.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced || k.Node == k.Node.RootNode && k.Node.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None).Select(k => k.Symbol);
                        var ruleVar = new GrammarVocabulary(key.symbols, reducedRules.ToArray());
                        intersection |= (key.GetTokenVariant() | ruleVar);
                        if (!intersection.IsEmpty)
                            result._Add(intersection, targets);
                    }
                    else
                        DoReductionAware(fullSeries, result, key);
                }
            }
            else
            {
                foreach (var key in this.Value.Keys.ToArray())
                    DoReductionAware(fullSeries, result, key);
            }
            /* *
             * Lexical ambiguity handling follows after the full transition table is known.
             * */
            SyntacticAnalysisCore.CreateLexicalAmbiguityTransitions(grammarSymbols, result, null, this, fullSeries, ruleVocabulary);
        }


        private void DoReductionAware(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>> result, GrammarVocabulary key)
        {
            var tokenVar = key.GetTokenVariant();
            var reducedRules = 
                key
                .GetRuleVariant()
                .GetSymbols()
                .Select(k => (IGrammarRuleSymbol)k)
                .Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k })
                .Where(k => k.Node.HasBeenReduced).Select(k => k.Symbol);// || k.Node == k.Node.RootNode && k.Node.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None && this == this.RootNode
            var ruleVar = new GrammarVocabulary(key.symbols, reducedRules.ToArray());
            if (!tokenVar.IsEmpty)
                result._Add(tokenVar, this.Value[key]);
            if (!ruleVar.IsEmpty)
                result._Add(ruleVar, this.Value[key]);
        }

        internal HashSet<ProductionRuleProjectionDPathSet> PathSets { get; private set; }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.Value.Rule; }
        }

        public IEnumerable<ProductionRuleProjectionDPath> IncomingPaths
        {
            get
            {
                return this.incoming;
            }
        }

        internal void AddPath(ProductionRuleProjectionDPath path)
        {
            lock (incoming)
                incoming.Add(path);
        }

        internal void ClearCache()
        {
            this.ambiguityContexts = new HashSet<ProductionRuleProjectionDPathSet>();
            this.steppedAmbiguityContexts = new HashSet<ProductionRuleProjectionReductionDetail>();
            this.followFailures.Clear();
            this.reductionTree.Clear();
            this.Value.ClearCache();
            this.PathSets.Clear();
            this.followAmbiguities.Clear();
        }

        internal bool CalculateTerminalAmbiguities(
            Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> allProjectionNodes,
            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleDFAStates,
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup,
            GrammarSymbolSet grammarSymbols)
        {
            var ambiguities = ObtainTerminalAmbiguities(allProjectionNodes, ruleDFAStates, ruleLookup, grammarSymbols);
            /* *
             * First step, reduce the ambiguity data provided to us.
             * *
             * This is done here versus in the ObtainTerminalAmbiguities
             * due to the 'research' nature of this process.
             * *
             * It might have to be rewritten or the information added
             * back in.
             * */
            var ambiguityRewrite = (from ambigKeysValue in ambiguities
                                    from reductionTuple in ambigKeysValue.Value
                                    from pathSet in reductionTuple.Item1
                                    select new
                                    {
                                        AmbiguousTransition = ambigKeysValue.Keys.Key1,
                                        TyingPaths = ambigKeysValue.Keys.Key2,
                                        FollowAmbiguity = new ProductionRuleProjectionFollow(pathSet.Item2, this)//ConstructInitialFollow(this, ambigKeysValue.Keys.Key1, reductionTuple, allProjectionNodes, ruleDFAStates, ruleLookup),
                                    }).ToArray();
            foreach (var rewrite in ambiguityRewrite)
            {
                if (rewrite.FollowAmbiguity.InitialPaths.All(initialPath => initialPath.CurrentNode.Value.OriginalState.IsEdge))
                    continue;
                List<ProductionRuleProjectionFollow> currentSet;
                if (!this.followAmbiguities.TryGetValue(rewrite.AmbiguousTransition, rewrite.TyingPaths, out currentSet))
                    followAmbiguities.Add(rewrite.AmbiguousTransition, rewrite.TyingPaths, currentSet = new List<ProductionRuleProjectionFollow>());
                currentSet.Add(rewrite.FollowAmbiguity);
            }
            return followAmbiguities.Count > 0;
        }

        internal MultikeyedDictionary<GrammarVocabulary, int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>> ObtainTerminalAmbiguities(
            Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, 
            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleDFAs,
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup,
            GrammarSymbolSet grammarSymbols)
        {
            if (!this.Value.OriginalState.IsEdge)
                return new MultikeyedDictionary<GrammarVocabulary, int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>>();
            var tempDictionary = new MultikeyedDictionary<ProductionRuleProjectionDPath, GrammarVocabulary, Tuple<int[], ProductionRuleProjectionDPathSet>>();
            var rootNode = fullSeries[ruleDFAs[this.Rule]];
            var currentIncoming = new List<ProductionRuleProjectionDPath>(rootNode.incoming);
            var totalIncoming = new HashSet<ProductionRuleProjectionDPath>();
            Dictionary<ProductionRuleProjectionDPathSet, ProductionRuleProjectionDPathSet> uniqueSet = new Dictionary<ProductionRuleProjectionDPathSet, ProductionRuleProjectionDPathSet>();
            foreach (var path in currentIncoming)
            {
                var transitionTableResult = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<int, ProductionRuleProjectionDPath>>();
                var masterPathChop = new ProductionRuleProjectionDPath(path.Take(path.Depth).Concat(new[] { this }).ToArray(), path.Depth, false, false);

                masterPathChop.SetDeviations(path.GetDeviationsUpTo(path.Depth));
                ObtainTerminalAmbiguitiesOnPath(masterPathChop, fullSeries, ruleDFAs, ruleLookup, transitionTableResult, grammarSymbols);
                /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \
                 * Construct the sequence of Path->Grammar->PathSets based           *
                 * off of the table provided by the ObtainTerminalAmbiguitiesOnPath. *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **/
                foreach (var transition in transitionTableResult.Keys)
                {
                    var epDepthLookup =
                        (from entry in transitionTableResult[transition]
                         group entry.Item1 by entry.Item2).ToDictionary(k => k.Key, v => v.ToArray());
                    //int minDepth;
                    //var uniqueCurrent = GetUniqueDPathSet(uniqueSet, transitionTableResult, transition, epDepthLookup);
                    //var subPath = new ProductionRuleProjectionDPath(path.Skip(minDepth).ToList(), path.Depth - minDepth, minDepth: path.MinDepth);
                    tempDictionary.TryAdd(path, transition, Tuple.Create((from epDepth in transitionTableResult[transition]
                                                                          select epDepth.Item1).ToArray(), GetFollowDPathSet(transitionTableResult, transition, epDepthLookup)));
                }
            }



            var regrouping = (from ksvp in tempDictionary
                              group new { Path = ksvp.Keys.Key1, Set = ksvp.Value } by ksvp.Keys.Key2).ToDictionary(k => k.Key, v => v.ToArray());
            /*
            foreach (var pathSet in from key in regrouping.Keys
                                    let value = regrouping[key]
                                    from r in value
                                    select r.Set)
                pathSet.Item2.FixAllPaths();//*/

            var comparisons = (from key in regrouping.Keys
                               let value = regrouping[key]
                               let kRewrite = from r in value
                                              select r.Set
                               let commonalities = ProductionRuleProjectionDPathSet.GetCompoundRightSideSimilarities(kRewrite)
                               select new { Commonalities = commonalities, Transition = key }).ToDictionary(k => k.Transition, v => v.Commonalities);
            var resultDictionary = new MultikeyedDictionary<GrammarVocabulary, int, List<Tuple<Tuple<int[], ProductionRuleProjectionDPathSet>[], int[]>>>();

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \
             * Once we're done, we need to rebuild the paths with the MinDepth set to  *
             * their current Depth.  This is to ensure that it the PathSets don't try  *
             * to reduce more than necessary.                                          *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Then we take that set and perform a right-hand-side comparison between  *
             * the elements.                                                           *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * This will be used later to reduce the walking necessary to isolate the  *
             * proper machine to use to disambiguate the look-ahead.                   *
             \ * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **/
            foreach (var comparison in comparisons.Keys)
            {
                var comparisonElements = comparisons[comparison];
                foreach (var memberCount in comparisonElements.Keys)
                {
                    var comparisonElement = comparisonElements[memberCount];
                    resultDictionary.Add(comparison, memberCount, comparisonElement);
                }
            }
            return resultDictionary;
        }


        private ProductionRuleProjectionDPathSet GetFollowDPathSet(FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<int, ProductionRuleProjectionDPath>> transitionTableResult, GrammarVocabulary transition, Dictionary<ProductionRuleProjectionDPath, int[]> epDepthLookup)
        {
            //var minMinDepth = (from deviationPath in transitionTableResult[transition]
            //                   select deviationPath.Item2.MinDepth).Min();
            //minDepth = minMinDepth;
            //epDepthLookup = epDepthLookup.ToDictionary(dps => new ProductionRuleProjectionDPath(dps.Key.Skip(minMinDepth).ToList(), dps.Key.Depth - minMinDepth, minDepth: dps.Key.MinDepth - minMinDepth), dps => dps.Value);
            var result = ProductionRuleProjectionDPathSet.GetPathSet(transition, epDepthLookup.Keys.ToList(), this, ProductionRuleProjectionType.FollowAmbiguity, PredictionDerivedFrom.LookAhead_FollowPrediction);
            result.SetFollowEpsilonData(provider => epDepthLookup[provider]);
            return result;
        }

        private void ObtainTerminalAmbiguitiesOnPath(
            ProductionRuleProjectionDPath sourceMasterPath, 
            Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, 
            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleDFAs,
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup,
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<int, ProductionRuleProjectionDPath>> transitionTableResult,
            GrammarSymbolSet grammarSymbols)
        {
            var ambiguousSymbols = grammarSymbols.AmbiguousSymbols.ToList();
            Stack<Tuple<int, ProductionRuleProjectionDPath>> toProcess = new Stack<Tuple<int, ProductionRuleProjectionDPath>>();
            var fullCheck = this.Value.Keys.Aggregate(GrammarVocabulary.UnionAggregateDelegate);
            /* * * * * * * * * * * * * * * * * * * * * * * * * *  * *\
             * Kick things off with the master path, this is derived *
             * from the incoming states from the parent rule.        *
             * * * * * * * * * * * * * * * * * * * * * * * * * *  * */
            toProcess.Push(Tuple.Create(0, sourceMasterPath));
            //foreach (var transition in this.Value.Keys)
            //    foreach (var target in this.Value[transition])
            //        toProcess.Push(new ProductionRuleProjectionDPath(sourceMasterPath.Take(sourceMasterPath.Depth).Concat(target).ToList(), sourceMasterPath.Depth, true, false, sourceMasterPath.GetDeviationsUpTo(sourceMasterPath.Depth)));
            HashSet<ProductionRuleProjectionDPath> seen = new HashSet<ProductionRuleProjectionDPath>();

            while (toProcess.Count > 0)
            {
                var currentPathTuple = toProcess.Pop();
                var currentPath = currentPathTuple.Item2;
                if (currentPath.Depth == 0)
                    continue;
                if (currentPath.CurrentNode.Value.Keys.Aggregate(GrammarVocabulary.UnionAggregateDelegate).Intersect(fullCheck).IsEmpty)
                    continue;
                /* *
                 * Walk up the tree one level into the state that 
                 * follows the current rule in whatever the current
                 * calling rule might be.
                 * */
                var incomingTransitions = currentPath.FollowTransition(fullSeries, ruleLookup[currentPath.CurrentNode.Value.Rule], ruleLookup, true, false, true, true);
                /* *
                 * Aggregate the transitions from our previously 
                 * calculated LL(1) look-ahead information.
                 * */
                /* *
                 * On the state following this rule, gather up the
                 * targets which do overlap one or more of our
                 * transitions.
                 * */
                var fullAmbiguityContext = incomingTransitions.FullCheck | fullCheck;

                var intersectionPoints = (from t in incomingTransitions.Keys
                                          let intersection = t.Intersect(fullCheck)
                                          where !intersection.IsEmpty
                                          from path in incomingTransitions[t]
                                          select new { Intersection = intersection, Path = path }).ToArray();
                var overlap = GrammarVocabulary.NullInst;
                if ((fullAmbiguityContext != null && !fullAmbiguityContext.IsEmpty) && grammarSymbols.IsGrammarPotentiallyAmbiguous(fullAmbiguityContext))
                    foreach (var ambiguity in ambiguousSymbols)
                    {
                        var localIntersect = fullCheck & ambiguity.AmbiguityKey;
                        var incomingIntersect = incomingTransitions.FullCheck & ambiguity.AmbiguityKey;
                        if (localIntersect.IsEmpty || incomingIntersect.IsEmpty)
                            /* If the intersection isn't present on one or the other sides; then, for the purposes of
                             * this follow check, the union would not be relevant, it would likely be identified within the
                             * prediction upon the state itself. */
                            continue;
                        var aggregateIntersect = fullAmbiguityContext & ambiguity.AmbiguityKey;
                        var overlapIntersect = overlap & ambiguity.AmbiguityKey;
                        if (aggregateIntersect.Equals(ambiguity.AmbiguityKey) && !overlapIntersect.Equals(ambiguity.AmbiguityKey))
                        {
                            /*  */
                            var ambiguityIntersection =
                                (from t in incomingTransitions.Keys
                                 let intersection = t.Intersect(incomingIntersect)
                                 where !intersection.IsEmpty
                                 from path in incomingTransitions[t]
                                 select new { Intersection = intersection, Path = path }).ToArray();
                            var ambiguityVocabulary = new GrammarVocabulary(grammarSymbols, ambiguity);
                            foreach (var intersectingPoint in ambiguityIntersection)
                            {
                                var path = intersectingPoint.Path;
                                var intersection = localIntersect;
                                if (path.Equals(currentPath))
                                    continue;
                                /* Notify the nodes of the ambiguities so their state-machines can be patched up.
                                 * This is done explicitly to avoid the ambiguity cropping up and allowing it everywhere,
                                 * which is incorrect. If it shows up where it's not supposed to, we have a logic error
                                 * somwehere. */
                                foreach (var node in (from ambigPath in PushIntersection(sourceMasterPath, transitionTableResult, toProcess, seen, currentPathTuple, currentPath, path, intersection, ambiguityVocabulary)
                                                      select ambigPath.CurrentNode).Distinct().ToArray())
                                    node.DenoteLexicalAmbiguity(ambiguity);
                            }
                            ambiguity.Occurrences++;
                            overlap |= aggregateIntersect;
                        }
                    }
                foreach (var intersectingPoint in intersectionPoints)
                {
                    var path = intersectingPoint.Path;
                    var intersection = intersectingPoint.Intersection;
                    if (path.Equals(currentPath))
                        continue;
                    PushIntersection(sourceMasterPath, transitionTableResult, toProcess, seen, currentPathTuple, currentPath, path, intersection).ToArray();
                }
            }
        }

        private static IEnumerable<ProductionRuleProjectionDPath> PushIntersection(
            ProductionRuleProjectionDPath sourceMasterPath, 
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<int, ProductionRuleProjectionDPath>> transitionTableResult, 
            Stack<Tuple<int, ProductionRuleProjectionDPath>> toProcess,
            HashSet<ProductionRuleProjectionDPath> seen, 
            Tuple<int, ProductionRuleProjectionDPath> currentPathTuple, 
            ProductionRuleProjectionDPath currentPath, 
            ProductionRuleProjectionDPath path, 
            GrammarVocabulary intersection,
            GrammarVocabulary transitionKey = null)
        {
            /* * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Construct the transition table with the matching *
             * paths.                                           *
             * * * * * * * * * * * * * * * * * * * * * * * * * */
            var intersectingCurrent = from key in currentPath.CurrentNode.Value.Keys
                                      let intersect = key.Intersect(intersection)
                                      where !intersect.IsEmpty
                                      let subPathSet = currentPath.CurrentNode.Value[key]
                                      from subPath in subPathSet.UnalteredOriginals
                                      let rPath = new ProductionRuleProjectionDPath(sourceMasterPath.Take(currentPath.Depth).Concat(subPath).ToList(), currentPath.Depth + subPath.Depth, true, false, currentPath.GetDeviationsUpTo(currentPath.Depth), minDepth: currentPath.Depth)
                                      where rPath.Valid
                                      select rPath;
            var set = new List<Tuple<int, ProductionRuleProjectionDPath>>(from ip in intersectingCurrent
                                                                          select Tuple.Create(currentPathTuple.Item1, ip)) { Tuple.Create(currentPathTuple.Item1 + 1, new ProductionRuleProjectionDPath(path.baseList, path.Depth, false, false, minDepth: path.Depth)) };
            var distinctNodeArray = (from element in set
                                     select element.Item2).Distinct().ToArray();
            foreach (var element in distinctNodeArray)
                yield return element;
            transitionTableResult.Add(transitionKey ?? intersection, set);
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * If the current point is an edge, walk further        *
             * up the parse tree to see what else lies in store.    *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Further ambiguities might result in additional look- *
             * ahead being necessary.                               *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            if (seen.Add(path) && path.CurrentNode.Value.OriginalState.IsEdge)
                toProcess.Push(Tuple.Create(currentPathTuple.Item1 + 1, path));
        }
        private class TerminalTailSource :
            IProductionRuleSource
        {
            private readonly ProductionRuleProjectionNode parent;

            public TerminalTailSource(ProductionRuleProjectionNode parent)
            {
                this.parent = parent;
            }
            public IOilexerGrammarProductionRuleEntry Rule
            {
                get { return this.parent.Rule; }
            }
        }

        public ProductionRuleProjectionNode RootNode
        {
            get
            {
                return this.rootNode ?? this;
            }
            internal set
            {
                this.rootNode = value;
            }
        }

        public IProductionRuleProjectionDecision SetDecisionFor(GrammarVocabulary rootTransitionKey, ProductionRuleProjectionNode targetStateNode)
        {
            Debug.Assert(rootTransitionKey != null, "Error, the root transition key to make a decision was invalid!");
            IProductionRuleProjectionDecision result;
            if (targetStateNode != null)
            {
                
                if (!this.decisionTree.TryGetValue(rootTransitionKey, targetStateNode, out result))
                    this.decisionTree.Add(rootTransitionKey, targetStateNode, result = new ProductionRuleProjectionDecision() { DecidingFactor = rootTransitionKey, Target = targetStateNode });
            }
            else
            {
                if (!this.followFailures.TryGetValue(rootTransitionKey, out result))
                    this.followFailures.Add(rootTransitionKey, result = new ProductionRuleProjectionFollowFailure(this) { DecidingFactor = rootTransitionKey });
            }
            return result;
        }

        public ProductionRuleProjectionReduction SetReductionOn(GrammarVocabulary reducedRule, int deviationLevel, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries)
        {
            Debug.Assert(reducedRule != null, "Error, the root transition key to make a reduction was invalid!");
            ProductionRuleProjectionReduction result;
            var vocabSymbol = reducedRule.GetSymbols()[0];
            var ruleVocabSymbol = vocabSymbol as IGrammarRuleSymbol;
            if (ruleVocabSymbol != null)
            {
                var rootNode = fullSeries.GetRuleNodeFromFullSeries(ruleVocabSymbol.Source);//fullSeries.Where(k => k.Key is SyntacticalDFARootState && ((SyntacticalDFARootState)(k.Key)).Entry == ruleVocabSymbol.Source).Single().Key;
                rootNode.HasBeenReduced = true;
            }
            lock (this.reductionTree)
                if (!this.reductionTree.TryGetValue(reducedRule, deviationLevel, out result))
                    this.reductionTree.Add(reducedRule, deviationLevel, result = new ProductionRuleProjectionReduction() { ReducedRule = reducedRule, LookAheadDepth = deviationLevel, Rule = this.RootNode.Rule });
            return result;
        }

        internal int BucketCount
        {
            get
            {
                lock (this.laBucketLock)
                    return this.bucketCount;
            }
            set
            {
                lock (this.laBucketLock)
                    this.bucketCount = value;
            }
        }

        public int GetNewLookAheadBucket()
        {
            lock (this.laBucketLock)
                return ++this.bucketCount;
        }


        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionFollow"/>
        /// which denotes the series of edge paths from the call chain 
        /// that originated the parent rule.
        /// </summary>
        public IEnumerable<ProductionRuleProjectionFollow> FollowAmbiguities
        {
            get
            {
                foreach (var ambiguitySet in this.followAmbiguities)
                    foreach (var ambiguity in ambiguitySet.Value)
                        yield return ambiguity;
            }
        }

        /// <summary>
        /// Denotes an ambiguous context which identifies the current node as consistent within the ambiguity.  
        /// </summary>
        /// <param name="ambiguityContext">
        /// The <see cref="ProductionRuleProjectionDPathSet"/> which represents the ambiguous context, or the 'paths' from which the rule of the current node
        /// was entered.</param>
        /// <remarks>
        /// A reduction on the rule of the node will take place, the ambiguous context needs identified so follow-based ambiguities can be isolated and handled.
        /// </remarks>
        public void DenoteReductionPoint(ProductionRuleProjectionDPathSet ambiguityContext)
        {
            lock (this.ambiguityContexts)
                this.ambiguityContexts.Add(ambiguityContext);
            AddRootReduction(ambiguityContext);
        }
        /// <summary>
        /// Denotes an ambiguous context which identifies the current node as consistent within the ambiguity.  
        /// </summary>
        /// <param name="ambiguityContext">
        /// The <see cref="ProductionRuleProjectionDPathSet"/> which represents the ambiguous context, or the 'paths' from which the rule of the current node
        /// was entered.</param>
        /// <remarks>
        /// A reduction on the rule of the node will take place, the ambiguous context needs identified so follow-based ambiguities can be isolated and handled.
        /// </remarks>
        public void DenoteReductionPoint(ProductionRuleProjectionDPathSet ambiguityContext, ProductionRuleProjectionDPathSet declarationContext)
        {
            var entry = new ProductionRuleProjectionReductionDetail { Entrypoint = declarationContext, ReductionPoint = ambiguityContext };
            lock (this.steppedAmbiguityContexts)
                this.steppedAmbiguityContexts.Add(entry);
            AddRootReduction(declarationContext);
        }

        private void AddRootReduction(ProductionRuleProjectionDPathSet entry)
        {
            var currentSet = new HashList<HashList<ProductionRuleProjectionNode>>();
            lock (this.RootNode.rootAmbiguityContexts)
            {
                foreach (var path in entry)
                {
                    var currentItem = new HashList<ProductionRuleProjectionNode>(path.Skip(path.MinDepth).Take(path.Depth - (path.MinDepth - 1)));
                    currentSet.Add(currentItem);
                }
                this.RootNode.rootAmbiguityContexts.Add(currentSet);
            }
        }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> which specifies the ambiguous contexts on which the rule the current node exists within.
        /// </summary>
        /// <remarks>
        /// Used to generate possible additional state-machines for follow ambiguity resolution.  Specifically when a prediction path reduces
        /// a given rule, but is itself further ambiguous upon what follows the reduced rule.
        /// </remarks>
        public HashSet<ProductionRuleProjectionDPathSet> AmbiguousReduceContexts { get { return this.ambiguityContexts; } }
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> which denotes the ambiguous contexts which have consumed input in order to get to the reduction point.
        /// </summary>
        public HashSet<ProductionRuleProjectionReductionDetail> AmbiguousReduceSteppedContexts { get { return this.steppedAmbiguityContexts; } }
        public HashSet<HashList<HashList<ProductionRuleProjectionNode>>> RootAmbiguityContexts { get { return this.rootAmbiguityContexts; } }

        internal void DenoteLexicalAmbiguity(IGrammarAmbiguousSymbol ambiguity)
        {
            if (this._lexicalAmbiguities == null)
                this._lexicalAmbiguities = new HashSet<IGrammarAmbiguousSymbol>();
            this._lexicalAmbiguities.Add(ambiguity);
        }

        public IEnumerable<IGrammarAmbiguousSymbol> LexicalAmbiguities
        {
            get
            {
                var temp = this._LexicalAmbiguities.ToArray();
                if (temp.Length == 0)
                    return new IGrammarAmbiguousSymbol[0];
                else
                    return temp.Distinct();
            }
        }
        
        private HashSet<IGrammarAmbiguousSymbol> _lexicalAmbiguities;

        private IEnumerable<IGrammarAmbiguousSymbol> _LexicalAmbiguities
        {
            get
            {
                if (this._lexicalAmbiguities != null)
                    foreach (var symbol in this._lexicalAmbiguities)
                        yield return symbol;
                foreach (var node in this.Values)
                    if (node._lexicalAmbiguities != null)
                        foreach (var ambiguity in node._lexicalAmbiguities)
                            yield return ambiguity;
            }
        }
    }

    public class ProductionRuleProjectionReductionDetail
    {
        public ProductionRuleProjectionDPathSet Entrypoint { get; set; }
        public ProductionRuleProjectionDPathSet ReductionPoint { get; set; }
    }
}
