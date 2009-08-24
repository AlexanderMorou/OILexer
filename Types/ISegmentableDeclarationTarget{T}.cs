using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for a <see cref="IDeclarationTarget"/> definition that can
    /// be segmented into partial instances.
    /// </summary>
    /// <typeparam name="T">The type which can be segmented into multiple instances.</typeparam>
    public interface ISegmentableDeclarationTarget<T> :
        ISegmentableDeclarationTarget
        where T :
            ISegmentableDeclarationTarget<T>
    {
        /// <summary>
        /// Returns the root <typeparamref name="T"/> which is responsible for managing
        /// the data elements of the <see cref="ISegmentableDeclarationTarget{T}"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/> which reflects the root instance of the 
        /// <see cref="ISegmentableDeclarationTarget{T}"/>.</returns>
        new T GetRootDeclaration();
        /// <summary>
        /// Returns the partial elements of the <see cref="ISegmentableDeclarationTarget{TItem}"/>.
        /// </summary>
        new ISegmentableDeclarationTargetPartials<T> Partials { get; }
    }
}
