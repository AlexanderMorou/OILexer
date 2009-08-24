using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.Resources;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a class declaration.
    /// </summary>
    public interface IClassType :
        IDeclaredType<CodeTypeDeclaration>,
        ISegmentableDeclaredType<IClassType, CodeTypeDeclaration>,
        IParameteredParentType<CodeTypeDeclaration>,
        IInterfaceImplementableType
    {
        /// <summary>
        /// Returns/sets whether the <see cref="IClassType"/> and its members are static.
        /// </summary>
        bool IsStatic { get; set; }
        /// <summary>
        /// Returns/sets whether the <see cref="IClassType"/> is abstract, or whether it
        /// can -not- be instanciated.
        /// </summary>
        bool IsAbstract { get; set; }
        /// <summary>
        /// Returns/sets whether the <see cref="IClassType"/> can be derived from.
        /// </summary>
        bool IsSealed { get; set; }
        /// <summary>
        /// Returns/sets the base-type that the <see cref="IClassType"/> derives from.
        /// </summary>
        ITypeReference BaseType { get; set; }
        /// <summary>
        /// Returns the partial instances for the <see cref="IClassType"/>.
        /// </summary>
        new IClassPartials Partials { get; }
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type.
        /// </summary>
        new IDeclaredTypeReference<CodeTypeDeclaration> GetTypeReference();
        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the generic
        /// type.
        /// </summary>
        new IDeclaredTypeReference<CodeTypeDeclaration> GetTypeReference(ITypeReferenceCollection typeParameters);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        new IDeclaredTypeReference<CodeTypeDeclaration> GetTypeReference(params ITypeReference[] typeReferences);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        new IDeclaredTypeReference<CodeTypeDeclaration> GetTypeReference(params IType[] typeReferences);
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        new IDeclaredTypeReference<CodeTypeDeclaration> GetTypeReference(object[] typeReferences);
        
    }
}