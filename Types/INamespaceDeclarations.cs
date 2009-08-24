using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a type-strict dictionary of 
    /// namespaces.
    /// </summary>
    public interface INameSpaceDeclarations :
        IDeclarations<INameSpaceDeclaration>
    {
        /// <summary>
        /// Returns the <see cref="INameSpaceDeclaration"/> the <see cref="INameSpaceDeclarations"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="INameSpaceDeclaration"/> that contains the <see cref="INameSpaceDeclarations"/>.
        /// </returns>
        new INameSpaceParent TargetDeclaration { get; }
        /// <summary>
        /// Creates a new instance of <see cref="INameSpaceDeclaration"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new <see cref="INameSpaceDeclaration"/>.</param>
        /// <returns>A new instance of <see cref="INameSpaceDeclaration"/> if successful.</returns>
        INameSpaceDeclaration AddNew(string name);
        void Add(INameSpaceDeclaration nameSpace);
        CodeNamespace[] GenerateCodeDom(ICodeDOMTranslationOptions options);
        INameSpaceDeclarations GetPartialClone(INameSpaceParent basePartial);
        string GetUniqueName(string baseName);
        /// <summary>
        /// Returns the number of types in the <see cref="INameSpaceDeclarations"/> that 
        /// <paramref name="target"/> what's provided.
        /// </summary>
        /// <param name="target">The namespace parent to check which against the namespaces.</param>
        /// <returns>An integer containing the number of <see cref="INameSpaceDeclaration"/> instances that 
        /// are children of the <paramref name="target"/>.</returns>
        int GetCountForTarget(INameSpaceParent target);
    }
}
