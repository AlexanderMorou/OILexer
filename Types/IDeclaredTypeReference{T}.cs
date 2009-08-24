using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    public interface IDeclaredTypeReference<T> :
        ITypeReference<IDeclaredType<T>>,
        IDeclaredTypeReference
        where T :
            CodeTypeDeclaration
    {
        /// <summary>
        /// Returns the <see cref="IDeclaredType{T}"/> which the <see cref="IDeclaredTypeReference{T}"/> represents.
        /// </summary> 
        new IDeclaredType<T> TypeInstance { get; }
    }
}
