using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.FiniteAutomata
{
    public interface IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> : 
        IFiniteAutomataTransitionTable<TCheck, TState, List<TState>>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        /// <summary>
        /// Removes a state by the given target.
        /// </summary>
        /// <param name="check">The <typeparamref name="TCheck"/>
        /// denoting the condition for the transition.</param>
        /// <param name="target">The <typeparamref name="TState"/>
        /// to remove from the set.</param>
        void Remove(TCheck check, TState target);
    }
}
