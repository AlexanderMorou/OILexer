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
        public static Dictionary<IOilexerGrammarProductionRuleEntry, PredictionTreeLeaf> ConstructProjectionNodes(ParserCompiler compiler)
        {
            /* *
             * Constructs a lookup table for OilexerGrammarProductionRuleEntry->
             * PredictionTreeLeaf.
             * *
             * This lookup is used in further steps to isolate firstSeries-sets,
             * follow sets, predict sets, and the ambiguity
             * isolation that might result.
             * */
            var result = new Dictionary<IOilexerGrammarProductionRuleEntry, PredictionTreeLeaf>();

            foreach (var rule in compiler.RuleDFAStates.Keys)
            {
                PredictionTreeLeaf current = new PredictionTreeLeaf() { Compiler = compiler };
                current.Veins = new PredictionTreeLeafVeins(current, compiler.RuleDFAStates[rule], rule);
                result.Add(rule, current);
            }
            return result;
        }

        public static Dictionary<SyntacticalDFAState, PredictionTreeLeaf> ConstructRemainingNodes(Dictionary<IOilexerGrammarProductionRuleEntry, PredictionTreeLeaf> rootLeaves, ParserCompiler compiler)
        {
            /* *
             * This constructs the remaining leaves for firstSeries/follow/predict 
             * parse trees.  The full set of trees within a given grammar's machine 
             * make up a parse forest.
             * */
            var result = (from r in rootLeaves.Keys
                          let node = rootLeaves[r]
                          select new { Node = node, State = node.Veins.DFAOriginState }).ToDictionary(k => k.State, v => v.Node);
            foreach (var rootState in (from rS in result.Keys
                                       select (SyntacticalDFARootState)rS).ToArray())
            {
                List<SyntacticalDFAState> currentSubstates = new List<SyntacticalDFAState>();
                SyntacticalDFAState.FlatlineState(rootState, currentSubstates);

                if (currentSubstates.Contains(rootState))
                    currentSubstates.Remove(rootState);
                foreach (var subState in currentSubstates)
                {
                    var currentNode = new PredictionTreeLeaf() { Compiler = compiler };
                    currentNode.Veins = new PredictionTreeLeafVeins(currentNode, subState, rootState.Entry);
                    currentNode.RootLeaf = rootLeaves[rootState.Entry];
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
                    orderby node.Veins.Rule.Name,
                            kvp.Key.StateValue
                    select kvp).ToDictionary(k => k.Key, v => v.Value);
        }

        public static void ConstructProjectionLinks(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, PredictionTreeLeaf> ruleSeries)
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
        /// <see cref="SyntacticalDFAState"/> to <see cref="PredictionTreeLeaf"/> associations.</param>
        /// <remarks>
        /// The look-up table is used to handle transitory movements from
        /// state->state in the event that more than one state is required in look-ahead analysis.
        /// </remarks>
        public static void PerformLookAheadProjection(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, int cycleDepth)
        {
            /* *
             * Construct a series of predictionTreeStacks used to delegate the
             * construction of the path data.
             * */
            int pCount = Environment.ProcessorCount;
            var ruleLeafs = (from node in fullSeries.Values
                             group node by node.Rule).ToDictionary(k => k.Key, v => v.ToList());
            var ruleSetIDs = (from index in 0.RangeTo(ruleLeafs.Count)
                              let pIndex = index % pCount
                              let ruleNodeKVP = ruleLeafs.ElementAt(index)
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
                        foreach (var leaf in ruleLeafs[rule])
                            leaf.Veins.ConstructInitialLookAheadProjection(i, cycleDepth);
                }
#if ParallelProcessing
                );
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
            Parallel.ForEach(
                ruleSetIDs.Values,
                ruleSetID =>
#else
            foreach (var ruleSetID in ruleSetIDs.Values)
#endif
            {
                foreach (var rule in ruleSetID)
                {
                    foreach (var leaf in ruleLeafs[rule])
                    {
                        if (leaf.Veins.ConstructEpsilonLookAheadProjection(fullSeries, ruleVocabulary, cycleDepth))
                            anyChange = true;
                    }
                }
            }
#if ParallelProcessing
            //
            );
#endif
            if (anyChange)
                goto ChangeOccurred;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void PerformExpandedLookAheadProjection(Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleLookup, ICompilerErrorCollection compilationErrors, GrammarSymbolSet grammarSymbols)
        {
#if ShowStackReductionProgress
            Console.Clear();
#endif
            /* *
             * The progressive look-ahead method, this should be rewritten
             * to break down the work into rule-sized predictionTreeStacks that can allow
             * the default Parallel library to delegate more effectively.
             * */
            Dictionary<int, Stack<PredictionTree>> processorStacks = new Dictionary<int, Stack<PredictionTree>>();
            //int pCount = fullSeries.Count < 100 ? 1 : Environment.ProcessorCount;//(int)Math.Pow(Environment.ProcessorCount, 2);
            var advanceCount = new Dictionary<int, int>();
            var previousCounts = new Dictionary<int, int>();
            var currentFrozen = new Dictionary<int, IOilexerGrammarProductionRuleEntry>();
            int rCount = fullSeries.Count;
            foreach (var processor in 0.RangeTo(fullSeries.Count))
            {
                processorStacks.Add(processor, new Stack<PredictionTree>());
                advanceCount.Add(processor, 0);
                currentFrozen.Add(processor, null);
            }
            int seriesIndex = 0;
            //int offset = 0;
            foreach (var ruleSeries in fullSeries)
            {
                var series = ruleSeries.Value;
                int currentStackIndex = seriesIndex++ % rCount;
                Stack<PredictionTree> treesToAdvance = processorStacks[currentStackIndex];
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
                if (series.Veins.DFAOriginState.OutTransitions.Count <= 1)
                    continue;
                foreach (var key in series.LookAhead.Keys)
                    treesToAdvance.Push((PredictionTree)series.LookAhead[key]);
            }
            var stacksWithTreesToProcess = processorStacks.Values.Where(k => k.Count > 0).ToArray();
            processorStacks.Clear();
            for (int i = 0; i < stacksWithTreesToProcess.Length; i++)
                processorStacks.Add(i, stacksWithTreesToProcess[i]);
            //**/);
            /* *
             * This needs rewritten immediately!
             * */
#if ShowStackReductionProgress
            object conLock = new object();
            int stackPrintCount = 0;
            PrintStackStates(processorStacks, previousCounts, true);
#endif
            object processorStacksLock = new object();
#if ShowStackReductionProgress
            DateTime lastPassTime = DateTime.Now;
#endif
#if ParallelProcessing
            Parallel.ForEach(processorStacks, indexAndtoAdvance =>
#else
            foreach (var indexAndtoAdvance in processorStacks)
#endif
            {
                var toAdvance = indexAndtoAdvance.Value;
#if ShowStackReductionProgress
                int stepCount = 0;
#endif
                while (toAdvance.Count > 0)
                {
#if ShowStackReductionProgress
                    bool clearedAndOrPrinted = false;
#endif
                    lock (compilationErrors)
                        if (compilationErrors.HasErrors)
                            return;
#if ShowStackReductionProgress
                    if ((DateTime.Now - lastPassTime).TotalMilliseconds > 1250)
                    {
#if ParallelProcessing
                        lock (conLock)
                        {
#endif
                            stackPrintCount++;
                            if (stackPrintCount % 15 == 0)
                                Console.Clear();
                            PrintStackStates(processorStacks, previousCounts, stackPrintCount % 15 == 0);
                            clearedAndOrPrinted = true;
#if ParallelProcessing
                            lastPassTime = DateTime.Now;
                        }
#endif
                    }
#endif
                    PredictionTree current = null;
                    /* *
                     * Caused by reallocation.
                     * */
                    if (toAdvance.Count == 0)
                        break;
                    current = toAdvance.Pop();
#if ShowStackReductionProgress
#if ParallelProcessing
                    lock (conLock)
#endif
                    if (!clearedAndOrPrinted && stepCount++ % 15 == 0)
                    {
                        stackPrintCount++;
                        if (stackPrintCount % 15 == 0)
                            Console.Clear();
                        PrintStackStates(processorStacks, previousCounts, stackPrintCount % 15 == 0);
                    }
#endif
                    current.Advance(fullSeries, ruleLookup, compilationErrors, grammarSymbols);
                    foreach (var currentElement in current.GetAndClearAdvanceSet())
                        PushAndAdvance(advanceCount, indexAndtoAdvance, toAdvance, /*totalStates, */currentElement);
#if ShowStackReductionProgress
                    lastPassTime = DateTime.Now;
#endif
                }
#if ShowStackReductionProgress
            lock (conLock)
                PrintStackStates(processorStacks, previousCounts, true);
#endif
#if ParallelProcessing
            });
#else
            }
#endif
        }

        private static KeyValuePair<int, Stack<PredictionTree>> PushAndAdvance(Dictionary<int, int> advanceCount, KeyValuePair<int, Stack<PredictionTree>> indexAndtoAdvance, Stack<PredictionTree> toAdvance, /*List<PredictionTree> totalStates, */PredictionTree currentElement)
        {
#if ParallelProcessing
            lock (toAdvance)
                //lock (totalStates)
                {
#endif
                    PushIt(toAdvance, /*totalStates, */currentElement);
#if ParallelProcessing
                    lock (advanceCount)
#endif
                        advanceCount[indexAndtoAdvance.Key]++;
#if ParallelProcessing
                }
#endif
            return indexAndtoAdvance;
        }

#if ShowStackReductionProgress
        private static void PrintStackStates(Dictionary<int, Stack<PredictionTree>> predictionTreeStacks, Dictionary<int, int> previousCounts, bool fullPrint)
        {
            const int totalWidth = 40;
            const int width = totalWidth - 2 - 5;
            const int ruleNameWidth = width - 7;
            const int ellipsesWidth = 3;
            if (fullPrint)
            {
                int columnIndex = 0;
                int rowIndex = 0;
                bool inSkipMode = false;
                foreach (var indexStackPair in predictionTreeStacks)
                {
                    if (previousCounts.ContainsKey(indexStackPair.Key) && previousCounts[indexStackPair.Key] == 0)
                        continue;
                    if (inSkipMode)
                        goto assignCount;
                    if ((rowIndex+1) % (Console.WindowHeight) == 0)
                    {
                        columnIndex++;
                        rowIndex = 0;
                        if (((columnIndex + 1) * totalWidth) > Console.WindowWidth)
                        {
                            inSkipMode = true;
                            goto assignCount;
                        }
                    }
                    Console.CursorTop = rowIndex;
                    Console.CursorLeft = columnIndex * totalWidth;
                    string ruleName = null;
                    if (indexStackPair.Value.Count > 0)
                    {
                        ruleName = indexStackPair.Value.Peek().Rule.Name;
                        if (ruleName.Length > ruleNameWidth)
                        {
                            var left = ruleName.Substring(0, (ruleNameWidth-ellipsesWidth) / 2 + (ruleNameWidth-ellipsesWidth) % 2);
                            var right = ruleName.Substring(ruleName.Length - (ruleNameWidth - ellipsesWidth) / 2);
                            ruleName = string.Format("{0}...{1}", left,right);
                        }
                        else if (ruleName.Length < ruleNameWidth)
                            ruleName = ruleName.PadRight(ruleNameWidth);
                        ruleName = string.Format("{0} ({1:000})", ruleName, indexStackPair.Value.Peek().Root.Veins.DFAOriginState.StateValue);
                    }
                    else
                        ruleName = "Stack {0:000} count   ";
                    
                    if (indexStackPair.Value.Count == 0)
                        Console.WriteLine("Stack {0:000} count:  *  ", indexStackPair.Key);
                    else
                        Console.WriteLine("{0}: {1:0000}", ruleName, indexStackPair.Value.Count);
                    rowIndex++;
                assignCount:
                    previousCounts[indexStackPair.Key] = indexStackPair.Value.Count;
                }
                
            }
            else
            {
                Console.CursorTop = 0;
                int columnIndex = 0;
                int rowIndex = 0;
                foreach (var indexStackPair in predictionTreeStacks)
                {
                    if (previousCounts.ContainsKey(indexStackPair.Key) && previousCounts[indexStackPair.Key] == 0 && indexStackPair.Value.Count == 0)
                        continue;
                    int lineLength = ruleNameWidth + 8;
                    if ((rowIndex + 1) % (Console.WindowHeight)== 0)
                    {
                        columnIndex++;
                        rowIndex = 0;
                        if (((columnIndex + 1) * totalWidth) > Console.WindowWidth)
                            break;
                    }
                    //Console.CursorLeft = lineLength;
                    if (previousCounts[indexStackPair.Key] != indexStackPair.Value.Count)
                    {
                        Console.CursorTop = rowIndex;
                        Console.CursorLeft = columnIndex * totalWidth + lineLength;
                        previousCounts[indexStackPair.Key] = indexStackPair.Value.Count;
                        if (indexStackPair.Value.Count == 0)
                            Console.WriteLine(" *  ");
                        else
                            Console.WriteLine("{0:0000}", indexStackPair.Value.Count);
                    }
                    rowIndex++;
                }
            }
        }
#endif
        private static void PushIt(Stack<PredictionTree> toAdvance, /*List<PredictionTree> totalStates, */PredictionTree currentElement)
        {
            toAdvance.Push(currentElement);
            //totalStates.Add(currentElement);
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

        public static Dictionary<PredictionTreeLeaf, PredictionTreeDFAdapter> ConstructLookAheadProjections(ParserCompiler compiler, Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> dfaLookup, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleVocabulary, GrammarSymbolSet symbols, ICompilerErrorCollection compilationErrors, ref int maxDFAState)
        {
            Dictionary<int, List<PredictionTreeLeaf>> processorNodes = null;
            int pCount = Math.Min(Environment.ProcessorCount, fullSeries.Count);
            var ruleBreakdown =
                (from f in fullSeries
                 group f.Value by f.Value.Veins.Rule).ToDictionary(k => k.Key, v => v.ToArray());
            processorNodes =
                (from index in 0.RangeTo(ruleBreakdown.Count)
                 let key = ruleBreakdown.Keys.ElementAt(index)
                 let nodeSet = ruleBreakdown[key]
                 from node in nodeSet
                 let pIndex = index % pCount
                 group node by pIndex).ToDictionary(k => k.Key, v => v.ToList());
            var processorAutomationSets =
                (from index in 0.RangeTo(pCount)
                 select new { Dictionary = new Dictionary<PredictionTreeLeaf, SyntacticalDFAState>(), Index = index }).ToDictionary(k => k.Index, v => v.Dictionary);
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
                    var dfa = node.ConstructAdvanceDFA(compiler);//fullSeries, ruleVocabulary, compilationErrors, dfaLookup, symbols);
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
                             orderby kvp.Key.Veins.Rule.Name,
                                     kvp.Key.Veins.DFAOriginState.StateValue
                             select kvp).ToDictionary(k => k.Key, v => v.Value);
            var subResultReverse = subResult.ToDictionary(k => k.Value, v => v.Key);
            var toReplace = new Dictionary<PredictionTreeLeaf, SyntacticalDFAState>();
            foreach (var leftKey in subResult.Keys)
            {
                if (toReplace.ContainsKey(leftKey))
                    continue;
                foreach (var rightKey in subResult.Keys.Except(toReplace.Keys))
                {
                    if (rightKey.RootLeaf != leftKey.RootLeaf)
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
            Dictionary<SyntacticalDFAState, PredictionTreeDFAdapter> currentDict = null;
            var result = (from r in subResult
                          let currentSet = currentDict = new Dictionary<SyntacticalDFAState, PredictionTreeDFAdapter>()
                          select new { Node = r.Key, Adapter = PredictionTreeDFAdapter.Adapt(r.Value, ref currentDict, compiler), Dictionary = currentDict }).ToDictionary(k => k.Node, v => new { Adapter = v.Adapter, Dictionary = v.Dictionary });
            foreach (var rootNode in result.Keys)
            {
                currentDict = result[rootNode].Dictionary;
                var rootAdapter = result[rootNode].Adapter;
                rootAdapter.AssociatedContext.StateAdapterLookup = new ControlledDictionary<SyntacticalDFAState, PredictionTreeDFAdapter>(currentDict);
                foreach (var state in currentDict.Keys)
                {
                    var adapter = currentDict[state];
                    adapter.AssociatedContext.PostConnect(rootNode, result[rootNode].Adapter);
                }
            }
            return result.ToDictionary(k => k.Key, v => v.Value.Adapter);
        }

        public static void AnalyzeDFARepetitions(PredictionTreeDFAContext[] targetContexts, Dictionary<PredictionTreeLeaf, PredictionTreeDFAdapter> lookup)
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
            //                                    where source is PredictionTree
            //                                    select (PredictionTree)source).ToList()
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
            //                 group new { PathSeries = new HashSet<PredictionTree>(sourceQuery), Owner = targetContext } by transitionTarget).ToDictionary(k => k.Key, v => v.ToArray());
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

        private static List<int> GetPathSetSeriesDistinctDepths(PredictionTree[] pathSetSeries)
        {
            return (from pathSet in pathSetSeries
                    let length = GetDPathSetTokenDepth(pathSet)
                    orderby length
                    select length).Distinct().ToList();
        }

        private static int GetDPathSetTokenDepth(PredictionTree set)
        {
            var series = set.GetCurrentPathSets();
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

        private static PredictionTree[] GetSourceSetDPathSets(IEnumerable<IProductionRuleSource> leftSources)
        {
            return (from l in leftSources
                    where l is PredictionTree
                    select (PredictionTree)l).ToArray();
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
                        //if (right.OutTransitions[leftTransition] != left.OutTransitions[leftTransition])
                        //    return false;
                    }
                    return SourcesAndTransitionsAreEquivalent(leftSources, rightSources) && SourcesAreEquivalent(leftSources, rightSources);
                }
                return false;
            };

        private static bool SourcesAndTransitionsAreEquivalent(List<IProductionRuleSource> leftSources, List<IProductionRuleSource> rightSources)
        {
            return leftSources.All(leftSet =>
            {
                var leftSource = leftSet as PredictionTree;
                if (leftSource == null)
                    return rightSources.Contains(leftSet);
                foreach (var rightSet in rightSources)
                {
                    var rightSource = rightSet as PredictionTree;
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

        //private static bool CheckBucketCompatibility(PredictionTree leftSource, PredictionTree rightSource)
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

        internal static IEnumerable<PredictionTreeLeaf> PerformEdgePredictions(
            ParserCompiler parserCompiler, Dictionary<SyntacticalDFAState, 
            PredictionTreeLeaf> allProjectionNodes, 
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
            var ambiguousNodes = new List<PredictionTreeLeaf>();

            foreach (var rule in possibleAmbiguities.Keys)
            {
                var ruleAmbiguities = possibleAmbiguities[rule];
                foreach (var possibleAmbiguity in ruleAmbiguities)
                    if (possibleAmbiguity.EdgeNode.CalculateTerminalAmbiguities(allProjectionNodes, ruleDFAStates, ruleGrammarLookup, grammarSymbolSet))
                        ambiguousNodes.Add(possibleAmbiguity.EdgeNode);
            }
            //Stack<PredictionTree> toProcess = new Stack<PredictionTree>();
            var processStacks = new MultikeyedDictionary<GrammarVocabulary, PredictionTree, Tuple<Stack<PredictionTree>, List<PredictionTree>>>();

            foreach (var node in ambiguousNodes)
            {

                foreach (var follow in node.FollowAmbiguities)
                {
                    processStacks.TryAdd(follow.InitialPaths.Discriminator, follow.InitialPaths, Tuple.Create(new Stack<PredictionTree>(new[] { follow.InitialPaths }), new List<PredictionTree>()));
                }
            }
            ;
            //toProcess.Push(follow.InitialPaths);
            //var mergeSets = new List<Tuple<PredictionTreeFollow, PredictionTreeFollow>>();
            /*
             * To reinsert the relevant elements, see 
             * 'CalculateTerminalAmbiguities' referenced above.
            foreach (var node in ambiguousNodes)
                foreach (var follow in node.FollowAmbiguities)
                    foreach (var path in follow.InitialPaths)
                        if (ambiguousNodes.Contains(path[0]))
                            foreach (var followAmbiguity in path[0].RootLeaf.GetAllFollowAmbiguities(allLeaves))
                                if (followAmbiguity.InitialPaths.Discriminator.Equals(follow.InitialPaths.Discriminator))
                                    mergeSets.Add(Tuple.Create(follow, followAmbiguity));
            /**/
            /* *
             * ToDo: reinsert situations where all initial paths are edge
             * states because the incoming paths from those paths may
             * themselves be ambiguous against the initial target set of
             * transitions.
             * */
        repeatProcess:
            /* We break down future work into a new multiple key dictionary to ensure largely multi-threaded environments can take advantage of that fact.
             * I found that in not doing so, it would pair off into small sets that would spin around a handfull of threads versus utilizing all it could.*/
            object locker = new object();
            var newProcessStacks = new MultikeyedDictionary<GrammarVocabulary, PredictionTree, Tuple<Stack<PredictionTree>, List<PredictionTree>>>();
#if ParallelProcessing
            Parallel.ForEach(processStacks, ksvp =>
#else
            foreach (var ksvp in processStacks)
#endif
            {
                var kqau = processStacks;
                var toProcessAndProcessed = ksvp.Value;
                var alreadyProcessed = toProcessAndProcessed.Item2;
                var toProcess = toProcessAndProcessed.Item1;
                var nextProcess = new Dictionary<GrammarVocabulary, Tuple<Stack<PredictionTree>, List<PredictionTree>>>();
                while (toProcess.Count > 0)
                {
                    var current = toProcess.Pop();
                    current.Advance(allProjectionNodes, ruleGrammarLookup, compilerErrorCollection, grammarSymbolSet);
                    foreach (var transition in current.LookAhead.Keys.ToArray())
                    {
                        Tuple<Stack<PredictionTree>, List<PredictionTree>> currentNextSet;
                        Stack<PredictionTree> currentNextStack;
                        if (!nextProcess.TryGetValue(transition, out currentNextSet))
                            nextProcess.Add(transition, currentNextSet = Tuple.Create(currentNextStack = new Stack<PredictionTree>(), alreadyProcessed));
                        else
                            currentNextStack = currentNextSet.Item1;
                        var currentTarget = (PredictionTree)current.LookAhead[transition];
                        bool lockTaken = false;
                        Monitor.Enter(alreadyProcessed, ref lockTaken);
                        var currentAlternate = alreadyProcessed.FirstOrDefault(k => k.Equals(currentTarget));
                        if (!alreadyProcessed.Contains(currentTarget))
                        {
                            alreadyProcessed.Add(currentTarget);
                            if (lockTaken)
                                Monitor.Exit(alreadyProcessed);
                            currentNextStack.Push(currentTarget);
                        }
                        else if (currentAlternate != null && !object.ReferenceEquals(currentAlternate, currentTarget))
                        {
                            if (lockTaken)
                                Monitor.Exit(alreadyProcessed);
                            current.ReplaceLookahead(transition, currentAlternate);
                        }
                        else if (lockTaken)
                            Monitor.Exit(alreadyProcessed);
                    }
                }
                foreach (var key in nextProcess.Keys)
                {
                    Tuple<Stack<PredictionTree>, List<PredictionTree>> currentProcess;
                    var nextCurrentProcess = nextProcess[key];
                    if (!newProcessStacks.TryGetValue(key, ksvp.Keys.Key2, out currentProcess))
                        newProcessStacks.Add(key, ksvp.Keys.Key2, nextCurrentProcess);
                    else
                    {
                        if (!object.ReferenceEquals(currentProcess.Item2, nextCurrentProcess.Item2))
                        {
                            bool lockTaken1 = false,
                                 lockTaken2 = false;
                            Monitor.Enter(currentProcess.Item2, ref lockTaken1);
                            Monitor.Enter(nextCurrentProcess.Item2, ref lockTaken2);
                            var limitSet = new HashSet<PredictionTree>(currentProcess.Item2.Concat(nextCurrentProcess.Item2));
                            if (lockTaken2)
                                Monitor.Exit(nextCurrentProcess.Item2);
                            currentProcess.Item2.Clear();
                            currentProcess.Item2.AddRange(limitSet);
                            if (lockTaken1)
                                Monitor.Exit(currentProcess.Item2);
                            Monitor.Enter(nextCurrentProcess.Item2, ref lockTaken2);
                            nextCurrentProcess.Item2.Clear();
                            nextCurrentProcess.Item2.AddRange(limitSet);
                            if (lockTaken2)
                                Monitor.Exit(nextCurrentProcess);
                        }
                        foreach (var element in nextCurrentProcess.Item1)
                            currentProcess.Item1.Push(element);
                    }
                }
            }
#if ParallelProcessing
                /* Auto indentation fix */
             );
#endif
            if (newProcessStacks.Count > 0)
            {
                processStacks = newProcessStacks;
                goto repeatProcess;
            }
            StateLocker slock = new StateLocker() { State = parserCompiler.ParserMaxState };

            //Parallel.ForEach(ambiguousNodes, ambiguousNode =>
            foreach (var ambiguousNode in ambiguousNodes)
            {
                foreach (var follow in ambiguousNode.FollowAmbiguities.ToArray())
                {
                    follow.BuildNFA(parserCompiler);// allLeaves, ruleDFAStates, ruleGrammarLookup, (GrammarSymbolSet)grammarSymbolSet, compilerErrorCollection);
                    follow.DeterminateAutomata();
                    follow.ReduceAutomata(false, PredictionDFAReductionComparer);
                }
            }//);*/
            var toReplace = new Dictionary<PredictionTreeFollow, PredictionTreeFollow>();
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
            ControlledDictionary<GrammarVocabulary, PredictionTree> la, 
            PredictionTree previous,
            PredictionTreeLeaf root,
            Dictionary<SyntacticalDFAState, PredictionTreeLeaf> fullSeries, 
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
                    PredictionTree ambiguityPathSet = PredictionTree.GetPathSet(ambiguityVocabulary, lookaheadPathSets, root, ProductionRuleProjectionType.LookAhead, PredictionDerivedFrom.LookAhead_AmbiguityCast);
                    var distinctNodeArray = (from path in ambiguityPathSet
                                             select path.CurrentNode).Distinct().ToArray();
                    foreach (var node in distinctNodeArray)
                        node.DenoteLexicalAmbiguity(ambiguity);
                    la._Add(ambiguityVocabulary, ambiguityPathSet);
                    
                    ambiguityPathSet.Previous = previous;
                    ambiguityPathSet.CheckReductionState(fullSeries, ruleVocabulary);
                    //ambiguityPathSet.ProcessCommonSymbol(fullSeries, ruleVocabulary);
                    ambiguity.Occurrences++;
                    overlap |= ambiguity.AmbiguityKey;
                }
            }
        }
    }
}
