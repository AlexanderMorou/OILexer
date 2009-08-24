using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    public interface ISegmentableDeclarationTargetPartials<T> :
        IControlledStateCollection<T>,
        IDisposable
        where T :
            ISegmentableDeclarationTarget<T>
    {
        /// <summary>
        /// Returns the <typeparamref name="T"/> for the current <see cref="ISegmentableDeclarationTarget{T}"/>.
        /// </summary>
        T RootDeclaration { get; }
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclarationTargetPartials{T}"/>
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        T AddNew();
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclarationTargetPartials{T}"/>, defining the partial
        /// location.  The location must be either the same parent or a partial of the parent
        /// of the initial instance.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        /// <param name="parentTarget">The point at which the instance will be placed.</param>
        T AddNew(IDeclarationTarget parentTarget);
        /// <summary>
        /// Removes the given partial.
        /// </summary>
        /// <param name="partial">The <typeparamref name="T"/> to remove.</param>
        void Remove(T partial);
    }
}
