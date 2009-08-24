
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type-strict dictionary
    /// of named <typeparamref name="TItem"/>s.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="ITypeParameterMember{TParentDom}"/> used
    /// as elements of the <see cref="ITypeParameterMembers{TItem, TParentDom}"/>.</typeparam>
    /// <typeparam name="TParentDom">The <see cref="CodeTypeDeclaration"/> the parent
    /// instances of <typeparamref name="TItem"/> export.</typeparam>
    public interface ITypeParameterMembers<TItem, TParentDom> :
        ITypeParameterMembers<TItem, CodeTypeParameter, IDeclaredType<TParentDom>>
        where TItem :
            ITypeParameterMember<TParentDom>
        where TParentDom :
            CodeTypeDeclaration
    {
        void Clear();

        IDictionary<string, ITypeReference> GetTypeReferenceListing();

        new void Remove(string name);
    }
}
