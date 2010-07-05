using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    /// <summary>
    /// Defines properties and methods for working with a 
    /// finite automation's transition node.
    /// </summary>
    /// <typeparam name="TCheck">The type of set used in the
    /// automation.</typeparam>
    /// <typeparam name="TTarget">The type of target pointed to
    /// by the <see cref="IFiniteAutomataTransitionNode{TCheck, TTarget}"/>.</typeparam>
    public interface IFiniteAutomataTransitionNode<TCheck, TTarget>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
    {
        /// <summary>
        /// Returns the <typeparamref name="TCheck"/>
        /// value used to define the constraint for transitioning
        /// into the <see cref="Target"/>.
        /// </summary>
        TCheck Check { get; }
        /// <summary>
        /// Returns the <typeparamref name="TTarget"/> 
        /// of the current <see cref="IFiniteAutomataTransitionNode{TCheck, TTarget}"/>.
        /// </summary>
        TTarget Target { get; }
    }
}
