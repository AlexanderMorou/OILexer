using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A generic type declaration's type-argument.
    /// </summary>
    /// <typeparam name="TParentDom">The parent's yielded <see cref="CodeTypeDeclaration"/>.</typeparam>
    [Serializable]
    public class TypeParameterMember<TParentDom> :
        TypeParameterMember<CodeTypeParameter, IDeclaredType<TParentDom>>,
        ITypeParameterMember<TParentDom>
        where TParentDom :
            CodeTypeDeclaration
    {

        protected internal TypeParameterMember(string name, IDeclaredType<TParentDom> parentTarget)
            : base(name, parentTarget)
        {
        }

    }
}
