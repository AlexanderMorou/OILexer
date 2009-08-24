using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a declaration.
    /// </summary>
    public interface IDeclaration :
        IDeclarationTarget,
        ITypeReferenceable
    {
        /// <summary>
        /// Returns a unique identifier that relates to the <see cref="IDeclaration"/> relative
        /// to the data components that comprise the <see cref="IDeclaration"/> implementation.
        /// </summary>
        /// <returns>A string representing the uniqueness of the <see cref="IDeclaration"/>.</returns>
        string GetUniqueIdentifier();
        /// <summary>
        /// Returns/sets the accessability of the <see cref="IDeclaration"/>.
        /// </summary>
        DeclarationAccessLevel AccessLevel { get; set; }
        /// <summary>
        /// Indicates the declaration has changed.
        /// </summary>
        event EventHandler<DeclarationChangeArgs> DeclarationChange;

        /// <summary>
        /// Generates the <see cref="CodeObject"/> that represents the <see cref="IDeclaration"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeObject"/> if successful.-null- otherwise.</returns>
        CodeObject GenerateCodeDom(ICodeDOMTranslationOptions options);
    }
}
