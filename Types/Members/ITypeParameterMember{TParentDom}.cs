using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properteis and methods for working with a generic non-delegate type's
    /// type-argument.
    /// </summary>
    /// <typeparam name="TParentDom">The parent's yielded <see cref="CodeTypeDeclaration"/>.</typeparam>
    public interface ITypeParameterMember<TParentDom> :
        ITypeParameterMember<CodeTypeParameter, IDeclaredType<TParentDom>>
        where TParentDom :
            CodeTypeDeclaration
    {
    }
}
