using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Oilexer.FiniteAutomata.Tokens;
namespace Oilexer.FiniteAutomata
{
    /// <summary>
    /// Provides a basic non-deterministic finite automation.
    /// </summary>
    /// <typeparam name="TCheck">The type of set used
    /// to represent the transition from state set to state set.</typeparam>
    /// <typeparam name="TState">The <typeparamref name="TState"/>
    /// used to represent the non-deterministic elements of the
    /// automation.</typeparam>
    /// <typeparam name="TDFA">The type used to construct
    /// a deterministic model of the current nondeterministic 
    /// automation.</typeparam>
    public partial class NFAState<TCheck, TState, TDFA, TSourceElement> :
        FiniteAutomataState<TCheck, TState, List<TState>, TSourceElement>,
        INFAState<TCheck, TState, TDFA, TSourceElement>,
        IEquatable<TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            NFAState<TCheck, TState, TDFA, TSourceElement>
        where TDFA :
            FiniteAutomataState<TCheck, TDFA, TDFA, TSourceElement>,
            IDFAState<TCheck, TDFA, TSourceElement>,
            new()
        where TSourceElement :
            IFiniteAutomataSource
    {
        private int flatlineIndex = 0;
        private static Dictionary<int, List<TState>> flatlined = new Dictionary<int, List<TState>>();
        private static List<TState> ToStringStack = new List<TState>();
        protected override IFiniteAutomataTransitionTable<TCheck, TState, List<TState>> InitializeOutTransitionTable()
        {
            return new FiniteAutomataMultiTargetTransitionTable<TCheck, TState>();
        }

        /// <summary>
        /// Returns a multi-target transition table for the states
        /// leaving the automation.
        /// </summary>
        public new IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> OutTransitions
        {
            get
            {
                return (IFiniteAutomataMultiTargetTransitionTable<TCheck, TState>)(base.OutTransitions);
            }
        }

        #region INFAState<TCheck,TState,TDFA> Members

        /// <summary>
        /// Creates a version of the current
        /// <see cref="NFAState{TCheck, TState, TDFA}"/> which is 
        /// deterministic by creating a left-side union on elements
        /// which overlap on their <typeparamref name="TCheck"/> 
        /// transition requirements.
        /// </summary>
        /// <returns>A new <typeparamref name="TDFA"/> 
        /// instance which represents the current automation
        /// in a deterministic manner.</returns>
        public TDFA DeterminateAutomata()
        {

            DFAEntryTable entrySet = new DFAEntryTable(this.GetDFAState);
            TDFA result = GetRootDFAState();
            ReplicateSourcesToAlt<TDFA, TDFA>(result);
            ReplicateStateTransitions(result, this.OutTransitions, entrySet);
            result.IsEdge = this.IsEdge;
            return result;
        }

        protected virtual TDFA GetDFAState()
        {
            return new TDFA();
        }

        protected virtual TDFA GetRootDFAState()
        {
            return this.GetDFAState();
        }

        private void ReplicateStateTransitions(TDFA result, IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> table, DFAEntryTable entrySet)
        {
            /* *
             * For every transition on the current transition table,
             * construct a new version of the target state via merging
             * the full transition tables of the NFA states.
             * *
             * This process is repeated with this merged table.
             * */
            foreach (var transition in table.Keys)
            {
                var set = table[transition].Distinct().ToList();
                if (entrySet.ContiansEntryFor(transition, set))
                    result.MoveTo(transition, entrySet[transition][set].DFA);
                else
                {
                    var newElement = entrySet.Add(transition, set);
                    FiniteAutomataMultiTargetTransitionTable<TCheck, TState> mergedTable = new FiniteAutomataMultiTargetTransitionTable<TCheck, TState>();
                    foreach (var subState in set)
                        foreach (var subTransition in subState.OutTransitions.Keys)
                            mergedTable.Add(subTransition, subState.OutTransitions[subTransition]);
                    var curDFA = newElement.DFA;
                    result.MoveTo(transition, curDFA);
                    foreach (var item in set)
                        item.ReplicateSourcesToAlt<TDFA, TDFA>(curDFA);
                    ReplicateStateTransitions(newElement.DFA, mergedTable, entrySet);
                }
            }
        }

        #endregion 

        /// <summary>
        /// Obtaines the edges of the current finite automation
        /// state.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> which iterates
        /// the edge states of the current
        /// <see cref="FiniteAutomataState"/>.</returns>
        public override sealed IEnumerable<TState> ObtainEdges()
        {
            Stack<TState> toCheck = new Stack<TState>();
            toCheck.Push((TState)this);
            List<TState> considered = new List<TState>();
            /* *
             * To avoid cyclic models, track the states passed
             * and yield only the edge states.
             * */
            while (toCheck.Count > 0)
            {
                var current = toCheck.Pop();
                considered.Add(current);
                if (current.IsEdge)
                    yield return current;
                foreach (var transition in current.OutTransitions.Values)
                    foreach (var state in transition)
                        if (!considered.Contains(state))
                            toCheck.Push(state);
            }
            yield break;
        }


        #region INFAState<TCheck,TState,TDFA> Members

        public void Concat(TState target)
        {
            var edges = this.ObtainEdges().ToArray();
            foreach (var transition in target.OutTransitions)
                foreach (var edge in edges)
                {
                    target.ReplicateSources(edge);
                    foreach (var state in transition.Value)
                        edge.MoveTo(transition.Key, state);
                }
            foreach (var edge in edges)
                if (edge.IsEdge ^ target.IsEdge)
                    edge.IsEdge = target.IsEdge;
                //if (target.IsEdge)
                //    edge.IsEdge = true;
                //else
                //    edge.IsEdge = false;
        }

        public virtual void Union(TState target)
        {
            this.IsEdge = target.IsEdge || this.IsEdge;
            foreach (var transition in target.OutTransitions)
                foreach (var state in transition.Value)
                    this.MoveTo(transition.Key, state);
            base.UnifySources(target);
        }

        public void RelativeComplement(TState target)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void MoveTo(TCheck condition, TState target)
        {
            this.OutTransitions.Add(condition, new List<TState>() { target });
            target.MovedInto(condition, (TState)this);
        }

        #region IEquatable<TState> Members

        public bool Equals(TState other) { return object.ReferenceEquals(this, other); }

        #endregion

        internal override int CountStates()
        {
            return CountStates((TState)this, new HashSet<TState>());
        }

        private int CountStates(TState target, HashSet<TState> covered)
        {
            if (covered.Contains(target))
                return 0;
            covered.Add(target);
            int count = 1;
            foreach (var transition in target.OutTransitions.Keys)
                foreach (var transitionTarget in target.OutTransitions[transition])
                    if (!covered.Contains(transitionTarget))
                        count += CountStates(transitionTarget, covered);
            return count;
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
                    foreach (var item in this.OutTransitions)
                    {
                        foreach (var subItem in item.Value)
                        {
                            string current = subItem.ToString();
                            string[] currentLines = current.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            itw.Write("{0}", item.Key);
                            for (int i = 0; i < currentLines.Length; i++)
                            {
                                string line = currentLines[i];
                                if (subItem.IsEdge)
                                    itw.Write("->{{{0}}}", subItem.StateValue);
                                else
                                    itw.Write("->[{0}]", subItem.StateValue);
                                itw.Write(line.TrimStart());
                                itw.WriteLine();
                            }
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

        internal static void FlatlineState(TState state, List<TState> result)
        {
            /* *
             * The state doesn't place itself, but it does insert the transition
             * states: this ensures the flatline set doesn't contain the initial 
             * state, it would be bad to replace state 0.
             * */
            foreach (var subStateSet in state.OutTransitions.Values)
                foreach (var subState in subStateSet)
                    if (!result.Contains(subState))
                    {
                        result.Add(subState);
                        FlatlineState(state, result);
                    }
        }

    }
}
