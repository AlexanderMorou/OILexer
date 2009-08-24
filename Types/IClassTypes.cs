using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IClassType"/>
    /// instance implementations of an <see cref="ITypeParent"/>.
    /// </summary>
    public interface IClassTypes :
        IDeclaredTypes<IClassType, CodeTypeDeclaration>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IClassTypes"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="partialTarget">The partial of the <see cref="IDeclaredTypes{TItem, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IClassTypes"/> implementation instance.</param>
        /// <returns>A new <see cref="IClassTypes"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IClassTypes GetPartialClone(ITypeParent partialTarget);
        /// <summary>
        /// Creates a new instance of <see cref="IClassType"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new <see cref="IClassType"/>.</param>
        /// <param name="typeParameters">The type-parameters of the new <see cref="IClassType"/>.</param>
        /// <returns>A new instance of <see cref="IClassType"/> if successful.</returns>
        IClassType AddNew(string name, params TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Removes a class from the <see cref="IClassTypes"/> with the instance of the <see cref="IClassType"/>
        /// to remove provided.
        /// </summary>
        /// <param name="class">The class to be removed.</param>
        void Remove(IClassType @class);
    }
}
