using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.ComponentModel;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a type-strict dictionary of 
    /// <typeparamref name="TItem"/> instances which make up all, or part of, 
    /// the members of <typeparamref name="TParent"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IMember{TParent, TDom}"/> contained
    /// within the <see cref="IMembers{TItem, TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="IMember{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    public interface IMembers<TItem, TParent, TDom> :
        IDeclarations<TItem>
        where TItem :
            IMember<TParent, TDom>
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        event EventHandler MembersChanged;
        /// <summary>
        /// Returns the <typeparamref name="TParent"/> the <see cref="IMembers{TItem, TParent, TDom}"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <typeparamref name="TParent"/> that contains the <see cref="IMembers{TItem, TParent, TDom}"/>.
        /// </returns>
        new TParent TargetDeclaration { get; }

        /// <summary>
        /// Generates a series of <typeparamref name="TDom"/>s which correlates to the series
        /// of <typeparamref name="TItem"/> elements in the <see cref="IMembers{TItem, TParent, TDom}"/> dictionary.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A series of <typeparamref name="TDom"/>s pertinent to the <see cref="IMembers{TItem, TParent, TDom}"/>
        /// entries.</returns>
        TDom[] GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Creates a new instance of the <see cref="IMembers{TItem, TParent, TDom}"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="TargetDeclaration"/> which
        /// needs a <see cref="IMembers{TItem, TParent, TDom}"/> implementation instance.</param>
        /// <returns>A new <see cref="IMembers{TItem, TParent, TDom}"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        IMembers<TItem, TParent, TDom> GetPartialClone(TParent parent);
        /// <summary>
        /// Returns the number of members in the declarations that target what's provided.
        /// </summary>
        /// <param name="target">The member to check which target.</param>
        /// <returns>An integer containing the number of members that are children of the <paramref name="target"/>.</returns>
        int GetCountForTarget(IDeclarationTarget target);
        void Remove(string name);
        void Remove(TItem member);
        string GetUniqueName(string baseName);
        void Add(TItem item);
    }
}
