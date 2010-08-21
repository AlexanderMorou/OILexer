using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a type reference.
    /// </summary>
    public interface ITypeReference : 
        IEquatable<ITypeReference>//,
        //IFauxable<Type>
    {
        /// <summary>
        /// Returns the <see cref="IType"/> which the <see cref="ITypeReference"/> represents.
        /// </summary> 
        IType TypeInstance { get;set; }

        /// <summary>
        /// Returns the <see cref="CodeTypeReference"/> which links back to the
        /// <see cref="TypeInstance"/>.
        /// </summary>
        /// <returns>A <see cref="CodeTypeReference"/> which links back to the <see cref="TypeInstance"/>.</returns>
        CodeTypeReference GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Returns/sets the array rank of the item.
        /// </summary>
        int ArrayRank { get; set; }

        /// <summary>
        /// Returns/sets the rank of the pointer reference.
        /// </summary>
        int PointerRank { get; set; }

        /// <summary>
        /// Returns/setes whether the <see cref="ITypeReference"/> is a reference to a nullable type.
        /// </summary>
        bool Nullable { get; set; }

        /// <summary>
        /// Returns/sets whether the <see cref="ITypeReference"/> is a by-reference type-reference.
        /// </summary>
        bool ByRef { get; set; }

        /// <summary>
        /// Returns/sets the type of elements in the array.
        /// </summary>
        ITypeReference ArrayElementType { get; set; }
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> list containing data about the TypeParameters for the
        /// <see cref="ITypeReference"/>.
        /// </summary>
        ITypeReferenceCollection TypeParameters { get; }

        /// <summary>
        /// Returns a new <see cref="ITypeReference"/> as an array type reference given the rank provided.
        /// </summary>
        /// <param name="rank">The rank of the array.</param>
        /// <returns>A new <see cref="ITypeReference"/> as an array type reference given the rank provided.</returns>
        ITypeReference MakeArray(int rank);

        /// <summary>
        /// Obtains an expression buider which links to the <see cref="TypeInstance"/>.
        /// </summary>
        /// <param name="isStatic">Determines whether or not the call is static.</param>
        /// <returns>A new <see cref="IMemberParentExpression"/> implementation which 
        /// refers back to <see cref="TypeInstance"/> if <paramref name="isStatic"/>
        /// is true, otherwise it refers to a 'this' reference.</returns>
        IMemberParentExpression GetMemberExpression(bool isStatic);
        /// <summary>
        /// Obtains an expression builder which links name-wise to a standard declaration.
        /// </summary>
        /// <param name="link">The <see cref="IDeclaration"/> which uses the <see cref="ITypeReference"/>.</param>
        /// <returns>A new <see cref="IMemberParentExpression"/> implementation which 
        /// refers back to the <paramref name="link"/>.</returns>
        IMemberParentExpression GetMemberExpression(IDeclaration link);

        ITypeReferenceExpression GetTypeExpression();

        ICreateNewObjectExpression GetNewExpression(IExpressionCollection arguments);
        TypeReferenceResolveOptions ResolutionOptions { get; set; }

        string ToString(ICodeTranslationOptions options);
    }
}
