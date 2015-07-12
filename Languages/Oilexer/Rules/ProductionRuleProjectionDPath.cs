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
    public class ProductionRuleProjectionDPath :
        ControlledCollection<ProductionRuleProjectionNode>,
        IProductionRuleProjectionDPath<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>
    {
        private int[] epsilonDerivations;
        private ProductionRuleProjectionDPath next;
        private ProductionRuleProjectionDPath previous;
        private static int instanceTrackerMain = 0;
        private short[] deviations;
        internal int instanceTracker = instanceTrackerMain++;

        public int MinDepth { get; private set; }
        public int Depth { get; set; }

        //private MultikeyedDictionary<int, int, List<ProductionRuleProjectionNode>, ProductionRuleProjectionDPath> cache = new MultikeyedDictionary<int,int,List<ProductionRuleProjectionNode>,ProductionRuleProjectionDPath>();

        public ProductionRuleProjectionDPath() { }
        protected internal ProductionRuleProjectionDPath(IList<ProductionRuleProjectionNode> baseList, int depth = 0, bool chop = false, bool passAlong = true, short[] deviations = null, int[] epsilonState = null, int minDepth = 0)
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
                        var addedPath = new ProductionRuleProjectionDPath(this.Take(depth + 1).ToList(), depth, true);
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

        public ProductionRuleProjectionNode CurrentNode
        {
            get
            {
                return this.Skip(Depth).FirstOrDefault();
            }
        }

        public ProductionRuleProjectionDPath DepthAt(int index, bool passAlong = true)
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

        public bool Equals(ProductionRuleProjectionDPath other)
        {
            if (other == null)
                return false;
            if (other.Depth != this.Depth || other.MinDepth != this.MinDepth)
                return false;
            using (var myEnum = this.GetEnumerator())
            using (var otherEnum = other.GetEnumerator())
                for (; myEnum.MoveNext() & otherEnum.MoveNext(); )
                    if (otherEnum.Current != myEnum.Current)
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
            return !this.CurrentNode.Value.OriginalState.OutTransitions.FullCheck.Intersect(collider).IsEmpty;
        }

        public override int GetHashCode()
        {
            int result = this.Depth | this.MinDepth << 16;
            foreach (var element in this)
                result ^= element.GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ProductionRuleProjectionDPath);
        }

        public IEnumerable<ProductionRuleProjectionDPath> ObtainEpsilonTransitions(IDictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries)
        {
            var resultPaths = new Stack<ProductionRuleProjectionDPath>();
            resultPaths.Push(this);
            var observed = new List<ProductionRuleProjectionDPath>();
            while (resultPaths.Count > 0)
            {
                var currentPath = resultPaths.Pop();
                if (observed.Contains(currentPath))
                    continue;
                observed.Add(currentPath);
                if (currentPath.Depth == currentPath.MinDepth)
                    continue;
                if (currentPath.CurrentNode.Value.OriginalState.IsEdge)
                {
                    var prevNode = currentPath.DecreaseDepth();
                    var r = prevNode.FollowEpsilonTransition(currentPath.CurrentNode.Value.Rule, fullSeries);
                    foreach (var element in r)
                    {
                        resultPaths.Push(element);
                        yield return element;
                    }
                }
            }
        }

        private IEnumerable<ProductionRuleProjectionDPath> FollowEpsilonTransitionInternal(IOilexerGrammarProductionRuleEntry rule, IDictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries)
        {
            var collidingNodeKeys = (from t in this.CurrentNode.Value.OriginalState.OutTransitions.Keys
                                     from s in t.GetSymbols()
                                     let rS = s as IGrammarRuleSymbol
                                     where rS != null && rS.Source == rule
                                     let targetOfTransition = this.CurrentNode.Value.OriginalState.OutTransitions[t]
                                     select new { FullTransitionKey = t, Target = fullSeries[targetOfTransition] }).ToArray();
            if (collidingNodeKeys.Length > 0)
            {
                foreach (var key in collidingNodeKeys)
                {
                    var currentPathSet = key.Target;
                    var transitionedElements = (from pathKey in currentPathSet.Value.Keys
                                                from path in currentPathSet.Value[pathKey].UnalteredOriginals
                                                let newPath = GetEpsilonTransitionPath(path)// { MinDepth = this.Depth }
                                                select newPath).ToArray();
                    foreach (var element in transitionedElements)
                    {
                        yield return element;
                    }
                }
            }
        }

        private ProductionRuleProjectionDPath GetEpsilonTransitionPath(ProductionRuleProjectionDPath path)
        {
            short[] deviations = this.GetDeviationsUpTo(this.Depth);//IncrementDeviations(this.Depth, false);
            var result = new ProductionRuleProjectionDPath(this.Take(this.Depth).Concat(path).ToArray(), this.Depth, deviations: deviations, epsilonState: this.epsilonDerivations.ToList().ToArray());
            result.SetEpsilonDerived(result.Depth);
            return result;
        }
        public IEnumerable<ProductionRuleProjectionDPath> FollowEpsilonTransition(IOilexerGrammarProductionRuleEntry rule, IDictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries)
        {
            return FollowEpsilonTransitionInternal(rule, fullSeries).Distinct();
        }

        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionDPath"/> which is
        /// one level deeper.
        /// </summary>
        /// <returns>A <see cref="ProductionRuleProjectionDPath"/> instance
        /// which represents a single level deeper within
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to <see cref="IControlledCollection{T}.Count"/> - 1.</exception>
        public ProductionRuleProjectionDPath IncreaseDepth(bool passAlong = true)
        {
            if (Depth >= Count - 1)
                throw new InvalidOperationException();
            if (this.next == null)
                this.next = new ProductionRuleProjectionDPath(this.baseList, this.Depth + 1, passAlong: passAlong, deviations: this.deviations, epsilonState: this.epsilonDerivations) { previous = this };
            return this.next;
        }

        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionDPath"/> which is
        /// one level above.
        /// </summary>
        /// <returns>A <see cref="ProductionRuleProjectionDPath"/> instance
        /// which represents a single level above 
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to <see cref="MinDepth"/>.</exception>
        public ProductionRuleProjectionDPath DecreaseDepth(bool passAlong = true)
        {
            if (this.Depth <= MinDepth)
                throw new InvalidOperationException();
            if (this.previous == null)
                this.previous = new ProductionRuleProjectionDPath(this.baseList, this.Depth - 1, passAlong: passAlong, deviations: this.deviations, epsilonState: this.epsilonDerivations) { next = this };
            return this.previous;
        }

        /// <summary>
        /// Yields the top-most <see cref="ProductionRuleProjectionDPath"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.
        /// </summary>
        /// <returns>A <see cref="ProductionRuleProjectionDPath"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.</returns>
        /// <remarks>Will not return null, and 
        /// may return itself.</remarks>
        public ProductionRuleProjectionDPath GetTopLevel(bool passAlong = true)
        {
            var current = this;
            while (current.Depth > this.MinDepth)
                current = current.DecreaseDepth(passAlong);
            return current;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> instance
        /// which obtains all possible depth variants of the
        /// current <see cref="ProductionRuleProjectionDPath"/>.
        /// </summary>
        public IEnumerable<ProductionRuleProjectionDPath> GetAllDepths()
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
        /// current <see cref="ProductionRuleProjectionDPath"/>.
        /// </summary>
        public IEnumerable<ProductionRuleProjectionDPath> GetAllDepthsFromCurrent()
        {
            var current = this;
            while (current.Depth < current.Count)
            {
                yield return current;
                if (current.Depth + 1 >= current.Count)
                    yield break;
                current = current.IncreaseDepth();
            }
        }

        public override string ToString()
        {
            if (this.CurrentNode == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            bool first = true;
            var currentInst = this.GetTopLevel();
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
                    bool isEdge = subElement.Value.OriginalState.IsEdge;
                    bool eps = this.IsEpsilonDerived(deviationIndex++);
                    if (subElement.Value.IsRuleNode)
                        if (deviation > 0)
                            sb.AppendFormat("{3}{0}[{1}]{2}", subElement.Value.Rule.Name, deviations[deviationIndex], isEdge ? "*" : string.Empty, eps ? "ε" : string.Empty);
                        else
                            sb.Append(subElement.Value.Rule.Name);
                    else
                        if (deviation > 0)
                            sb.AppendFormat("{4}{0}({1})[{2}]{3}", subElement.Value.Rule.Name, subElement.Value.OriginalState.StateValue, deviation, isEdge ? "*" : string.Empty, eps ? "ε" : string.Empty);
                        else
                            sb.AppendFormat("{3}{0}({1}){2}", subElement.Value.Rule.Name, subElement.Value.OriginalState.StateValue, isEdge ? "*" : string.Empty, eps ? "ε" : string.Empty);
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

                sb.Append(currentInst.CurrentNode.Value.Rule.Name);
                short deviation = this.deviations[deviationIndex];
                bool isEdge = currentInst.CurrentNode.Value.OriginalState.IsEdge;
                bool eps = this.IsEpsilonDerived(deviationIndex);
                if (!currentInst.CurrentNode.Value.IsRuleNode)
                    sb.AppendFormat("{3}({0}){1}{2}", currentInst.CurrentNode.Value.OriginalState.StateValue, deviation == 0 ? string.Empty : string.Format("[{0}]", deviation), isEdge ? "*" : string.Empty, eps ? "ε" : string.Empty);
                else if (deviation > 0)
                    sb.AppendFormat("{2}[{0}]{1}", deviation, isEdge ? "*" : string.Empty, eps ? "ε" : string.Empty);
                if (currentInst == this)
                    sb.Append("]");
                if (currentInst.Depth + 1 >= this.Count)
                    break;
                currentInst = currentInst.IncreaseDepth();
                deviationIndex++;
            }
            return sb.ToString();
        }

        internal FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath> FollowTransition(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool injectIncomingPaths, bool includeRootEdges = false, bool isEpsilonTransition = false, bool silentlyFail = false)
        {
            var resultData = from u in FollowTransitionInternal(fullSeries, transitionKey, ruleLookup, usePrevious, injectIncomingPaths, includeRootEdges, isEpsilonTransition, silentlyFail)
                             group u.Item2 by u.Item1;
            FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath> result = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, ProductionRuleProjectionDPath>();
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

        internal IEnumerable<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>> FollowTransitionInternal(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool inectIncomingPaths = true, bool includeRootEdges = false, bool isEpsilonTransition = false, bool silentlyFail = false)
        {
            if (transitionKey.IsEmpty)
                return new Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>[0];
            List<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>> result = new List<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>>();
            /* *
             * The transition was broken up earlier to yield
             * subsets that overlapped on a portion of the
             * transition.
             * */
            var ambiguityVocabulary = transitionKey.DisambiguateVocabulary();
            var targetPath = usePrevious ? this.Depth == this.MinDepth && this.Depth > 0 ? new ProductionRuleProjectionDPath(this.baseList, this.Depth - 1, false, false, this.deviations, this.epsilonDerivations, this.MinDepth - 1) : this.DecreaseDepth(!isEpsilonTransition) : this;
            var fullTransitionKey = targetPath.CurrentNode.Value.OriginalState.OutTransitions.Keys.FirstOrDefault(v => !v.Intersect(transitionKey.DisambiguateVocabulary()).IsEmpty);
            if (fullTransitionKey == null && !silentlyFail)
                Debug.Assert(fullTransitionKey != null);
            else if (fullTransitionKey == null && silentlyFail)
                return result;
            var targetOfTransition = targetPath.CurrentNode.Value.OriginalState.OutTransitions[fullTransitionKey];
            var targetNodeOfTransition = fullSeries[targetOfTransition];
            ProductionRuleProjectionNode[] previousNodes = null;
            if (targetPath.Depth > 0)
                previousNodes = targetPath.Take(targetPath.Depth).ToArray();
            else
                previousNodes = new ProductionRuleProjectionNode[0];
            if (targetNodeOfTransition.Value.Keys.Count > 0)
            {
                foreach (var key in targetNodeOfTransition.Value.Keys)
                {
                    var tokenVar = key.GetTokenVariant();
                    var originalSet = key.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).ToArray();
                    var reducedRules = originalSet.Where(k => NodeIsRelevantToThis(k.Node, targetNodeOfTransition)).Select(k => k.Symbol).ToArray();
                    var ruleVar = new GrammarVocabulary(key.symbols, reducedRules);
                    foreach (var path in targetNodeOfTransition.Value[key].UnalteredOriginals)
                    {
                        var currentSubPath = path;
                        /* *
                         * The transition is derived from the parent node's
                         * transitions because the current node is omittable.
                         * */
                        while (!currentSubPath.CurrentNode.Value.Keys.Any(k => !k.Intersect(key).IsEmpty) && currentSubPath.Depth > 0)
                            currentSubPath = currentSubPath.DecreaseDepth();
                        if (!tokenVar.IsEmpty)
                            result.Add(Tuple.Create(tokenVar, GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition)));
                        if (!ruleVar.IsEmpty)
                            result.Add(Tuple.Create(ruleVar, GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition)));
                    }
                }
                if (targetOfTransition.IsEdge)
                {
                    var result2 = result.ToArray().ToList();
                    if (previousNodes.Length > 0)
                        result.AddRange(GetTransitionPath(targetPath, previousNodes, targetNodeOfTransition, ruleLookup, usePrevious, isEpsilonTransition, false).FollowTransitionInternal(fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
                    else
                        result.Add(Tuple.Create(GrammarVocabulary.NullInst, new ProductionRuleProjectionDPath(new[] { targetNodeOfTransition })));
                }
            }
            else if (targetOfTransition.IsEdge && previousNodes.Length > 0)
                result.AddRange(targetPath.FollowTransitionInternal(fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
            else if (targetOfTransition.IsEdge && previousNodes.Length == 0/* && includeRootEdges*/)
                result.Add(Tuple.Create(GrammarVocabulary.NullInst, new ProductionRuleProjectionDPath(new[] { targetNodeOfTransition })));
            return result;
        }

        private static bool NodeIsRelevantToThis(ProductionRuleProjectionNode node, ProductionRuleProjectionNode relevantNode)
        {
            lock (node.RootAmbiguityContexts)
                return node.RootAmbiguityContexts
                    .Any(k => 
                        k
                        .Any(v => 
                            v.Contains(relevantNode)));
        }

        private ProductionRuleProjectionDPath GetTransitionPath(ProductionRuleProjectionDPath targetPath, ProductionRuleProjectionNode[] previousNodes, ProductionRuleProjectionDPath currentSubPath, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool isEpsilonTransition = false, bool incrementDeviations = true)
        {
            var currentNode = currentSubPath[0];
            if (previousNodes.Length > 0)
                Debug.Assert(previousNodes[previousNodes.Length - 1].Value.OriginalState.OutTransitions.Keys.Any(k => !k.Intersect(ruleLookup[currentNode.Rule]).IsEmpty));
            short[] deviations = incrementDeviations ? IncrementDeviations(previousNodes.Length, isEpsilonTransition) : this.GetDeviationsUpTo(previousNodes.Length);
            var result = new ProductionRuleProjectionDPath(previousNodes.Concat(currentSubPath).ToArray(), targetPath.Depth + currentSubPath.Depth, passAlong: false, deviations: deviations);
            result.MinDepth = targetPath.MinDepth;
            return result;
        }

        private ProductionRuleProjectionDPath GetTransitionPath(ProductionRuleProjectionDPath targetPath, ProductionRuleProjectionNode[] previousNodes, ProductionRuleProjectionNode currentNode, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool isEpsilonTransition = false, bool incrementDeviations = true)
        {
            if (previousNodes.Length > 0)
                Debug.Assert(previousNodes[previousNodes.Length - 1].Value.OriginalState.OutTransitions.Keys.Any(k => !k.Intersect(ruleLookup[currentNode.Rule]).IsEmpty));
            short[] deviations = incrementDeviations ? IncrementDeviations(previousNodes.Length, isEpsilonTransition) : this.GetDeviationsUpTo(previousNodes.Length);

            var result = new ProductionRuleProjectionDPath(previousNodes.Concat(new[] { currentNode }).ToArray(), targetPath.Depth, passAlong: false, deviations: deviations);
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

        internal ProductionRuleProjectionDPath CreateFixed(bool passAlong = true)
        {
            return new ProductionRuleProjectionDPath(this.baseList, this.Depth, false, passAlong, this.deviations) { MinDepth = this.Depth };
        }


        public ProductionRuleProjectionDPath GetBottomLevel(bool passAlong = true)
        {
            return this.DepthAt(this.Count - this.MinDepth, passAlong);
        }

        public ProductionRuleLeftRecursionType GetRecursionType()
        {
            if (this.Count == 0)
                return ProductionRuleLeftRecursionType.None;
            var first = this[0];
            if (!first.Value.IsRuleNode)
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
                if (!currentNode.Value.IsRuleNode && !(this.IsEpsilonDerived(i) && currentNode.RootNode == first))
                    return result;
                if (currentNode == first || this.IsEpsilonDerived(i) && currentNode.RootNode == first)
                    result |= (i == 1 ? ProductionRuleLeftRecursionType.Direct : ProductionRuleLeftRecursionType.Indirect);
            }
            return result;
        }

        public bool RecursesIntoRoot
        {
            get
            {
                if (this.Count == 0)
                    return false;
                var first = this.CurrentNode.RootNode;
                for (int i = this.Depth + 1; i < this.Count; i++)
                {
                    var currentNode = this[i];
                    if (!currentNode.Value.IsRuleNode && !(this.IsEpsilonDerived(i) && currentNode.RootNode == first))
                        return false;
                    if (currentNode == first || this.IsEpsilonDerived(i) && currentNode.RootNode == first)
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

        internal static FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>> FollowTransitionEx(ProductionRuleProjectionDPath origin, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool injectIncomingPaths, bool includeRootEdges = false, bool isEpsilonTransition = false)
        {
            var resultData = from u in FollowTransitionInternalEx(origin, fullSeries, transitionKey, ruleLookup, usePrevious, injectIncomingPaths, includeRootEdges, isEpsilonTransition)
                             let k = u.Item1
                             group new { Path = k.Item2, Cause = u.Item2 } by k.Item1;
            var result = new FiniteAutomataMultiTargetTransitionTable<GrammarVocabulary, Tuple<ProductionRuleProjectionDPath, ProductionRuleProjectionDPath>>();
            foreach (var element in resultData)
            {
                if (element.Key.IsEmpty)
                    result.SetAutoSegment(false);
                result.Add(element.Key, (from u in element.ToList()
                                         select Tuple.Create(u.Path, u.Cause)).ToList());
                if (element.Key.IsEmpty)
                    result.SetAutoSegment(true);
            }
            return result;
        }

        internal static IEnumerable<Tuple<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>, ProductionRuleProjectionDPath>> FollowTransitionInternalEx(ProductionRuleProjectionDPath current, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, GrammarVocabulary transitionKey, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, bool usePrevious, bool inectIncomingPaths = true, bool includeRootEdges = false, bool isEpsilonTransition = false)
        {
            if (transitionKey.IsEmpty)
                return new Tuple<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>, ProductionRuleProjectionDPath>[0];
            var result = new List<Tuple<Tuple<GrammarVocabulary, ProductionRuleProjectionDPath>, ProductionRuleProjectionDPath>>();
            /* *
             * The transition was broken up earlier to yield
             * subsets that overlapped on a portion of the
             * transition.
             * */
            var targetPath = usePrevious ? current.DecreaseDepth(!isEpsilonTransition) : current;
            var fullTransitionKey = targetPath.CurrentNode.Value.OriginalState.OutTransitions.Keys.FirstOrDefault(v => !v.Intersect(transitionKey).IsEmpty);
            if (fullTransitionKey == null)
            {
                Debug.Assert(fullTransitionKey != null);
            }
            var targetOfTransition = targetPath.CurrentNode.Value.OriginalState.OutTransitions[fullTransitionKey];
            var targetNodeOfTransition = fullSeries[targetOfTransition];
            ProductionRuleProjectionNode[] previousNodes = null;
            if (targetPath.Depth > 0)
                previousNodes = targetPath.Take(targetPath.Depth).ToArray();
            else
                previousNodes = new ProductionRuleProjectionNode[0];
            if (targetNodeOfTransition.Value.Keys.Count > 0)
            {
                foreach (var key in targetNodeOfTransition.Value.Keys)
                {
                    var tokenVar = key.GetTokenVariant();
                    foreach (var path in targetNodeOfTransition.Value[key].UnalteredOriginals)
                    {
                        var currentSubPath = path;
                        /* *
                         * The transition is derived from the parent node's
                         * transitions because the current node is omittable.
                         * */
                        while (!currentSubPath.CurrentNode.Value.Keys.Any(k => !k.Intersect(key).IsEmpty) && currentSubPath.Depth > 0)
                            currentSubPath = currentSubPath.DecreaseDepth();
                        var transitionPath = current.GetTransitionPath(targetPath, previousNodes, currentSubPath, ruleLookup, usePrevious, isEpsilonTransition);
                        var originalSet = key.GetRuleVariant().GetSymbols().Select(k => (IGrammarRuleSymbol)k).Select(k => new { Node = fullSeries.GetRuleNodeFromFullSeries(k.Source), Symbol = k }).Where(k => k.Node.HasBeenReduced).ToArray();
                        var reducedRules = originalSet.Where(k => NodeIsRelevantToThis(k.Node, targetNodeOfTransition)).Select(k => k.Symbol);
                        var ruleVar = new GrammarVocabulary(key.symbols, reducedRules.ToArray());
                        if (!tokenVar.IsEmpty)
                            result.Add(Tuple.Create(Tuple.Create(tokenVar, transitionPath), current));
                        if (!ruleVar.IsEmpty)
                            result.Add(Tuple.Create(Tuple.Create(ruleVar, transitionPath), current));
                    }
                }
                if (targetOfTransition.IsEdge)
                {
                    var result2 = result.ToArray().ToList();
                    if (previousNodes.Length > 0)
                        result.AddRange(FollowTransitionInternalEx(current.GetTransitionPath(targetPath, previousNodes, targetNodeOfTransition, ruleLookup, usePrevious, isEpsilonTransition, false), fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
                    else
                        result.Add(Tuple.Create(Tuple.Create(GrammarVocabulary.NullInst, new ProductionRuleProjectionDPath(new[] { targetNodeOfTransition })), current));
                }
            }
            else if (targetOfTransition.IsEdge && previousNodes.Length > 0)
                result.AddRange(FollowTransitionInternalEx(targetPath, fullSeries, ruleLookup[targetNodeOfTransition.Value.Rule], ruleLookup, true, isEpsilonTransition: isEpsilonTransition));
            else if (targetOfTransition.IsEdge && previousNodes.Length == 0/* && includeRootEdges*/)
                result.Add(Tuple.Create(Tuple.Create(GrammarVocabulary.NullInst, new ProductionRuleProjectionDPath(new[] { targetNodeOfTransition })), current));
            return result;
        }

        public bool Valid { get; private set; }
    }
}
