using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer.Comments
{
    /// <summary>
    /// Defines properties and methods for a <see cref="IDocumentationWrapperComment"/> which
    /// wraps a <see cref="IMembers{TItem, TParent, TDom}"/> instance.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IMember{TParent, TDom}"/> contained
    /// within the <see cref="Members{TItem, TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="IMember{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    public interface IMembersWrapperComment<TItem, TParent, TDom> :
        IDocumentationWrapperComment
        where TItem :
            IMember<TParent, TDom>
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        /// <summary>
        /// Returns the members that the <see cref="IMembersWrapperComment{TItem, TParent, TDom}"/>
        /// wraps.
        /// </summary>
        IMembers<TItem, TParent, TDom> WrappedMembers { get; }
    }
}
