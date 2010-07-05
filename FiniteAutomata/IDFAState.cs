using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    public interface IDFAState<TCheck, TState, TSourceElement> :
        IFiniteAutomataState<TCheck, TState, TState, TSourceElement>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IDFAState<TCheck, TState, TSourceElement>
        where TSourceElement :
            IFiniteAutomataSource
    {
        /// <summary>
        /// Returns the <see cref="IFiniteAutomataSingleTargetTransitionTable{TCheck, TState}"/>
        /// which denotes the table of single-target transition nodes going away
        /// from the current state.
        /// </summary>
        new IFiniteAutomataSingleTargetTransitionTable<TCheck, TState> OutTransitions { get; }
    }
    public class ReductionEventArgs : 
        EventArgs
    {
        /// <summary>
        /// Returns the <see cref="Int32"/> value representing
        /// the current state count on the root deterministic
        /// state.
        /// </summary>
        public int StateCount { get; private set; }

        public ReductionEventArgs(int stateCount)
        {
            this.StateCount = stateCount;
        }
    }
    public class ReductionStepEventArgs :
        ReductionEventArgs
    {
        public ReductionStepEventArgs(int stateCount, int previousStateCount)
            : base(stateCount)
        {
            this.PreviousStateCount = previousStateCount;
        }

        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the state count
        /// on the previous reduction step.
        /// </summary>
        public int PreviousStateCount { get; private set; }
    }
}
