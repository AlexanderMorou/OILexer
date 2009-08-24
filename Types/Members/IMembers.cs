using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a declared member.
    /// </summary>
    public interface IMembers :
        IDeclarations,
        ITypeReferenceable
    {
        event EventHandler MembersChanged;

        /// <summary>
        /// Returns the <see cref="IDeclarationTarget"/> the <see cref="IMembers"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="IDeclarationTarget"/> that contains the <see cref="IMembers"/>.
        /// </returns>
        IDeclarationTarget TargetDeclaration { get; }        /// <summary>
        /// Generates a series of <see cref="CodeObject"/>s which correlates to the series
        /// of <see cref="IMember"/> elements in the <see cref="IMembers"/> dictionary.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A series of <see cref="CodeObject"/>s pertinent to the <see cref="IMembers"/>
        /// entries.</returns>
        CodeObject[] GenerateCodeDom(ICodeDOMTranslationOptions options);
        /// <summary>
        /// Creates a new instance of the <see cref="IMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="TargetDeclaration"/> which
        /// needs a <see cref="IMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        IMembers GetPartialClone(IDeclaredType parent);
        /// <summary>
        /// Returns the number of members in the declarations that target what's provided.
        /// </summary>
        /// <param name="target">The member to check which target.</param>
        /// <returns>An integer containing the number of members that are children of the <paramref name="target"/>.</returns>
        int GetCountForTarget(IDeclarationTarget target);
        new void Remove(string name);
        void Remove(IMember member);
        string GetUniqueName(string baseName);
        void Add(IMember item);
    }
}
