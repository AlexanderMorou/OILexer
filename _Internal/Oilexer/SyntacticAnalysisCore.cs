//#define AdvanceSeries
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal static partial class SyntacticAnalysisCore
    {
        public static Dictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleProjectionNode> ConstructProjectionNodes(ParserCompiler compiler)
        {
            /* *
             * Constructs a lookup table for OilexerGrammarProductionRuleEntry->
             * ProductionRuleProjectionNode.
             * *
             * This lookup is used in further steps to isolate firstSeries-sets,
             * follow sets, predict sets, and the ambiguity
             * isolation that might result.
             * */
            var result = new Dictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleProjectionNode>();

            foreach (var rule in compiler.RuleDFAStates.Keys)
            {
                ProductionRuleProjectionNode current = new ProductionRuleProjectionNode();
                current.Value = new ProductionRuleProjectionNodeInfo(current, compiler.RuleDFAStates[rule], rule);
                result.Add(rule, current);
            }
            return result;
        }

        public static Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> ConstructRemainingNodes(Dictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleProjectionNode> rootNodes)
        {
            /* *
             * This constructs the remaining nodes for firstSeries/follow/predict 
             * projection.
             * *
             * The notion of 'projection' is used due to the lack of a single
             * firstSeries table being generated, this affects all possible outcomes
             * and yields an optimal set of resolution paths.
             * */
            var result = (from r in rootNodes.Keys
                          let node = rootNodes[r]
                          select new { Node = node, State = node.Value.OriginalState }).ToDictionary(k => k.State, v => v.Node);
            foreach (var rootState in (from rS in result.Keys
                                       select (SyntacticalDFARootState)rS).ToArray())
            {
                List<SyntacticalDFAState> currentSubstates = new List<SyntacticalDFAState>();
                SyntacticalDFAState.FlatlineState(rootState, currentSubstates);

                if (currentSubstates.Contains(rootState))
                    currentSubstates.Remove(rootState);
                foreach (var subState in currentSubstates)
                {
                    var currentNode = new ProductionRuleProjectionNode();
                    currentNode.Value = new ProductionRuleProjectionNodeInfo(currentNode, subState, rootState.Entry);
                    currentNode.RootNode = rootNodes[rootState.Entry];
                    result.Add(subState, currentNode);
                }
            }
            /* *
             * Rebuild dictionary to order the elements by the
             * rules which contain them.
             * *
             * This ends up in a sequential list of incrementing
             * state-values.
             * */
            return (from kvp in result
                    let node = kvp.Value
                    orderby node.Value.Rule.Name,
                            kvp.Key.StateValue
                    select kvp).ToDictionary(k => k.Key, v => v.Value);
        }

        public static void ConstructProjectionLinks(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleProjectionNode> ruleSeries)
        {
            /* *
             * Link the projections up, for every state
             * inject the rule-transitions from the transition table
             * of the states to the appropriate rule node.
             * *
             * This is used later in path expansion.
             * */
            foreach (var node in fullSeries.Values)
                node.LinkProjections(ruleSeries);
        }

        /// <summary>
        /// Performs look-ahead analysis on the projection nodes.
        /// </summary>
        /// <param name="fullSeries">The <see cref="Dictionary{TKey, TValue}"/> of
        /// <see cref="SyntacticalDFAState"/> to <see cref="ProductionRuleProjectionNode"/> associations.</param>
        /// <remarks>
        /// The look-up table is used to handle transitory movements from
        /// state->state in the event that more than one state is required in look-ahead analysis.
        /// </remarks>
        public static void PerformLookAheadProjection(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, int cycleDepth)
        {
            /* *
             * Construct a series of stacks used to delegate the
             * construction of the path data.
             * */
            int pCount = Environment.ProcessorCount;
            var ruleNodes = (from node in fullSeries.Values
                             group node by node.Rule).ToDictionary(k => k.Key, v => v.ToList());
            var ruleSetIDs = (from index in 0.RangeTo(ruleNodes.Count)
                              let pIndex = index % pCount
                              let ruleNodeKVP = ruleNodes.ElementAt(index)
                              let rule = ruleNodeKVP.Key
                              group rule by pIndex).ToDictionary(k => k.Key, v => v.ToList());
            object acLock = new object();
            for (int i = 0; i < cycleDepth; i++)
            {
#if ParallelProcessing
                Parallel.ForEach(ruleSetIDs.Values, ruleSetID =>
#else
                foreach (var ruleSetID in ruleSetIDs.Values)
#endif
                {
                    foreach (var rule in ruleSetID)
                        foreach (var node in ruleNodes[rule])
                            node.Value.ConstructInitialLookAheadProjection(i, cycleDepth);
#if ParallelProcessing
                });
#else
                }
#endif
            }
        /* *
         * Once the initial set of states has been created, 
         * perform epsilon translation.  This might be handled
         * a few times as empty rules will progressively inject
         * epsilon transitions.
         * */
        ChangeOccurred:
            bool anyChange = false;
#if ParallelProcessing
            Parallel.ForEach(ruleSetIDs.Values, ruleSetID =>
#else
            foreach (var ruleSetID in ruleSetIDs.Values)
#endif
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                foreach (var rule in ruleSetID)
                {
                    foreach (var node in ruleNodes[rule])
                    {
                        if (node.Value.ConstructEpsilonLookAheadProjection(fullSeries, ruleVocabulary, cycleDepth))
                            lock (acLock)
                                anyChange = true;
                    }
                }
#if ParallelProcessing
            });
#else
            }
#endif
            if (anyChange)
                goto ChangeOccurred;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void PerformExpandedLookAheadProjection(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
#if ShortcutToFindBug13
            return;
#endif
            /* *
             * The progressive look-ahead method, this should be rewritten
             * to break down the work into rule-sized stacks that can allow
             * the default Parallel library to delegate more effectively.
             * */
            Dictionary<int, Stack<ProductionRuleProjectionDPathSet>> processorStacks = new Dictionary<int, Stack<ProductionRuleProjectionDPathSet>>();
            int pCount = fullSeries.Count < 100 ? 1 : Environment.ProcessorCount;//(int)Math.Pow(Environment.ProcessorCount, 2);
            var advanceCount = new Dictionary<int, int>();
            var currentFrozen = new Dictionary<int, IOilexerGrammarProductionRuleEntry>();
            foreach (var processor in 0.RangeTo(pCount))
            {
                processorStacks.Add(processor, new Stack<ProductionRuleProjectionDPathSet>());
                advanceCount.Add(processor, 0);
                currentFrozen.Add(processor, null);
            }
            int seriesIndex = 0;
            Dictionary<IOilexerGrammarProductionRuleEntry, List<ProductionRuleProjectionDPathSet>> uniqueElements = (from r in ruleLookup.Keys
                                                                                                       select new { Rule = r, Set = new List<ProductionRuleProjectionDPathSet>() }).ToDictionary(k => k.Rule, v => v.Set);
            //fullSeries.Values.OnAll(series =>
            foreach (var ruleSeries in fullSeries)
            {
                var series = ruleSeries.Value;
                int currentStackIndex = seriesIndex++ % pCount;
                Stack<ProductionRuleProjectionDPathSet> toAdvance = processorStacks[currentStackIndex];
                series.ConstructInitialLookahead(grammarSymbols, fullSeries, ruleLookup);
                /* * * * * * * * * * * * * * * * * * * * * * * * * * *\
                 * Assume that we need to disambiguate all states     *
                 * it's easier to do this and throw away the ones     *
                 * which yield a single decision on all avenues than  *
                 * to 'guess' at which ones need the advance machine, *
                 * from the get go.                                   *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * **
                 * You basically have to do the same kind of break-   *
                 * down to know if it's necessary, so just use it     *
                 * as its own proof.                                  *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * **/
                foreach (var key in series.LookAhead.Keys)
                    toAdvance.Push((ProductionRuleProjectionDPathSet)series.LookAhead[key]);
            }
            //**/);
            /* *
             * This needs rewritten immediately!
             * */
#if ShowStackReductionProgress
            object conLock = new object();
            PrintStackStates(processorStacks, true);
#endif
            object processorStacksLock = new object();
#if ParallelProcessing
            Parallel.ForEach(processorStacks, indexAndtoAdvance =>
#else
            foreach (var indexAndtoAdvance in processorStacks)
#endif
            //foreach (var toAdvance in processorStacks.Values)
            //processorStacks.Values.OnAll(toAdvance =>
            {
                var toAdvance = indexAndtoAdvance.Value;
#if ShowStackReductionProgress
                DateTime lastPassTime = DateTime.Now;
#endif
#if ParallelProcessing
#if ShowStackReductionProgress
                int stepCount = 0;
#endif
            Reprocess:
#endif
                while (toAdvance.Count > 0)
                {
                    lock (compilationErrors)
                        if (compilationErrors.HasErrors)
                            return;
#if ShowStackReductionProgress
                    if ((DateTime.Now - lastPassTime).TotalMilliseconds > 250)
                        lock (conLock)
                            PrintStackStates(processorStacks, false);
#endif
                    ProductionRuleProjectionDPathSet current = null;
#if ParallelProcessing
                    lock (toAdvance)
                        lock (currentFrozen)
                        {
#endif
                            /* *
                             * Caused by reallocation.
                             * */
                            if (toAdvance.Count == 0)
                                break;
                            current = toAdvance.Pop();
                            currentFrozen[indexAndtoAdvance.Key] = current.Rule;
#if ParallelProcessing
                        }
#endif
                    var totalStates = uniqueElements[current.Root.Value.Rule];
#if ShowStackReductionProgress
#if ParallelProcessing
                    lock (conLock)
                        if (stepCount++ % 50 == 0)
#endif
                            PrintStackStates(processorStacks, false);
#endif

                    current.Advance(fullSeries, ruleLookup, compilationErrors, grammarSymbols);
                    foreach (var element in current.LookAhead.Keys.ToArray())
                    {
                        ProductionRuleProjectionDPathSet currentElement = (ProductionRuleProjectionDPathSet)current.LookAhead[element];
                        ProductionRuleProjectionDPathSet targetExistingPath;
#if ParallelProcessing
                        lock (toAdvance)
                            lock (totalStates)
#endif
                                targetExistingPath = (ProductionRuleProjectionDPathSet)totalStates.FirstOrDefault(state => state.Equals(currentElement));
                        if (targetExistingPath == null)
                            PushAndAdvance(advanceCount, indexAndtoAdvance, toAdvance, totalStates, currentElement);
                        else
                        {
                            var left = currentElement.GetCurrentPathSets().Cast<ProductionRuleProjectionDPathSet>().ToArray();
                            var right = targetExistingPath.GetCurrentPathSets().Cast<ProductionRuleProjectionDPathSet>().ToArray();
                            if (left.Length == right.Length)
                            {
                                var allEqual = (from index in 0.RangeTo(left.Length)
                                                let leftElement = left[index]
                                                let rightElement = right[index]
                                                select Tuple.Create(leftElement, rightElement)).All(k => k.Item1.Equals(k.Item2));
                                /* *
                                 * If two states yield an identical layout, replace
                                 * one with the original because it has already
                                 * been processed.
                                 * */
                                if (allEqual)
                                    current.ReplaceLookahead(element, targetExistingPath);
                                else
                                    PushAndAdvance(advanceCount, indexAndtoAdvance, toAdvance, totalStates, currentElement);
                            }
                            else
                                PushAndAdvance(advanceCount, indexAndtoAdvance, toAdvance, totalStates, currentElement);
                        }
                    }
#if ParallelProcessing
                    lock (toAdvance)
                        lock (currentFrozen)
#endif
                            currentFrozen[indexAndtoAdvance.Key] = null;
#if ShowStackReductionProgress
                    lastPassTime = DateTime.Now;
#endif
                }
#if ShowStackReductionProgress
                lock (conLock)
                    PrintStackStates(processorStacks, true);
#endif
#if ParallelProcessing
            RecheckReallocate:

                int reallocateResult = Reallocate(processorStacks, indexAndtoAdvance.Key, toAdvance, currentFrozen, processorStacksLock);
                if (reallocateResult == 1)
                {
#if ShowStackReductionProgress
                    lock (conLock)
                        PrintStackStates(processorStacks, true);
#endif
                    goto Reprocess;
                }
                else if (reallocateResult == -1)
                    goto RecheckReallocate;
            });
#else
            }
#endif
        }

        private static KeyValuePair<int, Stack<ProductionRuleProjectionDPathSet>> PushAndAdvance(Dictionary<int, int> advanceCount, KeyValuePair<int, Stack<ProductionRuleProjectionDPathSet>> indexAndtoAdvance, Stack<ProductionRuleProjectionDPathSet> toAdvance, List<ProductionRuleProjectionDPathSet> totalStates, ProductionRuleProjectionDPathSet currentElement)
        {
#if ParallelProcessing
            lock (toAdvance)
                lock (totalStates)
                {
#endif
                    PushIt(toAdvance, totalStates, currentElement);
#if ParallelProcessing
                    lock (advanceCount)
#endif
                        advanceCount[indexAndtoAdvance.Key]++;
#if ParallelProcessing
                }
#endif
            return indexAndtoAdvance;
        }
#if ParallelProcessing

        private static int Reallocate(Dictionary<int, Stack<ProductionRuleProjectionDPathSet>> processorStacks, int currentStack, Stack<ProductionRuleProjectionDPathSet> toAdvance, Dictionary<int, IOilexerGrammarProductionRuleEntry> currentFrozen, object processorStacksLock)
        {
            /* *
             * Lock all the relevant stacks.
             * */
            var timer = DateTime.Now;
            Dictionary<int, IOilexerGrammarProductionRuleEntry> currentFrozenCopy;
            lock (processorStacksLock)
            {
                foreach (int index in processorStacks.Keys)
                    Monitor.Enter(processorStacks[index]);
                lock (currentFrozen)
                    currentFrozenCopy = new Dictionary<int, IOilexerGrammarProductionRuleEntry>(currentFrozen);
                bool allNull = currentFrozenCopy.All(k => k.Value == null);
                bool allEmpty = processorStacks.Values.Select(k => k.Count).Sum() == 0;
                if (allNull)
                    return allEmpty ? 0 : -1;
                try
                {
                    int currentFullCount = 0;
                    int activeStacks = 0;
                    /* *
                     * Count the number of active stacks,
                     * the current stack and others that might be
                     * sleeping aren't relevant to the count.
                     * *
                     * Current inactive thread is considered below.
                     * */
                    foreach (int index in processorStacks.Keys)
                    {
                        if (index == currentStack)
                            continue;
                        int currentCount = processorStacks[index].Count;
                        currentFullCount += currentCount;
                        if (currentCount > 0)
                            activeStacks++;
                    }
                    /* *
                     * Small sets can finish on their own.
                     * *
                     * No need to complicate it more than 
                     * necessary.
                     * */
                    if (currentFullCount < 100)
                        return 0;
                    Stack<ProductionRuleProjectionDPathSet> buffer = new Stack<ProductionRuleProjectionDPathSet>();

                    /* *
                     * The current stack wasn't considered in this, because
                     * it is known to be empty.
                     * */
                    int countPerStack = currentFullCount / (activeStacks + 1);

                    /* *
                     * We're done!
                     * */
                    if (currentFullCount == 0)
                        return 0;
                    /* *
                     * The number of elements isn't a multiple of the stacks
                     * present.
                     * *
                     * In this case, we'll instruct the thread to sleep.
                     * Unless it took a long time to do portions above.
                     * */
                    if (countPerStack == 0)
                    {
                        /* *
                         * Unless... the processing time is high enough
                         * that continuing the sleep/reallocate cycle is
                         * an effort of futility.
                         * */
                        if ((DateTime.Now - timer).TotalMilliseconds > 1000)
                            return 0;
                        else
                            return -1;
                    }
                    /* *
                     * Iterate through the stacks and pull out the ones
                     * which have too many elements.
                     * */
                    bool allSameRule = false;
                    var ruleCounts = (from elementSet in processorStacks.Values
                                      from element in elementSet
                                      group element by element.Rule).ToDictionary(k => k.Key, v => v.Count());
                    var ruleCounts2 = 0;
                    foreach (int index in processorStacks.Keys)
                    {
                        if (index == currentStack)
                            continue;
                        var stack = processorStacks[index];
                        if (stack.Count == 0)
                            continue;
                        var currentFrozenRule = currentFrozenCopy[index];
                        Stack<ProductionRuleProjectionDPathSet> frozenElements = new Stack<ProductionRuleProjectionDPathSet>(from s in stack
                                                                                                                             where s.Rule == currentFrozenRule || currentFrozenRule == null
                                                                                                                             select s);

                        Stack<ProductionRuleProjectionDPathSet> remainder = new Stack<ProductionRuleProjectionDPathSet>(stack.Except(frozenElements));
                        ruleCounts2 += frozenElements.Count;
                        while (remainder.Count + frozenElements.Count > countPerStack && remainder.Count > 0)
                        {
                            var peekRule = remainder.Count > 0 ? remainder.Peek().Rule : null;
                            var currentRuleElements = new Stack<ProductionRuleProjectionDPathSet>(from s in remainder
                                                                                                  where s.Rule == peekRule
                                                                                                  select s);
                            remainder = new Stack<ProductionRuleProjectionDPathSet>(remainder.Except(currentRuleElements));
                            while (currentRuleElements.Count > 0)
                                buffer.Push(currentRuleElements.Pop());
                        }
                        stack.Clear();
                        if (ruleCounts2 == currentFullCount)
                            allSameRule = true;
                        while (remainder.Count > 0)
                            stack.Push(remainder.Pop());
                        while (frozenElements.Count > 0)
                            stack.Push(frozenElements.Pop());
                    }
                    /* *
                     * We ignore stacks which are all the same rule,
                     * so if all stacks are focusing on a single rule
                     * each, we can't segment them due to the single-threaded
                     * nature of the methods being called.
                     * *
                     * Once we put them in differing stacks, the isolation
                     * breaks down and we get 'chaos'.
                     * */
                    if (allSameRule)
                        return 0;
                    if (buffer.Count == 0)
                    {
                        /* *
                         * Yield when taking too long.
                         * *
                         * The stacks that are still going
                         * will finish on their own.
                         * */
                        if ((DateTime.Now - timer).TotalMilliseconds > 1000)
                            return 0;
                        else
                            return -1;
                    }

                    foreach (int index in processorStacks.Keys)
                    {
                        var stack = processorStacks[index];
                        if (stack.Count != 0 || index == currentStack)
                        {
                        rePeek:
                            var peekRule = buffer.Peek().Rule;
                            if (stack.Count + ruleCounts[peekRule] < countPerStack)
                            {
                                while (buffer.Count > 0 && buffer.Peek().Rule == peekRule)
                                    stack.Push(buffer.Pop());
                                if (buffer.Count == 0)
                                    break;
                                goto rePeek;
                            }
                        }
                    }
                    if (buffer.Count > 0)
                    {
                        var stack = processorStacks[currentStack];
                        while (buffer.Count > 0)
                            stack.Push(buffer.Pop());
                    }
                    if ((DateTime.Now - timer).TotalMilliseconds > 1000)
                        /* *
                         * If it's taking this long to redistribute
                         * resources, exit stage left.
                         * */
                        return 0;
                    else
                        return 1;
                }
                finally
                {
                    foreach (int index in processorStacks.Keys)
                        Monitor.Exit(processorStacks[index]);
                }
            }
        }
#endif
#if ShowStackReductionProgress
        private static void PrintStackStates(Dictionary<int, Stack<ProductionRuleProjectionDPathSet>> stacks, bool fullPrint)
        {
            if (fullPrint)
            {
                Console.Clear();
                foreach (var indexStackPair in stacks)
                    if (indexStackPair.Value.Count == 0)
                        Console.WriteLine("Stack {0} count:  *  ", indexStackPair.Key, indexStackPair.Value.Count);
                    else
                        Console.WriteLine("Stack {0} count: {1:0000}", indexStackPair.Key, indexStackPair.Value.Count);

            }
            else
            {
                Console.CursorTop = 0;
                foreach (var indexStackPair in stacks)
                {

                    int lineLength = string.Format("Stack {0} count: ", indexStackPair.Key).Length;
                    Console.CursorLeft = lineLength;
                    if (indexStackPair.Value.Count == 0)
                        Console.WriteLine(" *  ");
                    else
                        Console.WriteLine("{0:0000}", indexStackPair.Value.Count);
                }
            }
        }
#endif
        private static void PushIt(Stack<ProductionRuleProjectionDPathSet> toAdvance, List<ProductionRuleProjectionDPathSet> totalStates, ProductionRuleProjectionDPathSet currentElement)
        {
            toAdvance.Push(currentElement);
            totalStates.Add(currentElement);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> series, Func<T, bool> splitPredicate, bool removeEmpty = true)
        {
            List<T> current = new List<T>();
            foreach (var element in series)
                if (splitPredicate(element))
                {
                    if (!removeEmpty || current.Count > 0)
                        yield return current.ToArray();
                    current = new List<T>();
                }
                else
                    current.Add(element);
            if (!removeEmpty || current.Count > 0)
                yield return current.ToArray();
            yield break;
        }

        public static Dictionary<ProductionRuleProjectionNode, ProductionRuleProjectionAdapter> ConstructLookAheadProjections(ParserCompiler compiler, Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> dfaLookup, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, GrammarSymbolSet symbols, ICompilerErrorCollection compilationErrors, ref int maxDFAState)
        {
            Dictionary<int, List<ProductionRuleProjectionNode>> processorNodes = null;
            int pCount = Math.Min(Environment.ProcessorCount, fullSeries.Count);
            var ruleBreakdown =
                (from f in fullSeries
                 group f.Value by f.Value.Value.Rule).ToDictionary(k => k.Key, v => v.ToArray());
            processorNodes =
                (from index in 0.RangeTo(ruleBreakdown.Count)
                 let key = ruleBreakdown.Keys.ElementAt(index)
                 let nodeSet = ruleBreakdown[key]
                 from node in nodeSet
                 let pIndex = index % pCount
                 group node by pIndex).ToDictionary(k => k.Key, v => v.ToList());
            var processorAutomationSets =
                (from index in 0.RangeTo(pCount)
                 select new { Dictionary = new Dictionary<ProductionRuleProjectionNode, SyntacticalDFAState>(), Index = index }).ToDictionary(k => k.Index, v => v.Dictionary);
            /* *
             * Can't pass a by-reference parameter into a
             * closure due to the hoisting that occurs during
             * CIL translation.
             * *
             * A field and a parameter aren't the same thing 
             * and the compiler can't change the value of the by-ref
             * parameter if the closure method's called, say, after
             * the closure-creating method exits.
             * *
             * Also... wrapped inside an object instance of our own
             * creation, because .NET's parallel library
             * doesn't *yet* provide the compiler with context that
             * the hoisted elements would need to be thread-safe.
             * */
            StateLocker locker = new StateLocker() { State = maxDFAState };
#if ParallelProcessing
            Parallel.ForEach(processorNodes, setKeyValuePair =>
#else
            foreach (var setKeyValuePair in processorNodes)
#endif

            {
                var index = setKeyValuePair.Key;
                var nodesToProcess = setKeyValuePair.Value;
                var rDict = processorAutomationSets[index];
                foreach (var node in nodesToProcess)
                {
                    if (!node.RequiresLookAheadAutomation)
                        continue;
                    var dfa = node.ConstructAdvanceDFA(fullSeries, ruleVocabulary, compilationErrors, dfaLookup, symbols);
                    dfa.ReduceDFA(false, PredictionDFAReductionComparer);
                    lock (locker)
                    {
                        int maxState = locker.State;
                        dfa.Enumerate(ref maxState);
                        locker.State = maxState;
                    }
                    rDict.Add(node, dfa);
                }
#if ParallelProcessing
            });
#else
            }
#endif
            maxDFAState = locker.State;
            var subResult = (from set in processorAutomationSets.Values
                             from kvp in set
                             orderby kvp.Key.Value.Rule.Name,
                                     kvp.Key.Value.OriginalState.StateValue
                             select kvp).ToDictionary(k => k.Key, v => v.Value);
            var subResultReverse = subResult.ToDictionary(k => k.Value, v => v.Key);
            var toReplace = new Dictionary<ProductionRuleProjectionNode, SyntacticalDFAState>();
            foreach (var leftKey in subResult.Keys)
            {
                if (toReplace.ContainsKey(leftKey))
                    continue;
                foreach (var rightKey in subResult.Keys.Except(toReplace.Keys))
                {
                    if (rightKey.RootNode != leftKey.RootNode)
                        continue;
                    if (object.ReferenceEquals(leftKey, rightKey))
                        continue;
                    if (toReplace.ContainsKey(rightKey))
                        continue;

                    SyntacticalDFAState leftState = subResult[leftKey];
                    SyntacticalDFAState rightState = subResult[rightKey];
                    if (ComparePredictionDFAReduction(leftState, rightState))
                        toReplace.Add(rightKey, leftState);
                }
            }

            foreach (var key in toReplace.Keys)
                subResult[key] = toReplace[key];
            Dictionary<SyntacticalDFAState, ProductionRuleProjectionAdapter> currentDict = null;
            var result = (from r in subResult
                          let currentSet = currentDict = new Dictionary<SyntacticalDFAState, ProductionRuleProjectionAdapter>()
                          select new { Node = r.Key, Adapter = ProductionRuleProjectionAdapter.Adapt(r.Value, ref currentDict, compiler), Dictionary = currentDict }).ToDictionary(k => k.Node, v => new { Adapter = v.Adapter, Dictionary = v.Dictionary });
            foreach (var rootNode in result.Keys)
            {
                currentDict = result[rootNode].Dictionary;
                var rootAdapter = result[rootNode].Adapter;
                rootAdapter.AssociatedContext.StateAdapterLookup = new ControlledDictionary<SyntacticalDFAState, ProductionRuleProjectionAdapter>(currentDict);
                foreach (var state in currentDict.Keys)
                {
                    var adapter = currentDict[state];
                    adapter.AssociatedContext.PostConnect(rootNode, result[rootNode].Adapter);
                }
            }
            return result.ToDictionary(k => k.Key, v => v.Value.Adapter);
        }

        public static void AnalyzeDFARepetitions(ProductionRuleProjectionContext[] targetContexts, Dictionary<ProductionRuleProjectionNode, ProductionRuleProjectionAdapter> lookup)
        {
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Repetition points need noted within the deterministic  *
             * automations: the look-ahead calculated to a specific   *
             * point may vary in step quantity if a cycle within the  *
             * automation is noted.                                   *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * It is necessary only for 'reduction' points to know    *
             * what the look-ahead depth is.  This way if a rule      *
             * being projected upon cycles back, the look-ahead will  *
             * be properly adjusted upon reduction.                   *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Without this, the effectiveness of the reductions are  *
             * greatly limited.                                       *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            //var groupings = (from targetContext in targetContexts
            //                 let sourceQuery = (from sourceInfo in targetContext.Adapter.AssociatedState.Sources
            //                                    let source = sourceInfo.Item1
            //                                    where source is ProductionRuleProjectionDPathSet
            //                                    select (ProductionRuleProjectionDPathSet)source).ToList()
            //                 where sourceQuery.Count > 0
            //                 from transitionKey in targetContext.Adapter.OutgoingTransitions.Keys
            //                 let transitionTarget = targetContext.Adapter.OutgoingTransitions[transitionKey]
            //                 /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
            //                  * Ignore cases where the points are edges with no exit transitions.  *
            //                  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
            //                  * Those are taken care of by being decision points, where the        *
            //                  * look-ahead depth becomes irrelevant since the callee marks the     *
            //                  * entry look-ahead depth.                                            *
            //                  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            //                 where !(transitionTarget.AssociatedState.IsEdge && transitionTarget.AssociatedState.OutTransitions.Count == 0)
            //                 group new { PathSeries = new HashSet<ProductionRuleProjectionDPathSet>(sourceQuery), Owner = targetContext } by transitionTarget).ToDictionary(k => k.Key, v => v.ToArray());
            //foreach (var transitionTarget in groupings.Keys)
            //    foreach (var grouping in groupings[transitionTarget])
            //        transitionTarget.AssociatedContext.DefineBucketOnTarget(grouping.Owner);
        }

        static Func<Tuple<IProductionRuleSource, FiniteAutomationSourceKind>, IProductionRuleSource> sourceSelector = a => a.Item1;

        private static bool ComparePredictionDFAReduction(SyntacticalDFAState left, SyntacticalDFAState right, Stack<Tuple<SyntacticalDFAState, SyntacticalDFAState>> alternations = null)
        {
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Due to the regularity of some areas of a given language,   *
             * the decision trees within them might overlap.  This        *
             * detects these commonalities and will throw the duplicates  *
             * out.                                                       *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * This reduces the result machine complexity and code size.  *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            if (alternations == null)
                alternations = new Stack<Tuple<SyntacticalDFAState, SyntacticalDFAState>>();
            var currentAlternation = new Tuple<SyntacticalDFAState, SyntacticalDFAState>(left, right);
            if (alternations.Contains(currentAlternation))
                return true;
            alternations.Push(currentAlternation);
            try
            {
                if (left.OutTransitions.Count == 0 &&
                    right.OutTransitions.Count == 0 &&
                    left.IsEdge && right.IsEdge)
                {
                    var leftSources = left.Sources.Select(sourceSelector).ToList();
                    var rightSources = right.Sources.Select(sourceSelector).ToList();
                    if (leftSources.Count != rightSources.Count)
                        return false;
                    return leftSources.All(k => rightSources.Contains(k));
                }
                else if (left.OutTransitions.Count == right.OutTransitions.Count)
                {
                    var leftSources = left.Sources.Select(sourceSelector).ToList();
                    var rightSources = right.Sources.Select(sourceSelector).ToList();
                    foreach (var leftTransition in left.OutTransitions.Keys)
                    {
                        if (!right.OutTransitions.ContainsKey(leftTransition))
                            return false;
                        var leftState = left.OutTransitions[leftTransition];
                        var rightState = right.OutTransitions[leftTransition];
                        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
                         * Since the states themselves are different by reference,  *
                         * we have to compare their relative similarity between one *
                         * another.                                                 *
                         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                        if (!ComparePredictionDFAReduction(leftState, rightState, alternations))
                            return false;
                    }
                    return SourcesAreEquivalent(leftSources, rightSources); //return SourcesAndTransitionsAreEquivalent(leftSources, rightSources);
                }
                return false;
            }
            finally
            {
                alternations.Pop();
            }
        }

        private static bool SourcesAreEquivalent(IEnumerable<IProductionRuleSource> leftSources, IEnumerable<IProductionRuleSource> rightSources)
        {
            var filteredLeft = GetSourceSetDPathSets(leftSources);
            var filteredRight = GetSourceSetDPathSets(rightSources);
            if (filteredLeft.Length == 0)
                return filteredRight.Length == 0;
            var leftDepths = GetPathSetSeriesDistinctDepths(filteredLeft);
            var rightDepths = GetPathSetSeriesDistinctDepths(filteredRight);
            return leftDepths.Count == rightDepths.Count &&
                leftDepths.SequenceEqual(rightDepths);
        }

        private static List<int> GetPathSetSeriesDistinctDepths(ProductionRuleProjectionDPathSet[] pathSetSeries)
        {
            return (from pathSet in pathSetSeries
                    let length = GetDPathSetTokenDepth(pathSet)
                    orderby length
                    select length).Distinct().ToList();
        }

        private static int GetDPathSetTokenDepth(ProductionRuleProjectionDPathSet set)
        {
            var series = set.GetCurrentPathSets().ToArray();
            int result = 0;
            if (set.Discriminator.IsEmpty)
                result--;
            for (int pathSetIndex = series.Length - 1; pathSetIndex >= 0; pathSetIndex--)
            {
                var currentSet = series[pathSetIndex];
                if (currentSet.ReductionType == LookAheadReductionType.CommonForwardSymbol)
                    break;
                result++;
            }
            return result;
        }

        private static ProductionRuleProjectionDPathSet[] GetSourceSetDPathSets(IEnumerable<IProductionRuleSource> leftSources)
        {
            return (from l in leftSources
                    where l is ProductionRuleProjectionDPathSet
                    select (ProductionRuleProjectionDPathSet)l).ToArray();
        }

        static Func<SyntacticalDFAState, SyntacticalDFAState, bool> PredictionDFAReductionComparer =
            (left, right) =>
            {
#if !ReduceProjections
                return false;
#endif
                if (left.OutTransitions.Count == 0 &&
                    right.OutTransitions.Count == 0 &&
                    left.IsEdge && right.IsEdge)
                {
                    var leftSources = left.Sources.Select(sourceSelector).ToList();
                    var rightSources = right.Sources.Select(sourceSelector).ToList();
                    if (leftSources.Count != rightSources.Count)
                        return false;
                    return SourcesAndTransitionsAreEquivalent(leftSources, rightSources) && SourcesAreEquivalent(leftSources, rightSources);
                }
                else if (left.OutTransitions.Count == right.OutTransitions.Count)
                {
                    var leftSources = left.Sources.Select(sourceSelector).ToList();
                    var rightSources = right.Sources.Select(sourceSelector).ToList();
                    foreach (var leftTransition in left.OutTransitions.Keys)
                    {
                        if (!right.OutTransitions.ContainsKey(leftTransition))
                            return false;
                        if (right.OutTransitions[leftTransition] != left.OutTransitions[leftTransition])
                            return false;
                    }
                    return SourcesAndTransitionsAreEquivalent(leftSources, rightSources) && SourcesAreEquivalent(leftSources, rightSources);
                }
                return false;
            };

        private static bool SourcesAndTransitionsAreEquivalent(List<IProductionRuleSource> leftSources, List<IProductionRuleSource> rightSources)
        {
            return leftSources.All(leftSet =>
            {
                var leftSource = leftSet as ProductionRuleProjectionDPathSet;
                if (leftSource == null)
                    return rightSources.Contains(leftSet);
                foreach (var rightSet in rightSources)
                {
                    var rightSource = rightSet as ProductionRuleProjectionDPathSet;
                    if (rightSource == null)
                        continue;
                    if (leftSource.ReductionType == rightSource.ReductionType)
                    {
                        bool currentResult = false;
                        switch (rightSource.ReductionType)
                        {
                            case LookAheadReductionType.LocalTransition:
                            case LookAheadReductionType.None:
                                currentResult = leftSource.ProjectedRootTarget == rightSource.ProjectedRootTarget &&
                                       leftSource.ProjectedRootTransition == rightSource.ProjectedRootTransition;
                                break;
                            case LookAheadReductionType.CommonForwardSymbol:
                                currentResult = (leftSource.PointOfCommonality == rightSource.PointOfCommonality &&
                                                 leftSource.ProjectedRootTarget == rightSource.ProjectedRootTarget &&
                                                 leftSource.ProjectedRootTransition == rightSource.ProjectedRootTransition);
                                break;
                            case LookAheadReductionType.RepetitionPoint:
                                currentResult = leftSource.ReplicationPoint == rightSource.ReplicationPoint;
                                break;
                        }
                        if (currentResult && GetDPathSetTokenDepth(leftSource) == GetDPathSetTokenDepth(rightSource))
                            return true;
                    }
                }
                return false;
            });
        }

        //private static bool CheckBucketCompatibility(ProductionRuleProjectionDPathSet leftSource, ProductionRuleProjectionDPathSet rightSource)
        //{
        //    /* *
        //     * Bucket compatibility isn't relevant when:
        //     * 1. Both states are edges
        //     * 2. These edges have no outgoing transitions.
        //     * *
        //     * This is because the look-ahead is reset
        //     * upon method exit.
        //     * */
        //    if (leftSource.IsEdge && leftSource.LookAhead.Count == 0 &&
        //        rightSource.IsEdge && rightSource.LookAhead.Count == 0)
        //        return true;
        //}
        public class StateLocker
        {
            private int state;
            /// <summary>
            /// Denotes the state of the <see cref="StateLocker"/>
            /// which will be used within parallel code within a
            /// closure.
            /// </summary>
            public int State { get { return this.state; } set { this.state = value; } }
        }

        internal static IEnumerable<ProductionRuleProjectionNode> PerformEdgePredictions(
            ParserCompiler parserCompiler, Dictionary<SyntacticalDFAState, 
            ProductionRuleProjectionNode> allProjectionNodes, 
            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleDFAStates, 
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleGrammarLookup,
            GrammarSymbolSet grammarSymbolSet,
            ICompilerErrorCollection compilerErrorCollection)
        {
            var possibleAmbiguities = (from rule in ruleDFAStates.Keys
                                       let dfa = ruleDFAStates[rule]
                                       from edge in dfa.ObtainEdges()
                                       where edge.OutTransitions.Count > 0
                                       let edgeNode = allProjectionNodes[edge]
                                       group new { Rule = rule, EdgeNode = edgeNode } by rule).ToDictionary(k => k.Key, v => v.ToArray());
            var ambiguousNodes = new List<ProductionRuleProjectionNode>();

            foreach (var rule in possibleAmbiguities.Keys)
            {
                var ruleAmbiguities = possibleAmbiguities[rule];
                foreach (var possibleAmbiguity in ruleAmbiguities)
                    if (possibleAmbiguity.EdgeNode.CalculateTerminalAmbiguities(allProjectionNodes, ruleDFAStates, ruleGrammarLookup, grammarSymbolSet))
                        ambiguousNodes.Add(possibleAmbiguity.EdgeNode);
            }
            //Stack<ProductionRuleProjectionDPathSet> toProcess = new Stack<ProductionRuleProjectionDPathSet>();
            MultikeyedDictionary<GrammarVocabulary, ProductionRuleProjectionDPathSet, Tuple<Stack<ProductionRuleProjectionDPathSet>, List<ProductionRuleProjectionDPathSet>>> processStacks =
                new MultikeyedDictionary<GrammarVocabulary, ProductionRuleProjectionDPathSet, Tuple<Stack<ProductionRuleProjectionDPathSet>, List<ProductionRuleProjectionDPathSet>>>();

            foreach (var node in ambiguousNodes)
            {

                foreach (var follow in node.FollowAmbiguities)
                {
                    processStacks.TryAdd(follow.InitialPaths.Discriminator, follow.InitialPaths, Tuple.Create(new Stack<ProductionRuleProjectionDPathSet>(new[] { follow.InitialPaths }), new List<ProductionRuleProjectionDPathSet>()));
                }
            }
            //toProcess.Push(follow.InitialPaths);
            var mergeSets = new List<Tuple<ProductionRuleProjectionFollow, ProductionRuleProjectionFollow>>();
            /*
             * To reinsert the relevant elements, see 
             * 'CalculateTerminalAmbiguities' referenced above.
            foreach (var node in ambiguousNodes)
                foreach (var follow in node.FollowAmbiguities)
                    foreach (var path in follow.InitialPaths)
                        if (ambiguousNodes.Contains(path[0]))
                            foreach (var followAmbiguity in path[0].RootNode.GetAllFollowAmbiguities(allProjectionNodes))
                                if (followAmbiguity.InitialPaths.Discriminator.Equals(follow.InitialPaths.Discriminator))
                                    mergeSets.Add(Tuple.Create(follow, followAmbiguity));
            /**/
            /* *
             * ToDo: reinsert situations where all initial paths are edge
             * states because the incoming paths from those paths may
             * themselves be ambiguous against the initial target set of
             * transitions.
             * */
#if ParallelProcessing
            Parallel.ForEach(processStacks, ksvp =>
#else
            foreach (var ksvp in processStacks)
#endif
            {
                var toProcessAndProcessed = ksvp.Value;
                var alreadyProcessed = toProcessAndProcessed.Item2;
                var toProcess = toProcessAndProcessed.Item1;
                while (toProcess.Count > 0)
                {
                    var current = toProcess.Pop();
                    current.Advance(allProjectionNodes, ruleGrammarLookup, compilerErrorCollection, grammarSymbolSet);
                    foreach (var transition in current.LookAhead.Keys.ToArray())
                    {
                        var currentTarget = (ProductionRuleProjectionDPathSet)current.LookAhead[transition];
                        var currentAlternate = alreadyProcessed.FirstOrDefault(k => k.Equals(currentTarget));
                        if (!alreadyProcessed.Contains(currentTarget))
                        {
                            toProcess.Push(currentTarget);
                            alreadyProcessed.Add(currentTarget);
                        }
                        else if (currentAlternate != null && !object.ReferenceEquals(currentAlternate, currentTarget))
                            current.ReplaceLookahead(transition, currentAlternate);
                    }
                }
            }
#if ParallelProcessing
                /* Auto indentation fix */
            );
#endif

            StateLocker slock = new StateLocker() { State = parserCompiler.ParserMaxState };

            //Parallel.ForEach(ambiguousNodes, ambiguousNode =>
            foreach (var ambiguousNode in ambiguousNodes)
            {
                foreach (var follow in ambiguousNode.FollowAmbiguities.ToArray())
                {
                    follow.BuildNFA(allProjectionNodes, ruleDFAStates, ruleGrammarLookup, (GrammarSymbolSet)grammarSymbolSet, compilerErrorCollection);
                    follow.DeterminateAutomata();
                    follow.ReduceAutomata(false, PredictionDFAReductionComparer);
                }
            }//);*/
            var toReplace = new Dictionary<ProductionRuleProjectionFollow, ProductionRuleProjectionFollow>();
            foreach (var ambiguousNode in ambiguousNodes)
            {
                foreach (var leftKey in ambiguousNode.FollowAmbiguities)
                {
                    if (toReplace.ContainsKey(leftKey))
                        continue;
                    foreach (var rightKey in ambiguousNode.FollowAmbiguities.Except(toReplace.Keys))
                    {
                        if (rightKey.Rule != leftKey.Rule)
                            continue;
                        if (object.ReferenceEquals(leftKey, rightKey))
                            continue;
                        if (toReplace.ContainsKey(rightKey))
                            continue;

                        SyntacticalDFAState leftState = leftKey.DFAState;
                        SyntacticalDFAState rightState = rightKey.DFAState;
                        if (ComparePredictionDFAReduction(leftState, rightState))
                            toReplace.Add(rightKey, leftKey);
                    }
                }
            }
            /* *
             * First, construct the states that aren't to be replaced.
             * */
#if ParallelProcessing
            Parallel.ForEach(ambiguousNodes, ambiguousNode =>
#else
            foreach (var ambiguousNode in ambiguousNodes)
#endif
            {
                foreach (var follow in ambiguousNode.FollowAmbiguities)
                {
                    if (!toReplace.ContainsKey(follow))
                    {
                        follow.Enumerate(slock);
                        follow.Adapt(parserCompiler);
                    }
                }
            }
                /* *
                 * Then, handle the replacements in cases where
                 * the automations were identical.
                 * *
                 * This reduction is to prevent excessive state-machine
                 * generation that occurs because the parent automation
                 * contains multiple states which tie for the same set
                 * of transition elements, but once utilized, transition
                 * into the same states for the target automation.
                 * */
#if ParallelProcessing
            );

            Parallel.ForEach(ambiguousNodes, ambiguousNode =>
#else
            foreach (var ambiguousNode in ambiguousNodes)
#endif
            {
                foreach (var follow in ambiguousNode.FollowAmbiguities)
                    if (toReplace.ContainsKey(follow))
                    {
                        var replacementSource = toReplace[follow];
                        follow.ReplaceState(replacementSource.DFAState, replacementSource.Adapter);
                    }
            }
#if ParallelProcessing
              /* Auto indentation fix */
            );
#endif
            parserCompiler.ParserMaxState = slock.State;
            foreach (var ambiguity in ambiguousNodes)
                yield return ambiguity;
        }

        public static void CreateLexicalAmbiguityTransitions(
            GrammarSymbolSet grammarSymbols, 
            ControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<ProductionRuleProjectionDPath, ProductionRuleProjectionNode>> la, 
            ProductionRuleProjectionDPathSet previous,
            ProductionRuleProjectionNode root,
            Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, 
            Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary)
        {
            GrammarVocabulary fullSet = la.Keys.DefaultIfEmpty().Aggregate(GrammarVocabulary.UnionAggregateDelegate);
            if (!grammarSymbols.IsGrammarPotentiallyAmbiguous(fullSet))
                return;
            var overlap = GrammarVocabulary.NullInst;
            var ambiguousSymbols = grammarSymbols.AmbiguousSymbols.ToList();
            //var idents = fullSet.GetSymbols();
            
            foreach (var ambiguity in ambiguousSymbols)
            {
                /* There is nothing remaining, edge states. */
                if (fullSet == null || 
                    fullSet.IsEmpty || 
                    /*There's only one element within the transition table, no need to continue.*/
                    fullSet.TrueCount == 1)
                    break;
                var intersection = fullSet.Intersect(ambiguity.AmbiguityKey);
                var overlapIntersection = overlap.Intersect(ambiguity.AmbiguityKey);



                if (intersection.Equals(ambiguity.AmbiguityKey) && 
                    /* To avoid calling out redundant smaller sets, we track the 'overlap', if the overlap intersection equals the ambiguity, then a larger token ambiguity has already handled the case. */
                    !overlapIntersection.Equals(ambiguity.AmbiguityKey))
                {
                    var lookaheadPathSets = (from laKey in la.Keys
                                             let laTarget = la[laKey]
                                             let laKeyIntersection = laKey & ambiguity.AmbiguityKey
                                             where !laKeyIntersection.IsEmpty
                                             select laTarget.UnalteredOriginals.ToArray()).ToArray().ConcatinateSeries().Distinct().ToList();
                    var ambiguityVocabulary = new GrammarVocabulary(grammarSymbols, ambiguity);
                    if (la.ContainsKey(ambiguityVocabulary))
                    {
                        overlap |= ambiguity.AmbiguityKey;
                        continue;
                    }
                    ProductionRuleProjectionDPathSet ambiguityPathSet = ProductionRuleProjectionDPathSet.GetPathSet(ambiguityVocabulary, lookaheadPathSets, root, ProductionRuleProjectionType.LookAhead, PredictionDerivedFrom.LookAhead_AmbiguityCast);
                    var distinctNodeArray = (from path in ambiguityPathSet
                                             select path.CurrentNode).Distinct().ToArray();
                    foreach (var node in distinctNodeArray)
                        node.DenoteLexicalAmbiguity(ambiguity);
                    la._Add(ambiguityVocabulary, ambiguityPathSet);
                    
                    ambiguityPathSet.Previous = previous;
                    ambiguityPathSet.CheckReductionState(fullSeries, ruleVocabulary);
                    ambiguityPathSet.ProcessCommonSymbol();
                    ambiguity.Occurrences++;
                    overlap |= ambiguity.AmbiguityKey;
                }
            }
        }

    }
}
