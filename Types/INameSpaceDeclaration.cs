using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a namespace declaration which
    /// can contain types, sub-namespaces and can be segmented across multiple instances.
    /// </summary>
    public interface INameSpaceDeclaration :
        IDeclaration<INameSpaceParent, CodeNamespace>,
        ISegmentableDeclarationTarget<INameSpaceDeclaration>,
        ITypeParent,
        INameSpaceParent
    {
        /// <summary>
        /// Generates the <see cref="CodeNamespace"/> array that represents the <see cref="INameSpaceDeclaration"/> and
        /// its child-namespaces. 
        /// </summary>
        /// <returns>An array of <see cref="CodeNamespace"/> if successful.-null- otherwise.</returns>
        CodeNamespace[] GenerateGroupCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Returns the <see cref="INameSpaceDeclarations"/> dictionary which contains
        /// the <see cref="INameSpaceDeclaration"/> instances that are parented to the current
        /// <see cref="INameSpaceDeclaration"/>.
        /// </summary>
        INameSpaceDeclarations ChildSpaces { get; }
        /// <summary>
        /// Returns the full name of the namespace using concatination and its [potential]
        /// parent.
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// Adds a type to the <see cref="INameSpaceDeclaration"/> in the proper location.
        /// </summary>
        /// <param name="newItem"></param>
        void AddType(IDeclaredType newItem);
        /// <summary>
        /// Returns the partial elements of the <see cref="INameSpaceDeclaration"/>.
        /// </summary>
        new INameSpaceDeclarationPartials Partials { get; }
        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the <see cref="INameSpaceDeclaration"/> 
        /// belongs to.
        /// </summary>
        /// <remarks>Depending on how the <see cref="INameSpaceDeclaration"/> was created or
        /// setup, this -can- be null.</remarks>
        IIntermediateProject Project { get; }
        /// <summary>
        /// Returns the number of types declared nested in the <see cref="INameSpaceDeclaration"/>.
        /// </summary>
        /// <param name="includePartials">Whether to include the members declared inside all instances
        /// of the <see cref="INameSpaceDeclaration"/>.</param>
        /// <returns>A <see cref="System.Int32"/> value representing the number of members on the
        /// current instance or all instances based upon <paramref name="includePartials"/>.</returns>
        int GetTypeCount(bool includePartials);
    }
}
