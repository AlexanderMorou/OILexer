using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a generic parameterized type.  That is,
    /// a type which contains type-parameters, and potential conditions on those parameters
    /// as a restriction on the types supplied.
    /// </summary>
    /// <typeparam name="TParentDom">The yielded result of the parent linked through
    /// the <see cref="IParameteredDeclaredType{TParentDom}.TypeParameters"/></typeparam>
    public interface IParameteredDeclaredType<TParentDom> :
        IDeclaredType<TParentDom>,
        IParameteredDeclaredType
        where TParentDom :
            CodeTypeDeclaration
    {
        /// <summary>
        /// Returns the <see cref="ITypeParameterMembers{TItem, TParentDom}"/> which denote
        /// the <see cref="IParameteredDeclaredType{TParentDom}"/>'s type-parameters.
        /// </summary>
        new ITypeParameterMembers<ITypeParameterMember<TParentDom>, TParentDom> TypeParameters { get; }
    }
}
