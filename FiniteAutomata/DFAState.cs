using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Oilexer.Parser.GDFileData.TokenExpression;
using System.Threading.Tasks;
using Oilexer.FiniteAutomata.Tokens;
namespace Oilexer.FiniteAutomata
{
    public abstract class DFAState<TCheck, TState, TSourceElement> :
        FiniteAutomataState<TCheck, TState, TState, TSourceElement>,
        IDFAState<TCheck, TState, TSourceElement>
        where TCheck :
            class,
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            DFAState<TCheck, TState, TSourceElement>
        where TSourceElement :
            IFiniteAutomataSource
    {
        private static List<TState> ToStringStack = new List<TState>();

        protected override IFiniteAutomataTransitionTable<TCheck, TState, TState> InitializeOutTransitionTable()
        {
            return new FiniteAutomataSingleTargetTransitionTable<TCheck, TState>();
        }

        public new FiniteAutomataSingleTargetTransitionTable<TCheck, TState> OutTransitions
        {
            get
            {
                return ((FiniteAutomataSingleTargetTransitionTable<TCheck, TState>)base.OutTransitions);
            }
        }

        /// <summary>
        /// Returns the <see cref="IFiniteAutomataSingleTargetTransitionTable{TCheck, TState}"/>
        /// which denotes the table of single-target transition nodes going away
        /// from the current state.
        /// </summary>
        IFiniteAutomataSingleTargetTransitionTable<TCheck, TState> IDFAState<TCheck, TState, TSourceElement>.OutTransitions
        {
            get
            {
                return (IFiniteAutomataSingleTargetTransitionTable<TCheck, TState>)(base.OutTransitions);
            }
        }

        public override IEnumerable<TState> ObtainEdges()
        {
            Stack<TState> toCheck = new Stack<TState>();
            toCheck.Push((TState)this);
            List<TState> considered = new List<TState>();
            while (toCheck.Count > 0)
            {
                var current = toCheck.Pop();
                if (current.IsEdge)
                    yield return current;
                foreach (var state in current.OutTransitions.Values)
                    if (!considered.Contains(state))
                    {
                        considered.Add(state);
                        toCheck.Push(state);
                    }
            }
            yield break;
        }

        public override void MoveTo(TCheck condition, TState target)
        {
            this.OutTransitions.Add(condition, target);
            target.MovedInto(condition, (TState)this);
        }

        private void MoveToInternal(TCheck condition, TState target)
        {
            this.OutTransitions.AddInternal(condition, target);
            target.MovedInto(condition, (TState)this);
        }

        internal override int CountStates()
        {
            return CountStates((TState)this, new HashSet<TState>());
        }

        private int CountStates(TState target, HashSet<TState> covered)
        {
            if (covered.Contains(target))
                return 0;
            else
            {
                covered.Add(target);
                int count = 1;
                foreach (var transition in target.OutTransitions.Keys)
                    count += CountStates(target.OutTransitions[transition], covered);
                return count;
            }
        }

        public override string ToString()
        {
            var thisTState = this as TState;
            bool firstOnStack = ToStringStack.Count == 0;
            if (ToStringStack.Contains(thisTState))
                return string.Format("* ({0})", this.StateValue);
            ToStringStack.Add(thisTState);
            try
            {
                MemoryStream ms = new MemoryStream();
                TextWriter tw = new StreamWriter(ms);
                IndentedTextWriter itw = new IndentedTextWriter(tw);
                itw.Indent++;
                if (this.OutTransitions != null && this.OutTransitions.Count > 0)
                {
                    if (this.IsEdge)
                    {
                        itw.Write("<END>");
                        itw.WriteLine();
                    }
                    foreach (var subItem in this.OutTransitions)
                    {
                        string current = subItem.Value.ToString();
                        string[] currentLines = current.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        itw.Write("{0}", subItem.Key);
                        for (int i = 0; i < currentLines.Length; i++)
                        {
                            string line = currentLines[i];
                            if (subItem.Value.IsEdge)
                                itw.Write("->{{{0}}}", subItem.Value.StateValue);
                            else
                                itw.Write("->[{0}]", subItem.Value.StateValue);
                            itw.Write(line.TrimStart());
                            itw.WriteLine();
                        }
                    }
                }
                else
                {
                    itw.Write("<END>");
                    itw.WriteLine();
                }
                itw.Indent--;
                itw.Flush();
                TextReader sr = new StreamReader(ms);
                ms.Seek(0, SeekOrigin.Begin);
                string result = "    " + sr.ReadToEnd();
                itw.Close();
                tw.Close();
                sr.Close();
                sr.Dispose();
                tw.Dispose();
                ms.Close();
                ms.Dispose();
                itw.Dispose();
                if (firstOnStack)
                    result = string.Format("Regular State Count: {0}\r\n {{Start {1}}}", this.CountStates().ToString(), this.StateValue) + "\r\n" + result;
                return result;
            }
            finally
            {
                if (firstOnStack)
                    ToStringStack.Clear();
            }
        }

        
        protected static void Reduce(TState target, bool recognizer)
        {
            var blackListed = new List<TState>();
            int last = 0;
        Repeat:
            Dictionary<TState, List<TState>> forwardDuplications = new Dictionary<TState, List<TState>>();
            List<TState> flatForm = new List<TState>();
            flatForm.Add(target);
            FlatlineState(target, flatForm);
            flatForm = flatForm.Distinct().ToList();
            bool[] found = new bool[flatForm.Count];
            Parallel.For(0, flatForm.Count, i =>
            {
                lock (found)
                {
                    if (found[i])
                        return;
                }
                TState iItem = flatForm[i];
                /* *
                 * If the state is already being replaced,
                 * no need to check it again.
                 * */

                if (recognizer)
                    /* *
                     * Merge backwards as well as forwards on cases where the 
                     * exact way the match occurred is not important
                     * */
                    if (RecognizerTerminalEquivalencyCheck(iItem, i, flatForm, found, forwardDuplications))
                        return;
                //Merge forward.
                StateEquivalencyCheck(iItem, i, flatForm, found, forwardDuplications);
            });
            //for (int i = 0; i < flatForm.Count; i++)
            //{
            //}
            /* *
             * Replace the states that are considered extraneous.
             * */
            foreach (var masterState in forwardDuplications.Keys)
            {
                var currentSet = forwardDuplications[masterState];
                for (int i = 0; i < currentSet.Count; i++)
                    ReplaceState(masterState, currentSet[i]);
            }
            /* *
             * If there were reductions performed in this step, repeat
             * the process, since two states that didn't pass the equivalency
             * examination pointed to two different, yet similar, states,
             * the process can be repeated on these, now redundant, states.
             * */
            CondenseTransitions(target);
            last = flatForm.Count;
            flatForm.Clear();
            if (forwardDuplications.Count > 0)
            {
                forwardDuplications.Clear();
                //Reduces recursive dependency.  No need to call the method again.
                goto Repeat;
            }
        }

        private static void CondenseTransitions(TState target)
        {
            List<TState> condenseStack = new List<TState>();
            CondenseTransitions(target, condenseStack);
        }

        private static void CondenseTransitions(TState target, List<TState> condenseStack)
        {
            if (condenseStack.Contains(target))
                return;
            condenseStack.Add(target);

            Dictionary<TState, List<TCheck>> stateTransitionLookup = new Dictionary<TState, List<TCheck>>();
            /* *
             * Enumerate the transitions and create a reverse lookup
             * for the transitions.
             * */
            foreach (var transition in target.OutTransitions.Keys)
            {
                var transitionTarget = target.OutTransitions[transition];
                CondenseTransitions(transitionTarget, condenseStack);
                if (!stateTransitionLookup.ContainsKey(transitionTarget))
                    stateTransitionLookup.Add(transitionTarget, new List<TCheck>());
                stateTransitionLookup[transitionTarget].Add(transition);
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
                TCheck fullRange = null;
                foreach (var transition in condensed.Value)
                {
                    transitionState.InTransitions.Remove(transition, target);
                    target.OutTransitions.Remove(transition);//, transitionState);
                    if (fullRange == null)
                        fullRange = transition;
                    else
                        fullRange = fullRange.Union(transition);
                }
                target.MoveToInternal(fullRange, transitionState);
                //target.OutTransitions.Add(fullRange, transitionState);
                //transitionState.InTransitions.Add(fullRange, new List<TState>(new TState[1] { target }));
            }
            if (condenseStack[0] == target)
                condenseStack.Clear();
        }

        private static bool RecognizerTerminalEquivalencyCheck(TState targetState, int targetCompareIndex, List<TState> compare, bool[] found, Dictionary<TState, List<TState>> targetRedundancyLookup)
        {
            /* *
             * Merge behind.  Basically any edge which has equal incoming transitions
             * to another, is equal to that edge.  When those are replaced the forward
             * logic takes control and simplifies the rest.
             * */
            if (targetState.OutTransitions == null || targetState.OutTransitions.Count == 0)
            {
                Parallel.For(targetCompareIndex + 1, compare.Count, j =>
                //for (int j = targetCompareIndex + 1; j < compare.Count; j++)
                {
                    lock (found)
                        if (found[j])
                            return;
                   TState currentState = compare[j];
                    if (currentState.OutTransitions != null && currentState.OutTransitions.Count > 0)
                        return;
                    if (currentState.InTransitions == null ||
                        currentState.InTransitions.Count != targetState.InTransitions.Count)
                        return;
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
                            if (kTransition.Value.Count != targetState.InTransitions[kTransition.Key].Count)
                            {
                                match = false;
                                break;
                            }
                            var qTransition = targetState.InTransitions[kTransition.Key];
                            for (int q = 0; q < kTransition.Value.Count; q++)
                            {
                                if (kTransition.Value.Contains(qTransition.ElementAt(q)))
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
                            return;
                        if (targetState.IsEdge == currentState.IsEdge &&
                            currentState != targetState)
                        {
                            lock (targetRedundancyLookup)
                            {
                                lock (found)
                                    found[j] = true;
                                if (!targetRedundancyLookup.ContainsKey(targetState))
                                    targetRedundancyLookup.Add(targetState, new List<TState>());
                                if (!targetRedundancyLookup[targetState].Contains(currentState))
                                    targetRedundancyLookup[targetState].Add(currentState);
                            }
                        }
                    }
                });
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

        internal static void FlatlineState(TState state, List<TState> result)
        {
            if (state == null)
                throw new ArgumentNullException("state");
            /* *
             * The state doesn't place itself, but it does insert the transition
             * states: this ensures the flatline set doesn't contain the initial 
             * state, it would be bad to replace state 0.
             * */
            foreach (var subState in state.OutTransitions.Values)
                if (!result.Contains(subState))
                {
                    result.Add(subState);
                    FlatlineState(subState, result);
                }
        }


        private static void StateEquivalencyCheck(TState targetState, int targetCompareIndex, List<TState> compare, bool[] found, Dictionary<TState, List<TState>> targetRedundancyLookup)
        {
            Parallel.For(targetCompareIndex + 1, compare.Count, j =>
            //for (int j = targetCompareIndex + 1; j < compare.Count; j++)
            {
                TState currentState = compare[j];
                if (currentState.OutTransitions == null)
                    return;
                if (currentState.OutTransitions.Count == 0)
                    return;
                if (currentState.OutTransitions.Count != targetState.OutTransitions.Count)
                    return;
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
                        /* *
                         * Obtain the transition for the target state,
                         * use the transition key vs. the index
                         * since index equivalency shoulld exist, but 
                         * future changes to the code might alter this
                         * truth.
                         * */
                        var qTransition = targetState.OutTransitions.GetNode(kTransition.Key);
                        if (kTransition.Value != qTransition.Target)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (!match)
                        return;
                    /* *
                     * If match is still assumed, only continue if their edge
                     * status is equal, replacing an edge state for a non edge
                     * state would be bad.  One last check for current vs. target
                     * state for good measure.  Purely for final assurance, since
                     * future changes cannot be predicted, and I might blunder
                     * and make a mistake (ie. send the target state that exists
                     * at an index later than the one indicated).
                     * */
                    if (targetState.IsEdge == currentState.IsEdge &&
                        currentState != targetState)
                    {
                        lock (found)
                            found[j] = true;
                        lock (targetRedundancyLookup)
                        {
                            if (!targetRedundancyLookup.ContainsKey(targetState))
                                targetRedundancyLookup.Add(targetState, new List<TState>());
                            targetRedundancyLookup[targetState].Add(currentState);
                        }
                    }
                }
            });
        }
        /// <summary>
        /// Replaces the <paramref name="original"/> state with the
        /// <paramref name="replacement"/> provided.
        /// </summary>
        /// <param name="replacement">The master state which is noted to be 
        /// equivalent to the <paramref name="original"/> provided.</param>
        /// <param name="original">The <see cref="RegularLanguageState"/> 
        /// to replace.</param>
        private static void ReplaceState(TState replacement, TState original)
        {
            /* *
             * Build a table relative to the states which flow into
             * the original state and use it to retarget those values
             * to the replacement.
             * */
            original.IterateSources((source, kind) =>
                {
                    switch (kind)
                    {
                        case FiniteAutomationSourceKind.Initial:
                            replacement.SetInitial(source);
                            break;
                        case FiniteAutomationSourceKind.Intermediate:
                            replacement.SetIntermediate(source);
                            break;
                        case FiniteAutomationSourceKind.RepeatPoint:
                            replacement.SetRepeat(source);
                            break;
                        case FiniteAutomationSourceKind.Final:
                            replacement.SetFinal(source);
                            break;
                    }
                });
            Dictionary<TCheck, Dictionary<TCheck, HashSet<TState>>> inStates = new Dictionary<TCheck, Dictionary<TCheck, HashSet<TState>>>();
            foreach (var backCheck in original.InTransitions.Keys)
                lock (backCheck)
                    foreach (var backTarget in original.InTransitions[backCheck])
                        foreach (var forwardCheck in backTarget.OutTransitions.Keys)
                            lock (forwardCheck)
                                if (backTarget.OutTransitions[forwardCheck] == original)
                                {
                                    /* *
                                     * Because the number of states going into another state are
                                     * 1:many, versus 1:1 for outgoing states, there's no guarantee
                                     * that the same incoming condition for entering a state is the 
                                     * same as the outgoing condition that's actually used.
                                     * */
                                    Dictionary<TCheck, HashSet<TState>> firstSet;
                                    if (!inStates.TryGetValue(forwardCheck, out firstSet))
                                        inStates.Add(forwardCheck, firstSet = new Dictionary<TCheck, HashSet<TState>>());
                                    HashSet<TState> subSet;
                                    if (!firstSet.TryGetValue(backCheck, out subSet))
                                        firstSet.Add(backCheck, subSet = new HashSet<TState>());
                                    if (!subSet.Contains(backTarget))
                                        subSet.Add(backTarget);
                                }

            foreach (var forwardCheck in inStates.Keys)
            {
                lock (forwardCheck)
                    foreach (var backCheck in inStates[forwardCheck].Keys)
                    {
                        lock (backCheck)
                            foreach (var backState in inStates[forwardCheck][backCheck])
                            {
                                backState.OutTransitions.Remove(forwardCheck);
                                /* *
                                 * Collision checks on the DFA transition table are 
                                 * performed to ensure that if a collision occurs
                                 * an error is thrown to indicate a malformed
                                 * request, since DFA states have one target
                                 * per transition.
                                 * *
                                 * Perform an internal move-to to ensure
                                 * un-necessary set computations aren't performed,
                                 * since the logic above removes the un-needed
                                 * targets from scope.
                                 * */
                                backState.MoveToInternal(forwardCheck, replacement);
                            }
                    }
            }
        }
        public IEnumerable<TSourceElement> SourceSet
        {
            get
            {
                List<TSourceElement> results = new List<TSourceElement>();
                PropagateSources((TState)this, results, new List<TState>(), SourceSetPredicate);
                foreach (var source in results)
                    yield return source;
            }
        }

        protected abstract bool SourceSetPredicate(TSourceElement source);

        public static void PropagateSources(TState target, List<TSourceElement> results, List<TState> observed, Predicate<TSourceElement> limiter)
        {
            if (observed.Contains(target))
                return;
            observed.Add(target);
            foreach (var source in target.Sources)
                if ((!results.Contains(source.Item1)) && limiter(source.Item1))
                    results.Add(source.Item1);
            foreach (var transition in target.OutTransitions)
                if (!(observed.Contains(transition.Value)))
                    PropagateSources(transition.Value, results, observed, limiter);
        }

    }
}
