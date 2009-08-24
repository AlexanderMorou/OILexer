using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IStructType"/>
    /// instance implementations of an <see cref="ITypeParent"/>.
    /// </summary>
    public interface IStructTypes :
        IDeclaredTypes<IStructType, CodeTypeDeclaration>
    {
        new IStructTypes GetPartialClone(ITypeParent partialBase);
    }
}
