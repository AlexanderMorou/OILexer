using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IInterfaceType"/>
    /// instance implementations of an <see cref="ITypeParent"/>.
    /// </summary>
    public interface IInterfaceTypes :
        IDeclaredTypes<IInterfaceType, CodeTypeDeclaration>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IInterfaceTypes"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="partialTarget">The partial of the <see cref="IDeclaredTypes{TItem, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IInterfaceTypes"/> implementation instance.</param>
        /// <returns>A new <see cref="IInterfaceTypes"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IInterfaceTypes GetPartialClone(ITypeParent partialTarget);
        /// <summary>
        /// Creates a new instance of <see cref="IInterfaceType"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new <see cref="IInterfaceType"/>.</param>
        /// <param name="typeParameters">The type-parameters of the new <see cref="IInterfaceType"/>.</param>
        /// <returns>A new instance of <see cref="IInterfaceType"/> if successful.</returns>
        IInterfaceType AddNew(string name, params TypeConstrainedName[] typeParameters);
    }
}
