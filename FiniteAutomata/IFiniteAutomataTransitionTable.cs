using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.FiniteAutomata
{

    public interface IFiniteAutomataTransitionTable<TCheck, TState, TNodeTarget> :
        IControlledStateDictionary<TCheck, TNodeTarget>,
        IFiniteAutomataTransitionTable<TCheck, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        TCheck GetColliders(TCheck condition, out  IDictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TNodeTarget>> colliders);
        new IControlledStateCollection<TCheck> Keys { get; }
        void Add(TCheck check, TNodeTarget target);
        IFiniteAutomataTransitionNode<TCheck, TNodeTarget> GetNode(TCheck key);
    }
    /// <summary>
    /// Defines properties and methods for working with a generalized
    /// transition table for a <see cref="IFiniteAutomataState{TCheck, TState}"/>.
    /// </summary>
    /// <typeparam name="TCheck">The type of set used in the
    /// automation.</typeparam>
    /// <typeparam name="TState">The type of state which contains
    /// the <see cref="IFiniteAutomataTransitionTable{TCheck, TState}"/>.</typeparam>
    public interface IFiniteAutomataTransitionTable<TCheck, TState>
    {
        IControlledStateCollection<TCheck> Keys { get; }
        void AddState(TCheck check, TState target);
        void Remove(TCheck check);
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of
        /// targets contained within the current transition table.
        /// </summary>
        IEnumerable<TState> Targets { get; }
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of 
        /// conditions which are required to transition into the 
        /// <see cref="Targets"/> of the <see cref="IFiniteAutomataTransitionTable{TCheck, TState}"/>.
        /// </summary>
        IEnumerable<TCheck> Checks { get; }
        /// <summary>
        /// Returns the <typeparamref name="TCheck"/> of all of the transition checks
        /// combined into one.
        /// </summary>
        TCheck FullCheck { get; }
    }
}
