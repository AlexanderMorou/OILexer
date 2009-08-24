using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a working with a type's method members.
    /// </summary>
    public interface IMethodMembers :
        IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>
    {
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name which the <see cref="IMethodMember"/> will be referred by in code.</param>
        /// <param name="returnType">The <see cref="ITypeReference"/> which the method
        /// returns.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <paramref name="name"/> and
        /// <paramref name="returnType"/> provided.</returns>
        new IMethodMember AddNew(string name, ITypeReference returnType);
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the <see cref="IMethodMember"/>.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        new IMethodMember AddNew(TypedName nameAndReturn);
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodMember"/>.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <paramref name="nameAndReturn"/>
        /// and <paramref name="parameters"/> provided.</returns>
        new IMethodMember AddNew(TypedName nameAndReturn, params TypedName[] parameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodMember"/>.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <see cref="nameAndReturn"/>
        /// and <paramref name="typeParameters"/> provided.</returns>
        new IMethodMember AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodMember"/>.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <see cref="nameAndReturn"/>,
        /// <paramref name="parameters"/>, and <paramref name="typeParameters"/> provided.</returns>
        new IMethodMember AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters);

        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodMember"/>.</param>
        /// <param name="privateImplTarget">The type which method implements support for privately.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <paramref name="nameAndReturn"/>
        /// and <paramref name="parameters"/> provided.</returns>
        IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, params TypedName[] parameters);

        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IDeclaration.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodMember"/>.</param>
        /// <param name="privateImplTarget">The type which method implements support for privately.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <see cref="nameAndReturn"/>
        /// and <paramref name="typeParameters"/> provided.</returns>
        IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, params TypeConstrainedName[] typeParameters);
        /// <summary>
        /// Inserts a new <see cref="IMethodMember"/> implementation into the 
        /// <see cref="IMethodMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the new <see cref="IMethodMember"/>.</param>
        /// <param name="parameters">The names and types of the parameters for the new <see cref="IMethodMember"/>.</param>
        /// <param name="typeParameters">The names and constraints of the type-parameters for the new <see cref="IMethodMember"/>.</param>
        /// <param name="privateImplTarget">The type which method implements support for privately.</param>
        /// <returns>A new <see cref="IMethodMember"/> implementation with the <see cref="nameAndReturn"/>,
        /// <paramref name="parameters"/>, and <paramref name="typeParameters"/> provided.</returns>
        IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters);


        /// <summary>
        /// Creates a new instance of the <see cref="IMethodMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IMethodMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IMethodMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IMethodMembers GetPartialClone(IMemberParentType parent);

        new IMethodMember FindBySig(string name, params Type[] parameterTypes);
        new IMethodMember FindBySig(string name, params IType[] parameterTypes);
        new IMethodMember FindBySig(string name, params ITypeReference[] parameterTypes);
        new IMethodMember FindBySig(string name, int typeParamCount, params Type[] parameterTypes);
        new IMethodMember FindBySig(string name, int typeParamCount, params IType[] parameterTypes);
        new IMethodMember FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes);
        /// <summary>
        /// Obtains a free-unattached name to be used for building methods, when name
        /// collisions might occur when the method is in-construction.
        /// </summary>
        /// <param name="baseName">The basic pattern of the name to receive</param>
        /// <returns>A <see cref="System.String"/> which contains an unused name.</returns>
        string GetUnusedName(string baseName);

        new IMethodMember this[string name] { get; }
        new IMethodMember this[int index] { get; }
        new IReadOnlyCollection<IMethodMember> Values { get; }
        new IEnumerator<KeyValuePair<string, IMethodMember>> GetEnumerator();
    }
}
