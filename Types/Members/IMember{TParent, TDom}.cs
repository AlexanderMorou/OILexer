using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a generic member of an <paramref name="IDeclaration{TParent, TDom}"/>.
    /// </summary>
    /// <typeparam name="TParent">The type of <see cref="IDeclarationTarget"/> the members
    /// belong to.</typeparam>
    /// <typeparam name="TDom">The <see cref="CodeObject"/> the <see cref="IMember{TParent, TDom}"/> yields.</typeparam>
    /// <remarks>Members cannot be type-referenced, ergo the split of concepts into a
    /// members namespace.</remarks>
    public interface IMember<TParent, TDom> :
        IDeclaration<TParent, TDom>,
        IMember
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        /// <summary>
        /// Returns/sets the parent of the current <see cref="IMember{TParent, TDom}"/>.
        /// </summary>
        new TParent ParentTarget { get; set; }
    }
}
