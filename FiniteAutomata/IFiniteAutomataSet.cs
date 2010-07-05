using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    /// <summary>
    /// Defines properties and methods for working with a finite set of elements.
    /// </summary>
    public interface IFiniteAutomataSet<TCheck> :
        IEquatable<TCheck>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
    {
        /// <summary>
        /// Returns whether the <see cref="IFiniteAutomataSet{TCheck}"/> is an empty set.
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Creates a union of the current <see cref="IFiniteAutomataSet{TCheck}"/> and the
        /// <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The other <typeparamref name="TCheck"/> to
        /// create a union with.</param>
        /// <returns>A new <typeparamref name="TCheck"/> which contains
        /// the elements of both the current instance and the 
        /// <paramref name="other"/> instance provided.</returns>
        TCheck Union(TCheck other);
        /// <summary>
        /// Creates an intersection of the current <see cref="IFiniteAutomataSet{TCheck}"/>
        /// and the <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to make the 
        /// intersection of.</param>
        /// <returns>A new <typeparamref name="TCheck"/> which represents
        /// the points of overlap between the current <see cref="IFiniteAutomataSet{TCheck}"/>
        /// and the <paramref name="other"/> provided.</returns>
        TCheck Intersect(TCheck other);
        /// <summary>
        /// Returns the complement of the current <see cref="IFiniteAutomataSet{TChecK}"/>.
        /// </summary>
        /// <returns>A new <typeparamref name="TCheck"/> which represents the
        /// complement of the current <see cref="IFiniteAutomataSet{TCheck}"/>.</returns>
        TCheck Complement();
        /// <summary>
        /// Creates a new set which represents the symmetric
        /// difference between the current <see cref="IFiniteAutomataSet{TCheck}"/> 
        /// and the <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to make the
        /// symmetric difference of.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which represents the
        /// elements from both sets that are mutually exclusive between the two.</returns>
        TCheck SymmetricDifference(TCheck other);
        /// <summary>
        /// Creates a new set which represents the relative complement
        /// of the <paramref name="other"/> <typeparamref name="TCheck"/> instance
        /// relative to the current <see cref="IFiniteAutomataSet{TCheck}"/>.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to
        /// use to create the symmetric difference to intersect.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which represents
        /// the elements exclusive to the current 
        /// <see cref="IFiniteAutomataSet{TCheck}"/>.
        /// </returns>
        /// <remarks><para>A&#8898;&#8705;B or A&#8726;B</para><para>
        /// i.e. The intersection of A and the complement of B, or A minus the elements of B.</para></remarks>
        TCheck RelativeComplement(TCheck other);
    }
}
