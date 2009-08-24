using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    /* *
     * In complex problems: code readability and clarity is key.
     * */
    using StateDuplicationTable = Dictionary<SimpleLanguageState, List<SimpleLanguageState>>;
    using ScatteredStateLookupTable = Dictionary<SimpleLanguageState, List<SimpleLanguageBitArray>>;
    partial class SimpleLanguageState
    {
        public static partial class Merger
        {
            /// <summary>
            /// Determines whether two states are equivalent to one another.
            /// </summary>
            /// <param name="targetState">The <see cref="SimpleLanguageState"/>
            /// which represents the target (master) state.</param>
            /// <param name="targetCompareIndex">The index of the <paramref name="targetState"/>.</param>
            /// <param name="compare">The set of states to compare against.</param>
            /// <param name="targetRedundancyLookup">The redundancy dictionary which 
            /// uses the master as a key and the equivalent states as a value.</param>
            /// <remarks>Determinization relegated to <paramref name="targetRedundancyLookup"/>.</remarks>
            private static void StateEquivalencyCheck(SimpleLanguageState targetState, int targetCompareIndex, List<SimpleLanguageState> compare, StateDuplicationTable targetRedundancyLookup)
            {
                for (int j = targetCompareIndex + 1; j < compare.Count; j++)
                {
                    SimpleLanguageState currentState = compare[j];
                    if (currentState == targetState ||
                        currentState.OutTransitions == null)
                        continue;
                    var currentStateTransitionCount = currentState.OutTransitions.Count();
                    if (currentStateTransitionCount == 0 ||
                        currentStateTransitionCount != targetState.OutTransitions.Count())
                        continue;
                    /* *
                     * Only continue if every transition set in currentState exists in
                     * the target state.  This process occurs after DFA conversion
                     * so equivalency is more likely vs. the NFA setup that might
                     * have partial overlaps, that would later transition into the
                     * redundant states being cleaned up here.
                     * */
                    if (currentState.OutTransitions.All(p => targetState.OutTransitions.Contains(p.Check)))
                    {
                        //Assume success.
                        bool match = true;
                        for (int k = 0; k < currentStateTransitionCount; k++)
                        {
                            /* *
                             * Should always be '1' on the count, regardless,
                             * post DFA conversion but again, err on the side 
                             * of caution.
                             * */
                            var kTransition = currentState.OutTransitions.ElementAt(k);
                            var kTransitionTransitionCount = kTransition.Targets.Count;
                            if (kTransition.Targets.Count != targetState.OutTransitions[kTransition.Check].Count())
                            {
                                match = false;
                                break;
                            }
                            /* *
                             * Obtain the transition for the target state,
                             * use the transition key vs. the index
                             * since index equivalency shoulld exist, but 
                             * future changes to the code might alter this
                             * truth.
                             * */
                            var qTransition = targetState.OutTransitions[kTransition.Check];
                            for (int q = 0; q < kTransition.Targets.Count; q++)
                            {
                                var qTarget = qTransition.ElementAt(q);
                                if (!kTransition.Targets.Contains(qTransition.ElementAt(q)))
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (!match)
                                break;
                        }
                        if (!match)
                            continue;
                        /* *
                         * If match is still assumed, only continue if their edge
                         * status is equal, replacing an edge state for a non edge
                         * state would be bad.
                         * */
                        if (targetState.IsEdge() == currentState.IsEdge())
                        {
                            if (!targetRedundancyLookup.ContainsKey(targetState))
                                targetRedundancyLookup.Add(targetState, new List<SimpleLanguageState>());
                            targetRedundancyLookup[targetState].Add(currentState);
                        }
                    }
                }
            }

            /// <summary>
            /// Replaces the <paramref name="original"/> state with the
            /// <paramref name="replacement"/> provided.
            /// </summary>
            /// <param name="replacement">The master state which is noted to be 
            /// equivalent to the <paramref name="original"/> provided.</param>
            /// <param name="original">The <see cref="SimpleLanguageState"/> 
            /// to replace.</param>
            private static void ReplaceState(SimpleLanguageState replacement, SimpleLanguageState original)
            {
                var currentInStates = original.InTransitions.ToArray();
                foreach (var backTransition in currentInStates)
                {
                    //Backup the check, once removed the transition element nullifies it.
                    var check = backTransition.Check;
                    /* *
                     * After being determinized this should only contain a single
                     * state, but for some reason I don't like .Targets.FirstOrDefault() 
                     * because it looks ugly.
                     * */
                    foreach (var backState in backTransition.Targets.ToArray())
                    {
                        /* *
                         * Handle the source back propagation before merging the
                         * sets, otherwise it'll be redundant, and the dead states
                         * might hold references to other, possibly later, dead states
                         * and the GC won't be able to clean them up.
                         * */
                        original.InTransitions.Remove(check, backState);
                        var originalNode = backState.OutTransitions.GetNode(check);
                        var s = new SimpleLanguageTransitionNode.SourceSet(originalNode.Sources);
                        backState.OutTransitions.Remove(check, original);
                        /* *
                         * When moving to the new state, container variables 
                         * have to be used due to transition table nullifying 
                         * the original node's members.
                         * */
                        backState.MoveTo(check, replacement, s);
                    }
                }
            }

            /// <summary>
            /// Converts a NFA <see cref="SimpleLanguageState"/> to a DFA 
            /// <see cref="SimpleLanguageState"/>.
            /// </summary>
            /// <param name="target">The <see cref="SimpleLanguageState"/> to convert to 
            /// DFA form.</param>
            /// <returns>An equivalent <see cref="SimpleLanguageState"/> in DFA form.</returns>
            public static SimpleLanguageState ToDFA(SimpleLanguageState target)
            {
                SimpleLanguageState result = target.GetNewInstance();
                NFALookupTable lookupTable = new NFALookupTable();

                /* *
                 * Push the transition data to the merger data class, it takes care of the
                 * transition comparisons and dividing state sets as needed.
                 * *
                 * Basically converting the multi-state lookups into a single state
                 * for that given permutation of states and transition criteria.
                 * */
                if (target.IsEdge())
                    result.MakeEdge();
                ReplicateStateTransitions(result, target.OutTransitions, lookupTable);
                /* *
                 * Not quite done, next step is state reduction.
                 * When the transitions from one state to another for any given state
                 * equal that of another, the states are equal.
                 * */
                Reduce(result);
                var flatFormFinal = result.GetFlatform();
                /* *
                 * Reference bug-fix.  ToDo: Find the cause of reference bug.
                 * */
                DeleteDeadInStates(result, result, flatFormFinal);
                foreach (var state in flatFormFinal)
                    DeleteDeadInStates(result, state, flatFormFinal);

                /* *
                 * Known to happen, basically two transitions end up being the same
                 * state, but on different transition elements.  This merges
                 * transitions which target the same outgoing state with different
                 * check requirements.
                 * */
                CondenseTransitions(result);
                CondenseInStateTransitions(result);
                foreach (var state in flatFormFinal)
                    CondenseInStateTransitions(state);
                
                return result;
            }

            private static void DeleteDeadInStates(SimpleLanguageState result, SimpleLanguageState state, List<SimpleLanguageState> flatFormFinal)
            {
                /* *
                 * Every state that's in the InTransitions but not in the final 
                 * flat-form of the DFA state is a dead state.
                 * *
                 * ToDo: Find the source of the dead states.
                 * Assumed to be from the reduction phase.
                 * */
                var query = (from t in state.InTransitions.ToArray()
                             let deadQuery = (from target in t.Targets
                                              where (!(flatFormFinal.Contains(target))) && target != result
                                              select target).ToArray()
                             select new { Transition = t, DeadStates = deadQuery.Distinct() }).Distinct().ToArray();
                /* *
                 * Using the anonymous type's selected transition with the 
                 * dead state, remove every occurence of the dead state.
                 * */
                foreach (var deadTransitionElement in query)
                    foreach (var deadTarget in deadTransitionElement.DeadStates)
                        deadTransitionElement.Transition.Targets.Remove(deadTarget);
            }

            private static Stack<SimpleLanguageState> condenseStack = new Stack<SimpleLanguageState>();
            public static void CondenseTransitions(SimpleLanguageState target)
            {
                if (condenseStack.Contains(target))
                    return;
                condenseStack.Push(target);

                ScatteredStateLookupTable stateTransitionLookup = new ScatteredStateLookupTable();
                /* *
                 * Enumerate the transitions and create a reverse lookup
                 * for the transitions.
                 * */
                foreach (var transition in target.OutTransitions)
                {
                    var firstTransition = transition.Targets.First();
                    CondenseTransitions(firstTransition);
                    if (!stateTransitionLookup.ContainsKey(firstTransition))
                        stateTransitionLookup.Add(firstTransition, new List<SimpleLanguageBitArray>());
                    stateTransitionLookup[firstTransition].Add(transition.Check);
                }
                /* *
                 * Next, in cases where the number of hits for a given transition is greater
                 * than one, reduce where needed.
                 * */
                var condenseQuery = from s in stateTransitionLookup
                                    where s.Value.Count > 1
                                    select s;
                foreach (var condensed in condenseQuery)
                {
                    var transitionState = condensed.Key;
                    SimpleLanguageBitArray fullRange = null;
                    List<SimpleLanguageTransitionNode.SourceElement> currentSources = new List<SimpleLanguageTransitionNode.SourceElement>();
                    foreach (var transition in condensed.Value)
                    {
                        transitionState.InTransitions.Remove(transition, target);
                        //Should be 1, force of habit.
                        if (target.OutTransitions[transition].Count() == 1)
                            currentSources.AddRange(target.OutTransitions.GetNode(transition).Sources.ToArray());
                        target.OutTransitions.Remove(transition, transitionState);
                        if (fullRange == null)
                            fullRange = transition;
                        else
                            fullRange |= transition;
                    }
                    target.MoveTo(fullRange, transitionState);
                    //Reissue the sources so they're not lost in never-never land.
                    if (currentSources.Count > 0)
                    {
                        var currentNode = target.OutTransitions.GetNode(fullRange);
                        foreach (var source in currentSources.Distinct())
                            if (!currentNode.Sources.Contains(source))
                                currentNode.Sources.Add(source);
                    }
                }
            }

            private static void CondenseInStateTransitions(SimpleLanguageState target)
            {
                Dictionary<SimpleLanguageTransitionNode, List<SimpleLanguageTransitionNode>> inStateReductionTable = new Dictionary<SimpleLanguageTransitionNode, List<SimpleLanguageTransitionNode>>();
                for (int i = 0; i < target.InTransitions.Count; i++)
                {
                    var iElement = target.InTransitions.ElementAt(i);
                    if (inStateReductionTable.Values.Any(p => p.Contains(iElement)))
                        continue;
                    for (int j = i + 1; j < target.InTransitions.Count; j++)
                    {
                        var jElement = target.InTransitions.ElementAt(j);
                        if (jElement.Targets.Count != iElement.Targets.Count)
                            continue;
                        bool jMatch = true;
                        for (int k = 0; k < iElement.Targets.Count; k++)
                        {
                            var iTarget = iElement.Targets[k];
                            bool iTargetFound = false;
                            for (int l = 0; l < jElement.Targets.Count; l++)
                            {
                                var jTarget = jElement.Targets[l];
                                iTargetFound = jTarget == iTarget;
                                if (iTargetFound)
                                    break;
                            }
                            if (!iTargetFound)
                            {
                                jMatch = false;
                                break;
                            }
                        }
                        if (jMatch)
                        {
                            if (!inStateReductionTable.ContainsKey(iElement))
                                inStateReductionTable.Add(iElement, new List<SimpleLanguageTransitionNode>());
                            inStateReductionTable[iElement].Add(jElement);
                        }
                    }
                }
                foreach (var node in inStateReductionTable.Keys)
                {
                    List<SimpleLanguageTransitionNode.SourceElement> sources = new List<SimpleLanguageTransitionNode.SourceElement>();
                    List<SimpleLanguageState> targets = new List<SimpleLanguageState>();
                    SimpleLanguageBitArray fullInRange = new SimpleLanguageBitArray(node.Check);
                    targets.AddRange(node.Targets);
                    sources.AddRange(node.Sources);
                    target.InTransitions.Remove(node);
                    foreach (var matchingNode in inStateReductionTable[node])
                    {
                        fullInRange |= new SimpleLanguageBitArray(matchingNode.Check);
                        foreach (var source in matchingNode.Sources)
                        {
                            if (!sources.Contains(source))
                                sources.Add(source);
                        }
                        target.InTransitions.Remove(matchingNode);
                    }
                    target.InTransitions.Add(fullInRange, targets, sources);
                }
            }

            /// <summary>
            /// Reduce the DFA states by finding cases of similarity between
            /// the transitions.
            /// </summary>
            /// <param name="target">The DFA <see cref="SimpleLanguageState"/> to reduce.</param>
            private static void Reduce(SimpleLanguageState target)
            {
                //The process is repeated until no further reductions occur
            Repeat:
                StateDuplicationTable forwardDuplications = new StateDuplicationTable();
                List<SimpleLanguageState> flatForm = new List<SimpleLanguageState>();
                flatForm.Add(target);
                FlatlineState(target, flatForm);
                flatForm = new List<SimpleLanguageState>(flatForm.Distinct());
                for (int i = 0; i < flatForm.Count; i++)
                {
                    SimpleLanguageState iItem = flatForm[i];
                    bool skip = false;
                    /* *
                     * If the state is already being replaced,
                     * no need to check it again.
                     * */
                    foreach (var k in forwardDuplications.Values)
                        if (k.Contains(iItem))
                        {
                            skip = true;
                            break;
                        }
                    /* *
                     * Skip elements already set to reduce and cases where 
                     * the instates is null; ie.: the initial state.
                     * */
                    if (skip)
                        continue;

                    //Transitory equivalency check.
                    StateEquivalencyCheck(iItem, i, flatForm, forwardDuplications);
                }
                /* *
                 * Replace the states that are considered extraneous.
                 * */
                foreach (var masterState in forwardDuplications)
                {
                    var currentSet = masterState.Value;
                    for (int i = 0; i < currentSet.Count; i++)
                        ReplaceState(masterState.Key, currentSet[i]);
                }
                /* *
                 * If there were reductions performed in this step, repeat
                 * the process, since two states that didn't pass the equivalency
                 * examination pointed to two different, yet similar, states,
                 * the process can be repeated on these, now redundant, states.
                 * */
                CondenseTransitions(target);
                condenseStack.Clear();
                if (forwardDuplications.Count > 0)
                {
                    flatForm.Clear();
                    forwardDuplications.Clear();
                    //Reduces recursive dependency.  No need to call the method again.
                    goto Repeat;
                }
            }

            private static List<SimpleLanguageState> flatlined = new List<SimpleLanguageState>();
            internal static void FlatlineState(SimpleLanguageState state, List<SimpleLanguageState> result)
            {
                if (state == null)
                    throw new ArgumentNullException("state");
                if (flatlined.Contains(state))
                    return;
                flatlined.Add(state);
                /* *
                 * The state doesn't place itself, but it does insert the transition
                 * states: this ensures the flatline set doesn't contain the initial 
                 * state, it would be bad to replace state 0.
                 * */
                foreach (var transition in state.OutTransitions)
                    foreach (var subState in transition.Targets)
                        if (!result.Contains(subState))
                            result.Add(subState);
                foreach (var transition in state.OutTransitions)
                    foreach (var subState in transition.Targets)
                        if (!flatlined.Contains(subState))
                            FlatlineState(subState, result);
                if (flatlined[0] == state)
                    flatlined.Clear();
            }

            /* *
             * DFA conversion is surprisingly simple.
             * */
            private static void ReplicateStateTransitions(SimpleLanguageState result, TransitionTable targetTransitionData, NFALookupTable dfaLookup)
            {
                foreach (var transition in targetTransitionData)
                {
                    /* *
                     * Remove replicants that got added to the end due to the merge.
                     * Shouldn't cause an issue to remove the excess.
                     * */
                    var set = transition.Targets.ToArray();
                    if (!dfaLookup.ContainsTransitionFor(set, transition.Check))
                    {
                        var lookupData = dfaLookup.CreateTransitionFor(set, transition.Check);
                        TransitionTable transitionData = new TransitionTable();
                        foreach (var setState in set)
                        {
                            if (setState.OutTransitions == null)
                                continue;
                            foreach (var setTransition in setState.OutTransitions)
                                transitionData.Add(setTransition);
                        }
                        result.MoveTo(transition.Check, lookupData.DFAState);
                        result.OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
                        ReplicateStateTransitions(lookupData.DFAState, transitionData, dfaLookup);
                    }
                    else
                        result.MoveTo(transition.Check, dfaLookup.GetTransitionFor(set, transition.Check).DFAState);
                }
                //Copy over the source data from the states.
                foreach (var item in targetTransitionData)
                {
                    var currentNode = result.OutTransitions.GetNode(item.Check);
                    foreach (var target in currentNode.Targets)
                    {
                        /* *
                         * Replicate the source information on the opposing direction
                         * sources.
                         * */
                        var inNode = target.InTransitions.GetNode(item.Check);
                        foreach (var source in item.Sources)
                            if (!inNode.Sources.Contains(source))
                                inNode.Sources.Add(source);
                    }
                    foreach (var source in item.Sources)
                        if (!currentNode.Sources.Contains(source))
                            currentNode.Sources.Add(source);
                }
            }

        }

    }
}
