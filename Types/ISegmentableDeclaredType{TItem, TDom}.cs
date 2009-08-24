using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for a generic <see cref="IDeclaredType{T}"/>
    /// that can be segmented about a series of instances.
    /// </summary>
    /// <typeparam name="TItem">The segmentable <see cref="IDeclaredType{TDom}"/> which.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeTypeDeclaration"/> the
    /// <see cref="ISegmentableDeclaredType"/> yields.</typeparam>
    public interface ISegmentableDeclaredType<TItem, TDom> :
        IDeclaredType<TDom>,
        ISegmentableDeclarationTarget<TItem>,
        ISegmentableDeclaredType
        where TItem :
            ISegmentableDeclaredType<TItem, TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        /// <summary>
        /// Returns the root <typeparamref name="TItem"/> which is responsible for managing
        /// the data elements of the <see cref="ISegmentableDeclaredType{TItem, TDom}"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TItem"/> which reflects the root instance of the 
        /// <see cref="ISegmentableDeclaredType{TItem, TDom}"/>.</returns>
        new TItem GetRootDeclaration();
        /// <summary>
        /// Returns the partial elements of the <see cref="ISegmentableDeclaredType{TItem, TDom}"/>.
        /// </summary>
        new ISegmentableDeclaredTypePartials<TItem, TDom> Partials { get; }
    }
}
