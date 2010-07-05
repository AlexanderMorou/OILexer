using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a state machine
    /// which operates upon characters within a stream.
    /// </summary>
    public interface IRegularLanguageStateMachine
    {
        /// <summary>
        /// Moves to the next position inside the state machine.
        /// </summary>
        /// <param name="c">The character which acts as the key in 
        /// transitioning from one state of the machine to the next.
        /// </param>
        /// <returns>true if the state machine has a transition in 
        /// its current state for the provided <paramref name="c"/>; 
        /// false, otherwise.</returns>
        bool Next(char c);
        /// <summary>
        /// Resets the state machine back to its initial state.
        /// </summary>
        void Reset();
        /// <summary>
        /// Returns whether the <see cref="IRegularLanguageStateMachine"/>
        /// is in a valid end state.
        /// </summary>
        bool InValidEndState { get; }

        /// <summary>
        /// Returns the longest length the state machine has encountered.
        /// </summary>
        int LongestLength { get; }

        void AddEntries(RegularLanguageScanData result);
    }
}
