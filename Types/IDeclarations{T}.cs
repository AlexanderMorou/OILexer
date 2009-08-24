using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a type-strict dictionary
    /// of named <typeparamref name="T"/>s.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IDeclaration"/>s the <see cref="IDeclarations{T}"/> consists of.</typeparam>
    public interface IDeclarations<T> :
        IControlledStateDictionary<string, T>,
        IDisposable,
        ITypeReferenceable
        where T :
            IDeclaration
    {
        event EventHandler<EventDeclarationArgs<T>> ItemAdded;
        event EventHandler<EventDeclarationArgs<T>> ItemRemoved;
        /// <summary>
        /// Returns the <see cref="IDeclarationTarget"/> the <see cref="IDeclarations{T}"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="IDeclarationTarget"/> that contains the <see cref="IDeclarations{T}"/>.
        /// </returns>
        IDeclarationTarget TargetDeclaration { get; }
        /// <summary>
        /// Returns the element at the index provided
        /// </summary>
        /// <param name="index">The index of the member to find.</param>
        /// <returns>The instance of <typeparam name="T"/> at the index provided.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is  beyond the range of the 
        /// <see cref="IDeclarations{T}"/>.
        /// </exception>
        new T this[int index] { get; }
    }
}
