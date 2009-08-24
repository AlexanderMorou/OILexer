using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IType"/> that is either
    /// internally or externally located.
    /// </summary>
    public interface IType :
        IEquatable<IType>
    {
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type.
        /// </summary>
        ITypeReference GetTypeReference();
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeParameters">The <see cref="ITypeReferenceCollection"/> that relates to the
        /// type-parameters of the <see cref="IType"/>.</param>
        ITypeReference GetTypeReference(ITypeReferenceCollection typeParameters);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="ITypeReference"/> elements
        /// that relates to the type-parameters of the <see cref="IType"/>.</param>
        ITypeReference GetTypeReference(params ITypeReference[] typeReferences);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="IType"/> elements
        /// that relates to the type-parameters of the <see cref="IType"/>.</param>
        ITypeReference GetTypeReference(params IType[] typeReferences);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="object"/> elements
        /// that relates to the type-parameters of the <see cref="IType"/>.</param>
        ITypeReference GetTypeReference(params object[] typeReferences);
        /// <summary>
        /// Returns/sets whether the <see cref="IType"/> is a generic type.
        /// </summary>
        /// <returns>True, if the <see cref="IType"/> is a generic, false otherwise.</returns>
        /// <remarks>If it is a generic and the value is changed to false, the generic
        /// type-parameters are cleared.</remarks>
        bool IsGeneric { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is a class
        /// </summary>
        bool IsClass { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is an interface.
        /// </summary>
        bool IsInterface { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is a delegate.
        /// </summary>
        bool IsDelegate { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is an enumerator.
        /// </summary>
        bool IsEnumerator { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is a structure.
        /// </summary>
        bool IsStructure { get; }
        /// <summary>
        /// Returns whether the <see cref="IType"/> is in generic form when <see cref="IsGeneric"/> is true.
        /// </summary>
        bool IsGenericTypeDefinition { get; }
        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        string GetTypeName(ICodeTranslationOptions options);
        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="commentStyle">Whether or not the type name is represented in 
        /// comment style with the type-parameters expanded and encased in curly 
        /// braces ('{' and '}').</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        string GetTypeName(ICodeTranslationOptions options, bool commentStyle);
        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="typeParameterValues">The series of <see cref="ITypeReference"/> instance
        /// implementations which relate to the generic-parameters of the <see cref="IType"/>.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        string GetTypeName(ICodeTranslationOptions options, ITypeReference[] typeParameterValues);
        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="commentStyle">Whether or not the type name is represented in 
        /// comment style with the type-parameters expanded and encased in curly 
        /// braces ('{' and '}').</param>
        /// <param name="typeParameterValues">The series of <see cref="ITypeReference"/> instance
        /// implementations which relate to the generic-parameters of the <see cref="IType"/>.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        string GetTypeName(ICodeTranslationOptions options, bool commentStyle, ITypeReference[] typeParameterValues);
    }
}
