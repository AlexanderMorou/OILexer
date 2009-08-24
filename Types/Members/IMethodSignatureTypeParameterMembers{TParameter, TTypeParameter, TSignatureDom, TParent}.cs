using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a generic-form method signature's 
    /// type parameters.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameters used.</typeparam>
    /// <typeparam name="TTypeParameter">The type of type-parameters used.</typeparam>
    /// <typeparam name="TParent">The type of parent that contains the signatures.</typeparam>
    public interface IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> :
        ITypeParameterMembers<TTypeParameter, CodeTypeParameter, IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>>
        where TParameter :
            IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TTypeParameter :
            IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
        where TParent :
            IDeclarationTarget
        where TSignatureDom :
            CodeMemberMethod,
            new()
    {
        /// <summary>
        /// Indicates when the signature has changed.
        /// </summary>
        event EventHandler<DeclarationChangeArgs> SignatureChange;
    }
}
