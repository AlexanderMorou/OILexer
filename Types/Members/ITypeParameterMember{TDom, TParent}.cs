using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properteis and methods for working with a generic's
    /// type-argument.
    /// </summary>
    /// <typeparam name="TDom">The <see cref="CodeTypeParameter"/> the <see cref="ITypeParameterMember{TDom, TParent}"/> yields.</typeparam>
    /// <typeparam name="TParent">The type of parent of the <see cref="ITypeParameterMember{TDom, TParent}"/>.</typeparam>
    public interface ITypeParameterMember<TDom, TParent> :
        IMember<TParent, TDom>,
        ITypeParameterMember
        where TParent :
            IDeclaration
        where TDom :
            CodeTypeParameter,
            new()
    {
    }
}
