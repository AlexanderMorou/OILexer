using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IDelegateType"/>
    /// instance implementations of an <see cref="ITypeParent"/>.
    /// </summary>
    public interface IDelegateTypes :
        IDeclaredTypes<IDelegateType, CodeTypeDelegate>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IDelegateTypes"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="partialTarget">The partial of the <see cref="IDeclaredTypes{TItem, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IDelegateTypes"/> implementation instance.</param>
        /// <returns>A new <see cref="IDelegateTypes"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IDelegateTypes GetPartialClone(ITypeParent partialTarget);
        /// <summary>
        /// Creates a new instance of <see cref="IDelegateType"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new <see cref="IDelegateType"/>.</param>
        /// <param name="typeParameters">The type-parameters of the new <see cref="IDelegateType"/>.</param>
        /// <returns>A new instance of <see cref="IDelegateType"/> if successful.</returns>
        IDelegateType AddNew(string name, params TypeConstrainedName[] typeParameters);
    }
}
