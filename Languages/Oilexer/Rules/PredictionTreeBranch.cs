using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Utilities.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
#if x64
using SlotType = System.UInt64;
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeBranch :
        ControlledCollection<PredictionTreeLeaf>
    {
        private int[] epsilonDerivations;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PredictionTreeBranch next;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PredictionTreeBranch previous;
        private static int instanceTrackerMain = 0;
        private short[] deviations;
        internal int instanceTracker = instanceTrackerMain++;
        
        public int MinDepth { get; private set; }
        public int Depth { get; set; }

        //private MultikeyedDictionary<int, int, List<PredictionTreeLeaf>, PredictionTreeBranch> cache = new MultikeyedDictionary<int,int,List<PredictionTreeLeaf>,PredictionTreeBranch>();

        public PredictionTreeBranch() { }
        protected internal PredictionTreeBranch(IList<PredictionTreeLeaf> baseList, int depth = 0, bool chop = false, bool passAlong = true, short[] deviations = null, int[] epsilonState = null, int minDepth = 0)
            : base(baseList)
        {
            this.epsilonDerivations = epsilonState ?? new int[(baseList.Count + (sizeof(SlotType) - 1)) / sizeof(SlotType)];
            this.MinDepth = minDepth;
            this.Depth = depth;
            this.SetDeviations(deviations ?? new short[0]);
            if (passAlong)
                if (!chop && depth > 0)
                {
                    if (depth == this.Count - 1)
                        baseList[depth].AddPath(this);
                    else
                    {
                        var addedPath = new PredictionTreeBranch(this.Take(depth + 1).ToList(), depth, true, false);
                        addedPath.SetDeviations(this.deviations);
                        baseList[depth].AddPath(addedPath);
                    }
                }
            this.Validate();
        }

        private void Validate()
        {
            /* *
             * Simple check verifying whether the calling parent 
             * node actually contains the current node's rule.
             * */
            if (this.Depth == 0)
                return;
            if (this.Depth >= this.Count)
            {
                this.Valid = false;
                return;
            }
            var currentNode = this.CurrentNode;
            var previousNode = this[this.Depth - 1];
            this.Valid = previousNode.ContainsKey(currentNode.Rule);
        }

        public void SetDeviations(short[] deviations)
        {
            this.deviations = new short[this.Count];
            Array.ConstrainedCopy(deviations, 0, this.deviations, 0, Math.Min(this.Count, deviations.Length));
        }

        public short GetCurrentDeviation()
        {
            return this.deviations[this.Depth];
        }

        public int GetDeviationToCurrent()
        {
            int result = 0;
            for (int index = 0; index < this.Depth; index++)
                result += this.deviations[index];
            return result;
        }

        internal int GetDeviationsAtAndBeyondCurrent(int depth)
        {
            int result = 0;
            for (int index = this.Depth; index < this.Count; index++)
                result += this.deviations[index];
            return result;
        }
        public int OverallDeviation()
        {
            int result = 0;
            for (int index = 0; index < this.Count; index++)
                result += this.deviations[index];
            return result;
        }

        public short GetDeviationAt(int depth)
        {
            if (depth < 0 || depth >= this.Count)
                throw new ArgumentOutOfRangeException("depth");
            return this.deviations[depth];
        }


        public short[] GetDeviationsUpTo(int depth)
        {
            if (depth < 0 || depth >= this.Count)
                throw new ArgumentOutOfRangeException("depth");
            short[] result = new short[depth + 1];
            Array.ConstrainedCopy(this.deviations, 0, result, 0, depth + 1);
            return result;
        }

        public PredictionTreeLeaf CurrentNode
        {
            get
            {
                return this[Depth];
                //return this.Skip(Depth).FirstOrDefault();
            }
        }

        public PredictionTreeBranch DepthAt(int index, bool passAlong = true)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException("index");
            var top = this.GetTopLevel(passAlong);
            for (int i = MinDepth; i <= index && top != null; i++, top = top.Depth == top.Count - 1 ? null : top.IncreaseDepth(passAlong))
            {
                if (top != null)
                {
                    if (i == index)
                        return top;
                }
                else
                    break;

            }
            return null;
        }

        public bool Equals(PredictionTreeBranch other)
        {
            if (other == null)
                return false;
            if (other.Depth != this.Depth || other.MinDepth != this.MinDepth)
                return false;
            if (this.CurrentNode != other.CurrentNode)
                return false;
            if (this.Count != other.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
                if (this[i] != other[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Returns whether the <see cref="CurrentNode"/>'s States Transition Table
        /// collides with the <paramref name="collider"/> provided.
        /// </summary>
        /// <param name="collider">The <see cref="GrammarVocabulary"/>
        /// to check for collisions with.</param>
        /// <returns>true, if the <paramref name="collider"/> collides with the 
        /// transition table.</returns>
        public bool CollidesWith(GrammarVocabulary collider)
        {
            if (this.CurrentNode == null ||
                collider == null ||
                collider.IsEmpty)
                return false;
            return !this.CurrentNode.Veins.DFAOriginState.OutTransitions.FullCheck.Intersect(collider).IsEmpty;
        }

        public override int GetHashCode()
        {
            int result = this.Depth | this.MinDepth << 16;
            //foreach (var element in this)
            //    result ^= element.GetHashCode();
            return result ^ this.Count;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PredictionTreeBranch);
        }

        public IEnumerable<PredictionTreeBranch> ObtainEpsilonTransitions(IDictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries)
        {
            var resultPaths = new Stack<PredictionTreeBranch>();
            resultPaths.Push(this);
            var observed = new List<PredictionTreeBranch>();
            while (resultPaths.Count > 0)
            {
                var currentPath = resultPaths.Pop();
                if (observed.Contains(currentPath))
                    continue;
                observed.Add(currentPath);
                if (currentPath.Depth == currentPath.MinDepth)
                    continue;
                if (currentPath.CurrentNode.Veins.DFAOriginState.IsEdge)
                {
                    var prevNode = currentPath.DecreaseDepth();
                    var r = prevNode.FollowEpsilonTransition(currentPath.CurrentNode.Veins.Rule, fullSeries);
                    foreach (var element in r)
                    {
                        resultPaths.Push(element);
                        yield return element;
                    }
                }
            }
        }

        private IEnumerable<PredictionTreeBranch> FollowEpsilonTransitionInternal(IOilexerGrammarProductionRuleEntry rule, IDictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries)
        {
            var collidingNodeKeys = (from t in this.CurrentNode.Veins.DFAOriginState.OutTransitions.Keys
                                     from s in t.GetSymbols()
                                     let rS = s as IGrammarRuleSymbol
                                     where rS != null && rS.Source == rule
                                     let targetOfTransition = this.CurrentNode.Veins.DFAOriginState.OutTransitions[t]
                                     select new { FullTransitionKey = t, Target = fullSeries[targetOfTransition] }).ToArray();
            if (collidingNodeKeys.Length > 0)
            {
                foreach (var key in collidingNodeKeys)
                {
                    var currentPathSet = key.Target;
                    var transitionedElements = (from pathKey in currentPathSet.Veins.Keys
                                                from path in currentPathSet.Veins[pathKey].UnalteredOriginals
                                                let newPath = GetEpsilonTransitionPath(path)// { MinDepth = this.Depth }
                                                select newPath).ToArray();
                    foreach (var element in transitionedElements)
                    {
                        yield return element;
                    }
                }
            }
        }

        private PredictionTreeBranch GetEpsilonTransitionPath(PredictionTreeBranch path)
        {
            short[] deviations = this.GetDeviationsUpTo(this.Depth);//IncrementDeviations(this.Depth, false);
            var result = new PredictionTreeBranch(this.Take(this.Depth).Concat(path).ToArray(), this.Depth, passAlong: false, deviations: deviations, epsilonState: this.epsilonDerivations.ToList().ToArray());
            result.SetEpsilonDerived(result.Depth);
            return result;
        }
        public IEnumerable<PredictionTreeBranch> FollowEpsilonTransition(IOilexerGrammarProductionRuleEntry rule, IDictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries)
        {
            return FollowEpsilonTransitionInternal(rule, fullSeries).Distinct();
        }

        /// <summary>
        /// Returns the <see cref="PredictionTreeBranch"/> which is
        /// one level deeper.
        /// </summary>
        /// <returns>A <see cref="PredictionTreeBranch"/> instance
        /// which represents a single level deeper within
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to <see cref="IControlledCollection{T}.Count"/> - 1.</exception>
        public PredictionTreeBranch IncreaseDepth(bool passAlong = true)
        {
            if (Depth >= Count - 1)
                throw new InvalidOperationException();
            if (this.next == null)
                this.next = new PredictionTreeBranch(this.baseList, this.Depth + 1, passAlong: passAlong, deviations: this.deviations, epsilonState: this.epsilonDerivations) { previous = this };
            return this.next;
        }

        /// <summary>
        /// Returns the <see cref="PredictionTreeBranch"/> which is
        /// one level above.
        /// </summary>
        /// <returns>A <see cref="PredictionTreeBranch"/> instance
        /// which represents a single level above 
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to <see cref="MinDepth"/>.</exception>
        public PredictionTreeBranch DecreaseDepth(bool passAlong = true)
        {
            if (this.Depth <= MinDepth)
                throw new InvalidOperationException();
            if (this.previous == null)
                this.previous = new PredictionTreeBranch(this.baseList, this.Depth - 1, passAlong: passAlong, deviations: this.deviations, epsilonState: this.epsilonDerivations) { next = this };
            return this.previous;
        }

        /// <summary>
        /// Yields the top-most <see cref="PredictionTreeBranch"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.
        /// </summary>
        /// <returns>A <see cref="PredictionTreeBranch"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.</returns>
        /// <remarks>Will not return null, and 
        /// may return itself.</remarks>
        public PredictionTreeBranch GetTopLevel(bool passAlong = true)
        {
            var current = this;
            while (current.Depth > this.MinDepth)
                current = current.DecreaseDepth(passAlong);
            return current;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> instance
        /// which obtains all possible depth variants of the
        /// current <see cref="PredictionTreeBranch"/>.
        /// </summary>
        public IEnumerable<PredictionTreeBranch> GetAllDepths()
        {
            var topLevel = this.GetTopLevel();
            while (topLevel.Depth < topLevel.Count)
            {
                yield return topLevel;
                if (topLevel.Depth + 1 >= topLevel.Count)
                    yield break;
                topLevel = topLevel.IncreaseDepth();
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> instance
        /// which obtains all possible depth variants of the
        /// current <see cref="PredictionTreeBranch"/>.
        /// </summary>
        public IEnumerable<PredictionTreeBranch> GetAllDepthsFromCurrent(bool passAlong = true)
        {
            var current = this;
            while (current.Depth < current.Count)
            {
                yield return current;
                if (current.Depth + 1 >= current.Count)
                    yield break;
                current = current.IncreaseDepth(passAlong);
            }
        }

        public override string ToString()
        {
            return ToStringInternal();
        }

        public string ToString(bool includePastDepth)
        {
            return ToStringInternal(includePastDepth);
        }

        private string ToStringInternal(bool includePastDepth = true)
        {
            const string epsGlpyh = "<ε>";
            if (this.CurrentNode == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            bool first = true;
            var currentInst = this.GetTopLevel(false);
            int deviationIndex = 0;
            if (currentInst.Depth > 0)
            {
                var subElements = currentInst.Take(currentInst.Depth);
                foreach (var subElement in subElements)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append("::");
                    short deviation = this.deviations[deviationIndex];
                    bool isEdge = subElement.Veins.DFAOriginState.IsEdge;
                    bool eps = this.IsEpsilonDerived(deviationIndex++);
                    if (subElement.Veins.IsRuleEntryPoint)
                        if (deviation > 0)
                            sb.AppendFormat("{3}{0}[{1}]{2}", subElement.Veins.Rule.Name, deviations[deviationIndex], isEdge ? "*" : string.Empty, eps ? epsGlpyh : string.Empty);
                        else
                            sb.Append(subElement.Veins.Rule.Name);
                    else
                        if (deviation > 0)
                            sb.AppendFormat("{4}{0}({1})[{2}]{3}", subElement.Veins.Rule.Name, subElement.Veins.DFAOriginState.StateValue, deviation, isEdge ? "*" : string.Empty, eps ? epsGlpyh : string.Empty);
                        else
                            sb.AppendFormat("{3}{0}({1}){2}", subElement.Veins.Rule.Name, subElement.Veins.DFAOriginState.StateValue, isEdge ? "*" : string.Empty, eps ? epsGlpyh : string.Empty);
                }
            }
            while (currentInst.Depth < this.Count)
            {
                if (first)
                    first = false;
                else
                    sb.Append("::");
                if (currentInst == this)
                    sb.Append("[");

                sb.Append(currentInst.CurrentNode.Veins.Rule.Name);
                short deviation = this.deviations[deviationIndex];
                bool isEdge = currentInst.CurrentNode.Veins.DFAOriginState.IsEdge;
                bool eps = this.IsEpsilonDerived(deviationIndex);
                if (!currentInst.CurrentNode.Veins.IsRuleEntryPoint)
                    sb.AppendFormat("{3}({0}){1}{2}", currentInst.CurrentNode.Veins.DFAOriginState.StateValue, deviation == 0 ? string.Empty : string.Format("[{0}]", deviation), isEdge ? "*" : string.Empty, eps ? epsGlpyh : string.Empty);
                else if (deviation > 0)
                    sb.AppendFormat("{2}[{0}]{1}", deviation, isEdge ? "*" : string.Empty, eps ? epsGlpyh : string.Empty);
                if (currentInst == this)
                    sb.Append("]");
                if (currentInst.Depth + 1 >= this.Count)
                    break;
                if (currentInst == this && !includePastDepth)
                    break;
                currentInst = currentInst.IncreaseDepth(false);
                deviationIndex++;
            }
            return sb.ToString();
        }

        internal FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, PredictionTreeBranch> 
            FollowTransition(
            Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries,
            GrammarVocabulary transitionKey,
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, 
            bool usePrevious, 
            bool injectIncomingPaths, 
            bool includeRootEdges = false,
            bool isEpsilonTransition = false,
            bool silentlyFail = false,
            bool isReduction = false)
        {
            var resultData = from u in FollowTransitionInternal(fullSeries, transitionKey, ruleLookup, usePrevious, isEpsilonTransition, silentlyFail, isReduction)
                             group u.Item2 by u.Item1;
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, PredictionTreeBranch> result = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, PredictionTreeBranch>();
            foreach (var element in resultData)
            {
                if (element.Key.IsEmpty)
                    result.SetAutoSegment(false);
                result.Add(element.Key, element.ToList());
                if (element.Key.IsEmpty)
                    result.SetAutoSegment(true);
            }
            return result;
        }

        internal IEnumerable<Tuple<GrammarVocabulary, PredictionTreeBranch>>
            FollowTransitionInternal(
                Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries,
                GrammarVocabulary transitionKey,
                Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup,
                bool usePrevious, 
                bool isEpsilonTransition = false,
                bool silentlyFail = false,
                bool isReduction = false)
        {
            if (transitionKey.IsEmpty)
                return new Tuple<GrammarVocabulary, PredictionTreeBranch>[0];


            List<Tuple<GrammarVocabulary, PredictionTreeBranch>> result = new List<Tuple<GrammarVocabulary, PredictionTreeBranch>>();
            /* *
             * The transition was broken up earlier to yield
             * subsets that overlapped on a portion of the
             * transition.
             * */
            var ambiguityVocabulary = transitionKey.DisambiguateVocabulary();
            short[] deviationsToPass = this.deviations;
            if (isReduction)
                deviationsToPass = this.IncrementDeviations(this.Depth, false);
            var targetPath = usePrevious ? this.Depth == this.MinDepth && this.Depth > 0 ? new PredictionTreeBranch(this.baseList, this.Depth - 1, false, false, deviationsToPass, this.epsilonDerivations, this.MinDepth - 1) : this.DecreaseDepth(false) : this;
            if (isReduction)
                targetPath = new PredictionTreeBranch(targetPath.baseList, targetPath.Depth, false, false, deviationsToPass, this.epsilonDerivations, targetPath.MinDepth);
            var fullTransitionKey = targetPath.CurrentNode.Veins.DFAOriginState.OutTransitions.Keys.FirstOrDefault(v => !v.Intersect(transitionKey.DisambiguateVocabulary()).IsEmpty);
            var cachedRequest = GetTransitionRequest(transitionKey, targetPath, usePrevious, isReduction);
            if (cachedRequest != null)
                return cachedRequest.Detail;
            if (fullTransitionKey == null && !silentlyFail)
                Debug.Assert(fullTransitionKey != null);
            else if (fullTransitionKey == null && silentlyFail)
                return result;
            var targetOfTransition = targetPath.CurrentNode.Veins.DFAOriginState.OutTransitions[fullTransitionKey];

            var targetNodeOfTransition = fullSeries[targetOfTransition];
            PredictionTreeLeaf[] previousNodes = null;
            if (targetPath.Depth > 0)
                previousNodes = targetPath.Take(targetPath.Depth).ToArray();
            else
                previousNodes = new PredictionTreeLeaf[0];
            if (targetNodeOfTransition.Veins.Keys.Count > 0)
            {
                foreach (var key in targetNodeOfTransition.Veins.Keys)
                {
                    var tokenVar = key.GetTokenVariant();
                    var originalSet = key.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).ToArray();
                    var reducedRules = originalSet.Where(k => NodeIsRelevantToThis(k.Node, targetNodeOfTransition)).Select(k => k.Symbol).ToArray();
                    var ruleVar = new GrammarVocabulary(key.symbols, reducedRules);
                    foreach (var path in targetNodeOfTransition.Veins[key].UnalteredOriginals)
                    {
                        var currentSubPath = path;
                        /* *
                         * The transition is derived from the parent node's transitions because the current node is omittable.
                         * */
                        while (!currentSubPath.CurrentNode.Veins.Keys.Any(k => !k.Intersect(key).IsEmpty) && currentSubPath.Depth > 0)
                            currentSubPath = currentSubPath.DecreaseDepth(false);
                        if (!tokenVar.IsEmpty)
                            result.Add(Tuple.Create(tokenVar, GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition)));
                        if (!ruleVar.IsEmpty)
                            result.Add(Tuple.Create(ruleVar, GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition)));
                    }
                }
                if (targetOfTransition.IsEdge)
                {
                    HandleLeftRecursiveInjection(fullSeries, ruleLookup, usePrevious, isEpsilonTransition, silentlyFail, result, targetPath, targetNodeOfTransition, previousNodes);
                    var result2 = result.ToArray().ToList();
                    if (previousNodes.Length > 0)
                        result.AddRange(GetTransitionPath(targetPath, previousNodes, targetNodeOfTransition, ruleLookup, usePrevious, isEpsilonTransition, false).FollowTransitionInternal(fullSeries, ruleLookup[targetNodeOfTransition.Veins.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
                    else
                        result.Add(Tuple.Create(GrammarVocabulary.NullInst, new PredictionTreeBranch(new[] { targetNodeOfTransition }, passAlong: false)));
                }
            }
            else if (targetOfTransition.IsEdge && previousNodes.Length > 0)
            {
                HandleLeftRecursiveInjection(fullSeries, ruleLookup, usePrevious, isEpsilonTransition, silentlyFail, result, targetPath, targetNodeOfTransition, previousNodes);
                result.AddRange(targetPath.FollowTransitionInternal(fullSeries, ruleLookup[targetNodeOfTransition.Veins.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
            }
            else if (targetOfTransition.IsEdge && previousNodes.Length == 0/* && includeRootEdges*/)
                result.Add(Tuple.Create(GrammarVocabulary.NullInst, new PredictionTreeBranch(new[] { targetNodeOfTransition }, passAlong:false)));
            var transitionDetail = PushTransitionRequest(transitionKey, targetPath, usePrevious, isReduction);
            if (transitionDetail.Detail == null)
                transitionDetail.Detail = result;
            return result;
        }

        private void HandleLeftRecursiveInjection(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool isEpsilonTransition, bool silentlyFail, List<Tuple<GrammarVocabulary, PredictionTreeBranch>> result, PredictionTreeBranch targetPath, PredictionTreeLeaf targetNodeOfTransition, PredictionTreeLeaf[] previousNodes)
        {
            if (targetNodeOfTransition.RootLeaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None && targetNodeOfTransition.RootLeaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.Direct/* && !isReduction*/)
            {
                var sPaths = targetNodeOfTransition.RootLeaf.IncomingPaths.Where(k => k.Depth > 0 && k.CurrentNode == targetNodeOfTransition.RootLeaf && k.MinDepth == targetPath.MinDepth && k[0] == targetNodeOfTransition.RootLeaf).ToArray();
                var transitionPaths =
                    (from p in sPaths
                     let nPath = GetTransitionPath(targetPath, previousNodes, p, ruleLookup, usePrevious, isEpsilonTransition, false)
                     where IsSafeToExpand(nPath)
                     select nPath).ToArray();
                foreach (var nPath in transitionPaths)
                    result.AddRange(nPath.FollowTransitionInternal(fullSeries, ruleLookup[targetNodeOfTransition.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition, silentlyFail: silentlyFail));
            }
        }

        private bool IsSafeToExpand(PredictionTreeBranch nPath)
        {
            Dictionary<IOilexerGrammarProductionRuleEntry, int> counts = new Dictionary<IOilexerGrammarProductionRuleEntry, int>();
            //var currentRoot = nPath.CurrentNode.RootLeaf;
            //int currentCount = 1;
            for (int i = nPath.Depth - 1; i >= 0; i--)
            {

                var currentNode = nPath[i];
                if (currentNode.RootLeaf != currentNode)
                    return true;
                if (counts.ContainsKey(currentNode.Rule))
                {
                    var value = ++counts[currentNode.Rule];
                    if (value > 2)
                        return false;
                }
                else
                    counts[currentNode.Rule] = 1;

                //if (currentNode.RootLeaf == currentRoot)
                //    currentCount++;
            }
            return true;
            //return currentCount <= 2;
        }

        private static bool NodeIsRelevantToThis(PredictionTreeLeaf node, PredictionTreeLeaf relevantNode)
        {
            lock (node.RootAmbiguityContexts)
                return node.RootAmbiguityContexts
                    .Any(k => 
                        k
                        .Any(v => 
                            v.Contains(relevantNode)));
        }

        internal PredictionTreeBranch GetTransitionPath(PredictionTreeBranch targetPath, PredictionTreeLeaf[] previousNodes, PredictionTreeBranch currentSubPath, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool isEpsilonTransition = false, bool incrementDeviations = true)
        {
            var currentNode = currentSubPath[0];
            if (previousNodes.Length > 0)
                Debug.Assert(previousNodes[previousNodes.Length - 1].Veins.DFAOriginState.OutTransitions.Keys.Any(k => !k.Intersect(ruleLookup[currentNode.Rule]).IsEmpty));
            short[] deviations = incrementDeviations ? targetPath.IncrementDeviations(previousNodes.Length, isEpsilonTransition) : targetPath.GetDeviationsUpTo(previousNodes.Length);
            var result = new PredictionTreeBranch(previousNodes.Concat(currentSubPath).ToArray(), targetPath.Depth + currentSubPath.Depth, passAlong: false, deviations: deviations);
            result.MinDepth = targetPath.MinDepth;
            return result;
        }

        internal PredictionTreeBranch GetTransitionPath(PredictionTreeBranch targetPath, PredictionTreeLeaf[] previousNodes, PredictionTreeLeaf currentNode, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool isEpsilonTransition = false, bool incrementDeviations = true)
        {
            if (previousNodes.Length > 0)
                Debug.Assert(previousNodes[previousNodes.Length - 1].Veins.DFAOriginState.OutTransitions.Keys.Any(k => !k.Intersect(ruleLookup[currentNode.Rule]).IsEmpty));
            short[] deviations = incrementDeviations ? IncrementDeviations(previousNodes.Length, isEpsilonTransition) : this.GetDeviationsUpTo(previousNodes.Length);

            var result = new PredictionTreeBranch(previousNodes.Concat(new[] { currentNode }).ToArray(), targetPath.Depth, passAlong: false, deviations: deviations);
            result.MinDepth = targetPath.MinDepth;
            return result;
        }

        private short[] IncrementDeviations(int depth, bool isEpsilonTransition)
        {
            short[] deviations = this.GetDeviationsUpTo(depth);
            if (!isEpsilonTransition)
                for (int d = 0; d <= depth; d++)
                    deviations[d]++;
            return deviations;
        }

        public PredictionTreeBranch GetBottomLevel(bool passAlong = true)
        {
            return this.DepthAt(this.Count - this.MinDepth, passAlong);
        }

        public ProductionRuleLeftRecursionType GetRecursionType()
        {
            if (this.Count == 0)
                return ProductionRuleLeftRecursionType.None;
            var first = this[0];
            if (!first.Veins.IsRuleEntryPoint)
                return ProductionRuleLeftRecursionType.None;
            /* *
             * Iterate through the sequence, if we encounter
             * one which points to the same rule node, we have
             * a left-recursive cycle.
             * * 
             * ToDo: Add case to handle epsilon transitions which
             * cause left-recursion. AKA: Hidden Left Recursion.
             * */
            ProductionRuleLeftRecursionType result = ProductionRuleLeftRecursionType.None;
            for (int i = 1; i < this.Count; i++)
            {
                var currentNode = this[i];
                if (!currentNode.Veins.IsRuleEntryPoint && !(this.IsEpsilonDerived(i) && currentNode.RootLeaf == first))
                    return result;
                if (currentNode == first || this.IsEpsilonDerived(i) && currentNode.RootLeaf == first)
                    result |= (i == 1 ? ProductionRuleLeftRecursionType.Direct : ProductionRuleLeftRecursionType.Indirect);
            }
            return result;
        }

        public PredictionTreeBranch GetFirstRecursion(bool passAlong = true)
        {
            if (this.Count == 0)
                return null;
            var first = this[0];
            if (!first.Veins.IsRuleEntryPoint)
                return null;
            /* *
             * Iterate through the sequence, if we encounter
             * one which points to the same rule node, we have
             * a left-recursive cycle.
             * * 
             * ToDo: Add case to handle epsilon transitions which
             * cause left-recursion. AKA: Hidden Left Recursion.
             * */
            for (int i = 1; i < this.Count; i++)
            {
                var currentNode = this[i];
                if (!currentNode.Veins.IsRuleEntryPoint && !(this.IsEpsilonDerived(i) && currentNode.RootLeaf == first))
                    return null;
                if (currentNode == first || this.IsEpsilonDerived(i) && currentNode.RootLeaf == first)
                    return this.DepthAt(i, passAlong);
            }
            return null;
        }

        public bool RecursesIntoRoot
        {
            get
            {
                if (this.Count == 0)
                    return false;
                var first = this.CurrentNode.RootLeaf;
                for (int i = this.Depth + 1; i < this.Count; i++)
                {
                    var currentNode = this[i];
                    if (!currentNode.Veins.IsRuleEntryPoint && !(this.IsEpsilonDerived(i) && currentNode.RootLeaf == first))
                        return false;
                    if (currentNode == first || this.IsEpsilonDerived(i) && currentNode.RootLeaf == first)
                        return true;
                }
                return false;
            }
        }

        public bool IsEpsilonDerived(int index)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException("index");
            return (epsilonDerivations[index / sizeof(SlotType)] & (1 << (index % sizeof(SlotType)))) != 0;
        }
        private void SetEpsilonDerived(int index)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException("index");
            epsilonDerivations[index / sizeof(SlotType)] |= (1 << (index % sizeof(SlotType)));
        }

        internal static Func<ProductionRuleLeftRecursionType, ProductionRuleLeftRecursionType, ProductionRuleLeftRecursionType> LeftRecursionAggregateDelegate = (a, b) =>
            {
                return a | b;
            };

        private static MultikeyedDictionary<Tuple<bool, bool>, GrammarVocabulary, PredictionTreeBranch, List<TransitionRequest>> transitionRequests = new MultikeyedDictionary<Tuple<bool, bool>, GrammarVocabulary, PredictionTreeBranch, List<TransitionRequest>>();

        private static TransitionRequest GetTransitionRequest(GrammarVocabulary transition, PredictionTreeBranch path, bool usePrevious, bool isReduction)
        {
            List<TransitionRequest> result;
            var fKey = Tuple.Create(usePrevious, isReduction);
            lock (transitionRequests)
                if (transitionRequests.TryGetValue(fKey, transition, path, out result))
                    lock (result)
                        return result.FirstOrDefault(p => p.DeviationDetail.SequenceEqual(path.deviations));
            return null;
        }

        private static TransitionRequest PushTransitionRequest(GrammarVocabulary transition, PredictionTreeBranch path, bool usePrevious, bool isReduction)
        {
            var fKey = Tuple.Create(usePrevious, isReduction);
            lock (transitionRequests)
            {
                List<TransitionRequest> resultSet;
                if (!transitionRequests.TryGetValue(fKey, transition, path, out resultSet))
                    transitionRequests.Add(fKey, transition, path, resultSet = new List<TransitionRequest>());
                lock (resultSet)
                {
                    TransitionRequest result = resultSet.FirstOrDefault(p => p.DeviationDetail.SequenceEqual(path.deviations));
                    if (result == null)
                        resultSet.Add(result = new TransitionRequest(transition, path, usePrevious, isReduction));
                    return result;
                }
            }
        }

        //private static TransitionRequest AddTransitionRequest(GrammarVocabulary transition, PredictionTreeBranch path, bool usePrevious, bool isReduction, MultikeyedDictionary<bool, bool, GrammarVocabulary, HashSet<TransitionRequest>> current)
        //{
        //    HashSet<TransitionRequest> resultSet;
        //    lock (current)
        //    {
        //        if (!current.TryGetValue(usePrevious, isReduction, transition, out resultSet))
        //            current.Add(usePrevious, isReduction, transition, resultSet = new HashSet<TransitionRequest>());
        //        lock (resultSet)
        //        {
        //            TransitionRequest result = resultSet.FirstOrDefault(p => p.DeviationDetail.SequenceEqual(path.deviations));
        //            if (result == null)
        //                resultSet.Add(result = new TransitionRequest(transition, path, usePrevious, isReduction));
        //            return result;
        //        }
        //    }
        //}

        private class TransitionRequest :
            IEquatable<TransitionRequest>
        {
            private PredictionTreeBranch path;

            private GrammarVocabulary transitionRequirement;
            private bool usePrevious;
            private bool isReduction;

            public short[] DeviationDetail { get { return this.path.deviations; } }

            public TransitionRequest(GrammarVocabulary transition, PredictionTreeBranch path, bool usePrevious, bool isReduction)
            {
                this.path = path;
                this.transitionRequirement = transition;
                this.usePrevious = usePrevious;
                this.isReduction = isReduction;
            }

            public List<Tuple<GrammarVocabulary, PredictionTreeBranch>> Detail { get; set; }

            public bool Equals(TransitionRequest other)
            {
                if (other != null && other.usePrevious == this.usePrevious && other.isReduction == this.isReduction && other.transitionRequirement.Equals(this.transitionRequirement) && other.path.Equals(this.path))
                {
                    //For speed, path equivalence doesn't compare deviations.
                    for (int i = 0; i < other.path.Count; i++)
                        if (other.path.deviations[i] != this.path.deviations[i])
                            return false;
                    return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                if (usePrevious)
                    if (isReduction)
                        return 0x101 ^ this.transitionRequirement.GetHashCode() ^ this.path.GetHashCode();
                    else
                        return 0x100 ^ this.transitionRequirement.GetHashCode() ^ this.path.GetHashCode();
                else
                    if (this.isReduction)
                        return 1 ^ this.transitionRequirement.GetHashCode() ^ this.path.GetHashCode();
                    else
                        return this.transitionRequirement.GetHashCode() ^ this.path.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TransitionRequest);
            }
        }

        //internal static FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<PredictionTreeBranch, PredictionTreeBranch>> FollowTransitionEx(PredictionTreeBranch origin, Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool injectIncomingPaths, bool includeRootEdges = false, bool isEpsilonTransition = false)
        //{
        //    var resultData = from u in FollowTransitionInternalEx(origin, fullSeries, transitionKey, ruleLookup, usePrevious, injectIncomingPaths, includeRootEdges, isEpsilonTransition)
        //                     let k = u.Item1
        //                     group new { Path = k.Item2, Cause = u.Item2 } by k.Item1;
        //    var result = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<PredictionTreeBranch, PredictionTreeBranch>>();
        //    foreach (var element in resultData)
        //    {
        //        if (element.Key.IsEmpty)
        //            result.SetAutoSegment(false);
        //        result.Add(element.Key, (from u in element.ToList()
        //                                 select Tuple.Create(u.Path, u.Cause)).ToList());
        //        if (element.Key.IsEmpty)
        //            result.SetAutoSegment(true);
        //    }
        //    return result;
        //}

        //internal static IEnumerable<Tuple<Tuple<GrammarVocabulary, PredictionTreeBranch>, PredictionTreeBranch>> FollowTransitionInternalEx(PredictionTreeBranch current, Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool inectIncomingPaths = true, bool includeRootEdges = false, bool isEpsilonTransition = false)
        //{
        //    if (transitionKey.IsEmpty)
        //        return new Tuple<Tuple<GrammarVocabulary, PredictionTreeBranch>, PredictionTreeBranch>[0];
        //    var result = new List<Tuple<Tuple<GrammarVocabulary, PredictionTreeBranch>, PredictionTreeBranch>>();
        //    /* *
        //     * The transition was broken up earlier to yield
        //     * subsets that overlapped on a portion of the
        //     * transition.
        //     * */
        //    var targetPath = usePrevious ? current.DecreaseDepth(!isEpsilonTransition) : current;
        //    var fullTransitionKey = targetPath.CurrentNode.Value.DFAOriginState.OutTransitions.Keys.FirstOrDefault(v => !v.Intersect(transitionKey).IsEmpty);
        //    if (fullTransitionKey == null)
        //    {
        //        Debug.Assert(fullTransitionKey != null);
        //    }
        //    var targetOfTransition = targetPath.CurrentNode.Value.DFAOriginState.OutTransitions[fullTransitionKey];
        //    var targetNodeOfTransition = fullSeries[targetOfTransition];
        //    PredictionTreeLeaf[] previousNodes = null;
        //    if (targetPath.Depth > 0)
        //        previousNodes = targetPath.Take(targetPath.Depth).ToArray();
        //    else
        //        previousNodes = new PredictionTreeLeaf[0];
        //    if (targetNodeOfTransition.Value.Keys.Count > 0)
        //    {
        //        foreach (var key in targetNodeOfTransition.Value.Keys)
        //        {
        //            var tokenVar = key.GetTokenVariant();
        //            foreach (var path in targetNodeOfTransition.Value[key].UnalteredOriginals)
        //            {
        //                var currentSubPath = path;
        //                /* *
        //                 * The transition is derived from the parent node's
        //                 * transitions because the current node is omittable.
        //                 * */
        //                while (!currentSubPath.CurrentNode.Value.Keys.Any(k => !k.Intersect(key).IsEmpty) && currentSubPath.Depth > 0)
        //                    currentSubPath = currentSubPath.DecreaseDepth();
        //                var transitionPath = current.GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition);
        //                var originalSet = key.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).ToArray();
        //                var reducedRules = originalSet.Where(k => NodeIsRelevantToThis(k.Node, targetNodeOfTransition)).Select(k => k.Symbol);
        //                var ruleVar = new GrammarVocabulary(key.symbols, reducedRules.ToArray());
        //                if (!tokenVar.IsEmpty)
        //                    result.Add(Tuple.Create(Tuple.Create(tokenVar, transitionPath), current));
        //                if (!ruleVar.IsEmpty)
        //                    result.Add(Tuple.Create(Tuple.Create(ruleVar, transitionPath), current));
        //            }
        //        }
        //        if (targetOfTransition.IsEdge)
        //        {
        //            var result2 = result.ToArray().ToList();
        //            if (previousNodes.Length > 0)
        //                result.AddRange(FollowTransitionInternalEx(current.GetTransitionPath(targetPath, previousNodes, targetNodeOfTransition, ruleLookup, usePrevious, isEpsilonTransition, false), fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
        //            else
        //                result.Add(Tuple.Create(Tuple.Create(GrammarVocabulary.NullInst, new PredictionTreeBranch(new[] { targetNodeOfTransition })), current));
        //        }
        //    }
        //    else if (targetOfTransition.IsEdge && previousNodes.Length > 0)
        //        result.AddRange(FollowTransitionInternalEx(targetPath, fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
        //    else if (targetOfTransition.IsEdge && previousNodes.Length == 0/* && includeRootEdges*/)
        //        result.Add(Tuple.Create(Tuple.Create(GrammarVocabulary.NullInst, new PredictionTreeBranch(new[] { targetNodeOfTransition })), current));
        //    return result;
        //}

        public bool Valid { get; private set; }
    }
}
