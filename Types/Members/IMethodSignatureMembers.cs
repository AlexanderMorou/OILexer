using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{ 
    public interface IMethodSignatureMembers :
        IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>
    {
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember"/> implementation into the 
        /// <see cref="IMethodSignatureMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name which the <see cref="IMethodSignatureMember"/> will be referred by in code.</param>
        /// <param name="returnType">The <see cref="ITypeReference"/> which the method
        /// returns.</param>
        /// <returns>A new <see cref="IMethodSignatureMember"/> implementation with the <paramref name="name"/> and
        /// <paramref name="returnType"/> provided.</returns>
        new IMethodSignatureMember AddNew(string name, ITypeReference returnType);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember"/> implementation into the 
        /// <see cref="IMethodSignatureMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the <see cref="IMethodSignatureMember"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        new IMethodSignatureMember AddNew(TypedName nameAndReturn);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember"/> implementation into the 
        /// <see cref="IMethodSignatureMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodSignatureMember"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember"/> implementation with the <paramref name="nameAndReturn"/>
        /// and <paramref name="parameters"/> provided.</returns>
        new IMethodSignatureMember AddNew(TypedName nameAndReturn, params TypedName[] parameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember"/> implementation into the 
        /// <see cref="IMethodSignatureMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodSignatureMember"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember"/> implementation with the <see cref="nameAndReturn"/>
        /// and <paramref name="typeParameters"/> provided.</returns>
        new IMethodSignatureMember AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember"/> implementation into the 
        /// <see cref="IMethodSignatureMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodSignatureMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodSignatureMember"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember"/> implementation with the <see cref="nameAndReturn"/>,
        /// <paramref name="parameters"/>, and <paramref name="typeParameters"/> provided.</returns>
        new IMethodSignatureMember AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Creates a new instance of the <see cref="IMethodSignatureMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IMethodSignatureMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IMethodSignatureMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IMethodSignatureMembers GetPartialClone(ISignatureMemberParentType parent);



        new IMethodSignatureMember FindBySig(string name, params Type[] parameterTypes);
        new IMethodSignatureMember FindBySig(string name, params IType[] parameterTypes);
        new IMethodSignatureMember FindBySig(string name, params ITypeReference[] parameterTypes);
        new IMethodSignatureMember FindBySig(string name, int typeParamCount, params Type[] parameterTypes);
        new IMethodSignatureMember FindBySig(string name, int typeParamCount, params IType[] parameterTypes);
        new IMethodSignatureMember FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes);
    }
}
