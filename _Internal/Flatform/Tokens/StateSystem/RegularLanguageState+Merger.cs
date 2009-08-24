using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    using ScatteredStateLookupTable = Dictionary<RegularLanguageState, List<RegularLanguageBitArray>>;
    using System.Reflection;
    partial class RegularLanguageState
    {
        public static partial class Merger
        {
            private static List<RegularLanguageState> flatlined = new List<RegularLanguageState>();
            private static bool RecognizerTerminalEquivalencyCheck(RegularLanguageState targetState, int targetCompareIndex, List<RegularLanguageState> compare, Dictionary<RegularLanguageState, List<RegularLanguageState>> targetRedundancyLookup)
            {
                /* *
                 * Merge behind.  Basically any edge which has equal incoming transitions
                 * to another, is equal to that edge.  When those are replaced the forward
                 * logic takes control and simplifies the rest.
                 * */
                if (targetState.OutTransitions == null || targetState.OutTransitions.Count == 0)
                {
                    for (int j = targetCompareIndex + 1; j < compare.Count; j++)
                    {
                        RegularLanguageState currentState = compare[j];
                        if (currentState.OutTransitions != null && currentState.OutTransitions.Count > 0)
                            continue;
                        if (currentState.InTransitions == null ||
                            currentState.InTransitions.Count != targetState.InTransitions.Count)
                            continue;
                        /* *
                         * A bit different than the reverse.
                         * Only worry about cases where 
                         * */
                        if (currentState.InTransitions.Checks.All(p => targetState.InTransitions.Checks.Contains(p)))
                        {
                            //Assume success.
                            bool match = true;
                            for (int k = 0; k < currentState.InTransitions.Count; k++)
                            {
                                var kTransition = currentState.InTransitions.ElementAt(k);

                                if (kTransition.Targets.Count != targetState.InTransitions[kTransition.Check].Count())
                                {
                                    match = false;
                                    break;
                                }
                                var qTransition = targetState.InTransitions[kTransition.Check];
                                for (int q = 0; q < kTransition.Targets.Count; q++)
                                {
                                    if (kTransition.Targets.Contains(qTransition.ElementAt(q)))
                                    {
                                        //When the states are the same, do exit.
                                        match = false;
                                        break;
                                    }
                                }
                                if (!match)
                                    break;
                            }
                            if (!match)
                                continue;
                            if (targetState.IsEdge() == currentState.IsEdge() &&
                                currentState != targetState)
                            {
                                if (!targetRedundancyLookup.ContainsKey(targetState))
                                    targetRedundancyLookup.Add(targetState, new List<RegularLanguageState>());
                                if (!targetRedundancyLookup[targetState].Contains(currentState))
                                    targetRedundancyLookup[targetState].Add(currentState);
                            }
                        }
                    }
                    /* *
                     * The primary procedure that reduces the states
                     * utilizes a loop, states that contain no transitions
                     * don't need checked for out-state transition equivalency,
                     * so returning false omits checking the targetState further.
                     * */
                    return true; //A terminal reduction check occurred.
                }
                return false; //Not a terminal reduction.
            }

            private static void StateEquivalencyCheck(RegularLanguageState targetState, int targetCompareIndex, List<RegularLanguageState> compare, Dictionary<RegularLanguageState, List<RegularLanguageState>> targetRedundancyLookup)
            {
                for (int j = targetCompareIndex + 1; j < compare.Count; j++)
                {
                    RegularLanguageState currentState = compare[j];
                    if (currentState.OutTransitions == null)
                        continue;
                    if (currentState.OutTransitions.Count == 0)
                        continue;
                    if (currentState.OutTransitions.Count != targetState.OutTransitions.Count)
                        continue;
                    /* *
                     * Only continue if every transition set in currentState exists in
                     * the target state.  This process occurs after DFA conversion
                     * so equivalency is more likely vs. the NFA setup that might
                     * have partial overlaps that would later transition into the
                     * redundant states being cleaned up here.
                     * */
                    if (currentState.OutTransitions.Checks.All(p => targetState.OutTransitions.Checks.Contains(p)))
                    {
                        //Assume success.
                        bool match = true;
                        for (int k = 0; k < currentState.OutTransitions.Count; k++)
                        {
                            /* *
                             * Should always be '1' on the count, regardless,
                             * post DFA conversion but again, err on the side 
                             * of caution.
                             * */
                            var kTransition = currentState.OutTransitions.ElementAt(k);
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
                            var qTransition = targetState.OutTransitions.GetNode(kTransition.Check);
                            for (int q = 0; q < kTransition.Targets.Count; q++)
                                if (!kTransition.Targets.Contains(qTransition.Targets[q]))
                                {
                                    match = false;
                                    break;
                                }
                            if (!match)
                                break;
                        }
                        if (!match)
                            continue;
                        /* *
                         * If match is still assumed, only continue if their edge
                         * status is equal, replacing an edge state for a non edge
                         * state would be bad.  One last check for current vs. target
                         * state for good measure.  Purely for final assurance, since
                         * future changes cannot be predicted, and I might blunder
                         * and make a mistake (ie. send the target state that exists
                         * at an index later than the one indicated).
                         * */
                        if (targetState.IsEdge() == currentState.IsEdge() &&
                            currentState != targetState)
                        {
                            if (!targetRedundancyLookup.ContainsKey(targetState))
                                targetRedundancyLookup.Add(targetState, new List<RegularLanguageState>());
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
            /// <param name="original">The <see cref="RegularLanguageState"/> 
            /// to replace.</param>
            private static void ReplaceState(RegularLanguageState replacement, RegularLanguageState original, List<RegularLanguageState> blackListed)
            {
                foreach (var source in original.sources)
                    if (!replacement.sources.Contains(source))
                        replacement.sources.Add(source);
                var currentInStates = original.InTransitions.Distinct().ToArray();
                foreach (var backTransition in currentInStates)
                {
                    //Backup the check, once removed the transition element nullifies it.
                    var check = backTransition.Check;
                    /* *
                     * After being determinized this should only contain a single
                     * state, but for some reason I don't like .Targets.FirstOrDefault() due to its
                     * ugly nature.
                     * */
                    foreach (var backState in backTransition.Targets.ToArray())
                    {
                        bool black = blackListed.Contains(backState);
                        if (black)
                            continue;
                        var originalNode = backState.OutTransitions.GetNode(check);
                        var originalSources = new List<RegularLanguageTransitionNode.SourceElement>(originalNode.Sources);
                        /* *
                         * Handle the source back propagation before merging the
                         * sets, otherwise it'll be redundant, and the dead states
                         * might hold references to other, possibly later, dead states
                         * and the GC won't be able to clean them up.
                         * */
                        original.InTransitions.Remove(check, backState);
                        backState.OutTransitions.Remove(check, original);

                        /* *
                         * When moving to the new state, container variables 
                         * have to be used due to transition table nullifying 
                         * the original node's members.
                         * */
                        backState.MoveTo(check, replacement, originalSources);
                    }
                }
            }

            /// <summary>
            /// Transforms the given <paramref name="target"/> Non-deterministic Finite Automation
            /// state series into a deterministic finite automation by creating new states
            /// from each unique state set and transition pair.
            /// </summary>
            /// <param name="target">The <see cref="RegularLanguageState"/> to determinate.</param>
            /// <param name="recognizer">Whether to reduce the state set into a recognizer (true) or
            /// maintain enough states for a transducer (false).</param>
            /// <returns>A new <see cref="RegularLanguageState"/> which is a deterministic
            /// equivalent of the <paramref name="target"/> provided.</returns>
            public static RegularLanguageState ToDFA(RegularLanguageState target, bool recognizer)
            {
                NFALookupTable lookupTable = new NFALookupTable();
                TransitionTable targetTransitionData = new TransitionTable(true);
                /* *
                 * Push the transition data to the merger data class, it takes care of the
                 * character comparisons and splitting things as needed.
                 * *
                 * Basically converting the multi-state lookups into a single state
                 * for that given permutation of states and input characters.
                 * */
                foreach (var transition in target.OutTransitions)
                    targetTransitionData.Add(transition);
                RegularLanguageState result = new RegularLanguageState();
                if (target.IsEdge())
                    result.MakeEdge();
                ReplicateStateTransitions(result, targetTransitionData, lookupTable);
                /* *
                 * Done, right?
                 * Not quite, next step is state reduction.
                 * When all transitions from one state are identical to another in
                 * transition requirement and the state transitioned to, the states
                 * are equivalent.
                 * *
                 * Additionally when two terminal edges have equal incoming transition 
                 * characters they're equivalent to one another, this with the above rule
                 * performs right-reduction.  This is for recognizer DFAs only.
                 * */
                Reduce(result, recognizer);
                return result;
            }

            private static List<RegularLanguageState> condenseStack = new List<RegularLanguageState>();
            private static void CondenseTransitions(RegularLanguageState target)
            {
                if (condenseStack.Contains(target))
                    return;
                condenseStack.Add(target);

                Dictionary<RegularLanguageState, List<RegularLanguageBitArray>> stateTransitionLookup = new Dictionary<RegularLanguageState, List<RegularLanguageBitArray>>();
                /* *
                 * Enumerate the transitions and create a reverse lookup
                 * for the transitions.
                 * */
                foreach (var transition in target.OutTransitions)
                {
                    var firstTransition = transition.Targets.First();
                    CondenseTransitions(firstTransition);
                    if (!stateTransitionLookup.ContainsKey(firstTransition))
                        stateTransitionLookup.Add(firstTransition, new List<RegularLanguageBitArray>());
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
                    RegularLanguageBitArray fullRange = null;
                    foreach (var transition in condensed.Value)
                    {
                        transitionState.InTransitions.Remove(transition, target);
                        target.OutTransitions.Remove(transition, transitionState);
                        if (fullRange == null)
                            fullRange = transition;
                        else
                            fullRange |= transition;
                    }
                    target.OutTransitions.Add(fullRange, transitionState);
                    transitionState.InTransitions.Add(fullRange, target);
                }
                if (condenseStack[0] == target)
                    condenseStack.Clear();
            }

            private static void Reduce(RegularLanguageState target, bool recognizer)
            {
                var blackListed = new List<RegularLanguageState>();
            Repeat:
                Dictionary<RegularLanguageState, List<RegularLanguageState>> forwardDuplications = new Dictionary<RegularLanguageState, List<RegularLanguageState>>();
                List<RegularLanguageState> flatForm = new List<RegularLanguageState>();
                flatForm.Add(target);
                FlatlineState(target, flatForm);
                flatForm = new List<RegularLanguageState>(flatForm.Distinct());
                for (int i = 0; i < flatForm.Count; i++)
                {
                    RegularLanguageState iItem = flatForm[i];
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
                    if (skip)// || iItem.instates == null)
                        continue;

                    if (recognizer)
                        if (RecognizerTerminalEquivalencyCheck(iItem, i, flatForm, forwardDuplications))
                            continue;
                    //Merge forward.
                    StateEquivalencyCheck(iItem, i, flatForm, forwardDuplications);
                }
                /* *
                 * Replace the states that are considered extraneous.
                 * */
                foreach (var masterState in forwardDuplications.Keys)
                {
                    var currentSet = forwardDuplications[masterState];
                    for (int i = 0; i < currentSet.Count; i++)
                    {
                        ReplaceState(masterState, currentSet[i], blackListed);
                        if (!blackListed.Contains(currentSet[i]))
                            blackListed.Add(currentSet[i]);
                    }
                }
                /* *
                 * If there were reductions performed in this step, repeat
                 * the process, since two states that didn't pass the equivalency
                 * examination pointed to two different, yet similar, states,
                 * the process can be repeated on these, now redundant, states.
                 * */
                CondenseTransitions(target);
                if (forwardDuplications.Count > 0)
                {
                    flatForm.Clear();
                    forwardDuplications.Clear();
                    //Reduces recursive dependency.  No need to call the method again.
                    goto Repeat;
                }
                foreach (var state in flatForm)
                    state.sources = new List<ITokenItem>(state.sources.Distinct());
            }

            internal static void FlatlineState(RegularLanguageState state, List<RegularLanguageState> result)
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

            private static void ReplicateStateTransitions(RegularLanguageState result, TransitionTable targetTransitionData, NFALookupTable dfaLookup)
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
                        TransitionTable transitionData = new TransitionTable(true);
                        foreach (var setState in set)
                        {
                            if (setState.OutTransitions == null)
                                continue;
                            foreach (var setTransition in setState.OutTransitions)
                                transitionData.Add(setTransition);
                            //Copy over the source data from the states.
                            foreach (var source in setState.sources)
                                lookupData.DFAState.sources.Add(source);
                        }
                        result.MoveTo(transition.Check, lookupData.DFAState);
                        ReplicateStateTransitions(lookupData.DFAState, transitionData, dfaLookup);
                    }
                    else
                        result.MoveTo(transition.Check, dfaLookup.GetTransitionFor(set, transition.Check).DFAState);
                }
            }
        }
    }
}
