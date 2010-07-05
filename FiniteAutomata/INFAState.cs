using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    /// <summary>
    /// Defines properties and methods for working with a 
    /// nondeterministic finite state automation which has a
    /// fixed set of transitions from one state to another.
    /// </summary>
    /// <typeparam name="TCheck"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TDFA"></typeparam>
    public interface INFAState<TCheck, TState, TDFA, TSourceElement> :
        IFiniteAutomataState<TCheck, TState, List<TState>, TSourceElement>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            INFAState<TCheck, TState, TDFA, TSourceElement>
        where TDFA :
            IDFAState<TCheck, TDFA, TSourceElement>,
            new()
        where TSourceElement :
            IFiniteAutomataSource
    {
        /// <summary>
        /// Creates a version of the current <see cref="INFAState{TCheck, TState, TDFA}"/>
        /// which is deterministic by creating a left-side union on elements which overlap
        /// on their <typeparamref name="TCheck"/> transition requirements.
        /// </summary>
        /// <returns></returns>
        TDFA DeterminateAutomata();
        new IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> OutTransitions { get; }

        void Concat(TState target);
        void Union(TState target);
        void RelativeComplement(TState target);

    }
}
