/* *
 * Oilexer - Code Dom Layer
 * 
 * Copyright © 2007 Allen Copeland Jr. (henceforth, AUTHOR).
 * 
 * There is no warranty when using this software (SOURCE SOFTWARE), it
 * is under  the basis of an as-is term.  If something happens to 
 * your computer while using it, the AUTHOR shall not be held 
 * accountable.
 * 
 * The AUTHOR grants permission for this to be used in other software 
 * (OTHER SOFTWARE) as long as the following guidelines of use 
 * (USAGE GUIDELINES) are met:
 * 
 * A. Regardless of whether OTHER SOFTWARE is commercial, non-profit,
 * free or otherwise, if any, or part of, SOURCE SOFTWARE is used in 
 * OTHER SOFTWARE no claims shall be made stating that OTHER SOFTWARE
 * is wholly original. 
 * 
 * B. If SOURCE SOFTWARE is altered (thus creating ALTERED SOURCE), OTHER SOFTWARE
 * using ALTERED SOURCE must denote the change accordingly as to
 * not confuse ALTERED SOURCE with SOURCE SOFTWARE.
 * 
 * C. Do not remove this notice, permission to do so is denied.
 * */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of generic-form method signatures.
    /// </summary>
    public interface IMethodSignatureMembers<TParameter, TTypeParameter, TSignatureDom, TParent> :
        IMembers<IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>, TParent, TSignatureDom>
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
        /// Inserts a new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation into the 
        /// <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name which the <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> will be referred by in code.</param>
        /// <param name="returnType">The <see cref="ITypeReference"/> which the method
        /// returns.</param>
        /// <returns>A new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation with the <paramref name="name"/> and
        /// <paramref name="returnType"/> provided.</returns>
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> AddNew(string name, ITypeReference returnType);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation into the 
        /// <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> AddNew(TypedName nameAndReturn);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation into the 
        /// <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation with the <paramref name="nameAndReturn"/>
        /// and <paramref name="parameters"/> provided.</returns>
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> AddNew(TypedName nameAndReturn, params TypedName[] parameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation into the 
        /// <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation with the <see cref="nameAndReturn"/>
        /// and <paramref name="typeParameters"/> provided.</returns>
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation into the 
        /// <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/>.</param>
        /// <returns>A new <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation with the <see cref="nameAndReturn"/>,
        /// <paramref name="parameters"/>, and <paramref name="typeParameters"/> provided.</returns>
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters);


        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, params Type[] parameterTypes);
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, params IType[] parameterTypes);
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, params ITypeReference[] parameterTypes);
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, int typeParamCount, params Type[] parameterTypes);
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, int typeParamCount, params IType[] parameterTypes);
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes);

        /// <summary>
        /// Creates a new instance of the <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation instance.</param>
        /// <returns>A new <see cref="IMethodSignatureMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IMethodSignatureMembers<TParameter, TTypeParameter, TSignatureDom, TParent> GetPartialClone(TParent parent);
    }
}
