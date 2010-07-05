using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    public interface IFiniteAutomataState<TCheck, TState, TForwardNodeTarget, TSourceElement> :
        IFiniteAutomataState<TCheck, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState, TForwardNodeTarget, TSourceElement>
        where TSourceElement :
            IFiniteAutomataSource
    {
        new IFiniteAutomataTransitionTable<TCheck, TState, TForwardNodeTarget> OutTransitions { get; }
    }

    public interface IFiniteAutomataState<TCheck, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        /// <summary>
        /// Returns/sets whether the current <see cref="IFiniteAutomataState{TCheck, TState}"/>
        /// is an edge state.
        /// </summary>
        bool IsEdge { get; set; }
        /// <summary>
        /// Returns whether the current <see cref="IFiniteAutomataState{TCheck, TState}"/> 
        /// is marked as an edge state regardless of the number of
        /// outgoing transitions.
        /// </summary>
        bool IsMarked { get; }
        bool ForcedNoEdge { get; set; }

        IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> InTransitions { get; }
        IFiniteAutomataTransitionTable<TCheck, TState> OutTransitions { get; }
        /// <summary>
        /// Obtains the edges of the current
        /// <see cref="IFiniteAutomataState{TCheck, TState}"/>
        /// which are plausible points to terminate the current state.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance which
        /// yields the edges of the current 
        /// <see cref="IFiniteAutomataState{TCheck, TState}"/>.
        /// </returns>
        IEnumerable<TState> ObtainEdges();
        /// <summary>
        /// Creates a transition from the current 
        /// <see cref="IFiniteAutomataState{TCheck, TState}"/>
        /// to the <paramref name="target"/> with the
        /// <paramref name="condition"/> for transition provided.
        /// </summary>
        /// <param name="condition">The <typeparamref name="TCheck"/>
        /// which restricts the move.</param>
        /// <param name="target">The <typeparamref name="TState"/>
        /// to move into.</param>
        void MoveTo(TCheck condition, TState target);
        /// <summary>
        /// Returns the <see cref="Int32"/> value unique to the current
        /// state.
        /// </summary>
        int StateValue { get; }
    }
}
