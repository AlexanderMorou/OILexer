using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a partials collection of a 
    /// partial-able type.
    /// </summary>
    /// <typeparam name="TItem">The type of members in the partials collection.</typeparam>
    /// <typeparam name="TDom">The <see cref="CodeTypeDeclaration"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    public interface ISegmentableDeclaredTypePartials<TItem, TDom> :
        ISegmentableDeclarationTargetPartials<TItem>
        where TItem :
            ISegmentableDeclaredType<TItem, TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        /// <summary>
        /// Inserts a new partial of the <see cref="ISegmentableDeclarationTargetPartials{TItem}.RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclaredTypePartials{TItem, TDom}"/>, defining the partial
        /// location.  The location must be either the same parent or a partial of the parent
        /// of the initial instance.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="TItem"/> as a partial
        /// of <see cref="ISegmentableDeclarationTargetPartials{TItem}.RootDeclaration"/>.</returns>
        /// <param name="parentTarget">The point at which the instance will be placd.</param>
        TItem AddNew(ITypeParent parentTarget);
    }
}
