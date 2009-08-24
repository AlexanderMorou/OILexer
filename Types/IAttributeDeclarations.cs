using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IAttributeDeclaration"/> instance
    /// implementations.
    /// </summary>
    public interface IAttributeDeclarations :
        IControlledStateCollection<IAttributeDeclaration>,
        ITypeReferenceable
    {
        /// <summary>
        /// Generates a <see cref="CodeAttributeDeclarationCollection"/> which correlates to the series
        /// of <see cref="IAttributeDeclaration"/> elements in the <see cref="IAttributeDeclarations"/> list.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A <see cref="CodeAttributeDeclarationCollection"/> pertinent to the <see cref="IAttributeDeclarations"/>
        /// entries.</returns>
        CodeAttributeDeclarationCollection GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(ITypeReference attributeType);
        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(IType attributeType);
        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to, the <see cref="System.Type"/> is encapsulated in an implementation
        /// of <see cref="IExternType"/>.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(Type attributeType);
        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> and
        /// <paramref name="parameters"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to.</param>
        /// <param name="parameters">The series of <see cref="IAttributeConstructorParameter"/> which 
        /// will instantiate the <see cref="IAttributeDeclaration"/>.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(ITypeReference attributeType, params IAttributeConstructorParameter[] parameters);
        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> and
        /// <paramref name="parameters"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to.</param>
        /// <param name="parameters">The series of <see cref="IAttributeConstructorParameter"/> which 
        /// will instantiate the <see cref="IAttributeDeclaration"/>.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(IType attributeType, params IAttributeConstructorParameter[] parameters);
        /// <summary>
        /// Adds a new <see cref="IAttributeDeclaration"/> of the <paramref name="attributeType"/> and
        /// <paramref name="parameters"/> provided.
        /// </summary>
        /// <param name="attributeType">The type of <see cref="System.Attribute"/> the new <see cref="IAttributeDeclaration"/> instance
        /// will relate to, the <see cref="System.Type"/> is encapsulated in an implementation
        /// of <see cref="IExternType"/>.</param>
        /// <param name="parameters">The series of <see cref="IAttributeConstructorParameter"/> which 
        /// will instantiate the <see cref="IAttributeDeclaration"/>.</param>
        /// <returns>A new <see cref="IAttributeDeclaration"/> instance implementation which
        /// relates to the <paramref name="attributeType"/> given.</returns>
        IAttributeDeclaration AddNew(Type attributeType, params IAttributeConstructorParameter[] parameters);
        IAttributeDeclaration AddNew(ITypeReference attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters);
        IAttributeDeclaration AddNew(IType attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters);
        IAttributeDeclaration AddNew(Type attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters);
        /// <summary>
        /// Returns whether the <paramref name="attributeType"/> is defined in the <see cref="IAttributeDeclarations"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="ITypeReference"/> to check for the declaration of.</param>
        /// <returns>true if there is an attribute declaration with the type of <paramref name="attributeType"/>.</returns>
        bool IsDefined(ITypeReference attributeType);
        /// <summary>
        /// Returns whether the <paramref name="attributeType"/> is defined in the <see cref="IAttributeDeclarations"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="Type"/> to check for the declaration of.</param>
        /// <returns>true if there is an attribute declaration with the type of <paramref name="attributeType"/>.</returns>
        bool IsDefined(Type attributeType);
        /// <summary>
        /// Returns whether the <paramref name="attributeType"/> is defined in the <see cref="IAttributeDeclarations"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="IType"/> to check for the declaration of.</param>
        /// <returns>true if there is an attribute declaration with the type of <paramref name="attributeType"/>.</returns>
        bool IsDefined(IType attributeType);
    }
}
