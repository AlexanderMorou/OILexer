using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IEnumeratorType"/>
    /// instance implementations of an <see cref="ITypeParent"/>.
    /// </summary>
    public interface IEnumeratorTypes :
        IDeclaredTypes<IEnumeratorType, CodeTypeDeclaration>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IEnumeratorTypes"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="partialTarget">The partial of the <see cref="TargetDeclaration"/> which
        /// needs a <see cref="IEnumeratorTypes"/> implementation instance.</param>
        /// <returns>A new <see cref="IEnumeratorTypes"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IEnumeratorTypes GetPartialClone(ITypeParent partialTarget);

        void Remove(IEnumeratorType @enum);
    }
}
