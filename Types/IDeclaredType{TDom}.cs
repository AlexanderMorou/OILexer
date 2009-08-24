using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a genericly declared type.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="CodeTypeDeclaration"/> which the
    /// <see cref="IDeclaredType{T}"/> yields.</typeparam>
    public interface IDeclaredType<T> :
        IDeclaration<ITypeParent, T>,
        IDeclaredType
        where T :
            CodeTypeDeclaration
    {
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type.
        /// </summary>
        new IDeclaredTypeReference<T> GetTypeReference();
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the generic
        /// type.
        /// </summary>
        /// <param name="typeParameters">The <see cref="ITypeReferenceCollection"/> that relates to the
        /// type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        new IDeclaredTypeReference<T> GetTypeReference(ITypeReferenceCollection typeParameters);
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="ITypeReference"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        new IDeclaredTypeReference<T> GetTypeReference(params ITypeReference[] typeReferences);
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="IType"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        new IDeclaredTypeReference<T> GetTypeReference(params IType[] typeReferences);
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="object"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        new IDeclaredTypeReference<T> GetTypeReference(params object[] typeReferences);
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="object"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        IDeclaredTypeReference<T> GetTypeReference(ICollection<ITypeReference> typeReferences);
        
    }
}
